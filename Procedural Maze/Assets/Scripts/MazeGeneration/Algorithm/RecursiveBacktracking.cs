using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecursiveBacktracking : MazeGenerationAlgorithm
{
    [SerializeField]private GameObject current;
    private GameObject cur;
    private int visitedCellNum = 0;
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------递归回溯算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Stack<MazeCell> mazeStack = new Stack<MazeCell>();
        MazeCell startCell,previousCell,currentCell;
        startCell = _maze.maze[1, 1];
        //endCell = maze[2 * width - 1, 2 * length - 1];
        mazeStack.Push(startCell);
        previousCell = null;
        while (visitedCellNum < (width * length))
        {
            if (cur != null)
            {
                Destroy(cur);
            }
            if (mazeStack.Count <= 0)
            {
                break;
            }
            currentCell = mazeStack.Peek();
            currentCell.IsVisited = true;
            
            List<MazeCell> neighbour = _maze.GetNeighbours(currentCell);
            if (neighbour.Count > 0)
            {
                ////Debug.Log(neighbour.Count);
                //foreach (var item in neighbour)
                //{
                //    if (mazeStack.Contains(item) == false)
                //        mazeStack.Push(item);
                //}
                int index = Random.Range(0, neighbour.Count);
                mazeStack.Push(neighbour[index]);
                visitedCellNum++;
            }
            else
            {
                mazeStack.Pop();
                Debug.Log(currentCell.ToString() + "没有可选邻居");
            }
            
            GeneratePathInMaze(previousCell, currentCell);
            previousCell = currentCell;
            cur=Instantiate(current, currentCell.CellPos+new Vector3(0,0.2f,0), Quaternion.identity);
            yield return null;
            
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
    }
}
