using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    Transform canvas;

    void Start()
    {
        DataManager.Instance.LoadConfig();
        canvas = GameObject.Find("Canvas").transform;
        LoadObjects();
        Menu.Instance.gameObject.SetActive(true);
    }

    // 從AB包載入GameObjects
    void LoadObjects()
    {
        SetUIObject("menu", "menu");
        SetUIObject("map", "map");
        SetUIObject("playerui", "playerui");
        SetUIObject("playerui", "eventwindow");
        SetUIObject("playerui", "backpack");
        SetUIObject("shop", "shop");
        SetObject("battlenew", "battlemanager");
        SetUIObject("skilltree", "skilltree");
    }

    // 設置UI類的GameObjects
    void SetUIObject(string abName, string resName)
    {
        GameObject obj = ABManager.Instance.LoadRes<GameObject>(abName, resName);
        obj.transform.SetParent(canvas);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.SetActive(false);
        ABManager.Instance.UnLoad(abName);
    }

    // 設置非UI類的GameObjects
    void SetObject(string abName, string resName)
    {
        GameObject obj = ABManager.Instance.LoadRes<GameObject>(abName, resName);
        obj.GetComponent<Transform>().position = Vector2.zero;
        obj.SetActive(false);
        ABManager.Instance.UnLoad(abName);
    }
}
