using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIMain : PageUIBase
{
    [Header("UI Settings")]
    public ServerInfoDisplayUIManager serverUIContainer;
    public Button ipSettingButton;
    public Button specialCareSettingsButton;
    public Button motdButton;
    public string ipSettingsPage;
    public string specialCareSettingsPage;
    public string motdPage;
    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        if (pars.Length == 1)
        {
            List<IPData> serverList = pars[0] as List<IPData>;
            L4D2QueryAgentManager.Instance.StartQuery(serverList);//Create Query Agents
            serverUIContainer.GenerateUIGroup(L4D2QueryAgentManager.Instance.agents);//Generate UI Group
        }
        ipSettingButton.onClick.AddListener(OnIPSettingsPressed);
        specialCareSettingsButton.onClick.AddListener(OnSpecialCareSettingsPressed);
        motdButton.onClick.AddListener(OnMOTDPressed);
        VipIDManager.Instance.CommitVipIDRequest(OnReceiveVipIDList);
    }

    public override void OnResume()
    {
        base.OnResume();
        if (PageUIIPSettings.ipDirty)
        {
            PageUIIPSettings.ipDirty = false;

            L4D2QueryAgentManager.Instance.StopQuery();
            serverUIContainer.DestroyUIGroup();
            IPListManager.Instance.CommitServerInfoRequest(OnReceiveServerInfoList);
        }
        VipIDManager.Instance.CommitVipIDRequest(OnReceiveVipIDList);
    }

    protected override void OnDestroyPage()
    {
        base.OnDestroyPage();
        ipSettingButton.onClick.RemoveListener(OnIPSettingsPressed);
        specialCareSettingsButton.onClick.RemoveListener(OnSpecialCareSettingsPressed);
        motdButton.onClick.RemoveListener(OnMOTDPressed);
    }

    void OnIPSettingsPressed()
    {
        PageUIManager.Instance.LoadPageByPageName(ipSettingsPage);
    }

    void OnSpecialCareSettingsPressed()
    {
        PageUIManager.Instance.LoadPageByPageName(specialCareSettingsPage);
    }

    void OnMOTDPressed()
    {
        PageUIManager.Instance.LoadPageByPageName(motdPage);
    }

    void OnReceiveServerInfoList(List<IPData> list, bool remoteQuerySucceed)
    {
        L4D2QueryAgentManager.Instance.StartQuery(list);//Create Query Agents
        serverUIContainer.GenerateUIGroup(L4D2QueryAgentManager.Instance.agents);//Generate UI Group
    }

    void OnReceiveVipIDList(List<IDDecorator> list, bool remoteQuerySucceed)
    {
        PlayerIDManager.Instance.BuildVipIDTable(list);
    }
}
