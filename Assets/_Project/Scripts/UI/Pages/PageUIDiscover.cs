using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIDiscover : PageUIBase
{
    public RectTransform contentRoot;
    protected GameObject contentUI;

    #region Internal Methods
    protected List<PageIndex> GetAllPages()
    {
        if (PageUIManager.Instance != null)
        {
            return PageUIManager.Instance.GetAllPages();
        }
        else
        {
            return new List<PageIndex>();
        }
    }
    #endregion

    #region Page Life Cycle
    protected override void OnLoadPage(params object[] pars)
    {
        //
        StartCoroutine(LoadPageRoutine());
    }

    IEnumerator LoadPageRoutine()
    {
        if(DiscoverContentManager.Instance.ContentUIPrefab!=null)
        {
            CreateContentUI();
            yield break;
        }

        DiscoverContentManager.Instance.CommitDiscoverContentRequest();

        float timer = 0f;
        while(DiscoverContentManager.Instance.ContentUIPrefab==null)
        {
            timer += Time.deltaTime;
            if(timer>20f)
            {
                yield break;
            }
            yield return null;
        }

        CreateContentUI();
    }

    void CreateContentUI()
    {
        contentUI = Instantiate(DiscoverContentManager.Instance.ContentUIPrefab);
        contentUI.SetActive(true);

        RectTransform rectTransform = contentUI.GetComponent<RectTransform>();
        rectTransform.SetParent(contentRoot);

        rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);

        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);

        //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
        //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);

        rectTransform.localScale =new Vector3(1, 1, 1);
    }

    public void GoBack()
    {
        StopAllCoroutines();
        PageUIManager.Instance.GoBack();
    }

    protected override void OnDestroyPage()
    {
        if(contentUI!=null)
        {
            Destroy(contentUI);
        }
    }
    #endregion
}
