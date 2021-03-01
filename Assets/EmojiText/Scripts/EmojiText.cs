using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class EmojiText : Text
{
    private const float ICON_SCALE_OF_DOUBLE_SYMBOLE = 1f;
    private readonly static string EMOJI_REPLACER_STR = "国";//Emoji占位符
    private readonly static int EMOJI_REPLACER_LENGTH = EMOJI_REPLACER_STR.Length; //Emoji占位符长度

    public override float preferredWidth
    {
        get
        {
            return cachedTextGeneratorForLayout.GetPreferredWidth(emojiText, GetGenerationSettings(rectTransform.rect.size)) / pixelsPerUnit;
        }
    }
    public override float preferredHeight
    {
        get
        {
            return cachedTextGeneratorForLayout.GetPreferredHeight(emojiText, GetGenerationSettings(rectTransform.rect.size)) / pixelsPerUnit;
        }      
    }

    public string emojiText;
    

    private static Dictionary<string,EmojiInfo> m_EmojiIndexDict = null;

	struct EmojiInfo
	{
		public float x;
		public float y;
		public float size;
	}
		
	readonly UIVertex[] m_TempVerts = new UIVertex[4];

    private static string GetConvertedString(string inputString)
    {
        //Debug.Log(inputString);
        string[] converted = inputString.Split('-');
        for (int j = 0; j < converted.Length; j++)
        {
            converted[j] = char.ConvertFromUtf32(Convert.ToInt32(converted[j], 16));
        }

        return string.Join(string.Empty, converted);
    }

    string GetRenderableText()
    {
        //Parse Renderable Text From Original Text to Calculate Emoji Render Position
        string renderableText = text;
        renderableText = Regex.Replace(renderableText, @"<color=?[ 0-9a-zA-Z#]*>|</color>|<b>|</b>|<i>|</i>|\s", "");
        return renderableText;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
	{
		if (font == null)
        {
            return;
        }
        if (m_EmojiIndexDict == null)
        {
            //Try Build Emoji Index Dict
            m_EmojiIndexDict = new Dictionary<string, EmojiInfo>();

			//load emoji data, and you can overwrite this segment code base on your project.
			TextAsset emojiContent = Resources.Load<TextAsset> ("emoji");
			string[] lines = emojiContent.text.Split ('\n');
			for(int i = 1 ; i < lines.Length; i ++)
			{
				if (! string.IsNullOrEmpty (lines [i])) {
					string[] strs = lines [i].Split ('\t');
					EmojiInfo info;
					info.x = float.Parse (strs [3]);
					info.y = float.Parse (strs [4]);
					info.size = float.Parse (strs [5]);
                    string key = strs[0].Substring(1, strs[0].Length - 2);//Remove '{' '}'
                    key = GetConvertedString(key);
                    m_EmojiIndexDict.Add (key, info);
				}
			}
		}

		Dictionary<int,EmojiInfo> emojiDic = new Dictionary<int, EmojiInfo> ();


        emojiText = text;
        
        if (supportRichText)
        {	
			int nOffset = 0;
          
            int i = 0;
            string renderableText = GetRenderableText();

            //Debug.Log(renderableText);
            while (i < renderableText.Length)
            {
                string singleChar = renderableText.Substring(i, 1);
                string doubleChar = "";
                string fourChar = "";

                if (i < (renderableText.Length - 1))
                {
                    doubleChar = renderableText.Substring(i, 2);
                }

                if (i < (renderableText.Length - 3))
                {
                    fourChar = renderableText.Substring(i, 4);
                }

                EmojiInfo info;
                if (m_EmojiIndexDict.TryGetValue(fourChar, out info))
                {
                    // 四字符Emoji
                    emojiDic.Add(i - nOffset, info);
                    emojiText = emojiText.Replace(fourChar, EMOJI_REPLACER_STR);
                    nOffset += (4-EMOJI_REPLACER_LENGTH);
                    i += 4;
                }
                else if (m_EmojiIndexDict.TryGetValue(doubleChar, out info))
                {
                    // 双字符Emoji
                    emojiDic.Add(i - nOffset, info);
                    emojiText = emojiText.Replace(doubleChar, EMOJI_REPLACER_STR);
                    nOffset += (2 - EMOJI_REPLACER_LENGTH);
                    i += 2;
                }
                else if (m_EmojiIndexDict.TryGetValue(singleChar, out info))
                {
                    // 单字符Emoji
                    emojiDic.Add(i - nOffset, info);
                    emojiText = emojiText.Replace(singleChar, EMOJI_REPLACER_STR);
                    nOffset += (1 - EMOJI_REPLACER_LENGTH);
                    i++;
                }
                else
                {
                    i++;
                }
            }
        }

		// We don't care if we the font Texture changes while we are doing our Update.
		// The end result of cachedTextGenerator will be valid for this instance.
		// Otherwise we can get issues like Case 619238.
		m_DisableFontTextureRebuiltCallback = true;

		Vector2 extents = rectTransform.rect.size;
        //Debug.Log(emojiText);
		var settings = GetGenerationSettings(extents);
		cachedTextGenerator.Populate(emojiText, settings);		

		Rect inputRect = rectTransform.rect;

		// get the text alignment anchor point for the text in local space
		Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
		Vector2 refPoint = Vector2.zero;
		refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
		refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
		float unitsPerPixel = 1 / pixelsPerUnit;
		int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0)
        {
            toFill.Clear();
            return;
        }

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                {
                    toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
        }
        else
        {
            //Debug.Log(verts.Count);
            for (int i = 0; i < verts.Count; ++i)
            {
				EmojiInfo info;
                int index = i / 4;
                if (emojiDic.TryGetValue (index, out info))
                {
                    //Processing Emoji
                    //计算2个EE的距离
                    float emojiSize = EMOJI_REPLACER_LENGTH * (verts[i + 1].position.x - verts[i].position.x) * ICON_SCALE_OF_DOUBLE_SYMBOLE;

                    float fCharHeight = verts[i + 1].position.y - verts[i + 2].position.y;
                    float fCharWidth = verts[i + 1].position.x - verts[i].position.x;

                    float fHeightOffsetHalf = (emojiSize - fCharHeight) * 0.5f;
                    float fStartOffset = emojiSize * (1 - ICON_SCALE_OF_DOUBLE_SYMBOLE);

                    m_TempVerts [3] = verts [i];//1
					m_TempVerts [2] = verts [i + 1];//2
					m_TempVerts [1] = verts [i + 2];//3
					m_TempVerts [0] = verts [i + 3];//4

                    m_TempVerts[0].position += new Vector3(fStartOffset, -fHeightOffsetHalf, 0);
                    m_TempVerts[1].position += new Vector3(fStartOffset - fCharWidth + emojiSize, -fHeightOffsetHalf, 0);
                    m_TempVerts[2].position += new Vector3(fStartOffset - fCharWidth + emojiSize, fHeightOffsetHalf, 0);
					m_TempVerts [3].position += new Vector3(fStartOffset, fHeightOffsetHalf, 0);
					
					m_TempVerts [0].position *= unitsPerPixel;
					m_TempVerts [1].position *= unitsPerPixel;
					m_TempVerts [2].position *= unitsPerPixel;
					m_TempVerts [3].position *= unitsPerPixel;
					
					float pixelOffset = emojiDic [index].size / 32 / EMOJI_REPLACER_LENGTH;
					m_TempVerts [0].uv1 = new Vector2 (emojiDic [index].x + pixelOffset, emojiDic [index].y + pixelOffset);
					m_TempVerts [1].uv1 = new Vector2 (emojiDic [index].x - pixelOffset + emojiDic [index].size, emojiDic [index].y + pixelOffset);
					m_TempVerts [2].uv1 = new Vector2 (emojiDic [index].x - pixelOffset + emojiDic [index].size, emojiDic [index].y - pixelOffset + emojiDic [index].size);
					m_TempVerts [3].uv1 = new Vector2 (emojiDic [index].x + pixelOffset, emojiDic [index].y - pixelOffset + emojiDic [index].size);

					toFill.AddUIVertexQuad (m_TempVerts);

                    i += 4 * EMOJI_REPLACER_LENGTH - 1;
                }
                else
                {					
					int tempVertsIndex = i & 3;
					m_TempVerts [tempVertsIndex] = verts [i];
					m_TempVerts [tempVertsIndex].position *= unitsPerPixel;
					if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
			}

		}
		m_DisableFontTextureRebuiltCallback = false;
	}
}
