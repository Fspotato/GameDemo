using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver // 可序列化Dictionary
{
    [SerializeField] List<TKey> keys = new List<TKey>();
    [SerializeField] List<TValue> values = new List<TValue>();


    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}

[Serializable]
public class SerializableList<T> : List<T>, ISerializationCallbackReceiver // 可序列化List
{
    [SerializeField] List<T> list = new List<T>();

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            this.Add(list[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        list.Clear();
        foreach (T item in this)
        {
            list.Add(item);
        }
    }
}