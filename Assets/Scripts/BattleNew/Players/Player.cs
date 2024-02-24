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
            BuffManager.Reset();
            ItemCheck();
            FirstTurn();
            StartTurn();
        }

        // 道具檢查
        public override void ItemCheck()
        {

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
        public virtual void UseSkill(uint id, Enemy enemy, List<GameObject> enemies)
        {
            BattleUI.Instance.FadeOut(SkillManager.Instance.GetSkillById(id).name, Color.white, new Vector3(0f, 280f, 0f), false, false);
        }

        // 首回合特殊行動
        public virtual void FirstTurn() { }

        // 每回合開始特殊動作
        public virtual void StartTurn()
        {
            BattleUI.Instance.ShowHpBar(hp, maxHp);
            BuffCheckBeforeAttack();
        }

        // 每回合結束特殊動作
        public virtual void EndTurn()
        {
            BuffManager.RoundOver();
        }

        // 戰鬥結束特殊動作
        public virtual void BattleEnd() { }

        #region Buff檢查

        #endregion
    }
}
