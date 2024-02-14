using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ABManager : BaseManager<ABManager>
{
    // 主包
    private AssetBundle mainAB = null;
    // 依賴包獲取用的配置文件
    private AssetBundleManifest manifest = null;
    // AB包存放路徑 這樣的寫法方便之後修改路徑
    private string PathUrl => Application.streamingAssetsPath + "/";
    private string MainABName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#else
            return "PC";
#endif
        }
    }

    public Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    void OnApplicationQuit()
    {
        ClearAB();
    }

    // 同步加載
    public Object LoadRes(string abName, string resName)
    {
        LoadAB(abName);
        // 為了外面方便 在加載資源時判斷是否是GameObject
        Object obj = abDic[abName].LoadAsset(resName);
        if (obj is GameObject) return Instantiate(obj);
        else return obj;
    }

    // 同步加載 根據類型加載 (Lua支援)
    public Object LoadRes(string abName, string resName, System.Type type)
    {
        LoadAB(abName);
        Object obj = abDic[abName].LoadAsset(resName, type);
        if (obj is GameObject) return Instantiate(obj);
        else return obj;
    }

    // 同步加載 根據泛型加載 (Lua不支援)
    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        LoadAB(abName);
        T obj = abDic[abName].LoadAsset<T>(resName);
        if (obj is GameObject) return Instantiate(obj);
        else return obj;
    }

    // 異步加載
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        StartCoroutine(LoadABAsync(abName, (isOver) =>
            {
                if (isOver) StartCoroutine(IELoadResAsync(abName, resName, callBack));
            }
        ));
    }
    private IEnumerator IELoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
        yield return abr;
        if (abr.asset is GameObject) callBack(Instantiate(abr.asset));
        else callBack(abr.asset);
    }

    // 異步加載 根據類型加載 (Lua支援)
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        StartCoroutine(LoadABAsync(abName, (isOver) =>
             {
                 if (isOver) StartCoroutine(IELoadResAsync(abName, resName, type, callBack));
             }
         ));
    }
    private IEnumerator IELoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
        yield return abr;
        if (abr.asset is GameObject) callBack(Instantiate(abr.asset));
        else callBack(abr.asset);
    }

    // 異步加載 根據泛型加載 (Lua不支援)
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        StartCoroutine(LoadABAsync(abName, (isOver) =>
            {
                if (isOver) StartCoroutine(IELoadResAsync<T>(abName, resName, callBack));
            }
        ));
    }
    private IEnumerator IELoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        if (abr.asset is GameObject) callBack(Instantiate(abr.asset as T));
        else callBack(abr.asset as T);
    }


    // 指定AB包卸載
    public void UnLoad(string abName)
    {
        if (abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }

    // 卸載所有AB包
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }

    // 同步加載依賴包及AB包
    private void LoadAB(string abName)
    {
        // 加載主包 (根據平台不同有不同的主包)
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        // 獲取依賴包相關信息 (根據主包文件判斷)
        string[] depends = manifest.GetAllDependencies(abName);
        AssetBundle ab;
        for (int i = 0; i < depends.Length - 1; i++)
        {
            // 判斷是否被加載過
            if (!abDic.ContainsKey(depends[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + depends[i]);
                abDic.Add(depends[i], ab);
            }
        }

        // 加載要用的包
        // 如果沒被加載過再加載
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }
    }

    // 異步加載依賴包及AB包
    private IEnumerator LoadABAsync(string abName, UnityAction<bool> callBack)
    {
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFileAsync(PathUrl + MainABName).assetBundle;
            yield return mainAB;
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        string[] depends = manifest.GetAllDependencies(abName);
        AssetBundle ab;
        for (int i = 0; i < depends.Length - 1; i++)
        {
            // 判斷是否被加載過
            if (!abDic.ContainsKey(depends[i]))
            {
                ab = AssetBundle.LoadFromFileAsync(PathUrl + depends[i]).assetBundle;
                yield return ab;
                abDic.Add(depends[i], ab);
            }
        }

        // 加載要用的包
        // 如果沒被加載過再加載
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFileAsync(PathUrl + abName).assetBundle;
            yield return ab;
            abDic.Add(abName, ab);
        }
        callBack(true);
    }
}