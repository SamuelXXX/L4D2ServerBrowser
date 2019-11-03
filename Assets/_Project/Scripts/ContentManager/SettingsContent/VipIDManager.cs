using PowerInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class IDDecorator
{
    const string defaultDecorateExp = "<color=red>[USERNAME]</color>";

    public string id;
    public string decorateExp;

    public IDDecorator(string id)
    {
        this.id = id;
        decorateExp = defaultDecorateExp;
    }

    public IDDecorator(string id, string decorateExp)
    {
        this.id = id;
        this.decorateExp = decorateExp;
    }

    public string DecoratedID
    {
        get
        {
            return decorateExp.Replace("[USERNAME]", id);
        }
    }
}

public class VipIDManager : ShortLifeSingleton<VipIDManager>
{
    #region Settings
    public string vipLibraryUrl;
    public string localSpecialCareConfigFile = "SpecialCareLib.txt";
    #endregion

    #region Unity Life Cycle
    protected override void OnDestroy()
    {
        base.OnDestroy();
        ExitLocalEditingMode();
    }
    #endregion

    #region Exposed API
    [SerializeField, PowerList(Key = "id")]
    protected List<IDDecorator> idDataCache = new List<IDDecorator>();
    protected Action<List<IDDecorator>, bool> contentReadyHandler;
    bool inCommitingProcess = false;

    protected List<IDDecorator> cloudVipCache = new List<IDDecorator>();
    public void CommitVipIDRequest(Action<List<IDDecorator>, bool> onContentReadyHandler)
    {
        if (inCommitingProcess)
        {
            if (onContentReadyHandler != null)
                contentReadyHandler += onContentReadyHandler;
            return;
        }

        inCommitingProcess = true;
        idDataCache.Clear();
        contentReadyHandler = onContentReadyHandler;
        if (cloudVipCache.Count == 0)
            WebRequestAgent.Instance.Get(vipLibraryUrl, OnReceiveVipIDLibraryResponse);
        else
        {
            idDataCache.AddRange(cloudVipCache);

            idDataCache.AddRange(StringToIDList(ReadLocalConfig()));
            contentReadyHandler?.Invoke(idDataCache, true);
            contentReadyHandler = null;
            inCommitingProcess = false;
        }
    }
    #endregion

    #region Tool Methods
    List<IDDecorator> StringToIDList(string s)
    {
        List<IDDecorator> idDecorators = new List<IDDecorator>();

        string[] records = s.Split('\n');
        foreach (var c in records)
        {
            string[] ps = c.Split(':');
            if (ps.Length != 2)
                continue;

            string id = ps[0];
            string decorateExp = ps[1];

            id.Replace("<*>", ":");
            decorateExp.Replace("<*>", ":");

            IDDecorator idDec = new IDDecorator(id.Trim(), decorateExp.Trim());

            idDecorators.Add(idDec);
        }
        return idDecorators;
    }

    string IDListToString(List<IDDecorator> idList)
    {
        if (idList == null)
            return "";

        string retString = "";
        foreach (var s in idList)
        {
            if (s == null)
                continue;
            retString += s.id.Replace(":", "<*>").Trim() + ":" + s.decorateExp.Replace(":", "<*>").Trim() + "\n";
        }

        return retString;
    }
    #endregion

    #region Local Configure Editing
    string ReadLocalConfig()
    {
        string filePath = Application.persistentDataPath + "/" + localSpecialCareConfigFile;
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
            return s;
        }
    }

    void WriteLocalConfig(string s)
    {
        string filePath = Application.persistentDataPath + "/" + localSpecialCareConfigFile;
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
    [HideInInspector]
    public List<IDDecorator> localSpecialCareEditingCache;

    public void EnterLocalEditingMode()
    {
        if (inLocalEditingMode)
            return;

        inLocalEditingMode = true;
        localSpecialCareEditingCache = StringToIDList(ReadLocalConfig());
    }

    bool dataDirty = false;
    public IDDecorator AppendSpecialCareID(string ip)
    {
        if (!inLocalEditingMode)
            return null;

        IDDecorator idd = new IDDecorator(ip);
        localSpecialCareEditingCache.Add(idd);
        dataDirty = true;
        return idd;
    }

    public void DeleteSpecialCareID(IDDecorator s)
    {
        if (!inLocalEditingMode)
            return;
        localSpecialCareEditingCache.Remove(s);
        dataDirty = true;
    }

    public bool ExitLocalEditingMode()
    {
        if (!inLocalEditingMode)
            return false;

        bool ret = dataDirty;
        dataDirty = false;
        inLocalEditingMode = false;
        WriteLocalConfig(IDListToString(localSpecialCareEditingCache));
        return ret;
    }
    #endregion

    #region Web Session
    void OnReceiveVipIDLibraryResponse(WebRequestAgent.WebResponseData data)
    {
        bool serverQuerySucceed = false;
        if (data.responseType == WebRequestAgent.ResponseDataType.Text)
        {
            serverQuerySucceed = true;
            idDataCache.AddRange(StringToIDList(data.text));
            cloudVipCache = StringToIDList(data.text);
        }

        idDataCache.AddRange(StringToIDList(ReadLocalConfig()));
        contentReadyHandler?.Invoke(idDataCache, serverQuerySucceed);
        contentReadyHandler = null;
        inCommitingProcess = false;
    }
    #endregion
}
