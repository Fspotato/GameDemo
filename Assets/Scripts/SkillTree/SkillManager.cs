using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SkillManager : BaseManager<SkillManager>
{

    [SerializeField] List<SkillConfig> configs = new List<SkillConfig>();

    public SkillTree skillTree;

    // 依照選擇職業創建技能樹 (0:劍士)
    public void CreateSkillTree(ClassType type)
    {
        int index = (int)type;
        skillTree = new SkillTree(configs[index].DeepCopy());
        GetSkillPoint(10);
        SkillTreeUI.Instance.ShowSkillTree(skillTree);
        SaveSkillTree();
    }

    // 讀取技能樹
    public void LoadSkillTree()
    {
        if (File.Exists(Application.persistentDataPath + "/skilltree.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/skilltree.json");
            skillTree = JsonUtility.FromJson<SkillTree>(json);
            skillTree.ReLinkEquipedSkills();
        }
        else
        {
            print("Can't find skilltree.json in presistentDataPath!");
        }
        SkillTreeUI.Instance.ShowSkillTree(skillTree);
    }

    // 獲得技能點
    public void GetSkillPoint(int amount)
    {
        skillTree.GetSkillPoint(amount);
        SkillTreeUI.Instance.ShowSkillPoint();
        SaveSkillTree();
    }

    // 使用技能點
    public void UseSkillPoint(int amount)
    {
        skillTree.UseSkillPoint(amount);
        SkillTreeUI.Instance.ShowSkillPoint();
        SaveSkillTree();
    }

    // 解鎖技能
    public void UnLockSkill(Skill skill)
    {
        if (skillTree.SkillPoint >= skill.require)
        {
            UseSkillPoint(skill.require);
            skillTree.unLockedSkills.Add(skill.point);
            skill.unLocked = true;
            SkillTreeUI.Instance.ShowSkillBox(skill);
        }
        else
        {
            print("You don't have enough skill point!");
        }
    }

    // 裝備技能
    public void EquipSkill(Skill skill)
    {
        if (skillTree.equipedSkills.ContainsKey(skill.type))
        {
            if (skill.Equals(GetEquipedSkillByType(skill.type)))
            {
                skillTree.equipedSkills[skill.type] = default;
                skill.isEquiped = false;
            }
            else
            {
                if (skillTree.equipedSkills[skill.type] != default)
                    skillTree.skills.FirstOrDefault(s => s.id == skillTree.equipedSkills[skill.type]).isEquiped = false;

                skill.isEquiped = true;
                skillTree.equipedSkills[skill.type] = skill.id;
            }
        }
        else
        {
            skillTree.equipedSkills.Add(skill.type, skill.id);
            skill.isEquiped = true;
        }
        SaveSkillTree();
    }

    // 解鎖技能 重載一: 用名稱解鎖
    public void UnLockSkill(string name)
    {
        Skill skill = skillTree.skills.FirstOrDefault(s => s.name == name);
        if (skill == default(Skill)) return;
        UnLockSkill(skill);
    }

    // 裝備技能 重載一: 用名稱解鎖
    public void EquipSkill(string name)
    {
        Skill skill = skillTree.skills.FirstOrDefault(s => s.name == name);
        if (skill == default) return;
        EquipSkill(skill);
    }

    // 儲存技能樹
    public void SaveSkillTree()
    {
        File.WriteAllText(Application.persistentDataPath + "/skilltree.json", skillTree.ToJson());
    }

    #region 技能檢視

    // 技能名稱檢視(戰鬥模塊)
    public string GetSkillName(SkillType type)
    {
        if (skillTree.equipedSkills.ContainsKey(type))
        {
            return GetEquipedSkillByType(type).name;
        }
        else
        {
            return "未裝備技能";
        }
    }

    // 技能描述檢視(戰鬥模塊)
    public string GetSkillDescription(SkillType type)
    {
        if (skillTree.equipedSkills.ContainsKey(type))
        {
            return GetEquipedSkillByType(type).name;
        }
        else
        {
            return "未裝備技能";
        }
    }

    #endregion

    public Skill GetSkillById(uint id)
    {
        return skillTree.skills.FirstOrDefault(s => s.id == id);
    }

    public Skill GetEquipedSkillByType(SkillType type)
    {
        if (!skillTree.equipedSkills.ContainsKey(type)) return default;
        if (skillTree.equipedSkills[type] == default) return default;
        return GetSkillById(skillTree.equipedSkills[type]);
    }

}

public enum SkillType
{
    BasicSkill = 0,
    SkillA = 1,
    SkillB = 2,
    Ultimate = 3,
}

public enum ClassType
{
    SwordMan = 0,
    Sworder = 1,
}
