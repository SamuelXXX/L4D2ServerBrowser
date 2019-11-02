using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPRecordManager : MonoBehaviour
{
    #region Data
    public IPRecordItem uiPrefab;
    protected List<IPRecordItem> ipRecords = new List<IPRecordItem>();
    #endregion

    #region UI Management
    public void GenerateUIGroup()
    {
        DestroyUIGroup();
        foreach (var r in IPListManager.Instance.localServerInfoEditingCache)
        {
            AddRecordUI(r);
        }
    }

    public void AddRecordUI(IPData info)
    {
        if (info == null)
            return;

        var go = Instantiate(uiPrefab.gameObject);
        var ui = go.GetComponent<IPRecordItem>();
        ui.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), true);
        ui.transform.localScale = Vector3.one;
        ui.BindIPRecord(info);
        ipRecords.Add(ui);
    }

    public void RemoveRecordUI(IPRecordItem r)
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
    #endregion
}
