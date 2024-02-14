using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class MenuClickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Text text;
    [SerializeField] MenuClickType type;

    void Start()
    {
        if (type == MenuClickType.ContinueGame)
        {
            if (File.Exists(Application.persistentDataPath + "/backpack.json"))
            {
                text.color = Menu.Instance.clickableColor;
            }
            else
            {
                text.color = Menu.Instance.UnClickableColor;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case MenuClickType.Back:
                Menu.Instance.Back();
                break;
            case MenuClickType.ComingSoon:
                print("Coming Soon!");
                break;
            case MenuClickType.ContinueGame:
                if (File.Exists(Application.persistentDataPath + "/backpack.json"))
                {
                    Menu.Instance.ContinueGame();
                }
                else
                {
                    print("you dont have any record in game!");
                }
                break;
            case MenuClickType.Exit:
                Application.Quit();
                break;
            case MenuClickType.NewGame:
                Menu.Instance.NewGame();
                break;
            case MenuClickType.Normal:
                Menu.Instance.Normal();
                break;
            case MenuClickType.StartGameWithSwordMan:
                Menu.Instance.StartNewGame(MenuClickType.StartGameWithSwordMan);
                break;
        }
    }
}


public enum MenuClickType
{
    Back = 0,
    ComingSoon = 1,
    ContinueGame = 2,
    Exit = 3,
    NewGame = 4,
    Normal = 5,
    StartGameWithSwordMan = 6,
}