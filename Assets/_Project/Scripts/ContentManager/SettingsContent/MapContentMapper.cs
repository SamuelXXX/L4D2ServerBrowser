using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerInspector;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "MapInfo", menuName = "MapInfo")]
public class MapContentMapper : ScriptableObject
{
    public static MapContentMapper OfficialInstance
    {
        get
        {
            return Resources.Load<MapContentMapper>("MapInfo");
        }
    }

    public static MapContentMapper ThirdPartyInstance
    {
        get;
        set;
    }

    [System.Serializable]
    public class MapInfoItem
    {
        public string mapCNName;
        public Sprite mapPosterImage;
        [SerializeField]
        protected string mapCheckExp;

        protected Regex pattern = null;

        public bool CheckMapExpMatch(string mapIndex)
        {
            if (pattern == null)
                pattern = new Regex(mapCheckExp);
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
