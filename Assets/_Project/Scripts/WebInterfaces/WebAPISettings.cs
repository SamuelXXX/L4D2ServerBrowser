using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerInspector;
using System.Linq;

[System.Serializable]
public class WebAPIReference
{
    public string refKey;
    public string apiDescriptor;
}


[CreateAssetMenu(fileName = "WebAPISettings", menuName = "WebAPISettings")]
public class WebAPISettings : ScriptableObject
{
    [PowerList(Key = "refKey")]
    public List<WebAPIReference> webAPI = new List<WebAPIReference>();

    public static WebAPIReference GetAPI(string refKey)
    {
        WebAPISettings settings = Resources.Load<WebAPISettings>("WebAPISettings");
        if (settings == null)
            return null;

        return settings.webAPI.Find(p => p.refKey == refKey);
    }

    public static List<string> GetAllAPIKeys()
    {
        WebAPISettings settings = Resources.Load<WebAPISettings>("WebAPISettings");
        if (settings == null)
            return new List<string>();

        return new List<string>(from p in settings.webAPI select p.refKey);

    }
}
