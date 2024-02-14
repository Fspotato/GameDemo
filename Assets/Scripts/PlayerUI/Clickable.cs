using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Text text;
    [SerializeField] ClickType type;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case ClickType.Skills:
                PlayerUI.Instance.ShowSkillTree();
                break;
            case ClickType.Options:
                PlayerUI.Instance.ShowOptions();
                break;
            case ClickType.Settings:
                print("Settings");
                break;
            case ClickType.Backpack:
                PlayerUI.Instance.ShowBackpack();
                break;
            case ClickType.Exit:
                PlayerUI.Instance.ExitGame();
                break;
        }
    }
}

public enum ClickType
{
    Settings = 0,
    Options = 1,
    Skills = 2,
    Backpack = 3,
    Exit = 99,
}
