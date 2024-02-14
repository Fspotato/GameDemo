using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsMan : Player
{
    #region 技能組

    protected override void SkillR00(string skillName, Enemy enemy, GameObject[] enemies)
    {
        switch (skillName)
        {
            case "劈砍":
                enemy.TakeDamage(4);
                break;
            case "強攻":
                enemy.TakeDamage(8);
                break;
            case "火焰刀":
                enemy.TakeDamage(5);
                enemy.Buffs.GetBuff(BuffType.Ignite, 50001, 5, 1, 3, 1);
                break;
        }
    }

    protected override void SkillB00(string skillName, Enemy enemy, GameObject[] enemies)
    {
        switch (skillName)
        {
            case "守護":
                Buffs.GetBuff(BuffType.Guard, 50002, 2, 1, 1, 2);
                break;
        }
    }

    protected override void SkillY00(string skillName, Enemy enemy, GameObject[] enemies)
    {
        switch (skillName)
        {
            case "鬥氣":
                Heal(10);
                break;
        }
    }

    #endregion
}
