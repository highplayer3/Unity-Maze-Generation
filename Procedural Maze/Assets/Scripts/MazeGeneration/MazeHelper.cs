using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public static class MazeHelper
{
    /// <summary>
    /// 让列表变得随机
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> MakeListRandom<T>(List<T> list)
    {
        var random = new Random();
        var newList = new List<T>();
        foreach (var item in list)
        {
            newList.Insert(Random.Range(0,newList.Count), item);
        }
        return newList;
    }
    /// <summary>
    /// 随机从列表中选择一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RandomlyChooseAnItemFromList<T>(List<T> list)
    {
        //Debug.Log(list.Count);
        if (list == null || list.Count <= 0)
        {
            Debug.Log("list参数不能为空/空列表");
            return default(T);
        }
        return list[Random.Range(0, list.Count)];
    }
}
public static class Direction
{
    public static List<Vector2Int> direction = new List<Vector2Int>
    {
        new Vector2Int(2,0),
        new Vector2Int(0,2),
        new Vector2Int(-2,0),
        new Vector2Int(0,-2)
    };
}

