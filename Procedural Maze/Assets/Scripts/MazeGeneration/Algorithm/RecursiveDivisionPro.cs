using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveDivisionPro : MazeGenerationAlgorithm
{
    private Dictionary<int, List<MazeCell>> hashList;
    private List<MazeCell> mazeCellList;

    protected override void Start()
    {
        mazeCellList = new List<MazeCell>();
        hashList = new Dictionary<int, List<MazeCell>>();
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
        int dictionaryNum = 0;
        MazeCell seedA, seedB;
        seedA = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(mazeCellList);
        seedB = MazeHelper.RandomlyChooseAnItemFromList<MazeCell>(mazeCellList);
        AddCellToDictionary(seedA, dictionaryNum++);
        AddCellToDictionary(seedB, dictionaryNum++);
        while (mazeCellList.Count > 0)
        {

            yield return null;
        }
    }

    private void AddCellToDictionary(MazeCell seedA, int dictionaryNum)
    {
        if (!hashList.ContainsKey(dictionaryNum))
        {
            Debug.Log("给定的dictionaryNum未包含在hashList,需要添加对应Key的元素");
            hashList.Add(dictionaryNum, new List<MazeCell>());
            hashList[dictionaryNum].Add(seedA);
        }
        else
        {
            hashList[dictionaryNum].Add(seedA);
            Debug.Log("已经将" + seedA.ToString() + "添加到序号为:" + dictionaryNum);
        }
    }
}
