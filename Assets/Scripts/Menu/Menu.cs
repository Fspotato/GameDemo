using System.Linq;
using UnityEngine;

public class Menu : BaseManager<Menu>
{
    #region 變數
    GameObject back;

    GameObject newGame;
    GameObject continueGame;
    GameObject exit;

    GameObject normal;
    GameObject comingSoon;

    GameObject[] characters = new GameObject[3];

    int processIndex;

    public Color32 clickableColor = Color.white;
    public Color32 UnClickableColor = Color.gray;

    #endregion

    void Start()
    {
        newGame = LoadObj<GameObject>("newgame", new Vector2(0f, 200f));
        continueGame = LoadObj<GameObject>("continue", new Vector2(0f, 0f));
        exit = LoadObj<GameObject>("exit", new Vector2(0f, -200f));
        normal = LoadObj<GameObject>("normal", new Vector2(-380f, 0f));
        comingSoon = LoadObj<GameObject>("comingsoon", new Vector2(380f, 0f));
        back = LoadObj<GameObject>("back", new Vector2(-850f, 470f));
        characters[0] = LoadObj<GameObject>("swordsman", new Vector2(-450f, 0f));
        characters[1] = LoadObj<GameObject>("comingsoon", new Vector2(0f, 0f));
        characters[2] = LoadObj<GameObject>("comingsoon", new Vector2(450f, 0f));
        processIndex = 0;
        ToProcess();
    }

    // 開新遊戲
    public void StartNewGame(MenuClickType type)
    {
        switch (type)
        {
            case MenuClickType.StartGameWithSwordMan:
                DataManager.Instance.NewGame(10001);
                SkillManager.Instance.CreateSkillTree(ClassType.SwordMan);
                // 設置初始技能 劈砍
                SkillManager.Instance.UnLockSkill("劈砍");
                SkillManager.Instance.EquipSkill("劈砍");
                SkillManager.Instance.UnLockSkill("守護");
                SkillManager.Instance.EquipSkill("守護");
                SkillManager.Instance.UnLockSkill("鬥氣");
                SkillManager.Instance.EquipSkill("鬥氣");
                SkillTreeUI.Instance.SetNodeStates();
                SkillTreeUI.Instance.ShowEquipedSkill();
                break;
        }
        MapManager.Instance.GenerateNewMap();
        ToMainGame();
    }

    // 繼續遊戲
    public void ContinueGame()
    {
        DataManager.Instance.LoadData();
        SkillManager.Instance.LoadSkillTree();
        MapManager.Instance.LoadMap();
        ToMainGame();
    }

    // 進入一般模式選單
    public void Normal()
    {
        processIndex = 2;
        ToProcess();
    }

    // 進入開新遊戲選單
    public void NewGame()
    {
        processIndex = 1;
        ToProcess();
    }

    // 返回上一個選單
    public void Back()
    {
        if (processIndex > 0) processIndex--;
        ToProcess();
    }

    // 進入選單 (0:起始頁面 1:選擇模式 2:選擇角色)
    public void ToProcess()
    {
        switch (processIndex)
        {
            case 0:
                newGame.SetActive(true);
                continueGame.SetActive(true);
                exit.SetActive(true);
                normal.SetActive(false);
                comingSoon.SetActive(false);
                back.SetActive(false);
                for (int i = 0; i < characters.Length; i++)
                    characters[i].SetActive(false);
                break;
            case 1:
                newGame.SetActive(false);
                continueGame.SetActive(false);
                exit.SetActive(false);
                normal.SetActive(true);
                comingSoon.SetActive(true);
                back.SetActive(true);
                for (int i = 0; i < characters.Length; i++)
                    characters[i].SetActive(false);
                break;
            case 2:
                newGame.SetActive(true);
                continueGame.SetActive(false);
                exit.SetActive(true);
                normal.SetActive(false);
                comingSoon.SetActive(false);
                back.SetActive(true);
                for (int i = 0; i < characters.Length; i++)
                    characters[i].SetActive(true);
                break;
        }
    }

    // 離開選單至遊戲
    void ToMainGame()
    {
        PlayerUI.Instance.gameObject.SetActive(true);
        MapManager.Instance.gameObject.SetActive(true);
        processIndex = 0;
        ToProcess();
        gameObject.SetActive(false);
    }

    // AB包資源加載
    private T LoadObj<T>(string name, Vector2 position) where T : Object
    {
        T obj = ABManager.Instance.LoadRes<T>("menu", name);
        if (obj is GameObject)
        {
            (obj as GameObject).transform.SetParent(transform);
            (obj as GameObject).GetComponent<RectTransform>().anchoredPosition = position;
        }
        return obj;
    }
}
