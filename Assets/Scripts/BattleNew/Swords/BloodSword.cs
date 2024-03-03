using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BattleNew
{
    public class BloodSword : Sword
    {
        public override void Effect(Enemy enemy)
        {
            enemy.TakeDamage(5);
            enemy.GetBuff(50001, 10, 2);
            SworderUI.Instance.GetComponent<SworderUI>().RemoveSword(this);
        }

        public override void Effect(Enemy enemy, List<GameObject> enemies)
        {
            Effect(enemy);
        }
    }
}