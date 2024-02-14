using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapLine
{
    public GameObject obj;
    public MapNode from;
    public MapNode to;

    public MapLine(GameObject obj, MapNode from, MapNode to)
    {
        this.obj = obj;
        this.from = from;
        this.to = to;
    }
}
