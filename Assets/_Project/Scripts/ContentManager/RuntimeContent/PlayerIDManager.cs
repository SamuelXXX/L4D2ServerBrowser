using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerIDRunningData
{
    public string id;
    public string serverName;
    public float gamingTime;

    public PlayerIDRunningData(string id, string serverName, float gamingTime)
    {
        this.id = id;
        this.serverName = serverName;
        this.gamingTime = gamingTime;
    }
}


public class PlayerIDManager : ShortLifeSingleton<PlayerIDManager>
{
    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        CollectPlayersRuntimeData();
    }

    public List<PlayerIDRunningData> collectedPlayerData = new List<PlayerIDRunningData>();

    void CollectPlayersRuntimeData()
    {
        HashSet<string> nameHash = new HashSet<string>();
        foreach (var c in collectedPlayerData)
        {
            if (!string.IsNullOrEmpty(c.id))
                nameHash.Add(c.id);
        }

        collectedPlayerData.Clear();
        foreach (var a in L4D2QueryAgentManager.Instance.agents)
        {
            var serverName = a.serverInfo.serverName;
            foreach (var p in a.playersInfo.playerInfos)
            {
                var d = new PlayerIDRunningData(p.name.Replace("\0", ""), serverName, p.duration);
                collectedPlayerData.Add(d);
                if (!nameHash.Contains(d.id) && !string.IsNullOrEmpty(d.id) && d.gamingTime < 90f)//Need to check play time to determine login action
                {
                    OnPlayerLoginAction?.Invoke(d);
                }
            }
        }
    }

    public Action<PlayerIDRunningData> OnPlayerLoginAction;
    #endregion

    #region Exposed API
    Dictionary<string, IDDecorator> decoratorDict = new Dictionary<string, IDDecorator>();
    public void BuildVipIDTable(List<IDDecorator> list)
    {
        decoratorDict.Clear();
        foreach (var c in list)
        {
            if (!decoratorDict.ContainsKey(c.id))
                decoratorDict.Add(c.id, c);
        }
    }

    public string DecoratePlayerID(string id)
    {
        if (string.IsNullOrEmpty(id))
            return "";

        if (decoratorDict.ContainsKey(id))
        {
            return decoratorDict[id].DecoratedID;
        }
        else
        {
            return id;
        }
    }
    #endregion
}
