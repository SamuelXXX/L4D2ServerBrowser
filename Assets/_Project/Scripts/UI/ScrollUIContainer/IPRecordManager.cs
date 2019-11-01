using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPRecordManager : MonoBehaviour
{
    public IPRecord uiPrefab;

    protected List<IPRecord> ipRecords = new List<IPRecord>();
    public void GenerateUIGroup()
    {
        DestroyUIGroup();
        foreach (var r in ServerInfoLibraryManager.Instance.localServerInfoEditingCache)
        {
            AddRecordUI(r);
        }
    }

    public void AddRecordUI(ServerConnectInfo info)
    {
        if (info == null)
            return;

        var go = Instantiate(uiPrefab.gameObject);
        var ui = go.GetComponent<IPRecord>();
        ui.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), true);
        ui.transform.localScale = Vector3.one;
        ui.BindIPRecord(info);
        ipRecords.Add(ui);
    }

    public void RemoveRecordUI(IPRecord r)
    {
        ipRecords.Remove(r);
    }

    public void DestroyUIGroup()
    {
        foreach (var p in ipRecords)
        {
            Destroy(p.gameObject);
        }

        ipRecords.Clear();
    }
}
