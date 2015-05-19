//#define En
#define zh_TW
using UnityEngine;
using System;
using System.Collections;
using Newtonsoft.Json;
using GameStruct;

public class GameStart : KnightSingleton<GameStart> {
	public static GameStart instance;
	public SceneTest  SceneMode = SceneTest.Single;
	public GameTest TestMode = GameTest.None;
	public CameraTest TestCameraMode = CameraTest.None;
	public bool IsSplitScreen = false;
	public float CrossTimeX = 0.5f;
	public float CrossTimeZ = 1;
	public TScoreRate ScoreRate = new TScoreRate(1);

	//server
	public bool ConnectToServer = false;
	
	void Start(){
		SceneMgr.Get.SetDontDestory (gameObject);

		if (SceneMode == SceneTest.Single)
			SceneMgr.Get.ChangeLevel (SceneName.Court_0);
		else if(SceneMode == SceneTest.Release){
			ConnectToServer = true;
			CheckServerData();
		}

		GameData.Init();
		TextConst.Init();
	}

	public void CheckServerData()
	{
		if (ConnectToServer && SendHttp.Get.CheckNetwork()) {
			WWWForm form = new WWWForm();
			form.AddField("OS", getOS());
			SendHttp.Get.Command(URLConst.Version, waitVersion, form);
		} else {
			if (GameData.LoadTeamSave())
				SceneMgr.Get.ChangeLevel(SceneName.Lobby);
			else 
				SceneMgr.Get.ChangeLevel(SceneName.Court_0);
		}
	}

	private string getOS() {
		string os = "0";
		#if UNITY_EDITOR
		
		#else
		#if UNITY_IOS
		os = "1";
		#endif
		#if UNITY_ANDROID
		os = "2";
		#endif
		#if (!UNITY_IOS && !UNITY_ANDROID)
		os = "3";
		#endif
		#endif
		
		return os;
	}

	private void waitVersion(bool ok, WWW www) {
		if (ok) {
			GameData.ServerVersion = www.text;
			if (www.text.CompareTo(BundleVersion.version) != 1)
				SendLogin();
			else
				UIHint.Get.ShowHint("Version is different.", Color.red);
		} else
			SceneMgr.Get.ChangeLevel(SceneName.Court_0);
	}

	private void SendLogin() {
		GameData.Team.Identifier = "";
		WWWForm form = new WWWForm();
		form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
		form.AddField("Language", GameData.Setting.Language.GetHashCode());
		form.AddField("OS", getOS ());
		
		SendHttp.Get.Command(URLConst.DeviceLogin, waitDeviceLogin, form);
	}

	private void waitDeviceLogin(bool flag, WWW www)
	{
		if (flag) {
			try {
				string text = GSocket.Get.OnHttpText(www.text);
				GameData.Team = JsonConvert.DeserializeObject <TTeam>(text); 
				
				if (www.responseHeaders.ContainsKey("SET-COOKIE")){
					SendHttp.Get.CookieHeaders.Clear();
					SendHttp.Get.CookieHeaders.Add("COOKIE", www.responseHeaders ["SET-COOKIE"]);
				}
				
				OnCloseLoading();
			} catch (Exception e) {
				Debug.Log(e.ToString());
			}
		} else
			Application.LoadLevel(GameConst.SceneGamePlay);
	}

	private void OnCloseLoading()
	{	
		if (GameData.Team.Player.Lv == 0)
			UICreateRole.UIShow(true);
		else {
			SceneMgr.Get.ChangeLevel(SceneName.Lobby);
		}
	}
	
	void OnGUI()
	{
		if (SceneMode == SceneTest.Multi){
			if (GUI.Button (new Rect (100, 0, 100, 50), "Lobby"))
				{
					SceneMgr.Get.ChangeLevel(SceneName.Lobby);
				}
				
				if (GUI.Button (new Rect (200 , 0, 100, 50), "InGame"))
				{
					SceneMgr.Get.ChangeLevel(SceneName.Court_0);
				}
		}
	}

	private void InitCom(){

	}
}
