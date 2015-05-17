using UnityEngine;
using System;
using Newtonsoft.Json;
using GameStruct;

public class LobbyStart : KnightSingleton<LobbyStart> {
	public bool ConnectToServer = false;

	private LobbyBehaviour myPlayer;

	void Start () {
		UIHint.Get.ShowHint("Enter lobby.", Color.blue);

		if (ConnectToServer && SendHttp.Get.CheckNetwork()) {
			WWWForm form = new WWWForm();
			form.AddField("OS", getOS());
			SendHttp.Get.Command(URLConst.Version, waitVersion, form);
		} else {
			if (GameData.LoadTeamSave()) {

			} else {
				//UIPlayerShow.UIShow(true);
				//Application.LoadLevel(GameConst.SceneGamePlay);
			}
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

	private void waitDeviceLogin(bool flag, WWW www)
	{
		if (flag) {
			try {
				string text = GSocket.Get.OnHttpText(www.text);
				GameData.Team = JsonConvert.DeserializeObject <TTeam>(text); 
				GameData.Init();
				
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

	private void SendLogin() {
		GameData.Team.Identifier = "";
		WWWForm form = new WWWForm();
		form.AddField("Identifier", SystemInfo.deviceUniqueIdentifier);
		form.AddField("Language", GameData.Setting.Language.GetHashCode());
		form.AddField("OS", getOS ());
		
		SendHttp.Get.Command(URLConst.DeviceLogin, waitDeviceLogin, form);
	}

	private void OnCloseLoading()
	{	
		if (GameData.Team.Player.Lv == 0)
			UICreateRole.UIShow(true);
		else {
			EnterLobby();
		}
	}

	private void createMyPlayer() {
		if (!myPlayer) {
			CreateLobbyPlayer(Vector3.zero, GameData.Team.Player);
		}
	}

	public void CreateLobbyPlayer(Vector3 BornPos, TPlayer playerattr){
		GameObject Res = new GameObject();
		
		ModelManager.Get.SetAvatar (ref Res, playerattr.Avatar, true);
		Res.transform.parent = ModelManager.Get.PlayerInfoModel.transform;
		Res.transform.localPosition = BornPos;
		LobbyBehaviour PB = Res.AddComponent<LobbyBehaviour>();
	}

	public void EnterLobby() {
		try {
			UIMain.UIShow(true);
			createMyPlayer();
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
}
