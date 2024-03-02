using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BattleNew
{
    public class BattleClickable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image image;
        [SerializeField] Text text;
        [SerializeField] BattleClickType type;
        [SerializeField] GameObject selected;

        public GameObject Selected => selected;
        public BattleClickType Type => type;

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (type)
            {
                case BattleClickType.EndTurn:
                    BattleManager.Instance.EnemyTurn();
                    break;
                case BattleClickType:
                    BattleManager.Instance.SelectSkill(this);
                    break;
            }
        }
    }

    public enum BattleClickType
    {
        EndTurn = 0,
        BasicSkill = 1,
        SkillA = 2,
        SkillB = 3,
        Ultimate = 4,
    }
}