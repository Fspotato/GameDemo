using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class Event : ScriptableObject
{
    [SerializeField] Image imgae;
    [SerializeField] EventType type;
    [SerializeField] string eventName;
    [SerializeField] string description;
    [SerializeField] List<string> selections;

    public EventType Type => type;
    public string EventName => eventName;
    public string Description => description;
    public List<string> Selections => selections;

    protected int level = 0;

    public virtual void ResetEvent()
    {
        level = 0;
    }

    public virtual void Selection1()
    {

    }

    public virtual void Selection2()
    {

    }

    public virtual void Selection3()
    {

    }
}

public enum EventType
{
    Normal = 0,
    Shop = 1,
}
