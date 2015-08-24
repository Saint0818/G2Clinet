using DG.Tweening;
using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class UICreateRole : UIBase
{
	private static UICreateRole Instance = null;
	private const string UIName = "UICreateRole";

    private GameObject smallInfo;
	private GameObject largeInfo;

	private GameObject playerCenter;
	private GameObject[] playerPos;

	private float[] limitAngle;
	private TAvatar[] tAvatar;
	private int[] equipmentItems = new int[8];
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
			if(Instance)
				return Instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void SetVisible(bool visible)
    {
		if(Instance)
        {
			if(visible)
                Instance.Show(true);
			else
                RemoveUI(UIName);
        }
		else if(visible)
        {
			UI3D.UIShow(true);
			UI3D.Get.Open3DUI(UIKind.CreateRole);
			Get.Show(true);
		}
	}
	
	public static UICreateRole Get
	{
		get
        {
			if(!Instance) 
//				Instance = Load3DUI(UIName) as UICreateRole;
				Instance = LoadUI(UIName) as UICreateRole;
			
			return Instance;
		}
	}
	
	protected override void InitCom()
    {
//		smallInfo = GameObject.Find(UIName + "/BottomLeft/ButtonInfo");
//		largeInfo = GameObject.Find(UIName + "/BottomLeft/ButtonOpen");
//
//		playerCenter = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center");
//		playerPos = new GameObject[playerCount];
//		playerPos[0] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos1");
//		playerPos[1] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos2");
//		playerPos[2] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos3");
//		playerPos[3] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos4");
//		playerPos[4] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos5");
//		playerPos[5] = GameObject.Find(UIName + "/PlayerList/CenterTurn/Center/pos6");
//
//		labelName = GameObject.Find(UIName + "/BottomCenter/InputName/LabelEnter").GetComponent<UILabel>();
//
//		UIEventListener.Get(GameObject.Find (UIName + "/BottomCenter/RotationButton/ButtonRight")).onPress = OnRotateRight;
//		UIEventListener.Get(GameObject.Find (UIName + "/BottomCenter/RotationButton/ButtonLeft")).onPress = OnRotateLeft;
//
//		SetBtnFun (UIName + "/BottomLeft/ButtonInfo", OnClickSmallInfo);
//		SetBtnFun (UIName + "/BottomLeft/ButtonOpen", OnClickLargeInfo);
//		SetBtnFun (UIName + "/BottomRight/ButtonNext", OnCreateRole);
//		SetBtnFun (UIName + "/BottomCenter/ButtonRoll", OnRandomName);

//		largeInfo.SetActive(false);
//		init();
	}
	
	protected override void InitData() {
		
	}

	protected override void InitText(){

	}
	
	protected override void OnShow(bool isShow)
    {
//		loadJSON();
//		OnRandomName();
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

	public void OnCreateRole()
    {
		if (GameData.DPlayers.ContainsKey(currentPlayer+1)) {
			for (int i = 0; i < equipmentItems.Length; i++)
				equipmentItems[i] = 1 + i*10;

			WWWForm form = new WWWForm();
			GameData.Team.Player.ID = GameData.DPlayers[currentPlayer+1].ID;
			GameData.Team.Player.Name = labelName.text;
			GameData.Team.Player.Avatar = tAvatar[currentPlayer];
			form.AddField("PlayerID", GameData.Team.Player.ID);
			form.AddField("Name", GameData.Team.Player.Name);
			form.AddField("Items", JsonConvert.SerializeObject(equipmentItems));
			
			SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
		}
	}

	private void waitCreateRole(bool ok, WWW www) {
		if (ok) {
			GameData.Team.Player.Init();
			GameData.SaveTeam();
			SetVisible(false);

			if (SceneMgr.Get.CurrentScene != SceneName.Lobby)
				SceneMgr.Get.ChangeLevel(SceneName.Lobby);
			else
				LobbyStart.Get.EnterLobby();
		}
	}

	private void init(){
		playerCount = 3;
		limitAngle = new float[playerCount];
		tAvatar = new TAvatar[playerCount];
		for(int i=0; i<playerCount; i++) {
			if (GameData.DPlayers.ContainsKey(i+1)) {
				limitAngle[i] = (360 / playerCount) * i;

				GameObject player = new GameObject();
				TPlayer p = new TPlayer(0);
				p.ID = i+1;
				p.SetAvatar();
				player.name = i.ToString();
				player.transform.parent = playerPos[i].transform;
				tAvatar[i] = p.Avatar;
				ModelManager.Get.SetAvatar(ref player, p.Avatar, GameData.DPlayers[p.ID].BodyType, false);
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
