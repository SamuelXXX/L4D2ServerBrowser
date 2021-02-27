using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[ScriptedImporter(1,"lua")]
public class LuaAssetImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        //Debug.Log("Import Lua Script:" + ctx.assetPath);
        FileInfo fi = new FileInfo(ctx.assetPath);

        string script = "";
        using (StreamReader sw = fi.OpenText())
        {
            script=sw.ReadToEnd();
            sw.Close();
        }

        //Debug.Log("Lua Script Read:" + script);
        TextAsset luaAsset = new TextAsset(script);

        ctx.AddObjectToAsset("lua asset", luaAsset);
        ctx.SetMainObject(luaAsset);
    }
}
