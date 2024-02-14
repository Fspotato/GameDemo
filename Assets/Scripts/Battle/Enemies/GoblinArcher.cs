using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArcher : Enemy
{
    public override void EnemyTurn(Player player)
    {
        player.TakeDamage(Attack);
        player.Buffs.GetBuff(BuffType.Forzen, 50003, Attack / 2f, 1, 2, 2);
    }
}
