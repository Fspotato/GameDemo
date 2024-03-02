using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleNew
{
    public class BattleManager : BaseManager<BattleManager>
    {
        [SerializeField] GameObject[] enemies = new GameObject[4];
        [SerializeField] List<EnemyConfig> configs = new List<EnemyConfig>();
        [SerializeField] BattleUI ui;

        GameObject playerObj;
        GameObject aim;
        Player player;

        BattleType type;
        EnemyType[] enemyTypes;

        int selectedIndex = 0;
        BattleClickable selectedSkill;

        readonly float downOffset = 0.5f; // 角色和怪物物件位置向下偏移值
        readonly float spacing = 0.5f; // 怪物物件位置之間的間距

        bool bossExist;
        bool initialized = false;
        bool isEnd = false;


        void Update()
        {
            SelectSkill();
            InputHandle();
        }

        // 初始化 瞄準點的Active被設置為false了 如果要用到記得改掉
        private void Initialize()
        {
            if (initialized) return;
            initialized = true;
            switch (DataManager.Instance.GetPlayerClass())
            {
                case "執劍者": playerObj = ABManager.Instance.LoadRes<GameObject>("battlenew", "sworder"); break;
            }

            playerObj.transform.SetParent(transform);
            playerObj.transform.position = new Vector2(-4f, playerObj.GetComponent<Renderer>().bounds.size.y / 2 - downOffset);
            player = playerObj.GetComponent<Player>();

            aim = ABManager.Instance.LoadRes<GameObject>("battlenew", "aim");
            aim.SetActive(false);
            aim.transform.SetParent(transform);
        }

        // 戰鬥開始
        public void BattleStart(BattleType type, params EnemyType[] enemyTypes)
        {
            if (!initialized) Initialize();
            isEnd = false;

            gameObject.SetActive(true);
            ui.BattleStart();

            playerObj.SetActive(true);
            player.LoadData(GetComponentInChildren<Canvas>());

            if (enemyTypes.Length != 0) this.enemyTypes = enemyTypes;
            else enemyTypes = null;

            this.type = type;
            SetEnemies();

            selectedIndex = 0;
            Aim();

            selectedSkill = null;
        }

        // 戰鬥結束
        public void BattleEnd(bool winLose)
        {
            isEnd = true;
            for (int i = 0; i < enemies.Length; i++) if (enemies[i] != null) Destroy(enemies[i]);
            ui.BattleEnd();

            if (winLose)
            {
                PlayerUI.Instance.gameObject.SetActive(true);
                MapManager.Instance.gameObject.SetActive(true);
                MapManager.Instance.EnterNode();
                DataManager.Instance.SetPlayerHp((int)player.GetComponent<Player>().Hp);
            }

            gameObject.SetActive(false);
        }

        // 按條件抽選隨機設定檔
        private List<EnemyConfig> SetConfigs()
        {
            List<EnemyConfig> config = new List<EnemyConfig>();
            if (type == BattleType.Boss && bossExist == false) config = configs.FindAll(c => c.Type == EnemyType.Boss);
            if (enemyTypes != null && config.Count == 0) config = configs.FindAll(c => enemyTypes.Contains(c.Type));
            if (config.Count == 0) config = configs.FindAll(c => c.Type != EnemyType.Boss);
            return config;
        }

        // 怪物設置
        private void SetEnemies()
        {
            bossExist = false;

            int rnd = Random.Range(1, 3);

            for (int i = 0; i < rnd; i++)
            {
                List<EnemyConfig> tempConfigs = SetConfigs();
                int cRnd = Random.Range(0, tempConfigs.Count);

                int eRnd;
                switch (type)
                {
                    case BattleType.Normal: eRnd = FindNormalEnemy(tempConfigs[cRnd]); break;
                    case BattleType.EliteOnly: eRnd = FindEliteEnemy(tempConfigs[cRnd]); break;
                    case BattleType.Boss:
                        eRnd = bossExist ? FindNormalEnemy(tempConfigs[cRnd]) : FindEnemy(tempConfigs[cRnd]);
                        bossExist = true;
                        break;
                    default: eRnd = FindEnemy(tempConfigs[cRnd]); break;
                }

                enemies[i] = Instantiate(tempConfigs[cRnd].Enemies[eRnd], transform);
                enemies[i].GetComponent<Enemy>().LevelCoefCheck();
            }

            // 交換Boss位置 如果Boss在第一位
            if (enemies[1] != null && enemies[0].GetComponent<Enemy>().IsBoss)
            {
                GameObject temp = enemies[0];
                enemies[0] = enemies[1];
                enemies[1] = temp;
            }

            // 設置初始位置
            float offset = 0f;
            for (int i = 0; i < rnd; i++)
            {
                Renderer er = enemies[i].GetComponent<Renderer>();
                enemies[i].transform.position = new Vector2(er.bounds.size.x / 2 + offset, er.bounds.size.y / 2 - downOffset);
                offset += er.bounds.size.x + spacing;
            }

            // 計算位移 如果只有一個敵人
            if (enemies[1] == null)
            {
                enemies[0].transform.position = new Vector2(4.2375f, enemies[0].transform.position.y);
                enemies[0].GetComponent<Enemy>().ShowData();
                return;
            }

            // 計算位移
            offset = 4.2375f - enemies[1].transform.position.x;
            for (int i = 0; i < rnd; i++)
            {
                enemies[i].transform.position += new Vector3(offset, 0f);
                enemies[i].GetComponent<Enemy>().ShowData();
            }
        }

        // 選擇瞄準 這裡被return了 不會有任何效果 如果之後要用記得弄掉
        private void Select()
        {
            return;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectedIndex > 0)
                {
                    selectedIndex--;
                    while (enemies[selectedIndex].activeSelf == false)
                    {
                        selectedIndex++;
                        if (selectedIndex >= enemies.Length) return;
                    }
                    Aim();
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectedIndex < enemies.Length - 2)
                {
                    selectedIndex++;
                    while (enemies[selectedIndex].activeSelf == false)
                    {
                        selectedIndex--;
                        if (selectedIndex < 0) return;
                    }
                    Aim();
                }
            }
        }

        // 瞄準敵人 這裡被return了 不會有任何效果 如果之後要用記得弄掉
        private void Aim()
        {
            return;
            if (selectedIndex >= enemies.Length) return;
            aim.transform.position = enemies[selectedIndex].transform.position + new Vector3(0, enemies[selectedIndex].GetComponent<Renderer>().bounds.size.y / 2 + 0.5f, 0);
        }

        // 檢查當前敵人死否死亡
        public void EnemyCheck()
        {
            bool isEmpty = true;
            if (enemies[selectedIndex].GetComponent<Enemy>().IsDead)
            {
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i] == null) continue;
                    if (enemies[i].GetComponent<Enemy>().IsDead == false)
                    {
                        isEmpty = false;
                        selectedIndex = i;
                        Aim();
                        break;
                    }
                }
            }
            else isEmpty = false;
            if (isEmpty) BattleEnd(true);
        }

        // 取得所有存活敵人資訊
        public List<GameObject> GetEnemies()
        {
            List<GameObject> tempEnemies = new List<GameObject>();
            foreach (var enemy in enemies)
            {
                if (enemy == null || enemy.GetComponent<Enemy>().IsDead) continue;
                tempEnemies.Add(enemy);
            }
            return tempEnemies;
        }

        // 取得當前目標敵人資訊
        public Enemy GetEnemy()
        {
            return enemies[selectedIndex].GetComponent<Enemy>();
        }

        // 選擇技能
        public void SelectSkill(BattleClickable bc)
        {
            if (bc.Equals(selectedSkill))
            {
                switch (bc.Type)
                {
                    case BattleClickType.BasicSkill: UseSkill(SkillType.BasicSkill); break;
                    case BattleClickType.SkillA: UseSkill(SkillType.SkillA); break;
                    case BattleClickType.SkillB: UseSkill(SkillType.SkillB); break;
                    case BattleClickType.Ultimate: UseSkill(SkillType.Ultimate); break;
                    default: break;
                }
            }
            else
            {
                if (selectedSkill != null) selectedSkill.Selected.SetActive(false);
                selectedSkill = bc;
                selectedSkill.Selected.SetActive(true);
                ui.ShowSkill(bc.Type);
            }
        }

        // 快捷鍵選擇技能
        private void SelectSkill()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SelectSkill(ui.transform.Find("BasicSkillButton").GetComponent<BattleClickable>());
            }
        }

        // 使用技能
        public void UseSkill(SkillType type)
        {
            Skill skill = SkillManager.Instance.GetEquipedSkillByType(type);
            if (skill == default) return;
            player.UseSkill(skill.id, GetEnemy(), GetEnemies());
            selectedSkill.Selected.SetActive(false);
            selectedSkill = null;
            ui.ResetSkillWindow();
        }

        // 敵人回合
        public void EnemyTurn()
        {
            if (isEnd) return;
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null || enemies[i].GetComponent<Enemy>().IsDead) continue;

                Enemy enemy = enemies[i].GetComponent<Enemy>();
                enemy.StartTurn(player);
                print($"角色剩餘血量{player.Hp}點");
            }

            if (player.IsDead) BattleEnd(false);

            StartTurn();
        }

        // 我方回合開始
        public void StartTurn()
        {
            if (isEnd) return;
            player.StartTurn();
        }

        // 治癒角色
        public void HealPlayer(float value)
        {
            player.GetComponent<Player>().Heal(value);
        }

        private void InputHandle()
        {
            if (Input.GetKeyDown(KeyCode.D)) EnemyTurn();
        }

        #region 尋找敵人

        private int FindNormalEnemy(EnemyConfig config)
        {
            int rnd;
            if (config.Enemies.FirstOrDefault(e => e.GetComponent<Enemy>().IsElite == false && e.GetComponent<Enemy>().IsBoss == false) == default) return FindEnemy(config);
            do rnd = Random.Range(0, config.Enemies.Count);
            while (config.Enemies[rnd].GetComponent<Enemy>().IsElite == true || config.Enemies[rnd].GetComponent<Enemy>().IsBoss == true);
            return rnd;
        }

        private int FindEliteEnemy(EnemyConfig config)
        {
            int rnd;
            if (config.Enemies.FirstOrDefault(e => e.GetComponent<Enemy>().IsElite) == default) return FindEnemy(config);
            do rnd = Random.Range(0, config.Enemies.Count);
            while (config.Enemies[rnd].GetComponent<Enemy>().IsElite != true);
            return rnd;
        }

        private int FindEnemy(EnemyConfig config)
        {
            return Random.Range(0, config.Enemies.Count);
        }

        #endregion

    }

    public enum BattleType
    {
        Normal = 0,
        Elite = 1,
        EliteOnly = 2,
        Boss = 3,
    }
}