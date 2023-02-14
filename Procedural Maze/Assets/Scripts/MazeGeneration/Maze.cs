using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    public MazeCell[,] maze;
    private int width, length;

    public Maze(int _width,int _length)
    {
        this.width = _width;
        this.length = _length;
        maze = new MazeCell[2 * length + 1, 2 * width + 1];
    }
    public int Width { get => width;}
    public int Length { get => length;}

    public MazeCell GetCellFromIndex(int i,int j)
    {
        if (IsIndexValid(i, j))
        {
            return maze[i, j];
        }
        return null;
    }
    public (int i,int j) GetIndexFromCell(MazeCell cell)
    {
        for (int i = 0; i < 2*width+1; i++)
        {
            for (int j = 0; j < 2*length+1; j++)
            {
                if (maze[j, i] == cell)
                {
                    //Debug.Log(cell.ToString() + "的下标为" + i + "-" + j);
                    return (j, i);
                }
            }
        }
        return (-1, -1);
    }
    public bool IsIndexValid(int i, int j)
    {
        if (i < 0 || i > 2 * length || j < 0 || j > 2 * width)
        {
            //Debug.Log("索引i:" + i + " j:" + j + "不合法");
            return false;
        }
        return true;
    }
    /// <summary>
    /// 获取当前Cell指定方向的邻居
    /// </summary>
    /// <param name="curCell"></param>
    /// <param name="vector"></param>
    /// <returns></returns>
    public MazeCell GetDesignatedDirectionNeighbour(MazeCell curCell,Vector2Int vector)
    {
        if (Direction.direction.Contains(vector) == false)
        {
            Debug.Log("vector参数有误");
        }
        else
        {
            (int indexRow, int indexCol) = GetIndexFromCell(curCell);
            int row = indexRow + vector.x;
            int col = indexCol + vector.y;
            if (IsIndexValid(row, col))
            {
                return maze[row, col];
            }
        }
        return null;
    }
    public MazeCell GetLeftCell(MazeCell curCell)
    {
        (int indexRow, int indexCol) = GetIndexFromCell(curCell);
        int i = indexRow;
        int j = indexCol-2;
        //Debug.Log(x + "--" + y);
        if (IsIndexValid(i, j))
        {
            return maze[i, j];
        }
        Debug.Log(curCell.ToString() + "不存在左邻居");
        return null;
    }
    /// <summary>
    /// 获取周围的邻居(不包括已经访问过的)
    /// </summary>
    /// <param name="curCell"></param>
    /// <returns></returns>
    public List<MazeCell> GetNeighbours(MazeCell curCell) 
    {
        List<Vector2Int> randomDir = MazeHelper.MakeListRandom<Vector2Int>(Direction.direction);
        (int indexX,int indexY) = GetIndexFromCell(curCell);
        //Debug.Log(indexX + "-" + indexY);
        //Debug.Log(curCell.ToString());
        List<MazeCell> neighbours = new List<MazeCell>();
        foreach (Vector2Int dir in randomDir)
        {
            if(GetCellFromIndex(indexX + dir.x, indexY + dir.y) == null)
            {
                continue;
            }
            if (GetCellFromIndex(indexX + dir.x, indexY + dir.y).IsVisited == false)
            {
                neighbours.Add(GetCellFromIndex(indexX + dir.x, indexY + dir.y));
            }
        }
        return neighbours;
    }
    /// <summary>
    /// 获取周围的邻居(包括已经访问过的)
    /// </summary>
    /// <param name="curCell"></param>
    /// <returns></returns>
    public List<MazeCell> GetNeighboursWithVisited(MazeCell curCell)
    {
        //List<Vector2Int> randomDir = MazeHelper.MakeListRandom<Vector2Int>(Direction.direction);
        (int indexX, int indexY) = GetIndexFromCell(curCell);
        List<MazeCell> neighbours = new List<MazeCell>();
        foreach (Vector2Int dir in Direction.direction)
        {
            if (GetCellFromIndex(indexX + dir.x, indexY + dir.y) == null)
            {
                continue;
            }
            neighbours.Add(GetCellFromIndex(indexX + dir.x, indexY + dir.y));
        }
        return neighbours;
    }
    public Vector2Int GetVector(MazeCell curCell,MazeCell nextCell)
    {
        (int indexRow, int indexCol) = GetIndexFromCell(curCell);
        (int indexRow1, int indexCol1) = GetIndexFromCell(nextCell);
        int vert = indexRow1 - indexRow;
        int horizon = indexCol1 - indexCol;
        Vector2Int result = new Vector2Int(vert, horizon);
        Debug.Log(result);
        return result;
    }
}
