using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    public Image blueprint;
    public uint id;
    public string name;
    public string description;
    public SkillType type;
    public SerializableList<uint> frontSkills = new SerializableList<uint>();
    public Vector2 position;
    public int require;
    public bool unLocked;
    public bool isEquiped;
    public bool isEnabled;

    public bool Equals(Skill skill)
    {
        if (skill == null) return false;
        return name == skill.name;
    }
}

public enum SkillType
{
    BasicSkill = 0,
    SkillA = 1,
    SkillB = 2,
    Ultimate = 3,
    Passive = 4,
}

public enum ClassType
{
    SwordMan = 0,
    Sworder = 1,
}
