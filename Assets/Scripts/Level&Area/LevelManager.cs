using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : BaseManager<LevelManager>
{
    [SerializeField] LevelInfo levelInfo;
    public LevelInfo LevelInfo => levelInfo;

    public void Init()
    {
        levelInfo = new LevelInfo(1);
    }

    public void LoadInfo()
    {
        if (File.Exists(Application.persistentDataPath + "/level.json"))
        {
            levelInfo = JsonUtility.FromJson<LevelInfo>(File.ReadAllText(Application.persistentDataPath + "/level.json"));
        }
        else
        {
            Init();
        }
    }

    public void LevelChange(int value)
    {
        levelInfo.level += value;
        levelInfo.Refresh();
    }

    public void ExtraEnhanceCoefChange(float value)
    {
        levelInfo.extraEnhanceCoef += value;
    }

    public void SetLevel(int level)
    {
        levelInfo.level = level;
        levelInfo.Refresh();
    }

    public void SetExtraEnhanceCoef(float value)
    {
        levelInfo.extraEnhanceCoef = value;
    }

    public void SaveInfo()
    {
        File.WriteAllText(Application.persistentDataPath + "/level.json", levelInfo.ToJson());
    }
}

[System.Serializable]
public struct LevelInfo
{
    public int level;
    public float enhanceCoef;
    public float extraEnhanceCoef;

    public float TotalEnhanceCoef => enhanceCoef + extraEnhanceCoef;

    public LevelInfo(int level)
    {
        this.level = level;
        enhanceCoef = 0.05f * level;
        extraEnhanceCoef = 0.75f;
    }

    public void Refresh()
    {
        enhanceCoef = 0.05f * level;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}