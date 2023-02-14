using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinaryTreeAlgorithm : MazeGenerationAlgorithm
{
    [Tooltip("决定迷宫的走向")]
    public DiagonalSet diagonalSet;
    [Tooltip("控制迷宫的走向，越靠近两端，生成的迷宫越具有方向性")]
    [SerializeField,Range(0f,1f)] private float ChooseTheFirstDirectionProbability = 0.5f;

    private Vector2Int[] directions;
    protected override void Start()
    {
        directions = new Vector2Int[2];
        InitializeDirections();
        base.Start();
    }

    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------二叉树算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        MazeCell curCell, nextCell;
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < width; i++)
            {
                curCell = _maze.maze[2 * j + 1, 2 * i + 1];
                Debug.Log(curCell.ToString());
                if (IsTheDirectionValid(curCell, directions[0]) && IsTheDirectionValid(curCell, directions[1]))
                {
                    Debug.Log("两个方向都符合条件");
                    if (Random.value < ChooseTheFirstDirectionProbability)
                    {
                        Debug.Log("选择的方向:" + directions[0]);
                        nextCell = _maze.GetDesignatedDirectionNeighbour(curCell, directions[0]);
                    }
                    else
                    {
                        Debug.Log("选择的方向:" + directions[1]);
                        nextCell = _maze.GetDesignatedDirectionNeighbour(curCell, directions[1]);
                    }
                }
                else if(IsTheDirectionValid(curCell,directions[0]))
                {
                    Debug.Log("唯一选择的方向:" + directions[0]);
                    nextCell = _maze.GetDesignatedDirectionNeighbour(curCell, directions[0]);
                }
                else if (IsTheDirectionValid(curCell, directions[1]))
                {
                    Debug.Log("唯一选择的方向:" + directions[1]);
                    nextCell = _maze.GetDesignatedDirectionNeighbour(curCell, directions[1]);
                }
                else
                {
                    Debug.Log("两个方向都不符合条件，不做处理");
                    continue;
                }
                GeneratePathInMaze(curCell, nextCell);
                yield return null;
            }
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
    }

    private void InitializeDirections()
    {
        switch (diagonalSet)
        {
            case DiagonalSet.NorthWest:
                directions[0] = new Vector2Int(2, 0);
                directions[1] = new Vector2Int(0, -2);
                break;
            case DiagonalSet.NorthEast:
                directions[0] = new Vector2Int(2, 0);
                directions[1] = new Vector2Int(0, 2);
                break;
            case DiagonalSet.SouthWest:
                directions[0] = new Vector2Int(-2, 0);
                directions[1] = new Vector2Int(0, -2);
                break;
            case DiagonalSet.SouthEast:
                directions[0] = new Vector2Int(-2, 0);
                directions[1] = new Vector2Int(0, 2);
                break;
            default:
                break;
        }
    }
    private bool IsTheDirectionValid(MazeCell curCell, Vector2Int dir)
    {
        (int indexRow, int indexCol) = _maze.GetIndexFromCell(curCell);
        int y = indexRow + dir.x;
        int x = indexCol + dir.y;
        if (_maze.IsIndexValid(y, x))
        {
            return true;
        }
        return false;
    }

    public enum DiagonalSet
    {
        NorthWest,
        NorthEast,
        SouthWest,
        SouthEast
    }
}
