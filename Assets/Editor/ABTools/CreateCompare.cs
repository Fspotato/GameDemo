using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class CreateCompare
{

    public static void CreateABCompareFile(string platform)
    {
        // 獲取文件夾信息
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + platform);
        // 獲取該目錄下的所有文件信息
        FileInfo[] fileInfos = directory.GetFiles();
        // 宣告一個StringBuilder
        StringBuilder abCompareInfo = new StringBuilder();
        // 遍歷文件信息
        foreach (FileInfo info in fileInfos)
        {
            // 如果後綴為空 (判斷AB包)
            if (info.Extension == "")
            {
                abCompareInfo.AppendJoin(" ", info.Name, info.Length, GetMD5(info.FullName));
                abCompareInfo.Append('|');
            }
        }
        // 刪除最後面的 '|'
        abCompareInfo.Remove(abCompareInfo.Length - 1, 1);
        // 儲存AB包對比文件
        File.WriteAllText(Application.dataPath + "/ArtRes/AB/" + platform + "/AbCompareInfo.txt", abCompareInfo.ToString());
        // 刷新編輯器中的Asset (讓他自動刷新 就不用手動刷新了)
        AssetDatabase.Refresh();
        Debug.Log("AB包對比文件生成成功!");
    }
    // 獲取MD5碼
    public static string GetMD5(string filePath)
    {
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            //申明一個MD5對象 用於生成MD5碼
            MD5 md5 = new MD5CryptoServiceProvider();
            //利用API 得到數據的MD5碼(16字節數組)
            byte[] md5Info = md5.ComputeHash(file);
            //關閉檔案 避免檔案溢出
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
