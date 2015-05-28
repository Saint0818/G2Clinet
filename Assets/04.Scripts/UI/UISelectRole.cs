using UnityEngine;
using System.Collections;
using GameStruct;

public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";
	public GameObject PlayerInfoModel = null;
	public GameObject UITouchDrog;
	private TAvatar[]  AvatarAy;
	private Vector3 [] Ay = new Vector3[3];
	private GameObject Select;
	private GameObject[] BtnAy = new GameObject[6];
	private GameObject RandomBtn;
	private GameObject RoleInfo;
	private TPlayer [] PlayerAy = new TPlayer[3];
	private GameObject [] PlayerObjAy = new GameObject[3];
	private int [] SelectIDAy = new int[3];

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
			if (isShow)
				Get.Show(isShow);
	}
	
	public static UISelectRole Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISelectRole;
			
			return instance;
		}
	}
	
	protected override void InitCom() 
	{
		PlayerInfoModel = new GameObject();
		PlayerInfoModel.name = "PlayerInfoModel";
		Ay [0] = new Vector3 (-3.5f, 0, 0);
		Ay [1] = new Vector3 (0, 0, 3);
		Ay [2] = new Vector3 (0, 0, -3);
		Select = GameObject.Find (UIName + "/Bottom/select");

		UITouchDrog = GameObject.Find(UIName + "/Center/TouchRange");
		if(UITouchDrog)
			UIEventListener.Get(UITouchDrog).onClick = OnClickPlayer;

		for (int i = 0; i < 6; i++) 
		{
			SetBtnFun(UIName + "/Bottom/SelectCharacter/Button" + i.ToString(), SelectRole);
			BtnAy[i] = GameObject.Find(UIName + "/Bottom/SelectCharacter/Button" + i.ToString());
		}

		SetBtnFun(UIName + "/Bottom/SelectCharacter/ButtonRandom", RandomRole);
		RandomBtn = GameObject.Find (UIName + "/Bottom/SelectCharacter/ButtonRandom");
		RoleInfo = GameObject.Find (UIName + "/Center/TouchInfo");
	}

	int SelectRoleIndex = 0;
	public void RandomRole()
	{
		Select.transform.localPosition = new Vector3(RandomBtn.transform.localPosition.x - 90, 
		                                             RandomBtn.transform.localPosition.y,
		                                             RandomBtn.transform.localPosition.z);

		PlayerAy[SelectRoleIndex].ID = UnityEngine.Random.Range(0, GameData.DPlayers.Count) + 1;
		PlayerAy[SelectRoleIndex].SetAvatar();
		AvatarAy[SelectRoleIndex] = PlayerAy[SelectRoleIndex].Avatar;
		ModelManager.Get.SetAvatar(ref PlayerObjAy[SelectRoleIndex], PlayerAy[SelectRoleIndex].Avatar, false);
	}

	public void SelectRole()
	{
		int Index;
		if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out Index))
		{
			Select.transform.localPosition = new Vector3(BtnAy[Index].transform.localPosition.x - 90, 
			                                             BtnAy[Index].transform.localPosition.y,
			                                             BtnAy[Index].transform.localPosition.z);

			if(SelectIDAy[SelectRoleIndex] == 0 || SelectIDAy[SelectRoleIndex] != Index + 1)
			{
				bool same = false;
				for(int i = 0; i < SelectIDAy.Length; i++)
				{
					if(SelectIDAy[i] == (Index + 1))
					{
						same = true;
						break;
					}
				}

				if(!same)
				{
					PlayerAy[SelectRoleIndex].ID = Index + 1;
					SelectIDAy[SelectRoleIndex] = Index + 1;
					PlayerAy[SelectRoleIndex].SetAvatar();
					AvatarAy[SelectRoleIndex] = PlayerAy[SelectRoleIndex].Avatar;
					ModelManager.Get.SetAvatar(ref PlayerObjAy[SelectRoleIndex], PlayerAy[SelectRoleIndex].Avatar, false);
				}
			}
		}
	}
	
	protected override void InitData() 
	{
		AvatarAy = new GameStruct.TAvatar[Ay.Length];
		for(int i = 0; i < Ay.Length; i++) 
		{
			PlayerObjAy[i] = new GameObject();
			PlayerAy[i] = new TPlayer();
			PlayerAy[i].ID = i + 1;
			PlayerAy[i].SetAvatar();
			PlayerObjAy[i].name = i.ToString();
			PlayerObjAy[i].transform.parent = PlayerInfoModel.transform;
			AvatarAy[i] = PlayerAy[i].Avatar;
			ModelManager.Get.SetAvatar(ref PlayerObjAy[i], PlayerAy[i].Avatar, false);
			PlayerObjAy[i].transform.localPosition = Ay[i];
			PlayerObjAy[i].transform.LookAt(new Vector3(0 -90, 0));
			PlayerObjAy[i].transform.localScale = Vector3.one;
			PlayerObjAy[i].tag = "Player";
			PlayerObjAy[i].layer = LayerMask.NameToLayer(PlayerObjAy[i].tag);
			SelectIDAy[i] = i + 1;

			for (int j = 0; j <PlayerObjAy[i].transform.childCount; j++) 
			{ 
				if(PlayerObjAy[i].transform.GetChild(j).name.Contains("PlayerMode")) 
				{
					PlayerObjAy[i].transform.GetChild(j).localScale = Vector3.one;
					PlayerObjAy[i].transform.GetChild(j).localEulerAngles = Vector3.zero;
					PlayerObjAy[i].transform.GetChild(j).localPosition = Vector3.zero;
				}
			}
		}
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	void FixedUpdate()
	{

	}

	private void OnClickPlayer(GameObject obj)
	{
		GameObject go = CameraMgr.Get.GetTouch (8);

		if (go) 
		{
			int idx = int.Parse(go.name);
			SelectRoleIndex = idx;
		} 
	}
}

