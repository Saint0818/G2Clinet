﻿//#define ShowHttpLog

using System;
using System.Collections;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public delegate void BoolCallback(bool flag);

public struct TSessionResult {
	public string sessionID;
}

public struct TDailyRecordResult {
    public TDailyRecord DailyRecord;
    public TDailyRecord WeeklyRecord;
    public TDailyRecord MonthlyRecord;
}

public struct TSkillCardBag {
	public int Diamond;
	public int SkillCardBagCount;
}

public delegate void TBooleanWWWObj(bool ok, WWW www);

public static class URLConst {
	//public const string AppStore = "https://itunes.apple.com/tw/app/lan-qiu-hei-bang/id959833713?l=zh&ls=1&mt=8";
    private const string GooglePlay = "https://play.google.com/store/apps/details?id=com.nicemarket.g2candan";
    private const string NiceMarketApk = "http://nicemarket.com.tw/assets/apk/g2.apk";
    private const string PubGameApk = "http://nicemarket.com.tw/assets/apk/g2pubgame.apk";
    private const string CanadaApk = "http://nicemarket.com.tw/assets/apk/g2canada.apk";

    public static string DownLoadURL {
        get {
            if (GameData.Company == ECompany.NiceMarket)
                return GooglePlay;
            else
            if (GameData.Company == ECompany.Canada)
                return CanadaApk;
            else
            if (GameData.Company == ECompany.PubGame)
                return PubGameApk;
            else
                return NiceMarketApk;
        }
    }

	public const string Version = "version";
	public const string CheckSession = "checksession";
	public const string DeviceLogin = "devicelogin";
	public const string LookPlayerBank = "lookplayerbank";
	public const string CreateRole = "createrole";
	public const string AddMaxPlayerBank = "addmaxplayerbank";
	public const string SelectRole = "selectrole";
	public const string DeleteRole = "deleterole";
	public const string AddTutorialFlag = "addtutorialflag";
	public const string ClearTutorialFlag = "cleartutorialflag";
    public const string ClearMission = "clearmission";
	public const string EquipsSkillCard = "equipskillcard";
	public const string ChangeSkillPage = "changeskillpage";
	public const string AddSuitCardExecute = "addsuitcardexecute";
	public const string RemoveSuitCardExecute = "removesuitcardexecute";
	public const string SellSkillcard = "sellskillcard";
	public const string ReinforceSkillcard = "reinforceskillcard";
	public const string EvolutionSkillcard = "evolutionskillcard";
	public const string BuySkillcardBag = "buyskillcardbag";
	public const string PickLottery = "picklottery";
	public const string ScenePlayer = "sceneplayer";
	public const string ChangePlayerName = "changeplayername";
	public const string StageStart = "stagestart";
	public const string StageWin = "stagewin";
	public const string StageRewardAgain = "stagerewardagain";
	public const string ResetStageDailyChallenge = "resetstagedailychallenge";
	public const string AddStageTutorial = "addstagetutorial";
	public const string GameRecord = "gamerecord";
    public const string SyncDailyRecord = "syncdailyrecord";
	public const string BuyAvatarItem = "buyavataritem";
    public const string MissionFinish = "missionfinish";
    public const string SearchFriend = "searchfriend";
    public const string LookFriends = "lookfriends";
    public const string FreshFriends = "freshfriends";
    public const string MakeFriend = "makefriend";
    public const string ConfirmMakeFriend = "confirmmakefriend";
    public const string RemoveFriend = "removefriend";
    public const string LookSocialEvent = "looksocialevent";
    public const string LookWatchFriend = "lookwatchfriend";
	public const string LookAvatar = "lookavatar";
    public const string Good = "good";
	public const string BuyDiamond = "buydiamond";
	public const string BuyFromShop = "buyfromshop";
    public const string RefreshShop = "refreshshop";
    public const string BuyMyShop = "buymyshop";
    public const string ChangeStrategy = "changestrategy";
	public const string LinkFB = "linkfb";
	public const string Conference = "conference";
	public const string IsSeePVPList = "isseepvplist";
	public const string Revenge = "revenge";
	public const string Tutorial = "tutorial";
	public const string PlayerKind = "playerkind";
	public const string AutoPower = "autopower";
	public const string Rank = "rank";

    //pvp
    public const string PVPStart = "pvpstart";
    public const string PVPEnd = "pvpend";
    public const string PVPRank = "pvprank";
    public const string PVPGetEnemy = "pvpgetenemy";
    public const string PVPMyRank = "pvpmyrank";
    public const string PVPAward = "pvpaward";
    public const string PVPBuyTicket = "pvpbuyticket";

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
	public const string Polling = "polling";
	public const string ItemUp = "itemup";
	public const string SellItem = "sellitem";
	public const string FullPower = "fullPower";
	public const string BuyLuckBox = "buyluckbox";
	public const string BuyStoreItem = "buystoreitem";
	public const string PlayerEvo = "playerevo";
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
	public const string CheckResetStage = "checkresetstage";
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
	public const string GMAddItem = "gmadditem";
	public const string GMRemoveItem = "gmremoveitem";
	public const string GMAddMoney = "gmaddmoney";
	public const string GMAddDiamond = "gmadddiamond";
	public const string GMAddPower = "gmaddpower";
    public const string GMAddExp = "gmaddexp";
    public const string GMSetLv = "gmsetlv";
    public const string GMSetMaxPlayerBank = "gmsetmaxplayerbank";
    public const string GMAddAvatarPotential = "gmaddavatarpotential";
    public const string GMSavePotential = "gmsavepotential";
    public const string GMSetNextMainStageID = "gmsetnextmainstageid";
    public const string GMResetStage = "gmresetstage";
    public const string GMResetDailyLoginNums = "gmresetdailyloginnums";
    public const string GMResetReceivedDailyLoginNums = "gmresetreceiveddailyloginnums";
    public const string GMSetDailyLoginNums = "gmsetdailyloginnums";
    public const string GMSetLifetimeLoginNum = "gmsetlifetimeloginnum";
    public const string GMResetLifetimeReceivedLoginNum = "gmresetlifetimereceivedloginnum";
    public const string GMResetNextInstanceIDs = "gmresetnextinstanceids";
    public const string GMSetNextInstanceID = "gmsetnextinstanceid";
	public const string GMChangeAvatarUseKind = "gmchangeavatarusekind";
	public const string GMSetStageDailyChallengeNum = "gmsetstagedailychallengenum";
    public const string ChangeAvatar = "changeavatar";
	public const string Potential = "potential";
	public const string ValueItemExchange = "valueitemexchange";
	public const string ValueItemAddInlay = "valueitemaddinlay";
	public const string ValueItemUpgrade = "valueitemupgrade";
    public const string ChangeHeadTexture = "changeheadtexture";
    public const string ChangeLanguage = "changelanguage";
    public const string RequireComputeTeamPower = "requirecomputeteampower";
    public const string ReceivedDailyLoginReward = "receivedailyloginreward";
    public const string ReceivedLifetimeLoginReward = "receivelifetimeloginreward";
}

public class SendHttp : KnightSingleton<SendHttp> {
	public JsonSerializerSettings JsonSetting = new JsonSerializerSettings();
	private Dictionary<string, string> cookieHeaders = new Dictionary<string, string>();
	private bool versionChecked = false;
	private int focusCount = 0;

	private string waitingURL;
	private TBooleanWWWObj waitingCallback = null;
	private WWWForm waitingForm = null;
    private Queue<string> confirmFriends = new Queue<string>();

    private EventDelegate.Callback LookFriendsEvent;
    private BoolCallback FreshFriendsEvent;
	private EventDelegate.Callback MakeFriendEvent;
	private EventDelegate.Callback BuySkillCardBagEvent;

	protected override void Init() {
		JsonSetting.NullValueHandling = NullValueHandling.Ignore;
		DontDestroyOnLoad(gameObject);
	}

	void OnApplicationFocus(bool focusStatus) {
		if (!focusStatus) {

		} else {
			focusCount++;
			if (focusCount > 1 && CheckNetwork(true)) {
                if (needResendLogin()) {
					checkVersion ();
				} else
				if (GameData.Team.Player.Lv > 0) {

					//if (DateTime.UtcNow.Day != GameData.Team.LoginTime.ToUniversalTime().Day)
						//Command(URLConst.CheckResetToday, waitResetToday);
				}
			}

            if (GameData.Company == ECompany.NiceMarket)
                StartCoroutine(hidePGTool());
		}
	}

	public void Command(string url, TBooleanWWWObj callback, WWWForm form = null, bool waiting = true){
		if (!LobbyStart.Get.ConnectToServer)
			return;
        
		waitingURL = url;
		waitingCallback = callback;
		waitingForm = form;

		if (CheckNetwork(true)){
			url = FileManager.URL + url;
			WWW www = null;

			//http post
			if (form == null)
				form = new WWWForm();
			
			if (!string.IsNullOrEmpty(GameData.Team.sessionID)) 
				form.AddField("sessionID", GameData.Team.sessionID);

			if(cookieHeaders.Count == 0)
				www = new WWW(url, form.data);
			else
				www = new WWW(url, form.data, cookieHeaders);

			StartCoroutine(WaitForRequest(www, callback));

			if (waiting) 
				UIWaitingHttp.Get.SaveProtocol(url, callback, form);
		}
	}
	
	private IEnumerator WaitForRequest(WWW www,TBooleanWWWObj callback) {
		yield return www;
		
		bool flag = false;
		if (checkResponse(www)) {
			#if ShowHttpLog
			Debug.Log("Rec From Server:" + www.text);
			#endif
			
			if (UIWaitingHttp.Visible) { 
				if (www.url == URLConst.Polling || www.url == URLConst.AutoPower) {
					
				} else
					UIWaitingHttp.UIShow(false);
			}
			
			flag = true;
		}
		
		try {
			if (callback != null)
				callback(flag, www);
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
		
		www.Dispose();
	}

	public bool CheckNetwork(bool showWarning){
		bool internetPossiblyAvailable = false;
		
		#if UNITY_EDITOR
		if (Network.player.ipAddress != "127.0.0.1" && Network.player.ipAddress != "0.0.0.0")
			internetPossiblyAvailable = true;
		#else
		if (Application.internetReachability != NetworkReachability.NotReachable)
			internetPossiblyAvailable = true;
		#endif

		if (showWarning && !internetPossiblyAvailable && !UIMessage.Visible) {
			UIWaitingHttp.Get.SaveProtocol(waitingURL, waitingCallback, waitingForm);
			UIMessage.Get.ShowMessage(TextConst.S(505), TextConst.S(506), ResentCommond);
		}
		
		return internetPossiblyAvailable;
	}

	public void ResentCommond() {
		UIWaitingHttp.UIShow(true);
		UIWaitingHttp.Get.ShowResend(TextConst.S(506));
	}

    private bool needResendLogin() {
        if (string.IsNullOrEmpty(GameData.Team.Identifier) && !UIUpdateVersion.Visible && 
            !UIPubgame.Visible && !UIMessage.Visible)
            return true;
        else
            return false;
    }

	private bool checkResponse(WWW www){
		if (string.IsNullOrEmpty(www.error)) {
			if (www.text.Contains("{err:")) {
				string e = www.text.Substring(6, www.text.Length - 7);
                #if ShowHttpLog
				Debug.Log(www.url);
				Debug.Log(e);
                #endif
				
                if (needResendLogin()) {
					if (!versionChecked)
						checkVersion();
					else
						SendLogin();
					
					return false;
				}
				
				if (e == "Please login first.") {
					if (UIWaitingHttp.Visible) {
						WWWForm form = new WWWForm();
						form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
						Command(URLConst.CheckSession, waitCheckSession, form);
						if (UIWaitingHttp.Visible)
							UIWaitingHttp.Get.WaitForCheckSession();
					}
				} else
					if (e == "Team not Found." ||
					    e == "You have not created." ||
					   e.Contains("connect")) {
					if (UIWaitingHttp.Visible)
						UIWaitingHttp.Get.ShowResend(TextConst.S (508));
				} else
				if (e == "Fight end." || e == "Receipt error.") {
					UIWaitingHttp.UIShow(false);
				} else {
					UIWaitingHttp.UIShow(false);
				}
			} else
				return true;
		} else
		{
            #if ShowHttpLog
			Debug.Log(www.url + " : " + www.error);
            #endif
            if (www.error.Contains("Failed to connect to") ||
                www.error.Contains("couldn't connect to host") || 
                www.error.Contains("Couldn't resolve host")) {
				UIWaitingHttp.UIShow(false);
                UIMessage.Get.ShowMessage(TextConst.S(503), TextConst.S(504), checkVersion, openNotic);
			} else
            if (needResendLogin()) {
				if (!versionChecked)
					checkVersion();
				else
					SendLogin();
			} else
			if (www.error.Contains("java")|| 
			    www.error.Contains("parse")||
			    www.error.Contains("key") || 
			    www.error.Contains("host") || 
			    www.error.Contains("time out") ||
			    www.error.Contains("request")|| 
			    www.error.Contains("connect") ||
			    www.error.Contains("Connection") ||
			    www.error == "Empty reply from server")
			{
				if (UIWaitingHttp.Visible)
					UIWaitingHttp.Get.ShowResend(TextConst.S (508));
			} else 
			if (www.error.Contains("404 Not Found")){
				UIWaitingHttp.UIShow(false);
			} else {
				//UIMessage.Get.ShowMessage(TextConst.S(36), www.error);
			}
		}
		
		return false;
	}

    private void addLoginInfo(ref WWWForm form, string openID="") {
        form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
		form.AddField("Language", GameData.Setting.Language.GetHashCode());
		form.AddField("OS", GameData.OS);
		form.AddField("Company", GameData.Company);
		form.AddField("Version", BundleVersion.Version.ToString());
        if (!string.IsNullOrEmpty(openID))
            form.AddField("OpenID", openID);
	}

	private void waitResetToday(bool Value, WWW www) {
		if(Value){
			if(string.Empty != www.text){
				TTeam result = JsonConvert.DeserializeObject<TTeam> (www.text, JsonSetting);		
				GameData.Team.LoginTime = result.LoginTime;
			}
		}
	}

	private void waitCheckSession(bool ok, WWW www) {
		if (ok) {
			TSessionResult result = (TSessionResult)JsonConvert.DeserializeObject(www.text, (typeof(TSessionResult)));
			GameData.Team.sessionID = result.sessionID;
			UIMessage.Get.ShowMessage(TextConst.S(505), TextConst.S(507));
			
			if (www.responseHeaders.ContainsKey("SET-COOKIE")) {
				cookieHeaders.Clear();
				cookieHeaders.Add("COOKIE", www.responseHeaders ["SET-COOKIE"]);
			}
		}
	}

	public void checkVersion() {
		WWWForm form = new WWWForm();
		addLoginInfo(ref form);
		Command(URLConst.Version, waitVersion, form);
	}

	public void CheckServerData(bool connectToServer)
	{
		if (connectToServer)
			checkVersion();
		else 
			SceneMgr.Get.ChangeLevel(ESceneName.SelectRole);
	}

    public bool CheckServerMessage(string text) {
        if (!string.IsNullOrEmpty(text)) {
            if (text.Length <= 6) {
                int index = -1;
                if (int.TryParse(text, out index))
                    UIHint.Get.ShowHint(TextConst.S(index), Color.red);
            } else
                return true;
        }

        return false;
    }

    private IEnumerator hidePGTool() {
        yield return new WaitForSeconds(2);

        Pubgame.PubgameSdk.Get.SetPgToolsActive(false);
    } 
	
	private void waitVersion(bool ok, WWW www) {
		if (ok) {
			versionChecked = true;
            if (float.TryParse(www.text, out GameData.ServerVersion) && BundleVersion.Version >= GameData.ServerVersion) {
                if (GameData.Company == ECompany.PubGame) {
                    UIPubgame.Visible = true;
                    UIPubgame.Get.LoginHandle = waitPubgameLogin;
                    UILoading.UIShow(false);
                } else {
                    SendLogin();
                    //for pubgame login record
                    if (GameData.Company == ECompany.NiceMarket)
                        Pubgame.PubgameSdk.Get.InitSDK();
                }
            } else {
				UILoading.UIShow(false);
				UIUpdateVersion.UIShow(true);
			}
		}
	}

    private void openNotic() {
        UINotic.Visible = true;
    }
	
    private void SendLogin(string openID="") {
		GameData.Team.Identifier = "";
		WWWForm form = new WWWForm();
        addLoginInfo(ref form, openID);
		Command(URLConst.DeviceLogin, waitDeviceLogin, form);
	}
	
    private void waitDeviceLogin(bool ok, WWW www) {
		if (ok) {
			try {
				string text = GSocket.Get.OnHttpText(www.text);
				GameData.Team = JsonConvert.DeserializeObject <TTeam>(text, JsonSetting); 
				GameData.Team.Init();

				if (www.responseHeaders.ContainsKey("SET-COOKIE")){
					SendHttp.Get.cookieHeaders.Clear();
					SendHttp.Get.cookieHeaders.Add("COOKIE", www.responseHeaders ["SET-COOKIE"]);
				}

                if (GameData.Team.Friends == null)
                    GameData.Team.Friends = new Dictionary<string, TFriend>();

                if (GameData.Team.StageTutorial == 0)
                    GameData.Team.StageTutorial = 3;

				GameData.StageID = GameData.Team.StageTutorial + 1;
				if (GameData.Team.Player.Lv == 0 && StageTable.Ins.HasByID(GameData.StageID)) {
					int courtNo = StageTable.Ins.GetByID(GameData.StageID).CourtNo;
					SceneMgr.Get.CurrentScene = "";
					SceneMgr.Get.ChangeLevel (courtNo);
				} else {
                    SyncDailyRecord();
                    LookFriends(null, GameData.Team.Identifier, false);
                    StartCoroutine(longPollingSocialEvent(0));
                    StartCoroutine(longPollingWatchFriends(0));

					//UILoading.OpenUI = UILoading.OpenNotic;
                    UILoading.UIShow(true, ELoading.Lobby);
				}

                if (GameData.Company == ECompany.NiceMarket)
                    Pubgame.PubgameSdk.Get.SetPgToolsActive(false);
			} catch (Exception e) {
				Debug.Log(e.ToString());
			}
		} else
			UIHint.Get.ShowHint("Login fail.", Color.red);
	}

    public void SyncDailyRecord() {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", GameData.Team.Identifier);
        Command(URLConst.SyncDailyRecord, waitSyncDailyRecord, form, false);
    }

    private void waitSyncDailyRecord(bool ok, WWW www) {
        if (ok) {
            TDailyRecordResult result = JsonConvert.DeserializeObject<TDailyRecordResult>(www.text, JsonSetting);
            GameData.Team.DailyRecord = result.DailyRecord;
            GameData.Team.WeeklyRecord = result.WeeklyRecord;
            GameData.Team.MonthlyRecord = result.MonthlyRecord;

            GameData.Team.NeedForSyncRecord = false;
            GameData.Team.Player.NeedForSyncRecord = false;
        }
    }

    public void LookFriends(EventDelegate.Callback e, string id, bool waiting) {
        LookFriendsEvent = e;
        WWWForm form = new WWWForm();
        form.AddField("Identifier", id);
        SendHttp.Get.Command(URLConst.LookFriends, waitLookFriends, form, waiting);
    }

    private void waitLookFriends(bool flag, WWW www) {
        if (flag && www.text != "") {
            try {
                string text = GSocket.Get.OnHttpText(www.text);
                if (!string.IsNullOrEmpty(text)) {
                    TTeam team = JsonConvert.DeserializeObject <TTeam>(text, SendHttp.Get.JsonSetting);
                    team.InitFriends();
                    GameData.Team.Friends = team.Friends;
                    GameData.Team.FreshFriendTime = team.FreshFriendTime;
                }

                if (LookFriendsEvent != null)
                    LookFriendsEvent();
            } catch (Exception e) {
                Debug.Log(e.ToString());
            }
        }
    }

    public void FreshFriends(BoolCallback e, bool waiting) {
        FreshFriendsEvent = e;
        WWWForm form = new WWWForm();
        form.AddField("Identifier", GameData.Team.Identifier);
        SendHttp.Get.Command(URLConst.FreshFriends, waitFreshFriends, form, waiting);
    }

    private void waitFreshFriends(bool flag, WWW www) {
        if (flag) {
            if (CheckServerMessage(www.text)) {
                string text = GSocket.Get.OnHttpText(www.text);
                if (!string.IsNullOrEmpty(text)) {
                    TTeam team = JsonConvert.DeserializeObject <TTeam>(text, SendHttp.Get.JsonSetting);
                    team.InitFriends();

                    GameData.Team.FreshFriendTime = team.FreshFriendTime;
                    GameData.Team.DailyCount.FreshFriend = team.DailyCount.FreshFriend;
                    GameData.Team.Friends = team.Friends;
                    GameData.Team.FreshFriendTime = team.FreshFriendTime;
                    if (GameData.Team.Diamond != team.Diamond)
                        GameData.Team.Diamond = team.Diamond;
                }
            }
        }

        if (FreshFriendsEvent != null) {
            FreshFriendsEvent(flag);
            FreshFriendsEvent = null;
        }
    }

    public void MakeFriend(EventDelegate.Callback e, string friendID) {
        MakeFriendEvent = e;
        WWWForm form = new WWWForm();
        form.AddField("Identifier", GameData.Team.Identifier);
        form.AddField("Name", GameData.Team.Player.Name);
        form.AddField("FriendID", friendID);
        SendHttp.Get.Command(URLConst.MakeFriend, waitMakeFriend, form);
    }

    private void waitMakeFriend(bool flag, WWW www) {
        if (flag) {
            if (CheckServerMessage(www.text)) {
                if (GameData.Team.Friends == null)
                    GameData.Team.Friends = new Dictionary<string, TFriend>();
                
                TFriend friend = JsonConvert.DeserializeObject <TFriend>(www.text, SendHttp.Get.JsonSetting);
                friend.PlayerInit();
                if (GameData.Team.Friends.ContainsKey(friend.Identifier))
                    GameData.Team.Friends[friend.Identifier] = friend;
                else
                    GameData.Team.Friends.Add(friend.Identifier, friend);

                if (MakeFriendEvent != null)
                    MakeFriendEvent();
            }
        }
    }

	public void AddSkillCardBag (EventDelegate.Callback e) {
		BuySkillCardBagEvent = e;
		WWWForm form = new WWWForm();
		form.AddField("Identifier", GameData.Team.Identifier);
		SendHttp.Get.Command(URLConst.BuySkillcardBag, waitAddSkillCardBag, form);
	}

	private void waitAddSkillCardBag(bool flag, WWW www) {
		if (flag) {
			TSkillCardBag bag = JsonConvert.DeserializeObject <TSkillCardBag>(www.text, SendHttp.Get.JsonSetting);
			GameData.Team.Diamond = bag.Diamond;
			GameData.Team.SkillCardMax = bag.SkillCardBagCount;

		}
		if (BuySkillCardBagEvent != null)
			BuySkillCardBagEvent();
	}

    private IEnumerator longPollingSocialEvent(int kind) {
        yield return new WaitForSeconds(20); //every 20 seconds request once

        lookSocialEvent(kind);
    }

    private void lookSocialEvent(int kind) {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", GameData.Team.Identifier);

        if (kind > 0)
            form.AddField("Time", GameData.Team.SocialEventTime.ToString());

        SendHttp.Get.Command(URLConst.LookSocialEvent, waitLookSocialEvent, form, false);
    }

    private void waitLookSocialEvent(bool ok, WWW www) {
        GameData.Team.SocialEventTime = DateTime.UtcNow;
        if (ok) {
			bool needSave = false;
            if (!string.IsNullOrEmpty(www.text)) {
                TSocialEvent[] events = JsonConvert.DeserializeObject <TSocialEvent[]>(www.text, SendHttp.Get.JsonSetting);
                for (int i = 0; i < events.Length; i++) {
                    bool flag = false;
                    if (events[i].Identifier == GameData.Team.Identifier) {
                        events[i].Player.Name = GameData.Team.Player.Name;
                        GameData.SocialEvents.Add(events[i]);
                    } else
                    if (!string.IsNullOrEmpty(events[i].Identifier) && GameData.Team.Friends.ContainsKey(events[i].Identifier)) {
                        events[i].Player.Name = GameData.Team.Friends[events[i].Identifier].Player.Name;
                        GameData.SocialEvents.Add(events[i]);
                        flag = true;
                    }

                    if (flag && GameData.Setting.SocialEventTime.CompareTo(events[i].Time.ToUniversalTime()) < 0) {
                        UIHint.Get.ShowHint(events[i].Player.Name + TextConst.GetSocialText(events[i]), Color.blue);
                        if (UIMainLobby.Get.IsVisible)
                            UIMainLobby.Get.Main.SocialNotice = true;

						needSave = true;
                    }
                }

                if (UISocial.Visible)
                    UISocial.Get.FreshSocialEvent(0);

				if (needSave) {
					GameData.Setting.ShowEvent = true;
					PlayerPrefs.SetInt(ESave.ShowEvent.ToString(), 1);

					GameData.Setting.SocialEventTime = DateTime.UtcNow;
					PlayerPrefs.SetString(ESave.SocialEventTime.ToString(), DateTime.UtcNow.ToString());

					PlayerPrefs.Save();
				}
            }
        }

        StartCoroutine(longPollingSocialEvent(1));
    }

    private IEnumerator longPollingWatchFriends(int kind) {
        yield return new WaitForSeconds(23); //every 23 seconds request once

        lookWatchFriends(kind);
    }

    private void lookWatchFriends(int kind) {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", GameData.Team.Identifier);

        if (kind > 0) 
            form.AddField("Time", GameData.Team.WatchFriendsTime.ToString());

        SendHttp.Get.Command(URLConst.LookWatchFriend, waitLookWatchFriends, form, false);
    }

    private void waitLookWatchFriends(bool ok, WWW www) {
        GameData.Team.WatchFriendsTime = DateTime.UtcNow;
        if (ok) {
            if (!string.IsNullOrEmpty(www.text)) {
                TSocialEvent[] events = JsonConvert.DeserializeObject <TSocialEvent[]>(www.text, SendHttp.Get.JsonSetting);

                if (events.Length > 0) {
                    if (GameData.Team.Friends == null)
                        GameData.Team.Friends = new Dictionary<string, TFriend>();
                    
                    bool flag = false;
					bool needSave = false;
                    for (int i = 0; i < events.Length; i++) {
                        if (!string.IsNullOrEmpty(events[i].TargetID)) {
                            if (events[i].Value == EFriendKind.Waiting || events[i].Value == EFriendKind.Ask) {
                                TFriend friend = new TFriend();
                                friend.Identifier = events[i].TargetID;
                                friend.Player.Name = events[i].Name;
                                friend.Time = events[i].Time;
                                friend.Kind = EFriendKind.Ask;//events[i].Value;

                                if (!GameData.Team.Friends.ContainsKey(events[i].TargetID))
                                    GameData.Team.Friends.Add(events[i].TargetID, friend);
                                else
                                    GameData.Team.Friends[events[i].TargetID] = friend;

                                if (GameData.Setting.WatchFriendTime.CompareTo(events[i].Time.ToUniversalTime()) < 0) {
                                    UIHint.Get.ShowHint(friend.Player.Name + TextConst.S(5029), Color.white);
                                    if (UIMainLobby.Get.IsVisible)
                                        UIMainLobby.Get.Main.SocialNotice = true;

									needSave = true;
                                }

                                flag = true;
                            } else 
                            if (events[i].Value == EFriendKind.Friend) {
                                confirmFriends.Enqueue(events[i].TargetID);
                            }
                        }
                    }

					if (needSave) {
						GameData.Setting.ShowWatchFriend = true;
						PlayerPrefs.SetInt(ESave.ShowWatchFriend.ToString(), 1);
						GameData.Setting.WatchFriendTime = DateTime.UtcNow;
						PlayerPrefs.SetString(ESave.WatchFriendTime.ToString(), DateTime.UtcNow.ToString());
						PlayerPrefs.Save();
					}

                    if (confirmFriends.Count > 0)
                        confirmFriend(confirmFriends.Dequeue());

                    if (flag && UISocial.Visible)
                        UISocial.Get.FreshFriend(1);
                }
            }
        }

        StartCoroutine(longPollingWatchFriends(1));
    }

    private void waitConfirm(bool ok, WWW www) {
        if (ok) {
            if (SendHttp.Get.CheckServerMessage(www.text)) {
                TFriend friend = JsonConvert.DeserializeObject <TFriend>(www.text, SendHttp.Get.JsonSetting);

                if (friend.Kind == EFriendKind.Friend) {
                    GameData.Team.LifetimeRecord.FriendCount++;
                    GameData.Team.NeedForSyncRecord = true;
                    friend.PlayerInit();
                    if (GameData.Team.Friends.ContainsKey(friend.Identifier))
                        GameData.Team.Friends[friend.Identifier] = friend;
                    else
                        GameData.Team.Friends.Add(friend.Identifier, friend);

                    UIHint.Get.ShowHint(string.Format(TextConst.S(5035), friend.Player.Name), Color.white);
                    if (UISocial.Visible)
                        UISocial.Get.FreshFriend(3);
                    
                    WWWForm form = new WWWForm();
                    form.AddField("Identifier", GameData.Team.Identifier);
                    form.AddField("FriendID", friend.Identifier);

                    SendHttp.Get.Command(URLConst.LookSocialEvent, waitLookSocialEvent, form, false);
                } else 
                    if (GameData.Team.Friends.ContainsKey(friend.Identifier))
                        GameData.Team.Friends.Remove(friend.Identifier);

                if (confirmFriends.Count > 0)
                    confirmFriend(confirmFriends.Dequeue());
            }
        }
    }

    private void confirmFriend(string id) {
        WWWForm form = new WWWForm();
        form.AddField("Identifier", GameData.Team.Identifier);
        form.AddField("FriendID", id);
        form.AddField("Name", GameData.Team.Player.Name);
        form.AddField("Ask", "1");
        Command(URLConst.ConfirmMakeFriend, waitConfirm, form);
    }

    private void waitPubgameLogin(int resultCode, string playerId, string token) {
        if (resultCode != 99) {
            if (!string.IsNullOrEmpty(playerId)) {
                SendLogin(playerId);
                UIPubgame.Visible = false;
            } else
                UIHint.Get.ShowHint(TextConst.S(514) + " -50", Color.red);
        } else
            UIHint.Get.ShowHint(TextConst.S(514) + " " + resultCode.ToString(), Color.red);
    }
}
