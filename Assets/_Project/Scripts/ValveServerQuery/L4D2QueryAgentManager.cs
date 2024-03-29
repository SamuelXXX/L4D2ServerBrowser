﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IPData
{
    public string ip;
    public ushort port;
}

public class L4D2QueryAgentManager : ShortLifeSingleton<L4D2QueryAgentManager>
{
    #region Settings
    [PowerInspector.PowerList(Key = "ip")]
    public List<IPData> serverIPInfos = new List<IPData>();
    public L4D2ServerQueryAgent agentPrefab;
    public float queryTimeGap = 1f;

    [System.NonSerialized]
    public List<L4D2ServerQueryAgent> agents = new List<L4D2ServerQueryAgent>();
    #endregion

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    bool offline = false;
    // Update is called once per frame
    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            offline = true;
            DisconnectAllAgents();
        }
        else if (offline)//Reconnect
        {
            offline = false;
            ReconnectAllAgents();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            ReconnectAllAgents();
        }
        else
        {
            DisconnectAllAgents();
        }
    }
    #endregion

    #region Query API
    bool isQuerying = false;
    public void StartQuery(List<IPData> serverInfo)
    {
        if (isQuerying)
        {
            StopQuery();
        }

        isQuerying = true;
        serverIPInfos = serverInfo;
        CreateAgentGroup(serverInfo);
        StartCoroutine(QueryRoutine());
    }

    public void PauseQuery()
    {
        if (isQuerying)
            StopAllCoroutines();
    }

    public void ReusmeQuery()
    {
        if (isQuerying)
            StartCoroutine(QueryRoutine());
    }

    public void StopQuery()
    {
        StopAllCoroutines();
        DestroyAgentGroup();
        serverIPInfos = null;
        isQuerying = false;
    }

    IEnumerator QueryRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(queryTimeGap);
            foreach (var a in agents)
            {
                a.PerformServerQuery();
            }
        }
    }
    #endregion

    #region Agents Management
    void CreateAgentGroup(List<IPData> ipDatas)
    {
        foreach (var a in agents)
        {
            Destroy(a.gameObject);
        }
        agents.Clear();

        foreach (var s in ipDatas)
        {
            var go = Instantiate(agentPrefab.gameObject);
            var agent = go.GetComponent<L4D2ServerQueryAgent>();
            agents.Add(agent);
            go.transform.parent = transform;
            go.name = "Agent-->" + s.ip + ":" + s.port.ToString();

            agent.Connect(s.ip, s.port);
        }
    }

    void DestroyAgentGroup()
    {
        foreach (var a in agents)
        {
            DestroyImmediate(a.gameObject);
        }
        agents.Clear();
    }

    void ReconnectAllAgents()
    {
        foreach (var a in agents)
        {
            a.Connect();
        }
    }

    void DisconnectAllAgents()
    {
        foreach (var a in agents)
        {
            a.Disconnect();
        }
    }
    #endregion
}
