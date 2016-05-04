using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using GameEnum;

public struct TGamePersonalValue {
	public string Identifier;
	public string Name;
	public float CombatPower;
	public int PVPScore;
	public string FacePic;
	public string PositionPic;
	public int Lv;
	public int FriendKind;
	public bool isJoystick;
	public ETeamKind Team;
}

public struct TGamePersonalView {
	public UILabel labelName;
	public UILabel labelCombat;
	public UISprite spritePvPRank;
	public UILabel labelPvPScore;
	public UISprite spriteFace;
	public UISprite spritePosition;
	public UILabel labelLevel;
	public UILabel IDCheckLabel;

	public void UpdateView (TGamePersonalValue value) {
		labelName.text = value.Name;
		labelCombat.text = value.CombatPower.ToString();
		spritePvPRank.spriteName = GameFunction.PVPRankIconName(GameFunction.GetPVPLv(value.PVPScore));
		labelPvPScore.text = value.PVPScore.ToString();
		spriteFace.spriteName = value.FacePic;
		spritePosition.spriteName = value.PositionPic;
		labelLevel.text = value.Lv.ToString();
		spritePvPRank.gameObject.SetActive(value.PVPScore > 0);
		labelLevel.gameObject.SetActive(value.Lv > 0);

		if(value.FriendKind == EFriendKind.Mercenary)
			IDCheckLabel.text = TextConst.S(9526);
		else if(!string.IsNullOrEmpty(value.Identifier))
			IDCheckLabel.text = TextConst.S(9524);
		else
			IDCheckLabel.text = TextConst.S(9525);
		
		IDCheckLabel.gameObject.SetActive((value.Team == ETeamKind.Self) && !value.isJoystick);
			
	}
}

public class UIGamePlayerInfo : UIBase {
	private static UIGamePlayerInfo instance = null;
	private const string UIName = "UIGamePlayerInfo";

	private TGamePersonalValue gamePersonalvalue = new TGamePersonalValue();
	private TGamePersonalView gamePersonalView = new TGamePersonalView();
	private UILabel[] normalAbilitys = new UILabel[12];
	private UILabel[] buffAbilitys = new UILabel[12];
	private UISprite[] buffLines = new UISprite[12];
	private UISprite[] buffArrow = new UISprite[12];
	private UIScrollView scrollView;
	private UIPanel skillPanel;
	private UIButton buttonMakeFriend;

	private List<TPassiveSkillCard> skillList = new List<TPassiveSkillCard>();
	private TSkill[] skillCards;
	private GameObject itemSkill;

	private bool isFriend;
	private string playerID;

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
					RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else						
			Get.Show(isShow);
	}

	public static UIGamePlayerInfo Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGamePlayerInfo;

			return instance;
		}
	}

	protected override void InitCom() {
		gamePersonalView.labelName = GameObject.Find(UIName + "/Window/Right/View/PersonalView/PlayerName/NameLabel").GetComponent<UILabel>();
		gamePersonalView.labelCombat = GameObject.Find(UIName + "/Window/Right/View/PersonalView/CombatView/CombatLabel").GetComponent<UILabel>();
		gamePersonalView.spritePvPRank = GameObject.Find(UIName + "/Window/Right/View/PersonalView/PvPRankIcon").GetComponent<UISprite>();
		gamePersonalView.labelPvPScore = GameObject.Find(UIName + "/Window/Right/View/PersonalView/PvPRankIcon/ScoreLabel").GetComponent<UILabel>();
		gamePersonalView.spriteFace = GameObject.Find(UIName + "/Window/Right/View/PersonalView/PlayerInGameBtn/PlayerPic").GetComponent<UISprite>();
		gamePersonalView.spritePosition = GameObject.Find(UIName + "/Window/Right/View/PersonalView/PlayerInGameBtn/PlayerPic/PositionIcon").GetComponent<UISprite>();
		gamePersonalView.labelLevel = GameObject.Find(UIName + "/Window/Right/View/PersonalView/PlayerInGameBtn/LevelGroup").GetComponent<UILabel>();
		gamePersonalView.IDCheckLabel = GameObject.Find(UIName + "/Window/Right/View/PersonalView/PlayerName/IDCheckLabel").GetComponent<UILabel>();

		for(int i=0; i<12; i++) {
			normalAbilitys[i] = GameObject.Find(UIName + "/Window/Right/View/AbilityView/AttrGroup/" + i.ToString() + "/ValueBaseLabel").GetComponent<UILabel>();
			SetBtnFun(UIName + "/Window/Right/View/AbilityView/AttrGroup/" + i.ToString(), OnAttributeHint);
			buffAbilitys[i] = GameObject.Find(UIName + "/Window/Right/View/AbilityView/BuffGroup/" + i.ToString()).GetComponent<UILabel>();
			buffLines[i] =  GameObject.Find(UIName + "/Window/Right/View/AbilityView/BuffGroup/" + i.ToString() + "/BuffLine").GetComponent<UISprite>();
			buffArrow[i] =  GameObject.Find(UIName + "/Window/Right/View/AbilityView/BuffGroup/" + i.ToString() + "/Arrow").GetComponent<UISprite>();
		}

		scrollView = GameObject.Find(UIName + "/Window/Right/View/SkillView/CardScroll/List").GetComponent<UIScrollView>();
		skillPanel = GameObject.Find(UIName + "/Window/Right/View/SkillView/CardScroll/List").GetComponent<UIPanel>();
		buttonMakeFriend = GameObject.Find(UIName + "/Window/Right/View/MakeFriend").GetComponent<UIButton>();

		itemSkill =  Resources.Load(UIPrefabPath.ItemCardEquipped) as GameObject;

		SetBtnFun (ref buttonMakeFriend, OnClose);
		SetBtnFun (UIName + "/Window/Right/View/NoBtn", OnClose);
	}

	public void OnClose () {
		UIShow (false);
	}

	private void OnAttributeHint()
	{
		int index;
		EAttribute att = EAttribute.Point2;

		if (int.TryParse(UIButton.current.name, out index))
		{
			switch (index)
			{
			case 0:
				att = EAttribute.Point2;
				break;
			case 1:
				att = EAttribute.Point3;
				break;
			case 2:
				att = EAttribute.Dunk;
				break;
			case 3:
				att = EAttribute.Rebound;
				break;
			case 4:
				att = EAttribute.Block;
				break;
			case 5:
				att = EAttribute.Steal;
				break;
			case 6:
				att = EAttribute.Speed;
				break;
			case 7:
				att = EAttribute.Stamina;
				break;
			case 8:
				att = EAttribute.Strength;
				break;
			case 9:
				att = EAttribute.Defence;
				break;
			case 10:
				att = EAttribute.Dribble;
				break;
			case 11:
				att = EAttribute.Pass;
				break;
			}
			int kind = GameFunction.GetEBounsIndexByAttribute(att);
			UIAttributeHint.Get.UpdateView(kind);
		}
	}

	private bool checkIsFriend (string id) {
		if(!string.IsNullOrEmpty(id)) {
			if(GameData.Team.CheckFriend(id)) 
				return true;
			else 
				return false;
		} 
		return true;
	}

	public void OnMakeFriend () {
		if(isFriend) {
			isFriend = false;
		} else {
			isFriend = true;
			SendHttp.Get.MakeFriend(CheckFriendLike, playerID);
		}
	}

	public void CheckFriendLike () {
		buttonMakeFriend.gameObject.SetActive(false);
	}

	public void ShowView (TTeam team, PlayerBehaviour player) {
		UIShow(true);
		playerID = team.Player.Identifier;
		isFriend = checkIsFriend(playerID);
		skillCards = team.Player.SkillCards;
		buttonMakeFriend.gameObject.SetActive(!isFriend);
		gamePersonalvalue.Name = team.Player.Name;
		gamePersonalvalue.Identifier = team.Identifier;
		gamePersonalvalue.CombatPower = team.Player.CombatPower();
		gamePersonalvalue.PVPScore = team.PVPIntegral;
		gamePersonalvalue.FacePic = team.Player.FacePicture;
		gamePersonalvalue.PositionPic = GameFunction.PositionIcon(team.Player.BodyType);
		gamePersonalvalue.Lv = team.Player.Lv;
		gamePersonalvalue.FriendKind = team.Player.FriendKind;
		gamePersonalvalue.isJoystick = (GameController.Get.Joysticker == player);
		gamePersonalvalue.Team = player.Team;
		gamePersonalView.UpdateView(gamePersonalvalue);
		updateNormalAttr(player.BaseAttribute);
		updateBuffAttr(player.BaseAttribute, player.Attribute);
		initSkillList();
	}

	public void OnSkillHint() {
		int index = -1;
		if (int.TryParse(UIButton.current.name, out index) && index >= -1 && index < skillList.Count)
			UISkillInfo.Get.ShowFromNewCard(skillList[index].Skill);
	}

	private void initSkillList() {
		for (int i = 0; i < skillCards.Length; i++)
			addSkillItem(i, skillCards[i]);

		for (int i = skillCards.Length; i < skillList.Count; i++)
			skillList[i].Enable = false;

		scrollView.gameObject.transform.localPosition = Vector3.zero;
		scrollView.Scroll(0);
		skillPanel.clipOffset = Vector2.zero;
	}

	private void addSkillItem(int index, TSkill skill) {
		if (index >= skillList.Count) {
			TPassiveSkillCard sc = new TPassiveSkillCard();
			GameObject obj = Instantiate(itemSkill) as GameObject;
			sc.Init(scrollView.gameObject, obj, new EventDelegate(OnSkillHint));

			obj.name = skill.ID.ToString();
			obj.GetComponent<UIDragScrollView>().scrollView = scrollView;
			skillList.Add(sc);
		}

		skillList[index].Enable = (GameData.DSkillData.ContainsKey(skill.ID));
		skillList[index].UpdateView(index, skill, new Vector3(0, 148 -index * 70, 0));
	}
		
	private void updateNormalAttr (TPlayer attr) {
		for(int i=0; i<normalAbilitys.Length; i++) 
			normalAbilitys[i].text = getAttrValue(attr, i).ToString();
	}

	private void updateBuffAttr (TPlayer normal, TPlayer after) {
		for(int i=0; i<buffAbilitys.Length; i++) {
			float normalValue = getAttrValue(normal, i);
			float afterValue = getAttrValue(after, i);
			buffAbilitys[i].text = (afterValue - normalValue).ToString();
			if(afterValue - normalValue > 0) {
				buffAbilitys[i].color = Color.green;
				buffLines[i].spriteName = "Buff_outline";
				buffArrow[i].spriteName = "Buff_Up";
			} else {
				buffAbilitys[i].color = Color.red;
				buffLines[i].spriteName = "DeBuff_outline";
				buffArrow[i].spriteName = "Buff_Down";
			}

			buffAbilitys[i].gameObject.SetActive((afterValue - normalValue != 0));
		}
	}

	private float getAttrValue (TPlayer attr, int index) {
		switch (index)
		{
		case 0:
			return attr.Point2;
		case 1:
			return attr.Point3;
		case 2:
			return attr.Dunk;
		case 3:
			return attr.Rebound;
		case 4:
			return attr.Block;
		case 5:
			return attr.Steal;
		case 6:
			return attr.Speed;
		case 7:
			return attr.Stamina;
		case 8:
			return attr.Strength;
		case 9:
			return attr.Defence;
		case 10:
			return attr.Dribble;
		case 11:
			return attr.Pass;
		}
		return 0;
	}
}

