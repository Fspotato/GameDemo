using UnityEngine;
using System.IO;
using System.Net;
using System;
using System.Threading.Tasks;

public class FTPServer
{
    public static void UploadAllABFiles(string platform, string serverIP)
    {
        DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath + "/ArtRes/AB/" + platform);
        FileInfo[] fileInfos = directory.GetFiles();
        foreach (FileInfo info in fileInfos)
        {
            if (info.Extension == "" || info.Extension == ".txt")
            {
                FtpUploadFile(info.FullName, info.Name, platform, serverIP);
            }
        }
    }

    // 上傳資料
    private async static void FtpUploadFile(string filePath, string fileName, string platform, string serverIP)
    {
        await Task.Run(() =>
            {
                // 創立一個FTP連接
                FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AB/" + platform + "/" + fileName)) as FtpWebRequest;
                // 設置一個通信憑證 並更換req的通信憑證
                NetworkCredential acc = new NetworkCredential("Fspotato", "qwe142857");
                req.Credentials = acc;
                // 設置代理為null
                req.Proxy = null;
                // 上傳完關閉連接
                req.KeepAlive = false;
                // 操作命令 - 上傳
                req.Method = WebRequestMethods.Ftp.UploadFile;
                // 指定傳書類型 - 二進制
                req.UseBinary = true;

                Stream upLoadStream = req.GetRequestStream();
                using (FileStream file = File.OpenRead(filePath))
                {
                    // 一點一點的上傳內容
                    byte[] bytes = new byte[2048];
                    // 讀取了多少個字節
                    int contentLength = file.Read(bytes, 0, bytes.Length);

                    // 循環上傳文件中的信息
                    while (contentLength != 0)
                    {
                        // 寫入到上傳流中
                        upLoadStream.Write(bytes, 0, contentLength);
                        // 寫完再讀
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }

                    //循環完畢後證明上傳結束
                    file.Close();
                    upLoadStream.Close();
                }
                Debug.Log(fileName + "上傳成功!");
            });
    }
}
