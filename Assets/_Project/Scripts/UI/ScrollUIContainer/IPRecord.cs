using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IPRecord : MonoBehaviour
{
    public Text ipDisplayField;
    public Button delButton;

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

    ServerConnectInfo bindConnectInfo;
    public void BindIPRecord(ServerConnectInfo connectInfo)
    {
        bindConnectInfo = connectInfo;
    }

    void OnDeletePressed()
    {
        if (bindConnectInfo != null)
        {
            ServerInfoLibraryManager.Instance.DeleteIPRecord(bindConnectInfo);
            GetComponentInParent<IPRecordManager>().RemoveRecordUI(this);
            Destroy(gameObject);
        }
    }
}
