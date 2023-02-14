using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SidewinderAlgorithm : MazeGenerationAlgorithm
{
    [SerializeField,Range(0f,1f)] private float CarveEastProbability = 0.5f;
    private List<MazeCell> runSet;
    private Vector2Int east = new Vector2Int(0, 2);
    private Vector2Int north = new Vector2Int(2, 0);

    protected override void Start()
    {
        runSet = new List<MazeCell>();
        base.Start();
    }
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------响尾蛇算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        MazeCell curCell, nextCell;
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < width; i++)
            {
                curCell = _maze.maze[2 * j + 1, 2 * i + 1];
                runSet.Add(curCell);
                if (Random.value < CarveEastProbability)
                {
                    nextCell = _maze.GetDesignatedDirectionNeighbour(curCell, east);
                    if (nextCell != null)
                        GeneratePathInMaze(curCell, nextCell);
                    else
                        GeneratePathInMaze(curCell, _maze.GetDesignatedDirectionNeighbour(curCell, north));
                    //curCell = nextCell;
                    //runSet.Add(curCell);
                }
                else
                {
                    curCell = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(runSet);
                    nextCell = _maze.GetDesignatedDirectionNeighbour(curCell, north);
                    if (nextCell != null)
                        GeneratePathInMaze(curCell, nextCell);
                    else GeneratePathInMaze(curCell, _maze.GetDesignatedDirectionNeighbour(curCell, east));
                    runSet.Clear();
                }
                runSet.Clear();
                yield return null;
            }
            yield return null;
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
    }
}
