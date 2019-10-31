using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerInspector;

public class NotchSizeController : MonoBehaviour
{
    #region Settings
    public RectTransform notch;
    public RectTransform safeArea;

    [SerializeField, PresetData] protected float notchHeight;
    [SerializeField] protected float minimumNotchHeight = 0f;
    #endregion

    #region Unity Life Cycle
    // Start is called before the first frame update
    void Start()
    {
        var safeAreaSize = new Rect(0, 0, 1f, 1f);
        safeAreaSize.width = Screen.safeArea.width / Screen.width;
        safeAreaSize.height = (Screen.safeArea.height + Screen.safeArea.y) / Screen.height;
        notchHeight = 1 - safeAreaSize.height;

        //Looks like that Screen.safeArea is not compatiable for ios
#if UNITY_EDITOR
        notchHeight = 0.042f;
#endif

        notchHeight = Mathf.Clamp(notchHeight, minimumNotchHeight, 1f);

        UpdateNotchSize();

        Debug.Log("Screen.width:" + Screen.width);
        Debug.Log("Screen.height:" + Screen.height);
        Debug.Log("Screen.safeArea:" + Screen.safeArea.ToString());
        Debug.Log("Calculated Relative Safe Area:" + safeAreaSize.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        UpdateNotchSize();
    }

    void UpdateNotchSize()
    {
        if (notch != null)
        {
            notch.anchorMin = new Vector2(0, 1 - notchHeight);
            notch.anchorMax = new Vector2(1f, 1f);
        }

        if (safeArea != null)
        {
            safeArea.anchorMin = new Vector2(0, 0);
            safeArea.anchorMax = new Vector2(1, 1 - notchHeight);
        }
    }
    #endregion
}
