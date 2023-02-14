using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HuntAndKillAlgorithm : MazeGenerationAlgorithm
{
    private List<MazeCell> mazeCellList;

    protected override void Start()
    {
        mazeCellList = new List<MazeCell>();
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < width; i++)
            {
                mazeCellList.Add(_maze.maze[2 * j + 1,2 * i + 1]);
            }
        }
        base.Start();
    }
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------Hunt-And-Kill算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        MazeCell curCell, nextCell;
        curCell = RandomlyChooseACell();
        while (mazeCellList.Count > 0)
        {
            curCell.IsVisited = true;
            mazeCellList.Remove(curCell);
            var neighbour = _maze.GetNeighbours(curCell);
            if (neighbour.Count == 0)
            {
                curCell = HuntMode();
                continue;
            }
            nextCell = neighbour[Random.Range(0, neighbour.Count)];
            nextCell.IsVisited = true;
            mazeCellList.Remove(nextCell);
            GeneratePathInMaze(curCell, nextCell);
            curCell = nextCell;
            yield return null;
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
    }

    private MazeCell HuntMode()
    {
        Debug.Log("进入Hunt模式");
        foreach (var cell in mazeCellList)
        {
            MazeCell mazeCell=HasNotBeenVisitedWithAVisitedNeighbour(cell);
            if (mazeCell != null)
            {
                GeneratePathInMaze(mazeCell, cell);
                return cell;
            }
        }
        return null;
    }

    private MazeCell HasNotBeenVisitedWithAVisitedNeighbour(MazeCell cell)
    {
        var neighbour = _maze.GetNeighboursWithVisited(cell);
        foreach (var nei in neighbour)
        {
            if (nei.IsVisited == true)
            {
                return nei;
            }
        }
        return null;
    }

    private MazeCell RandomlyChooseACell()
    {
        MazeCell result = mazeCellList[Random.Range(0, mazeCellList.Count)];
        //result.IsVisited = true;
        //mazeCellList.Remove(result);
        return result;
    }
}
