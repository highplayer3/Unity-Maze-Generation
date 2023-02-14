using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EllerAlgorithm : MazeGenerationAlgorithm
{
    [SerializeField, Range(0.1f, 0.8f)] private float extendLeftProbability = 0.5f;
    private Dictionary<int, List<MazeCell>> listDic;

    protected override void Start()
    {
        listDic = new Dictionary<int, List<MazeCell>>();
        for (int i = 0; i < width; i++)
        {
            listDic.Add(2*i+1, new List<MazeCell>());
        }
        base.Start();
    }
    public override IEnumerator RunAlgorithm()
    {
        Debug.Log("---------Eller算法---------");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        InitializeHashSet();
        for (int j = 0; j < length; j++)
        {
            HandleWithRow(2 * j + 1);
            //PaintRow(2 * j + 1);
            GenerateWalls(2 * j + 1);
            RemoveRowFromListDic(2 * j + 1);
            yield return null;
        }
        DeleteListDic();
        sw.Stop();
        TimeSpan time = sw.Elapsed;
        Debug.Log("-----算法结束-----");
        Debug.Log("算法共耗时:" + time.TotalSeconds + "s");
        
    }

    private void DeleteListDic()
    {
        foreach (var item in listDic)
        {
            item.Value.Clear();
        }
        listDic.Clear();
    }

    private void RemoveRowFromListDic(int v)
    {
        Debug.Log("----移除第" + v + "行的数据----");
        for (int i = 0; i < width; i++)
        {
            MazeCell curCell = _maze.maze[v, 2 * i + 1];
            listDic[curCell.attachedHash].Remove(curCell);
        }
    }

    private void GenerateWalls(int v)
    {
        MazeCell curCell;
        int curIndex = -1;
        for (int i = 0; i < width-1; i++)
        {
            curCell = _maze.maze[v, 2 * i + 1];
            curIndex = curCell.attachedHash;
            MazeCell nextCell = _maze.maze[v, 2 * i + 3];
            if (nextCell.attachedHash == curIndex)
            {
                Destroy(_maze.maze[v, 2 * i + 2].obj);
                Instantiate(floor, _maze.maze[v, 2 * i + 2].CellPos, Quaternion.identity, transform);
            }
        }
        if (v != 2 * length - 1) 
        {
            for (int i = 0; i < width; i++)
            {
                curCell = _maze.maze[v, 2 * i + 1];
                curIndex = curCell.attachedHash;
                MazeCell downCell = _maze.maze[v + 2, 2 * i + 1];
                if (downCell.attachedHash == curIndex)
                {
                    Destroy(_maze.maze[v + 1, 2 * i + 1].obj);
                    Instantiate(floor, _maze.maze[v+1, 2 * i + 1].CellPos, Quaternion.identity, transform);
                }
            }
        }
        else
        {
            Debug.Log("已经是最后一行，不需要向下处理");
        }
        
    }

    //private void PaintRow(int v)
    //{
    //    for (int i = 0; i < width; i++)
    //    {
    //        Debug.Log("第"+v+"行:"+_maze.maze[v, 2 * i + 1].attachedHash);
    //    }
    //}

    private void HandleWithRow(int row)
    {
        Pretreatment(row);
        if (row == 2 * length - 1)
        {
            //Last Row
            for (int i = 0; i < width-1; i++)
            {
                Destroy(_maze.maze[row, 2 * i + 2].obj);
                Instantiate(floor, _maze.maze[row, 2 * i + 2].CellPos, Quaternion.identity, transform);
            }
            Debug.Log("目前是最后一行");
        }
        else
        {
            for (int i = 0; i < width; i++)
            {
                HandleSingleCell(_maze.maze[row, 2 * i + 1]);
            }
            ExtendSetDownwards(row);
        }
        
    }

    private void Pretreatment(int v)
    {
        Debug.Log("第" + v + "行的预处理");
        for (int i = 0; i < width; i++)
        {
            if (_maze.maze[v, 2 * i + 1].attachedHash == -1)
            {
                int indices;
                List<MazeCell> newList;
                (indices, newList) = SearchEmptyList();
                newList.Add(_maze.maze[v, 2 * i + 1]);
                _maze.maze[v, 2 * i + 1].attachedHash = indices;
                Debug.Log(_maze.maze[v, 2 * i + 1].ToString() + "需要被重新划分给序号为" + indices + "的集合");
            }
        }
    }

    private void ExtendSetDownwards(int row)
    {
        HashSet<int> listIndex = new HashSet<int>();
        for (int i = 0; i < width; i++)
        {
            listIndex.Add(_maze.maze[row, 2 * i + 1].attachedHash);
        }
        //Debug.Log("第" + row + "行的集合序号");
        foreach (var index in listIndex)
        {
            //保证至少有一个向下延展
            int extendDownwardsNum = Random.Range(1, listDic[index].Count);
            List<MazeCell> tempList = GetRandomElementInList(extendDownwardsNum, listDic[index]);
            foreach (var item in tempList)
            {
                (int indexRow, int indexCol) = _maze.GetIndexFromCell(item);
                Debug.Log(item.ToString() + "被拓展到" + index + "的集合中");
                listDic[index].Add(_maze.maze[indexRow + 2, indexCol]);
                _maze.maze[indexRow + 2, indexCol].attachedHash = index;
            }
        }
    }

    private List<MazeCell> GetRandomElementInList(int extendDownwardsNum, List<MazeCell> mazeCells)
    {
        List<MazeCell> list = new List<MazeCell>(mazeCells);
        List<MazeCell> result = new List<MazeCell>();
        while (extendDownwardsNum > 0)
        {
            MazeCell cell = list[Random.Range(0, list.Count)];
            result.Add(cell);
            list.Remove(cell);
            extendDownwardsNum--;
        }
        return result;
    }

    private void HandleSingleCell(MazeCell mazeCell)
    {
        //与当前Cell的左邻居的集合合并
        if (Random.value < extendLeftProbability)
        {
            MazeCell leftNeighbour = _maze.GetLeftCell(mazeCell);
            if (leftNeighbour != null)
            {
                //Debug.Log(mazeCell.ToString() + "已合并到左邻居的集合"+leftNeighbour.attachedHash+"中");
                //Debug.Log(listDic[mazeCell.attachedHash].Count);
                listDic[mazeCell.attachedHash].Remove(mazeCell);
                //Debug.Log(listDic[mazeCell.attachedHash].Count);
                listDic[leftNeighbour.attachedHash].Add(mazeCell);
                mazeCell.attachedHash = leftNeighbour.attachedHash;
            }
        }
        //保持当前的集合
        else
        {
            Debug.Log(mazeCell.ToString() + "保持不变");
        }
    }

    private (int,List<MazeCell>) SearchEmptyList()
    {
        foreach (var item in listDic)
        {
            //Debug.Log(item.Key + "集合数目:" + item.Value.Count);
            if (item.Value.Count == 0)
            {
                //Debug.Log(item.Key + "是空集合");
                return (item.Key, item.Value);
            }
        }
        Debug.LogWarning("There is no useful hashSet");
        int indices = 0;
        while (listDic.ContainsKey(indices))
        {
            indices += 2;
        }
        listDic.Add(indices, new List<MazeCell>());
        return (indices, listDic[indices]);
    }
    //private void CheckListDic()
    //{
    //    Debug.Log("---------********----------");
    //    foreach (var item in listDic)
    //    {
    //        Debug.Log(item.Key + "集合中的元素:" + item.Value.Count);
    //    }
    //}
    private void InitializeHashSet()
    {
        for (int i = 0; i < width; i++)
        {
            listDic[2 * i + 1].Add(_maze.maze[1, 2 * i + 1]);
            _maze.maze[1, 2 * i + 1].attachedHash = 2 * i + 1;
        }
    }
}
