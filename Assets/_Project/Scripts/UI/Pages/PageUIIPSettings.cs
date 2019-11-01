using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUIIPSettings : PageUIBase
{
    public static bool ipDirty = false;
    #region Settings
    [Header("UI Reference")]
    public Button addButton;
    public Button exitButton;
    public GameObject ipInputWindow;
    public InputField ipField;
    public InputField portField;
    public Button ipConfirmButton;
    public Button ipCancelButton;
    public IPRecordManager ipRecordManager;
    #endregion

    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        ServerInfoLibraryManager.Instance.EnterLocalEditingMode();
        ipRecordManager.GenerateUIGroup();
        addButton.onClick.AddListener(OnAddButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        ipConfirmButton.onClick.AddListener(OnIPConfirmButtonClicked);
        ipCancelButton.onClick.AddListener(OnIPCancelButtonClicked);
        ipInputWindow.SetActive(false);
    }

    protected override void OnDestroyPage()
    {
        base.OnDestroyPage();
        addButton.onClick.RemoveListener(OnAddButtonClicked);
        exitButton.onClick.RemoveListener(OnExitButtonClicked);
        ipConfirmButton.onClick.RemoveListener(OnIPConfirmButtonClicked);
        ipCancelButton.onClick.RemoveListener(OnIPCancelButtonClicked);
        ipDirty = ServerInfoLibraryManager.Instance.ExitLocalEditingMode();
    }

    void OnAddButtonClicked()
    {
        ipInputWindow.SetActive(true);
        ipField.text = "";
        portField.text = "";
    }

    void OnIPCancelButtonClicked()
    {
        ipInputWindow.SetActive(false);
    }

    void OnIPConfirmButtonClicked()
    {
        ipInputWindow.SetActive(false);
        ipRecordManager.AddRecordUI(ServerInfoLibraryManager.Instance.AppendIPRecord(ipField.text, ushort.Parse(portField.text)));
    }

    void OnExitButtonClicked()
    {
        PageUIManager.Instance.GoBack();
    }
}
