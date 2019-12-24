using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerInfoDisplayItem : MonoBehaviour
{
    #region UI Settings
    [Header("UI Component Reference")]
    public Image posterImage;
    public Text serverNameText;
    public Text mapCNNameText;
    public Text mapIndexText;
    public Text playerCountText;
    public Text playersNameText;
    public Text statusText;
    public Button moreInfoButton;
    public Button coverButton;

    public GameObject infoLayer;
    public Text infoText;

    [Header("Page Settings")]
    public string detailPage;
    #endregion

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        var rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        coverButton.onClick.AddListener(OnEnterDetailInfo);
    }

    string lastCheckMapName;
    MapContentMapper.MapInfoItem mapInfo;
    // Update is called once per frame
    void Update()
    {
        if (bindAgent == null)
        {
            infoLayer.SetActive(true);
            infoText.text = "未连接";
        }
        else
        {

            if (bindAgent.status != L4D2ServerAgentStatus.OK)
            {
                infoLayer.SetActive(true);
                switch (bindAgent.status)
                {
                    case L4D2ServerAgentStatus.Offline:
                        infoText.text = "客户端无法访问网络";
                        break;
                    case L4D2ServerAgentStatus.WaitForChallengeNumber:
                        infoText.text = "正在等待服务器响应...\n" + bindAgent.ip + ":" + bindAgent.port.ToString();
                        break;
                    case L4D2ServerAgentStatus.NotResponding:
                        infoText.text = "服务器未响应";
                        break;
                    default: break;
                }
            }
            else if (string.IsNullOrEmpty(bindAgent.serverInfo.serverName))
            {
                infoLayer.SetActive(true);
                infoText.text = "正在等待服务器响应...\n" + bindAgent.ip + ":" + bindAgent.port.ToString();
            }
            else
            {
                infoLayer.SetActive(false);

                serverNameText.text = bindAgent.serverInfo.serverName;
                if (lastCheckMapName != bindAgent.serverInfo.serverMap)
                {
                    if (MapContentMapper.ThirdPartyInstance != null)
                        mapInfo = MapContentMapper.ThirdPartyInstance.QueryByMapIndex(bindAgent.serverInfo.serverMap);

                    if (mapInfo == null || string.IsNullOrEmpty(mapInfo.mapCNName))
                        mapInfo = MapContentMapper.OfficialInstance.QueryByMapIndex(bindAgent.serverInfo.serverMap);
                    lastCheckMapName = bindAgent.serverInfo.serverMap;
                }

                mapCNNameText.text = mapInfo.mapCNName;
                posterImage.sprite = mapInfo.mapPosterImage;
                mapIndexText.text = "地图：" + bindAgent.serverInfo.serverMap;
                playerCountText.text = "玩家：" + bindAgent.serverInfo.players.ToString() + "/" + bindAgent.serverInfo.maxPlayers.ToString();
                statusText.text = "";


                if (bindAgent.serverInfo.players == 0)
                {
                    playersNameText.text = "当前服务器暂无玩家！";
                }
                else
                {
                    string players = "";
                    foreach (var c in bindAgent.playersInfo.playerInfos)
                    {
                        var name = c.name.Replace("\0", "");//Remove '\0'
                        players += PlayerIDManager.Instance.DecoratePlayerID(name);
                        players += "\n";
                    }
                    if (players.Length >= 1)
                        players = players.Substring(0, players.Length - 1);
                    playersNameText.text = players;
                }
            }
        }
    }
    #endregion

    protected L4D2ServerQueryAgent bindAgent;
    public void BindAgent(L4D2ServerQueryAgent agent)
    {
        this.bindAgent = agent;
    }

    void OnEnterDetailInfo()
    {
        PageUIManager.Instance.LoadPageByPageName(detailPage, bindAgent);
    }
}
