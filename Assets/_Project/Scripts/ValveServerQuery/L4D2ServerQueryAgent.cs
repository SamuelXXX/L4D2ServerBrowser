using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ValveServerQuery;
using PowerInspector;
using System;

public enum L4D2ServerAgentStatus
{
    Offline = 0,
    WaitForChallengeNumber,
    OK,
    NotResponding
}

public class L4D2ServerQueryAgent : MonoBehaviour
{
    #region Settings
    public string ip;
    public int port;
    public bool autoStart;

    [RuntimeData]
    public L4D2ServerAgentStatus status = L4D2ServerAgentStatus.Offline;
    public float notRespondingThreashold = 3f;
    protected float lastRespondTime;

    public PowerButton button = new PowerButton("查询服务器", "PerformServerQuery", 30f);
    #endregion

    #region Runtime Data
    protected ValveServerQueryClient client;

    [SerializeField, RuntimeData]
    public ValveServerResponseData.A2S_Info serverInfo;
    [SerializeField, RuntimeData]
    public ValveServerResponseData.A2S_ChallengeNumber challengeNumberInfo;
    [SerializeField]
    public ValveServerResponseData.A2S_Player playersInfo;
    #endregion

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        if (autoStart)
        {
            Connect(ip, port);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (client == null)
            return;

        client.AsyncProcessingMessage();
        if (status == L4D2ServerAgentStatus.OK && lastRespondTime + notRespondingThreashold < Time.time)
        {
            status = L4D2ServerAgentStatus.NotResponding;
        }
    }

    private void OnDestroy()
    {
        if (client != null)
            client.Close();
    }
    #endregion

    #region Exposed API
    /// <summary>
    /// In fact there is no connection in UDP 
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void Connect(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        Connect();
    }

    /// <summary>
    /// Create new socket
    /// </summary>
    public void Connect()
    {
        if (client != null)
        {
            client.Close();
            StopAllCoroutines();
        }

        status = L4D2ServerAgentStatus.WaitForChallengeNumber;
        lastRespondTime = Time.time;
        client = new ValveServerQueryClient(ip, port);
        client.MessageHandler += OnReceiveMessage;
        StartCoroutine(ChallengeNumberAcquireRoutine());
    }

    /// <summary>
    /// Dispose socket
    /// </summary>
    public void Disconnect()
    {
        if (client != null)
            client.Close();

        status = L4D2ServerAgentStatus.Offline;
        client = null;
        StopAllCoroutines();
    }

    IEnumerator ChallengeNumberAcquireRoutine()
    {
        while (status == L4D2ServerAgentStatus.WaitForChallengeNumber && client != null)
        {
            client.SendQueryMessage((byte)ValveServerRequestType.A2S_Player, -1);//Require for Challenge Number
            yield return new WaitForSeconds(1f);
        }
    }

    public void PerformServerQuery()
    {
        StartCoroutine(PerformQueryRoutine());
    }

    IEnumerator PerformQueryRoutine()
    {
        if (client != null && status != L4D2ServerAgentStatus.WaitForChallengeNumber)
        {
            client.SendQueryMessage((byte)ValveServerRequestType.A2S_Info, challengeNumberInfo.challengeNumber);
            yield return new WaitForSeconds(0.1f);//Wait for 0.1 seconds to perform players query
            client.SendQueryMessage((byte)ValveServerRequestType.A2S_Player, challengeNumberInfo.challengeNumber);
        }
    }
    #endregion

    private void OnReceiveMessage(ByteBuffer buff)
    {
        status = L4D2ServerAgentStatus.OK;
        lastRespondTime = Time.time;

        ValveServerResponseData data = new ValveServerResponseData(buff);

        switch ((ValveServerResponseType)data.header)
        {
            case ValveServerResponseType.A2S_Info:
                serverInfo = data.serverInfoBody;
                break;
            case ValveServerResponseType.A2S_ChallengeNumber:
                challengeNumberInfo = data.challengeNumberBody;
                break;
            case ValveServerResponseType.A2S_Player:
                playersInfo = data.playersInfoBody;
                break;
            default: break;
        }

    }
}
