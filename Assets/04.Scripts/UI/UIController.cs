//#define ShowHttpLog
//#define En
#define zh_TW
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

public struct TSessionResult
{
	public string sessionID;
}

public class UIController : MonoBehaviour
{
    public static UIController Get = null;
    public Hashtable cookieHeaders = new Hashtable();
    private UnibillDemo unibillDemo = null;

	public bool VersionChecked = false;
    private int stageIndex = -1;
    public int itemIndex = -1;
    public int itemUseTarget = -1;
	
	private bool IsSendPolling = false;
	private DateTime PollingTime;
	private DateTime LiveTime;
	private float trainTime = 0;
    private float totalTime = 0;
    private float AutoCDTime = 0;

//	void OnGUI(){
//		if (GUI.Button(new Rect(60,10,50,50), "Do"))
//			test();
//	}
	  
	/*
    void hideAllUI()
    {
        UIMall.UIShow(false);
        UILogo.UIShow(false);
        UIWaitingHttp.UIShow(false);
        UILoadingGame.UIShow(false);
        UIMessage.UIShow(false);
        UIMain.UIShow(false);
        UITeamManage.UIShow(false);
        UITeamName.UIShow(false);
        UIConference.UIShow(false);
        UIGame.UIShow(false);
		UIRecordGame.UIShow(false);
		UIGameResult.UIShow(false);
        UIRank.UIShow(false);
        UIDialog.UIShow(false);
        UIItemHint.UIShow(false);
        UIItemInfo.UIShow(false);
        UIItemCompose.UIShow(false);
		UIItemPopHint.UIShow(false);
        UIPlayerHint.UIShow(false);
        UIPlayerInfo.UIShow(false);
        UIPlayerLVUP.UIShow(false);
		UIPlayerDraft.UIShow(false);
		UIPlayerShow.UIShow(false);
		UIPlayerEvolution.UIShow(false);
        UIStage.UIShow(false);
		UIAction.UIShow(false);
		UISkillLvUp.UIShow(false);
		UISkillInfo.UIShow(false);
		UIMatch.UIShow(false);
		UIMatchHelp.UIShow(false);
		UIBattleStage.UIShow (false);
		UIMission.UIShow (false);
		UIBuilding.UIShow(false);
		UIGym.UIShow(false);
    }

    bool checkNetwork()
    {
        #if UNITY_EDITOR
        if (Network.player.ipAddress != "127.0.0.1" && Network.player.ipAddress != "0.0.0.0")
            return true;
        #else
        #if UNITY_IPHONE
            if (iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
                return true;
        #endif
        #if UNITY_ANDROID
            if (iPhoneSettings.internetReachability != iPhoneNetworkReachability.NotReachable)
                return true;
        #endif
        #if (!UNITY_IPHONE && !UNITY_ANDROID)
        if (Network.player.ipAddress != "127.0.0.1" && Network.player.ipAddress != "0.0.0.0")
            return true;
        #endif
        #endif

		UIMessage.Get.ShowMessage(TextConst.S(37), TextConst.S(93));
        return false;
    }

    public void CheckVersion()
    {
		UIMessage.UIShow(false);
		FileManager.AlreadyDownlandCount = 0;
		SendHttp(URLConst.Version, waitVersion, null, true);
    }

    private void initResolution()
    {
        try
        {
            GameObject ui2d = GameObject.Find("UI2D");
            if (ui2d != null)
            {
                UIRoot root = ui2d.GetComponent<UIRoot>();
                if (root != null)
                {
                    float width = 0;
                    float height = 0;
                    
                    if (Screen.width > Screen.height)
                    {
                        width = Screen.width;
                        height = Screen.height;
                    } else
                    {
                        width = Screen.height;
                        height = Screen.width;
                    }
                    
                    int rate = Mathf.CeilToInt(1.6f * 800f * height / width);
                    if (rate > 800)
                        root.manualHeight = rate;
                }
            }
        } catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void Start()
    {
		#if UNITY_EDITOR
			#if En
			TeamManager.LanguageKind = Language.en;
			#endif

			#if zh_TW
			TeamManager.LanguageKind = Language.zh_TW;
			#endif
		#else
			switch (Application.systemLanguage) {
			case SystemLanguage.English:
				TeamManager.LanguageKind = Language.en;
				break;
			case SystemLanguage.Chinese:
				TeamManager.LanguageKind = Language.zh_TW;
				break;
			default:
				TeamManager.LanguageKind = Language.en;
				break;
			}
		#endif

		if (PlayerPrefs.HasKey ("UserLanguage")) {
			int temp = Convert.ToInt16(PlayerPrefs.GetString("UserLanguage"));
			if(temp == Language.en.GetHashCode())
				TeamManager.LanguageKind = Language.en;
			else if(temp == Language.zh_TW.GetHashCode())
				TeamManager.LanguageKind = Language.zh_TW;
			else if(temp == Language.zh_CN.GetHashCode())
				TeamManager.LanguageKind = Language.zh_CN;
		}

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		TextConst.InitText ();
        Get = GameObject.Find("UI2D").GetComponent<UIController>();
        unibillDemo = gameObject.GetComponent<UnibillDemo>();
        initResolution();
        FacebookAPI.Init();

        TeamManager.Team = new TTeam(0);
        UILoading.UIShow(true);
		UILoading.Get.Title = TextConst.S (20230);
        TextureManager.InitResource();
		CheckVersion();
    }

    public void updateAutoPower()
    {
		totalTime += Time.deltaTime;
		if (totalTime > AutoCDTime + 1 && TeamManager.TrainTime != null)
        {
            if (TeamManager.Team.Lv > 0) 
                checkPowerCD();
            
            AutoCDTime = totalTime;
        }
    }

    void checkPowerCD()
    {
		if (TeamManager.Team.Lv > 0 && TeamManager.Team.Power < TeamManager.MaxPower() && UIMain.Visible)
        {
            if ((9 - DateTime.UtcNow.Subtract(TeamManager.Team.PowerCD.ToUniversalTime()).Minutes) >= 0)
                UIMain.Get.LabelPowerMax.text = (9 - DateTime.UtcNow.Subtract(TeamManager.Team.PowerCD.ToUniversalTime()).Minutes).ToString() + ":" + 
                    string.Format("{0:00}", (59 - DateTime.UtcNow.Subtract(TeamManager.Team.PowerCD.ToUniversalTime()).Seconds));
            else
                UIMain.Get.LabelPowerMax.text = TextConst.S(135);
        }
    }

    private void autoPowerResult(bool Value, WWW www)
    {
        if (Value && www.text != "")
        {
            TAutoPowerResult Data = (TAutoPowerResult)JsonConvert.DeserializeObject(www.text, (typeof(TAutoPowerResult)));
            TeamManager.Team.PowerCD = Data.PowerCD;
            UIMain.Get.AddPower(Data.Power);
        }
    }
    
    void OnApplicationFocus(bool focusStatus)
    {
		if (!focusStatus)
		{
			if (UIGame.Visible && UIGame.Get.IsStart) {
				UIGame.Get.SaveGame();
			}

			#if UNITY_EDITOR
            #else
			if(TeamManager.Team.Lv > 0){
				int LiveSec = Convert.ToInt32(DateTime.Now.Subtract(LiveTime).TotalSeconds);
				if(LiveSec > 0){
					WWWForm form = new WWWForm();
					form.AddField("LiveSec", LiveSec.ToString());
					SendHttp(URLConst.LiveTime, LiveTimeResult, form, true);
				}
			}
			#endif
		}else{
			if (UILoading.Visible) {
				CheckVersion ();
			} else
			if(TeamManager.Team.Lv > 0){
				LiveTime = DateTime.Now;

				UIController.Get.SendAutoPower();
				if(DateTime.UtcNow.Day != TeamManager.Team.LoginTime.ToUniversalTime().Day)
					SendHttp(URLConst.CheckResetToday, ResetTodayResult);
			}
		}
    }

	private void LiveTimeResult(bool Value, WWW www){

	}

	private void ResetTodayResult(bool Value, WWW www){
		if(Value){
			if(string.Empty != www.text){
				TResetTeamData Result = (TResetTeamData)JsonConvert.DeserializeObject (www.text, (typeof(TResetTeamData)));		
				TeamManager.Team.LoginTime = Result.LoginTime;
				TeamManager.Team.MatchScore = Result.MatchScore;
				TeamManager.Team.DunkGameCount = Result.DunkGameCount;
				TeamManager.Team.Shoot3GameCount = Result.Shoot3GameCount;
				TeamManager.Team.PVPFreeCount = Result.PVPFreeCount;
				TeamManager.Team.CrusadeCount = Result.CrusadeCount;
				TeamManager.Team.DailyFinishAy = Result.DailyFinishAy;
				TeamManager.Team.PlayerDailyRecord = Result.PlayerDailyRecord;
				TeamManager.Team.LogoDate = Result.LogoDate;
				TeamManager.Team.Items = Result.Items;
				TeamManager.Team.GymData = Result.GymData;
				TeamManager.Team.VipGymData = Result.VipGymData;
				TeamManager.Team.BuyPower = Result.BuyPower;

				if(Result.StageCharges != null)
					TeamManager.Team.StageCharges = Result.StageCharges;

				TeamManager.Team.InitTeamAttribute();
			}
		}
    }

	void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			if (UIGame.Visible && UIGame.Get.IsStart) {
				UIGame.Get.SaveGame();
			}
		}
	}

	void updatePlayerTrain() {
		if (Time.time - trainTime >= 1)
		{
			trainTime = Time.time;

			if (UIPlayerLVUP.Visible && TeamManager.Team.TrainingCDAy != null)
			{
				for (int i = 0; i < TeamManager.Team.TrainingCDAy.Length; i++)
				{
					if (TeamManager.CheckPlayerIndex(TeamManager.Team.TrainingCDAy [i].PlayerIndex))
					{
						int sec = (int)TeamManager.Team.TrainingCDAy [i].TrainTime.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds;
						if (sec == 0)
						{
							//Train Finish
							UIPlayerLVUP.Get.UpdateToFinish(i);
						} else
						if (sec > 0)
						{
							//Updata Time
							UIPlayerLVUP.Get.UpdateTimeStr(i);
						}
					}
				}
			}
			
			if (!UIGame.Visible && TeamManager.Team.TrainingCDAy != null && TeamManager.ShowPlayer != null && TeamManager.ShowPlayer.Length > 0)
			{
				for (int i = 0; i < TeamManager.Team.TrainingCDAy.Length; i++)
				{
					if (TeamManager.CheckPlayerIndex(TeamManager.Team.TrainingCDAy [i].PlayerIndex))
					{
						int sec = (int)TeamManager.Team.TrainingCDAy [i].TrainTime.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds;
						if (sec == 0)
						{
							//Train Finish
							for(int j = 0; j < TeamManager.ShowPlayer.Length; j++){
								if(TeamManager.ShowPlayer[j] == TeamManager.Team.TrainingCDAy [i].PlayerIndex){
									GameObject mplayer = null;
									if(UIMain.Get.playerItems.ContainsKey (j))
										mplayer = UIMain.Get.playerItems[j].Module;
									else
										mplayer = GameObject.Find("PlayerInfoModel/" + j.ToString());
									
									if(mplayer != null){
										Transform obj = mplayer.transform.FindChild("PlayerUpgradeDone");
										if(obj == null)
											EffectManager.Get.PlayEffect("PlayerUpgradeDone", mplayer.transform.localPosition, mplayer);
										else
											obj.gameObject.SetActive(true);
										
										Transform obj2 = mplayer.transform.FindChild("PlayerUpgrading");
										if(obj2 != null)
											obj2.gameObject.SetActive(false);
									}
								}
							}							
						} 
					}
				}
			}
		}
	}
    
    void FixedUpdate()
    {
		GSocket.Get.ReceivePotocol();

		if (!(UIGame.Visible || UIRecordGame.Visible || UILoadingGame.Visible)) {
        	updateAutoPower();
			updatePlayerTrain();
		}

		#if UNITY_ANDROID
		if (Input.GetKeyDown (KeyCode.Escape))
			UIDialog.Get.ShowDialog(TextConst.S(259), TextConst.S(260), AskToCloseApplication);
		#endif
    }

	public void AskToCloseApplication(bool flag)
	{
		if (flag)
			Application.Quit();
	}

    private void OnImageReadyMain(string id, int width, int height, Texture2D image)
    {       
        TextureManager.FBImages [id] = image;       
    }
        
    bool checkResponse(WWW www)
    {
        if (string.IsNullOrEmpty(www.error))
        {
            if (www.text.Contains("{err:"))
            {
                string e = www.text.Substring(6, www.text.Length - 7);
                Debug.Log(e);

				if (UILoading.Visible) {
					if (!VersionChecked)
						CheckVersion();
					else
						SendLogin();

					return false;
				}

                if (e == "Please login first.") {
					if (UIWaitingHttp.Visible) {
						WWWForm form = new WWWForm();
						form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
						SendHttp(URLConst.CheckSession, waitCheckSession, form);
						if (UIWaitingHttp.Visible)
							UIWaitingHttp.Get.WaitForCheckSession();
					}
				}
                else
				if (e == "Team not Found." ||
					e == "You have not created." ||
					e.Contains("connect")) {
					if (UIWaitingHttp.Visible)
						UIWaitingHttp.Get.ShowResend();
				} else
				if (e == "Fight end." || e == "Receipt error.") {
					UIWaitingHttp.UIShow(false);
					Debug.Log(e);
				} else {
					//UIMessage.Get.ShowMessage(TextConst.S(36), e);
				}
            } else
                return true;
        } else
        {
            Debug.Log(www.error);
            if (www.error == "couldn't connect to host")
				UIMessage.Get.ShowMessage(TextConst.S(38), TextConst.S(7));
            else
			if (UILoading.Visible) {
				if (!VersionChecked)
					CheckVersion();
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
					UIWaitingHttp.Get.ShowResend();
            } else 
			if (www.error.Contains("404 Not Found")){
				UIWaitingHttp.UIShow(false);
			} else {
				//UIMessage.Get.ShowMessage(TextConst.S(36), www.error);
			}
        }
        
        return false;
    }

	public void SendHttp(string url, TBooleanWWWObj callback, WWWForm form = null, bool waiting = false, bool resend = false)
	{
		if (checkNetwork())
		{
			WWW www = null;
			//http get
			if (url == URLConst.Version || url == URLConst.Polling)
				www = new WWW(url);
			else { //http post
				if (form == null)
					form = new WWWForm();

				if (!resend && !string.IsNullOrEmpty(TeamManager.Team.sessionID)) 
					form.AddField("sessionID", TeamManager.Team.sessionID);
			
				www = new WWW(url, form.data, TeamManager.CookieHeaders);
			}

			StartCoroutine(WaitForRequest(www, callback));

			if (waiting) 
				UIWaitingHttp.Get.SaveProtocol(url, callback, form);

			#if ShowHttpLog
			Debug.Log("Send To Server:" + url);
			#endif
		}
	}
	
	private IEnumerator WaitForRequest(WWW www, TBooleanWWWObj callback)
	{
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
		callback(flag, www);
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}

		www.Dispose();
	}

	public void SendLogin() {
		TeamManager.Team.Identifier = "";
		WWWForm form = new WWWForm();
		form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
		form.AddField("Language", TeamManager.LanguageKind.GetHashCode());
		#if UNITY_EDITOR
		form.AddField("OS", "0");
		#else
		#if UNITY_IPHONE
		form.AddField("OS", "1");
		#endif
		#if UNITY_ANDROID
		form.AddField("OS", "2");
		#endif
		#if (!UNITY_IPHONE && !UNITY_ANDROID)
		form.AddField("OS", "3");
		#endif
		#endif
		SendHttp(URLConst.deviceLogin, waitDeviceLogin, form, true);
	}
    
    private void waitVersion(bool ok, WWW www)
    {
        if (ok)
        {
			VersionChecked = true;
			if (www.text.CompareTo(BundleVersion.version) != 1)
				SendLogin();
             else
				UIUpdateVersion.UIShow(true);
        }
    }

	private void waitCheckSession(bool ok, WWW www)
	{
		if (ok)
		{
			TSessionResult result = (TSessionResult)JsonConvert.DeserializeObject(www.text, (typeof(TSessionResult)));
			TeamManager.Team.sessionID = result.sessionID;
			UIMessage.Get.ShowMessage(TextConst.S(36), TextConst.S(39));

			if (www.responseHeaders.ContainsKey("SET-COOKIE"))
			{
				cookieHeaders.Clear();
				cookieHeaders.Add("COOKIE", www.responseHeaders ["SET-COOKIE"]);
				TeamManager.CookieHeaders.Clear();
				TeamManager.CookieHeaders.Add("COOKIE", www.responseHeaders ["SET-COOKIE"]);
			}
			//if (UIWaitingHttp.Visible)
			//	UIWaitingHttp.Get.ShowResend();
		}
	}
    
    void polling()
    {
		if(!UIGame.Visible && !UIRecordGame.Visible && !UILoadingGame.Visible && 
		   !TeamManager.Team.BeAttack && !IsSendPolling &&
		   DateTime.Now.Subtract(PollingTime).TotalSeconds > 60) {
			PollingTime = DateTime.Now;
			IsSendPolling = true;
        	SendHttp(URLConst.Polling, waitPolling);
		}
    }
    
    private void waitPolling(bool Value, WWW www)
    {
        if (Value)
        {
			IsSendPolling = false;
            
            if (!string.IsNullOrEmpty(www.text))
            {
				TAttackInformation RevengeInfo = (TAttackInformation)JsonConvert.DeserializeObject(www.text, (typeof(TAttackInformation)));

				TeamManager.Team.AttackInfo = RevengeInfo.AttackInfo;
				TeamManager.Team.Badge = RevengeInfo.Badge;
				TeamManager.Team.Popularity = RevengeInfo.Popularity;
				TeamManager.Team.PVPWin = RevengeInfo.PVPWin;
				TeamManager.Team.BeAttack = true;
				UIMain.Get.AddMoney(RevengeInfo.Money);
            }
        }
    }

    public void waitDeviceLogin(bool flag, WWW www)
    {
		if (flag){
			if(UILoading.Visible){
				string text = GSocket.Get.OnHttpText(www.text);
				TeamManager.Team = JsonConvert.DeserializeObject <TTeam>(text); 
				TeamManager.Team.Init();
				LoadClientGameData();
				LiveTime = DateTime.Now;
				PollingTime = DateTime.Now;
				
				if (www.responseHeaders.ContainsKey("SET-COOKIE")){
					cookieHeaders.Clear();
					cookieHeaders.Add("COOKIE", www.responseHeaders ["SET-COOKIE"]);
					TeamManager.CookieHeaders.Clear();
					TeamManager.CookieHeaders.Add("COOKIE", www.responseHeaders ["SET-COOKIE"]);
				}
			}
		}
    }

    public void OnLogout()
    {
        cookieHeaders.Clear();
        TeamManager.CookieHeaders.Clear();
        UIMain.Get.ClearPlayerItem();

        if (UIGame.Visible)
			UIGame.Get.AskExitGame(true);
				
		hideAllUI();
        UILoading.UIShow(true);
    
        GC.Collect();
    }
    
	public void AskReloadGame(bool flag) {
		if (flag) {
			UILoadingGame.UIShow(true);
			UIGame.UIShow(true);
			UIGame.Get.InitEnemy(stageIndex);
			UIGame.Get.InitGameData(stageIndex);
			SceneMgr.Inst.ChangeLevel(stageIndex);
			if (UIGame.Get.LoadGame(stageIndex)) {
				UIMain.UIShow(false);
				ModelManager.Inst.PlayerInfoModel.SetActive (false);
				UIGame.Get.OnReadyStartGame();
			}
			else {
				stageIndex = -1;
				UILoadingGame.UIShow(false);
				UIGame.Get.AskExitGame(true);
			}
		} else
			stageIndex = -1;
	}
	
	public void OnCloseLoading()
	{
		if (UILoading.Get.ProgressValue == 1)
        {
            UILoading.UIShow(false);
			TeamManager.Team.InitTeamAttribute();

			if (PlayerPrefs.HasKey("Music"))
				TeamManager.Music = Convert.ToBoolean(PlayerPrefs.GetString("Music"));
			else
			{
				TeamManager.Music = true;
				PlayerPrefs.SetString("Music", TeamManager.Music.ToString());
				PlayerPrefs.Save();
			}
			
			if (PlayerPrefs.HasKey("Sound"))
				TeamManager.Sound = Convert.ToBoolean(PlayerPrefs.GetString("Sound"));
			else
			{
				TeamManager.Sound = true;
				PlayerPrefs.SetString("Sound", TeamManager.Sound.ToString());
				PlayerPrefs.Save();
			}

			if (PlayerPrefs.HasKey("Effect"))
				TeamManager.Effect = Convert.ToBoolean(PlayerPrefs.GetString("Effect"));
			else
			{
				TeamManager.Effect = true;
				PlayerPrefs.SetString("Effect", TeamManager.Effect.ToString());
				PlayerPrefs.Save();
			}

			if (PlayerPrefs.HasKey("SlowMotion"))
				TeamManager.SlowMotion = Convert.ToBoolean(PlayerPrefs.GetString("SlowMotion"));
			else
			{
				TeamManager.SlowMotion = false;
				PlayerPrefs.SetString("SlowMotion", TeamManager.SlowMotion.ToString());
				PlayerPrefs.Save();
			}
            
			SceneMgr.Inst.ChangeLevel(TeamManager.Team.MaxStage);

			if(TeamManager.UpdateNo != TeamManager.Updateinfo.UpdateNo)
				UIUpdateinfo.UIShow(true);

			InitPlayerStoreData();
			UI3D.Get.ShowCamera(false);

			if (TeamManager.Team.Lv == 0)
				UI3D.Get.Open3DUI(UIKind.CreateRole);
            else {
				CameraMgr.Inst.InitUICam();
                UIMain.Get.InitMainUI();

				//reload game
				try {
					int stageNo = PlayerPrefs.GetInt("Game_NowStage", -1);
					PlayerPrefs.SetInt("Game_NowStage", -1);
					if (stageNo <= TeamManager.Team.MaxStage && TeamManager.DStages.ContainsKey(stageNo) && 
					    TeamManager.DStages[stageNo].WinFlag >= 13 && TeamManager.DStages[stageNo].WinFlag <= 15) {
						if (!UIGame.Visible) {
							stageIndex = stageNo;
							UIDialog.Get.ShowDialog(TextConst.S (283), string.Format(TextConst.S (284), stageNo + 1, TeamManager.DStages[stageNo].Name), AskReloadGame);
						}
					}
				}
				catch (Exception e)
				{
					Debug.Log(e.ToString());
				}
            }


        }
    }

	public void InitPlayerStoreData()
	{
		if (TeamManager.PlayerStoreData.Count > 0)
			return;

		TStorePlayerPackage[] itemAy = new TStorePlayerPackage[2];

		for (int i = 0; i < itemAy.Length; i++) {
				itemAy [i] = new TStorePlayerPackage ();
				itemAy [i].PlayerIDAy = new List<int> ();
		}

		int itemId = 0;
		for (int i = 0; i < TeamManager.PlayerStore.Length; i++) {
			for (int j = 0; j < itemAy.Length; j++) {
				if (j == 0)
					itemId = TeamManager.PlayerStore [i].ItemID1;
				else
					itemId = TeamManager.PlayerStore [i].VipItemID1;

				if (TeamManager.DItems.ContainsKey (itemId) && TeamManager.DItems [itemId].Kind == 14) {
					itemAy [j].Kind = 1 + j;
					itemAy [j].PlayerIDAy.Add (TeamManager.DItems [itemId].Value);
				}
			}
		}

		TeamManager.PlayerStoreData.Add (itemAy [0]);
		TeamManager.PlayerStoreData.Add (itemAy [1]);
	}
    
	public void SendAutoPower() {
		if (DateTime.UtcNow.Subtract(TeamManager.Team.PowerCD.ToUniversalTime()).Minutes >= 10)
			SendHttp(URLConst.AutoPower, autoPowerResult);
	}

    public void OnBackToMain()
    {       
		if (UIHint.Get.NoHint())
			UIHint.UIShow(false);

		if (!UIWaitingHttp.Visible)
			UIWaitingHttp.Get.ReleaseUI();

        UILogo.UIShow(false);
        UIRank.UIShow(false);
        UITeamManage.UIShow(false);
        UIStage.UIShow(false);
		UIPlayerDraft.UIShow(false);

        UIMain.UIShow(true);
		UIMain.Get.SetPlayerDataVisible(true);

		polling();
		SendAutoPower();
    }

    private void waitOpenGem(bool Value, WWW www)
    {
        if (Value)
        {
            TOpenGemResult result = (TOpenGemResult)JsonConvert.DeserializeObject(www.text, (typeof(TOpenGemResult)));
            TeamManager.Team.Items = result.Items;
            if (TeamManager.DItems.ContainsKey(result.ID))
            {
                UIHint.Get.ShowHint(string.Format(TextConst.S(72), TeamManager.DItems [result.ID].Name), Color.blue);
                TItemData item = TeamManager.DItems [result.ID];

                if (item.Effect == 4 && TeamManager.DStages.ContainsKey(stageIndex) && TeamManager.DStages[stageIndex].WinFlag == 6){
					for (int i = 0; i < TeamManager.Team.Items.Length; i ++) {
						if (TeamManager.Team.Items [i].ID == result.ID){
							UITeamManage.UIShow(true);
							break;
						}
					}
                }
            }
        }
    }
    
    private void waitUseItemToPlayer(bool Value, WWW www)
    {
        if (Value)
        {
            TUseItemResult result = (TUseItemResult)JsonConvert.DeserializeObject(www.text, (typeof(TUseItemResult)));
            if (result.PlayerIndex >= 0 && result.PlayerIndex < TeamManager.Team.Players.Length)            
                TeamManager.Team.Players [result.PlayerIndex] = result.Player;

            TeamManager.Team.Items = result.Items;
        }
    }
    
    private void sendUseItem(TBooleanWWWObj waitHttp, bool waiting = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("ItemIndex", itemIndex);
        form.AddField("PlayerIndex", itemUseTarget);
        SendHttp(URLConst.UseItem, waitHttp, form, waiting);
    }

    public void sendUseItem(TBooleanWWWObj waitHttp, int ItemIndex, int PlayerIndex = -1, bool waiting = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("ItemIndex", ItemIndex);
        form.AddField("PlayerIndex", PlayerIndex);
        SendHttp(URLConst.UseItem, waitHttp, form, waiting);
    }

    public void OnUseItemToPlayer(int index)
    {
        itemUseTarget = index;
        OnUseItem();
    }
    
    public void OnUseItem()
    {
        if (itemIndex >= 0 && itemIndex < TeamManager.Team.Items.Length)
        {
            TItemData item = TeamManager.DItems [TeamManager.Team.Items [itemIndex].ID];
            
            if (item.UseTarget == 1)
            { //for team
                switch (item.Kind)
                {
                    case 11:
                        sendUseItem(waitOpenGem);
                        break;
                }
            } else
            if (item.UseTarget == 2)
            { //for player
                if (item.Effect == 4 && TeamManager.Team.Players [itemUseTarget].SkillA > 0)
                    UIHint.Get.ShowHint(TextConst.S(90), Color.red);
                else
                    sendUseItem(waitUseItemToPlayer, true);
            } else
				UIHint.Get.ShowHint(TextConst.S(63), Color.red);
        }
        
        UITeamManage.UIShow(false);
		if (UIMain.Get.gameObject.activeInHierarchy && !ModelManager.Inst.PlayerInfoModel.activeInHierarchy)
			ModelManager.Inst.PlayerInfoModel.SetActive(true);
    }

    private void waitPurchased(bool Value, WWW www)
    {
        if (Value)
        {
            try
            {
                if (!string.IsNullOrEmpty(www.text))
                {
                    TDayGiftResult result = (TDayGiftResult)JsonConvert.DeserializeObject(www.text, (typeof(TDayGiftResult)));
					TeamManager.Team.PlayerRecord.BuyDiamond = result.PlayerRecord.BuyDiamond;
					UIMain.Get.AddDiamond(result.Diamond);
                    if (TeamManager.DItems.ContainsKey(result.ItemID))
                    {
                        TeamManager.Team.Items = result.Items;
						UIHint.Get.ShowHint(string.Format(TextConst.S(251), TeamManager.DItems [result.ItemID].Name), Color.blue);
                    }

					if (TeamManager.DItems.ContainsKey(result.PlayerItemID))
					{
						TeamManager.Team.Players = result.Players;
						UIHint.Get.ShowHint(string.Format(TextConst.S(251), TeamManager.DPlayers[TeamManager.DItems [result.ItemID].Value].Name), Color.blue);
						TeamManager.Team.InitTeamAttribute();
						UIMain.Get.NeddResetTeam = true;
					}

					if(result.Achievements != null && result.Achievements.Length > TeamManager.Team.Achievements.Length){
						TeamManager.Team.Achievements = result.Achievements;
						if(UIMall.Visible)
							UIMall.Get.ResetBonus();
					}

					if(result.BuyMonth){
						TeamManager.Team.FreeDiamondDays = result.FreeDiamondDays;
						TeamManager.Team.FreeDiamondStar = result.FreeDiamondStar;
						if(UITeamManage.Visible)
							UITeamManage.Get.ReSetFreeDate();

						UIHint.Get.ShowHint(TextConst.S(285), Color.blue);
						UIHint.Get.ShowHint(TextConst.S(286), Color.blue);
						UIHint.Get.ShowHint(TextConst.S(287), Color.blue);
					}	

					if(result.Coupons != null)
						TeamManager.Team.Coupons = result.Coupons;

                    UIMain.Get.ResetTeamInformation();
					if(UIMall.Visible)
						UIMall.Get.ResetVip();
                }
            } catch (Exception e)
            {
				Debug.Log(e.Message);
            }
        }
	}

	string tempid = "";
	string tempReceipt = "";
    private void handlePurchased(string id, string Receipt)
    {
		tempid = id;
		tempReceipt = Receipt;
		DoSendBuyDiamond();
	}

	public void DoSendBuyDiamond(){
		WWWForm form = new WWWForm();
		form.AddField("id", tempid);
		form.AddField("Receipt", tempReceipt);
		SendHttp(URLConst.BuyDiamond, waitPurchased, form, true);
	}

    private IEnumerator HideWaitForRequest(WWW www, TBooleanWWWObj callback)
    {
        yield return www;
        
        if (string.IsNullOrEmpty(www.error))
        {
            #if ShowHttpLog
            Debug.Log("Receive from URL and Success:" + www.text);
            #endif
            callback(true, www);
        } else
        {     
            Debug.LogError("Receive from URL and Error:" + www.error);
            callback(false, www);
        }
        
        www.Dispose();
    }

    public void OnBuyDiamond()
    {
        int i = int.Parse(UIButton.current.name);
        if (i >= 0 && i <= 5) 
            unibillDemo.BuyItem(i, handlePurchased);
    }

	public void OnBuyMonth(){
		unibillDemo.BuyItem(5, handlePurchased);
	}
	
    public void AskOpenMall(bool flag)
    {
        if (flag) 
            OnOpenMall();
    }
	
	public void OnOpenMall() {
		if (UIMain.Visible)
			UIMain.Get.SetPlayerDataVisible(false);

		UILogo.UIShow(false);
		UIMessage.UIShow (false);
		UITeamManage.UIShow(false);
		UITeamName.UIShow(false);
		UIConference.UIShow (false);
		UIStage.UIShow (false);
		UIRank.UIShow(false);
		UIDialog.UIShow (false);
		UIMall.Get.OnOpenMall();
	}
    
    public void DoFullPower()
    {
        if (TeamManager.Team.Power < TeamManager.MaxPower())
            UIDialog.Get.ShowDialog(TextConst.S(244), string.Format(TextConst.S(229), TeamManager.BuyPowerDiamond), AskFullPower, 2, TeamManager.BuyPowerDiamond, 17);
        else
            UIHint.Get.ShowHint(TextConst.S(236), Color.red);
    }
    
    private void AskFullPower(int Kind)
    {
        switch (Kind)
        {
            case 0:
                if (TeamManager.Team.Diamond >= 100){
					if(TeamManager.MaxBuyPower > 0)
                   		SendHttp(URLConst.fullPower, FullPower, null, true);
					else
						UIHint.Get.ShowHint(TextConst.S(393), Color.red);
                } else
                    UIDialog.Get.ShowDialog(TextConst.S(245), TextConst.S(128), AskOpenMall);
                break;
            case 1:
                int mItemIndex = -1;
                if (TeamManager.Team.Items.Length > 0)
                {
                    for (int i = 0; i < TeamManager.Team.Items.Length; i++)
                    {
                        int ItemID = TeamManager.Team.Items [i].ID;
                        if (TeamManager.DItems.ContainsKey(ItemID))
                        {
                            if (TeamManager.DItems [ItemID].Kind == 17)
                            {
                                mItemIndex = i;
                                break;
                            }
                        }
                    }
                }
            
                if (mItemIndex >= 0 && mItemIndex < TeamManager.Team.Items.Length)
                    sendUseItem(UseFullPowerItem, mItemIndex, -1, true);
                else 
					UIHint.Get.ShowHint(TextConst.S(20003), Color.red);
					
					
                break;
        }
    }
    
    private void FullPower(bool Value, WWW www)
    {
        if (Value)
        {
            TFullPowerDiamondResult Result = (TFullPowerDiamondResult)JsonConvert.DeserializeObject(www.text, (typeof(TFullPowerDiamondResult)));
            UIMain.Get.AddPower(Result.Power);
			UIMain.Get.AddDiamond(Result.Diamond);
			TeamManager.Team.BuyPower = Result.BuyPower;
            UIHint.Get.ShowHint(TextConst.S(231), Color.blue);

			if(UIStage.Get.gameObject.activeInHierarchy)
				UIStage.Get.ResetPower();
        }
    }
    
    private void UseFullPowerItem(bool Value, WWW www)
    {
        if (Value)
        {
            TUseItemResult Result = (TUseItemResult)JsonConvert.DeserializeObject(www.text, (typeof(TUseItemResult)));
            TeamManager.Team.Items = Result.Items;
			UIMain.Get.AddPower(Result.Power);
            UIHint.Get.ShowHint(TextConst.S(231), Color.blue);

			if(UIStage.Get.gameObject.activeInHierarchy)
				UIStage.Get.ResetPower();
        }
    }

	private void LoadClientGameData ()
	{
		List<TUpdateData> DataList = new List<TUpdateData> ();
		for (int i = 0; i < FileManager.DownloadFiles.Length; i ++) {
			TUpdateData Data = new TUpdateData();
			Data.version = "0";
			Data.fileName = FileManager.DownloadFiles [i];
			DataList.Add(Data);
		}

		UILoading.Get.Title = TextConst.S (20231);
		FileManager.Get.LoadFileResource(DataList, LoadClientGameDataDone);
	}

	private void LoadClientGameDataDone(){
		if (UILoading.Visible && UILoading.Get.DownloadDone) {
			OnCloseLoading();

			switch(FileManager.NowMode){
			case VersionMode.debug:

				break;
			case VersionMode.release:
				List<TUpdateData> DataList = new List<TUpdateData> ();
				TUpdateData Data = new TUpdateData();
				Data.version = "0";
				Data.fileName = "updatedata.json";
				DataList.Add(Data);
				FileManager.Get.LoadFileServer(DataList, TeamManager.DownloadGameData);
				break;
			}
		}
	}
	*/
}
