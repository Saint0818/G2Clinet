using UnityEngine;
using System;
using Newtonsoft.Json;
using GameStruct;

public class LobbyStart : KnightSingleton<LobbyStart> {
	public bool ConnectToServer = false;

	private GameObject touchObject;
	private RPGCamera rpgCamera;
	private RPGMotor myPlayer;
	private GameObject moveToObject;
	private RPGMotor[] followPlayers = new RPGMotor[2];
	private GameObject[] followPoints = new GameObject[2];

	private TTeam[] scenePlayers;

	void Start () {
		Time.timeScale = 1;
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

		GameData.Init();
		TextConst.Init();
		SceneMgr.Get.CurrentScene = SceneName.Lobby;
    }
    
    void Update() {
		if (rpgCamera != null && rpgCamera.UsedCamera != null) {
			if (Input.GetMouseButtonDown(0)) {
				Ray ray = rpgCamera.UsedCamera.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;  
				LayerMask mask = 1 << LayerMask.NameToLayer("Player"); 
				if (Physics.Raycast (ray, out hit, 100, mask.value))
					touchObject = hit.collider.gameObject;
			}

			if (Input.GetMouseButtonUp(0)) {
				Ray ray1 = rpgCamera.UsedCamera.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit1;  
				LayerMask mask = 1 << LayerMask.NameToLayer("Player"); 
				if (Physics.Raycast (ray1, out hit1, 100, mask.value) && hit1.collider.gameObject == touchObject) 
					clickPlayer(hit1.collider.gameObject);
				else
				if (myPlayer && !touchObject) {
					mask = 1 << LayerMask.NameToLayer("Scene");
					if (Physics.Raycast (ray1, out hit1, 100, mask.value))
						clickScene(hit1.point);
				}

				touchObject = null;
	        }
		}
    }
    
    private void clickPlayer(GameObject player) {
		if (player != myPlayer.gameObject) {
			for (int i = 0; i < followPlayers.Length; i ++) {
				if (followPlayers[i] && followPlayers[i].gameObject == player) {
					SetTeamMember(i, "");
					EffectManager.Get.PlayEffect("TeamQuit", new Vector3(0, 1, 0), player);
					followPlayers[i].FollowObject = null;
					followPlayers[i] = null;
					return;
				}
			}

			for (int i = 0; i < followPlayers.Length; i ++) {
				if (!followPlayers[i]) {
					SetTeamMember(i, player.name);
					EffectManager.Get.PlayEffect("TeamJoin", new Vector3(0, 1, 0), player);
					RPGMotor rpgMotor = player.GetComponent<RPGMotor>();
					if (rpgMotor) {
						rpgMotor.FollowObject = followPoints[i];
						followPlayers[i] = rpgMotor;
					}

					return;
				}
			}

			EffectManager.Get.PlayEffect("TeamFull", new Vector3(0, 1, 0), player);
		}
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

	private void waitDeviceLogin(bool flag, WWW www) {
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

	private void waitScenePlayer(bool flag, WWW www) {
		if (flag) {
			try {
			scenePlayers = JsonConvert.DeserializeObject <TTeam[]>(www.text);
			initScenePlayers();
			setTeamMemberToFoller();
			} catch (Exception e) {
				Debug.Log(e.ToString());
			}
		}
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
		else 
			EnterLobby();
	}

	private bool addRPGController(GameObject player) {
		if (player) {
			rpgCamera = player.AddComponent<RPGCamera>();
			if (rpgCamera != null) {
				rpgCamera.CameraPivotLocalPosition = new Vector3(0, 3, 0);
				rpgCamera.LockMouseX = true;
				rpgCamera.LockMouseY = true;
				rpgCamera.MinDistance = 9;
				rpgCamera.MaxDistance = 9;
				rpgCamera.UsedCamera.cullingMask =  (1 << LayerMask.NameToLayer("Default")) | 
													(1 << LayerMask.NameToLayer("Player")) | 
													(1 << LayerMask.NameToLayer("Scene"));
			}
			
			RPGViewFrustum vf = player.GetComponent<RPGViewFrustum>();
			if (vf != null)
				vf.FadeOutAlpha = 1;
			
			RPGController rpgController = player.AddComponent<RPGController>();
			rpgController.AcceptInput = true;
			myPlayer = player.GetComponent<RPGMotor>();
			myPlayer.Target = player.transform.position;

			CharacterController c = player.GetComponent<CharacterController>();
			if (c != null) 
				c.center = new Vector3(0, 1.25f, 0);

			return true;
		}

		return false;
    }

	private void initScenePlayers(){
		for (int i = 0; i < scenePlayers.Length; i ++) {
			scenePlayers[i].Player.SetAvatar();
			GameObject player = createScenePlayer(ref scenePlayers[i].Player);
			player.transform.position = new Vector3(UnityEngine.Random.Range(23, 26), 0, UnityEngine.Random.Range(-14, 16));
			player.transform.eulerAngles = new Vector3(0, 90, 0);
			player.AddComponent<RPGController>();
			CharacterController c = player.GetComponent<CharacterController>();
			if (c != null) 
				c.center = new Vector3(0, 1.25f, 0);
		}
	}

	private GameObject createScenePlayer(ref TPlayer player) {
		GameObject Res = new GameObject();
		Res.name = player.Name;
		Res.layer = LayerMask.NameToLayer ("Player");
		
		ModelManager.Get.SetAvatar (ref Res, player.Avatar, true);
		Res.transform.parent = ModelManager.Get.PlayerInfoModel.transform;
		Res.transform.localPosition = Vector3.zero;

		Animator ani = Res.GetComponent<Animator>();
		if (ani != null) {
			RuntimeAnimatorController con = Resources.Load("Character/MMOCharacter") as RuntimeAnimatorController;
			if (con != null) 
				ani.runtimeAnimatorController = con;
		}
		
		CapsuleCollider cc = Res.GetComponent<CapsuleCollider>();
		if (cc != null)
			cc.enabled = false;

		GameObject DummyBall = GameObject.Find("PlayerInfoModel/" + player.Name + "/DummyBall");
		if (DummyBall) 
			DummyBall.SetActive(false);

		BoxCollider bc = Res.AddComponent<BoxCollider>();
		bc.center = new Vector3(0, 1.5f, 0);
		bc.size = new Vector3(0, 3, 0);
		bc.isTrigger = true;

		GameObject obj = Resources.Load("Prefab/Lobby/Projector") as GameObject;
		if (obj) {
			GameObject obj1 = Instantiate(obj);
			obj1.name = "Projector";
			obj1.transform.parent = Res.transform;
			obj1.transform.localScale = Vector3.one;
			obj1.transform.localPosition = Vector3.zero;
		}

		obj = Resources.Load("Prefab/Lobby/PlayerName") as GameObject;
		if (obj) {
			GameObject obj1 = Instantiate(obj);
			obj1.name = "PlayerName";
			UILabel label = obj1.GetComponentInChildren<UILabel>();
			if (label)
				label.text = player.Name;

			obj1.transform.parent = Res.transform;
			obj1.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
			obj1.transform.localPosition = new Vector3(0, 3.5f, 0);
		}

		return Res;
    }
    
    private void createMyPlayer() {
        if (!myPlayer) {
			GameObject player = createScenePlayer(ref GameData.Team.Player);
			player.name = "Myself";
			player.transform.eulerAngles = new Vector3(0, 90, 0);
            GameObject obj = Resources.Load("Prefab/Lobby/Follow") as GameObject;
			if (obj) {
				GameObject obj2 = Instantiate(obj);
				obj2.name = "Follow";

				obj2.transform.parent = player.transform;
				obj2.transform.localScale = Vector3.one;
				obj2.transform.localEulerAngles = Vector3.zero;
				obj2.transform.localPosition = Vector3.zero;

				followPoints[0] = GameObject.Find("PlayerInfoModel/Myself/Follow/LeftBottom");
				followPoints[1] = GameObject.Find("PlayerInfoModel/Myself/Follow/RightBottom");
            }
           
            addRPGController(player);
		}
	}

	private void setTeamMemberToFoller() {
		for (int i = 0; i < GameData.TeamMembers.Length; i ++)
			if (GameData.TeamMembers[i].Player.Name != "") {
				bool flag = true;
				GameObject player = GameObject.Find("PlayerInfoModel/" + GameData.TeamMembers[i].Player.Name);
				if (player) {
					RPGMotor rpgMotor = player.GetComponent<RPGMotor>();
					if (rpgMotor) {
						rpgMotor.FollowObject = followPoints[i];
						followPlayers[i] = rpgMotor;
						flag = false;
	                }
	            }

				if (flag)
					GameData.TeamMembers[i] = new TTeam();
        }
	}

	private void SetTeamMember(int index, string name) {
		if (index >= 0 && index < GameData.TeamMembers.Length) {
			if (name == "")
				GameData.TeamMembers[index] = new TTeam();
			else {
				for (int i = 0; i < scenePlayers.Length; i++)
					if (name == scenePlayers[i].Player.Name) {
						GameData.TeamMembers[index] = scenePlayers[i];
		                break;
		            }
			}
		}
    }
    
    public void EnterLobby() {
		try {
			GameData.Init();
			UIMain.UIShow(true);
			createMyPlayer();
			WWWForm form = new WWWForm();
			SendHttp.Get.Command(URLConst.ScenePlayer, waitScenePlayer, form);
			if (UI3D.Visible)
				UI3D.Get.ShowCamera(false);
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
}
