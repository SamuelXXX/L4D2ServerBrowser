using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PowerInspector.Example
{
    public abstract class PowerEditorExampleBase : MonoBehaviour
    {
        public PowerSpaceField upSpace = new PowerSpaceField(20f);
        public PowerTextureField logo = new PowerTextureField("PowerInspectorLogo", 0.3f, ScaleMode.ScaleAndCrop);
        public PowerButton button = new PowerButton("Press me to check effect at run time!", "Play", 30f);
        public PowerSpaceField downSpace = new PowerSpaceField(20f);

        protected void Play()
        {
#if UNITY_EDITOR&&UNITY_2019
            EditorApplication.EnterPlaymode();
#elif UNITY_EDITOR
            Debug.Log("Looks like entering play mode by script is not allowed in your unity version.");
#endif
        }

    }
}


