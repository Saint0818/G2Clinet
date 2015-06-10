using UnityEngine;
using System.Collections;
using GameStruct;
using System;
using System.Collections.Generic;
using DG.Tweening; 

public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";
	public GameObject PlayerInfoModel = null;
	private TAvatar[]  AvatarAy;
	private Vector3 [] Ay = new Vector3[3];
	private GameObject Select;
	private GameObject[] BtnAy = new GameObject[6];
	private GameObject RoleInfo;
	private TPlayer [] PlayerAy = new TPlayer[3];
	private GameObject [] PlayerObjAy = new GameObject[3];
	private int [] SelectIDAy = new int[3];
	private GameObject ViewLoading;
	private GameObject OkBtn;
	private GameObject InfoRange;
	private GameObject Left;
	private GameObject CharacterInfo;
	private UILabel CharacterInfoLabel;
	private int MaxValue = 100;
	private float Value = 0;
	private float axisX;
	private UILabel PlayerName;
	private GameObject PlayerNameObj;
	private UILabel PlayerBody;
	private GameObject PlayerBodyObj;
	private UILabel [] SelectABName = new UILabel[2];
	private UILabel [] SelectABBody = new UILabel[2];
	public static int [] RoleIDAy = new int[6]{14, 24, 34, 19, 29, 39};  // playerID

	private UILabel [] SelectAListName = new UILabel[3];
	private UILabel [] SelectAListBody = new UILabel[3];

	private UILabel [] SelectBListName = new UILabel[3];
	private UILabel [] SelectBListBody = new UILabel[3];
	private int [] UnSelectIDAy = new int[3];
	private Animator [] animatorAy = new Animator[3];
	private string [] AnimatorNameAy = new string[3]{"Move", "Dunk0", "Layup0"};

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
		Ay [0] = new Vector3 (0, 0, 0);
		Ay [1] = new Vector3 (3, 0, 0);
		Ay [2] = new Vector3 (-3, 0, 0);
		Select = GameObject.Find (UIName + "/Left/Select");

		for (int i = 0; i < 6; i++) 
		{
			SetBtnFun(UIName + "/Left/SelectCharacter/Button" + i.ToString(), SelectRole);
			BtnAy[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString());
		}

		SetBtnFun (UIName + "/Right/CharacterCheck", DoSelectRole);
		Left = GameObject.Find (UIName + "/Left");
		OkBtn = GameObject.Find (UIName + "/Right/CharacterCheck");
		RoleInfo = GameObject.Find (UIName + "/Center/TouchInfo");
		InfoRange = GameObject.Find (UIName + "/Right/InfoRange");
		ViewLoading = GameObject.Find (UIName + "/Center/ViewLoading");
		CharacterInfo = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo");
		CharacterInfoLabel = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/Label").GetComponent<UILabel>();
		CharacterInfoLabel.text = "";
		CharacterInfo.SetActive (false);

		UIEventListener.Get(GameObject.Find(UIName + "/Right/InfoRange/AttributeHexagon")).onClick = OnClickSixAttr;
		PlayerName = GameObject.Find (UIName + "/Right/InfoRange/PlayerName/Label").GetComponent<UILabel>();
		PlayerNameObj = GameObject.Find (UIName + "/Right/InfoRange/PlayerName");
		PlayerNameObj.SetActive (true);

		PlayerBody = GameObject.Find (UIName + "/Right/InfoRange/BodyType/Label").GetComponent<UILabel>();
		PlayerBodyObj = GameObject.Find (UIName + "/Right/InfoRange/BodyType");
		PlayerBodyObj.SetActive (true);

		SelectABName[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/PlayerNameA/Label").GetComponent<UILabel>();
		SelectABBody[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/BodyTypeA/Label").GetComponent<UILabel>();

		SelectABName[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/PlayerNameB/Label").GetComponent<UILabel>();
		SelectABBody[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/BodyTypeB/Label").GetComponent<UILabel>();

		for(int i = 0; i < SelectAListName.Length; i++)
		{
			SelectAListName [i] = GameObject.Find (UIName + "/Center/ViewLoading/SelectA/ListA/" + i.ToString() + "/PlayerName").GetComponent<UILabel>();
			SelectAListBody [i] = GameObject.Find (UIName + "/Center/ViewLoading/SelectA/ListA/" + i.ToString() + "/BodyType").GetComponent<UILabel>();
			SetBtnFun(UIName + "/Center/ViewLoading/SelectA/ListA/" + i.ToString(), DoListA);
		}

		for(int i = 0; i < SelectBListName.Length; i++)
		{
			SelectBListName [i] = GameObject.Find (UIName + "/Center/ViewLoading/SelectB/ListB/" + i.ToString() + "/PlayerName").GetComponent<UILabel>();
			SelectBListBody [i] = GameObject.Find (UIName + "/Center/ViewLoading/SelectB/ListB/" + i.ToString() + "/BodyType").GetComponent<UILabel>();
			SetBtnFun(UIName + "/Center/ViewLoading/SelectB/ListB/" + i.ToString(), DoListB);
		}
		SetBtnFun (UIName + "/Center/ViewLoading/GameStart", DoStart);


		float W = Screen.width;
		float H = Screen.height;
		float WH = W / H;
		if(WH >= 1.33f && WH <= 1.34f)
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 34));
		else if(WH >= 1.59f && WH <= 1.61f)
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 28.3f));
		else if(WH >= 1.66f && WH <= 1.67f)
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 27f));
		else if(WH >= 1.7f && WH <= 1.71f)
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 26.5f));
		else
			UITriangle.Get.CreateSixAttr (new Vector3(7, -0.9f, 25.3f));
	}

	public void OnClickSixAttr(GameObject obj)
	{
		//CharacterInfo.SetActive (!CharacterInfo.activeInHierarchy);
	}

	public void DoStart()
	{
		SceneMgr.Get.ChangeLevel (SceneName.Court_0);
	}

	public void DoListA()
	{
		int Index;
		if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out Index))
		{
			int SelectIndex = 0;
			for(int i = 0; i < RoleIDAy.Length; i++)
			{
				if(RoleIDAy[i] == UnSelectIDAy[Index])
				{
					SelectIndex = i;
					break;
				}
			}

			SetPlayerAvatar(1, SelectIndex);

			if(GameData.DPlayers.ContainsKey(SelectIDAy[1]))
			{
				GameData.TeamMembers[0].Player.ID = GameData.DPlayers[SelectIDAy[1]].ID;
				GameData.TeamMembers[0].Player.Name = GameData.DPlayers[SelectIDAy[1]].Name;
				GameData.TeamMembers[0].Player.AILevel = GameData.DPlayers[SelectIDAy[1]].AILevel;
			}

			int mIndex = 0;
			for(int j = 0; j < RoleIDAy.Length; j++)
			{
				if(RoleIDAy[j] != GameData.Team.Player.ID &&
				   RoleIDAy[j] != GameData.TeamMembers[0].Player.ID &&
				   RoleIDAy[j] != GameData.TeamMembers[1].Player.ID)
				{
					if(GameData.DPlayers.ContainsKey(RoleIDAy[j]))
					{
						UnSelectIDAy[mIndex] = RoleIDAy[j];
						mIndex++;
					}
				}
			}

			for(int i = 0; i < SelectAListName.Length; i++)
			{
				SelectAListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
				SelectAListBody [i].text = TextConst.S(GameData.DPlayers [UnSelectIDAy [i]].BodyType + 7);
				SelectBListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
				SelectBListBody [i].text = TextConst.S(GameData.DPlayers [UnSelectIDAy [i]].BodyType + 7);
			}
		}
	}

	public void DoListB()
	{
		int Index;
		if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out Index))
		{
			int SelectIndex = 0;
			for(int i = 0; i < RoleIDAy.Length; i++)
			{
				if(RoleIDAy[i] == UnSelectIDAy[Index])
				{
					SelectIndex = i;
					break;
				}
			}
			
			SetPlayerAvatar(2, SelectIndex);
			
			if(GameData.DPlayers.ContainsKey(SelectIDAy[2]))
			{
				GameData.TeamMembers[1].Player.ID = GameData.DPlayers[SelectIDAy[2]].ID;
				GameData.TeamMembers[1].Player.Name = GameData.DPlayers[SelectIDAy[2]].Name;
				GameData.TeamMembers[1].Player.AILevel = GameData.DPlayers[SelectIDAy[2]].AILevel;
			}
			
			int mIndex = 0;
			for(int j = 0; j < RoleIDAy.Length; j++)
			{
				if(RoleIDAy[j] != GameData.Team.Player.ID &&
				   RoleIDAy[j] != GameData.TeamMembers[0].Player.ID &&
				   RoleIDAy[j] != GameData.TeamMembers[1].Player.ID)
				{
					if(GameData.DPlayers.ContainsKey(RoleIDAy[j]))
					{
						UnSelectIDAy[mIndex] = RoleIDAy[j];
						mIndex++;
					}
				}
			}

			for(int i = 0; i < SelectAListName.Length; i++)
			{
				SelectAListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
				SelectAListBody [i].text = TextConst.S(GameData.DPlayers [UnSelectIDAy [i]].BodyType + 7);
				SelectBListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
				SelectBListBody [i].text = TextConst.S(GameData.DPlayers [UnSelectIDAy [i]].BodyType + 7);
			}
		}
	}

	public void DoSelectRole()
	{
		UITriangle.Get.Triangle.SetActive (false);
		InfoRange.SetActive (false);
		OkBtn.SetActive (false);
		Left.SetActive (false);
		PlayerNameObj.SetActive (false);
		PlayerBodyObj.SetActive (false);
		ViewLoading.SetActive (true);
		PlayerObjAy[0].transform.localEulerAngles = new Vector3(0, 180, 0);

		int RanID;
		int Count;
		for(int i = 1; i < Ay.Length; i++) 
		{
			PlayerObjAy[i].SetActive(true);

			RanID = UnityEngine.Random.Range(0, RoleIDAy.Length - i);
			Count = 0;

			for(int j = 0; j < RoleIDAy.Length; j++)
			{
				if(GameData.DPlayers.ContainsKey(RoleIDAy[j]))
				{
					if(i == 1)
					{
						if(RoleIDAy[j] != SelectIDAy[0])
						{
							if(Count == RanID)
							{
								SetPlayerAvatar(i, j);
								break;
							}
							else
								Count++;
						}
					}
					else
					{
						if(RoleIDAy[j] != SelectIDAy[0] && RoleIDAy[j] != SelectIDAy[1])
						{
							if(Count == RanID)
							{
								SetPlayerAvatar(i, j);
								break;
							}
							else
								Count++;
						}
					}
				}
			}
		}

		for(int i = 0; i < SelectIDAy.Length; i++) 
		{
			if(i == 0)
			{
				if(GameData.DPlayers.ContainsKey(SelectIDAy[i]))
				{
					GameData.Team.Player.ID = GameData.DPlayers[SelectIDAy[i]].ID;
					GameData.Team.Player.Name = GameData.DPlayers[SelectIDAy[i]].Name;
					GameData.Team.Player.AILevel = GameData.DPlayers[SelectIDAy[i]].AILevel;
				}
			}
			else
			{
				if(GameData.DPlayers.ContainsKey(SelectIDAy[i]))
				{
					GameData.TeamMembers[i - 1].Player.ID = GameData.DPlayers[SelectIDAy[i]].ID;
					GameData.TeamMembers[i - 1].Player.Name = GameData.DPlayers[SelectIDAy[i]].Name;
					GameData.TeamMembers[i - 1].Player.AILevel = GameData.DPlayers[SelectIDAy[i]].AILevel;
				}
			}
		}



		int Index = 0;
		for(int j = 0; j < RoleIDAy.Length; j++)
		{
			if(RoleIDAy[j] != GameData.Team.Player.ID &&
			   RoleIDAy[j] != GameData.TeamMembers[0].Player.ID &&
			   RoleIDAy[j] != GameData.TeamMembers[1].Player.ID)
			{
				if(GameData.DPlayers.ContainsKey(RoleIDAy[j]))
				{
					UnSelectIDAy[Index] = RoleIDAy[j];
					Index++;
				}
			}
		}


		for(int i = 0; i < SelectAListName.Length; i++)
		{
			SelectAListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
			SelectAListBody [i].text = TextConst.S(GameData.DPlayers [UnSelectIDAy [i]].BodyType + 7);
			SelectBListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
			SelectBListBody [i].text = TextConst.S(GameData.DPlayers [UnSelectIDAy [i]].BodyType + 7);
		}
	}

	public void SelectRole()
	{
		int Index;
		if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out Index))
		{
			Select.transform.localPosition = new Vector3(BtnAy[Index].transform.localPosition.x, 
			                                             BtnAy[Index].transform.localPosition.y,
			                                             BtnAy[Index].transform.localPosition.z);

			SetPlayerAvatar(0, Index);
			PlayerObjAy[0].transform.localEulerAngles = new Vector3(0, 180, 0);
			PlayAnimator(0, AnimatorNameAy[UnityEngine.Random.Range(0, AnimatorNameAy.Length)]);
			if(GameData.DPlayers.ContainsKey(SelectIDAy [0]))
			{
				TGreatPlayer data = GameData.DPlayers[SelectIDAy [0]];
				
				Value = data.Strength + data.Block;
				UITriangle.Get.ChangeValue (0, Value / 2 / MaxValue);
				
				Value = data.Defence + data.Steal;
				UITriangle.Get.ChangeValue (1, Value / 2 / MaxValue);
				
				Value = data.Dribble + data.Pass;
				UITriangle.Get.ChangeValue (2, Value / 2 / MaxValue);
				
				Value = data.Point2 + data.Point3;
				UITriangle.Get.ChangeValue (3, Value / 2 / MaxValue);
				
				Value = data.Speed + data.Stamina;
				UITriangle.Get.ChangeValue (4, Value / 2 / MaxValue);
				
				Value = data.Rebound + data.Dunk;
				UITriangle.Get.ChangeValue (5, Value / 2 / MaxValue);
			}
		}
	}

	private void SetPlayerAvatar(int RoleIndex, int Index)
	{
		PlayerAy[RoleIndex].ID = RoleIDAy[Index];
		PlayerAy [RoleIndex].AILevel = GameData.DPlayers [RoleIDAy [Index]].AILevel;
		SelectIDAy[RoleIndex] = RoleIDAy[Index];
		PlayerAy[RoleIndex].SetAvatar();
		AvatarAy[RoleIndex] = PlayerAy[RoleIndex].Avatar;
		ModelManager.Get.SetAvatar(ref PlayerObjAy[RoleIndex], PlayerAy[RoleIndex].Avatar, GameData.DPlayers [RoleIDAy [Index]].BodyType, false, false);
		animatorAy[RoleIndex] = PlayerObjAy[RoleIndex].GetComponent<Animator>();
		ChangeLayersRecursively(PlayerObjAy[RoleIndex].transform, "UI");

		switch(RoleIndex)
		{
		case 0:
			PlayerName.text = GameData.DPlayers [RoleIDAy [Index]].Name;
			PlayerBody.text = TextConst.S(GameData.DPlayers [RoleIDAy [Index]].BodyType + 7);
			break;
		case 1:
		case 2:
			SelectABName[RoleIndex - 1].text = GameData.DPlayers [RoleIDAy [Index]].Name;
			SelectABBody[RoleIndex - 1].text = TextConst.S(GameData.DPlayers [RoleIDAy [Index]].BodyType + 7);
			break;
		}

	}

	private void PlayAnimator(int Index, string name)
	{
		if(Index == 0 && Index < animatorAy.Length && name != "" && animatorAy [Index] != null)
			animatorAy [Index].SetTrigger (name);
	}
	
	protected override void InitData() 
	{
		AvatarAy = new GameStruct.TAvatar[Ay.Length];
		SelectIDAy [0] = RoleIDAy[0];
		for(int i = 0; i < Ay.Length; i++) 
		{
			PlayerObjAy[i] = new GameObject();
			PlayerAy[i] = new TPlayer(0);
			PlayerAy[i].ID = RoleIDAy[i];
			PlayerAy[i].SetAvatar();
			PlayerObjAy[i].name = i.ToString();
			PlayerObjAy[i].transform.parent = PlayerInfoModel.transform;
			AvatarAy[i] = PlayerAy[i].Avatar;
			ModelManager.Get.SetAvatar(ref PlayerObjAy[i], PlayerAy[i].Avatar, GameData.DPlayers[UISelectRole.RoleIDAy[i]].BodyType, false, false);
			animatorAy[i] = PlayerObjAy[i].GetComponent<Animator>();
			PlayerObjAy[i].transform.localPosition = Ay[i];
			PlayerAy [i].AILevel = GameData.DPlayers [RoleIDAy [i]].AILevel;
			PlayerObjAy[i].AddComponent<SelectEvent>();
			if(i == 0)
			{
				PlayerObjAy[i].transform.localPosition = new Vector3(0, -0.65f, 0);
				PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, 180, 0);
				PlayerName.text = GameData.DPlayers [PlayerAy[i].ID].Name;
				PlayerBody.text = TextConst.S(GameData.DPlayers [PlayerAy[i].ID].BodyType + 7);
			}
			else if(i == 1)
			{
				PlayerObjAy[i].transform.localPosition = new Vector3(0.42f, -0.33f, 2.87f);
				PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, 150, 0);
			}
			else if(i == 2)
			{
				PlayerObjAy[i].transform.localPosition = new Vector3(-0.4f, -0.35f, 2.58f);
				PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, -150, 0);
			}

			if(i == 0)
				PlayerObjAy[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			else
				PlayerObjAy[i].transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

			ChangeLayersRecursively(PlayerObjAy[i].transform, "UI");
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

		ViewLoading.SetActive (false);
		for(int i = 1; i < Ay.Length; i++) 		
			PlayerObjAy[i].SetActive(false);

		if(GameData.DPlayers.ContainsKey(SelectIDAy [0]))
		{
			TGreatPlayer data = GameData.DPlayers[SelectIDAy [0]];
			
			Value = data.Strength + data.Block;
			UITriangle.Get.ChangeValue (0, Value / 2 / MaxValue);
			
			Value = data.Defence + data.Steal;
			UITriangle.Get.ChangeValue (1, Value / 2 / MaxValue);
			
			Value = data.Dribble + data.Pass;
			UITriangle.Get.ChangeValue (2, Value / 2 / MaxValue);
			
			Value = data.Point2 + data.Point3;
			UITriangle.Get.ChangeValue (3, Value / 2 / MaxValue);
			
			Value = data.Speed + data.Stamina;
			UITriangle.Get.ChangeValue (4, Value / 2 / MaxValue);
			
			Value = data.Rebound + data.Dunk;
			UITriangle.Get.ChangeValue (5, Value / 2 / MaxValue);
		}
	}

	private void ChangeLayersRecursively(Transform trans, string name)
	{
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans)
		{            
			ChangeLayersRecursively(child, name);
		}
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	void FixedUpdate()
	{
		if(OkBtn.activeInHierarchy)
		{
			if(Input.GetMouseButton(0)) 
			{
				axisX = 0;

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
				PlayerObjAy[0].transform.Rotate(new Vector3(0, axisX, 0), Space.Self);
			} 
		}
	}
}

