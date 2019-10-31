using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerInspector;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "MapInfo", menuName = "MapInfo")]
public class MapContentMapper : ScriptableObject
{
    public static MapContentMapper Instance
    {
        get
        {
            return Resources.Load<MapContentMapper>("MapInfo");
        }
    }

    [System.Serializable]
    public class MapInfoItem
    {
        public string mapCNName;
        public Sprite mapPosterImage;
        [SerializeField]
        protected string mapCheckExp;

        public bool CheckMapExpMatch(string mapIndex)
        {
            Regex pattern = new Regex(mapCheckExp);
            return pattern.IsMatch(mapIndex);
        }
    }

    [SerializeField]
    protected MapInfoItem thirdPartyInfo = new MapInfoItem();

    [SerializeField, PowerList(Key = "mapCNName")]
    protected List<MapInfoItem> mapInfos = new List<MapInfoItem>();

    public MapInfoItem QueryByMapIndex(string mapIndex)
    {
        var ret = mapInfos.Find(p => p.CheckMapExpMatch(mapIndex));
        if (ret == null)
            ret = thirdPartyInfo;

        return ret;
    }


}
