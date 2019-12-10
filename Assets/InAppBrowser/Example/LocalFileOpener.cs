using UnityEngine;
using System.Collections;

public class LocalFileOpener : MonoBehaviour {

	/*
	 * Your local files should be placed in StreamingAssets directory
	 * 
	 * This path is relative to it, meaning full path will be 
	 * /StreamingAssets/LocalSite/index.html
	 * */
	public string pathToFile = "/LocalSite/index.html";

	public string htmlToLoad = "<p>Hello HTML!</p>";

	public void OnButtonClicked() {
		InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
		options.displayURLAsPageTitle = false;
		options.pageTitle = "Local File Example";
		InAppBrowser.OpenLocalFile(pathToFile, options);
	}

	public void OnHTMLClicked() {
		InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
		options.displayURLAsPageTitle = false;
		options.pageTitle = "HTML Example";
		InAppBrowser.LoadHTML(htmlToLoad, options);
	}
}
