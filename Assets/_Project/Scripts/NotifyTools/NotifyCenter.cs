using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;
using System;
using System.IO;

public class NotifyCenter : ShortLifeSingleton<NotifyCenter>
{
    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        IPListManager.Instance.CommitServerInfoRequest(OnReceiveServerInfoList);
        CommitNotifySubscribersRequest();
        PlayerIDManager.Instance.OnPlayerLoginAction += OnNewPlayerLogin;
    }
    #endregion

    #region Remote Notify Settings
    bool inCommitingProcess = false;

    public NotifySubcriberSettings notifySubscriberSettings = null;
    void CommitNotifySubscribersRequest()
    {
        if (inCommitingProcess)
        {
            return;
        }

        WebRequestAgent.Instance.Get(RCUrlManager.Instance.settings.urlNotifySubscribers, OnReceiveNotifySubscribersResponse);
    }

    void OnReceiveNotifySubscribersResponse(WebRequestAgent.WebResponseData data)
    {
        try
        {
            if (data.responseType == WebRequestAgent.ResponseDataType.Text)
            {
                notifySubscriberSettings = JsonUtility.FromJson<NotifySubcriberSettings>(data.text);
            }
            else
            {
                notifySubscriberSettings = null;
                Debug.LogError("Response Type Not Matched!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        inCommitingProcess = false;
    }

    void OnNewPlayerLogin(PlayerIDRunningData data)
    {
        Debug.Log(data.id + " login!");
        if (notifySubscriberSettings == null)
            return;
        foreach (var s in notifySubscriberSettings.subscribers)
        {
            foreach (var a in s.accountOfInterest)
            {
                if (a == data.id)
                {
                    SendEMailToSubScriber(data, s);
                }
            }
        }
    }
    #endregion

    #region Server Info List
    void OnReceiveServerInfoList(List<IPData> list, bool remoteQuerySucceed)
    {
        L4D2QueryAgentManager.Instance.StartQuery(list);//Create Query Agents
    }
    #endregion

    #region Email Operation
    void SendEMailToSubScriber(PlayerIDRunningData data, NotifySubscriberBody subscriber)
    {
        string subject = subscriber.notifySubjectTemplate.Replace("[INTEREST]", data.id);
        string body = subscriber.notifyBodyTemplate.Replace("[NAME]", subscriber.subscriberName);
        body = body.Replace("[INTEREST]", data.id);
        body = body.Replace("[SERVER]", data.serverName.Replace("\0", ""));
        SendQQMail(subscriber.subscriberMail, subject, body);
    }

    void SendQQMail(string toMail, string subject, string body)
    {
        if (notifySubscriberSettings == null)
            return;

        MailMessage message = new MailMessage();

        message.From = new MailAddress(notifySubscriberSettings.fromMail);
        message.To.Add(toMail);

        message.SubjectEncoding = System.Text.Encoding.UTF8;
        message.Subject = subject;

        message.BodyEncoding = System.Text.Encoding.UTF8;
        message.Body = body;
        message.IsBodyHtml = true;

        SmtpClient client = new SmtpClient();
        client.EnableSsl = true;
        client.Host = notifySubscriberSettings.emailHost;
        client.Port = notifySubscriberSettings.emailPort;
        client.Credentials = new System.Net.NetworkCredential(notifySubscriberSettings.fromMail, notifySubscriberSettings.fromMailPassword);

        try
        {
            client.Send(message);
        }
        catch (Exception ex)
        {
            string mssage = ex.ToString();
            Debug.LogError(message);
        }
    }
    #endregion
}
