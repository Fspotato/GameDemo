using UnityEngine;
using UnityEditor;
using System.IO;

public class MoveABtoSA
{
    public static void MoveABToStreamingAssets()
    {
        // 通過編輯器的 Selection類 中的方法 獲取在Project窗口中選中的資源
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        // 如果一個都沒有選 就直接跳出 沒有必要執行後面的邏輯
        if (selectedAssets.Length == 0) return;

        // 用於拼接本地默認AB包資源信息的字符串
        string abCompareInfo = "";

        // 遍歷選中的資源
        foreach (Object asset in selectedAssets)
        {
            // 通過 AssetDatabase類 獲取資源的路徑
            string assetPath = AssetDatabase.GetAssetPath(asset);
            // 擷取路徑當中的文件名 作為StreamAssets中的文件名
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));
            if (fileName.IndexOf('.') != -1) continue;
            // 利用 AssetDatabase類 中的API 將資源複製到目標路徑
            AssetDatabase.CopyAsset(assetPath, "Assets/StreamingAssets" + fileName);
            // 獲取複製到 StreamingAssets 的文件中的全部信息
            FileInfo fileInfo = new FileInfo(Application.streamingAssetsPath + fileName);
            // 拼接AB包信息到字符串中
            abCompareInfo += fileInfo.Name + " " + fileInfo.Length + " " + CreateCompare.GetMD5(fileInfo.FullName) + "|";
        }
        // 把字符串最後的 | 刪除 避免不必要的麻煩
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
        // 把拼接好的字符串寫入本地的資源對比文件當中
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo);
        Debug.Log("AB包文件已轉移至StreamingAssets");
        AssetDatabase.Refresh();
    }
}
