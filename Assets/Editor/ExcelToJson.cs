using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using BattleNew;

public class ExcelToJson
{
    [MenuItem("Custom Tool/EntityConfig.csv to .json")]
    private static void EntityConfigCsvToJson()
    {
        // 通過編輯器的 Selection類 中的方法 獲取在Project窗口中選中的資源 如果沒有選中任何資源就退出
        UnityEngine.Object[] selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectedAssets.Length == 0) return;

        // 用於放置所有實體資料
        SerializableDictionary<uint, EntityConfig> entityConfigs = new SerializableDictionary<uint, EntityConfig>();

        string assetPath;
        string[] temp;

        foreach (UnityEngine.Object asset in selectedAssets)
        {
            // 獲取資源位置
            assetPath = AssetDatabase.GetAssetPath(asset);
            // 檢測資源用 若有不是 csv 檔的資源則報錯退出
            temp = assetPath.Split('/');
            if (temp[temp.Length - 1].Split('.')[1] != "csv")
            {
                Debug.LogError("選中的資源中有不是csv檔的資源 請重新選擇!");
                return;
            }

            // 讀出csv中所有資源 並讀出其中的 EntityKey (從第三項開始讀 因為前二項是 ID ,Type 要分別處理)
            string[] excelConfigs = File.ReadAllLines(assetPath);
            string[] configKeys = excelConfigs[0].Split(',');
            List<EntityKey> entityKeys = new List<EntityKey>();
            for (int i = 2; i < configKeys.Length; i++)
            {
                entityKeys.Add(Enum.Parse<EntityKey>(configKeys[i]));
            }

            EntityConfig ec;
            string[] configValues;
            // 將每行 EntityConfig 逐一放入entityConfigs中 從1開始讀 因為第0行是EntityKey
            for (int i = 1; i < excelConfigs.Length; i++)
            {
                ec = new EntityConfig();
                configValues = excelConfigs[i].Split(',');
                ec.ID = uint.Parse(configValues[0]);
                ec.Type = Enum.Parse<EntityType>(configValues[1]);
                // 從第三項開始讀 因為前兩項是 ID ,Type 上面處理過了
                for (int j = 2; j < configValues.Length; j++)
                {
                    // int類的value放入IntValue 其他放進StrValue
                    int value;
                    if (int.TryParse(configValues[j], out value))
                    {
                        ec.IntValue.Add(entityKeys[j - 2], value);
                    }
                    else
                    {
                        ec.StrValue.Add(entityKeys[j - 2], configValues[j]);
                    }
                }
                entityConfigs.Add(ec.ID, ec);
            }
        }

        // 全部讀完後將資料轉成json格式並放進指定路徑中
        string jsonPath = Application.dataPath + "/ArtRes/Config/EntityConfig.json";
        string json = JsonUtility.ToJson(entityConfigs);
        File.WriteAllText(jsonPath, json);
        Debug.Log($"序列化完成, 已序列化{selectedAssets.Length}個文件, 已經文件存儲於預設地址");
        AssetDatabase.Refresh();
    }

    [MenuItem("Custom Tool/BuffConfig.csv to .json")]
    private static void BuffConfigCsvToJson()
    {
        // 通過編輯器的 Selection類 中的方法 獲取在Project窗口中選中的資源 如果沒有選中任何資源就退出
        UnityEngine.Object[] selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (selectedAssets.Length == 0) return;

        // 用於放置所有Buff資料
        SerializableDictionary<uint, Buff> buffConfigs = new SerializableDictionary<uint, Buff>();

        string assetPath;
        string[] temp;
        List<BuffType> types;
        Buff buff;

        foreach (UnityEngine.Object asset in selectedAssets)
        {
            // 獲取資源位置
            assetPath = AssetDatabase.GetAssetPath(asset);
            // 檢測資源用 若有不是 csv 檔的資源則報錯退出
            temp = assetPath.Split('/');
            if (temp[temp.Length - 1].Split('.')[1] != "csv")
            {
                Debug.LogError("選中的資源中有不是csv檔的資源 請重新選擇!");
                return;
            }

            // 讀出csv中所有資源
            string[] configs = File.ReadAllLines(assetPath);

            string[] configValues; // 0:Id(uint) , 1:Types(List<BuffType>) , 2:Name(string) , 3: MaxStack(int)
            // 將每行 BuffConfig 逐一放入 BuffConfigs中 從1開始讀 因為第0行是屬性行
            for (int i = 1; i < configs.Length; i++)
            {
                configValues = configs[i].Replace("\"", "").Split(',');
                if (configValues[0] == "") break;
                types = new List<BuffType>();
                foreach (string type in configValues[1].Split('/'))
                {
                    types.Add(Enum.Parse<BuffType>(type));
                }
                buff = new Buff(uint.Parse(configValues[0]), types, configValues[2], int.Parse(configValues[3]));
                buffConfigs.Add(buff.Id, buff);
            }
        }

        // 全部讀完後將資料轉成json格式並放進指定路徑中
        string jsonPath = Application.dataPath + "/ArtRes/Config/BuffConfig.json";
        string json = JsonUtility.ToJson(buffConfigs);
        File.WriteAllText(jsonPath, json);
        Debug.Log($"序列化完成, 已序列化{selectedAssets.Length}個文件為BuffConfig, 已經文件存儲於預設地址");
        AssetDatabase.Refresh();
    }
}