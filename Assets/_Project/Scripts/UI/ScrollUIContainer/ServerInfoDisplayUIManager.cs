using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInfoDisplayUIManager : MonoBehaviour
{
    public RectTransform leftLayout;
    public RectTransform rightLayout;
    public ServerInfoDisplayItem uiPrefab;
    protected List<ServerInfoDisplayItem> managedDisplayUI = new List<ServerInfoDisplayItem>();

    #region Unity Life Cycle
    // Update is called once per frame
    void Update()
    {
        float height = Mathf.Max(leftLayout.rect.height, rightLayout.rect.height);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 40f);
    }
    #endregion

    public void GenerateUIGroup(List<L4D2ServerQueryAgent> agents)
    {
        DestroyUIGroup();
        RectTransform curLayout = leftLayout;
        foreach (var a in agents)
        {
            var go = Instantiate(uiPrefab.gameObject);
            go.SetActive(true);
            var ui = go.GetComponent<ServerInfoDisplayItem>();
            ui.GetComponent<RectTransform>().SetParent(curLayout, true);
            if (curLayout == leftLayout)
            {
                curLayout = rightLayout;
            }
            else
            {
                curLayout = leftLayout;
            }
            ui.BindAgent(a);
            managedDisplayUI.Add(ui);
        }
    }

    public void DestroyUIGroup()
    {
        foreach (var g in managedDisplayUI)
        {
            Destroy(g.gameObject);
        }
        managedDisplayUI.Clear();
    }
}
