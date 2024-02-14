using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/DesertedCellar")]
public class DesertedCellar : Event
{
    public override void Selection1()
    {
        switch (level)
        {
            case 0:
                float rnd = Random.Range(0f, 100f);
                if (rnd <= 75f)
                {
                    EventManager.Instance.ShowEvent("遭遇敵人", "地窖裡有一些史萊姆", new List<string> { "進入戰鬥" });
                    level = 2;
                }
                else
                {
                    EventManager.Instance.ShowEvent("遭遇敵人", "地窖裡有一些史萊姆，而且看起來變異了", new List<string> { "進入戰鬥" });
                    level = 3;
                }
                break;
            case 1: EventManager.Instance.ExitEvent(); break;
            case 2:
                BattleManager.Instance.BattleStart(BattleType.Normal, EnemyType.Slime);
                EventManager.Instance.gameObject.SetActive(false);
                break;
            case 3:
                BattleManager.Instance.BattleStart(BattleType.EliteOnly, EnemyType.Slime);
                EventManager.Instance.gameObject.SetActive(false);
                break;
        }
    }

    public override void Selection2()
    {
        EventManager.Instance.ShowEvent("弱者的選擇", "你這個娘炮", new List<string> { "離開事件" });
        level = 1;
    }
}
