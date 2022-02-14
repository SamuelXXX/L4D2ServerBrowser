using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIDetailInfo : PageUIBase
{
    #region UI Settings
    [Header("UI Reference")]
    public Sprite defaultPosterImage;
    public Image posterImage;
    public Text serverNameText;
    public Text mapCNNameText;
    public Text mapIndexText;
    public Text playerCountText;
    public Text ipAddressText;
    public Text statusText;

    public GameObject infoLayer;
    public Text infoText;
    public Button exitButton;

    public PlayerInfoUIManager infoListUI;
    #endregion

    #region Runtime Data
    L4D2ServerQueryAgent bindAgent = null;
    #endregion

    #region Page UI Life Cycle
    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        if (pars.Length != 0)
        {
            bindAgent = pars[0] as L4D2ServerQueryAgent;
            infoListUI.GeneratePlayerUIGroup(bindAgent);
        }
        exitButton.onClick.AddListener(OnExitPressed);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (bindAgent == null)
        {
            infoLayer.SetActive(true);
            infoText.text = "未连接";
        }
        else
        {
            if (string.IsNullOrEmpty(bindAgent.serverInfo.serverName))
            {
                infoLayer.SetActive(true);
                infoText.text = "正在等待服务器响应...\n" + bindAgent.ip + ":" + bindAgent.port.ToString();
            }
            else
            {
                infoLayer.SetActive(false);

                switch (bindAgent.status)
                {
                    case L4D2ServerAgentStatus.Offline:
                        statusText.text = "<color=red>客户端离线</color>";
                        break;
                    case L4D2ServerAgentStatus.WaitForChallengeNumber:
                        statusText.text = "<color=red>检查服务器可用性</color>";
                        break;
                    case L4D2ServerAgentStatus.OK:
                        statusText.text = "";
                        break;
                    case L4D2ServerAgentStatus.NotResponding:
                        statusText.text = "<color=red>未响应</color>";
                        break;
                    default: break;
                }

                serverNameText.text = bindAgent.serverInfo.serverName;

                MapContentMapper.MapInfoItem mapInfo = null;
                if (MapContentMapper.ThirdPartyInstance != null)
                    mapInfo = MapContentMapper.ThirdPartyInstance.QueryByMapIndex(bindAgent.serverInfo.serverMap);

                if (mapInfo==null || string.IsNullOrEmpty(mapInfo.mapCNName))
                    mapInfo = MapContentMapper.OfficialInstance.QueryByMapIndex(bindAgent.serverInfo.serverMap);

                mapCNNameText.text = mapInfo.mapCNName;
                posterImage.sprite = mapInfo.mapPosterImage;
                mapIndexText.text = "地图：" + bindAgent.serverInfo.serverMap;
                playerCountText.text = "玩家：" + bindAgent.serverInfo.players.ToString() + "/" + bindAgent.serverInfo.maxPlayers.ToString();
                ipAddressText.text = "IP地址：" + bindAgent.ip + ":" + bindAgent.port.ToString();
            }
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    protected override void OnDestroyPage()
    {
        base.OnDestroyPage();
        bindAgent = null;
        infoListUI.DestroyPlayerUIGroup();
        exitButton.onClick.RemoveListener(OnExitPressed);

        serverNameText.text = "";
        mapCNNameText.text = "";
        posterImage.sprite = defaultPosterImage;
        mapIndexText.text = "地图：";
        playerCountText.text = "玩家：";
        ipAddressText.text = "IP地址：";
    }
    #endregion

    void OnExitPressed()
    {
        PageUIManager.Instance.GoBack();
    }
}
