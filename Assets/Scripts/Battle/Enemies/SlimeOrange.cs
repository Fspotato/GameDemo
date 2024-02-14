using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeOrange : Enemy
{
    public override void EnemyTurn(Player player)
    {
        player.TakeDamage(attack * 0.5f);
        player.Buffs.GetBuff(BuffType.Ignite, 51212, attack, 1, 3, 1);
    }
}
