using System.ComponentModel;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

[DisallowMultipleComponent]
public class UICreateRole : UIBase
{
	private static UICreateRole instance;
	private const string UIName = "UICreateRole";

    private UICreateRoleFrameView mFrameView;
    private UICreateRolePositionView mPositionView;
    private UICreateRoleStyleView mStyleView;

//    private GameObject smallInfo;
//	private GameObject largeInfo;

//	private GameObject playerCenter;
//	private GameObject[] playerPos;

//	private float[] limitAngle;
//	private TAvatar[] tAvatar;
	private int[] equipmentItems = new int[8];
//	private int playerCount = 6;
//	private int currentPlayer = 0;

//	private UILabel labelName;

//	private bool isTouchRotate;
//	private bool isRotateRight;
//	private bool isRotateLeft;
//	private bool isDrag;
//	private float axisX;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
		
		set {
			if (instance) {
				if (!value)
					RemoveUI(UIName);
				else
					instance.Show(value);
			} else
			if (value)
				Get.Show(value);
		}
	}

    public void ShowFrameView()
    {
        Show(true);

        mFrameView.Visible = true;
        mPositionView.Visible = false;
        mStyleView.Hide();
    }

    public void ShowPositionView()
    {
        Show(true);

        mFrameView.Visible = false;
        mPositionView.Visible = true;
        mStyleView.Hide();
    }

    public void ShowStyleView(EPlayerPostion pos)
    {
        Show(true);

        mFrameView.Visible = false;
        mPositionView.Visible = false;
        mStyleView.Show(pos);
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

	public static UICreateRole Get
	{
		get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UICreateRole;
            }
			
			return instance;
		}
	}

    [UsedImplicitly]
    private void Awake()
    {
        mFrameView = GetComponent<UICreateRoleFrameView>();
        mFrameView.Visible = true;

        mPositionView = GetComponent<UICreateRolePositionView>();
        mPositionView.Visible = false;

        mStyleView = GetComponent<UICreateRoleStyleView>();
        mStyleView.Hide();

		GameData.Team.Player.ID = 1;
		GameData.Team.Player.Name = SystemInfo.deviceUniqueIdentifier;
		mStyleView.EquipmentItems[0] = CreateRoleDataMgr.Ins.Body(EPlayerPostion.G, 0);
		mStyleView.EquipmentItems[1] = CreateRoleDataMgr.Ins.Hair(EPlayerPostion.G, 0);
		mStyleView.EquipmentItems[3] = CreateRoleDataMgr.Ins.Cloth(EPlayerPostion.G, 0);
		mStyleView.EquipmentItems[4] = CreateRoleDataMgr.Ins.Pants(EPlayerPostion.G, 0);
		mStyleView.EquipmentItems[5] = CreateRoleDataMgr.Ins.Shoes(EPlayerPostion.G, 0);
    }

	protected override void InitCom()
	{
	    
    }
	
//	public void OnCreateRole()
//    {
//		if (GameData.DPlayers.ContainsKey(currentPlayer+1)) {
//			for (int i = 0; i < equipmentItems.Length; i++)
//				equipmentItems[i] = 1 + i*10;
//
//			WWWForm form = new WWWForm();
//			GameData.Team.Player.ID = GameData.DPlayers[currentPlayer+1].ID;
//			GameData.Team.Player.Name = labelName.text;
//			GameData.Team.Player.Avatar = tAvatar[currentPlayer];
//			form.AddField("PlayerID", GameData.Team.Player.ID);
//			form.AddField("Name", GameData.Team.Player.Name);
//			form.AddField("Items", JsonConvert.SerializeObject(equipmentItems));
//			
//			SendHttp.Get.Command(URLConst.CreateRole, waitCreateRole, form, true);
//		}
//	}

//	private void waitCreateRole(bool ok, WWW www)
//    {
//	    if(!ok)
//            return;
//
//	    GameData.Team.Player.Init();
//	    GameData.SaveTeam();
//	    Hide();
//
//	    if (SceneMgr.Get.CurrentScene != SceneName.Lobby)
//	        SceneMgr.Get.ChangeLevel(SceneName.Lobby);
//	    else
//	        LobbyStart.Get.EnterLobby();
//    }

//	private void init()
//    {
//		playerCount = 3;
//		limitAngle = new float[playerCount];
//		tAvatar = new TAvatar[playerCount];
//		for(int i=0; i<playerCount; i++) {
//			if (GameData.DPlayers.ContainsKey(i+1)) {
//				limitAngle[i] = (360 / playerCount) * i;
//
//				GameObject player = new GameObject();
//				TPlayer p = new TPlayer(0);
//				p.ID = i+1;
//				p.SetAvatar();
//				player.name = i.ToString();
//				player.transform.parent = playerPos[i].transform;
//				tAvatar[i] = p.Avatar;
//				ModelManager.Get.SetAvatar(ref player, p.Avatar, GameData.DPlayers[p.ID].BodyType, false);
//				player.transform.localPosition = new Vector3(0, -1, 0);
//				player.transform.localEulerAngles = new Vector3(0, 180, 0);
//				player.transform.localScale = Vector3.one;
//				for (int j=0; j<player.transform.childCount; j++) { 
//					if(player.transform.GetChild(j).name.Contains("PlayerMode")) {
//						player.transform.GetChild(j).localScale = Vector3.one;
//						player.transform.GetChild(j).localEulerAngles = Vector3.zero;
//						player.transform.GetChild(j).localPosition = Vector3.zero;
//					}
//				}
//			}
//		}
//	}

//	private int findNearPlayer(float vy){
//		int index = 0;
//		float temp = 0;
//		if(vy < 0) 
//			vy = 360 + vy;
//		for (int i=0; i<playerCount; i++){
//			float minusValue = Mathf.Abs(vy - limitAngle[i]);
//			if (i==0)
//				temp = vy;
//			else {
//				if((360 - vy) < 30) 
//					index = 0;
//				else
//				if(minusValue < temp) {
//					temp = minusValue;
//					index = i;
//				}
//			}
//		}
//		return index;
//	}

//	private void resetPlayerEuler()
//    {
//		for (int i=0; i<playerCount; i++) {
//			playerPos[i].transform.localEulerAngles = Vector3.zero;
//			playerPos[i].transform.LookAt(playerPos[i].transform.position + new Vector3(1, 0, 0) , Vector3.up);
//		}
//	}

//    void FixedUpdate()
//    {
//		if(isRotateLeft && !isRotateRight) {
//			playerPos[currentPlayer].transform.Rotate(new Vector3(0,2,0));
//		} else if(!isRotateLeft && isRotateRight) {
//			playerPos[currentPlayer].transform.Rotate(new Vector3(0,-2,0));
//		}
//		if(Input.GetMouseButton(0)) {
//			axisX = 0;
//			if(Input.mousePosition.y > (Screen.height * 0.4f))
//				isDrag = true;
//			else
//				isTouchRotate = false;
//		} else {
//			axisX = 0;
//			isDrag = false;
//			if(isTouchRotate) {
//				currentPlayer = findNearPlayer(playerCenter.transform.localEulerAngles.y);
//				if (currentPlayer >= 0 && currentPlayer < limitAngle.Length && currentPlayer < tAvatar.Length){
//					float angle = limitAngle[currentPlayer];
//					playerCenter.transform.DOLocalRotate(new Vector3(0, angle, 0), 0.2f).OnUpdate(resetPlayerEuler);
//				}
//			}
//			isTouchRotate = false;
//		}
//		if(isDrag){
//			#if UNITY_EDITOR
//				axisX = -Input.GetAxis ("Mouse X");
//			#else
//			#if UNITY_IOS
//				if(Input.touchCount > 0)
//					axisX = -Input.touches[0].deltaPosition.x;
//			#endif
//			#if UNITY_ANDROID
//				if(Input.touchCount > 0)
//					axisX = -Input.touches[0].deltaPosition.x;
//			#endif
//			#if (!UNITY_IOS && !UNITY_ANDROID)
//				axisX = -Input.GetAxis ("Mouse X");
//			#endif
//			#endif
//			playerCenter.transform.Rotate(new Vector3(0, axisX, 0), Space.Self);
//
//			if(axisX != 0) {
//				isTouchRotate = true;
//				resetPlayerEuler();
//			}
//		}
//	}
    public static GameObject CreateModel(EPlayerPostion pos, Transform parent)
    {
        TPlayer p;
        if(pos == EPlayerPostion.G)
            p = new TPlayer(0) {ID = 10};
        else if(pos == EPlayerPostion.F)
            p = new TPlayer(0) { ID = 20 };
        else if(pos == EPlayerPostion.C)
            p = new TPlayer(0) { ID = 30 };
        else
            throw new InvalidEnumArgumentException(pos.ToString());
        p.SetAvatar();

        GameObject model = new GameObject {name = pos.ToString()};
        ModelManager.Get.SetAvatar(ref model, p.Avatar, GameData.DPlayers[p.ID].BodyType, false);

        model.transform.parent = parent;
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;
        model.layer = LayerMask.NameToLayer("UI");
        foreach(Transform child in model.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
        }

        return model;
    }

    public static GameObject CreateModel(Transform parent, string name, int playerID, int color, int hair, 
                                         int cloth, int pants, int shoes)
    {
        TPlayer player = new TPlayer(0) { ID = playerID };
        player.SetAvatar();

        player.Avatar.Body = color;
        player.Avatar.Hair = hair;
        player.Avatar.Cloth = cloth;
        player.Avatar.Pants = pants;
        player.Avatar.Shoes = shoes;

//        Debug.LogFormat("Player:{0}, Avatar:{1}", player, player.Avatar);

        GameObject model = new GameObject { name = name };
        ModelManager.Get.SetAvatar(ref model, player.Avatar, GameData.DPlayers[player.ID].BodyType, false);

        model.transform.parent = parent;
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;
        model.layer = LayerMask.NameToLayer("UI");
        foreach (Transform child in model.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
        }

        return model;
    }
}
