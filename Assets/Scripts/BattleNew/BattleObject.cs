using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleNew
{
    public class BattleObject : MonoBehaviour
    {
        [SerializeField] protected float hp;
        [SerializeField] protected float maxHp;
        [SerializeField] protected float attack;
        protected float extraMaxHp;
        protected float extraAttack;
        protected bool isDead = false;


        public float Attack => attack;
        public float Hp => hp;
        public float MaxHp => maxHp;
        public float ExtraAttack => extraAttack;
        public float ExtraMaxHp => extraMaxHp;
        public bool IsDead => isDead;
        public BuffManager BuffManager;

        // 道具檢查
        public virtual void ItemCheck() { }

        // 受到傷害
        public virtual void TakeDamage(float value)
        {
            if (isDead) return;
            value = BuffCheckOnTakeDamage(value);
            value = Mathf.Max(1f, value);
            BattleUI.Instance.FadeOut($"-{value}", Color.red, Camera.main.WorldToScreenPoint(transform.localPosition), true, true);
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
            if (isDead) return;
            hp += value;
            hp = Mathf.Min(hp, maxHp);
        }

        #region Buff檢查

        // 攻擊前檢查
        public virtual void BuffCheckBeforeAttack()
        {
            foreach (var buff in BuffManager.Buffs)
            {
                switch (buff.Type[0])
                {
                    case BuffType.Ignite:
                        print($"{gameObject.name} 受到了 {buff.Value * buff.Stack} 點燃燒傷害!");
                        TakeDamage(buff.Value * buff.Stack);
                        break;
                    case BuffType.Forzen:
                        print($"{gameObject.name} 受到了 {buff.Value * buff.Stack} 點寒冷傷害!");
                        TakeDamage(buff.Value * buff.Stack);
                        break;
                    case BuffType.Bleeding:
                        print($"{gameObject.name} 受到了 {buff.Value * buff.Stack} 點出血傷害!");
                        if (GetComponent<Enemy>() != null && DataManager.Instance.CheckItemExist("血劍")) BattleManager.Instance.HealPlayer(buff.Value * buff.Stack * 0.1f);
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
            foreach (var buff in BuffManager.Buffs)
            {
                switch (buff.Type[0])
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
            if (isDead) return;
            isDead = true;
            gameObject.SetActive(false);
        }
    }
}
