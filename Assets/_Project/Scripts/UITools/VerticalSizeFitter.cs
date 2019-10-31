using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class VerticalSizeFitter : MonoBehaviour
{
    protected RectTransform m_rect;
    public List<RectTransform> childRect = new List<RectTransform>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_rect == null)
            m_rect = GetComponent<RectTransform>();

        float height = 0f;
        foreach (var c in childRect)
        {
            Vector2 ap = c.anchoredPosition;
            ap.y = -height;
            c.anchoredPosition = ap;
            height += c.rect.height;
        }
        float width = m_rect.rect.width;
        SetUISize(width, height);
    }

    void SetUISize(float width, float height)
    {
        if (m_rect == null)
            return;

        m_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        m_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
