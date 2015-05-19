using UnityEngine;
using System;
using Newtonsoft.Json;
using GameStruct;

public class LobbyStart : KnightSingleton<LobbyStart> {
	public bool ConnectToServer = false;

	private RPGCamera rpgCamera;
	private RPGMotor myPlayer;
	private GameObject moveToObject;

	void Start () {
		if (ConnectToServer && SendHttp.Get.CheckNetwork()) {
			WWWForm form = new WWWForm();
			form.AddField("OS", getOS());
			SendHttp.Get.Command(URLConst.Version, waitVersion, form);
		} else {
			if (GameData.LoadTeamSave())
				EnterLobby();
			else 
				SceneMgr.Get.ChangeLevel(SceneName.Court_0);
		}
	}

	void FixedUpdate() {
		if (rpgCamera != null && rpgCamera.UsedCamera != null && Input.GetMouseButtonDown(0)) {
			Ray ray1 = rpgCamera.UsedCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;  
			LayerMask mask = 1 << LayerMask.NameToLayer("Player"); 
			if (Physics.Raycast (ray1, out hit, 100, mask.value))
				clickPlayer(hit.collider.gameObject);
			else
			if (myPlayer) {
				mask = 1 << LayerMask.NameToLayer("Scene");
				if (Physics.Raycast (ray1, out hit, 100, mask.value))
					clickScene(hit.point);
			}
		}
	}

	private void clickPlayer(GameObject player) {

	}

	private void clickScene(Vector3 point) {
		myPlayer.Target = point;
		if (!moveToObject)
			moveToObject = EffectManager.Get.PlayEffect("MoveTo", point);
		else {
			moveToObject.transform.position = point;
			moveToObject.SetActive(true);
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

	private bool addRPGController(GameObject player) {
		if (player) {
			Animator ani = player.GetComponent<Animator>();
			if (ani != null) {
				RuntimeAnimatorController con = Resources.Load("Character/MMOCharacter") as RuntimeAnimatorController;
				if (con != null) {
					ani.runtimeAnimatorController = con;
					
					rpgCamera = player.AddComponent<RPGCamera>();
					if (rpgCamera != null) {
						rpgCamera.CameraPivotLocalPosition = new Vector3(0, 3, 0);
						rpgCamera.LockMouseY = true;
						rpgCamera.MinDistance = 5;
						rpgCamera.MaxDistance = 10;
						rpgCamera.UsedCamera.cullingMask =  (1 << LayerMask.NameToLayer("Default")) | 
															(1 << LayerMask.NameToLayer("Player")) | 
															(1 << LayerMask.NameToLayer("Scene"));
					}
					
					RPGViewFrustum vf = player.GetComponent<RPGViewFrustum>();
					if (vf != null)
						vf.FadeOutAlpha = 1;
					
					player.AddComponent<RPGController>();
					myPlayer = player.GetComponent<RPGMotor>();
					myPlayer.Target = player.transform.position;

					CharacterController c = player.GetComponent<CharacterController>();
					if (c != null) 
						c.center = new Vector3(0, 1.25f, 0);

					return true;
	            }
	        }
		}

		return false;
    }
    
    private void createMyPlayer() {
        if (!myPlayer) {
            GameData.Init();
            GameObject Res = new GameObject();
            Res.name = "Myself";
			Res.layer = LayerMask.NameToLayer ("Player");

			ModelManager.Get.SetAvatar (ref Res, GameData.Team.Player.Avatar, true);
			Res.transform.parent = ModelManager.Get.PlayerInfoModel.transform;
			Res.transform.localPosition = Vector3.zero;

			CapsuleCollider cc = Res.GetComponent<CapsuleCollider>();
			if (cc != null)
				cc.enabled = false;

			GameObject DummyBall = GameObject.Find("PlayerInfoModel/Myself/DummyBall");
			if (DummyBall) 
				DummyBall.SetActive(false);

			GameObject obj = Resources.Load("Prefab/Lobby/Projector") as GameObject;
			if (obj) {
				GameObject obj1 = Instantiate(obj);
				obj1.name = "Projector";
				obj1.transform.parent = Res.transform;
				obj1.transform.localScale = Vector3.one;
				obj1.transform.localPosition = Vector3.zero;
			}

			obj = Resources.Load("Prefab/Lobby/Follow") as GameObject;
			if (obj) {
				GameObject obj2 = Instantiate(obj);
				obj2.name = "Follow";
				obj2.transform.parent = Res.transform;
				obj2.transform.localScale = Vector3.one;
				obj2.transform.localPosition = Vector3.zero;
			}
			
			addRPGController(Res);
		}
	}

	public void EnterLobby() {
		try {
			UIMain.UIShow(true);
			createMyPlayer();
			UI3D.Get.ShowCamera(false);
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
}
