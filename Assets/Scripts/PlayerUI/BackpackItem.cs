using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackItem : MonoBehaviour
{
    [SerializeField] public Text ItemName;
    [SerializeField] public Text ItemCount;
    [SerializeField] public GameObject selected;
    public uint ID;
}
