using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

public class BattleUI : BaseManager<BattleUI>
{
    #region 變量
    // 充能版和技能顯示窗
    [SerializeField] GameObject charger;
    [SerializeField] GameObject skillBox;
    // 元素球物件池
    ObjectPool<GameObject> redBallPool;
    ObjectPool<GameObject> blueBallPool;
    ObjectPool<GameObject> yellowBallPool;
    // 持球區、持球上限及額外持球
    [SerializeField] List<GameObject> balls = new List<GameObject>();
    int maxBall = 2; // 預設為2
    bool extra = false;
    // 充能板被選中格及選擇索引
    [SerializeField] GameObject selectedBlock;
    [SerializeField] int selectedIndex = 0;
    // 充能板元素球放置及其組合
    [SerializeField] GameObject[] ballSet = new GameObject[] { null, null, null };
    [SerializeField] char[] ballArrange = { '0', '0', '0' };
    // 血量條
    [SerializeField] GameObject hpBar;

    [Header("鍵位")]
    KeyCode setRed = KeyCode.A;
    KeyCode setBlue = KeyCode.S;
    KeyCode setYellow = KeyCode.D;
    KeyCode setSpace = KeyCode.Space;
    KeyCode removeBall = KeyCode.Escape;
    KeyCode attack = KeyCode.X;
    #endregion

    void Awake()
    {
        if (instance == null) instance = this;
        // 從AB包讀取UI物件
        redBallPool = GetObjectPool("redball");
        blueBallPool = GetObjectPool("blueball");
        yellowBallPool = GetObjectPool("yellowball");
    }

    void Update()
    {
        TakeBall();
        Attack();
        if (Input.GetKeyDown(KeyCode.C)) EndTurn();
    }

    // 戰鬥開始
    public void BattleStart()
    {
        extra = false;
        selectedIndex = 0;
        SelectBlock();
        TurnStart();
    }

    // 在技能格展示當前技能
    private void ShowSkill()
    {
        string skill = new(ballArrange);
        // 調用 DataManager 中的API
        skillBox.transform.Find("SkillName").GetComponent<Text>().text = SkillManager.Instance.GetSkillName(skill);
        skillBox.transform.Find("SkillDescription").GetComponent<Text>().text = SkillManager.Instance.GetSkillDescription(skill);
    }

    // 更改血量條顯示
    public void ShowHpBar(float hp, float maxHp)
    {
        hpBar.GetComponent<HpBarController>().ShowHpBar(hp, maxHp);
    }

    #region 控制相關
    // 攻擊判定
    private void Attack()
    {
        if (Input.GetKeyDown(attack))
        {
            // 檢測有幾顆球被放在充能板上
            int ballCount = 0;
            foreach (char b in ballArrange)
            {
                if (b != '0') ballCount++;
            }
            // 有球才進判定
            if (ballCount != 0)
            {
                // 輸出攻擊指令
                string arrange = new(ballArrange);
                BattleManager.Instance.PlayerAttack(arrange);
                // 元素球消除
                ChargerReset();
                // 攻擊判定結束後 把選中格重置至第一格 並刷新技能格顯示
                selectedIndex = 0;
                SelectBlock();
                ShowSkill();
            }
        }
    }
    // 拿取元素球至充能板
    private void TakeBall()
    {
        // 放紅球
        if (Input.GetKeyDown(setRed))
        {
            // 檢測置球區還有沒有紅球 有則進入元素球設置邏輯
            GameObject ball = balls.FirstOrDefault(obj => obj.name.StartsWith("RedBall"));
            if (ball != null) SetBall(ball, 'R');
            ShowSkill();
        }
        else if (Input.GetKeyDown(setBlue))
        {
            GameObject ball = balls.FirstOrDefault(obj => obj.name.StartsWith("BlueBall"));
            if (ball != null) SetBall(ball, 'B');
            ShowSkill();
        }
        else if (Input.GetKeyDown(setYellow))
        {
            GameObject ball = balls.FirstOrDefault(obj => obj.name.StartsWith("YellowBall"));
            if (ball != null) SetBall(ball, 'Y');
            ShowSkill();
        }
        else if (Input.GetKeyDown(setSpace) && selectedIndex != 0 && selectedIndex < 3)
        {
            selectedIndex++;
            SelectBlock();
            ShowSkill();
        }
        else if (Input.GetKeyDown(removeBall) && selectedIndex != 0)
        {
            selectedIndex--;
            SelectBlock();
            RemoveBall();
            ShowSkill();
        }
    }
    // 回合結束
    public void EndTurn()
    {
        EventSystem.current.SetSelectedGameObject(null);
        for (int i = 0; i < ballArrange.Length; i++)
        {
            if (ballArrange[i] != '0') extra = true;
        }
        ChargerReset();
        if (balls.Count != 0) extra = true;
        BattleManager.Instance.EnemyTurn();
        TurnStart();
    }
    #endregion

    #region 元素球放置區相關
    // 刷新元素放置區 (排序及顯示)
    private void RefreshBalls()
    {
        // 排序方式 (紅 -> 藍 -> 黃)
        balls.Sort((a, b) =>
        {
            if (a.name[0] == 'R')
            {
                return -1;
            }
            else if (a.name[0] == 'B')
            {
                if (b.name[0] == 'R')
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return 1;
            }
        });
        for (int i = 0; i < balls.Count; i++)
        {
            GameObject ball = balls[i];
            ball.GetComponent<RectTransform>().anchoredPosition = new Vector2(175f + i * 50f, -480f);
        }
    }
    // 獲得元素球
    private void GetBall(int choose)
    {
        // 最大持球數不超過 7 顆
        if (balls.Count >= 7) return;
        GameObject obj;
        // 0 紅球 1 藍球 2 黃球 
        if (choose == 0)
        {
            obj = redBallPool.Get();
        }
        else if (choose == 1)
        {
            obj = blueBallPool.Get();
        }
        else
        {
            obj = yellowBallPool.Get();
        }
        // 放入元素球放置區並刷新UI
        balls.Add(obj);
        RefreshBalls();
    }
    // 回合開始刷新元素球
    private void TurnStart()
    {
        // 清空持球區
        foreach (var ball in balls)
        {
            if (ball.name.Contains("Red"))
            {
                redBallPool.Release(ball);
            }
            else if (ball.name.Contains("Blue"))
            {
                blueBallPool.Release(ball);
            }
            else if (ball.name.Contains("Yellow"))
            {
                yellowBallPool.Release(ball);
            }
        }
        balls.Clear();
        // 根據持球上限刷新元素球 機率(紅球40% 藍球40% 黃球20%)
        for (int i = 0; i < maxBall; i++)
        {
            int rnd = Random.Range(0, 5);
            switch (rnd)
            {
                case 0:
                case 1:
                    GetBall(0);
                    break;
                case 2:
                case 3:
                    GetBall(1);
                    break;
                case 4:
                    GetBall(2);
                    break;
            }
        }
        // 如果上一回合有剩球 這回合則會多刷新一顆球
        if (extra)
        {
            int rnd = Random.Range(0, 5);
            switch (rnd)
            {
                case 0:
                case 1:
                    GetBall(0);
                    break;
                case 2:
                case 3:
                    GetBall(1);
                    break;
                case 4:
                    GetBall(2);
                    break;
            }
            extra = false;
        }
        // 回合開始時 把選中格重置至第一格
        selectedIndex = 0;
        SelectBlock();
    }
    #endregion

    #region 充能板相關
    // 選擇充能格
    private void SelectBlock()
    {
        // 重新選擇前把原本的設置為白色 選中後再把選中的變成黑色
        if (selectedBlock != null) selectedBlock.GetComponent<Image>().color = Color.white;
        if (selectedIndex < 3) selectedBlock = charger.transform.Find($"T{selectedIndex}").gameObject;
        if (selectedBlock != null && selectedIndex < 3) selectedBlock.GetComponent<Image>().color = Color.black;
    }
    // 充能板設置元素球
    private void SetBall(GameObject ball, char element)
    {
        // 先檢測選中的充能格上是否有球
        RemoveBall();
        // 從放置區中移除該球 並刷新放置區面板
        balls.Remove(ball);
        RefreshBalls();
        // 將球放至充能板上 並記錄下組合順序
        ballSet[selectedIndex] = ball;
        ballArrange[selectedIndex] = element;
        // 把球放至充能板下 才不會導致座標偏移
        ball.transform.SetParent(charger.transform);
        ball.GetComponent<RectTransform>().anchoredPosition = selectedBlock.GetComponent<RectTransform>().anchoredPosition;
        // 將選中格移至下一格
        selectedIndex++;
        SelectBlock();
    }
    // 充能板移除元素球判定
    private void RemoveBall()
    {
        if (ballSet[selectedIndex] != null)
        {
            GameObject temp = ballSet[selectedIndex];
            if (temp.name.Contains("Red"))
            {
                // 從充能板上把球移除
                redBallPool.Release(temp);
                // 放回放球區
                GetBall(0);
            }
            else if (temp.name.Contains("Blue"))
            {
                blueBallPool.Release(temp);
                GetBall(1);
            }
            else if (temp.name.Contains("Yellow"))
            {
                yellowBallPool.Release(temp);
                GetBall(2);
            }
            ballSet[selectedIndex] = null;
            ballArrange[selectedIndex] = '0';
        }
    }
    // 充能板重置
    private void ChargerReset()
    {
        // 元素球消除
        for (int i = 0; i < ballSet.Length; i++)
        {
            if (ballSet[i] != null)
            {
                // 紅球消除
                if (ballSet[i].name.Contains("Red"))
                {
                    redBallPool.Release(ballSet[i]);
                }
                // 黃球消除
                else if (ballSet[i].name.Contains("Blue"))
                {
                    blueBallPool.Release(ballSet[i]);
                }
                // 黃球消除
                else if (ballSet[i].name.Contains("Yellow"))
                {
                    yellowBallPool.Release(ballSet[i]);
                }
            }
            ballSet[i] = null;
            ballArrange[i] = '0';
        }
    }
    #endregion

    #region 加載AB包相關
    // 創建元素球池
    private ObjectPool<GameObject> GetObjectPool(string name)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            () =>
            {
                // 創建元素球時執行的操作
                return LoadObj<GameObject>(name);
            },
            (ball) =>
            {
                // 從物件池拿出元素球的操作
                ball.SetActive(true);
            },
            (ball) =>
            {
                // 把球放回物件池
                ball.transform.SetParent(transform);
                ball.SetActive(false);
            },
            (ball) =>
            {
                // 當球不需要使用時
                Destroy(ball);
            }, true, 20, 20
        );
        return pool;
    }
    // 加載AB包資源
    private T LoadObj<T>(string name, Vector2 position) where T : Object
    {
        T obj = ABManager.Instance.LoadRes<T>("battleui", name);
        if (obj is GameObject)
        {
            (obj as GameObject).transform.SetParent(transform);
            (obj as GameObject).GetComponent<RectTransform>().anchoredPosition = position;
        }
        return obj;
    }
    // 加載AB包資源 無座標重載
    private T LoadObj<T>(string name) where T : Object
    {
        T obj = ABManager.Instance.LoadRes<T>("battleui", name);
        if (obj is GameObject)
        {
            (obj as GameObject).transform.SetParent(transform);
        }
        return obj;
    }
    #endregion
}
