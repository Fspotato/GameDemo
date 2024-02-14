using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventClickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Text text;
    public EventClickType type;


    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case EventClickType.Selection1:
                EventManager.Instance.Selections(0);
                break;
            case EventClickType.Selection2:
                EventManager.Instance.Selections(1);
                break;
            case EventClickType.Selection3:
                EventManager.Instance.Selections(2);
                break;
        }
    }
}

public enum EventClickType
{
    Selection1,
    Selection2,
    Selection3,
}
