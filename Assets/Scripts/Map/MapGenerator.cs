using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 地圖生成器
public static class MapGenerator
{
    private static MapConfig config;

    // 隨機節點
    // 可以放入地圖設定檔中 這樣可以讓特定地圖不會生成某些節點
    private static List<NodeType> RandomNodes = new List<NodeType>
    {NodeType.Mystery, NodeType.Enemy, NodeType.Elite, NodeType.Shop,  NodeType.Event};

    // 各階級之間的距離
    private static List<float> layerDistance = new List<float>();

    // 點位連成的一條條的路徑
    private static List<List<Point>> paths = new List<List<Point>>();

    // 地圖上的節點
    private static List<List<Node>> nodes = new List<List<Node>>();

    // 生成地圖的API 通常都外部調用此API
    public static Map GetMap(MapConfig conf)
    {
        if (conf == null)
        {
            // 如果找不到沒有給設定檔就會報錯
            Debug.LogWarning("Can't find MapConfig in MapGenerator.GetMap()");
            return null;
        }

        config = conf;
        // 清除節點內容 防殘留
        nodes.Clear();

        GenerateLayerDistance();

        // 將每一階級的節點生成出來
        for (int i = 0; i < config.layers.Count; i++)
            PlaceLayer(i);

        GeneratePath();

        RandomizeNodePositions();

        SetUpConnections();

        RemoveCrossConnections();

        var nodesList = nodes.SelectMany(n => n).Where(n => n.incoming.Count > 0 || n.outgoing.Count > 0).ToList();

        SerializableList<Node> mapNodes = new SerializableList<Node>();
        foreach (var node in nodesList)
            mapNodes.Add(node);

        var bossNodeName = config.nodeBlueprints.Where(b => b.nodeType == NodeType.Boss).ToList().Random().name;

        return new Map(conf.name, bossNodeName, new SerializableList<Point>(), mapNodes);
    }

    // 隨機生成階級中的距離
    private static void GenerateLayerDistance()
    {
        // 清除內容 防殘留
        layerDistance.Clear();
        // 根據設定檔隨機產生距離
        foreach (var layer in config.layers)
            layerDistance.Add(layer.distanceFromPreLayer.GetValue());
    }
    // 拿取從初始階級到當前階級的總距離
    private static float GetDistanceToLayer(int index)
    {
        if (index < 0 || index > layerDistance.Count) return 0f;

        return layerDistance.Take(index + 1).Sum();
    }
    // 節點生成
    private static void PlaceLayer(int index)
    {
        // 獲得設定檔中的當前階層
        var layer = config.layers[index];
        // 此階層中的節點
        var nodesOnThisLayer = new List<Node>();

        // 似乎是讓節點置中的功能
        var offset = layer.distanceFromApart * config.GridWidth / 2f;

        for (var i = 0; i < config.GridWidth; i++)
        {
            // 隨機產生節點類型 可以從設定檔設置要不要隨機
            var nodeType = Random.Range(0f, 1f) < layer.randomizeNodes ? GetRandomNode() : layer.nodeType;
            // 根據節點類型獲得節點藍圖 如果該節點類型有多個藍圖則會隨機選擇
            var bluePrintName = config.nodeBlueprints.Where(b => b.nodeType == nodeType).ToList().Random().name;
            // 生成節點
            var node = new Node(new Point(i, index), nodeType, bluePrintName)
            {
                position = new Vector2(-offset + i * layer.distanceFromApart, GetDistanceToLayer(index))
            };
            nodesOnThisLayer.Add(node);
        }

        // 把當前階層放入整張地圖中
        nodes.Add(nodesOnThisLayer);
    }
    // 產生最後一個階層的唯一節點
    private static Point GetFinalNode()
    {
        var y = config.layers.Count - 1;
        // 隨機產生位置(同階層偏移) 沒特殊用途 固定產生也沒差
        if (config.GridWidth % 2 == 1)
            return new Point(config.GridWidth / 2, y);

        return Random.Range(0, 2) == 0
            ? new Point(config.GridWidth / 2, y)
            : new Point(config.GridWidth / 2 - 1, y);
    }
    // 生成路徑
    private static void GeneratePath()
    {
        var finalNode = GetFinalNode();
        paths.Clear();
        // 從設定檔中讀取起始階層和倒數第二階有幾個點位
        var numOfStartingNodes = config.numOfStartingNodes.GetValue();
        var numOfPreBossNodes = config.numOfPreBossNodes.GetValue();

        // 隨機點位產生
        var candidateXs = new List<int>();
        for (int i = 0; i < config.GridWidth; i++)
            candidateXs.Add(i);

        // 起始階層隨機點位產生
        candidateXs.Shuffle();
        var startingXs = candidateXs.Take(numOfStartingNodes);
        var startingPoints = (from x in startingXs select new Point(x, 0)).ToList();

        // 倒數第二階隨機點位產生
        candidateXs.Shuffle();
        var preBossXs = candidateXs.Take(numOfPreBossNodes);
        var preBossPoints = (from x in preBossXs select new Point(x, finalNode.y - 1)).ToList();

        // 路徑數量
        int numsOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes);
        for (int i = 0; i < numsOfPaths; ++i)
        {
            Point startNode = startingPoints[i % numOfStartingNodes];
            Point endNode = preBossPoints[i % numOfPreBossNodes];
            var path = Path(startNode, endNode);
            // 把最後的節點加到所有路徑的最後面
            path.Add(finalNode);
            paths.Add(path);
        }

    }
    // 生成路徑
    private static List<Point> Path(Point fromPoint, Point toPoint)
    {
        int toRow = toPoint.y;
        int toCol = toPoint.x;

        int lastNodeCol = fromPoint.x;

        var path = new List<Point> { fromPoint };
        var candidateCols = new List<int>();
        for (int row = 1; row < toRow; ++row)
        {
            candidateCols.Clear();

            int verticalDistance = toRow - row;
            int horizontalDistance;

            int forwardCol = lastNodeCol;
            horizontalDistance = Mathf.Abs(toCol - forwardCol);
            if (horizontalDistance <= verticalDistance)
                candidateCols.Add(forwardCol);

            int leftCol = lastNodeCol - 1;
            horizontalDistance = Mathf.Abs(toCol - leftCol);
            if (leftCol >= 0 && horizontalDistance <= verticalDistance)
                candidateCols.Add(leftCol);


            int rightCol = lastNodeCol + 1;
            horizontalDistance = Mathf.Abs(toCol - rightCol);
            if (rightCol < config.GridWidth && horizontalDistance <= verticalDistance)
                candidateCols.Add(rightCol);

            int index = Random.Range(0, candidateCols.Count);
            int candidateCol = candidateCols[index];
            var nextPoint = new Point(candidateCol, row);

            path.Add(nextPoint);

            lastNodeCol = candidateCol;
        }

        path.Add(toPoint);

        return path;
    }

    private static void RandomizeNodePositions()
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            var list = nodes[i];
            var layer = config.layers[i];
            var distToNextLayer = i + 1 >= layerDistance.Count
                ? 0f
                : layerDistance[i + 1];
            var distToPreviousLayer = layerDistance[i];

            foreach (var node in list)
            {
                var xRnd = Random.Range(-1f, 1f);
                var yRnd = Random.Range(-1f, 1f);

                var x = xRnd * layer.distanceFromApart / 2f;
                var y = yRnd < 0 ? distToPreviousLayer * yRnd / 2f : distToNextLayer * yRnd / 2f;

                node.position += new Vector2(x, y) * layer.randomizePosition;
            }
        }
    }

    private static void SetUpConnections()
    {
        foreach (var path in paths)
        {
            for (var i = 0; i < path.Count - 1; ++i)
            {
                var node = GetNode(path[i]);
                var nextNode = GetNode(path[i + 1]);
                node.AddOutgoing(nextNode.point);
                nextNode.AddIncoming(node.point);
            }
        }
    }

    private static Node GetNode(Point p)
    {
        if (p.y >= nodes.Count) return null;
        if (p.x >= nodes[p.y].Count) return null;
        return nodes[p.y][p.x];
    }

    private static void RemoveCrossConnections()
    {
        for (var i = 0; i < config.GridWidth; ++i)
            for (var j = 0; j < config.layers.Count; ++j)
            {
                var node = GetNode(new Point(i, j));
                if (node == null || node.HasNoConnections()) continue;
                var right = GetNode(new Point(i + 1, j));
                if (right == null || right.HasNoConnections()) continue;
                var top = GetNode(new Point(i, j + 1));
                if (top == null || top.HasNoConnections()) continue;
                var topRight = GetNode(new Point(i + 1, j + 1));
                if (topRight == null || topRight.HasNoConnections()) continue;

                if (!node.outgoing.Any(p => p.Equals(topRight.point))) continue;
                if (!right.outgoing.Any(p => p.Equals(top.point))) continue;

                node.AddOutgoing(top.point);
                top.AddIncoming(node.point);

                right.AddOutgoing(topRight.point);
                topRight.AddIncoming(right.point);

                var rnd = Random.Range(0f, 1f);
                if (rnd < 0.2f)
                {
                    // remove both cross connections:
                    // a) 
                    node.RemoveOutgoing(topRight.point);
                    topRight.RemoveIncoming(node.point);
                    // b) 
                    right.RemoveOutgoing(top.point);
                    top.RemoveIncoming(right.point);
                }
                else if (rnd < 0.6f)
                {
                    // a) 
                    node.RemoveOutgoing(topRight.point);
                    topRight.RemoveIncoming(node.point);
                }
                else
                {
                    // b) 
                    right.RemoveOutgoing(top.point);
                    top.RemoveIncoming(right.point);
                }
            }
    }

    private static NodeType GetRandomNode()
    {
        return RandomNodes[Random.Range(0, RandomNodes.Count)];
    }
}
