using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInfoDisplayUIManager : ShortLifeSingleton<ServerInfoDisplayUIManager>
{
    public RectTransform leftLayout;
    public RectTransform rightLayout;
    public ServerInfoDisplayUI uiPrefab;
    protected List<ServerInfoDisplayUI> managedDisplayUI = new List<ServerInfoDisplayUI>();

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float height = Mathf.Max(leftLayout.rect.height, rightLayout.rect.height);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height+40f);
    }
    #endregion

    public void GenerateUIGroup()
    {
        DestroyUIGroup();
        RectTransform curLayout = leftLayout;
        foreach (var a in QueryAgentManager.Instance.agents)
        {
            var go = Instantiate(uiPrefab.gameObject);
            go.SetActive(true);
            var ui = go.GetComponent<ServerInfoDisplayUI>();
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
