using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerConnectInfo
{
    public string ip;
    public int port;
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

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    public void StartQuery(List<ServerConnectInfo> serverInfo)
    {
        serverConnectInfos = serverInfo;
        CreateAgentGroup(serverInfo);
        StartCoroutine(QueryRoutine());
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
    public void CreateAgentGroup(List<ServerConnectInfo> serverConnectInfos)
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

            agent.StartSession(s.ip, s.port);
        }

        ServerInfoDisplayUIManager.Instance.GenerateUIGroup();
    }

    public void DestroyAgentGroup()
    {
        foreach (var a in agents)
        {
            Destroy(a.gameObject);
        }
        agents.Clear();
    }
    #endregion
}
