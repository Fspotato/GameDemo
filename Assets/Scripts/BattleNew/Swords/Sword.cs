using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BattleNew
{
    public class Sword : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
    {
        [SerializeField] SwordType type;
        Canvas canvas;
        CanvasGroup cg;
        RectTransform rect;

        GameObject copy;
        bool isCopy;

        public SwordType Type => type;

        // 初始化
        public void Init()
        {
            if (isCopy) return;
            cg = GetComponent<CanvasGroup>();
            rect = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();

            CreateCopy();
        }

        // 設置複製體 殘影顯示和復位用
        private void CreateCopy()
        {
            copy = Instantiate(gameObject, canvas.transform);
            copy.GetComponent<Sword>().isCopy = true;
            copy.transform.localPosition = transform.localPosition;
            Color color = GetComponent<Image>().color;
            copy.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.3f);
            copy.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isCopy) return;
            copy.transform.localPosition = transform.localPosition;
            copy.SetActive(true);
            cg.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData data)
        {
            if (isCopy) return;
            rect.anchoredPosition += data.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isCopy) return;
            cg.blocksRaycasts = true;
            transform.localPosition = copy.transform.localPosition;
            copy.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isCopy) return;
            Sworder.Instance.SelectSword(this);
        }

        // 劍丸效果
        public virtual void Effect(Enemy enemy) { }

        // 多敵人重載
        public virtual void Effect(Enemy enemy, List<GameObject> enemies) { }

        void OnDestroy()
        {
            Destroy(copy);
        }
    }

    [System.Serializable]
    public enum SwordType
    {
        BasicSword = 0,
        BloodSword = 1,
    }
}

