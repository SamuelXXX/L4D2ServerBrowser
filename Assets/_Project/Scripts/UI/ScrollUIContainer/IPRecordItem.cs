using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IPRecordItem : MonoBehaviour
{
    #region Settings
    public Text ipDisplayField;
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
        if (bindConnectInfo != null)
        {
            ipDisplayField.text = bindConnectInfo.ip + ":" + bindConnectInfo.port.ToString();
        }
    }
    #endregion

    #region UI Callbacks
    void OnDeletePressed()
    {
        if (bindConnectInfo != null)
        {
            IPListManager.Instance.DeleteIPRecord(bindConnectInfo);
            GetComponentInParent<IPRecordManager>().RemoveRecordUI(this);
            Destroy(gameObject);
        }
    }
    #endregion

    #region Exposed API
    IPData bindConnectInfo;
    public void BindIPRecord(IPData connectInfo)
    {
        bindConnectInfo = connectInfo;
        if (bindConnectInfo != null)
        {
            ipDisplayField.text = bindConnectInfo.ip + ":" + bindConnectInfo.port.ToString();
        }
    }
    #endregion
}
