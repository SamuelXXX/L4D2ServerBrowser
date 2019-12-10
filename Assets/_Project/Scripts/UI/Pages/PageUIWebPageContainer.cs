using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageUIWebPageContainer : PageUIBase
{
    #region Settings
    [Header("Web Page Setting")]
    public string webPageUrl;
    public BackButtonAction backAction;
    #endregion

    #region Internal Methods
    protected List<PageIndex> GetAllPages()
    {
        if (PageUIManager.Instance != null)
        {
            return PageUIManager.Instance.GetAllPages();
        }
        else
        {
            return new List<PageIndex>();
        }
    }
    #endregion

    #region Page Life Cycle
    protected override void OnLoadPage(params object[] pars)
    {
        closed = false;
        InAppBrowser.ClearCache();

        InAppBrowserProxy.Instance.UnregisterAllJSMessageHandlers();
        InAppBrowserProxy.Instance.RegisterJSMessageHandler(OnReceiveBrowserMessage);

        InAppBrowserProxy.Instance.UnregisterAllOnCloseHandlers();
        InAppBrowserProxy.Instance.RegisterOnCloseHandler(OnBrowserClosed);

        InAppBrowserProxy.Instance.OpenBrowser(webPageUrl, null, OnWebPageSuccessfullyLoaded);

        StartCoroutine(BrowserCloseDetectRoutine());
    }



    protected override void OnDestroyPage()
    {

    }
    #endregion

    IEnumerator BrowserCloseDetectRoutine()
    {
        if (Application.isEditor)
        {
            yield return new WaitForSeconds(2f);
            OnBrowserClosed();
        }
        else
        {
            while (!InAppBrowser.IsInAppBrowserOpened())
            {
                yield return null;
            }

            while (InAppBrowser.IsInAppBrowserOpened())
            {
                yield return null;
            }

            OnBrowserClosed();
        }
    }

    #region Web Browser Page Callbacks
    bool closed = true;
    protected void OnBrowserClosed()
    {
        if (!closed)
        {
            switch (backAction)
            {
                case BackButtonAction.GoBack:
                    PageUIManager.Instance.GoBack();
                    break;
                case BackButtonAction.QuitApp:
                    PageUIManager.Instance.GoBack();
                    break;
                default: break;
            }
        }
        closed = true;
    }
    #endregion

    protected virtual void OnReceiveBrowserMessage(string message)
    {
        if (message == "exit")
        {
            InAppBrowserProxy.Instance.CloseBrowser();
        }
    }

    protected virtual void OnWebPageSuccessfullyLoaded(string url)
    {

    }
}
