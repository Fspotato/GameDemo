using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillClickable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    [SerializeField] Text text;
    [SerializeField] SkillClickType type;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case SkillClickType.GetSkillPoint:
                SkillManager.Instance.GetSkillPoint(1);
                break;
            case SkillClickType.UnLock:
                SkillTreeUI.Instance.UnLockSkill();
                break;
            case SkillClickType.Close:
                SkillTreeUI.Instance.CloseSkillBox();
                break;
            case SkillClickType.EquipSkill:
                SkillTreeUI.Instance.EquipSkill();
                break;
            case SkillClickType.Exit:
                SkillManager.Instance.SaveSkillTree();
                SkillTreeUI.Instance.gameObject.SetActive(false);
                break;
            case SkillClickType.DoNothing:
                break;
        }
    }
}

public enum SkillClickType
{
    GetSkillPoint = 0,
    EquipSkill = 1,
    UnLock = 2,
    Close = 3,
    Exit = 4,
    DoNothing = 5,
}
