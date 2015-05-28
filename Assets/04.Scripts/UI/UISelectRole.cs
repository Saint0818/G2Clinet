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

	public void RandomRole()
	{
		Select.transform.localPosition = new Vector3(RandomBtn.transform.localPosition.x - 90, 
		                                             RandomBtn.transform.localPosition.y,
		                                             RandomBtn.transform.localPosition.z);
	}

	public void SelectRole()
	{
		int Index;
		if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out Index))
		{
			Select.transform.localPosition = new Vector3(BtnAy[Index].transform.localPosition.x - 90, 
			                                             BtnAy[Index].transform.localPosition.y,
			                                             BtnAy[Index].transform.localPosition.z);
		}
	}
	
	protected override void InitData() 
	{
		AvatarAy = new GameStruct.TAvatar[Ay.Length];
		for(int i = 0; i < Ay.Length; i++) 
		{
			GameObject player = new GameObject();
			GameStruct.TPlayer p = new GameStruct.TPlayer();
			p.ID = i + 1;
			p.SetAvatar();
			player.name = i.ToString();
			player.transform.parent = PlayerInfoModel.transform;
			AvatarAy[i] = p.Avatar;
			ModelManager.Get.SetAvatar(ref player, p.Avatar, false);
			player.transform.localPosition = Ay[i];
			player.transform.LookAt(new Vector3(0 -90, 0));
			player.transform.localScale = Vector3.one;
			player.tag = "Player";
			player.layer = LayerMask.NameToLayer(player.tag);

			for (int j = 0; j <player.transform.childCount; j++) 
			{ 
				if(player.transform.GetChild(j).name.Contains("PlayerMode")) 
				{
					player.transform.GetChild(j).localScale = Vector3.one;
					player.transform.GetChild(j).localEulerAngles = Vector3.zero;
					player.transform.GetChild(j).localPosition = Vector3.zero;
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
		}
	}
}

