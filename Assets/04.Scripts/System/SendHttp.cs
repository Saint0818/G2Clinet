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
		bool internetPossiblyAvailable;
		switch (Application.internetReachability)
		{
		case NetworkReachability.ReachableViaLocalAreaNetwork:
			internetPossiblyAvailable = true;
			break;
		case NetworkReachability.ReachableViaCarrierDataNetwork:
			internetPossiblyAvailable = true;
			break;
		default:
			internetPossiblyAvailable = false;
			break;
		}
		
		if (!internetPossiblyAvailable)
			UIMessage.Get.ShowMessage("", TextConst.S (93));
		
		return internetPossiblyAvailable;
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
