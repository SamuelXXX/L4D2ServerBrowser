using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text txt = GetComponent<EmojiText>();
        txt.text = "<color=red>12🐳3🌔4🌕5🌖6🌗7🌘🌗</color><color=blue>[USERNAME]</color><color=red> (一号陪练🐳)</color>";
    }

    // Update is called once per frame
    void Update()
    {
        Text txt = GetComponent<EmojiText>();
        txt.text = "<color=red>6🌗7🌘🐳🌗🌗一号陪练dff</color>";
        //txt.text = "12  3<color=blue>[USERNAME]</color><color=red>( )</color>";
    }
}
