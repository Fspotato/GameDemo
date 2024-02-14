using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : BaseManager<Backpack>
{
    [SerializeField] Text itemNameText;
    [SerializeField] Text itemDescriptionText;
    [SerializeField] Text moneyText;
    [SerializeField] GameObject content;
    [SerializeField] GameObject itemPrefab;

    GameObject itemParent;

    List<GameObject> itemObjs = new List<GameObject>();

    // 展示背包
    public void ShowBackpack()
    {
        gameObject.SetActive(true);

        CreateItemParent();

        ShowItems();

        ShowMoney();
    }

    // 展示道具
    public void ShowItem(uint id)
    {
        itemNameText.text = DataManager.Instance.GetEntityNameById(id);
        itemDescriptionText.text = DataManager.Instance.GetEntityDescriptionByID(id);
    }

    // 展示金錢
    private void ShowMoney()
    {
        moneyText.text = $"${DataManager.Instance.ShowMoney()}";
    }

    // 展示物件
    private void ShowItems()
    {
        int count = -1;
        foreach (var item in DataManager.Instance.backpack)
        {
            Entity e = item.Value;
            if (e.Type != EntityType.Item) continue; // 非道具類不顯示
            if (e.ID == 20001) continue; // 金錢額外顯示

            count++;
            GameObject itemObj = Instantiate(itemPrefab, itemParent.transform);
            itemObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200f + 275f * (count % 5), -200f - 275f * (count / 5));


            BackpackItem bItem = itemObj.GetComponent<BackpackItem>();
            bItem.ID = e.ID;
            bItem.ItemName.text = e.StrValue[EntityKey.Name];

            itemObjs.Add(itemObj);
        }

        content.GetComponent<RectTransform>().sizeDelta = new Vector2(1400f, 300f * (1 + count / 5));
    }

    // 創建道具欄母物件 方便刷新
    private void CreateItemParent()
    {
        if (itemParent != null) Destroy(itemParent);
        itemParent = new GameObject("Items");
        itemParent.transform.SetParent(content.transform);

        RectTransform parentRect = itemParent.AddComponent<RectTransform>();
        parentRect.anchorMin = new Vector2(0f, 1f);
        parentRect.anchorMax = new Vector2(0f, 1f);
        parentRect.anchoredPosition = Vector2.zero;

        itemObjs.Clear();
    }

    // 離開背包
    public void Exit()
    {
        gameObject.SetActive(false);
    }
}
