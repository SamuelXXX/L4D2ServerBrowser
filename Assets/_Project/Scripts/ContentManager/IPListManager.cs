using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/// <summary>
/// Managing the server ip:port library for querying
/// Two types of ip source, remote configure file and local config file
/// </summary>
public class IPListManager : ShortLifeSingleton<IPListManager>
{
    #region Settings
    public string ipLibraryUrl;
    public string localIPConfigFile = "IPLib.txt";

    protected Regex ipRegex = new Regex(@"^(\d{1,3}.){3}\d{1,3}$");
    #endregion

    #region Unity Life Cycle
    protected override void OnDestroy()
    {
        base.OnDestroy();
        ExitLocalEditingMode();
    }

    #endregion

    #region Exposed API
    protected List<IPData> serverConnectInfoCache = new List<IPData>();
    protected Action<List<IPData>, bool> contentReadyHandler;
    bool inCommitingProcess = false;
    public void CommitServerInfoRequest(Action<List<IPData>, bool> onContentReadyHandler)
    {
        if (inCommitingProcess)
        {
            if (onContentReadyHandler != null)
                contentReadyHandler += onContentReadyHandler;
            return;
        }

        inCommitingProcess = true;
        serverConnectInfoCache.Clear();
        contentReadyHandler = onContentReadyHandler;
        WebRequestAgent.Instance.Get(ipLibraryUrl, OnReceiveIPLibraryResponse);
    }
    #endregion

    #region Local Configure File Operation
    string ReadLocalConfig()
    {
        string filePath = Application.persistentDataPath + "/" + localIPConfigFile;
        if (!File.Exists(filePath))
        {
            File.CreateText(filePath);
            return "";
        }
        else
        {
            StreamReader reader = new StreamReader(filePath);
            string s = reader.ReadToEnd();
            reader.Close();
            Debug.Log("Read Local IPLib:" + s);
            return s;
        }
    }

    void WriteLocalConfig(string s)
    {
        string filePath = Application.persistentDataPath + "/" + localIPConfigFile;
        StreamWriter writer;
        if (!File.Exists(filePath))
        {
            writer = File.CreateText(filePath);
        }
        else
        {
            writer = new StreamWriter(filePath);
        }

        writer.Write(s);
        writer.Close();
    }

    bool inLocalEditingMode = false;
    public List<IPData> localServerInfoEditingCache;

    public void EnterLocalEditingMode()
    {
        if (inLocalEditingMode)
            return;

        inLocalEditingMode = true;
        localServerInfoEditingCache = StringToServerConnectInfos(ReadLocalConfig());
    }

    bool dataDirty = false;
    public IPData AppendIPRecord(string ip, ushort port)
    {
        if (!inLocalEditingMode)
            return null;

        if (!ipRegex.IsMatch(ip))
        {
            return null;
        }

        IPData sci = new IPData();
        sci.ip = ip;
        sci.port = port;
        localServerInfoEditingCache.Add(sci);
        dataDirty = true;
        return sci;
    }

    public void DeleteIPRecord(IPData s)
    {
        if (!inLocalEditingMode)
            return;
        localServerInfoEditingCache.Remove(s);
        dataDirty = true;
    }

    public bool ExitLocalEditingMode()
    {
        if (!inLocalEditingMode)
            return false;

        bool ret = dataDirty;
        dataDirty = false;
        inLocalEditingMode = false;
        WriteLocalConfig(ServerConnectInfosToString(localServerInfoEditingCache));
        return ret;
    }
    #endregion

    #region Tool Methods
    List<IPData> StringToServerConnectInfos(string s)
    {
        List<IPData> serverInfos = new List<IPData>();

        string[] records = s.Split('\n');
        foreach (var c in records)
        {
            string[] ps = c.Split(':');
            if (ps.Length != 2)
                continue;

            string ip = ps[0];
            string port = ps[1];

            if (!ipRegex.IsMatch(ip.Trim()))
            {
                continue;
            }

            IPData connectInfo = new IPData();
            connectInfo.ip = ip.Trim();
            connectInfo.port = ushort.Parse(port);


            serverInfos.Add(connectInfo);
        }
        return serverInfos;
    }

    string ServerConnectInfosToString(List<IPData> serverConnectInfos)
    {
        if (serverConnectInfos == null)
            return "";

        string retString = "";
        foreach (var s in serverConnectInfos)
        {
            if (s == null)
                continue;

            if (ipRegex.IsMatch(s.ip.Trim()))
            {
                retString += s.ip.Trim() + ":" + s.port.ToString() + "\n";
            }
        }

        return retString;
    }
    #endregion

    #region Web Session
    void OnReceiveIPLibraryResponse(WebRequestAgent.WebResponseData data)
    {
        bool serverQuerySucceed = false;
        if (data.responseType == WebRequestAgent.ResponseDataType.Text)
        {
            serverQuerySucceed = true;
            serverConnectInfoCache.AddRange(StringToServerConnectInfos(data.text));
        }

        serverConnectInfoCache.AddRange(StringToServerConnectInfos(ReadLocalConfig()));
        contentReadyHandler?.Invoke(serverConnectInfoCache, serverQuerySucceed);
        contentReadyHandler = null;
        inCommitingProcess = false;
    }
    #endregion
}
