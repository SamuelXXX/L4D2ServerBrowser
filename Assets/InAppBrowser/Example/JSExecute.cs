using UnityEngine;
using System.Collections;

public class JSExecute : MonoBehaviour {

	public string javaScriptCode = "alert('pong!')";

	void Start() {
		InAppBrowserBridge bridge = FindObjectOfType<InAppBrowserBridge>();
		bridge.onJSCallback.AddListener(OnMessageFromJS);
	}

	void OnMessageFromJS(string jsMessage) {
		if (jsMessage.Equals("ping")) {
			Debug.Log("Ping message received!");
			InAppBrowser.ExecuteJS(javaScriptCode);
		}
	}
		
}
