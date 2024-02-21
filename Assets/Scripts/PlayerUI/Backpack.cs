using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    BackpackItem currentItem;
    int selectedIndex = -1;


    List<GameObject> itemObjs = new List<GameObject>();

    void Update()
    {
        Select();
    }

    // 展示背包
    public void ShowBackpack()
    {
        gameObject.SetActive(true);

        CreateItemParent();

        ShowItems();

        if (selectedIndex == -1) selectedIndex = 0;

        if (itemObjs.Count != 0) ShowItem();

        ShowMoney();
    }

    // 展示道具
    public void ShowItem(BackpackItem item)
    {
        if (currentItem != null) currentItem.selected.SetActive(false);

        item.selected.SetActive(true);
        itemNameText.text = DataManager.Instance.GetEntityNameById(item.ID);
        itemDescriptionText.text = DataManager.Instance.GetEntityDescriptionByID(item.ID);

        selectedIndex = itemObjs.IndexOf(itemObjs.FirstOrDefault(i => i.Equals(item.gameObject)));
        currentItem = item;
    }

    // 展示道具 根據index重載
    private void ShowItem()
    {
        if (selectedIndex > itemObjs.Count - 1) return;
        ShowItem(itemObjs[selectedIndex].GetComponent<BackpackItem>());
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
            bItem.ItemCount.text = e.Amount.ToString();

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

    // 選擇道具
    private void Select()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex++;
            if (selectedIndex >= itemObjs.Count) selectedIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex -= 5;
            if (selectedIndex < 0) selectedIndex += 5;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex += 5;
            if (selectedIndex >= itemObjs.Count) selectedIndex -= 5;
        }
        if (selectedIndex <= itemObjs.Count - 1) ShowItem();
    }

    // 離開背包
    public void Exit()
    {
        gameObject.SetActive(false);
    }
}
