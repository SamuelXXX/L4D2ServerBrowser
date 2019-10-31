using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[ExecuteInEditMode]
public class TextFormatter : MonoBehaviour
{
    static readonly string no_breaking_space = "\u00A0";
    protected Text m_Text;

    private void Update()
    {
        if (m_Text == null)
            m_Text = GetComponent<Text>();

        OnTextChange();
    }

    public void OnTextChange()
    {
        m_Text.text = m_Text.text.Replace(" ", no_breaking_space);
    }
}
