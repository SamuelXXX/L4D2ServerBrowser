using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIMessageOfTheDay : PageUIBase
{
    #region Settings
    public string contentUrl;

    [Header("UI Reference")]
    public Text contentText;
    public Button backButton;
    #endregion

    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        backButton.onClick.AddListener(OnExitButtonClicked);
        contentText.text = "内容加载中...";
        WebRequestAgent.Instance.Get(contentUrl, OnReceiveMOTDContent);
    }

    protected override void OnDestroyPage()
    {
        base.OnDestroyPage();
        backButton.onClick.RemoveListener(OnExitButtonClicked);
    }

    void OnExitButtonClicked()
    {
        PageUIManager.Instance.GoBack();
    }

    void OnReceiveMOTDContent(WebRequestAgent.WebResponseData data)
    {
        if (data.responseType == WebRequestAgent.ResponseDataType.Text)
        {
            contentText.text = data.text;
        }
        else
        {
            contentText.text = "内容加载失败！！！";
        }
    }
}
