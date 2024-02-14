using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Skill skill;
    public SkillNodeState state;

    public void OnPointerClick(PointerEventData eventData)
    {
        SkillTreeUI.Instance.SelectSkill(this);
    }

    public void SetState(SkillNodeState state)
    {
        this.state = state;
        switch (state)
        {
            case SkillNodeState.UnLocked:
                gameObject.SetActive(true);
                image.color = Color.white;
                break;
            case SkillNodeState.Locked:
                gameObject.SetActive(true);
                image.color = Color.gray;
                break;
            case SkillNodeState.Hidden:
                gameObject.SetActive(false);
                break;
        }
    }
}

public enum SkillNodeState
{
    UnLocked,
    Locked,
    Hidden,
}
