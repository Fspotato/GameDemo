using UnityEngine;
using UnityEditor;

public class ABTools : EditorWindow
{
    private int nowSelectedIndex = 0;
    private string[] targetStrings = new string[] { "PC", "IOS", "Android" };
    private string serverIP = "ftp://127.0.0.1";

    [MenuItem("Custom Tool/Asset Bundles Tool...")]
    private static void OpenWindow()
    {
        ABTools window = EditorWindow.GetWindowWithRect(typeof(ABTools), new Rect(0, 0, 350, 220)) as ABTools;
        window.Show();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 15), "平台選擇");
        // 頁籤顯示 從數組中取出字符串內容來顯示 所以需要改變當前索引
        nowSelectedIndex = GUI.Toolbar(new Rect(20, 30, 250, 20), nowSelectedIndex, targetStrings);
        // FTP服務器IP地址設置
        GUI.Label(new Rect(10, 60, 150, 15), "FTP Server IP");
        serverIP = GUI.TextField(new Rect(20, 80, 150, 20), serverIP);
        // 創建對比文件 按鈕
        if (GUI.Button(new Rect(10, 110, 100, 40), "創建對比文件"))
        {
            CreateCompare.CreateABCompareFile(targetStrings[nowSelectedIndex]);
        }
        // 保存默認資源到StreamingAssets 按鈕
        if (GUI.Button(new Rect(115, 110, 225, 40), "保存默認資源到StreamingAssets"))
        {
            MoveABtoSA.MoveABToStreamingAssets();
        }
        // 上傳AB包和對比文件到FTP服務器 按鈕
        if (GUI.Button(new Rect(10, 160, 330, 40), "上傳AB包和對比文件到FTP服務器"))
        {
            FTPServer.UploadAllABFiles(targetStrings[nowSelectedIndex], serverIP);
        }
    }
}
