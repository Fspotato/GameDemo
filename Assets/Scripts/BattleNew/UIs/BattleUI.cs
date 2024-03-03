using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BattleNew
{
    public class BattleUI : BaseManager<BattleUI>
    {
        [SerializeField] GameObject hpBar;
        [SerializeField] BattleManager battleManager;
        [SerializeField] Text skillName;
        [SerializeField] Text skillDescription;
        ObjectPool<FadeOutText> fadeTexts;
        List<FadeOutText> textTemps = new List<FadeOutText>();

        bool initialized = false;

        public virtual void Init()
        {
            initialized = true;
            fadeTexts = new ObjectPool<FadeOutText>(
                () =>
                {
                    var fadeText = ABManager.Instance.LoadRes<GameObject>("art", "FadeOutText");
                    fadeText.transform.SetParent(transform);
                    return fadeText.GetComponent<FadeOutText>();
                },
                (fadeText) =>
                {
                    fadeText.gameObject.SetActive(true);
                },
                (fadeText) =>
                {
                    fadeText.fadeText.text = "";
                    fadeText.gameObject.SetActive(false);
                },
                (fadeText) =>
                {
                    Destroy(fadeText.gameObject);
                }, true, 50, 200
            );
        }

        // 戰鬥開始
        public virtual void BattleStart()
        {
            if (!initialized) Init();
            textTemps.Clear();
        }

        // 戰鬥結束
        public virtual void BattleEnd()
        {
            // 重置尚未結束FadeOut進程的FadeOutText
            for (int i = 0; i < textTemps.Count; i++)
            {
                if (textTemps[i] == null) continue;
                fadeTexts.Release(textTemps[i]);
            }
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
        // 有使用到Camera.main.WorldToScreenPoint給的座標的情況下 要設anchorChange為true位置才會正確
        public void FadeOut(string text, Color color, Vector3 position, bool anchorChange, bool isBold)
        {
            var fadeText = fadeTexts.Get();
            textTemps.Add(fadeText);
            if (anchorChange)
            {
                RectTransform textRect = fadeText.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0f, 0f);
                textRect.anchorMax = new Vector2(0f, 0f);
                textRect.anchoredPosition = new Vector2(position.x, position.y);
            }
            else
            {
                RectTransform textRect = fadeText.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.5f, 0.5f);
                textRect.anchorMax = new Vector2(0.5f, 0.5f);
                fadeText.transform.localPosition = position;
            }
            fadeText.FadeOut(text, color, isBold, (over) =>
            {
                if (over)
                {
                    textTemps.Remove(fadeText);
                    fadeTexts.Release(fadeText);
                }
            });
        }
    }
}

