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
            List<IPData> serverList = pars[0] as List<IPData>;
            L4D2QueryAgentManager.Instance.StartQuery(serverList);//Create Query Agents
            serverUIContainer.GenerateUIGroup(L4D2QueryAgentManager.Instance.agents);//Generate UI Group
        }
        settingButton.onClick.AddListener(OnSettingsPressed);
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

    void OnReceiveServerInfoList(List<IPData> list, bool remoteQuerySucceed)
    {
        L4D2QueryAgentManager.Instance.StartQuery(list);//Create Query Agents
        serverUIContainer.GenerateUIGroup(L4D2QueryAgentManager.Instance.agents);//Generate UI Group
    }
}
