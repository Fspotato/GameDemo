using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BackpackClickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Text text;
    [SerializeField] BackpackClickType type;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case BackpackClickType.View:
                Backpack.Instance.ShowItem(transform.GetComponent<BackpackItem>());
                break;
            case BackpackClickType.Exit:
                Backpack.Instance.Exit();
                break;
        }
    }
}

public enum BackpackClickType
{
    View = 0,
    Exit = 99,
}
