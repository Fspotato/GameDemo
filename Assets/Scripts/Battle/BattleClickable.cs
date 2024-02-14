using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleClickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Text text;
    [SerializeField] BattleClickType type;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case BattleClickType.EndTurn:
                BattleUI.Instance.EndTurn();
                break;
        }
    }
}

public enum BattleClickType
{
    EndTurn = 0,
}