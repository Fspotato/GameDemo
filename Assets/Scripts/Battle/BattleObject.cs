using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class BattleObject : MonoBehaviour
{
    [SerializeField] protected float hp;
    [SerializeField] protected float maxHp;

    public float Hp => hp;
    public float MaxHp => maxHp;
    public BuffManager Buffs;

    // 道具檢查
    public virtual void ItemCheck() { }

    // 受到傷害
    public virtual void TakeDamage(float value)
    {
        value = BuffCheckOnTakeDamage(value);
        value = Math.Max(1f, value);
        hp -= value;
        if (hp <= 0f)
        {
            hp = 0f;
            Die();
        }
    }

    // 恢復生命
    public virtual void Heal(float value)
    {
        hp += value;
        hp = Math.Min(hp, maxHp);
    }

    #region Buff檢查

    // 攻擊前檢查
    public virtual void BuffCheckBeforeAttack()
    {
        foreach (var buff in Buffs.Buffs)
        {
            switch (buff.Type)
            {
                case BuffType.Ignite:
                    TakeDamage(buff.Value * buff.Stack);
                    break;
                case BuffType.Forzen:
                    TakeDamage(buff.Value * buff.Stack);
                    break;
            }
        }
    }

    // 攻擊時檢查
    public void BuffCheckOnAttack()
    {
        // 暫無
    }

    // 攻擊後檢查
    public void BuffCheckAfterAttack()
    {
        // 暫無
    }

    // 受到傷害時檢查
    public float BuffCheckOnTakeDamage(float value)
    {
        foreach (var buff in Buffs.Buffs)
        {
            switch (buff.Type)
            {
                case BuffType.Guard:
                    value -= buff.Value * buff.Stack;
                    break;
            }
        }
        value = Math.Max(1, value);
        return value;
    }

    #endregion

    // 死亡
    void Die()
    {
        gameObject.SetActive(false);
    }
}
