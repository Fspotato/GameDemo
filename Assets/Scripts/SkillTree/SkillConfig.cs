using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillConfig : ScriptableObject
{
    public SerializableList<Skill> skills;

    public SerializableList<Skill> DeepCopy()
    {
        string json = JsonUtility.ToJson(skills);
        return JsonUtility.FromJson<SerializableList<Skill>>(json);
    }
}
