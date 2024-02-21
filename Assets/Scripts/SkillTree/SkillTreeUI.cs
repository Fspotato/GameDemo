using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SkillTreeUI : BaseManager<SkillTreeUI>
{
    [SerializeField] Text skillPointText;
    [SerializeField] Text skillText;
    [SerializeField] SkillManager skillManager;
    [SerializeField] GameObject skillBox;
    [SerializeField] GameObject content;
    [SerializeField] GameObject skillNodePrefab;
    [SerializeField] GameObject linePrefab;
    [SerializeField] Material lineMaterialPrefab;

    public List<SkillNode> skillNodes = new List<SkillNode>();
    public List<SkillLine> skillLines = new List<SkillLine>();

    SkillNode selected;
    GameObject nodeParent;

    void Update()
    {
        ReCalculateLineShader();
    }

    #region 顯示技能樹及面板相關

    // 展示技能樹
    public void ShowSkillTree(SkillTree tree)
    {
        ClearTree();

        CreateParent();

        CreateSkillNodes(tree.skills);

        DrawLines();

        SetNodeStates();

        ShowEquipedSkill();

        ShowSkillPoint();

        CloseSkillBox();
    }

    // 清除技能樹
    void ClearTree()
    {
        if (nodeParent != null) Destroy(nodeParent);
        selected = null;
        skillLines.Clear();
        skillNodes.Clear();
    }

    // 創建技能節點母物件 (用於更新技能樹時方便清空本來的技能節點)
    void CreateParent()
    {
        nodeParent = new GameObject("SkillNodes");
        nodeParent.transform.SetParent(content.transform);
        nodeParent.AddComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    // 創建技能節點
    void CreateSkillNodes(SerializableList<Skill> skills)
    {
        foreach (var skill in skills)
        {
            GameObject node = Instantiate(skillNodePrefab, nodeParent.transform);
            SkillNode sNode = node.GetComponent<SkillNode>();
            sNode.skill = skill;
            node.GetComponent<RectTransform>().anchoredPosition = new Vector2(skill.position.x * 100, (skill.position.y - 1) * 100);
            skillNodes.Add(sNode);
        }
    }

    // 繪製連線
    void DrawLines()
    {
        foreach (var node in skillNodes)
            foreach (var front in node.skill.frontSkills)
                AddLines(GetNode(front), node);
    }

    // 繪製連線
    void AddLines(SkillNode from, SkillNode to)
    {
        if (from == null || to == null) return;

        if (to.skill.position.x < from.skill.position.x)
        {
            var temp = from;
            from = to;
            to = temp;
        }

        GameObject line = Instantiate(linePrefab, nodeParent.transform);
        Material lm = line.GetComponent<Image>().material = Instantiate(lineMaterialPrefab);

        Vector2 fromPoint = from.GetComponent<RectTransform>().anchoredPosition;
        Vector2 toPoint = to.GetComponent<RectTransform>().anchoredPosition;

        line.GetComponent<RectTransform>().anchoredPosition = (fromPoint + toPoint) / 2f;
        line.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(fromPoint.x - toPoint.x) + 30f, Mathf.Abs(fromPoint.y - toPoint.y) + 30f);

        line.transform.SetAsFirstSibling();
        skillLines.Add(new SkillLine(from, to, line));

        lm.SetFloat("_LineWidth", 10f);
    }

    // 重新計算連線的Shader參數
    void ReCalculateLineShader()
    {
        foreach (var line in skillLines)
        {
            var image = line.obj.GetComponent<Image>();
            var material = image.GetModifiedMaterial(image.material);
            material.SetVector("_LP1", new Vector4(RectTransformUtility.WorldToScreenPoint(null, line.from.transform.position).x, 1080f - RectTransformUtility.WorldToScreenPoint(null, line.from.transform.position).y, 0f, 0f));
            material.SetVector("_LP2", new Vector4(RectTransformUtility.WorldToScreenPoint(null, line.to.transform.position).x, 1080f - RectTransformUtility.WorldToScreenPoint(null, line.to.transform.position).y, 0f, 0f));
            if (line.to.skill.unLocked == true) material.SetColor("_Color", Color.white);
            if (line.to.skill.unLocked == false) material.SetColor("_Color", Color.gray);
            if (line.from.skill.unLocked == false) material.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
        }
    }

    // 改變技能節點狀態
    public void SetNodeStates()
    {
        foreach (var node in skillNodes)
        {
            node.SetState(SkillNodeState.Hidden);
        }

        foreach (var node in skillNodes.Where(n => n.skill.frontSkills.Count == 0))
        {
            node.SetState(SkillNodeState.Locked);
        }

        foreach (var point in skillManager.skillTree.unLockedSkills)
        {
            skillNodes.FirstOrDefault(n => n.skill.point.Equals(point)).SetState(SkillNodeState.UnLocked);
        }

        foreach (var node in skillNodes)
        {
            if (node.state != SkillNodeState.Hidden) continue;

            foreach (var point in node.skill.frontSkills)
            {
                if (skillManager.skillTree.unLockedSkills.FirstOrDefault(p => p.Equals(point)) != default(Point))
                {
                    node.SetState(SkillNodeState.Locked);
                    continue;
                }
            }
        }
    }

    // 顯示穿戴中的技能
    public void ShowEquipedSkill()
    {
        string skills = "";
        foreach (var pair in skillManager.skillTree.equipedSkills)
        {
            if (pair.Value == default) continue;
            skills += $"{skillManager.GetSkillById(pair.Value).type} : {skillManager.GetSkillById(pair.Value).name}\n";
        }
        skillText.text = skills;
    }

    // 顯示技能點數
    public void ShowSkillPoint()
    {
        skillPointText.text = $"Skill Point:{skillManager.skillTree.SkillPoint}";
    }

    // 關閉技能檢視視窗
    public void CloseSkillBox()
    {
        if (GetSelectedNode() != null)
            GetSelectedNode().transform.Find("Selected").gameObject.SetActive(false);
        selected = null;
        skillBox.SetActive(false);
    }

    // 檢視技能
    public void ShowSkillBox(Skill skill)
    {
        skillBox.SetActive(true);
        skillBox.transform.Find("SkillName").GetComponent<Text>().text = skill.name;
        skillBox.transform.Find("SkillDescription").GetComponent<Text>().text = skill.description;
        if (skill.unLocked)
        {
            skillBox.transform.Find("Equip").gameObject.SetActive(true);
            if (skill.isEquiped) skillBox.transform.Find("Equip").Find("Text").GetComponent<Text>().text = "UnEquip";
            else skillBox.transform.Find("Equip").Find("Text").GetComponent<Text>().text = "Equip";
            skillBox.transform.Find("UnLock").gameObject.SetActive(false);
        }
        else
        {
            skillBox.transform.Find("Equip").gameObject.SetActive(false);
            skillBox.transform.Find("UnLock").gameObject.SetActive(true);
        }
    }

    // 根據點位獲取 SkillNode
    SkillNode GetNode(Point point)
    {
        return skillNodes.FirstOrDefault(n => n.skill.point.Equals(point));
    }

    #endregion

    // 獲取技能節點
    public SkillNode GetSelectedNode()
    {
        return skillNodes.FirstOrDefault(n => n.selected.activeSelf == true);
    }

    // 選擇技能節點
    public void SelectSkill(SkillNode node)
    {
        if (node.state == SkillNodeState.Hidden) return;
        if (GetSelectedNode() != null)
            GetSelectedNode().selected.SetActive(false);
        node.selected.SetActive(true);
        ShowSkillBox(node.skill);
        selected = node;
    }

    // 裝備技能
    public void EquipSkill()
    {
        SkillManager.Instance.EquipSkill(selected.skill);
        ShowSkillBox(selected.skill);
        ShowEquipedSkill();
        SetNodeStates();
    }

    // 解鎖技能
    public void UnLockSkill()
    {
        SkillManager.Instance.UnLockSkill(selected.skill);
        skillPointText.text = $"Skill Point:{skillManager.skillTree.SkillPoint}";
        ShowSkillBox(selected.skill);
        SetNodeStates();
    }
}

public class SkillLine
{
    public SkillNode from;
    public SkillNode to;
    public GameObject obj;

    public SkillLine(SkillNode from, SkillNode to, GameObject obj)
    {
        this.from = from;
        this.to = to;
        this.obj = obj;
    }
}
