using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : BaseManager<BattleManager>
{
    [SerializeField] GameObject[] enemies = new GameObject[4];
    [SerializeField] List<EnemyConfigs> configs = new List<EnemyConfigs>();
    [SerializeField] BattleUI ui;

    GameObject playerObj;
    GameObject aim;
    Player player;

    BattleType type;
    EnemyType[] enemyTypes;

    int selectedIndex = 0;
    bool bossExist;
    readonly float downOffset = 1f; // 角色和怪物物件位置向下偏移值
    readonly float spacing = 0.5f; // 怪物物件位置之間的間距

    // 加載AB包和設置實例 不這樣用 待修正 已修正怪物
    void Awake()
    {
        if (instance == null) instance = this;
        playerObj = ABManager.Instance.LoadRes<GameObject>("battle", "player");
        playerObj.transform.position = new Vector2(-4f, playerObj.GetComponent<Renderer>().bounds.size.y / 2 - downOffset);
        playerObj.transform.SetParent(transform);
        player = playerObj.GetComponent<Player>();

        aim = ABManager.Instance.LoadRes<GameObject>("battle", "aim");
        aim.transform.SetParent(transform);
    }

    void Update()
    {
        Select();
    }

    // 戰鬥開始
    public void BattleStart(BattleType type, params EnemyType[] enemyTypes)
    {
        gameObject.SetActive(true);
        ui.BattleStart();

        playerObj.SetActive(true);
        player.LoadData();

        if (enemyTypes.Length != 0) this.enemyTypes = enemyTypes;
        else enemyTypes = null;

        this.type = type;
        SetEnemies();

        selectedIndex = 0;
        Aim();
    }

    // 戰鬥結束
    public void BattleEnd(bool winLose)
    {
        for (int i = 0; i < enemies.Length; i++) if (enemies[i] != null) Destroy(enemies[i]);
        if (winLose)
        {
            PlayerUI.Instance.gameObject.SetActive(true);
            MapManager.Instance.gameObject.SetActive(true);
            DataManager.Instance.SetPlayerHp((int)player.GetComponent<Player>().Hp);
        }
        gameObject.SetActive(false);
    }

    // 按條件抽選隨機設定檔
    private List<EnemyConfigs> SetConfigs()
    {
        if (type == BattleType.Boss && bossExist == false) return configs.FindAll(c => c.Type == EnemyType.Boss);
        if (enemyTypes != null) return configs.FindAll(c => enemyTypes.Contains(c.Type));
        return configs.FindAll(c => c.Type != EnemyType.Boss);
    }

    // 怪物設置
    private void SetEnemies()
    {
        int rnd = Random.Range(2, 4);
        bossExist = false;

        for (int i = 0; i < rnd; i++)
        {
            List<EnemyConfigs> tempConfigs = SetConfigs();
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
        }

        if (enemies[0].GetComponent<Enemy>().IsBoss)
        {
            GameObject temp = enemies[0];
            enemies[0] = enemies[1];
            enemies[1] = temp;
        }

        float offset = 0f;
        for (int i = 0; i < rnd; i++)
        {
            Renderer er = enemies[i].GetComponent<Renderer>();
            enemies[i].transform.position = new Vector2(er.bounds.size.x / 2 + offset, er.bounds.size.y / 2 - downOffset);
            enemies[i].GetComponent<Enemy>().ShowData();
            offset += er.bounds.size.x + spacing;
        }
        offset = 4.2375f - enemies[1].transform.position.x;
        for (int i = 0; i < rnd; i++)
        {
            enemies[i].transform.position += new Vector3(offset, 0f);
            enemies[i].GetComponent<Enemy>().ShowData();
        }
    }

    // 選擇瞄準
    private void Select()
    {
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

    // 瞄準敵人
    private void Aim()
    {
        if (selectedIndex >= enemies.Length) return;
        aim.transform.position = enemies[selectedIndex].transform.position;
    }

    // 自動選擇 (當有敵人被擊殺時觸發)
    private IEnumerator Choose()
    {
        yield return new WaitForSeconds(0.016f);
        bool isEmpty = true;
        if (enemies[selectedIndex].activeSelf == false)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == null) continue;
                if (enemies[i].activeSelf == true)
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

    // 角色攻擊
    public void PlayerAttack(string arrange)
    {
        // 根據組合使用記錄下的攻擊
        player.UseSkill(arrange, enemies[selectedIndex].GetComponent<Enemy>(), enemies);
        StartCoroutine(Choose());
    }

    // 敵人回合
    public void EnemyTurn()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) continue;
            if (enemies[i].activeSelf == true)
            {
                Enemy enemy = enemies[i].GetComponent<Enemy>();

                enemy.BuffCheckBeforeAttack();
                StartCoroutine(Choose());
                if (enemy.gameObject.activeSelf == false) continue;

                enemy.EnemyTurn(player);
                print($"角色剩餘血量{player.Hp}點");

                enemy.Buffs.RoundOver();
            }
        }

        player.BuffCheckBeforeAttack();

        player.Buffs.RoundOver();

        if (playerObj.activeSelf == false) BattleEnd(false); ;
    }

    #region 尋找敵人

    private int FindNormalEnemy(EnemyConfigs config)
    {
        int rnd;
        do rnd = Random.Range(0, config.Enemies.Count);
        while (config.Enemies[rnd].GetComponent<Enemy>().IsElite == true || config.Enemies[rnd].GetComponent<Enemy>().IsBoss == true);
        return rnd;
    }

    private int FindEliteEnemy(EnemyConfigs config)
    {
        int rnd;
        do rnd = Random.Range(0, config.Enemies.Count);
        while (config.Enemies[rnd].GetComponent<Enemy>().IsElite != true);
        return rnd;
    }

    private int FindEnemy(EnemyConfigs config)
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
