using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleNew
{
    public class Player : BattleObject
    {
        protected Canvas canvas;

        // 載入資料
        public virtual void LoadData(Canvas canvas)
        {
            this.canvas = canvas;
            hp = DataManager.Instance.GetPlayerHp();
            maxHp = DataManager.Instance.GetPlayerMaxHp();
            attack = DataManager.Instance.GetPlayerAttack();
            Buffs = new BuffManager();
            ItemCheck();
            FirstTurn();
            StartTurn();
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

        // 技能使用
        public virtual void UseSkill(uint id, Enemy enemy, List<GameObject> enemies) { }

        // 首回合特殊行動
        public virtual void FirstTurn() { }

        // 每回合開始特殊動作
        public virtual void StartTurn()
        {
            BattleUI.Instance.ShowHpBar(hp, maxHp);
            BuffCheckBeforeAttack();
            Buffs.RoundOver();
        }

        // 每回合結束特殊動作
        public virtual void EndTurn() { }

        // 戰鬥結束特殊動作
        public virtual void BattleEnd() { }

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
    }
}
