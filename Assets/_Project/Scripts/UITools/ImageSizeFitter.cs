using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class ImageSizeFitter : MonoBehaviour
{
    protected RectTransform m_rect;
    protected Image m_Image;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_rect == null)
            m_rect = GetComponent<RectTransform>();
        if (m_Image == null)
            m_Image = GetComponent<Image>();
        float width = m_rect.rect.width;
        if (m_Image.sprite != null)
        {
            float ratio = (float)m_Image.sprite.texture.height / (float)m_Image.sprite.texture.width;
            float height = width * ratio;
            SetImageUISize(width, height);
        }
    }

    void SetImageUISize(float width, float height)
    {
        if (m_rect == null)
            return;

        m_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        m_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
