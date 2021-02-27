using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[CustomEditor(typeof(LuaAssetImporter))]
public class LuaAssetImporterEditor : ScriptedImporterEditor
{
    protected override bool needsApplyRevert =>false;
    public override void OnInspectorGUI()
    {
    }
}
