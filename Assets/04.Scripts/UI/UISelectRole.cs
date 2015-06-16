using UnityEngine;
using System.Collections;
using GameStruct;
using System;
using System.Collections.Generic;
using DG.Tweening; 

public struct TSelectAttrData
{
	public UISlider Slider;
	public UILabel Value;
}

public class UISelectRole : UIBase {
	private static UISelectRole instance = null;
	private const string UIName = "UISelectRole";
	public GameObject PlayerInfoModel = null;
	private TAvatar[]  AvatarAy;
	private Vector3 [] Ay = new Vector3[3];
	private GameObject Select;
	private GameObject[] BtnAy = new GameObject[6];
	private TPlayer [] PlayerAy = new TPlayer[3];
	private GameObject [] PlayerObjAy = new GameObject[3];
	private int [] SelectIDAy = new int[3];
	private GameObject ViewLoading;
	private GameObject OkBtn;
	private GameObject InfoRange;
	private GameObject Left;
	private GameObject CharacterInfo;
	private int MaxValue = 100;
	private float Value = 0;
	private float axisX;
	private UILabel PlayerName;
	private GameObject PlayerNameObj;
	private UISprite PlayerBodyPic;
	private GameObject PlayerBodyObj;
	private UILabel [] SelectABName = new UILabel[2];
	private UISprite [] SelectABBody = new UISprite[2];
	public static int [] RoleIDAy = new int[6]{14, 24, 34, 19, 29, 39};  // playerID

	private UILabel [] SelectAListName = new UILabel[3];
	private UISprite [] SelectAListPic = new UISprite[3];
	private UILabel [] SelectBListName = new UILabel[3];
	private UISprite [] SelectBListPic = new UISprite[3];
	private int [] UnSelectIDAy = new int[3];
	private Animator [] animatorAy = new Animator[3];
	private string [] AnimatorNameAy = new string[1]{""};
	private TSelectAttrData [] SelectAttrDataAy = new TSelectAttrData[12];
	private UISprite [] LineAy = new UISprite[6];
	private UISprite MusicOn;

	private float [] OldValueAy = new float[12];
	private float [] NewValueAy = new float[12];

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
			LineAy[i] = GameObject.Find(UIName + "/Left/SelectCharacter/Button" + i.ToString() + "/SpriteLine").GetComponent<UISprite>();
			if(i == 0)
				LineAy[i].fillAmount = 1;
			else
				LineAy[i].fillAmount = 0;
		}

		MusicOn = GameObject.Find (UIName + "/Right/MusicSwitch/ButtonMusic/On").GetComponent<UISprite>();
		SetBtnFun (UIName + "/Right/MusicSwitch/ButtonMusic", DoControlMusic);
		SetBtnFun (UIName + "/Right/CharacterCheck", DoSelectRole);
		SetBtnFun (UIName + "/Center/ViewLoading/Back", DoBackToSelectMe);
		Left = GameObject.Find (UIName + "/Left");
		OkBtn = GameObject.Find (UIName + "/Right/CharacterCheck");
		InfoRange = GameObject.Find (UIName + "/Right/InfoRange");
		ViewLoading = GameObject.Find (UIName + "/Center/ViewLoading");
		CharacterInfo = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo");
		CharacterInfo.SetActive (false);

		SelectAttrDataAy [0].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/2Point").GetComponent<UISlider>();
		SelectAttrDataAy [0].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/2Point/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [1].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/3Point").GetComponent<UISlider>();
		SelectAttrDataAy [1].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/3Point/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [2].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Speed").GetComponent<UISlider>();
		SelectAttrDataAy [2].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Speed/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [3].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Stamina").GetComponent<UISlider>();
		SelectAttrDataAy [3].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Stamina/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [4].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Rebound").GetComponent<UISlider>();
		SelectAttrDataAy [4].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Rebound/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [5].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Dunk").GetComponent<UISlider>();
		SelectAttrDataAy [5].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Dunk/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [6].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Block").GetComponent<UISlider>();
		SelectAttrDataAy [6].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Block/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [7].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Strength").GetComponent<UISlider>();
		SelectAttrDataAy [7].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Strength/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [8].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Defence").GetComponent<UISlider>();
		SelectAttrDataAy [8].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Defence/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [9].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Steal").GetComponent<UISlider>();
		SelectAttrDataAy [9].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Steal/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [10].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Dribble").GetComponent<UISlider>();
		SelectAttrDataAy [10].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Dribble/LabelValue").GetComponent<UILabel>();
		SelectAttrDataAy [11].Slider = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Pass").GetComponent<UISlider>();
		SelectAttrDataAy [11].Value = GameObject.Find (UIName + "/Right/InfoRange/CharacterInfo/AttributeBar/Pass/LabelValue").GetComponent<UILabel>();

		UIEventListener.Get(GameObject.Find(UIName + "/Right/InfoRange/AttributeHexagon")).onClick = OnClickSixAttr;
		UIEventListener.Get(GameObject.Find(UIName + "/Right/InfoRange/CharacterInfo")).onClick = OnClickSixAttr;
		PlayerName = GameObject.Find (UIName + "/Right/InfoRange/PlayerName/Label").GetComponent<UILabel>();
		PlayerNameObj = GameObject.Find (UIName + "/Right/InfoRange/PlayerName");
		PlayerNameObj.SetActive (true);

		PlayerBodyPic = GameObject.Find (UIName + "/Right/InfoRange/BodyType/SpriteType").GetComponent<UISprite>();
		PlayerBodyObj = GameObject.Find (UIName + "/Right/InfoRange/BodyType");
		PlayerBodyObj.SetActive (true);

		SelectABName[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/PlayerNameA/Label").GetComponent<UILabel>();
		SelectABBody[0] = GameObject.Find(UIName + "/Center/ViewLoading/SelectA/PlayerNameA/SpriteTypeA").GetComponent<UISprite>();

		SelectABName[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/PlayerNameB/Label").GetComponent<UILabel>();
		SelectABBody[1] = GameObject.Find(UIName + "/Center/ViewLoading/SelectB/PlayerNameB/SpriteTypeB").GetComponent<UISprite>();

		for(int i = 0; i < SelectAListName.Length; i++)
		{
			SelectAListName [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString() + "/PlayerName").GetComponent<UILabel>();
			SelectAListPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString() + "/BodyType").GetComponent<UISprite>();
			SetBtnFun(UIName + "/Center/ViewLoading/PartnerList/ListA/UIGrid/" + i.ToString(), DoListA);
		}

		for(int i = 0; i < SelectBListName.Length; i++)
		{
			SelectBListName [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString() + "/PlayerName").GetComponent<UILabel>();
			SelectBListPic [i] = GameObject.Find (UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString() + "/BodyType").GetComponent<UISprite>();
			SetBtnFun(UIName + "/Center/ViewLoading/PartnerList/ListB/UIGrid/" + i.ToString(), DoListB);
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

	public void DoBackToSelectMe()
	{
		UITriangle.Get.Triangle.SetActive (true);
		InfoRange.SetActive (true);
		OkBtn.SetActive (true);
		Left.SetActive (true);
		PlayerNameObj.SetActive (true);
		PlayerBodyObj.SetActive (true);
		ViewLoading.SetActive (false);

		for(int i = 1; i < Ay.Length; i++) 	
			PlayerObjAy[i].SetActive(false);
	}

	public void DoControlMusic()
	{
		MusicOn.enabled = !MusicOn.enabled;
		AudioMgr.Get.Mute(!MusicOn.enabled);		
	}

	private void SetSubAttr(int Index, float Value)
	{
		SelectAttrDataAy [Index].Slider.value = 0;//Value / 100;
		SelectAttrDataAy [Index].Value.text = Value.ToString ();
	}

	private void SetAttr(TGreatPlayer data)
	{
		if(OldValueAy[0] == 0)
		{
			OldValueAy[0] = data.Point2;
			NewValueAy[0] = data.Point2;
			SetSubAttr(0, data.Point2);
			OldValueAy[1] = data.Point3;
			NewValueAy[1] = data.Point3;
			SetSubAttr(1, data.Point3);
			OldValueAy[2] = data.Speed;
			NewValueAy[2] = data.Speed;
			SetSubAttr(2, data.Speed);
			OldValueAy[3] = data.Stamina;
			NewValueAy[3] = data.Stamina;
			SetSubAttr(3, data.Stamina);
			OldValueAy[4] = data.Rebound;
			NewValueAy[4] = data.Rebound;
			SetSubAttr(4, data.Rebound);
			OldValueAy[5] = data.Dunk;
			NewValueAy[5] = data.Dunk;
			SetSubAttr(5, data.Dunk);
			OldValueAy[6] = data.Block;
			NewValueAy[6] = data.Block;
			SetSubAttr(6, data.Block);
			OldValueAy[7] = data.Strength;
			NewValueAy[7] = data.Strength;
			SetSubAttr(7, data.Strength);
			OldValueAy[8] = data.Defence;
			NewValueAy[8] = data.Defence;
			SetSubAttr(8, data.Defence);
			OldValueAy[9] = data.Steal;
			NewValueAy[9] = data.Steal;
			SetSubAttr(9, data.Steal);
			OldValueAy[10] = data.Dribble;
			NewValueAy[10] = data.Dribble;
			SetSubAttr(10, data.Dribble);
			OldValueAy[11] = data.Pass;
			NewValueAy[11] = data.Pass;
			SetSubAttr(11, data.Pass);
		}
		else
		{
			NewValueAy[0] = data.Point2;
			NewValueAy[1] = data.Point3;
			NewValueAy[2] = data.Speed;
			NewValueAy[3] = data.Stamina;
			NewValueAy[4] = data.Rebound;
			NewValueAy[5] = data.Dunk;
			NewValueAy[6] = data.Block;
			NewValueAy[7] = data.Strength;
			NewValueAy[8] = data.Defence;
			NewValueAy[9] = data.Steal;
			NewValueAy[10] = data.Dribble;
			NewValueAy[11] = data.Pass;
		}
	}

	public void OnClickSixAttr(GameObject obj)
	{
		CharacterInfo.SetActive (!CharacterInfo.activeInHierarchy);
	}

	public void DoStart()
	{
		SetEnemyMembers ();
		GameData.Team.Player.SetAttribute();
		GameData.Team.Player.SetAvatar();
		GameData.TeamMembers [0].Player.SetAttribute ();
		GameData.TeamMembers [0].Player.SetAvatar ();
		GameData.TeamMembers [1].Player.SetAttribute ();
		GameData.TeamMembers [1].Player.SetAvatar ();		
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
				SetBodyPic(ref SelectAListPic[i], GameData.DPlayers[UnSelectIDAy[i]].BodyType);
				SelectBListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
				SetBodyPic(ref SelectBListPic[i], GameData.DPlayers[UnSelectIDAy[i]].BodyType);
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

			for(int i = 0; i < SelectBListName.Length; i++)
			{
				SelectAListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
				SetBodyPic(ref SelectAListPic[i], GameData.DPlayers[UnSelectIDAy[i]].BodyType);
				SelectBListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
				SetBodyPic(ref SelectBListPic[i], GameData.DPlayers[UnSelectIDAy[i]].BodyType);
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
			SetBodyPic(ref SelectAListPic[i], GameData.DPlayers[UnSelectIDAy[i]].BodyType);
			SelectBListName [i].text = GameData.DPlayers[UnSelectIDAy[i]].Name;
			SetBodyPic(ref SelectBListPic[i], GameData.DPlayers[UnSelectIDAy[i]].BodyType);
		}
	}

	private int SelectRoleIndex = 0;
	public void SelectRole()
	{
		int Index;
		if(int.TryParse(UIButton.current.name[UIButton.current.name.Length - 1].ToString(), out Index))
		{
			Select.transform.localPosition = new Vector3(BtnAy[Index].transform.localPosition.x, 
			                                             BtnAy[Index].transform.localPosition.y,
			                                             BtnAy[Index].transform.localPosition.z);
			SelectRoleIndex = Index;
			for(int i = 0; i < LineAy.Length; i++)
				LineAy[i].fillAmount = 0;
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
				
				Value = data.Speed + data.Stamina;
				UITriangle.Get.ChangeValue (3, Value / 2 / MaxValue);

				Value = data.Point2 + data.Point3;
				UITriangle.Get.ChangeValue (4, Value / 2 / MaxValue);
				
				Value = data.Rebound + data.Dunk;
				UITriangle.Get.ChangeValue (5, Value / 2 / MaxValue);
				SetAttr(data);
			}
		}
	}

	public void SetBodyPic(ref UISprite Pic, int Type)
	{
		switch(Type)
		{
		case 0:
			Pic.spriteName = "L_namecard_CENTER";
			break;
		case 1:
			Pic.spriteName = "L_namecard_FORWARD";
			break;
		case 2:
			Pic.spriteName = "L_namecard_GUARD";
			break;
		default:
			Pic.spriteName = "L_namecard_CENTER";
			break;
		}
	}

	private void SetPlayerAvatar(int RoleIndex, int Index)
	{
		PlayerAy[RoleIndex].ID = RoleIDAy[Index];
		PlayerAy [RoleIndex].AILevel = GameData.DPlayers [RoleIDAy [Index]].AILevel;
		SelectIDAy[RoleIndex] = RoleIDAy[Index];
		PlayerAy[RoleIndex].SetAvatar();
		AvatarAy[RoleIndex] = PlayerAy[RoleIndex].Avatar;
		GameObject temp = PlayerObjAy [RoleIndex];

		ModelManager.Get.SetAvatar(ref PlayerObjAy[RoleIndex], PlayerAy[RoleIndex].Avatar, GameData.DPlayers [RoleIDAy [Index]].BodyType, false, false, true);


		PlayerObjAy[RoleIndex].name = RoleIndex.ToString();
		PlayerObjAy[RoleIndex].transform.parent = PlayerInfoModel.transform;
		PlayerObjAy[RoleIndex].transform.localPosition = Ay[RoleIndex];
		PlayerObjAy[RoleIndex].AddComponent<SelectEvent>();
		PlayerObjAy[RoleIndex].transform.localPosition = temp.transform.localPosition;
		PlayerObjAy[RoleIndex].transform.localEulerAngles = temp.transform.localEulerAngles;
		PlayerObjAy[RoleIndex].transform.localScale = temp.transform.localScale;
		for (int j = 0; j <PlayerObjAy[RoleIndex].transform.childCount; j++) 
		{ 
			if(PlayerObjAy[RoleIndex].transform.GetChild(j).name.Contains("PlayerMode")) 
			{
				PlayerObjAy[RoleIndex].transform.GetChild(j).localScale = Vector3.one;
				PlayerObjAy[RoleIndex].transform.GetChild(j).localEulerAngles = Vector3.zero;
				PlayerObjAy[RoleIndex].transform.GetChild(j).localPosition = Vector3.zero;
			}
		}


		animatorAy[RoleIndex] = PlayerObjAy[RoleIndex].GetComponent<Animator>();
		ChangeLayersRecursively(PlayerObjAy[RoleIndex].transform, "UI");

		switch(RoleIndex)
		{
		case 0:
			PlayerName.text = GameData.DPlayers [RoleIDAy [Index]].Name;
			SetBodyPic(ref PlayerBodyPic, GameData.DPlayers [RoleIDAy [Index]].BodyType);
			break;
		case 1:
		case 2:
			SelectABName[RoleIndex - 1].text = GameData.DPlayers [RoleIDAy [Index]].Name;
			SetBodyPic(ref SelectABBody[RoleIndex - 1], GameData.DPlayers [RoleIDAy [Index]].BodyType);
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
				PlayerObjAy[i].transform.localPosition = new Vector3(0, -0.8f, 0);
				PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, 180, 0);
				PlayerName.text = GameData.DPlayers [PlayerAy[i].ID].Name;
				SetBodyPic(ref PlayerBodyPic, GameData.DPlayers [PlayerAy[i].ID].BodyType);
			}
			else if(i == 1)
			{
				PlayerObjAy[i].transform.localPosition = new Vector3(0.42f, -0.6f, 2.87f);
				PlayerObjAy[i].transform.localEulerAngles = new Vector3(0, 150, 0);
			}
			else if(i == 2)
			{
				PlayerObjAy[i].transform.localPosition = new Vector3(-0.4f, -0.6f, 2.58f);
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

			Value = data.Speed + data.Stamina;
			UITriangle.Get.ChangeValue (3, Value / 2 / MaxValue);

			Value = data.Point2 + data.Point3;
			UITriangle.Get.ChangeValue (4, Value / 2 / MaxValue);
			
			Value = data.Rebound + data.Dunk;
			UITriangle.Get.ChangeValue (5, Value / 2 / MaxValue);
			SetAttr(data);
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
		if (SelectRoleIndex >= 0 && SelectRoleIndex < LineAy.Length) 
		{
			if(LineAy[SelectRoleIndex].fillAmount < 1)
				LineAy[SelectRoleIndex].fillAmount += 0.1f;		
		}

		for(int i = 0; i < OldValueAy.Length; i++)
		{
			if(OldValueAy[i] != NewValueAy[i])
			{
				if(OldValueAy[i] > NewValueAy[i])
				{
					SetSubAttr(i, OldValueAy[i] - 1);
					OldValueAy[i] -= 1;
				}
				else
				{
					SetSubAttr(i, OldValueAy[i] + 1);
					OldValueAy[i] += 1;
				}
			}
		}

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

	public void SetEnemyMembers()
	{
		int Index = 0;

		for(int j = 0; j < RoleIDAy.Length; j++)
		{
			if(RoleIDAy[j] != GameData.Team.Player.ID &&
			   RoleIDAy[j] != GameData.TeamMembers[0].Player.ID &&
			   RoleIDAy[j] != GameData.TeamMembers[1].Player.ID)
			{
				if(GameData.DPlayers.ContainsKey(RoleIDAy[j]))
				{
					GameData.EnemyMembers[Index].Player.ID = RoleIDAy[j];
					GameData.EnemyMembers[Index].Player.Name = GameData.DPlayers[RoleIDAy[j]].Name;
					GameData.EnemyMembers[Index].Player.SetAttribute();
					GameData.EnemyMembers[Index].Player.SetAvatar();
					Index++;
				}
			}
		}
	}
}

