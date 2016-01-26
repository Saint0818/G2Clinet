using UnityEngine;
using System;
using System.Collections;
using Newtonsoft.Json;
using GameStruct;

//public delegate void CallBack();

public struct TPlayerObject {
	public GameObject PlayerObject;
	public TTeam PlayerData;
	public TScenePlayer ScenePlayer;
}

public class LobbyStart : MonoBehaviour {
    private static LobbyStart instance;
	public bool RPGMove = false;

	private int avatarID = 1;
	private TSend2_1 send2_1 = new TSend2_1();

	private TPlayer myPlayerData;

	public GameObject RootScenePlayers;
	public GameObject RootOnlinePlayers;
	private GameObject touchObject;
	private GameObject mySelfObject;
	private RPGCamera rpgCamera;
	private RPGMotor myRPGMotor;
	private GameObject moveToObject;
	private RPGMotor[] followPlayers = new RPGMotor[2];
	private GameObject[] followPoints = new GameObject[2];

	private TPlayerObject[] scenePlayers = new TPlayerObject[5];
	private TPlayerObject[] onlinePlayers = new TPlayerObject[2];

    public static LobbyStart Get {
        get {
            return instance;
        }
    }

    public static bool Visible {
        get {
            return instance && instance.gameObject.activeInHierarchy;
        }
    }

	void Awake ()
    {
        instance = gameObject.GetComponent<LobbyStart>();
        //DontDestroyOnLoad(gameObject);
		Time.timeScale = 1;

		/*RootScenePlayers = GameObject.Find("ScenePlayers");
		if (!RootScenePlayers) {
			RootScenePlayers = new GameObject();
			RootScenePlayers.name = "ScenePlayers";
		}

		RootOnlinePlayers = GameObject.Find("OnlinePlayers");
		if (!RootOnlinePlayers) {
			RootOnlinePlayers = new GameObject();
			RootOnlinePlayers.name = "OnlinePlayers";
		}*/
    }

    private void sceneMove() {
		if (RPGMove && rpgCamera != null && rpgCamera.UsedCamera != null) {
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) {
				Ray ray = UI2D.Get.Camera2D.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;  
				LayerMask mask = 1 << LayerMask.NameToLayer("UI"); 
				if (!Physics.Raycast (ray, out hit, 100, mask.value)) {
					if (UIMain.Visible) {
						ray = UIMain.Get.CameraScrollView.ScreenPointToRay (Input.mousePosition);
						mask = 1 << LayerMask.NameToLayer("ScrollView"); 
						if (Physics.Raycast (ray, out hit, 100, mask.value))
							return;
					}
					
					if (Input.GetMouseButtonDown(0)) {
						ray = rpgCamera.UsedCamera.ScreenPointToRay (Input.mousePosition);
						mask = 1 << LayerMask.NameToLayer("Player"); 
						if (Physics.Raycast (ray, out hit, 100, mask.value))
							touchObject = hit.collider.gameObject;
						else
						if (myRPGMotor && !touchObject) {
							mask = 1 << LayerMask.NameToLayer("Scene");
							if (Physics.Raycast (ray, out hit, 100, mask.value))
								clickScene(hit.point);
						}
					}
					
					if (Input.GetMouseButtonUp(0)) {
						ray = rpgCamera.UsedCamera.ScreenPointToRay (Input.mousePosition);
						mask = 1 << LayerMask.NameToLayer("Player"); 
						if (Physics.Raycast (ray, out hit, 100, mask.value) && hit.collider.gameObject == touchObject) 
							clickPlayer(hit.collider.gameObject);
						
						touchObject = null;
					}
				}
			}
		}
	}
    /*
    void FixedUpdate() {
		sceneMove();
    }*/
    
    private void clickPlayer(GameObject player) {
		if (player != myRPGMotor.gameObject) {
			for (int i = 0; i < followPlayers.Length; i ++) {
				if (followPlayers[i] && followPlayers[i].gameObject == player) {
					SetTeamMember(i, "");
					EffectManager.Get.PlayEffect("TeamQuit", Vector3.up, player);
					followPlayers[i].FollowObject = null;
					followPlayers[i] = null;
					return;
				}
			}

			for (int i = 0; i < followPlayers.Length; i ++) {
				if (!followPlayers[i]) {
					SetTeamMember(i, player.name);
					EffectManager.Get.PlayEffect("TeamJoin", Vector3.up, player);
					RPGMotor rpgMotor = player.GetComponent<RPGMotor>();
					if (rpgMotor) {
						rpgMotor.FollowObject = followPoints[i];
						followPlayers[i] = rpgMotor;
					}

					return;
				}
			}

			EffectManager.Get.PlayEffect("TeamFull", Vector3.up, player);
		}
	}

	private void clickScene(Vector3 point) {
		myRPGMotor.Target = point;
		if (!moveToObject)
			moveToObject = EffectManager.Get.PlayEffect("SelectA", point);
		else {
			moveToObject.transform.position = point;
			moveToObject.SetActive(true);
		}

		GameData.ScenePlayer.X = MyPlayerX;
		GameData.ScenePlayer.Z = MyPlayerZ;

		if (GameData.IsLoginRTS && GameData.RoomIndex > -1 && GSocket.Get.Connected) {
			send2_1.Kind = 1;
			send2_1.ScenePlayer.X = GameData.ScenePlayer.X;
			send2_1.ScenePlayer.Z = GameData.ScenePlayer.Z;
			send2_1.ScenePlayer.Dir = MyPlayerDir;
			send2_1.ScenePlayer.TX = (float) (Mathf.Floor(point.x * 100) / 100);
			send2_1.ScenePlayer.TZ = (float) (Mathf.Floor(point.z * 100) / 100);

			GSocket.Get.Send(2, 1, send2_1);
		}
    }

	public void PlayerOnTarget(GameObject other, GameObject trigger) {
		GameData.ScenePlayer.X = MyPlayerX;
		GameData.ScenePlayer.Z = MyPlayerZ;

		if (other.transform.name == "Myself" && myRPGMotor) {
			myRPGMotor.Target = myRPGMotor.transform.position;
			trigger.SetActive(false);

			if (GameData.IsLoginRTS && GameData.RoomIndex > -1 && GSocket.Get.Connected) {
				send2_1.Kind = 2;
				send2_1.ScenePlayer.X = GameData.ScenePlayer.X;
				send2_1.ScenePlayer.Z = GameData.ScenePlayer.Z;
				send2_1.ScenePlayer.Dir = MyPlayerDir;
				send2_1.ScenePlayer.TX = GameData.ScenePlayer.X;
				send2_1.ScenePlayer.TZ = GameData.ScenePlayer.Z;
				
				GSocket.Get.Send(2, 1, send2_1);
			}
		}
	}

	public void Rec_PlayerMove(ref TRec2_1 data) {
		if (RootOnlinePlayers.activeInHierarchy && (data.R == 1 || data.R == 2)) {
			GameObject player = GameObject.Find("OnlinePlayers/" + data.Name);
			if (player) {
				RPGMotor motor = player.GetComponent<RPGMotor>();
				if (motor)  
					motor.Target = new Vector3(data.ScenePlayer.TX, 0, data.ScenePlayer.TZ);
			}
		}
	}

	private void waitScenePlayer(bool flag, WWW www) {
		if (flag) {
			try {
				TTeam[] teams = JsonConvert.DeserializeObject <TTeam[]>(www.text);
				for (int i = 0; i < scenePlayers.Length; i ++) {
					Destroy(scenePlayers[i].PlayerObject);
					scenePlayers[i].PlayerObject = null;
					scenePlayers[i].PlayerData.Identifier = "";
				}

				for (int i = 0; i < teams.Length; i ++){
					if (i < scenePlayers.Length) 
						scenePlayers[i].PlayerData = teams[i];
				}

				StartCoroutine(initScenePlayers(waitScenePlayers));
			} catch (Exception e) {
				Debug.Log(e.ToString());
			}
		}
	}

	private bool addRPGController(GameObject player) {
		if (player) {
			if (rpgCamera == null) {
				rpgCamera = player.AddComponent<RPGCamera>();
				rpgCamera.CameraPivotLocalPosition = new Vector3(0, 3, 0);
				rpgCamera.LockMouseX = true;
				rpgCamera.LockMouseY = true;
				rpgCamera.MinDistance = 9;
				rpgCamera.MaxDistance = 9;
				if (rpgCamera.UsedCamera) {
					rpgCamera.UsedCamera.cullingMask =  (1 << LayerMask.NameToLayer("Default")) | 
														(1 << LayerMask.NameToLayer("Player")) | 
														(1 << LayerMask.NameToLayer("Scene"));

					rpgCamera.UsedCamera.transform.position = new Vector3(19.54f, 1.92f, -8);
					rpgCamera.UsedCamera.transform.eulerAngles = new Vector3(1.75f, 90, 0);
				}

				rpgCamera.enabled = false;
			}
			
			RPGViewFrustum vf = player.GetComponent<RPGViewFrustum>();
			if (vf != null)
				vf.FadeOutAlpha = 1;
			
			RPGController rpgController = player.AddComponent<RPGController>();
			rpgController.AcceptInput = true;
			myRPGMotor = player.GetComponent<RPGMotor>();
			myRPGMotor.Target = player.transform.position;

			CharacterController c = player.GetComponent<CharacterController>();
			if (c != null) 
				c.center = new Vector3(0, 1.25f, 0);

			return true;
		}

		return false;
    }

	private IEnumerator initOnlinePlayers(){
		for (int i = 0; i < onlinePlayers.Length; i ++) {
			if (onlinePlayers[i].PlayerData.Identifier != "" && !onlinePlayers[i].PlayerObject) {
				yield return new WaitForEndOfFrame();

				onlinePlayers[i].PlayerData.Init();
				onlinePlayers[i].PlayerObject = createScenePlayer(ref onlinePlayers[i].PlayerData.Player);
				onlinePlayers[i].PlayerObject.transform.parent = RootOnlinePlayers.transform;
				onlinePlayers[i].PlayerObject.transform.position = new Vector3(onlinePlayers[i].ScenePlayer.X, 0, onlinePlayers[i].ScenePlayer.Z);
				onlinePlayers[i].PlayerObject.transform.eulerAngles = new Vector3(0, onlinePlayers[i].ScenePlayer.Dir, 0);
				onlinePlayers[i].PlayerObject.AddComponent<RPGController>();
				CharacterController c = onlinePlayers[i].PlayerObject.GetComponent<CharacterController>();

				if (c != null) 
					c.center = new Vector3(0, 1.25f, 0);
			}
		}
	}

	private IEnumerator initScenePlayers(Action callback){
		for (int i = 0; i < scenePlayers.Length; i ++) {
			yield return new WaitForEndOfFrame();

			scenePlayers[i].PlayerData.Init();
			scenePlayers[i].PlayerObject = createScenePlayer(ref scenePlayers[i].PlayerData.Player);
			scenePlayers[i].PlayerObject.transform.parent = RootScenePlayers.transform;
			scenePlayers[i].PlayerObject.transform.position = new Vector3(UnityEngine.Random.Range(23, 26), 0, UnityEngine.Random.Range(-14, 16));
			scenePlayers[i].PlayerObject.transform.eulerAngles = new Vector3(0, 90, 0);
			scenePlayers[i].PlayerObject.AddComponent<RPGController>();
			CharacterController c = scenePlayers[i].PlayerObject.GetComponent<CharacterController>();

			if (c != null) 
				c.center = new Vector3(0, 1.25f, 0);
		}

		if (callback != null)
			callback();
	}

	private GameObject createScenePlayer(ref TPlayer player) {
		GameObject Res = new GameObject();
		Res.name = player.Name;
		Res.layer = LayerMask.NameToLayer ("Player");
		
		ModelManager.Get.SetAvatar (ref Res, player.Avatar, player.BodyType, EAnimatorType.AvatarControl, true);
		Res.transform.parent = ModelManager.Get.PlayerInfoModel.transform;
		Res.transform.localPosition = Vector3.zero;

		/*Animator ani = Res.GetComponent<Animator>();
		if (ani != null) {
			RuntimeAnimatorController con = Resources.Load("Character/MMOCharacter") as RuntimeAnimatorController;
			if (con != null) 
				ani.runtimeAnimatorController = con;
		}*/
		
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
		if (mySelfObject) {
			Destroy(mySelfObject);
			mySelfObject = null;
			myRPGMotor = null;
			rpgCamera = null;
		}

		myPlayerData = GameData.Team.Player;
		mySelfObject = createScenePlayer(ref GameData.Team.Player);
		mySelfObject.name = "Myself";
		mySelfObject.transform.position = new Vector3(24, 0.18f, -8);
		mySelfObject.transform.eulerAngles = new Vector3(0, 270, 0);
    	GameObject obj = Resources.Load("Prefab/Lobby/Follow") as GameObject;
		if (obj) {
			GameObject obj2 = Instantiate(obj);
			obj2.name = "Follow";

			obj2.transform.parent = mySelfObject.transform;
			obj2.transform.localScale = Vector3.one;
			obj2.transform.localEulerAngles = Vector3.zero;
			obj2.transform.localPosition = Vector3.zero;

			followPoints[0] = GameObject.Find("PlayerInfoModel/Myself/Follow/LeftBottom");
			followPoints[1] = GameObject.Find("PlayerInfoModel/Myself/Follow/RightBottom");
    	}
   
		addRPGController(mySelfObject);
	}

	private void waitScenePlayers() {
		setTeamMemberToFoller();
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
					if (name == scenePlayers[i].PlayerData.Player.Name) {
						GameData.TeamMembers[index] = scenePlayers[i].PlayerData;
		                break;
		            }
			}
		}
    }
    
    public void EnterLobby()
    {
		try
        {
			UILoading.UIShow(false);
			UIMainLobby.Get.Show();
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void InitOnlinePlayers(ref TTeam[] teams, ref TScenePlayer[] scenePlayers) {
		ClearOnlinePlayers();

		for (int i = 0; i < teams.Length; i ++) {
			if (i < onlinePlayers.Length) {
				onlinePlayers[i].PlayerData = teams[i];
				if (i < scenePlayers.Length)
					onlinePlayers[i].ScenePlayer = scenePlayers[i];
			}
		}

		StartCoroutine(initOnlinePlayers());
	}

	public void ClearOnlinePlayers() {
		for (int i = 0; i < onlinePlayers.Length; i ++) {
			onlinePlayers[i].PlayerData.Identifier = "";
			onlinePlayers[i].PlayerData.Player.ID = 0;
			if (onlinePlayers[i].PlayerObject) {
				Destroy(onlinePlayers[i].PlayerObject);
				onlinePlayers[i].PlayerObject = null;
			}
		}
	}

	public void AddOnlinePlayer(int index, ref TTeam team, ref TScenePlayer scenePlayer) {
		if (team.Player.ID > 0 && index > 0 && index < onlinePlayers.Length) {
			if (onlinePlayers[index].PlayerObject) {
				Destroy(onlinePlayers[index].PlayerObject);
				onlinePlayers[index].PlayerObject = null;
			}

			onlinePlayers[index].PlayerData = team;
			onlinePlayers[index].ScenePlayer = scenePlayer;
		}

		StartCoroutine(initOnlinePlayers());
	}

	public void RemoveOnlinePlayer(int index) {
		if (index > 0 && index < onlinePlayers.Length) {
			onlinePlayers[index].PlayerData.Identifier = "";
			onlinePlayers[index].PlayerData.Player.ID = 0;
			if (onlinePlayers[index].PlayerObject) {
				Destroy(onlinePlayers[index].PlayerObject);
				onlinePlayers[index].PlayerObject = null;
			}
		}
	}

	public void ShowOnlinePlayers(bool isShow) {
		if (isShow) {
			RootScenePlayers.SetActive(false);
			RootOnlinePlayers.SetActive(true);
			for (int i = 0; i < onlinePlayers.Length; i ++)
				if (onlinePlayers[i].PlayerObject && onlinePlayers[i].PlayerObject.transform.position.y < -0.17f)
					onlinePlayers[i].PlayerObject.transform.position = new Vector3(
						onlinePlayers[i].PlayerObject.transform.position.x, 0, 
						onlinePlayers[i].PlayerObject.transform.position.z);
		} else {
			RootScenePlayers.SetActive(true);
			RootOnlinePlayers.SetActive(false);
			for (int i = 0; i < scenePlayers.Length; i ++)
				if (scenePlayers[i].PlayerObject && scenePlayers[i].PlayerObject.transform.position.y < -0.17f)
					scenePlayers[i].PlayerObject.transform.position = new Vector3(
						scenePlayers[i].PlayerObject.transform.position.x, 0, 
						scenePlayers[i].PlayerObject.transform.position.z);
		}
	}

	private IEnumerator exchangeAvatar() {
		yield return new WaitForEndOfFrame();

		GameStruct.TAvatar a = new GameStruct.TAvatar();
		a.Body = 2000 + avatarID;
		a.Hair = 2000 + avatarID;
		a.AHeadDress = 1000 + avatarID;
		a.Cloth = 5000 + avatarID;
		a.Pants = 6000 + avatarID;
		a.Shoes = 1000 + avatarID;
		a.MHandDress = 1000 + avatarID;
		a.ZBackEquip = 1000 + avatarID;
		
		GameObject obj = myRPGMotor.gameObject;
		ModelManager.Get.SetAvatar(ref obj, a, 0, EAnimatorType.AvatarControl);
	}

	public void OnAvatar() {
		if (myRPGMotor != null) {
			if (avatarID == 1)
				avatarID = 2;
			else
				avatarID = 1;

			StartCoroutine(exchangeAvatar());
		}
	}

	private bool roleChanged {
		get {
			if (mySelfObject == null)
				return true;
			else 
				if (GameData.Team.Player.BodyType != myPlayerData.BodyType) 
					return true;
			else
				return false;
		}
	}

	public float MyPlayerX {
		get {
			if (myRPGMotor) 
				return (float) (Mathf.Floor(myRPGMotor.transform.position.x * 100) / 100);
			else
				return 0;
		}
	}

	public float MyPlayerZ {
		get {
			if (myRPGMotor) 
				return (float) (Mathf.Floor(myRPGMotor.transform.position.z * 100) / 100);
            else
                return 0;
        }
    }

	public float MyPlayerDir {
		get {
			if (myRPGMotor) 
				return (float) (Mathf.Floor(myRPGMotor.transform.rotation.y * 100) / 100);
			else
				return 0;
		}
	}
}
