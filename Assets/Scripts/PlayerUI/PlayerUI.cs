using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : BaseManager<PlayerUI>
{
    [SerializeField] Text playerName;
    [SerializeField] Text playerClass;

    [SerializeField] GameObject options;

    void Start()
    {
        playerName.text = "Fspotato";
        playerClass.text = DataManager.Instance.GetPlayerClass();
    }

    // 離開遊戲至主目錄
    public void ExitGame()
    {
        DataManager.Instance.SaveAllData();
        MapManager.Instance.gameObject.SetActive(false);
        Menu.Instance.gameObject.SetActive(true);
        options.SetActive(false);
        gameObject.SetActive(false);
    }

    // 選單開啟/關閉
    public void ShowOptions()
    {
        if (options.activeSelf == false) options.SetActive(true);
        else options.SetActive(false);
    }

    // 開啟技能樹
    public void ShowSkillTree()
    {
        SkillManager.Instance.gameObject.SetActive(true);
        SkillTreeUI.Instance.CloseSkillBox();
    }

    // 開啟背包
    public void ShowBackpack()
    {
        Backpack.Instance.ShowBackpack();
    }
}
