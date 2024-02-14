using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LineConnection
{
    public LineRenderer lr;
    public MapNode from;
    public MapNode to;

    public LineConnection(LineRenderer lr, MapNode from, MapNode to)
    {
        this.lr = lr;
        this.from = from;
        this.to = to;
    }
}
