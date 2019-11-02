using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialCareItem : MonoBehaviour
{
    #region Settings
    public Text idDisplayField;
    public Text statusDisplayText;
    public Button delButton;
    #endregion

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        delButton.onClick.AddListener(OnDeletePressed);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }
    #endregion

    void UpdateUI()
    {
        if (bindDecorator != null)
        {
            idDisplayField.text = bindDecorator.id;
            PlayerIDRunningData playerData = PlayerIDManager.Instance.collectedPlayerData.Find(p => p.id == bindDecorator.id);
            if (playerData == null)
            {
                statusDisplayText.text = "尚未查找到该id当前游戏信息";
            }
            else
            {
                statusDisplayText.text = "所在服务器:" + playerData.serverName.Replace("\0", "") + "  游戏时间:" + new PlayerInfoItem.TimePeriod(Mathf.FloorToInt(playerData.gamingTime)).ToString();
            }
        }
    }

    #region UI Callbacks
    void OnDeletePressed()
    {
        if (bindDecorator != null)
        {
            VipIDManager.Instance.DeleteSpecialCareID(bindDecorator);
            GetComponentInParent<SpecialCareManager>().RemoveSpecialCareItem(this);
            Destroy(gameObject);
        }
    }
    #endregion

    #region Exposed API
    IDDecorator bindDecorator;
    public void BindIDDecorator(IDDecorator decorator)
    {
        bindDecorator = decorator;
        UpdateUI();
    }
    #endregion
}
