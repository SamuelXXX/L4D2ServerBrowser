using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ValveServerQuery;

public class PlayerInfoItem : MonoBehaviour
{
    #region UI Reference
    [Header("UI Reference")]
    public Image backgroundImage;
    public EmojiText idText;
    public Text scoreText;
    public Text durationText;
    #endregion

    void SetBackgroundColor()
    {
        Color color = backgroundImage.color;
        color.a = 1;
        backgroundImage.color = color;
    }

    void ResetBackgroundColor()
    {
        Color color = backgroundImage.color;
        color.a = 0;
        backgroundImage.color = color;
    }

    int index;
    public void BindIndex(int index)
    {
        this.index = index;
    }

    public void UpdateContent(ValveServerResponseData.PlayerInfo playerInfo)
    {
        if (playerInfo == null)
        {
            if (index % 2 == 1)
            {
                ResetBackgroundColor();
            }
            else
            {
                SetBackgroundColor();
            }

            idText.text = "";
            scoreText.text = "";
            durationText.text = "";
        }
        else
        {
            if (index % 2 == 1)
            {
                ResetBackgroundColor();
            }
            else
            {
                SetBackgroundColor();
            }

            idText.text = PlayerIDManager.Instance.DecoratePlayerID(playerInfo.name.Replace("\0", ""));
            scoreText.text = playerInfo.score.ToString();
            TimePeriod p = new TimePeriod(Mathf.FloorToInt(playerInfo.duration));

            durationText.text = p.ToString();
        }
    }

    public class TimePeriod
    {
        public int hour;
        public int minute;
        public int second;

        public TimePeriod(int seconds)
        {
            hour = seconds / 3600;
            seconds %= 3600;

            minute = seconds / 60;
            seconds %= 60;

            second = seconds;
        }

        public override string ToString()
        {
            string s = "";
            if (hour != 0)
            {
                s += hour.ToString() + "h";
            }

            if (s == "")
            {
                if (minute != 0)
                {
                    s += minute.ToString() + "m";
                }
            }
            else
            {
                s += minute.ToString() + "m";
            }

            s += second.ToString() + "s";
            return s;
        }
    }

}
