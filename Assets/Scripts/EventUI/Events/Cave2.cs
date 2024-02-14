using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Cave2")]
public class Cave2 : Event
{
    public override void Selection1()
    {
        switch (level)
        {
            case 0:
                DataManager.Instance.GetMoney(10);
                EventManager.Instance.ShowEvent("獲得金幣", "你獲得了10金幣", new List<string> { "離開事件" });
                level = 1;
                break;
            case 1: EventManager.Instance.ExitEvent(); break;
        }
    }
}
