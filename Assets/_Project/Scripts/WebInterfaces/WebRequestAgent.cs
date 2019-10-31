using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestAgent : Singleton<WebRequestAgent>
{
    #region Data Type Defination
    public enum ResponseDataType
    {
        None = 0,//No response data
        Text,
        Binary,
        Error
    }

    public class WebResponseData
    {
        public ResponseDataType responseType;

        public string text;
        public byte[] binary;

        public WebResponseData(ResponseDataType responseType, string text, byte[] binary)
        {
            this.responseType = responseType;
            this.text = text;
            this.binary = binary;
        }
    }
    #endregion

    #region Web API
    public void Post(string uri, WWWForm form, Action<WebResponseData> responseAction)
    {
        StartCoroutine(PostRequestRoutine(uri, form, responseAction));
    }

    public string FormToString(WWWForm form)
    {
        if (form == null)
            return "null";

        string retValue = "";

        retValue = System.Text.Encoding.Default.GetString(form.data);
        return retValue;
    }

    IEnumerator PostRequestRoutine(string uri, WWWForm form, Action<WebResponseData> responseAction)
    {
        UnityWebRequest post = UnityWebRequest.Post(uri, form);
        Debug.Log("PostRequest:<color=green>" + uri + "</color>");

        Debug.Log("PostBody:<color=green>" + FormToString(form) + "</color>");
        yield return post.SendWebRequest();

        if (post.isHttpError)
        {
            Debug.LogError("Encounter a http error when send post request:" + uri);
            if (responseAction != null) responseAction.Invoke(new WebResponseData(ResponseDataType.Error, null, null));
            yield break;
        }

        if (post.isNetworkError)
        {
            Debug.LogError("Encounter a system error when send post request:" + uri);
            if (responseAction != null) responseAction.Invoke(new WebResponseData(ResponseDataType.Error, null, null));
            yield break;
        }

        while (!post.downloadHandler.isDone)
        {
            yield return null;
        }

        if (responseAction != null)
        {
            if (!string.IsNullOrEmpty(post.downloadHandler.text))
            {
                responseAction.Invoke(new WebResponseData(ResponseDataType.Text, post.downloadHandler.text, null));
                yield break;
            }

            else if (post.downloadHandler.data != null)
            {
                responseAction.Invoke(new WebResponseData(ResponseDataType.Binary, null, post.downloadHandler.data));
                yield break;
            }
            else
            {
                responseAction.Invoke(new WebResponseData(ResponseDataType.None, null, null));
                yield break;
            }
        }
    }

    public void Get(string uri, Action<WebResponseData> responseAction)
    {
        StartCoroutine(GetRequestRoutine(uri, responseAction));
    }

    IEnumerator GetRequestRoutine(string uri, Action<WebResponseData> responseAction)
    {
        UnityWebRequest get = UnityWebRequest.Get(uri);
        Debug.Log("Get Request:" + uri);
        yield return get.SendWebRequest();

        if (get.isHttpError)
        {
            Debug.LogError("Encounter a http error when send get request:" + uri);
            if (responseAction != null) responseAction.Invoke(new WebResponseData(ResponseDataType.Error, null, null));
            yield break;
        }

        if (get.isNetworkError)
        {
            Debug.LogError("Encounter a system error when send get request:" + uri);
            if (responseAction != null) responseAction.Invoke(new WebResponseData(ResponseDataType.Error, null, null));
            yield break;
        }

        while (!get.downloadHandler.isDone)
        {
            yield return null;
        }

        if (responseAction != null)
        {
            if (!string.IsNullOrEmpty(get.downloadHandler.text))
            {
                responseAction.Invoke(new WebResponseData(ResponseDataType.Text, get.downloadHandler.text, null));
                yield break;
            }

            else if (get.downloadHandler.data != null)
            {
                responseAction.Invoke(new WebResponseData(ResponseDataType.Binary, null, get.downloadHandler.data));
                yield break;
            }
            else
            {
                responseAction.Invoke(new WebResponseData(ResponseDataType.None, null, null));
                yield break;
            }
        }
    }
    #endregion
}
