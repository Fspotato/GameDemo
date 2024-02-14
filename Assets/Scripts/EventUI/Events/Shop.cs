using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Shop")]
public class Shop : Event
{
    public override void Selection1()
    {
        ShopManager.Instance.OpenShop(ShopType.DefaultShop);
        EventManager.Instance.gameObject.SetActive(false);
    }

    public override void Selection2()
    {
        EventManager.Instance.ExitEvent();
    }

    public override void Selection3()
    {

    }
}
