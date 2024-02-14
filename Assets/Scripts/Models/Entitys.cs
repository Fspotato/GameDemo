using System;
using UnityEngine;

[Serializable]
public class EntityConfig // 物件實體設定檔
{
    public uint ID; // 唯一ID
    public EntityType Type; // 實體種類
    public SerializableDictionary<EntityKey, int> IntValue;// 存儲所有整數屬性 
    public SerializableDictionary<EntityKey, string> StrValue; // 存儲所有字符串屬性

    public EntityConfig()
    {
        IntValue = new SerializableDictionary<EntityKey, int>();
        StrValue = new SerializableDictionary<EntityKey, string>();
    }

    public EntityConfig DeepCopy()
    {
        string json = JsonUtility.ToJson(this);
        return JsonUtility.FromJson<EntityConfig>(json);
    }
}

[Serializable]
public class Entity // 玩家擁有的實體
{
    public uint ID; // 唯一實體ID 用於辨識實體
    public EntityType Type; // 實體種類
    public int Amount; // 數量
    public SerializableDictionary<EntityKey, int> IntValue; // 存儲所有整數屬性
    public SerializableDictionary<EntityKey, string> StrValue; // 存儲所有字符串屬性
    public SerializableDictionary<EntityType, SerializableList<uint>> Child; // 存儲該實體的子實體 比如裝備被某角色穿戴 (角色就會存儲該裝備的唯一實體ID,該裝備也會指向角色的唯一實體ID)

    public Entity(EntityConfig entityConfig)
    {
        Amount = 1;
        ID = entityConfig.ID;
        Type = entityConfig.Type;
        IntValue = entityConfig.IntValue;
        StrValue = entityConfig.StrValue;
    }
}

[Serializable]
public enum EntityKey // 屬性索引枚舉
{
    MetaID = 0,
    Name = 1,
    Lvl = 2,
    Hp = 3,
    Mp = 4,
    Atk = 5,
    Def = 6,
    Description = 7,
    Speical = 8,
    CanLearn = 9,
    Amount = 10,
}

[Serializable]
public enum EntityType // 實體種類枚舉
{
    Player = 1,
    Weapon = 2,
    Skill = 3,
    Item = 4,
}