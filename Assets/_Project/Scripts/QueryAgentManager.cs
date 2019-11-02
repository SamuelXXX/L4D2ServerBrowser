using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerConnectInfo
{
    public string ip;
    public ushort port;
}

public class QueryAgentManager : ShortLifeSingleton<QueryAgentManager>
{
    [PowerInspector.PowerList(Key = "ip")]
    public List<ServerConnectInfo> serverConnectInfos = new List<ServerConnectInfo>();
    public L4D2ServerQueryAgent agentPrefab;
    public float timeGap = 1f;


    [System.NonSerialized]
    public List<L4D2ServerQueryAgent> agents = new List<L4D2ServerQueryAgent>();



    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {

    }

    bool offline = false;
    // Update is called once per frame
    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            offline = true;
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

    

    public void StartQuery(List<ServerConnectInfo> serverInfo)
    {
        serverConnectInfos = serverInfo;
        CreateAgentGroup(serverInfo);
        StartCoroutine(QueryRoutine());
    }

    public void StopQuery()
    {
        StopCoroutine("QueryRoutine");
        DestroyAgentGroup();
        serverConnectInfos = null;
    }

    IEnumerator QueryRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeGap);
            foreach (var a in agents)
            {
                a.PerformServerQuery();
            }
        }
    }

    #region Agents Management
    void CreateAgentGroup(List<ServerConnectInfo> serverConnectInfos)
    {
        foreach (var a in agents)
        {
            Destroy(a.gameObject);
        }
        agents.Clear();

        foreach (var s in serverConnectInfos)
        {
            var go = Instantiate(agentPrefab.gameObject);
            var agent = go.GetComponent<L4D2ServerQueryAgent>();
            agents.Add(agent);
            go.transform.parent = transform;
            go.name = "Agent-->" + s.ip + ":" + s.port.ToString();

            agent.StartQuerySession(s.ip, s.port);
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
            a.StartQuerySession(a.ip, a.port);
        }
    }

    void DisconnectAllAgents()
    {
        foreach (var a in agents)
        {
            a.StopQuerySession();
        }
    }
    #endregion
}
