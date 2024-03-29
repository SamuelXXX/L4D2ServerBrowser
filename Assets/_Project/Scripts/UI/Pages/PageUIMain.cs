﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIMain : PageUIBase
{
    #region Settings
    [Header("UI Component Reference")]
    public ServerInfoDisplayUIManager serverUIContainer;
    public Button ipSettingButton;
    public Button specialCareSettingsButton;
    public Button discoverButton;

    [Header("Page Settings")]
    public string ipSettingsPage;
    public string specialCareSettingsPage;
    public string discoverPage;
    public string appUpdate;
    #endregion

    #region Page UI Callbacks
    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);

        if (pars.Length == 1)
        {
            TPMPosterManager.Instance.CommitThirdPartyMapRequest();

            StopAllCoroutines();
            StartCoroutine(CommitRequestsRoutine());
#if UNITY_ANDROID
            StartCoroutine(AppUpdatePollingRoutine()); // Work only on android platform
#endif

            List<IPData> serverList = pars[0] as List<IPData>;
            L4D2QueryAgentManager.Instance.StartQuery(serverList);//Create Query Agents
            serverUIContainer.GenerateUIGroup(L4D2QueryAgentManager.Instance.agents);//Generate UI Group
        }

        ipSettingButton.onClick.AddListener(OnIPSettingsPressed);
        specialCareSettingsButton.onClick.AddListener(OnSpecialCareSettingsPressed);
        discoverButton.onClick.AddListener(OnDiscoverPressed);

    }

    public override void OnResume()
    {
        base.OnResume();
        if (PageUIIPSettings.ipDirty)
        {
            PageUIIPSettings.ipDirty = false;
            RefreshPage();
        }
        VipIDManager.Instance.CommitVipIDRequest(OnReceiveVipIDList);
    }

    protected override void OnDestroyPage()
    {
        base.OnDestroyPage();
        ipSettingButton.onClick.RemoveListener(OnIPSettingsPressed);
        specialCareSettingsButton.onClick.RemoveListener(OnSpecialCareSettingsPressed);
        discoverButton.onClick.RemoveListener(OnDiscoverPressed);
    }
    #endregion

    #region APP Update Polling
    IEnumerator AppUpdatePollingRoutine()
    {
        while (true)
        {
            if (AppUpdater.Instance.CheckNeedUpdate())
            {
                PageUIManager.Instance.LoadPageByPageName(appUpdate);
                yield break;
            }
            yield return new WaitForSeconds(5f);
        }
    }
    #endregion

    #region Internal Tool Methods
    IEnumerator CommitRequestsRoutine()
    {
        while (true)
        {
            CommitRequests();
            yield return new WaitForSeconds(10f);
        }
    }

    void CommitRequests()
    {
        VipIDManager.Instance.CommitVipIDRequest(OnReceiveVipIDList);
        AppUpdater.Instance.CommitAppUpdateRequest();
    }

    void RefreshPage()
    {
        L4D2QueryAgentManager.Instance.StopQuery();
        serverUIContainer.DestroyUIGroup();
        IPListManager.Instance.CommitServerInfoRequest(OnReceiveServerInfoList);
    }

    void RequestForVipID()
    {
        VipIDManager.Instance.CommitVipIDRequest(OnReceiveVipIDList);
    }
    #endregion

    #region Page Jumping
    void OnIPSettingsPressed()
    {
        PageUIManager.Instance.LoadPageByPageName(ipSettingsPage);
    }

    void OnSpecialCareSettingsPressed()
    {
        PageUIManager.Instance.LoadPageByPageName(specialCareSettingsPage);
    }

    void OnDiscoverPressed()
    {
        PageUIManager.Instance.LoadPageByPageName(discoverPage);
    }
    #endregion

    #region External Data Source Response Handler
    void OnReceiveServerInfoList(List<IPData> list, bool remoteQuerySucceed)
    {
        L4D2QueryAgentManager.Instance.StartQuery(list);//Create Query Agents
        serverUIContainer.GenerateUIGroup(L4D2QueryAgentManager.Instance.agents);//Generate UI Group
    }

    void OnReceiveVipIDList(List<IDDecorator> list, bool remoteQuerySucceed)
    {
        PlayerIDManager.Instance.BuildVipIDTable(list);
    }
    #endregion
}
