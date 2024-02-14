using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SkillTree的主類 技能樹存放在這個類裡面
[System.Serializable]
public class SkillTree
{
    public SerializableList<Skill> skills;

    public int SkillPoint;

    public SerializableList<Point> unLockedSkills = new SerializableList<Point>();
    public SerializableDictionary<string, Skill> equipedSkills = new SerializableDictionary<string, Skill>();

    public SkillTree(SerializableList<Skill> skills)
    {
        this.skills = skills;
    }

    public void GetSkillPoint(int amount)
    {
        SkillPoint += amount;
    }

    public void UseSkillPoint(int amount)
    {
        SkillPoint -= amount;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }
}
