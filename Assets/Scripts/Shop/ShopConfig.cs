using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShopConfig : ScriptableObject
{
    public SerializableList<ShopItem> shopItems;

    public List<ShopItem> DeepCopy()
    {
        string json = JsonUtility.ToJson(shopItems);
        return JsonUtility.FromJson<SerializableList<ShopItem>>(json);
    }
}
