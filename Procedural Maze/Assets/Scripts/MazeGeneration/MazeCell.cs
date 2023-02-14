using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell
{
    #region Common Fields
    private Vector3 cellPos;
    private CellType cell;
    private bool isVisited;
    public GameObject obj;
    #endregion

    #region Special Fields For Eller's Algorithm
    public int attachedHash = -1;
    #endregion

    public MazeCell()
    {
        //Debug.Log("MazeCell默认构造函数");
        this.cellPos = Vector3.zero;
        this.cell = CellType.None;
        this.isVisited = false;
    }
    public MazeCell(Vector3 pos,CellType cellType)
    {
        //Debug.Log("MazeCell有参构造函数");
        this.cellPos = pos;
        this.cell = cellType;
        this.isVisited = false;
    }
    public void ChangeCellType(CellType type)
    {
        this.cell = type;
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != this.GetType()) return false;
        //Debug.Log("调用Equals");
        MazeCell mazeCell = obj as MazeCell;
        return mazeCell.cellPos == this.cellPos;
    }
    public override int GetHashCode()
    {
        return cellPos.GetHashCode();
    }
    public override string ToString()
    {
        if (this == null)
        {
            return "当前为null";
        }
        return "Cell:" + cellPos.ToString() + "-" + cell /*+ "-" + attachedHash*/;
    }
    public Vector3 CellPos { get => cellPos;}
    public CellType Cell { get => cell;}
    public bool IsVisited { get => isVisited; set => isVisited = value; }
}
public enum CellType
{
    None,
    Floor,
    Wall,
}
