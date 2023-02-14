using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 生成树算法重点在于如何从algorithmNeedList中选择元素
/// 1、如果随机选择，那么就是Prim算法
/// 2、如果选择最新加入列表的元素，就相当于递归回溯算法
/// </summary>
public class GrowingTreeAlgorithm : MazeGenerationAlgorithm
{
    private List<MazeCell> mazeCellList;
    List<MazeCell> algorithmNeedList = new List<MazeCell>();
    //[SerializeField] private bool randomlyChoose = false;
    [SerializeField] private GameObject current;
    private GameObject cur;
    [Tooltip("列表选择的方式")]
    public ChooseMethod chooseMethod;

    protected override void Start()
    {
        mazeCellList = new List<MazeCell>();
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < width; i++)
            {
                mazeCellList.Add(_maze.maze[2 * j + 1, 2 * i + 1]);
            }
        }
        base.Start();
    }
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------生成树算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        MazeCell curCell, nextCell;
        curCell = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(mazeCellList);
        curCell.IsVisited = true;
        algorithmNeedList.Add(curCell);
        while (algorithmNeedList.Count > 0)
        {
            if (cur != null) Destroy(cur);
            var neighbour = _maze.GetNeighbours(curCell);
            if (neighbour.Count == 0)
            {
                algorithmNeedList.Remove(curCell);
                switch (chooseMethod)
                {
                    case ChooseMethod.Newest:
                        Debug.Log("Choose The Newest");
                        if (algorithmNeedList.Count <= 0) break;
                            curCell = algorithmNeedList[algorithmNeedList.Count - 1];
                        break;
                    case ChooseMethod.Random:
                        Debug.Log("Choose Randomly");
                        curCell = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(algorithmNeedList);
                        break;
                    case ChooseMethod.NewestBlendRandomWith3To1:
                        Debug.Log("Newest Blend Random With 3 : 1");
                        if (Random.value < 0.75f)
                        {
                            if (algorithmNeedList.Count <= 0) break;
                                curCell = algorithmNeedList[algorithmNeedList.Count - 1];
                        }
                        else
                        {
                            curCell = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(algorithmNeedList);
                        }
                        break;
                    case ChooseMethod.NewestBlendRandomWith1To1:
                        Debug.Log("Newest Blend Random With 1 : 1");
                        if (Random.value < 0.5f)
                        {
                            if (algorithmNeedList.Count <= 0) break;
                            curCell = algorithmNeedList[algorithmNeedList.Count - 1];
                        }
                        else
                        {
                            curCell = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(algorithmNeedList);
                        }
                        break;
                    case ChooseMethod.NewestBlendRandomWith1To3:
                        Debug.Log("Newest Blend Random With 1 : 3");
                        if (Random.value < 0.25f)
                        {
                            if (algorithmNeedList.Count <= 0) break;
                            curCell = algorithmNeedList[algorithmNeedList.Count - 1];
                        }
                        else
                        {
                            curCell = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(algorithmNeedList);
                        }
                        break;
                    case ChooseMethod.Oldest:
                        Debug.Log("Choose The Oldest");
                        if (algorithmNeedList.Count>0)
                            curCell = algorithmNeedList[0];
                        break;
                    case ChooseMethod.Middle:
                        Debug.Log("Choose In the Middle");
                        if (algorithmNeedList.Count > 0)
                        {
                            curCell = algorithmNeedList[algorithmNeedList.Count / 2];
                        }
                        break;
                    case ChooseMethod.NewestBlendOldestWith1To1:
                        Debug.Log("Newest Blend Oldest With 1 : 1");
                        if (Random.value < 0.5f)
                        {
                            if (algorithmNeedList.Count <= 0) break;
                                curCell = algorithmNeedList[algorithmNeedList.Count - 1];
                        }
                        else
                        {
                            if(algorithmNeedList.Count>0)
                                curCell = algorithmNeedList[0];
                        }
                        break;
                    default:
                        Debug.Log("不存在这种情况");
                        break;
                }
                continue;
            }
            cur = Instantiate(current, curCell.CellPos + new Vector3(0, 0.2f, 0), Quaternion.identity);
            nextCell = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(neighbour);
            GeneratePathInMaze(curCell, nextCell);
            nextCell.IsVisited = true;
            algorithmNeedList.Add(nextCell);
            curCell = nextCell;
            //yield return new WaitForSeconds(0.2f);
            yield return null;
        }
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
    }
    private void OnDrawGizmos()
    {
        if (algorithmNeedList.Count > 0)
        {
            Gizmos.color = Color.blue;
            foreach (var cell in algorithmNeedList)
            {
                Gizmos.DrawCube(cell.CellPos, new Vector3(0.5f, 0.5f, 0.5f));
            }
        }
    }
    public enum ChooseMethod
    {
        Newest,//总是选择最新加入列表的元素
        Random,//随机选择元素
        NewestBlendRandomWith3To1,//两者以3:1的比例混合
        NewestBlendRandomWith1To1,//两者以1:1的比例混合
        NewestBlendRandomWith1To3,//两者以1:3的比例混合
        Oldest,//总是选择列表中最后的元素
        Middle,//总是选择列表中间的元素
        NewestBlendOldestWith1To1,//二者混合
    }
}


