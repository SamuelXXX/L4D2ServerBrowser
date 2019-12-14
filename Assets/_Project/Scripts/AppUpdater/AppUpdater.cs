using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

public class AppUpdater : ShortLifeSingleton<AppUpdater>
{
    #region Definations
    [System.Serializable]
    public class AppUpdateConfig
    {
        public string version;
        public string urlApkDownloadingPath;
        public string updateInfo;

        public class VersionHolder
        {
            public int main = 0;
            public int second = 0;
            public int patch = 0;
            public VersionHolder(string version)
            {
                string[] vl = version.Split('.');
                if (vl.Length >= 1)
                {
                    main = int.Parse(vl[0]);
                }

                if (vl.Length >= 2)
                {
                    second = int.Parse(vl[1]);
                }

                if (vl.Length >= 3)
                {
                    patch = int.Parse(vl[2]);
                }
            }

            public static bool operator >(VersionHolder a, VersionHolder b)
            {
                if (a.main < b.main)
                    return false;

                if (a.main > b.main)
                    return true;

                if (a.second < b.second)
                    return false;

                if (a.second > b.second)
                    return true;

                if (a.patch < b.patch)
                    return false;

                if (a.patch > b.patch)
                    return true;


                return false;
            }

            public static bool operator <(VersionHolder a, VersionHolder b)
            {
                if (a.main < b.main)
                    return true;

                if (a.main > b.main)
                    return false;

                if (a.second < b.second)
                    return true;

                if (a.second > b.second)
                    return false;

                if (a.patch < b.patch)
                    return true;

                if (a.patch > b.patch)
                    return false;


                return false;
            }
        }

        public bool VersionNewerThan(string compared)
        {
            VersionHolder source = new VersionHolder(version);
            VersionHolder dest = new VersionHolder(compared);
            return source > dest;
        }
    }
    #endregion


    #region Settings
    public string appVersion = "2.0";
    public string urlAppUpdateConfigPath;
    #endregion

    #region Runtime Data
    protected AppUpdateConfig config = null;
    #endregion

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        WebRequestAgent.Instance.Get(urlAppUpdateConfigPath, OnReceiveAppUpdateData);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        PlayerSettings.bundleVersion = appVersion;
#endif
    }
    #endregion

    void OnReceiveAppUpdateData(WebRequestAgent.WebResponseData data)
    {
        if (data.responseType != WebRequestAgent.ResponseDataType.Text)
        {
            Debug.LogError("Downloading app update data failed!");
            return;
        }

        config = JsonUtility.FromJson<AppUpdateConfig>(data.text);
    }



    #region Exposed API

    protected Action<float> downloadProgressHandler;
    protected Action downloadSucceedHandler;
    protected Action<string> downloadFailedHandler;

    public bool CheckNeedUpdate()
    {
        if (config == null || string.IsNullOrEmpty(config.version))
        {
            Debug.LogError("App Update Config Invalid!");
            return false;
        }

        if (config.VersionNewerThan(appVersion))
        {
            return true;
        }
        else
        {
            Debug.Log("Is newest version!");
            return false;
        }
    }

    public string GetUpdateInfo()
    {
        if (config == null || string.IsNullOrEmpty(config.version))
        {
            Debug.LogError("App Update Config Invalid!");
            return null;
        }

        if (config.VersionNewerThan(appVersion))
        {
            return config.updateInfo;
        }
        else
        {
            Debug.Log("Is newest version!");
            return null;
        }
    }

    public bool PerformDownloadRoutine(Action<float> progressHandler, Action succeedHandler = null, Action<string> failedHandler = null)
    {
        downloadProgressHandler = progressHandler;
        downloadSucceedHandler = succeedHandler;
        downloadFailedHandler = failedHandler;

        if (config == null || string.IsNullOrEmpty(config.version))
        {
            Debug.LogError("App Update Config Invalid!");
            return false;
        }

        if (config.VersionNewerThan(appVersion))
        {
            StartCoroutine(DownloadAPK(config.urlApkDownloadingPath));
            return true;
        }
        else
        {
            Debug.Log("Is newest version!");
            return false;
        }
    }

    IEnumerator DownloadAPK(string apkDownloadPath)
    {
        UnityWebRequest www = UnityWebRequest.Get(apkDownloadPath);

        www.SendWebRequest();

        //下载需要更新的apk
        while (true)
        {
            downloadProgressHandler?.Invoke(www.downloadProgress);
            if (www.isDone)
            {
                break;
            }
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            downloadFailedHandler?.Invoke(www.error);
            yield return null;
        }

        //将apk写入沙盒目录
        string path = Application.persistentDataPath + "/L4D2ServerBrowser.apk";

        try
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(www.downloadHandler.data);
            bw.Close();
            fs.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        downloadSucceedHandler?.Invoke();
    }

    public void InstallNewVersionAPK()
    {
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        bool b = currentActivity.Call<bool>("installAPK", Application.persistentDataPath + "/L4D2ServerBrowser.apk");//APK路径

    }
    #endregion
}
