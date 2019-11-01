using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PowerInspector;
using System.Linq;

public enum BackButtonAction
{
    None = 0,
    GoBack,
    QuitApp,
    Custom
}


[System.Serializable]
public class PageIndex
{
    public string pageName;
    [ReadOnly]
    public string pageID;
    public string pageHeader;
    public PageUIBase page;
}

public class PageUIManager : ShortLifeSingleton<PageUIManager>
{
    public enum UICommandType
    {
        Loading = 0,
        GoBack
    }

    public class UICommand
    {
        public UICommandType commandType;
        public PageUIBase targetPage;
        public object[] loadingParameters;

        public UICommand(UICommandType commandType, PageUIBase targetPage = null, params object[] loadingParameters)
        {
            this.commandType = commandType;
            this.targetPage = targetPage;
            this.loadingParameters = loadingParameters;
        }
    }

    protected Queue<UICommand> uiCommands = new Queue<UICommand>();

    #region Settings
    [Header("UI Pages")]
    [PowerList(Key = "pageName"), SerializeField]
    protected List<PageIndex> pages = new List<PageIndex>();
    #endregion

    #region Runtime Data
    protected Stack<PageUIBase> pageStack = new Stack<PageUIBase>();
    #endregion

    #region Life Cycle
    private void Start()
    {
        if (pages.Count != 0)
            LoadPage(pages[0].page);
    }

    private void OnValidate()
    {
        foreach (var m in pages)
        {
            if (m.page != null)
            {
                m.pageID = m.page.PageID;
            }
        }
    }

    private void LateUpdate()
    {
        while (uiCommands.Count != 0)
        {
            var cmd = uiCommands.Dequeue();
            if (cmd != null)
            {
                switch (cmd.commandType)
                {
                    case UICommandType.Loading:
                        internalLoadPage(cmd.targetPage, cmd.loadingParameters);
                        break;
                    case UICommandType.GoBack:
                        internalGoBack();
                        break;
                    default: break;
                }
            }
        }
    }
    #endregion

    #region Page Navigate
    protected void internalLoadPage(PageUIBase page, params object[] pars)
    {
        if (page == null)
            return;

        var pageIndex = pages.Find(p => p.page == page);
        if (pageIndex == null)
            return;

        if (!page.isWindow)
        {
            CloseCurrentPage();
        }
        else
        {
            PauseCurrentPage();
        }

        pageStack.Push(page);
        page.OpenPage(pars);
    }

    protected void internalGoBack()
    {
        if (pageStack.Count == 0)
        {
            Debug.LogWarning("Cannot Go Back Because Page Stack Is Empty!");
            return;
        }

        var topPage = pageStack.Pop();
        topPage.ClosePage();
        if (topPage.isWindow)
        {
            ResumeCurrentPage();
        }
        else
        {
            ReOpenCurrentPage();
        }

        var currentPage = pageStack.Peek();
        if (currentPage.isWindow && currentPage.killWindowIfGoBack)
        {
            pageStack.Pop();
            currentPage.ClosePage();//Pop and close this window!
            ResumeCurrentPage();
        }
    }

    public void LoadPage(PageUIBase page, params object[] pars)
    {
        uiCommands.Enqueue(new UICommand(UICommandType.Loading, page, pars));
    }

    public void LoadPageByPageName(string pageName, params object[] pars)
    {
        var pageIndexer = pages.Find(p => p.pageName == pageName);
        if (pageIndexer == null || pageIndexer.page == null)
        {
            Debug.LogWarning("Page '" + pageName + "' Not Found!!!");
            return;
        }

        var page = pageIndexer.page;
        LoadPage(page, pars);
    }

    public void LoadPageByPageID(string pageID, params object[] pars)
    {
        var pageIndexer = pages.Find(p => p.page.PageID == pageID);
        if (pageIndexer == null || pageIndexer.page == null)
        {
            Debug.LogWarning("Page with ID '" + pageID + "' Not Found!!!");
            return;
        }

        var page = pageIndexer.page;
        LoadPage(page, pars);
    }

    public void GoBack()
    {
        uiCommands.Enqueue(new UICommand(UICommandType.GoBack));
    }

    public List<PageIndex> GetAllPages()
    {
        return pages;
    }
    #endregion

    #region Current Page Operation
    void ReOpenCurrentPage()
    {
        if (pageStack.Count != 0)
        {
            var currentPage = pageStack.Peek();
            currentPage.OpenPage();
            var index = pages.Find(p => p.page == currentPage);
        }
    }

    void CloseCurrentPage()
    {
        if (pageStack.Count != 0)
        {
            var currentPage = pageStack.Peek();
            currentPage.ClosePage();
        }

    }

    void PauseCurrentPage()
    {
        if (pageStack.Count != 0)
        {
            var currentPage = pageStack.Peek();
            currentPage.OnPause();
        }
    }

    void ResumeCurrentPage()
    {
        if (pageStack.Count != 0)
        {
            var currentPage = pageStack.Peek();
            currentPage.OnResume();
        }
    }
    #endregion
}
