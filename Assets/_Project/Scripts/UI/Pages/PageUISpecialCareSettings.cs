using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageUISpecialCareSettings : PageUIBase
{
    #region Settings
    [Header("UI Reference")]
    public Button addButton;
    public Button exitButton;
    public GameObject idInputWindow;
    public InputField idField;

    public Button idConfirmButton;
    public Button idCancelButton;
    public SpecialCareManager specialCaresManager;
    #endregion

    protected override void OnLoadPage(params object[] pars)
    {
        base.OnLoadPage(pars);
        VipIDManager.Instance.EnterLocalEditingMode();
        specialCaresManager.GenerateUIGroup();
        addButton.onClick.AddListener(OnAddButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        idConfirmButton.onClick.AddListener(OnIDConfirmButtonClicked);
        idCancelButton.onClick.AddListener(OnIDCancelButtonClicked);
        idInputWindow.SetActive(false);
    }

    protected override void OnDestroyPage()
    {
        base.OnDestroyPage();
        addButton.onClick.RemoveListener(OnAddButtonClicked);
        exitButton.onClick.RemoveListener(OnExitButtonClicked);
        idConfirmButton.onClick.RemoveListener(OnIDConfirmButtonClicked);
        idCancelButton.onClick.RemoveListener(OnIDCancelButtonClicked);
        VipIDManager.Instance.ExitLocalEditingMode();
    }

    void OnAddButtonClicked()
    {
        idInputWindow.SetActive(true);
        idField.text = "";
    }

    void OnIDCancelButtonClicked()
    {
        idInputWindow.SetActive(false);
    }

    void OnIDConfirmButtonClicked()
    {
        idInputWindow.SetActive(false);
        specialCaresManager.AddSpecialCareItem(VipIDManager.Instance.AppendSpecialCareID(idField.text));
    }

    void OnExitButtonClicked()
    {
        PageUIManager.Instance.GoBack();
    }
}
