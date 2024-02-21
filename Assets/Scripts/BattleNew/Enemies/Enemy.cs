using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BattleNew
{
    public class Enemy : BattleObject, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        [SerializeField] protected string enemyName;
        [SerializeField] protected bool isBoss;
        [SerializeField] protected bool isElite;

        public string Name => enemyName;
        public bool IsBoss => isBoss;
        public bool IsElite => isElite;

        GameObject info;

        // 受到傷害
        public override void TakeDamage(float value)
        {
            base.TakeDamage(value);
            BattleManager.Instance.EnemyCheck();
            ShowData();
        }

        // 行動 每個子類需要自己寫行動機制
        public virtual void StartTurn(Player player)
        {
            BuffCheckBeforeAttack();
            if (isDead) return;
            player.TakeDamage(attack);
            Buffs.RoundOver();
        }

        // 恢復血量
        public override void Heal(float value)
        {
            base.Heal(value);
            ShowData();
        }

        // 顯示怪物資訊
        public void ShowData()
        {
            if (info == null) info = ABManager.Instance.LoadRes<GameObject>("battle", "enemyinfo");
            else info.SetActive(true);

            if (!gameObject.activeSelf)
            {
                info.SetActive(false);
                return;
            }

            info.transform.Find("EnemyName").GetComponent<Text>().text = Name;
            info.transform.Find("EnemyHp").GetComponent<Text>().text = ((int)Hp).ToString() + "/" + ((int)MaxHp).ToString();
            info.transform.SetParent(GameObject.Find("Canvas").transform);
            Vector2 objPosition = new Vector2(transform.position.x, transform.position.y - transform.GetComponent<Renderer>().bounds.size.y / 2 + 0.75f);
            Vector2 cameraPosition = Camera.main.WorldToScreenPoint(objPosition);
            info.GetComponent<RectTransform>().anchoredPosition = new Vector2(cameraPosition.x, cameraPosition.y - 150f);
        }

        public void OnDrop(PointerEventData data)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            data.pointerDrag.GetComponent<Sword>().Effect(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

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
                        print($"{Name} 受到了 {buff.Value * buff.Stack} 點 燃燒傷害!");
                        break;
                    case BuffType.Forzen:
                        print($"{Name} 受到了 {buff.Value * buff.Stack} 點 寒冷傷害!");
                        break;
                }
            }
        }

        #endregion

        private void OnDestroy()
        {
            Destroy(info);
        }
    }
}