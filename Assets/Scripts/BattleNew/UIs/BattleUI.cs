using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleNew
{
    public class BattleUI : BaseManager<BattleUI>
    {
        [SerializeField] GameObject hpBar;
        [SerializeField] BattleManager battleManager;
        [SerializeField] Text skillName;
        [SerializeField] Text skillDescription;
        [SerializeField] Text fadeOutText;

        [SerializeField] float fadeTime = 1f;
        IEnumerator fading;

        // 戰鬥開始
        public virtual void BattleStart() { }

        // 戰鬥結束
        public virtual void BattleEnd()
        {
            fadeOutText.text = "";
        }

        // 結束回合
        public void EndTurn()
        {
            battleManager.EnemyTurn();
        }

        // 更改血量條顯示
        public void ShowHpBar(float hp, float maxHp)
        {
            hpBar.GetComponent<HpBarController>().ShowHpBar(hp, maxHp);
        }

        // 顯示當前選擇技能
        public void ShowSkill(BattleClickType type)
        {
            Skill skill = default;
            switch (type)
            {
                case BattleClickType.BasicSkill: skill = SkillManager.Instance.GetEquipedSkillByType(SkillType.BasicSkill); break;
                case BattleClickType.SkillA: skill = SkillManager.Instance.GetEquipedSkillByType(SkillType.SkillA); break;
                case BattleClickType.SkillB: skill = SkillManager.Instance.GetEquipedSkillByType(SkillType.SkillB); break;
                case BattleClickType.Ultimate: skill = SkillManager.Instance.GetEquipedSkillByType(SkillType.Ultimate); break;
            }
            skillName.text = skill == default ? "未裝備技能" : skill.name;
            skillDescription.text = skill == default ? "未裝備技能" : skill.description;
        }

        // 行動後刷新技能框
        public void ResetSkillWindow()
        {
            skillName.text = "";
            skillDescription.text = "";
        }

        // 使用技能提示字
        public void FadeOut(string skillName)
        {
            if (fading != null) StopCoroutine(fading);
            fading = IEFadeOut(skillName);
            if (gameObject.activeInHierarchy) StartCoroutine(fading);
        }

        private IEnumerator IEFadeOut(string skillName)
        {
            float elapsedTime = 0f;
            Color tColor = fadeOutText.color;
            fadeOutText.text = skillName;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                fadeOutText.color = new Color(tColor.r, tColor.g, tColor.b, alpha);
                yield return null;
            }
        }
    }
}

