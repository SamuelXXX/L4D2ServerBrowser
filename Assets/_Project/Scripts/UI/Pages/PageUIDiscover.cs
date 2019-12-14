using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIDiscover : PageUIBase
{
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

        InAppBrowserProxy.Instance.OpenBrowser(RCUrlManager.Instance.settings.urlDiscover, null, OnWebPageSuccessfullyLoaded);

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
            PageUIManager.Instance.GoBack();
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
