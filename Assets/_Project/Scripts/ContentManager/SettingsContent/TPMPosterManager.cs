using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TPMPosterManager : ShortLifeSingleton<TPMPosterManager>
{
    #region Definations
    public class AssetBundleDownloadResult
    {
        public enum ResultCode
        {
            Error = -1,
            Failed = 0,
            Succeed
        }

        public ResultCode resultCode;
        public AssetBundle bundle;

        public AssetBundleDownloadResult(ResultCode code, AssetBundle bundle)
        {
            this.resultCode = code;
            this.bundle = bundle;
        }

    }
    #endregion

    #region Settings
    public string urlThirdPartyBundleForAndroid;
    public string urlThirdPartyBundleForIOS;

    string urlThirdPartyMap
    {
        get
        {
#if UNITY_ANDROID
            return urlThirdPartyBundleForAndroid;
#elif UNITY_IOS
            return urlThirdPartyBundleForIOS;
#else
            return "";
#endif
        }
    }
    #endregion

    #region Exposed API
    bool inCommitingProcess = false;

    public void CommitThirdPartyMapRequest()
    {
        if (inCommitingProcess)
        {
            return;
        }

        StartCoroutine(DownloadRoutine());
    }

    IEnumerator DownloadRoutine()
    {
        inCommitingProcess = true;
        yield return DownloadBundleRoutine(urlThirdPartyMap, OnRecieveThirdPartyMap);
        inCommitingProcess = false;
    }

    IEnumerator DownloadBundleRoutine(string bundleUrl, Action<AssetBundleDownloadResult> resultHandler, Action<float> progressHandler = null)
    {
        string url = bundleUrl;
        Debug.Log("Download Bundle From:" + url);
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

        while (!asyncOperation.isDone)
        {
            progressHandler?.Invoke(asyncOperation.progress);
            yield return null;
        }

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
            resultHandler?.Invoke(new AssetBundleDownloadResult(AssetBundleDownloadResult.ResultCode.Error, null));
            yield break;
        }

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        if (resultHandler != null)
        {
            if (bundle != null)
            {
                resultHandler(new AssetBundleDownloadResult(AssetBundleDownloadResult.ResultCode.Succeed, bundle));
                Debug.Log("Download Bundle " + bundleUrl + " Succeed@" + url);
            }
            else
            {
                resultHandler(new AssetBundleDownloadResult(AssetBundleDownloadResult.ResultCode.Failed, null));
            }
        }
    }

    void OnRecieveThirdPartyMap(AssetBundleDownloadResult result)
    {
        if (result.resultCode == AssetBundleDownloadResult.ResultCode.Succeed)
            MapContentMapper.ThirdPartyInstance = result.bundle.LoadAsset<MapContentMapper>("MapInfo");
    }
    #endregion
}
