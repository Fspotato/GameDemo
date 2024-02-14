using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinRider : Enemy
{
    public override void EnemyTurn(Player player)
    {
        player.TakeDamage(Attack * 1.5f);
        print($"{Name} 使用了 衝鋒!");
    }
}
