using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BattleNew
{
    public class Sworder : Player
    {
        [SerializeField] List<GameObject> swordPrefabs = new List<GameObject>();
        public List<GameObject> swords = new List<GameObject>();
        public List<SwordSoul> swordSouls = new List<SwordSoul>();

        [SerializeField] Sword selectedSword;

        static Sworder instance;
        public static Sworder Instance => instance;

        void Awake()
        {
            if (instance == null) instance = this;
        }

        // 執行順序 ItemCheck > FirstTurn > StartTurn (可參照 Player.LoadData())
        // 道具檢查
        public override void ItemCheck()
        {
            base.ItemCheck();
            swordSouls.Clear(); swords.Clear(); selectedSword = null;
            if (DataManager.Instance.CheckItemExist("基礎劍胎")) swordSouls.Add(new SwordSoul(SwordType.BasicSword, 1));
            if (DataManager.Instance.CheckItemExist("血劍")) swordSouls.Add(new SwordSoul(SwordType.BloodSword, 2));
        }

        // 首回合特殊動作
        public override void FirstTurn()
        {
            base.FirstTurn();
            SelectSword();
        }

        // 回合開始
        public override void StartTurn()
        {
            base.StartTurn();
            foreach (var swordSoul in swordSouls)
            {
                swordSoul.StartTurn((check) =>
                    {
                        if (!check) return;
                        SpawnSword(swordSoul.Type);
                    }
                );
            }
        }

        // 生成劍
        public void SpawnSword(SwordType type)
        {
            switch (type)
            {
                case SwordType.BasicSword: SpawnSword(type, 2); break;
                case SwordType.BloodSword: SpawnSword(type, 1); break;
            }
            SelectSword();
        }

        // 生成劍
        private void SpawnSword(SwordType type, int times)
        {
            for (int i = 0; i < times; i++)
            {
                GameObject sword = Instantiate(swordPrefabs.FirstOrDefault(s => s.GetComponent<Sword>().Type == type), canvas.transform);
                sword.GetComponent<Sword>().Init();
                swords.Add(sword);
                SworderUI.Instance.GetComponent<SworderUI>().GetSwords(swords);
            }
        }

        // 技能組
        public override void UseSkill(uint id, Enemy enemy, List<GameObject> enemies)
        {
            switch (id)
            {
                case 30001:
                    if (selectedSword == null) return;
                    selectedSword.Effect(enemy, enemies);
                    SelectSword();
                    break;
                default:
                    break;
            }
            if (gameObject.activeInHierarchy) base.UseSkill(id, enemy, enemies);
        }

        // 選擇目前的劍 供外部調用
        public void SelectSword(Sword sword)
        {
            selectedSword = sword;
        }

        // 自動選擇
        public void SelectSword()
        {
            if (selectedSword != null && ReferenceEquals(selectedSword.gameObject, Missing.Value)) return;
            for (int i = 0; i < swords.Count; i++)
            {
                if (swords == null) continue;
                selectedSword = swords[i].GetComponent<Sword>();
                return;
            }
        }
    }
}