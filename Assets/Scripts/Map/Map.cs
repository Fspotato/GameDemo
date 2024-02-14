using System.Linq;
using UnityEngine;

// Map主類別 一張地圖的所有元素都用這個類別存放
[System.Serializable]
public class Map
{
    public SerializableList<Point> path;
    public SerializableList<Node> nodes;
    public string bossNodeName;
    public string configName;

    public Map(string configName, string bossNodeName, SerializableList<Point> path, SerializableList<Node> nodes)
    {
        this.configName = configName;
        this.bossNodeName = bossNodeName;
        this.path = path;
        this.nodes = nodes;
    }

    // 獲得首領節點
    public Node GetBoss()
    {
        return nodes.FirstOrDefault(n => n.nodeType == NodeType.Boss);
    }

    // 獲得建築節點
    public Node GetBuilding()
    {
        return nodes.FirstOrDefault(n => n.nodeType == NodeType.Building);
    }

    // 根據點位獲得節點
    public Node GetNode(Point point)
    {
        return nodes.FirstOrDefault(n => n.point.Equals(point));
    }

    // 獲得首領節點和起始節點的距離
    public float DistanceFromFirstToBoss()
    {
        var bossNode = GetBoss();
        var firstLayerNode = nodes.FirstOrDefault(n => n.point.y == 0);

        if (bossNode == null || firstLayerNode == null) return 0f;

        return bossNode.point.y - firstLayerNode.point.y;
    }

    // 轉存成json格式
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
