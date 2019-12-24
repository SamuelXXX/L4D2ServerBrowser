using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIQueryServerList : PageUIBase
{
    #region Settings
    [Header("UI Reference")]
    public Text infoText;
    public Button refreshButton;
    public Button continueButton;

    [Header("Page Reference")]
    public string continuePage;
    #endregion

    #region Page Life Cycle
    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        infoText.text = "正在获取服务器列表...";
        refreshButton.onClick.AddListener(OnRefreshButtonPressed);
        continueButton.onClick.AddListener(OnContinueButtonPressed);
        refreshButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        IPListManager.Instance.CommitServerInfoRequest(OnReceiveServerInfoList);
    }

    protected override void OnDestroyPage()
    {
        base.OnDestroyPage();
        refreshButton.onClick.RemoveListener(OnRefreshButtonPressed);
        continueButton.onClick.RemoveListener(OnContinueButtonPressed);
    }
    #endregion

    #region UI Button Callbacks
    void OnRefreshButtonPressed()
    {
        infoText.text = "正在获取服务器列表...";
        refreshButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        Invoke("CommitIPListRequest", 1f);
    }

    void CommitIPListRequest()
    {
        IPListManager.Instance.CommitServerInfoRequest(OnReceiveServerInfoList);
    }

    void OnContinueButtonPressed()
    {
        PageUIManager.Instance.LoadPageByPageName(continuePage, receiveServerList);
    }
    #endregion

    List<IPData> receiveServerList;
    void OnReceiveServerInfoList(List<IPData> list, bool remoteQuerySucceed)
    {
        if (remoteQuerySucceed)
        {
            infoText.text = "";
            refreshButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);
            PageUIManager.Instance.LoadPageByPageName(continuePage, list);
        }
        else
        {
            infoText.text = "云端服务器列表查询失败\n是否继续？";
            refreshButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
            receiveServerList = list;
        }
    }
}
