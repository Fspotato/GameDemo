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
    public SerializableDictionary<SkillType, uint> equipedSkills = new SerializableDictionary<SkillType, uint>();

    public SkillTree(SerializableList<Skill> skills)
    {
        this.skills = skills;
    }

    // json反序列化後會解除引用關係 變成兩個獨立的對象 因此需要重新鏈結
    public void ReLinkEquipedSkills()
    {
        /*equipedSkills.Clear();
        foreach (var skill in skills)
        {
            if (skill.isEquiped) equipedSkills.Add(skill.arrange, skill);
        }*/
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
