using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerInspector;

namespace PowerInspector.Example
{
    public class ColorModification : PowerEditorExampleBase
    {
        /// <summary>
        /// 'ColorTheme' attribute modifies target field's color style.
        /// Works on all serializable fields.
        /// Press menu 'PowerEditor/Open Color Theme' to edit color themes.
        /// </summary>
        [Header("Color Theme Reference")]    
        [ColorTheme("Dark")]
        public float darkStyle = 0.1f;

        [ColorTheme("White")]
        public float whiteStyle = 0.1f;

        /// <summary>
        /// You can modify color on color theme base if you wish to. 
        /// Two color code format supported:
        /// #RRGGBB:Popularest color format, each color channel are represented in hex-format such as '#FF99EE'
        /// (r,g,b):Color vector, each color channel are represented in dec-format such as '(0,255,128)'.
        /// </summary>
        [Header("Color Modification")]
        [ColorTheme(fontColor = "#00FF0")]
        public float greenFont = 0.1f;
        [ColorTheme(backgroundColor = "#00FF00")]
        public float greenBackground = 0.1f;

        [ColorTheme(runtimeFontColor = "#00FF00")]
        public float greenFontWhenRun = 0.1f;
        [ColorTheme(runtimeBackgroundColor = "#00FF00")]
        public float greenBackgroundWhenRun = 0.1f;

        [ColorTheme(fontColor = "#FF0000", backgroundColor = "#00FFFF", runtimeFontColor = "#0000FF", runtimeBackgroundColor = "#FFFF00")]
        public float combinationColor = 0.1f;

    }
}



