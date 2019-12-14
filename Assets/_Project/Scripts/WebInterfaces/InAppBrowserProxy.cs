using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InAppBrowserBridge))]
public class InAppBrowserProxy : ShortLifeSingleton<InAppBrowserProxy>
{
    #region Component Reference
    public bool pongMessage = false;
    protected InAppBrowserBridge m_Bridge;
    #endregion

    #region Life Cycle
    protected override void Awake()
    {
        base.Awake();
        m_Bridge = GetComponent<InAppBrowserBridge>();
        if (pongMessage)
        {
            m_Bridge.onJSCallback.AddListener(PongMessage);
        }
    }

    private void Update()
    {
        if (closeCommand)
        {
#if UNITY_IOS
            Invoke("iOSDelayOnBrowserClosed", 0.5f);
#endif
            InAppBrowser.CloseBrowser();
            closeCommand = false;
        }
    }

    private void LateUpdate()
    {
        while (jsCodeQueue.Count != 0)
        {
            InAppBrowser.ExecuteJS(jsCodeQueue.Dequeue());
        }
    }

    void iOSDelayOnBrowserClosed()
    {
        m_Bridge.onBrowserClosed.Invoke();
    }
    #endregion

    #region Bridge Communication
    public Queue<string> jsCodeQueue = new Queue<string>();
    public void ExecuteJS(string javaScriptCode)
    {
        Debug.Log("InAppBrowserExcuteJS:" + javaScriptCode);
        jsCodeQueue.Enqueue(javaScriptCode);
    }

    public void PongMessage(string message)
    {
        InAppBrowser.ExecuteJS("alert('" + message + "')");
    }

    public void RegisterJSMessageHandler(UnityAction<string> handler)
    {
        if (handler != null && handler.Target != null)
            m_Bridge.onJSCallback.AddListener(handler);
    }

    public void UnregisterJSMessageHandler(UnityAction<string> handler)
    {
        m_Bridge.onJSCallback.RemoveListener(handler);
    }

    public void UnregisterAllJSMessageHandlers()
    {
        m_Bridge.onJSCallback.RemoveAllListeners();
    }

    public void RegisterOnCloseHandler(UnityAction handler)
    {
        if (handler != null && handler.Target != null)
            m_Bridge.onBrowserClosed.AddListener(handler);
    }

    public void UnregisterOnCloseHandler(UnityAction handler)
    {
        m_Bridge.onBrowserClosed.RemoveListener(handler);
    }

    public void UnregisterAllOnCloseHandlers()
    {
        m_Bridge.onBrowserClosed.RemoveAllListeners();
    }


    public void RegisterAndroidBackButtonHandler(UnityAction handler)
    {
        if (handler != null && handler.Target != null)
            m_Bridge.onAndroidBackButtonPressed.AddListener(handler);
    }

    public void UnregisterAndroidBackButtonHandler(UnityAction handler)
    {
        m_Bridge.onAndroidBackButtonPressed.RemoveListener(handler);
    }

    public void UnregisterAllAndroidBackButtonHandlers()
    {
        m_Bridge.onAndroidBackButtonPressed.RemoveAllListeners();
    }
    #endregion

    #region Browser Operation
    public void OpenBrowser(string uri,
        UnityAction<string> startLoadingAction,
        UnityAction<string> finishLoadingAction)
    {
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();

        options.backButtonText = "回到主页";
        options.displayURLAsPageTitle = false;
        options.barBackgroundColor = "#D1D1D1";
        options.shouldStickToPortrait = true;


        //options.hidesTopBar = true;
        options.androidBackButtonCustomBehaviour = true;



        m_Bridge.onBrowserStartedLoading.RemoveAllListeners();
        m_Bridge.onBrowserFinishedLoading.RemoveAllListeners();

        if (startLoadingAction != null)
            m_Bridge.onBrowserStartedLoading.AddListener(startLoadingAction);

        if (finishLoadingAction != null)
            m_Bridge.onBrowserFinishedLoading.AddListener(finishLoadingAction);


        InAppBrowser.OpenURL(uri, options);
    }

    //Async close browser to avoid multi-thread problem
    bool closeCommand = false;
    public void CloseBrowser()
    {
        Debug.Log("InAppBrowser:" + "Manual Close Browser!");
        closeCommand = true;
    }
    #endregion
}
