using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WilsonAlgorithm : MazeGenerationAlgorithm
{
    private List<MazeCell> mazeCellList;
    HashSet<MazeCell> target = new HashSet<MazeCell>();

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
        Debug.Log("---------Wilson(威尔逊)算法---------");
        Dictionary<MazeCell, Vector2Int> cellAndDirectionDic = new Dictionary<MazeCell, Vector2Int>();
        //HashSet<MazeCell> target = new HashSet<MazeCell>();
        MazeCell curCell = null;
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        curCell = RandomlyChooseACell();
        Debug.Log("第一次要找的目标:" + curCell.ToString());
        target.Add(curCell); 
        curCell = RandomlyChooseACell();
        while (target.Count < (width * length)&&curCell!=null)
        {
            cellAndDirectionDic.Clear();
            //等待RandomlyWalk协程执行完毕，再往下执行
            yield return StartCoroutine(RandomlyWalk(curCell, target, cellAndDirectionDic));
            curCell = RandomlyChooseACell();
            yield return null;
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");

    }

    private IEnumerator RandomlyWalk(MazeCell curCell,HashSet<MazeCell> target,Dictionary<MazeCell, Vector2Int> cellAndDirectionDic)
    {
        Debug.Log("开启协程" + "起点:" + curCell.ToString()+"-" + target.Count);
        MazeCell previousCell = null;
        MazeCell nextCell=curCell;
        while (!target.Contains(nextCell))
        {
            //if (previousCell == null) continue;
            var neighbour = _maze.GetNeighboursWithVisited(nextCell);
            Debug.Log(nextCell.ToString()+"的邻居数量:"+neighbour.Count);
            if (previousCell != null)
            {
                Debug.Log("******一次步行过程******");
                Debug.Log("前一个Cell:" + previousCell.ToString());
                Debug.Log("后一个Cell:" + nextCell.ToString());
                Vector2Int vector = _maze.GetVector(previousCell, nextCell);
                if (cellAndDirectionDic.ContainsKey(nextCell))
                {
                    if (!cellAndDirectionDic.ContainsKey(previousCell))
                    {
                        Debug.Log("将" + vector + "添加给" + previousCell.ToString());
                        cellAndDirectionDic.Add(previousCell, vector);
                    }
                    previousCell = nextCell;
                    ChangeDictionaryValue(nextCell, cellAndDirectionDic);
                    nextCell = _maze.GetDesignatedDirectionNeighbour(nextCell, cellAndDirectionDic[nextCell]);
                    continue;
                }
                else
                {
                    if (!cellAndDirectionDic.ContainsKey(previousCell))
                    {
                        Debug.Log("将" + vector + "添加给" + previousCell.ToString());
                        cellAndDirectionDic.Add(previousCell, vector);
                    }
                    else 
                    {
                        Debug.Log("nextCell不被包含但是包含PreviousCell");
                    }
                }
            }
            previousCell = nextCell;
            nextCell = neighbour[Random.Range(0, neighbour.Count)];
            yield return null;
        }
        if (!cellAndDirectionDic.ContainsKey(previousCell))
            cellAndDirectionDic.Add(previousCell, _maze.GetVector(previousCell, nextCell));
        Debug.Log("找打了target列表中的Cell，结束寻找过程开始回溯");
        MazeCell child=curCell, parent;
        while (!child.Equals(nextCell))
        {
            target.Add(child);
            mazeCellList.Remove(child);
            Debug.Log(child.ToString() + "With direction:" + cellAndDirectionDic[child]);
            parent = _maze.GetDesignatedDirectionNeighbour(child, cellAndDirectionDic[child]);
            Debug.Log(parent.ToString());
            GeneratePathInMaze(child, parent);
            child = parent;
            yield return null;
        }
    }

    private void ChangeDictionaryValue(MazeCell nextCell, Dictionary<MazeCell, Vector2Int> cellAndDirectionDic)
    {
        Vector2Int previousDir = cellAndDirectionDic[nextCell];
        var neighbour = _maze.GetNeighboursWithVisited(nextCell);
        var cell = neighbour[Random.Range(0, neighbour.Count)];
        Vector2Int vec = _maze.GetVector(nextCell, cell);
        while (vec == previousDir)
        {
            cell = neighbour[Random.Range(0, neighbour.Count)];
            vec= _maze.GetVector(nextCell, cell); 
        }
        cellAndDirectionDic[nextCell] = vec;
        Debug.Log(nextCell.ToString() + "修改前:" + previousDir + "修改后:" + vec);
    }

    private MazeCell RandomlyChooseACell()
    {
        if (mazeCellList.Count <= 0)
        {
            return null;
        }
        MazeCell result = mazeCellList[Random.Range(0, mazeCellList.Count)];
        mazeCellList.Remove(result);
        return result;
    }
    //private void OnDrawGizmosSelected()
    //{
    //    if (target.Count > 0)
    //    {
    //        Gizmos.color = Color.red;
    //        foreach (var item in target)
    //        {
    //            Gizmos.DrawLine(item.CellPos, item.CellPos + new Vector3(0, 1f, 0));
    //        }
    //    }
    //}
}
