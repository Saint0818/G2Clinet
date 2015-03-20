#define ShowHttpLog
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SendHttp : MonoBehaviour
{
	public static SendHttp Get;
	public static string sessionID = "";
	public static Dictionary<string, string> CookieHeaders = new Dictionary<string, string>();

	public static void Init(){
		GameObject gobj = new GameObject(typeof(SendHttp).Name);
		DontDestroyOnLoad(gobj);
		Get = gobj.AddComponent<SendHttp>();
	}

	public void Command(string url, TBooleanWWWObj callback, WWWForm form = null, bool waiting = true){
		if (checkNetwork()){
			WWW www = null;

			if (form == null){
				//http get
				www = new WWW(url);
			}else { 
				//http post
				if (form == null)
					form = new WWWForm();
				
				if (!string.IsNullOrEmpty(sessionID)) 
					form.AddField("sessionID", sessionID);

				if(CookieHeaders.Count == 0)
					www = new WWW(url, form.data);
				else
					www = new WWW(url, form.data, CookieHeaders);
			}
			
			StartCoroutine(WaitForRequest(www, callback));

			#if ShowHttpLog
			Debug.Log("Send To Server:" + url);
			#endif
		}
	}
	
	private IEnumerator WaitForRequest(WWW www,TBooleanWWWObj BoolWWWObj){
		yield return www;

		if(checkResponse(www))
			BoolWWWObj(true, www);
		else
			BoolWWWObj(false, www);	

		www.Dispose();
	}

	private bool checkNetwork(){
		#if UNITY_EDITOR
		if (Network.player.ipAddress != "127.0.0.1" && Network.player.ipAddress != "0.0.0.0")
			return true;
		#else
		#if UNITY_IPHONE
		if (iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
			return true;
		#endif
		#if UNITY_ANDROID
//		if (iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
//			return true;
		#endif
		#if (!UNITY_IPHONE && !UNITY_ANDROID)
		if (Network.player.ipAddress != "127.0.0.1" && Network.player.ipAddress != "0.0.0.0")
			return true;
		#endif
		#endif
		
		UIMessage.Get.ShowMessage(TextConst.S(37), TextConst.S(93));
		return false;
	}

	private bool checkResponse(WWW www){
		if (string.IsNullOrEmpty(www.error)){
			if (www.text.Contains("{err:")){
				string e = www.text.Substring(6, www.text.Length - 7);
				#if ShowHttpLog
				Debug.LogError("Receive from URL and Error:" + e);
				#endif
			} else{
				#if ShowHttpLog
				Debug.Log("Receive from URL and Success:" + www.text);
				#endif
				return true;
			}
		}else{
			#if ShowHttpLog
			Debug.LogError("Receive from URL and Error:" + www.error);
			#endif

			if (www.error == "couldn't connect to host")
				UIMessage.Get.ShowMessage(TextConst.S(38), TextConst.S(7));
			else if (www.error.Contains("java")|| 
				     www.error.Contains("parse")||
				     www.error.Contains("key") || 
				     www.error.Contains("host") || 
				     www.error.Contains("time out") ||
				     www.error.Contains("request")|| 
				     www.error.Contains("connect") ||
				     www.error.Contains("Connection") ||
				     www.error == "Empty reply from server"){

			} else if (www.error.Contains("404 Not Found")){

			} 
		}
		
		return false;
	}
}
