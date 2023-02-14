using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AldousBorderAlgorithm : MazeGenerationAlgorithm
{
    private int visitedNumber = 0;
    private List<MazeCell> mazeCellsList;
    [SerializeField] private GameObject current;
    private GameObject cur;
    protected override void Start()
    {
        mazeCellsList = new List<MazeCell>();
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < width; i++)
            {
                mazeCellsList.Add(_maze.maze[2 * j + 1, 2 * i + 1]);
            }
        }
        base.Start();
    }
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------Aldous-Border算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        MazeCell curCell=null, nextCell=null;
        curCell = RandomlyChooseACell();
        curCell.IsVisited = true;
        visitedNumber++;
        while (visitedNumber < (width * length) )
        {
            if (cur != null) Destroy(cur);
            var neighbour = _maze.GetNeighboursWithVisited(curCell);
            nextCell = neighbour[Random.Range(0, neighbour.Count)];
            if (nextCell.IsVisited == true)
            {
                //DoNothing
                //Debug.Log(nextCell.ToString() + "已经被访问，无需产生路径");
            }
            else
            {
                GeneratePathInMaze(curCell, nextCell);
                nextCell.IsVisited = true;
                visitedNumber++;
            }
            curCell = nextCell;
            cur = Instantiate(current, curCell.CellPos+new Vector3(0,0.2f,0), Quaternion.identity);
            yield return null;
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");

    }

    private MazeCell RandomlyChooseACell()
    {
        return mazeCellsList[Random.Range(0, mazeCellsList.Count)];
    }
}
