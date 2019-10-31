using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerInfoLibraryManager : ShortLifeSingleton<ServerInfoLibraryManager>
{
    #region Settings
    public string ipLibraryUrl;

    [Header("UI Settings")]
    public Text infoText;
    public Button refreshButton;
    #endregion

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        WebRequestAgent.Instance.Get(ipLibraryUrl, OnReceiveIPLibraryResponse);
        refreshButton.onClick.AddListener(OnBtnRefreshClicked);
        infoText.text = "正在获取服务器列表...";
        refreshButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    void OnBtnRefreshClicked()
    {
        WebRequestAgent.Instance.Get(ipLibraryUrl, OnReceiveIPLibraryResponse);
        infoText.text = "正在获取服务器列表...";
        refreshButton.gameObject.SetActive(false);
    }

    #region Web Session
    void OnReceiveIPLibraryResponse(WebRequestAgent.WebResponseData data)
    {
        if (data.responseType != WebRequestAgent.ResponseDataType.Text)
        {
            refreshButton.gameObject.SetActive(true);
            infoText.text = "获取失败！！！";
            return;
        }

        refreshButton.gameObject.SetActive(false);
        infoText.text = "";

        List<ServerConnectInfo> serverInfos = new List<ServerConnectInfo>();
        
        string r = data.text;
        string[] records = r.Split('\n');
        foreach (var c in records)
        {
            string[] ps = c.Split(':');
            if (ps.Length != 2)
                continue;

            string ip = ps[0];
            string port = ps[1];
            ServerConnectInfo connectInfo = new ServerConnectInfo();
            connectInfo.ip = ip;
            connectInfo.port = int.Parse(port);

            serverInfos.Add(connectInfo);
        }

        QueryAgentManager.Instance.StartQuery(serverInfos);
    }
    #endregion
}
