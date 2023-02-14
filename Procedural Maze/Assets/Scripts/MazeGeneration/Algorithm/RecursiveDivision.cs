using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecursiveDivision : MazeGenerationAlgorithm
{
    private Queue<DivisionRegion> regionQueue;
    private List<MazeCell> vertical;
    private List<MazeCell> horizontal;

    protected override void Start()
    {
        regionQueue = new Queue<DivisionRegion>();
        DivisionRegion initialRegion = new DivisionRegion(0, 2 * width, 0, 2 * length);
        vertical = new List<MazeCell>();
        horizontal = new List<MazeCell>();
        regionQueue.Enqueue(initialRegion);
        base.Start();
    }
    protected override void CreateMaze()
    {
        for (int j = 0; j < 2 * length + 1; j++)
        {
            for (int i = 0; i < 2 * width + 1; i++)
            {
                MazeCell mazeCell;
                if (i == 0 || j == 0||i==2*width||j==2*length)
                {
                    mazeCell = new MazeCell(new Vector3(i, 0, j), CellType.Wall);
                    mazeCell.obj = Instantiate(wall, new Vector3(i, 0, j), Quaternion.identity, transform);
                    _maze.maze[j, i] = mazeCell;
                }
                else
                {
                    mazeCell = new MazeCell(new Vector3(i, 0, j), CellType.Floor);
                    mazeCell.obj = Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity, transform);
                    _maze.maze[j, i] = mazeCell;
                }
            }
        }
    }
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------递归划分算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        while (regionQueue.Count > 0)
        {
            var regionTobeDivided = regionQueue.Dequeue();
            //Debug.Log(regionTobeDivided.ToString() + "待划分");
            if (regionTobeDivided.CanBeDividedHorizontally() && regionTobeDivided.CanBeDividedVertically())
            {
                if (regionTobeDivided.colDistance>regionTobeDivided.rowDistance)
                {
                    SplitVertically(regionTobeDivided, regionQueue);
                }
                else if(regionTobeDivided.colDistance < regionTobeDivided.rowDistance)
                {
                    SplitHorizontally(regionTobeDivided, regionQueue);
                }
                else
                {
                    if (Random.value < 0.5f)
                    {
                        SplitHorizontally(regionTobeDivided, regionQueue);
                    }
                    else
                    {
                        SplitVertically(regionTobeDivided, regionQueue);
                    }
                }
            }
            else
            {
                if (regionTobeDivided.CanBeDividedHorizontally())
                {
                    SplitHorizontally(regionTobeDivided, regionQueue);
                }else if (regionTobeDivided.CanBeDividedVertically())
                {
                    SplitVertically(regionTobeDivided, regionQueue);
                }
                else
                {
                    Debug.Log(regionTobeDivided.ToString() + "不能被划分");
                }
            }
            //yield return new WaitForSeconds(0.5f);
            yield return null;
        }
        DeleteDeathEnds();
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
    }

    private void SplitVertically(DivisionRegion regionTobeDivided, Queue<DivisionRegion> regionQueue)
    {
        int dividedLine = Random.Range(regionTobeDivided.ColBorder.Item1+2, regionTobeDivided.ColBorder.Item2-1);
        Debug.Log(regionTobeDivided.ToString() + "垂直划分位置:" + dividedLine);
        CreateWalls(regionTobeDivided,dividedLine,true);
        //Debug.Log(regionTobeDivided.ColBorder.Item1 + "-" + regionTobeDivided.ColBorder.Item2 + "被划分的位置：" + dividedLine);
        DivisionRegion childLeft = new DivisionRegion(regionTobeDivided.ColBorder.Item1, dividedLine, regionTobeDivided.RowBorder.Item1, regionTobeDivided.RowBorder.Item2);
        DivisionRegion childRight = new DivisionRegion(dividedLine, regionTobeDivided.ColBorder.Item2, regionTobeDivided.RowBorder.Item1, regionTobeDivided.RowBorder.Item2);
        regionQueue.Enqueue(childLeft);
        regionQueue.Enqueue(childRight);
    }

    private void SplitHorizontally(DivisionRegion regionTobeDivided, Queue<DivisionRegion> regionQueue)
    {
        int dividedLine = Random.Range(regionTobeDivided.RowBorder.Item1 + 2, regionTobeDivided.RowBorder.Item2-1);
        Debug.Log(regionTobeDivided.ToString() + "水平划分位置:" + dividedLine);
        CreateWalls(regionTobeDivided,dividedLine, false);
        //Debug.Log(regionTobeDivided.RowBorder.Item1 + "-" + regionTobeDivided.RowBorder.Item2 + "被划分的位置：" + dividedLine);
        DivisionRegion childDown = new DivisionRegion(regionTobeDivided.ColBorder.Item1, regionTobeDivided.ColBorder.Item2, regionTobeDivided.RowBorder.Item1, dividedLine);
        DivisionRegion childUp = new DivisionRegion(regionTobeDivided.ColBorder.Item1, regionTobeDivided.ColBorder.Item2, dividedLine, regionTobeDivided.RowBorder.Item2);
        regionQueue.Enqueue(childDown);
        regionQueue.Enqueue(childUp);
    }
    private void DeleteDeathEnds()
    {
        foreach (var cell in vertical)
        {
            ChangeCell(cell, CellType.Floor);
            (int indexRow, int indexCol) = _maze.GetIndexFromCell(cell);
            MazeCell left=null, right=null;
            if (_maze.IsIndexValid(indexRow, indexCol - 1))
            {
                left = _maze.maze[indexRow, indexCol - 1];
            }
            if(_maze.IsIndexValid(indexRow, indexCol + 1))
            {
                right = _maze.maze[indexRow, indexCol + 1];
            }
            if (left != null)
            {
                ChangeCell(left, CellType.Floor);
            }
            if (right != null)
            {
                ChangeCell(right, CellType.Floor);
            }
        }
        foreach (var item in horizontal)
        {
            ChangeCell(item, CellType.Floor);
            (int indexRow, int indexCol) = _maze.GetIndexFromCell(item);
            MazeCell up = null, down = null;
            if (_maze.IsIndexValid(indexRow+1, indexCol))
            {
                up = _maze.maze[indexRow + 1, indexCol];
            }
            if (_maze.IsIndexValid(indexRow-1, indexCol))
            {
                down = _maze.maze[indexRow - 1, indexCol];
            }
            if (up != null)
            {
                ChangeCell(up, CellType.Floor);
            }
            if (down != null)
            {
                ChangeCell(down, CellType.Floor);
            }
        }
    }
    private void CreateWalls(DivisionRegion regionTobeDivided, int dividedLine, bool v)
    {
        if (v)
        {
            int randPos = Random.Range(regionTobeDivided.RowBorder.Item1+1, regionTobeDivided.RowBorder.Item2);
            for (int j = regionTobeDivided.RowBorder.Item1; j < regionTobeDivided.RowBorder.Item2; j++)
            {
                if (j == randPos)
                {
                    vertical.Add(_maze.maze[j, dividedLine]);
                    continue;
                }
                Destroy(_maze.maze[j, dividedLine].obj);
                _maze.maze[j, dividedLine].obj = Instantiate(wall, _maze.maze[j, dividedLine].CellPos, Quaternion.identity, transform);
                _maze.maze[j, dividedLine].ChangeCellType(CellType.Wall);
            }
        }
        else
        {
            int randPos = Random.Range(regionTobeDivided.ColBorder.Item1+1, regionTobeDivided.ColBorder.Item2);
            for (int i = regionTobeDivided.ColBorder.Item1; i < regionTobeDivided.ColBorder.Item2; i++)
            {
                if (i == randPos)
                {
                    horizontal.Add(_maze.maze[dividedLine, i]);
                    continue;
                }
                Destroy(_maze.maze[dividedLine, i].obj);
                _maze.maze[dividedLine, i].obj = Instantiate(wall, _maze.maze[dividedLine, i].CellPos, Quaternion.identity, transform);
                _maze.maze[dividedLine, i].ChangeCellType(CellType.Wall);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    if (vertical.Count > 0)
    //    {
    //        foreach (var item in vertical)
    //        {
    //            Gizmos.DrawLine(item.CellPos, item.CellPos + new Vector3(0, 1f, 0));
    //        }
    //    }
    //    if (horizontal.Count > 0)
    //    {
    //        Gizmos.color = Color.blue;
    //        foreach (var item in horizontal)
    //        {
    //            Gizmos.DrawLine(item.CellPos, item.CellPos + new Vector3(0, 1f, 0));
    //        }
    //    }
        
    //}

    //private void PaintRegion(DivisionRegion region)
    //{
    //    Vector3 leftdown = new Vector3(region.ColBorder.Item1, 0, region.RowBorder.Item1);
    //    Vector3 rightdown = new Vector3(region.ColBorder.Item2, 0, region.RowBorder.Item1);
    //    Vector3 leftup = new Vector3(region.ColBorder.Item1, 0, region.RowBorder.Item2);
    //    Vector3 rightup = new Vector3(region.ColBorder.Item2, 0, region.RowBorder.Item2);
    //    Gizmos.DrawLine(leftdown, rightdown);
    //    Gizmos.DrawLine(leftdown, leftup);
    //    Gizmos.DrawLine(leftup, rightup);
    //    Gizmos.DrawLine(rightup, rightdown);
    //}
}
public class DivisionRegion
{
    public Tuple<int, int> RowBorder;
    public Tuple<int, int> ColBorder;
    public int colDistance, rowDistance;

    public DivisionRegion(int xMin,int xMax,int yMin,int yMax)
    {
        RowBorder = new Tuple<int, int>(yMin, yMax);
        ColBorder = new Tuple<int, int>(xMin, xMax);
        colDistance = ColBorder.Item2 - ColBorder.Item1;
        rowDistance = RowBorder.Item2 - RowBorder.Item1;
    }
    public bool CanBeDividedHorizontally()
    {
        if (colDistance == 1 || rowDistance == 1) return false;
        if (rowDistance <=4)
        {
            return false;
        }
        return true;
    }
    public bool CanBeDividedVertically()
    {
        if (colDistance == 1 || rowDistance == 1) return false;
        if (colDistance <= 4)
        {
            return false;
        }
        return true;
    }
    public override string ToString()
    {
        return "横向边界:" + ColBorder.Item1 + "和" + ColBorder.Item2 + "-纵向边界:" + RowBorder.Item1 + "和" + RowBorder.Item2;
    }
}
