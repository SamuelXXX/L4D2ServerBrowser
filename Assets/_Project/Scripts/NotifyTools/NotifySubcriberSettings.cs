using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotifySubscriberBody
{
    public const string defaultBodyTemplate = @"[NAME]，你所关注的“[INTEREST]”已于“[SERVER]”上线，快去陪TA玩耍吧！！！";
    public const string defaultSubjectTemplate = @"“[INTEREST]”上线通知";

    public string subscriberName;
    public string subscriberMail;
    public List<string> accountOfInterest = new List<string>();
    public string notifySubjectTemplate = defaultSubjectTemplate;
    public string notifyBodyTemplate = defaultBodyTemplate;
}

[System.Serializable]
public class NotifySubcriberSettings
{
    public string fromMail;
    public string fromMailPassword;
    public string emailHost;
    public int emailPort;

    public List<NotifySubscriberBody> subscribers = new List<NotifySubscriberBody>();
}
