using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MapNode : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Node Node;
    public NodeBlueprint Blueprint;
    public NodeStates nodeStates;

    public void SetState(NodeStates nodeStates)
    {
        this.nodeStates = nodeStates;
        switch (nodeStates)
        {
            case NodeStates.Attainable:
                transform.GetComponent<Image>().color = Color.gray;
                break;
            case NodeStates.Locked:
                transform.GetComponent<Image>().color = Color.black;
                break;
            case NodeStates.Visited:
                transform.GetComponent<Image>().color = Color.white;
                break;
        }
    }


    public void OnPointerClick(PointerEventData data)
    {
        MapPlayerCtrl.Instance.SelectNode(this);
    }
}

public enum NodeStates
{
    Locked,
    Visited,
    Attainable
}
