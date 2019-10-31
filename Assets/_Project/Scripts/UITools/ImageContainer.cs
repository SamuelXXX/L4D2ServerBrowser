using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ImageContainer : MonoBehaviour
{
    public enum TextureDisplayMode
    {
        Contain = 0,
        Cover
    }

    public TextureDisplayMode textureDisplayMode;

    [SerializeField]
    protected Image m_ImageContainer;
    protected RectTransform m_rectTransform;


    #region Unity Life Cycle
    private void Awake()
    {
        if (m_ImageContainer != null)
        {
            if (m_ImageContainer.transform.parent != transform)
                m_ImageContainer = null;
            else
            {
                m_ImageContainer.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                m_ImageContainer.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                m_ImageContainer.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                m_ImageContainer.rectTransform.localPosition = Vector3.zero;
                m_ImageContainer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
                m_ImageContainer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
            }
        }


        m_rectTransform = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void LateUpdate()
    {
        float sizeRatio = 1f;

        Texture tex = m_ImageContainer.sprite.texture;
        if (tex != null)
            sizeRatio = (float)tex.width / (float)tex.height;

        float width = m_rectTransform.rect.width;
        float height = m_rectTransform.rect.height;
        float parentSizeRatio = m_rectTransform.rect.width / m_rectTransform.rect.height;

        width = m_rectTransform.rect.width;
        height = m_rectTransform.rect.height;
        parentSizeRatio = m_rectTransform.rect.width / m_rectTransform.rect.height;

        switch (textureDisplayMode)
        {
            case TextureDisplayMode.Contain:
                if (parentSizeRatio < sizeRatio)//width align
                {
                    height = width / sizeRatio;
                }
                else//height align
                {
                    width = height * sizeRatio;
                }
                break;
            case TextureDisplayMode.Cover:
                if (parentSizeRatio > sizeRatio)//width align
                {
                    height = width / sizeRatio;
                }
                else//height align
                {
                    width = height * sizeRatio;
                }
                break;
            default: break;
        }

        m_ImageContainer.rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        SetImageUISize(width, height);
    }
    #endregion

    void SetImageUISize(float width, float height)
    {
        if (m_ImageContainer == null)
            return;

        m_ImageContainer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        m_ImageContainer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
