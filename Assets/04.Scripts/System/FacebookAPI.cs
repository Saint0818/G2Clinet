using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FacebookAPI : MonoBehaviour {
	public static FacebookAPI Get; 
	public const string FBAppID = "405934796200816";
	private const string FBScope = "email,publish_actions";
	public bool isInitFB = false;
	private int fbPicSize = 128;
	public string SoccerID = "668238639891907";
	public string CompanyID = "217816971736371";
	public string GangID = "376036032527194";
	public static Dictionary<string, int> LikeData = new Dictionary<string, int>();

	public static void Init(){
		GameObject gobj = new GameObject(typeof(FacebookAPI).Name);
		DontDestroyOnLoad(gobj);
		Get = gobj.AddComponent<FacebookAPI>();
	}

	public void ConnectFB (){
		FB.Init (OnFBSDKConnected, OnHideUnity);
	}

	public void OnFBSDKConnected ()
	{
		if (!string.IsNullOrEmpty(FB.UserId)) {
			FB.API ("/me", Facebook.HttpMethod.GET, OnFBSDKProfile);
		} else {
			if (!FB.IsLoggedIn)
				OnFBSDKLogin ();
			else 
				GetFacebookFriend();
		}
		
		isInitFB = true;
	}

	public void FBLogout() {
		if (isInitFB) {
			isInitFB = false;
			FB.Logout();
		}
	}
	
	public void OnFBSDKLogin (){
		FB.Login(FBScope, LoginCallback);
	}

	private void LoginCallback (FBResult result){
		if (result.Error != null) {
			UIHint.Get.ShowHint (string.Format ("Facebook login fail : {)} , please login again.", result.Error), Color.red);
		} else if (!FB.IsLoggedIn) {
			UIHint.Get.ShowHint ("Login cancelled by player, please login again.", Color.red);
		} else {
			FB.API ("/me", Facebook.HttpMethod.GET, OnFBSDKProfile);
		}
	}

	public void GetUserImage(string userid, FBImageLoader.OnImageReady onImageReady){
		FBImageLoader.Get.RequestProfileImage (userid, fbPicSize, fbPicSize, onImageReady);
	}

	public void GetFacebookFriend(){
		FB.API ( "/me/friends", Facebook.HttpMethod.GET, OnFBSDKFriends );
	}

	public void OnFBSDKFriends (FBResult result)
	{
//		UIRank.Get.HandleFBFriends (result.Text);
	}

	private void OnFBSDKProfile (FBResult response)
	{
//		UIRank.Get.HandleFBProfile(response.Text);
	}

	private void OnHideUnity (bool isGameShown)
	{
		
	}

	public void GetUserLike(){
		if(FB.AccessToken != ""){
			FB.API ( "/me/likes", Facebook.HttpMethod.GET, OnFBSDKLikes);
		}
	}

	private void OnFBSDKLikes(FBResult result){
		object o = FBJsonSerializer.Decode (result.Text);
		Hashtable hashtable = o as Hashtable;
		
		if (hashtable != null) {
			LikeData.Clear();
			ArrayList friendsData = hashtable ["data"] as ArrayList;
			
			foreach (object friendData in friendsData) {
				Hashtable h = friendData as Hashtable;
				string id = h ["id"] as string;

				if(!LikeData.ContainsKey(id))
					LikeData.Add(id, 1);			
			}
		}
	}
}
