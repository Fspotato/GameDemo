using System.IO;
using System.Linq;
using UnityEngine;
using BattleNew;


public class DataManager : BaseManager<DataManager>
{
    // 目前只做了遊戲進行中的背包和裝備技能 全局持有的稱號功能待補完
    // 遊戲設定檔和背包 (最重要的檔案) 基本上整個遊戲中都會使用
    public SerializableDictionary<uint, EntityConfig> entityConfigs;
    public SerializableDictionary<uint, Entity> backpack;
    public SerializableDictionary<uint, Buff> buffConfigs;

    // 讀取設定檔
    public void LoadConfig()
    {
        TextAsset ab = ABManager.Instance.LoadRes<TextAsset>("config", "EntityConfig.json");
        string json = ab.text;
        entityConfigs = JsonUtility.FromJson<SerializableDictionary<uint, EntityConfig>>(json);
        ab = ABManager.Instance.LoadRes<TextAsset>("config", "BuffConfig.json");
        json = ab.text;
        buffConfigs = JsonUtility.FromJson<SerializableDictionary<uint, Buff>>(json);
        ABManager.Instance.UnLoad("config");
    }

    // 存檔功能
    private void SaveData()
    {
        string json = JsonUtility.ToJson(backpack);
        File.WriteAllText(Application.persistentDataPath + "/backpack.json", json);

    }

    public void SaveAllData()
    {
        SaveData();
        SkillManager.Instance.SaveSkillTree();
        MapManager.Instance.SaveMap();
        LevelManager.Instance.SaveInfo();
    }

    // 讀檔功能
    // 待完善 目前只補了背包
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/backpack.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/backpack.json");
            backpack = JsonUtility.FromJson<SerializableDictionary<uint, Entity>>(json);
        }
        else
        {
            print("Can't find backpack.json in persistentDataPath");
        }
    }

    // 開啟新遊戲 根據選擇的職業創建角色
    // 待完善 目前只初始化了職業
    public void NewGame(ClassType type)
    {
        backpack = new SerializableDictionary<uint, Entity>();
        switch (type)
        {
            case ClassType.Sworder:
                GetEntity(10001);
                break;
        }
        GetMoney(500);
    }

    #region 道具管理

    // 得到道具(金錢、遺物、職業) 放入backpack
    public void GetEntity(uint id)
    {
        if (backpack.ContainsKey(id))
        {
            backpack[id].Amount++;
        }
        else
        {
            if (!entityConfigs.ContainsKey(id)) return;
            Entity e = new Entity(entityConfigs[id].DeepCopy());
            backpack.Add(id, e);
        }
    }

    // 得到道具 名稱重載
    public void GetEntity(string name)
    {
        var temp = entityConfigs.FirstOrDefault(e => e.Value.StrValue[EntityKey.Name] == name).Value;
        if (temp == null) return;
        GetEntity(temp.ID);
    }

    // 得到多個道具重載
    public void GetEntity(uint id, int amount)
    {
        if (backpack.ContainsKey(id))
        {
            backpack[id].Amount += amount;
        }
        else
        {
            Entity e = new Entity(entityConfigs[id].DeepCopy())
            {
                Amount = amount
            };
            backpack.Add(id, e);
        }
    }

    // 檢查是否有某道具
    public bool CheckItemExist(uint id)
    {
        return backpack.ContainsKey(id);
    }

    // 檢查是否有某道具 名稱重載
    public bool CheckItemExist(string name)
    {
        uint id = backpack.FirstOrDefault(e => e.Value.StrValue[EntityKey.Name] == name).Key;
        return id != default;
    }

    #endregion

    #region 金幣管理

    // 檢查是否有足夠多的金幣
    public bool CheckMoney(int amount)
    {
        if (!backpack.ContainsKey(20001)) return false;
        return backpack[20001].Amount >= amount;
    }

    // 獲得金幣
    public void GetMoney(int amount)
    {
        GetEntity(20001, amount);
    }

    // 花費金幣
    public void SpendMoney(int amount)
    {
        GetEntity(20001, -amount);
    }

    // 展示目前有多少金幣
    public int ShowMoney()
    {
        if (!backpack.ContainsKey(20001)) return 0;
        return backpack[20001].Amount;
    }

    #endregion

    #region 角色相關

    // 讀取職業別
    public string GetPlayerClass()
    {
        return backpack.FirstOrDefault(e => e.Value.Type == EntityType.Player).Value.StrValue[EntityKey.Name];
    }

    // 讀取目前角色剩餘血量
    public int GetPlayerHp()
    {
        if (backpack == null) return 0;
        return backpack.FirstOrDefault(e => e.Value.Type == EntityType.Player).Value.IntValue[EntityKey.Hp];
    }

    // 讀取目前角色攻擊力
    public int GetPlayerAttack()
    {
        if (backpack == null) return 0;
        return backpack.FirstOrDefault(e => e.Value.Type == EntityType.Player).Value.IntValue[EntityKey.Attack];
    }

    // 設置目前角色剩餘血量
    public void SetPlayerHp(int hp)
    {
        if (backpack == null) return;
        if (hp >= GetPlayerMaxHp()) hp = GetPlayerMaxHp();
        backpack.FirstOrDefault(e => e.Value.Type == EntityType.Player).Value.IntValue[EntityKey.Hp] = hp;
    }

    // 得到角色最大生命值
    public int GetPlayerMaxHp()
    {
        if (backpack == null) return 0;
        return entityConfigs.FirstOrDefault(e => e.Value.Type == EntityType.Player).Value.IntValue[EntityKey.Hp];
    }

    #endregion

    #region 設定檔讀取

    // 得到道具名稱
    public string GetEntityNameById(uint id)
    {
        if (!entityConfigs.ContainsKey(id)) return "找不到道具";
        return entityConfigs[id].StrValue[EntityKey.Name];
    }

    // 得到道具描述
    public string GetEntityDescriptionByID(uint id)
    {
        if (!entityConfigs.ContainsKey(id)) return "找不到道具";
        return entityConfigs[id].StrValue[EntityKey.Description];
    }

    // 獲得Buff
    public Buff GetBuffById(uint id)
    {
        return buffConfigs[id].DeepCopy();
    }

    #endregion

    private void OnApplicationQuit()
    {
        SaveData();
    }
}