using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerInspector;

public abstract class PageUIBase : MonoBehaviour
{
    #region Page Settings
    [SerializeField, ReadOnly] protected int pageId = 0;
    public string PageID
    {
        get
        {
            return pageId.ToString();
        }
    }

    [Header("Page Properties Settings")]
    public bool isWindow = false;
    [HideWhenFalse(condition = "isWindow")]
    public bool killWindowIfGoBack = false;
    #endregion

    #region Runtime Data
    private object[] pageOpenParameters;
    #endregion

    #region Life Cycle
    private void OnValidate()
    {
        if (pageId == 0)
            pageId = Random.Range(1, 10000);
    }

    private void OnEnable()
    {
        OnLoadPage(pageOpenParameters);
    }

    private void OnDisable()
    {
        OnDestroyPage();
        pageOpenParameters = null;
    }
    #endregion

    #region Exposed API For UI Manager
    public void OpenPage(params object[] pars)
    {
        pageOpenParameters = pars;
        gameObject.SetActive(true);
    }

    public void ClosePage()
    {
        gameObject.SetActive(false);
    }
    #endregion


    #region UI Life Cycle
    protected virtual void OnLoadPage(params object[] pars)
    {

    }

    public virtual void OnPause()
    {

    }

    public virtual void OnResume()
    {

    }

    protected virtual void OnDestroyPage()
    {

    }
    #endregion
}
