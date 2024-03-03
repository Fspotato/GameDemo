using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    public List<MapConfig> allMapConfigs;

    public GameObject nodePrefab;

    public GameObject linePrefab;

    public GameObject mapLinePrefab;

    public Material materialPrefab;

    public GameObject mapParent;

    public GameObject content;

    public List<MapNode> mapNodes = new List<MapNode>();

    public List<LineConnection> lineConnections = new List<LineConnection>();

    public List<MapLine> mapLines = new List<MapLine>();

    public MapManager mapManager;

    bool initialized = false;

    void Update()
    {
        ReCalculateLineShader();
    }

    public void ShowMap(Map m)
    {
        if (!initialized)
        {
            initialized = true;
            mapLinePrefab = ABManager.Instance.ReadRes<GameObject>("art", "LinkLine");
            materialPrefab = ABManager.Instance.ReadRes<Material>("art", "LineMaterial");
        }

        ClearMap();

        CreateMapParent();

        CreateMapNodes(m.nodes);

        DrawLines();
    }

    private void DrawLines()
    {
        foreach (var node in mapNodes)
            foreach (var connection in node.Node.outgoing)
            {
                AddMapLine(node, GetNode(connection));
                AddLineConnection(node, GetNode(connection));
            }
    }

    private void AddMapLine(MapNode from, MapNode to)
    {
        if (mapLinePrefab == null)
        {
            Debug.LogError("mapLinePrefab can't found! in MapUI.AddMapLine");
            return;
        }

        GameObject lineObj = Instantiate(mapLinePrefab, mapParent.transform);
        Material lm = lineObj.GetComponent<Image>().material = Instantiate(materialPrefab);

        Vector2 fromPoint = from.GetComponent<RectTransform>().anchoredPosition;
        Vector2 toPoint = to.GetComponent<RectTransform>().anchoredPosition;

        lineObj.GetComponent<RectTransform>().anchoredPosition = (fromPoint + toPoint) / 2f;
        lineObj.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(fromPoint.x - toPoint.x) + 30f, Mathf.Abs(fromPoint.y - toPoint.y) + 30f);

        lineObj.transform.SetAsFirstSibling();
        mapLines.Add(new MapLine(lineObj, from, to));

        lm.SetFloat("_LineWidth", 7.5f);
    }

    private void ReCalculateLineShader()
    {
        foreach (var line in mapLines)
        {
            var image = line.obj.GetComponent<Image>();
            var material = image.GetModifiedMaterial(image.material);
            material.SetVector("_LP1", new Vector4(RectTransformUtility.WorldToScreenPoint(null, line.from.transform.position).x, 1080f - RectTransformUtility.WorldToScreenPoint(null, line.from.transform.position).y, 0f, 0f));
            material.SetVector("_LP2", new Vector4(RectTransformUtility.WorldToScreenPoint(null, line.to.transform.position).x, 1080f - RectTransformUtility.WorldToScreenPoint(null, line.to.transform.position).y, 0f, 0f));
            if (line.from.nodeStates == NodeStates.Visited && line.to.nodeStates == NodeStates.Attainable) material.SetColor("_Color", Color.gray);
            else if (line.from.nodeStates == NodeStates.Visited && line.to.nodeStates == NodeStates.Visited) material.SetColor("_Color", Color.white);
            else material.SetColor("_Color", Color.black);
        }
    }

    private void AddLineConnection(MapNode from, MapNode to)
    {
        if (linePrefab == null)
        {
            Debug.LogError("linePrefab is null in MapUI.CreateMapNodes()");
            return;
        }

        var lineObj = Instantiate(linePrefab, mapParent.transform);
        var lineRenderer = lineObj.GetComponent<LineRenderer>();
        var fromPoint = from.transform.position +
                        (to.transform.position - from.transform.position).normalized * 0.5f;
        var toPoint = to.transform.position +
                      (from.transform.position - to.transform.position).normalized * 0.5f;

        lineObj.transform.position = fromPoint;
        lineRenderer.useWorldSpace = false;

        lineRenderer.positionCount = 3;
        for (var i = 0; i < 3; i++)
        {
            lineRenderer.SetPosition(i,
                Vector3.Lerp(Vector3.zero, toPoint - fromPoint, (float)i / (3 - 1)));
        }

        lineConnections.Add(new LineConnection(lineRenderer, from, to));
    }

    private void ClearMap()
    {
        if (mapParent != null) Destroy(mapParent);
        lineConnections.Clear();
        mapLines.Clear();
        mapNodes.Clear();
    }

    private void CreateMapParent()
    {
        mapParent = new GameObject("MapContent");
        mapParent.transform.SetParent(content.transform);
        content.transform.localPosition = Vector2.zero;
        mapParent.AddComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void CreateMapNodes(List<Node> nodes)
    {
        if (nodePrefab == null)
        {
            Debug.LogError("nodePrefab is null in MapUI.CreateMapNodes()");
            return;
        }

        foreach (var node in nodes)
        {
            var mapNodeObj = Instantiate(nodePrefab, mapParent.transform);
            var mapNode = mapNodeObj.GetComponent<MapNode>();
            mapNode.Node = node;
            mapNode.Blueprint = GetBlueprint(node.blueprintName);
            mapNodeObj.transform.localPosition = new Vector2(node.position.y * 150, node.position.x * 150);
            mapNodes.Add(mapNode);
        }

        content.GetComponent<RectTransform>().sizeDelta = new Vector2(
            mapNodes[mapNodes.Count() - 1].gameObject.transform.localPosition.x - mapNodes[0].gameObject.transform.localPosition.x + 500f,
            content.GetComponent<RectTransform>().sizeDelta.y);

        RectTransform mapRect = mapParent.GetComponent<RectTransform>();
        mapRect.anchorMin = new Vector2(0f, 0.5f);
        mapRect.anchorMax = new Vector2(0f, 0.5f);
        mapRect.anchoredPosition = Vector2.zero;
    }

    public void SetAttainableNodes()
    {
        foreach (var node in mapNodes)
            node.SetState(NodeStates.Locked);

        if (mapManager.CurrentMap.path.Count == 0)
        {
            foreach (var node in mapNodes.Where(n => n.Node.point.y == 0))
                node.SetState(NodeStates.Attainable);
        }
        else
        {
            foreach (var point in mapManager.CurrentMap.path)
            {
                var mapNode = GetNode(point);
                if (mapNode != null) mapNode.SetState(NodeStates.Visited);
            }

            var currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
            var currentNode = mapManager.CurrentMap.GetNode(currentPoint);

            foreach (var point in currentNode.outgoing)
            {
                var mapNode = GetNode(point);
                if (mapNode != null) mapNode.SetState(NodeStates.Attainable);
            }

        }
    }

    public MapNode GetNode(Point p)
    {
        return mapNodes.FirstOrDefault(n => n.Node.point.Equals(p));
    }

    public MapConfig GetConfig(string configName)
    {
        return allMapConfigs.FirstOrDefault(c => c.name == configName);
    }

    public NodeBlueprint GetBlueprint(NodeType type)
    {
        var config = GetConfig(mapManager.CurrentMap.configName);
        return config.nodeBlueprints.FirstOrDefault(b => b.nodeType.Equals(type));
    }

    public NodeBlueprint GetBlueprint(string blueprintName)
    {
        var config = GetConfig(mapManager.CurrentMap.configName);
        return config.nodeBlueprints.FirstOrDefault(n => n.name == blueprintName);
    }
}
