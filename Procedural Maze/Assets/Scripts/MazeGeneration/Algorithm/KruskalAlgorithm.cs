using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class KruskalAlgorithm : MazeGenerationAlgorithm
{
    private List<MazeCell> wallsList;
    private Dictionary<int, HashSet<MazeCell>> hashSetDic;
    protected override void Start()
    {
        wallsList = new List<MazeCell>();
        hashSetDic = new Dictionary<int, HashSet<MazeCell>>();
        Pretreatment();
        base.Start();
    }
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------Kruskal算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        while (hashSetDic.Count > 1)
        {
            MazeCell firstWall = RandomlyChooseWall();
            if (firstWall == null)
            {
                break;
            }
            MazeCell cellFrom, cellTo;
            (int indexRow, int indexCol) = _maze.GetIndexFromCell(firstWall);
            //Debug.Log(indexRow + "-" + indexCol);
            if (indexRow % 2 == 0 && indexCol % 2 != 0)
            {
                cellFrom = _maze.maze[indexRow - 1, indexCol];
                cellTo = _maze.maze[indexRow + 1, indexCol];
                if (cellTo.attachedHash == cellFrom.attachedHash)
                {
                    continue;
                }
                int hash = cellTo.attachedHash;
                //Debug.Log(hash);
                foreach (var cell in hashSetDic[hash])
                {
                    hashSetDic[cellFrom.attachedHash].Add(cell);
                    cell.attachedHash = cellFrom.attachedHash;
                }
                hashSetDic[hash].Clear();
                hashSetDic.Remove(hash);
                Debug.Log("移除" + hash + "集合中的元素" + "合并到" + cellFrom.attachedHash + "集合");
                GeneratePathInMaze(cellFrom, cellTo);
            }
            else if (indexRow % 2 != 0 && indexCol % 2 == 0)
            {
                cellFrom = _maze.maze[indexRow, indexCol - 1];
                cellTo = _maze.maze[indexRow, indexCol + 1];
                if (cellTo.attachedHash == cellFrom.attachedHash)
                {
                    continue;
                }
                int hash = cellTo.attachedHash;
                //Debug.Log(hash);
                foreach (var cell in hashSetDic[hash])
                {
                    hashSetDic[cellFrom.attachedHash].Add(cell);
                    cell.attachedHash = cellFrom.attachedHash;
                }
                hashSetDic[hash].Clear();
                hashSetDic.Remove(hash);
                Debug.Log("移除" + hash + "集合中的元素"+"合并到" + cellFrom.attachedHash + "集合");
                GeneratePathInMaze(cellFrom, cellTo);
            }
            else
            {
                Debug.LogError("该墙的位置索引不对");
            }
            yield return null;
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
    }

    private void TravelHashSet()
    {
        foreach (var item in hashSetDic)
        {
            Debug.Log(item.Key + "对应的集合的个数" + item.Value.Count);
        }
    }
    private void Pretreatment()
    {
        int num = 0;
        for (int j = 1; j < 2 * length; j++)
        {
            for (int i = 1; i < 2 * width; i++)
            {
                if (_maze.maze[j, i].Cell == CellType.Floor)
                {
                    hashSetDic.Add(num, new HashSet<MazeCell>());
                    hashSetDic[num].Add(_maze.maze[j, i]);
                    _maze.maze[j, i].attachedHash = num;
                    ++num;
                }else if((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                {
                    if(_maze.maze[j,i].Cell==CellType.Wall)
                        wallsList.Add(_maze.maze[j, i]);
                }  
            }
        }
    }
    private MazeCell RandomlyChooseWall()
    {
        if (wallsList.Count <= 0) return null;
        MazeCell thefirstWall = wallsList[Random.Range(0, wallsList.Count)];
        wallsList.Remove(thefirstWall);
        //Debug.Log("选中的边为:"+thefirstWall.ToString());
        return thefirstWall;
        
    }
}
