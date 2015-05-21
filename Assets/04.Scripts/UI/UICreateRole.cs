using UnityEngine;
using System.Collections;
using DG.Tweening;
using Newtonsoft.Json;
using GameStruct;

public struct TTeamName
{
	public string TeamName1TW;
	public string TeamName2TW;
	public string TeamName3TW;
	public string TeamName1EN;
	public string TeamName2EN;
	public string TeamName3EN;
	
	public string TeamName1{
		get{
			switch(GameData.Setting.Language){
			case Language.TW:
				return TeamName1TW;
			case Language.EN:
				return TeamName1EN;
			default:
				return TeamName1EN;
			}
		}
	}
	
	public string TeamName2{
		get{
			switch(GameData.Setting.Language){
			case Language.TW:
				return TeamName2TW;
			case Language.EN:
				return TeamName2EN;
			default:
				return TeamName2EN;
			}
		}
	}
	
	public string TeamName3{
		get{
			switch(GameData.Setting.Language){
			case Language.TW:
				return TeamName3TW;
			case Language.EN:
				return TeamName3EN;
			default:
				return TeamName3EN;
			}
		}
	}
}

public class UICreateRole : UIBase {
	private static UICreateRole instance = null;
	private const string UIName = "UICreateRole";
	
	public static TTeamName[] TeamNameAy;

	private GameObject smallInfo;
	private GameObject largeInfo;
	private GameObject start;

	private GameObject playerCenter;
	private GameObject[] playerPos;

	private float[] limitAngle;
	private GameStruct.TAvatar[] tAvatar;
	private int playerCount = 6;
	private int currentPlayer = 0;

	private UILabel labelName;

	private bool isTouchRotate;
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

		playerCenter = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center");
		playerPos = new GameObject[playerCount];
		playerPos[0] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos1");
		playerPos[1] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos2");
		playerPos[2] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos3");
		playerPos[3] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos4");
		playerPos[4] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos5");
		playerPos[5] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos6");

		labelName = GameObject.Find(UIName + "/BottomCenter/InputName/LabelEnter").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find (UIName + "/BottomCenter/RotationButton/ButtonRight")).onPress = OnRotateRight;
		UIEventListener.Get(GameObject.Find (UIName + "/BottomCenter/RotationButton/ButtonLeft")).onPress = OnRotateLeft;

		SetBtnFun (UIName + "/BottomLeft/ButtonInfo", OnClickSmallInfo);
		SetBtnFun (UIName + "/BottomLeft/ButtonOpen", OnClickLargeInfo);
		SetBtnFun (UIName + "/BottomRight/ButtonNext", OnCreateRole);
		SetBtnFun (UIName + "/BottomCenter/ButtonRoll", OnRandomName);

		largeInfo.SetActive(false);
		init();
	}
	
	protected override void InitData() {
		
	}

	protected override void InitText(){

	}
	
	protected override void OnShow(bool isShow) {
		loadJSON();
		OnRandomName();
	}

	public void OnRandomName(){
		if (TeamNameAy != null && TeamNameAy.Length > 0) {
			int index1 = UnityEngine.Random.Range (0, TeamNameAy.Length - 1);
			int index2 = UnityEngine.Random.Range (0, TeamNameAy.Length - 1);
			int index3 = UnityEngine.Random.Range (0, TeamNameAy.Length - 1);
			labelName.text = TeamNameAy [index1].TeamName1 + TeamNameAy [index2].TeamName2 + TeamNameAy [index3].TeamName3;
		}
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
		if (GameData.DPlayers.ContainsKey(currentPlayer+1)) {
			WWWForm form = new WWWForm();
			GameData.Team.Player.ID = GameData.DPlayers[currentPlayer+1].ID;
			GameData.Team.Player.Name = labelName.text;
			GameData.Team.Player.Avatar = tAvatar[currentPlayer];
			form.AddField("PlayerID", GameData.Team.Player.ID);
			form.AddField("Name", GameData.Team.Player.Name);
			
			SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
		}
	}

	private void waitCreateRole(bool ok, WWW www) {
		if (ok) {
			GameData.SaveTeam();
			UIShow(false);
			SceneMgr.Get.ChangeLevel(SceneName.Lobby);
		}
	}

	private void init(){
		playerCount = GameData.DPlayers.Count;
		limitAngle = new float[playerCount];
		tAvatar = new GameStruct.TAvatar[playerCount];
		for(int i=0; i<playerCount; i++) {
			limitAngle[i] = (360 / playerCount) * i;

			GameObject player = new GameObject();
			GameStruct.TPlayer p = new GameStruct.TPlayer();
			p.ID = i+1;
			p.SetAvatar();
			player.name = i.ToString();
			player.transform.parent = playerPos[i].transform;
			tAvatar[i] = p.Avatar;
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

	private static void loadJSON() {
		if(TeamNameAy == null || TeamNameAy.Length == 0){
			TextAsset tx = Resources.Load ("GameData/teamname") as TextAsset;
			if (tx)
				TeamNameAy = (TTeamName[])JsonConvert.DeserializeObject (tx.text, typeof(TTeamName[]));
		}
	}

	void FixedUpdate(){
		if(isRotateLeft && !isRotateRight) {
			playerPos[currentPlayer].transform.Rotate(new Vector3(0,2,0));
		} else if(!isRotateLeft && isRotateRight) {
			playerPos[currentPlayer].transform.Rotate(new Vector3(0,-2,0));
		}
		if(Input.GetMouseButton(0)) {
			axisX = 0;
			if(Input.mousePosition.y > (Screen.height * 0.4f))
				isDrag = true;
			else
				isTouchRotate = false;
		} else {
			axisX = 0;
			isDrag = false;
			if(isTouchRotate) {
				currentPlayer = findNearPlayer(playerCenter.transform.localEulerAngles.y);
				if (currentPlayer >= 0 && currentPlayer < limitAngle.Length && currentPlayer < tAvatar.Length){
					float angle = limitAngle[currentPlayer];
					playerCenter.transform.DOLocalRotate(new Vector3(0, angle, 0), 0.2f).OnUpdate(resetPlayerEuler);
				}
			}
			isTouchRotate = false;
		}
		if(isDrag){
			#if UNITY_EDITOR
				axisX = -Input.GetAxis ("Mouse X");
			#else
			#if UNITY_IOS
				if(Input.touchCount > 0)
					axisX = -Input.touches[0].deltaPosition.x;
			#endif
			#if UNITY_ANDROID
				if(Input.touchCount > 0)
					axisX = -Input.touches[0].deltaPosition.x;
			#endif
			#if (!UNITY_IOS && !UNITY_ANDROID)
				axisX = -Input.GetAxis ("Mouse X");
			#endif
			#endif
			playerCenter.transform.Rotate(new Vector3(0, axisX, 0), Space.Self);

			if(axisX != 0) {
				isTouchRotate = true;
				resetPlayerEuler();
			}
		}
	}
}
