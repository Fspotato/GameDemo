using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    public override void EnemyTurn(Player player)
    {
        player.TakeDamage(attack);
        Heal(maxHp * 0.2f);
    }
}
