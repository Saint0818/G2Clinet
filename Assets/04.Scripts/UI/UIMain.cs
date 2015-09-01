using System.Collections.Generic;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class TRoomObject {
	public TRoomInfo roomInfo;
	public GameObject Item;
	public UILabel roomName;
}

public class UIMain : UIBase {
	private static UIMain instance = null;
	private const string UIName = "UIMain";
	private List<TRoomObject> roomObjects = new List<TRoomObject>();

    private GameObject[] EffectSwitch = new GameObject[2];
	private GameObject UIRoomInfo;
	private GameObject ButtonJoinRoom;
	private GameObject itemJoinRoom;
	private GameObject offsetRoom;
	private UIDraggableCamera cameraRoom;
	public Camera CameraScrollView;
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
//				if (!value)
//					RemoveUI(UIName);
//				else
					instance.Show(value);
			} else
			if (value)
				Get.Show(value);
		}
	}

	public static UIMain Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIMain;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		SetBtnFun(UIName + "/TopRight/ButtonCourt", OnStage);
		SetBtnFun(UIName + "/TopRight/ButtonOpenRoom", OnOpenRoom);
		SetBtnFun(UIName + "/TopRight/ButtonJoinRoom", OnLookingRoom);
		SetBtnFun(UIName + "/TopLeft/ButtonAvatar", OnAvatar);
		SetBtnFun(UIName + "/TopLeft/ButtonCreateRole", OnLookPlayerBank);
		SetBtnFun(UIName + "/TopLeft/ButtonSkillFormation", OnSkillFormation);

		itemJoinRoom = Resources.Load("Prefab/UI/Items/ItemJoinRoom") as GameObject;
		offsetRoom = GameObject.Find(UIName + "/TopRight/RoomInfo/View/Anchor/Offset");
		cameraRoom = GameObject.Find(UIName + "/TopRight/RoomInfo/View/ViewCamera").GetComponent<UIDraggableCamera>();
		CameraScrollView = GameObject.Find(UIName + "/TopRight/RoomInfo/View/ViewCamera").GetComponent<Camera>();
		ButtonJoinRoom = GameObject.Find(UIName + "/TopRight/ButtonJoinRoom");
		UIRoomInfo = GameObject.Find(UIName + "/TopRight/RoomInfo");
		UIRoomInfo.SetActive(false);
	}

	public void DoEffectSwitch(GameObject obj)
	{
		GameData.Setting.Effect = !GameData.Setting.Effect;
		EffectSwitch [0].SetActive (GameData.Setting.Effect);
		EffectSwitch [1].SetActive (!GameData.Setting.Effect);

		int index = 0;

		if (GameData.Setting.Effect)
			index = 1;

		CourtMgr.Get.EffectEnable (GameData.Setting.Effect);

		PlayerPrefs.SetInt (SettingText.Effect, index);
		PlayerPrefs.Save ();
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void OnCourt() {
		SceneMgr.Get.ChangeLevel(SceneName.Court_0);
	}

	private void waitRec1_2(JSONObject obj) {
		TRecBase[] result = JsonConvert.DeserializeObject<TRecBase[]>(obj.ToString());
		if (result.Length > 0) {
			GameData.RoomIndex = result[0].Index;
			SetLabel(UIName + "/TopRight/ButtonOpenRoom", "Close Room");
			ButtonJoinRoom.SetActive(false);
			UIRoomInfo.SetActive(false);
			LobbyStart.Get.ShowOnlinePlayers(true);
			LobbyStart.Get.ClearOnlinePlayers();
		}
	}

	private void waitOpenRoom() {
		GSocket.Get.Send(1, 2, null, waitRec1_2);
	}

	public void ExitRoom() {
		GameData.RoomIndex = -1;
		ButtonJoinRoom.SetActive(true);
		UIRoomInfo.SetActive(false);
		SetLabel(UIName + "/TopRight/ButtonOpenRoom", "Open Room");
		LobbyStart.Get.ShowOnlinePlayers(false);
	}

	public void OnLookPlayerBank() {
		WWWForm form = new WWWForm();
		SendHttp.Get.Command(URLConst.LookPlayerBank, waitLookPlayerBank, form);
	}

	private void waitLookPlayerBank(bool ok, WWW www)
	{
		if(ok)
        {
			TPlayerBank[] playerBank = JsonConvert.DeserializeObject<TPlayerBank[]>(www.text);

            foreach(var bank in playerBank)
            {
                Debug.Log(bank);
            }
            Visible = false;
            UICreateRole.Get.ShowFrameView(playerBank);
		}
        else
		    Debug.LogErrorFormat("Protocol:{0}", URLConst.LookPlayerBank);
	}

	public void OnStage() {
		UIStage.UIShow(!UIStage.Visible);
	}

	public void OnSkillFormation() {
		UISkillFormation.UIShow(!UISkillFormation.Visible);
	}

	public void OnOpenRoom() {
		if (GameData.RoomIndex == -1) {
			if (GSocket.Get.Connected) {
				if (GameData.IsLoginRTS)
					waitOpenRoom();
				else
					GSocket.Get.SendLoginRTS(waitOpenRoom);
			} else
				GSocket.Get.Connect(waitOpenRoom);
		} else {
			ExitRoom();
			GSocket.Get.Send(1, 3, null, waitRec1_3);
		}
	}

	private void waitRec1_3(JSONObject obj) {
		TRecBase[] result = JsonConvert.DeserializeObject<TRecBase[]>(obj.ToString());
		if (result.Length > 0) {
			if (result[0].R == 1)
				GSocket.Get.Close();
		}
	}

	private void waitRec1_4(JSONObject obj) {
		TRec1_4[] result = JsonConvert.DeserializeObject<TRec1_4[]>(obj.ToString());
		if (result.Length > 0) {
			if (result[0].R == 1) {
				for (int i = 0; i < result[0].Rooms.Length; i ++) {
					TRoomObject roomObj;
					if (roomObjects.Count > i) {
						roomObj = roomObjects[i];
						roomObj.Item.SetActive(true);
					} else {
						roomObj = new TRoomObject();

						GameObject room = Instantiate(itemJoinRoom) as GameObject;
						room.name = i.ToString();
						SetBtnFun(i.ToString(), OnJoinRoom);
						room.transform.parent = offsetRoom.transform;
						room.transform.localScale = Vector3.one;
						room.transform.localPosition = Vector3.zero;
						room.GetComponent<UIDragCamera>().draggableCamera = cameraRoom;

						roomObj.Item = room;
						roomObj.roomName = room.GetComponent<UILabel>();

						roomObjects.Add(roomObj);
					}

					roomObj.roomInfo = result[0].Rooms[i];
					roomObj.roomName.text = string.Format("Room{0} p{1}", i, roomObj.roomInfo.PlayerNum);
					roomObj.Item.transform.localPosition = new Vector3(0, i * 80, 0);
				}
			}
		}

		CameraScrollView.transform.localPosition = new Vector3(-120, 67, 0);
	}

	private void waitLookingRoom() {
		GSocket.Get.Send(1, 4, null, waitRec1_4);

		for (int i = 0; i < roomObjects.Count; i ++)
			roomObjects[i].Item.SetActive(false);
	}

	public void OnLookingRoom() {
		UIRoomInfo.SetActive(!UIRoomInfo.activeInHierarchy);
		if (UIRoomInfo.activeInHierarchy) {
			if (GSocket.Get.Connected)
				waitLookingRoom();
			else
				GSocket.Get.Connect(waitLookingRoom);
		} else {
			GSocket.Get.Close();
		}
	}

	private void waitRec1_5(JSONObject obj) {
		TRec1_5[] result = JsonConvert.DeserializeObject<TRec1_5[]>(obj.ToString());
		if (result.Length > 0) {
			if (result[0].R == 1) {
				LobbyStart.Get.ShowOnlinePlayers(true);
				LobbyStart.Get.InitOnlinePlayers(ref result[0].Teams, ref result[0].ScenePlayers);
				GameData.RoomIndex = result[0].Index;
				ButtonJoinRoom.SetActive(false);
				UIRoomInfo.SetActive(false);
				SetLabel(UIName + "/TopRight/ButtonOpenRoom", "Exit Room");
			}
		}
	}

	public void OnJoinRoom() {
		int index = -1;
		int.TryParse(UIButton.current.name, out index);
		if (index > -1 && index < roomObjects.Count) {
			TSend1_5 data = new TSend1_5();
			data.Index = roomObjects[index].roomInfo.Index;
			GSocket.Get.Send(1, 5, data, waitRec1_5);
		}
	}

	public void OnAvatar() {
		LobbyStart.Get.OnAvatar();
	}
}
