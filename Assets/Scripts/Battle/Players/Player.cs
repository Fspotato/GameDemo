using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BattleObject
{

    // 載入資料
    public void LoadData()
    {
        hp = DataManager.Instance.GetPlayerHp();
        maxHp = DataManager.Instance.GetPlayerMaxHp();
        Buffs = new BuffManager();
        ItemCheck();
        BattleUI.Instance.ShowHpBar(hp, maxHp);
    }

    // 道具檢查
    public override void ItemCheck()
    {
        if (DataManager.Instance.CheckItemExist("聖劍")) Buffs.GetBuff(BuffType.Guard, 12345, 5, 1, 1, 99);
    }

    // 受到傷害
    public override void TakeDamage(float value)
    {
        base.TakeDamage(value);
        BattleUI.Instance.ShowHpBar(hp, maxHp);
    }

    // 恢復生命
    public override void Heal(float value)
    {
        base.Heal(value);
        BattleUI.Instance.ShowHpBar(hp, maxHp);
    }

    #region 技能組

    // 供外部調用
    public virtual void UseSkill(string arrange, Enemy enemy, GameObject[] enemies)
    {

        string skillName = SkillManager.Instance.GetSkillName(arrange);

        switch (arrange)
        {
            case "R00": SkillR00(skillName, enemy, enemies); break;
            case "B00": SkillB00(skillName, enemy, enemies); break;
            case "Y00": SkillY00(skillName, enemy, enemies); break;
        }

        print($"你使用了 {skillName} !");
    }

    // 寫在子類角色腳本中
    protected virtual void SkillR00(string skillName, Enemy enemy, GameObject[] enemies) { }
    protected virtual void SkillB00(string skillName, Enemy enemy, GameObject[] enemies) { }
    protected virtual void SkillY00(string skillName, Enemy enemy, GameObject[] enemies) { }

    #endregion

    #region Buff檢查

    // 攻擊前檢查
    public override void BuffCheckBeforeAttack()
    {
        base.BuffCheckBeforeAttack();
        foreach (var buff in Buffs.Buffs)
        {
            switch (buff.Type)
            {
                case BuffType.Ignite:
                    print($"你受到了 {BuffCheckOnTakeDamage(buff.Value * buff.Stack)} 點 燃燒傷害!");
                    break;
                case BuffType.Forzen:
                    print($"你受到了 {BuffCheckOnTakeDamage(buff.Value * buff.Stack)} 點 寒冷傷害!");
                    break;
            }
        }
    }

    #endregion

    // 死亡
    void Die()
    {
        gameObject.SetActive(false);
    }
}
