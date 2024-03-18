using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace BattleNew
{
    public class BattleResult : MonoBehaviour
    {
        [SerializeField] Text hpText;
        [SerializeField] Text moneyText;
        [SerializeField] Text skillPointText;

        private const int basicMoney = 5; // 獲得金幣基準值
        private const int basicSkillPoint = 10; // 獲得技能點基準值

        public void ShowResult(int hp, BattleType type)
        {
            gameObject.SetActive(true);
            hpText.text = hp.ToString();
            switch (type)
            {
                case BattleType.Normal: NormalReward(); break;
                case BattleType.Elite: EliteReward(); break;
                case BattleType.EliteOnly: EliteReward(); break;
                case BattleType.Boss: BossReward(); break;
            }
        }

        private void NormalReward() // 可分配基準值 3
        {
            SkillManager.Instance.GetSkillPoint(basicSkillPoint * 1);
            DataManager.Instance.GetMoney(basicMoney * 2);
            moneyText.text = (basicMoney * 2).ToString();
            skillPointText.text = (basicSkillPoint * 1).ToString();
        }

        private void EliteReward() // 可分配基準值 6
        {
            SkillManager.Instance.GetSkillPoint(basicSkillPoint * 3);
            DataManager.Instance.GetMoney(basicMoney * 3);
            moneyText.text = (basicMoney * 3).ToString();
            skillPointText.text = (basicSkillPoint * 3).ToString();
        }

        private void BossReward() // 可分配基準值 20
        {
            SkillManager.Instance.GetSkillPoint(basicSkillPoint * 10);
            DataManager.Instance.GetMoney(basicMoney * 10);
            moneyText.text = (basicMoney * 10).ToString();
            skillPointText.text = (basicSkillPoint * 10).ToString();
        }
    }
}