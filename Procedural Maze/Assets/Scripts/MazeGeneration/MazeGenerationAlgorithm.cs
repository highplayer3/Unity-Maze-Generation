using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public abstract class MazeGenerationAlgorithm :MonoBehaviour
{
    [SerializeField]protected int width, length;
    [SerializeField]protected GameObject floor, wall;
    protected Maze _maze;

    protected void Awake()
    {
        InitializeMaze();
    }
    protected virtual void Start()
    {
        StartCoroutine(RunAlgorithm());
    }
    protected virtual void GeneratePathInMaze(MazeCell cellFrom, MazeCell cellTo)
    {
        //Debug.Log(cellFrom);
        //Debug.Log(cellTo);
        if (cellFrom == null || cellFrom.Equals(cellTo)|| cellTo == null)
        {
            Debug.Log("There is no need to generate path");
            return;
        }
        int xFrom, yFrom, xTo, yTo;
        (xFrom, yFrom) = _maze.GetIndexFromCell(cellFrom);
        (xTo, yTo) = _maze.GetIndexFromCell(cellTo);
        //Debug.Log(xFrom + "-" + xTo + "-" + yFrom + "-" + yTo);
        Debug.Log("From:"+cellFrom.ToString());
        Debug.Log("To"+cellTo.ToString());
        if (xFrom == xTo)
        {
            int minY = Mathf.Min(yFrom, yTo);
            int maxY = Mathf.Max(yFrom, yTo);
            //if (maxY - minY != 2) return;
            for (int j = minY; j <= maxY; j++)
            {
                if (_maze.maze[xFrom, j].Cell == CellType.Wall)
                {
                    Debug.Log(_maze.maze[xFrom, j].ToString() + ",need to be remove");
                    //Destroy(_maze.maze[xFrom, j].obj);
                    //_maze.maze[xFrom,j].obj=Instantiate(floor, _maze.maze[xFrom, j].CellPos, Quaternion.identity, transform);
                    ChangeCell(_maze.maze[xFrom, j], CellType.Floor);
                }
            }
        }
        if (yFrom == yTo)
        {
            int minX = Mathf.Min(xFrom, xTo);
            int maxX = Mathf.Max(xFrom, xTo);
            //if (maxX - minX != 2) return;
            for (int i = minX; i <= maxX; i++)
            {
                if (_maze.maze[i, yFrom].Cell == CellType.Wall)
                {
                    Debug.Log(_maze.maze[i,yFrom].ToString() + ",need to be remove");
                    //Destroy(_maze.maze[i,yFrom].obj);
                    //Instantiate(floor, _maze.maze[i,yFrom].CellPos, Quaternion.identity, transform);
                    ChangeCell(_maze.maze[i, yFrom], CellType.Floor);
                }
            }
        }
    }
    public abstract IEnumerator RunAlgorithm();
    private void InitializeMaze()
    {
        _maze = new Maze(width, length);
        CreateMaze();
    }
    protected virtual void CreateMaze()
    {
        for (int j = 0; j < 2 * length + 1; j++)
        {
            for (int i = 0; i < 2 * width + 1; i++)
            {
                MazeCell mazeCell;
                if ((i % 2 != 0) && (j % 2 != 0))
                {
                    mazeCell = new MazeCell(new Vector3(i, 0, j), CellType.Floor);
                    mazeCell.obj = Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity, transform);
                    _maze.maze[j, i] = mazeCell;
                }
                else
                {
                    mazeCell = new MazeCell(new Vector3(i, 0, j), CellType.Wall);
                    mazeCell.obj = Instantiate(wall, new Vector3(i, 0, j), Quaternion.identity, transform);
                    _maze.maze[j, i] = mazeCell;
                }
            }
        }
    }
    protected void ChangeCell(MazeCell mazeCell,CellType cellType)
    {
        if (mazeCell.Cell == cellType)
        {
            return;
        }
        mazeCell.ChangeCellType(cellType);
        Destroy(mazeCell.obj);
        if (mazeCell.Cell == CellType.Wall)
        {
            mazeCell.obj = Instantiate(wall, mazeCell.CellPos, Quaternion.identity, transform);
        }else if (mazeCell.Cell == CellType.Floor)
        {
            mazeCell.obj = Instantiate(floor, mazeCell.CellPos, Quaternion.identity, transform);
        }
        else
        {
            Debug.Log("未知的MazeCell类型");
        }
    }
}


