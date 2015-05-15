﻿#define ShowHttpLog
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void TBooleanWWWObj(bool ok, WWW www);

public static class URLConst {
	public const string AppStore = "https://itunes.apple.com/tw/app/lan-qiu-hei-bang/id959833713?l=zh&ls=1&mt=8";
	public const string GooglePlay = "https://play.google.com/store/apps/details?id=com.nicemarket.nbaa";
	public const string NiceMarketApk = "http://nicemarket.com.tw/assets/apk/BaskClub.apk";

	public const string Version = "version";
	public const string GetVersion = "getversion";
	public const string CheckSession = "checksession";
	public const string DeviceLogin = "devicelogin";
	public const string Signup = "signup";
	public const string Login = "login";
	public const string FBLogin = "fblogin";
	public const string LinkFB = "linkfb";
	public const string CreateRole = "createrole";
	public const string TeamName = "teamname";
	public const string Conference = "conference";
	public const string PVPAward = "pvpaward";
	public const string IsSeePVPList = "isseepvplist";
	public const string Revenge = "revenge";
	public const string PVPStart = "pvpstart";
	public const string PVPEnd = "pvpend";
	public const string StageStart = "pvestart";
	public const string StageEnd = "pveend";
	public const string Tutorial = "tutorial";
	public const string PlayerKind = "playerkind";
	public const string AutoPower = "autopower";
	public const string Rank = "rank";
	public const string PVPRank = "pvprank";
	public const string MyPVPRank = "mypvprank";
	public const string MatchRank = "matchrank";
	public const string MatchAward = "matchaward";
	public const string MatchExchange = "matchexchange";
	public const string FBGift = "fbgift";
	public const string DayGift = "daygift";
	public const string ChangeLogo = "changelogo";
	public const string MaxLogo = "maxlogo";
	public const string FBRank = "fbrank";
	public const string UseItem = "useitem";
	public const string BuyMoney = "buymoney";
	public const string BuyDiamond = "buydiamond";
	public const string Polling = "polling";
	public const string ItemUp = "itemup";
	public const string SellItem = "sellitem";
	public const string fullPower = "fullPower";
	public const string BuyLuckBox = "buyluckbox";
	public const string BuyStoreItem = "buystoreitem";
	public const string PlayerEvo = "playerevo";
	public const string StageAward = "stageAward";
	public const string BingoItem = "bingoitem";
	public const string RecordGameStart = "recordgamestart";
	public const string RecordGameEnd = "recordgameend";
	public const string RecordGameRank = "recordgamerank";
	public const string CrusadeData = "crusadedata";
	public const string CrusadeStart = "crusadestart";
	public const string CrusadeEnd = "crusadeend";
	public const string ResetCrusade = "resetcrusade";
	public const string CrusadeAward = "crusadeaward";
	public const string SkillUpgrade = "skillupgrade";
	public const string ChangeSkillEffect = "changeskilleffect";
	public const string MissionAward = "missionaward";
	public const string CheckResetToday = "checkresettoday";
	public const string GetGymData = "getgymdata";
	public const string SearchGym = "searchgym";
	public const string GymStart = "gymstart";
	public const string GymEnd = "gymend";
	public const string GymDef = "gymdef";
	public const string GymAward = "gymaward";
	public const string PVEQuickFinish = "pvequickfinish";
	public const string ResetPVE = "resetpve";
	public const string RecordAward = "recordaward";
	public const string LiveTime = "livetime";
	public const string SaveTalk = "savetalk";
	public const string OpenLogoBox = "openlogobox";
	public const string BuildUpgrade = "buildupgrade";
	public const string QuickTrainPlayer = "quicktrainplayer";
	public const string ChangeItemEffect = "changeitemeffect";
	public const string PlayoffPair = "playoffpair";
	public const string PlayoffStart = "playoffstart";
	public const string PlayoffEnd = "playoffend";
	public const string PlayoffReset = "playoffreset";
	public const string PlayoffExchange = "playoffexchange";
	public const string TeamStarting = "teamstarting";
}

public class SendHttp : KnightSingleton<SendHttp>
{
	public Dictionary<string, string> CookieHeaders = new Dictionary<string, string>();

	protected override void Init() {
		DontDestroyOnLoad(gameObject);
	}

	public void Command(string url, TBooleanWWWObj callback, WWWForm form = null, bool waiting = true){
		if (CheckNetwork()){
			url = FileManager.URL + url;
			WWW www = null;

			if (form == null){
				//http get
				www = new WWW(url);
			}else { 
				//http post
				if (form == null)
					form = new WWWForm();
				
				if (!string.IsNullOrEmpty(GameData.Team.sessionID)) 
					form.AddField("sessionID", GameData.Team.sessionID);

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
	
	private IEnumerator WaitForRequest(WWW www,TBooleanWWWObj BoolWWWObj) {
		yield return www;

		if (BoolWWWObj != null) {
			if(checkResponse(www))
				BoolWWWObj(true, www);
			else
				BoolWWWObj(false, www);
		}

		www.Dispose();
	}

	public bool CheckNetwork(){
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
		
		//if (!internetPossiblyAvailable)
		//	UIMessage.Get.ShowMessage("", TextConst.S (93));
		
		return internetPossiblyAvailable;
	}

	private bool checkResponse(WWW www){
		if (string.IsNullOrEmpty(www.error)){
			if (www.text.Contains("{err:")){
				string e = www.text.Substring(6, www.text.Length - 7);
				#if ShowHttpLog
				Debug.LogError("Receive from URL and Error:" + e);
				#endif
			} else {
				#if ShowHttpLog
				Debug.Log("Receive from URL and Success:" + www.text);
				#endif
				return true;
			}
		}else {
			#if ShowHttpLog
			UIHint.Get.ShowHint("Server error : " + www.error, Color.red);
			Debug.LogError("Server error : " + www.error);
			#endif

			if (www.error == "couldn't connect to host")
				UIMessage.Get.ShowMessage(TextConst.S(38), TextConst.S(7));
			else 
			if ( www.error.Contains("java")|| 
			     www.error.Contains("parse")||
			     www.error.Contains("key") || 
			     www.error.Contains("host") || 
			     www.error.Contains("time out") ||
			     www.error.Contains("request")|| 
			     www.error.Contains("connect") ||
			     www.error.Contains("Connection") ||
			     www.error == "Empty reply from server"){

			} else 
			if (www.error.Contains("404 Not Found")){

			} 
		}
		
		return false;
	}
}
