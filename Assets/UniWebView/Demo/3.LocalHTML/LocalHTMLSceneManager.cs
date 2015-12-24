using UnityEngine;
using System.Collections;

public class LocalHTMLSceneManager : MonoBehaviour {
	public string fileName;
	public string htmlText;
	#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
	public void LoadFromFile() {

		var webView = CreateWebView();

		#if UNITY_EDITOR
		webView.url = Application.streamingAssetsPath + fileName;
		#elif UNITY_IOS
		webView.url = Application.streamingAssetsPath + fileName;
		#elif UNITY_ANDROID
		webView.url = "file:///android_asset" + fileName;
		#elif UNITY_WP8
		webView.url = "Data/StreamingAssets" + fileName;
		#endif
		webView.Load();
		webView.Show();
	}
	
	public void LoadFromText() {
		var webView = CreateWebView();

		webView.LoadHTMLString(htmlText, null);
		webView.Show();
	}

	UniWebView CreateWebView() {
		var webViewGameObject = GameObject.Find("WebView");
		if (webViewGameObject == null) {
			webViewGameObject = new GameObject("WebView");
		}

		var webView = webViewGameObject.AddComponent<UniWebView>();

		webView.toolBarShow = true;
		return webView;
	}

	#else //End of #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
	void Start() {
		Debug.LogWarning("UniWebView only works on iOS/Android/WP8. Please switch to these platforms in Build Settings.");
	}
	#endif
}
