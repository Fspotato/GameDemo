using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EventConfig : ScriptableObject
{
    public SerializableList<Event> Events;
}
