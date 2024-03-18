using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BattleNew
{
    public class SworderUI : BattleUI
    {
        [SerializeField] List<GameObject> swords = new List<GameObject>();

        public override void BattleReslove()
        {
            for (int i = 0; i < swords.Count; i++) Destroy(swords[i]);
            swords.Clear();
            base.BattleReslove();
        }

        // 同步Sworder的劍丸池
        public void GetSwords(List<GameObject> swords)
        {
            this.swords = swords;
            RefreshSwords();
        }

        // 刷新劍丸顯示
        public void RefreshSwords()
        {
            if (swords.Count == 0) return;
            for (int i = 0; i < swords.Count; i++)
            {
                swords[i].transform.localPosition = new Vector2(-95f + i * 125f, -410f);
            }
        }

        // 使用後移除劍丸
        public void RemoveSword(Sword sword)
        {
            swords.Remove(sword.gameObject);
            Destroy(sword.gameObject);
            Sworder.Instance.SelectSword();
            RefreshSwords();
        }
    }
}