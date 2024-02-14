using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string name;
    public string description;
    public string arrange;
    public Point point;
    public SerializableList<Point> frontSkills = new SerializableList<Point>();
    public Vector2 position;
    public int require;
    public bool unLocked;
    public bool isEquiped;

    public bool Equals(Skill skill)
    {
        if (skill == null) return false;
        return name == skill.name;
    }
}
