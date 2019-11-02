using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ValveServerQuery;
using PowerInspector;
using System;

public enum L4D2ServerAgentStatus
{
    NotInitialized = 0,
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
    public L4D2ServerAgentStatus status = L4D2ServerAgentStatus.NotInitialized;
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
            StartQuerySession(ip, port);
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
        client.Close();
    }
    #endregion

    #region Exposed API
    public void StartQuerySession(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        if (client != null)
            client.Close();

        status = L4D2ServerAgentStatus.OK;
        lastRespondTime = Time.time;
        client = new ValveServerQueryClient(ip, port);
        client.MessageHandler += OnReceiveMessage;
    }

    public void StopQuerySession()
    {
        if (client != null)
            client.Close();

        status = L4D2ServerAgentStatus.NotInitialized;
        client = null;
    }

    public void PerformServerQuery()
    {
        if (client != null)
            client.SendQueryMessage((byte)ValveServerRequestType.A2S_Info, -1);
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
                client.SendQueryMessage((byte)ValveServerRequestType.A2S_Player, -1);
                break;
            case ValveServerResponseType.A2S_ChallengeNumber:
                challengeNumberInfo = data.challengeNumberBody;
                client.SendQueryMessage((byte)ValveServerRequestType.A2S_Player, challengeNumberInfo.challengeNumber);
                break;
            case ValveServerResponseType.A2S_Player:
                playersInfo = data.playersInfoBody;
                break;
            default: break;
        }

    }
}
