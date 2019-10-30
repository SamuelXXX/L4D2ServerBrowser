using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerInspector;

namespace PowerInspector.Example
{
    [System.Serializable]
    public class ExampleElement
    {
        public string displayKey;
        [SerializeField]
        protected int refId;
    }

    public class ElementSelectorExample : PowerEditorExampleBase
    {
        #region String Selector
        [Header("String Selector")]
        public List<string> weekDaySampleSet = new List<string>(new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" });


        /// <summary>
        /// 'StringSelector' attribute modify target string-type field as a string selector.
        /// Works only on string-type fields.
        /// 'ListGetter' parameter will mark a method under the same declaring body as string set source!
        /// </summary>
        [StringSelector(ListGetter = "GetWeekDays")]
        public string selectedWeekDay = "";

        List<string> GetWeekDays()
        {
            return weekDaySampleSet;
        }
        #endregion

        #region Element Selector

        [Header("Element Selector")]
        public List<ExampleElement> elementSampleSet = new List<ExampleElement>(new ExampleElement[4]);

        /// <summary>
        /// 'ElementSelector' attribute modify target string-type field as an element selector.An extension of string selector.
        /// Works only on string-type fields.
        ///'ListGetter' parameter marks a method under the same declaring body as element set source!
        ///'ReferenceKey' parameter marks a serialized field in element as reference id, the target string will store it's value, This value must stay unique and constant at all time.
        ///'DisplayKey' parameter marks a serialized string field in element as selector display content.
        /// Spliting declaration of 'DisplayKey' and 'ReferenceKey' instead of using same field allows you to modify 'DisplayKey' value at anytime.
        /// </summary>
        [ElementSelector(ListGetter = "GetElementSet", ReferenceKey = "refId", DisplayKey = "displayKey")]
        public string selectedElement = "";

        List<ExampleElement> GetElementSet()
        {
            return elementSampleSet;
        }
        #endregion

    }
}
