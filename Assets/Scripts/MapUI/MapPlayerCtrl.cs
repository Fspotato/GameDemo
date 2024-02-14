using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayerCtrl : MonoBehaviour
{
    public static MapPlayerCtrl Instance;
    public MapManager mapManager;
    public MapUI mapUI;

    void Awake()
    {
        Instance = this;
    }

    public void SelectNode(MapNode node)
    {
        if (node.nodeStates == NodeStates.Attainable) SendPlayerToNode(node);
    }

    public void SendPlayerToNode(MapNode node)
    {
        mapManager.CurrentMap.path.Add(node.Node.point);
        mapManager.SaveMap();
        mapUI.SetAttainableNodes();

        switch (node.Node.nodeType)
        {
            case NodeType.Boss: GoToEnemyNode(BattleType.Boss); break;
            case NodeType.Elite: GoToEnemyNode(BattleType.EliteOnly); break;
            case NodeType.Enemy: GoToEnemyNode(BattleType.Normal); break;
            case NodeType.Mystery:
                int rnd = Random.Range(0, 4);
                switch (rnd)
                {
                    case 0: GoToEnemyNode(BattleType.Normal); break;
                    case 1: GoToEventNode(EventType.Normal); break;
                    case 2: GoToEventNode(EventType.Shop); break;
                    case 3: GoToEnemyNode(BattleType.EliteOnly); break;
                }
                break;
            case NodeType.Event: GoToEventNode(EventType.Normal); break;
            case NodeType.Shop: GoToEventNode(EventType.Shop); break;
            case NodeType.Building: print("Enter to Building Node!"); break;
        }
    }

    void GoToEnemyNode(BattleType type)
    {
        BattleManager.Instance.BattleStart(type);
        PlayerUI.Instance.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    void GoToEventNode(EventType type)
    {
        EventManager.Instance.EnterEvent(type);
        PlayerUI.Instance.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
