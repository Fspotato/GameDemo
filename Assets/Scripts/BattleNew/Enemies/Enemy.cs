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
        [SerializeField] protected EnemyType type;

        public string Name => enemyName;
        public bool IsBoss => isBoss;
        public bool IsElite => isElite;
        public EnemyType Type => type;

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
            print($"{Name} 發動了攻擊!");
            BuffManager.RoundOver();
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
            if (info == null) info = ABManager.Instance.LoadRes<GameObject>("battlenew", "enemyinfo");
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
            Vector2 objOnCanvasPosition = Camera.main.WorldToScreenPoint(objPosition);
            info.GetComponent<RectTransform>().anchoredPosition = new Vector2(objOnCanvasPosition.x, objOnCanvasPosition.y - 150f);
        }

        public void OnDrop(PointerEventData data)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
            if (data.pointerDrag.GetComponent<Sword>() != null)
            {
                Sworder.Instance.SelectSword(data.pointerDrag.GetComponent<Sword>());
                Sworder.Instance.UseSkill(30001, this, BattleManager.Instance.GetEnemies());
            }
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

        #endregion

        private void OnDestroy()
        {
            Destroy(info);
        }
    }
}