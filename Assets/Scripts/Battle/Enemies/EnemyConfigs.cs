using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyConfigs : ScriptableObject
{
    public EnemyType Type;
    public SerializableList<GameObject> Enemies;
}

public enum EnemyType
{
    Boss = 0,
    Slime = 1,
    Goblin = 2,
}
