using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerInspector;

namespace PowerInspector.Example
{
    public class AccessLevelModification : PowerEditorExampleBase
    {
        /// <summary>
        /// 'PresetData' attribute make target field's readonly when playing.
        /// Works on all serializable fields.
        /// </summary>
        [PresetData]
        public float presetData = 0.1f;


        /// <summary>
        /// 'ReadOnly' attribute make target field's readonly when editing and playing.
        /// Works on all serializable fields.
        /// </summary>
        [ReadOnly]
        public float readOnly = 0.1f;

        /// <summary>
        /// 'RuntimeData' attribute make target field's invisible when editing and readonly when playing.
        /// Works on all serializable fields.
        /// </summary>
        [RuntimeData]
        public float runtimeData = 0.1f;

        [Header("Condition Hiding")]
        public bool condition = false;

        /// <summary>
        /// 'HideWhenTrue' attribute make target field's condition fulfiled.
        /// Works on all serializable fields.
        /// 'condition' parameter mark a boolean type field as condition checker
        /// </summary>
        [HideWhenTrue(condition = "condition")]
        public float hideWhenTrue = 0.1f;

        /// <summary>
        /// 'HideWhenFalse' is the opposite attribute of 'HideWhenTrue'
        /// </summary>
        [HideWhenFalse(condition = "condition")]
        public float hideWhenFalse = 0.1f;
    }
}

