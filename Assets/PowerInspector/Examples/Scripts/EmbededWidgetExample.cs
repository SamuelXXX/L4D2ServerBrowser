using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerInspector.Example
{
    public class EmbededWidgetExample : PowerEditorExampleBase
    {
        /// <summary>
        /// Create a space gap in inspector
        /// </summary>
        public PowerSpaceField exampleSpace = new PowerSpaceField(20f);
        /// <summary>
        /// Put a texture in inspector
        /// </summary>
        public PowerTextureField anotherLogo = new PowerTextureField("PowerInspectorLogo", 0.3f, ScaleMode.ScaleAndCrop);
        /// <summary>
        /// Put a button in inspector with button name and button callback name
        /// </summary>
        public PowerButton exampleButton = new PowerButton("Example Button", "OnButtonPressed", 30f);

        void OnButtonPressed()
        {
            Debug.Log("Example Button Pressed!");
        }
    }
}



