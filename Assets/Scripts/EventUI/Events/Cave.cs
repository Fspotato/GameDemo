using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Cave")]
public class Cave : Event
{
    public override void Selection1()
    {
        switch (level)
        {
            case 0:
                float rnd = Random.Range(0f, 100f);
                if (rnd <= 25f)
                {
                    EventManager.Instance.ShowEvent("空空如也", "什麼都沒有", new List<string> { "離開事件" });
                    level = 1;
                }
                else if (rnd <= 62.5f)
                {
                    DataManager.Instance.GetMoney(20);
                    EventManager.Instance.ShowEvent("獲得金幣", "你獲得了20金幣", new List<string> { "離開事件" });
                    level = 1;
                }
                else
                {
                    EventManager.Instance.ShowEvent("遭遇敵人", "洞窟裡隱藏著一些怪物!", new List<string> { "進入戰鬥" });
                    level = 2;
                }
                break;
            case 1: EventManager.Instance.ExitEvent(); break;
            case 2:
                BattleManager.Instance.BattleStart(BattleType.Normal);
                EventManager.Instance.gameObject.SetActive(false);
                break;
        }

    }

    public override void Selection2()
    {
        EventManager.Instance.ShowEvent("弱者的選擇", "你這個娘炮", new List<string> { "離開事件" });
        level = 1;
    }

    public override void Selection3()
    {

    }
}
