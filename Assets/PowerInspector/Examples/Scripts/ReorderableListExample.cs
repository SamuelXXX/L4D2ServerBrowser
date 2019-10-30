using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerInspector.Example
{
    public class ReorderableListExample : PowerEditorExampleBase
    {
        /// <summary>
        /// 'PowerList' attribute mark a list as reorderable list supported by Unity internal editor.
        /// 'Key' parameter mark a string field in target element type as display content on the reorderable list UI.
        /// </summary>
        [PowerList(Key ="displayKey")]
        public List<ExampleElement> reorderableConversations = new List<ExampleElement>();
    }
}


