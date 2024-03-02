using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : BaseManager<EventManager>
{
    [SerializeField] Image image;
    [SerializeField] Text eventName;
    [SerializeField] Text description;

    [SerializeField] List<GameObject> selections = new List<GameObject>();
    [SerializeField] List<EventConfig> configs;

    Event e;

    // 進入事件 (EventType 在 Event.cs 下)
    public void EnterEvent(EventType type)
    {
        gameObject.SetActive(true);

        int rnd;

        do rnd = Random.Range(0, configs[0].Events.Count);
        while (configs[0].Events[rnd].Type != type);

        e = configs[0].Events[rnd];
        e.ResetEvent();
        ShowEvent(e.EventName, e.Description, e.Selections);
    }

    // 展示事件
    public void ShowEvent(string name, string description, List<string> strs)
    {
        for (int i = 0; i < selections.Count; i++) selections[i].SetActive(false);

        eventName.text = name;
        this.description.text = description;
        for (int i = 0; i < strs.Count; i++)
        {
            if (i >= 3) break;
            selections[i].SetActive(true);
            selections[i].transform.Find("Text").GetComponent<Text>().text = strs[i];
        }
    }

    // 事件選擇
    public void Selections(int index)
    {
        switch (index)
        {
            case 0: e.Selection1(); break;
            case 1: e.Selection2(); break;
            case 2: e.Selection3(); break;
        }
    }

    // 離開事件
    public void ExitEvent()
    {
        MapManager.Instance.gameObject.SetActive(true);
        MapManager.Instance.EnterNode();
        PlayerUI.Instance.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}


