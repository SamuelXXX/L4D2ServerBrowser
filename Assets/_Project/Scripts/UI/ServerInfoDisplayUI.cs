using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerInfoDisplayUI : MonoBehaviour
{
    #region UI Settings
    [Header("UI Component Reference")]
    public Image posterImage;
    public Text serverNameText;
    public Text mapCNNameText;
    public Text mapIndexText;
    public Text playerCountText;
    public Text playersNameText;
    public Button moreInfoButton;

    public GameObject infoLayer;
    public Text infoText;
    #endregion

    protected L4D2ServerQueryAgent bindAgent;

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        var rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
    }

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
            if (!bindAgent.Connected || string.IsNullOrEmpty(bindAgent.serverInfo.serverName))
            {
                infoLayer.SetActive(true);
                infoText.text = "正在连接...\n" + bindAgent.ip + ":" + bindAgent.port.ToString();
            }
            else
            {
                infoLayer.SetActive(false);

                serverNameText.text = bindAgent.serverInfo.serverName;
                MapContentMapper.MapInfoItem mapInfo = MapContentMapper.Instance.QueryByMapIndex(bindAgent.serverInfo.serverMap);
                mapCNNameText.text = mapInfo.mapCNName;
                posterImage.sprite = mapInfo.mapPosterImage;
                mapIndexText.text = "地图：" + bindAgent.serverInfo.serverMap;
                playerCountText.text = "玩家：" + bindAgent.serverInfo.players.ToString() + "/" + bindAgent.serverInfo.maxPlayers.ToString();


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
                        players += name;
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

    public void BindAgent(L4D2ServerQueryAgent agent)
    {
        this.bindAgent = agent;
    }
}
