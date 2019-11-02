using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCareManager : MonoBehaviour
{
    #region Data
    public SpecialCareItem uiPrefab;
    protected List<SpecialCareItem> specialCareItems = new List<SpecialCareItem>();
    #endregion

    #region UI Management
    public void GenerateUIGroup()
    {
        DestroyUIGroup();
        foreach (var r in VipIDManager.Instance.localSpecialCareEditingCache)
        {
            AddSpecialCareItem(r);
        }
    }

    public void AddSpecialCareItem(IDDecorator info)
    {
        if (info == null)
            return;

        var go = Instantiate(uiPrefab.gameObject);
        var ui = go.GetComponent<SpecialCareItem>();
        ui.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), true);
        ui.transform.localScale = Vector3.one;
        ui.BindIDDecorator(info);
        specialCareItems.Add(ui);
    }

    public void RemoveSpecialCareItem(SpecialCareItem r)
    {
        specialCareItems.Remove(r);
    }

    public void DestroyUIGroup()
    {
        foreach (var p in specialCareItems)
        {
            Destroy(p.gameObject);
        }

        specialCareItems.Clear();
    }
    #endregion
}
