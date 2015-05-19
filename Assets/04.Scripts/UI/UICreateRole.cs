using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UICreateRole : UIBase {
	private static UICreateRole instance = null;
	private const string UIName = "UICreateRole";

	private GameObject smallInfo;
	private GameObject largeInfo;
	private GameObject start;

	private GameObject playerCenter;
	private GameObject[] playerPos;

	private float[] limitAngle;
	private int playerCount = 6;
	private int currentPlayer = 0;

	private UILabel labelName;

	private bool isRotateRight;
	private bool isRotateLeft;
	private bool isDrag;
	private float axisX;

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		}
		else
		if (isShow){
			UI3D.UIShow(true);
			UI3D.Get.Open3DUI(UIKind.CreateRole);
			Get.Show(isShow);
		}
	}
	
	public static UICreateRole Get
	{
		get {
			if (!instance) 
				instance = Load3DUI(UIName) as UICreateRole;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		smallInfo = GameObject.Find(UIName + "/BottomLeft/ButtonInfo");
		largeInfo = GameObject.Find(UIName + "/BottomLeft/ButtonOpen");
		start = GameObject.Find(UIName + "/BottomRight/ButtonNext");

		playerCenter = GameObject.Find(UIName + "/Test/Center");
		playerPos = new GameObject[playerCount];
		playerPos[0] = GameObject.Find(UIName + "/Test/Center/pos1");
		playerPos[1] = GameObject.Find(UIName + "/Test/Center/pos2");
		playerPos[2] = GameObject.Find(UIName + "/Test/Center/pos3");
		playerPos[3] = GameObject.Find(UIName + "/Test/Center/pos4");
		playerPos[4] = GameObject.Find(UIName + "/Test/Center/pos5");
		playerPos[5] = GameObject.Find(UIName + "/Test/Center/pos6");

		labelName = GameObject.Find(UIName + "/BottomCenter/InputName/LabelEnter").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find (UIName + "/BottomCenter/RotationButton/ButtonRight")).onPress = OnRotateRight;
		UIEventListener.Get(GameObject.Find (UIName + "/BottomCenter/RotationButton/ButtonLeft")).onPress = OnRotateLeft;

		SetBtnFun (UIName + "/BottomLeft/ButtonInfo", OnClickSmallInfo);
		SetBtnFun (UIName + "/BottomLeft/ButtonOpen", OnClickLargeInfo);
		SetBtnFun (UIName + "/BottomRight/ButtonNext", OnCreateRole);
		SetBtnFun (UIName + "/BottomCenter/ButtonRoll", OnRandomName);

		start.SetActive(false);
		largeInfo.SetActive(false);
		init();
	}
	
	protected override void InitData() {
		
	}

	protected override void InitText(){

	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void OnRandomName(){

	}

	public void OnRotateRight(GameObject go, bool state){
		isRotateRight = state;
	}

	public void OnRotateLeft(GameObject go, bool state){
		isRotateLeft = state;
	}

	public void OnClickSmallInfo(){
		smallInfo.SetActive(false);
		largeInfo.SetActive(true);
	}

	public void OnClickLargeInfo(){
		smallInfo.SetActive(true);
		largeInfo.SetActive(false);
	}

	public void OnCreateRole() {
		WWWForm form = new WWWForm();
		GameData.Team.Player.ID = GameData.DPlayers[currentPlayer+1].ID;
		GameData.Team.Player.Name = labelName.text;
		form.AddField("PlayerID", GameData.Team.Player.ID);
		form.AddField("Name", GameData.Team.Player.Name);
		
		SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
	}

	private void waitCreateRole(bool ok, WWW www) {
		if (ok) {
			GameData.SaveTeamSave();
			UIShow(false);
			LobbyStart.Get.EnterLobby();
		}
	}

	private void init(){
		playerCount = GameData.DPlayers.Count;
		limitAngle = new float[playerCount];
		for(int i=0; i<playerCount; i++) {
			limitAngle[i] = (360 / playerCount) * i;

			GameObject player = new GameObject();
			GameStruct.TPlayer p = new GameStruct.TPlayer();
			p.ID = i+1;
			p.SetAvatar();
			player.name = i.ToString();
			player.transform.parent = playerPos[i].transform;
			ModelManager.Get.SetAvatar(ref player, p.Avatar, false);
			player.transform.localPosition = new Vector3(0, -1, 0);
			player.transform.localEulerAngles = new Vector3(0, 180, 0);
			player.transform.localScale = Vector3.one;
			for (int j=0; j<player.transform.childCount; j++) { 
				if(player.transform.GetChild(j).name.Contains("PlayerMode")) {
					player.transform.GetChild(j).localScale = Vector3.one;
					player.transform.GetChild(j).localEulerAngles = Vector3.zero;
					player.transform.GetChild(j).localPosition = Vector3.zero;
				}
			}
		}
	}

	private int findNearPlayer(float vy){
		int index = 0;
		float temp = 0;
		if(vy < 0) 
			vy = 360 + vy;
		for (int i=0; i<playerCount; i++){
			float minusValue = Mathf.Abs(vy - limitAngle[i]);
			if (i==0)
				temp = vy;
			else {
				if((360 - vy) < 30) 
					index = 0;
				else
				if(minusValue < temp) {
					temp = minusValue;
					index = i;
				}
			}
		}
		return index;
	}

	private void resetPlayerEuler(){
		for (int i=0; i<playerCount; i++) {
			playerPos[i].transform.localEulerAngles = Vector3.zero;
			playerPos[i].transform.LookAt(playerPos[i].transform.position + new Vector3(1, 0, 0) , Vector3.up);
		}
	}

	void FixedUpdate(){
		if(labelName.text == "Enter your name" || labelName.text == ""){
			start.SetActive(false);
		} else {
			start.SetActive(true);
		}

		if(isRotateLeft && !isRotateRight) {
			playerPos[currentPlayer].transform.Rotate(new Vector3(0,2,0));
		} else if(!isRotateLeft && isRotateRight) {
			playerPos[currentPlayer].transform.Rotate(new Vector3(0,-2,0));
		}

		if(Input.GetMouseButtonDown(0)) {
			isDrag = true;
		}
		if(Input.GetMouseButtonUp(0)){
			isDrag = false;
			if(!isRotateLeft && !isRotateRight) {
				currentPlayer = findNearPlayer(playerCenter.transform.localEulerAngles.y);
				float angle = limitAngle[currentPlayer];
				playerCenter.transform.DOLocalRotate(new Vector3(0, angle, 0), 0.5f).OnUpdate(resetPlayerEuler);

			}
		}
		if(isDrag){
			if(!isRotateLeft && !isRotateRight) {
				axisX = -Input.GetAxis ("Mouse X");
				playerCenter.transform.Rotate(new Vector3(0, axisX, 0), Space.Self);
			}
			if(axisX != 0)
				resetPlayerEuler();
		}
	}
}
