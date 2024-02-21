using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BattleNew
{
    public class BasicSword : Sword
    {
        public override void Effect(Enemy enemy)
        {
            enemy.TakeDamage(10);
            SworderUI.Instance.GetComponent<SworderUI>().RemoveSword(this);
        }

        public override void Effect(Enemy enemy, List<GameObject> enemies)
        {
            Effect(enemy);
        }
    }
}