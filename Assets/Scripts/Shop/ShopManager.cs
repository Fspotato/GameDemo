using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : BaseManager<ShopManager>
{
    [SerializeField] Text moneyText;
    [SerializeField] GameObject shopItemPrefab;
    [SerializeField] List<ShopConfig> configs;


    GameObject itemParent;
    public List<ShopItem> items = new List<ShopItem>();
    public List<ItemObject> itemObjs = new List<ItemObject>();

    #region 開啟商店相關

    // 開啟商店 (0:預設商店)
    public void OpenShop(ShopType type)
    {
        gameObject.SetActive(true);

        CreateItemParent();

        Refresh();

        SetShopItems(type);
    }

    // 設置商品
    void SetShopItems(ShopType type)
    {
        items = configs[(int)type].DeepCopy();
        items.Shuffle();

        for (int i = 0; i < 6; i++)
        {
            GameObject obj = Instantiate(shopItemPrefab, itemParent.transform);
            ItemObject itemObj = obj.GetComponent<ItemObject>();
            itemObj.Item = items[i];
            itemObj.Text.text = DataManager.Instance.GetEntityNameById(items[i].id);
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-500f + i % 3 * 300f, 165f - i / 3 * 365f);
            itemObjs.Add(itemObj);
        }
    }

    // 清除/創建商品母物件(方便刷新
    void CreateItemParent()
    {
        if (itemParent != null) Destroy(itemParent);
        itemParent = new GameObject("ShopItems");
        itemParent.transform.SetParent(transform);
        itemParent.AddComponent<RectTransform>().anchoredPosition = Vector2.zero;
        items.Clear();
    }

    #endregion

    #region 商品買賣相關

    public void BuyItem(ItemObject item)
    {
        if (DataManager.Instance.CheckMoney(item.Item.price))
        {
            DataManager.Instance.SpendMoney(item.Item.price);
            DataManager.Instance.GetEntity(item.Item.id);
            Refresh();
            item.transform.Find("BuyButton").gameObject.SetActive(false);
            item.transform.Find("Image").GetComponent<Image>().color = Color.gray;
            item.transform.Find("ItemName").GetComponent<Text>().text += "\n\n售出!";
        }
        else
        {
            print("你沒有足夠的金幣購買這項物品!");
        }
    }

    #endregion

    // 離開商店
    public void CloseShop()
    {
        PlayerUI.Instance.gameObject.SetActive(true);
        MapManager.Instance.gameObject.SetActive(true);
        MapManager.Instance.EnterNode();
        gameObject.SetActive(false);
    }

    // 刷新頁面
    public void Refresh()
    {
        moneyText.text = "$" + DataManager.Instance.ShowMoney().ToString();
    }
}

public enum ShopType
{
    DefaultShop = 0,
}
