using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIDownloadProgress : PageUIBase
{
    #region Settings
    [Header("UI Reference")]
    public RectTransform progressFill;
    public float maxWidth;
    #endregion

    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        AppUpdater.Instance.PerformDownloadRoutine(OnDownloadProgressChanged, OnDownloadSucceed);
    }

    void OnDownloadProgressChanged(float progress)
    {
        //Debug.Log("Progress " + progress);
        progressFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth * progress);
    }

    void OnDownloadSucceed()
    {
        PageUIManager.Instance.GoBack();
        AppUpdater.Instance.InstallNewVersionAPK();        
    }
}
