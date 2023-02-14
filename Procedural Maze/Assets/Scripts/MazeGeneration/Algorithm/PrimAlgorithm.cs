using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrimAlgorithm : MazeGenerationAlgorithm
{
    private List<MazeCell> mazeCellContainer;

    protected override void Start()
    {
        mazeCellContainer = new List<MazeCell>();
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < width; i++)
            {
                mazeCellContainer.Add(_maze.maze[2 * j + 1, 2 * i + 1]);
            }
        }
        base.Start();
        
        
    }
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------Prim算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        List<MazeCell> willBeVisited = new List<MazeCell>();
        MazeCell firstCell=RandomlySelectTheFirstCell();
        MazeCell curCell = firstCell;
        MazeCell nextCell = null;
        while (mazeCellContainer.Count > 0)
        {
            List<MazeCell> neighbour = _maze.GetNeighbours(curCell);
            MergeList(willBeVisited, neighbour);
            nextCell = willBeVisited[Random.Range(0, willBeVisited.Count)];
            willBeVisited.Remove(nextCell);
            GeneratePathInMazeWithPrim(nextCell);
            nextCell.IsVisited = true;
            mazeCellContainer.Remove(nextCell);
            //cur = Instantiate(current, curCell.CellPos + new Vector3(0, 0.2f, 0), Quaternion.identity);
            //next = Instantiate(nextPrefab, nextCell.CellPos + new Vector3(0, 0.2f, 0), Quaternion.identity);
            curCell = nextCell;
            //yield return new WaitForSeconds(0.1f);
            yield return null;
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
    }

    private void GeneratePathInMazeWithPrim(MazeCell nextCell)
    {
        (int indexRow, int indexCol) = _maze.GetIndexFromCell(nextCell);
        List<Vector2Int> randomList = MazeHelper.MakeListRandom<Vector2Int>(Direction.direction);
        foreach (var dir in randomList)
        {
            int row = indexRow + dir.x;
            int col = indexCol + dir.y;
            if (_maze.IsIndexValid(row, col))
            {
                if (_maze.maze[row, col].IsVisited == true)
                {
                    GeneratePathInMaze(_maze.maze[row, col], nextCell);
                    break;
                }
            }
        }
    }

    private void MergeList(List<MazeCell> willBeVisited, List<MazeCell> neighbour)
    {
        //Debug.Log("合并前列表的元素个数:" + willBeVisited.Count);
        foreach (var item in neighbour)
        {
            if (willBeVisited.Contains(item) == false)
            {
                willBeVisited.Add(item);
            }
        }
        //Debug.Log("合并后列表的元素个数:" + willBeVisited.Count);
    }

    private MazeCell RandomlySelectTheFirstCell()
    {
        if (mazeCellContainer == null || mazeCellContainer.Count <= 0)
        {
            Debug.Log("非法访问mazeCellContainer");
            return null;
        }
        MazeCell thefirstCell = mazeCellContainer[Random.Range(0, width * length)];
        thefirstCell.IsVisited = true;
        mazeCellContainer.Remove(thefirstCell);
        Debug.Log("初始Cell:" + thefirstCell.ToString());
        return thefirstCell;
    }
}
