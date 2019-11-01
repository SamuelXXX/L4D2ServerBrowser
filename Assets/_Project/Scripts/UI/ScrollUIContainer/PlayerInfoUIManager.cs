using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoUIManager : MonoBehaviour
{
    public PlayerInfoItem uiPrefab;
    protected List<PlayerInfoItem> playerInfoItems = new List<PlayerInfoItem>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        List<ValveServerQuery.ValveServerResponseData.PlayerInfo> playerInfos = new List<ValveServerQuery.ValveServerResponseData.PlayerInfo>(queryAgent.playersInfo.playerInfos);
        playerInfos.Sort((a, b) => { return b.score - a.score; });

        for (int i = 0; i < playerInfoItems.Count; i++)
        {
            if (i < playerInfos.Count)
            {
                playerInfoItems[i].UpdateContent(playerInfos[i]);
            }
            else
            {
                playerInfoItems[i].UpdateContent(null);
            }
        }
    }

    protected L4D2ServerQueryAgent queryAgent;
    public void GeneratePlayerUIGroup(L4D2ServerQueryAgent agent)
    {
        queryAgent = agent;

        DestroyPlayerUIGroup();
        for (int i = 0; i < agent.serverInfo.maxPlayers; i++)
        {
            var go = Instantiate(uiPrefab.gameObject);

            var ui = go.GetComponent<PlayerInfoItem>();
            ui.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), true);
            ui.transform.localScale = Vector3.one;
            ui.BindIndex(i);
            playerInfoItems.Add(ui);
        }
    }

    public void DestroyPlayerUIGroup()
    {
        foreach (var p in playerInfoItems)
        {
            Destroy(p.gameObject);
        }

        playerInfoItems.Clear();
    }
}
