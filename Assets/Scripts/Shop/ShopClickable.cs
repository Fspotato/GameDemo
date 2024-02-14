using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopClickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Text text;
    [SerializeField] ShopClickType type;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case ShopClickType.CloseShop:
                ShopManager.Instance.CloseShop();
                break;
            case ShopClickType.Buy:
                ShopManager.Instance.BuyItem(transform.parent.GetComponent<ItemObject>());
                break;
        }
    }
}

public enum ShopClickType
{
    CloseShop = 0,
    Buy = 1,
}
