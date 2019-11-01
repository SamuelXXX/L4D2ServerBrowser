using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIMain : PageUIBase
{
    public ServerInfoDisplayUIManager serverUIContainer;
    public Button settingButton;
    public string ipSettingsPage;
    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        if (pars.Length == 1)
        {
            List<ServerConnectInfo> serverList = pars[0] as List<ServerConnectInfo>;
            QueryAgentManager.Instance.StartQuery(serverList);//Create Query Agents
            serverUIContainer.GenerateUIGroup(QueryAgentManager.Instance.agents);//Generate UI Group
        }
        settingButton.onClick.AddListener(OnSettingsPressed);
    }

    public override void OnResume()
    {
        base.OnResume();
        if (PageUIIPSettings.ipDirty)
        {
            PageUIIPSettings.ipDirty = false;

            QueryAgentManager.Instance.StopQuery();
            serverUIContainer.DestroyUIGroup();
            ServerInfoLibraryManager.Instance.CommitServerInfoRequest(OnReceiveServerInfoList);
        }
    }

    protected override void OnDestroyPage()
    {
        base.OnDestroyPage();
        settingButton.onClick.RemoveListener(OnSettingsPressed);
    }

    void OnSettingsPressed()
    {
        PageUIManager.Instance.LoadPageByPageName(ipSettingsPage);
    }

    void OnReceiveServerInfoList(List<ServerConnectInfo> list, bool remoteQuerySucceed)
    {
        QueryAgentManager.Instance.StartQuery(list);//Create Query Agents
        serverUIContainer.GenerateUIGroup(QueryAgentManager.Instance.agents);//Generate UI Group
    }
}
