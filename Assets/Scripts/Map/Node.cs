using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Point point;
    public SerializableList<Point> incoming = new SerializableList<Point>();
    public SerializableList<Point> outgoing = new SerializableList<Point>();
    public NodeType nodeType;
    public string blueprintName;
    public Vector2 position;

    public Node(Point point, NodeType nodeType, string blueprintName)
    {
        this.point = point;
        this.nodeType = nodeType;
        this.blueprintName = blueprintName;
    }

    public void AddIncoming(Point p)
    {
        if (incoming.Any(e => e.Equals(p))) return;
        incoming.Add(p);
    }

    public void AddOutgoing(Point p)
    {
        if (outgoing.Any(e => e.Equals(p))) return;
        outgoing.Add(p);
    }

    public void ClearIncoming()
    {
        incoming.Clear();
    }

    public void ClearOutgoing()
    {
        outgoing.Clear();
    }

    public void RemoveIncoming(Point point)
    {
        incoming.RemoveAll(p => p.Equals(point));
    }

    public void RemoveOutgoing(Point point)
    {
        outgoing.RemoveAll(p => p.Equals(point));
    }

    public bool HasNoConnections()
    {
        return incoming.Count == 0 && outgoing.Count == 0;
    }
}

[System.Serializable]
public enum NodeType
{
    Mystery,
    Enemy,
    Elite,
    Boss,
    Shop,
    Event,
    Building,
}

