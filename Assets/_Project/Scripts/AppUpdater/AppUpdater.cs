using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
using UnityEngine.Networking;
using PowerInspector;
using System.Text.RegularExpressions;

public class AppUpdater : ShortLifeSingleton<AppUpdater>
{
    #region Definations
    [System.Serializable]
    public class AppUpdateConfig
    {
        public string version;
        [ReadOnly]
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
    public string rootPath;
    public AppUpdateConfig appConfig;
    [ReadOnly]
    public string urlAppUpdateConfigPath;
    #endregion

    #region Runtime Data
    protected AppUpdateConfig remoteConfig = null;
    AndroidJavaObject apkInstaller = null;
    AndroidJavaObject APKInstaller
    {
        get
        {
            if(apkInstaller==null)
            {
                apkInstaller=new AndroidJavaObject("com.SamaelXXX.APKTools.APKInstaller");
            }
            return apkInstaller;
        }
    }  
    
    string ApkStorePath
    {
        get
        {
            string path = Application.persistentDataPath;
            return path + "/L4D2ServerBrowser.apk";
        }
    }
    #endregion

    #region Unity Life Cycle
    private void OnValidate()
    {
#if UNITY_EDITOR
        PlayerSettings.bundleVersion = appConfig.version;
        appConfig.urlApkDownloadingPath = rootPath + "Android/L4D2ServerBrowser.apk";
        urlAppUpdateConfigPath = rootPath + "AppUpdateConfig.txt";
        EditorUtility.SetDirty(this);
#endif
    }
    #endregion

    public void CommitAppUpdateRequest()
    {
        //Toast("Commit App Update Request");
        WebRequestAgent.Instance.Get(urlAppUpdateConfigPath, OnReceiveAppUpdateData);
    }

    void OnReceiveAppUpdateData(WebRequestAgent.WebResponseData data)
    {
        if (data.responseType != WebRequestAgent.ResponseDataType.Text)
        {
            Debug.LogError("Downloading app update data failed!");
            return;
        }

        remoteConfig = JsonUtility.FromJson<AppUpdateConfig>(data.text);
    }
    #region Exposed API

    protected Action<float> downloadProgressHandler;
    protected Action downloadSucceedHandler;
    protected Action<string> downloadFailedHandler;

    public bool CheckNeedUpdate()
    {
        if (remoteConfig == null || string.IsNullOrEmpty(remoteConfig.version))
        {
            Debug.LogError("App Update Config Invalid!");
            return false;
        }

        if (remoteConfig.VersionNewerThan(appConfig.version))
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
        if (remoteConfig == null || string.IsNullOrEmpty(remoteConfig.version))
        {
            Debug.LogError("App Update Config Invalid!");
            return null;
        }

        if (remoteConfig.VersionNewerThan(appConfig.version))
        {
            return remoteConfig.updateInfo;
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

        if (remoteConfig == null || string.IsNullOrEmpty(remoteConfig.version))
        {
            Debug.LogError("App Update Config Invalid!");
            return false;
        }

        if (remoteConfig.VersionNewerThan(appConfig.version))
        {
            StartCoroutine(DownloadAPK(remoteConfig.urlApkDownloadingPath));
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
        string path = ApkStorePath;
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);

        ulong length = 0;
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(apkDownloadPath);
            www.SetRequestHeader("Range", "bytes=" + length + "-");

            yield return www.SendWebRequest();

            string contentRange = www.GetResponseHeader("Content-Range");
            Debug.Log("Content-Range:" + contentRange);
            ContentRangeHandler crh = new ContentRangeHandler(contentRange);
            length += www.downloadedBytes;

            downloadProgressHandler?.Invoke(crh.end / (float)crh.total);

            bw.Write(www.downloadHandler.data);

            if (crh.IsFinalChunk())
                break;
        }

        bw.Close();
        fs.Close();


        downloadSucceedHandler?.Invoke();
    }

    public class ContentRangeHandler
    {
        public int start;
        public int end;
        public int total;
        public ContentRangeHandler(string responseHeader)
        {
            string[] pars = responseHeader.Split(' ', '-', '/');
            if (pars.Length != 4)
            {
                Debug.LogError("Content Range Length Not Matched");
                return;
            }

            start = int.Parse(pars[1]);
            end = int.Parse(pars[2]);
            total = int.Parse(pars[3]);
        }

        public bool IsFinalChunk()
        {
            return end == total - 1;
        }
    }

    public void InstallNewVersionAPK()
    {
        if (APKInstaller != null)
        {
            APKInstaller.Call("InstallApk", "com.SamaelXXX.L4D2ServerBrowser", ApkStorePath);
        }
    }

    public void Toast(string content)
    {
        if(APKInstaller!=null)
        {
            APKInstaller.Call<bool>("showToast", content);
        }
    }

#if UNITY_EDITOR
    public PowerButton button = new PowerButton("Build Update", "BuildUpdate", 50);
    public void BuildUpdate()
    {
        BuildAPK();
        BuildJSON();
    }

    void BuildAPK()
    {

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/_Project/Scenes/main.unity"};
        buildPlayerOptions.locationPathName="AppUpdate/Android/L4D2ServerBrowser.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        var report=BuildPipeline.BuildPlayer(buildPlayerOptions);
        var summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }

    }

    string GetTaildDataPath()
    {
        string assetsPath = Application.dataPath;
        Regex assetsPathRegExp = new Regex("/Assets$");

        return assetsPathRegExp.Replace(assetsPath, "");
    }

    void BuildJSON()
    {
        string projectPath = GetTaildDataPath();
        string jsonStorageFilePath = projectPath + "/AppUpdate/AppUpdateConfig.txt";

        try
        {
            CreateFile(jsonStorageFilePath, JsonFormatter.FormatJson(JsonUtility.ToJson(appConfig)));
            Debug.Log("Bundle Manifest file generated succeed@" + jsonStorageFilePath);
        }
        catch (Exception)
        {
            Debug.Log("Bundle Manifest file generated failed@" + jsonStorageFilePath);
        }
    }

    void CreateFile(string _filePath, string _data)
    {
        FileInfo fi = new FileInfo(_filePath);

        if (fi.Exists)
            fi.Delete();

        using (StreamWriter sw = fi.CreateText())
        {
            sw.Write(_data);
            sw.Close();
        }
    }
#endif
    #endregion
}
