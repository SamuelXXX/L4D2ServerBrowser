using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIAppUpdate : PageUIBase
{
    #region Settings
    [Header("UI Reference")]
    public Button updateButton;
    public Button cancelButton;
    public Text updateInfoText;

    [Header("Page Reference")]
    public string downloadingPage;
    #endregion

    #region Page UI Life Cycle
    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        updateInfoText.text = AppUpdater.Instance.GetUpdateInfo();
        cancelButton.onClick.AddListener(OnExitPressed);
        updateButton.onClick.AddListener(OnUpdateButtonClicked);
    }

    protected override void OnDestroyPage()
    {
        updateButton.onClick.RemoveListener(OnUpdateButtonClicked);
        cancelButton.onClick.RemoveListener(OnExitPressed);
        base.OnDestroyPage();
    }
    #endregion

    #region Button Callbacks
    void OnUpdateButtonClicked()
    {
        PageUIManager.Instance.GoBack();
        PageUIManager.Instance.LoadPageByPageName(downloadingPage);
    }

    void OnExitPressed()
    {
        PageUIManager.Instance.GoBack();
    }
    #endregion
}
