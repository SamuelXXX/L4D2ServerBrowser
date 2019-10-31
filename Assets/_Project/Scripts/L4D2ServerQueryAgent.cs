using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ValveServerQuery;
using PowerInspector;
using System;

public class L4D2ServerQueryAgent : MonoBehaviour
{
    #region Settings
    public string ip;
    public int port;
    public bool autoStart;
    [SerializeField, RuntimeData]
    protected bool connected = false;

    public PowerButton button = new PowerButton("查询服务器", "PerformServerQuery", 30f);

    public bool Connected
    {
        get
        {
            return connected;
        }
    }
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
    public void StartSession(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        if (client != null)
            client.Close();

        client = new ValveServerQueryClient(ip, port);
        client.MessageHandler += OnReceiveMessage;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (autoStart)
        {
            StartSession(ip, port);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (client == null)
            return;

        connected = client.IsConnected;

        if (connected)
        {
            client.AsyncProcessingMessage();
        }
        else
        {
            client.ManualConnect();
        }
    }
    #endregion

    public void PerformServerQuery()
    {
        if (connected)
        {
            client.SendQueryMessage((byte)ValveServerRequestType.A2S_Info, -1);
        }
        else
        {
            Debug.LogWarning("Not Connected Yet!!!");
        }
    }

    private void OnReceiveMessage(ByteBuffer buff)
    {
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
