using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FileDownloader : MonoBehaviour
{
    public string urlFilePath;
    public string fileStorePath;

    [ContextMenu("Partial Download")]
    void PartialDownload()
    {
        StartCoroutine(PartialDownloadRoutine(urlFilePath));
    }

    [ContextMenu("Whole Download")]
    void WholeDownload()
    {
        StartCoroutine(WholeDownloadRoutine(urlFilePath));
    }

    IEnumerator PartialDownloadRoutine(string apkDownloadPath)
    {
        string path = fileStorePath;
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);

        ulong length = 0;
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(apkDownloadPath);
            www.SetRequestHeader("Range", "bytes=" + length + "-");

            yield return www.SendWebRequest();

            Debug.Log("Response Code:" + www.responseCode);

            string contentRange = www.GetResponseHeader("Content-Range");
            Debug.Log("Content-Range:" + contentRange);
            ContentRangeHandler crh = new ContentRangeHandler(contentRange);
            length += www.downloadedBytes;


            bw.Write(www.downloadHandler.data);

            if (crh.IsFinalChunk())
                break;
        }

        bw.Close();
        fs.Close();

    }

    [System.Serializable]
    public class RequestHeader
    {
        public string key;
        public string value;
    }

    [Header("Request Headers"), PowerInspector.PowerList(Key = "key")]
    public List<RequestHeader> requestHeaders = new List<RequestHeader>();


    IEnumerator WholeDownloadRoutine(string downloadPath)
    {
        string path = fileStorePath;
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryWriter bw = new BinaryWriter(fs);

        UnityWebRequest www = UnityWebRequest.Get(downloadPath);

        foreach (var r in requestHeaders)
        {
            www.SetRequestHeader(r.key, r.value);
        }

        www.SendWebRequest();

        while (!www.isDone)
        {
            Debug.Log(www.downloadProgress);
            yield return null;
        }

        Debug.Log("Response Code:" + www.responseCode);

        foreach (var c in www.GetResponseHeaders())
        {
            Debug.Log(c.Key + ":" + c.Value);
        }

        Debug.Log(www.downloadHandler.text);

        string contentRange = www.GetResponseHeader("Content-Range");

        bw.Write(www.downloadHandler.data);

        bw.Close();
        fs.Close();

    }

    public class ContentRangeHandler
    {
        public int start;
        public int end;
        public int total;
        public ContentRangeHandler(string responseHeader)
        {
            if (responseHeader == null)
                return;
            string[] pars = responseHeader.Split(' ', '-', '/');
            if (pars.Length != 4)
            {
                Debug.LogError("Content Range Length Not Matched");
                return;
            }

            start = int.Parse(pars[1]);
            end = int.Parse(pars[2]);
            total = int.Parse(pars[3]);
        }

        public bool IsFinalChunk()
        {
            return end == total - 1;
        }
    }


}
