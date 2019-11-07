using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RCUrlSettings", menuName = "RCUrlSettings")]
public class RCUrlSettings : ScriptableObject
{
    public string urlIpList;
    public string urlVipIDList;
    public string urlMotd;
}
