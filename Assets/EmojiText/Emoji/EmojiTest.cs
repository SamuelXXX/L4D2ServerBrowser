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
        txt.text = "12🐳3🌔4🌕5🌖6🌗7🌘🌗";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
