﻿using UnityEngine;
using System.Collections;
using GameStruct;
using Newtonsoft.Json;

public struct TEvolution {
	public int Money;
	public TSkill[] TSkillCards;
	public TMaterialSkillCard[] MaterialSkillCards;
	public TItem Items;
}

public struct TSkillCardValue {
	public GameObject[] AttrView;
	public UILabel[] GroupLabel;
	public UILabel[] ValueLabel0;

	public void Init (Transform t) {
		AttrView = new GameObject[6];
		GroupLabel = new UILabel[6];
		ValueLabel0 = new UILabel[6];

		for(int i=0; i<AttrView.Length; i++) {
			AttrView[i] = t.FindChild("AttrView" + i.ToString()).gameObject;
			GroupLabel[i] = AttrView[i].transform.FindChild("GroupLabel").GetComponent<UILabel>();
			ValueLabel0[i] = AttrView[i].transform.FindChild("ValueLabel0").GetComponent<UILabel>();
		}
	}

	private void hideAll () {
		for(int i=0; i<AttrView.Length; i++) 
			AttrView[i].SetActive(false);	
	}

	public void UpdateView(TSkill skill) {
		hideAll ();
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].space > 0) {
				AttrView[index].SetActive(true);	
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Space(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				if(GameFunction.IsActiveSkill(skill.ID)) {
					GroupLabel[index].text = TextConst.S(7207);
					ValueLabel0[index].text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
				} else {
					GroupLabel[index].text = TextConst.S(7206);
					ValueLabel0[index].text = GameData.DSkillData[skill.ID].Rate(skill.Lv).ToString();
				}
				index ++;
			}
			if(GameData.DSkillData[skill.ID].aniRate > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7404);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7405);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].valueBase > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(10500 + GameData.DSkillData[skill.ID].AttrKind);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].lifeTime > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7406);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				index ++;
			}
		}
	}
}

public struct TSkillCardMaterial {
	public GameObject[] mMaterial;
	public UIButton[] MaterialItem;
	public UISprite[] ElementPic;
	public UILabel[] NameLabel;
	public UILabel[] AmountLabel; // 99/99

	public TSkill mSkill;
	public int material1count;
	public int material2count;
	public int material3count;

	public void Init (GameObject obj) {
		mMaterial = new GameObject[3];
		MaterialItem = new UIButton[3];
		ElementPic = new UISprite[3];
		NameLabel = new UILabel[3];
		AmountLabel = new UILabel[3];

		for (int i=0; i<3; i++) {
			mMaterial[i] = obj.transform.FindChild("ElementSlot" + i.ToString()).gameObject;
			MaterialItem[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem").GetComponent<UIButton>();
			ElementPic[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/ElementPic").GetComponent<UISprite>();
			NameLabel[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/NameLabel").GetComponent<UILabel>();
			AmountLabel[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/AmountLabel").GetComponent<UILabel>();
		}
	}

	public void UpdateView (TSkill skill) {
		HideAllMaterial ();
		mSkill = skill;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].Material1 != 0 && GameData.DSkillData[skill.ID].MaterialNum1 != 0) {
				mMaterial[0].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material1)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Atlas))) {
						ElementPic[0].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Atlas)];
					}
					ElementPic[0].spriteName = GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Icon;
					NameLabel[0].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Name;
					
					TMaterialSkillCard materialSkillCard = new TMaterialSkillCard();
					material1count = GameData.Team.FindMaterialSkillCard(GameData.DSkillData[skill.ID].Material1, ref materialSkillCard);
					
					if(material1count != -1)
						AmountLabel[0].text = material1count + "/" + GameData.DSkillData[skill.ID].MaterialNum1.ToString();
					else 
						AmountLabel[0].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum1.ToString();
				}
			}

			if(GameData.DSkillData[skill.ID].Material2 != 0 && GameData.DSkillData[skill.ID].MaterialNum2 != 0) {
				mMaterial[1].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material2)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Atlas))) {
						ElementPic[1].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Atlas)];
					}
					ElementPic[1].spriteName = GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Icon;
					NameLabel[1].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Name;

					TMaterialSkillCard materialSkillCard = new TMaterialSkillCard();
					material2count = GameData.Team.FindMaterialSkillCard(GameData.DSkillData[skill.ID].Material2, ref materialSkillCard);

					if(material2count != -1)
						AmountLabel[1].text = material2count + "/" + GameData.DSkillData[skill.ID].MaterialNum2.ToString();
					else 
						AmountLabel[0].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum2.ToString();
				}
			}

			if(GameData.DSkillData[skill.ID].Material3 != 0 && GameData.DSkillData[skill.ID].MaterialNum3 != 0) {
				mMaterial[2].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material3)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Atlas))) {
						ElementPic[2].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Atlas)];
					}
					ElementPic[2].spriteName = GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Icon;
					NameLabel[2].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Name;

					TMaterialSkillCard materialSkillCard = new TMaterialSkillCard();
					material3count = GameData.Team.FindMaterialSkillCard(GameData.DSkillData[skill.ID].Material3, ref materialSkillCard);

					if(material3count != -1)
						AmountLabel[2].text = material3count + "/" + GameData.DSkillData[skill.ID].MaterialNum3.ToString();
					else 
						AmountLabel[0].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum3.ToString();
				}
			}
		}
	}

	public bool IsEnoughMaterial {
		get {
			bool flag1 = true;
			bool flag2 = true;
			bool flag3 = true;
			if(GameData.DSkillData.ContainsKey(mSkill.ID) || GameData.DSkillData[mSkill.ID].EvolutionSkill != 0) {
				if(!GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material1) && 
					!GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material2) &&
					!GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material3))
					return false;
				
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material1)) {
					if(material1count >= GameData.DSkillData[mSkill.ID].MaterialNum1)
						flag1 = true;
					else 
						flag1 = false;
				}

				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material2)) {
					if(material2count >= GameData.DSkillData[mSkill.ID].MaterialNum2)
						flag2 = true;
					else
						flag2 = false;
				} 

				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material3)) {
					if(material3count >= GameData.DSkillData[mSkill.ID].MaterialNum3)
						flag3 = true;
					else
						flag3 = false;
				} 
				
				if(flag1 && flag2 && flag3)
					return true;
				else
					return false;
			} else 
				return false;
			
		}
	}

	public void HideAllMaterial () {
		for (int i=0; i<mMaterial.Length; i++) 
			mMaterial[i].SetActive(false);
	}


}

public class UISkillEvolution : UIBase {
	private static UISkillEvolution instance = null;
	private const string UIName = "UISkillEvolution";

	private int skillIndex;

	private TActiveSkillCard[] skillCards = new TActiveSkillCard[2];
	private TSkillCardValue[] skillCardValues = new TSkillCardValue[2];
	private TSkillCardMaterial skillCardMaterial;

	private UILabel labelPrice;

	private TSkill mSkill;
	private int evolutionPrice;
	private bool isEquiped;


	private GameObject goEvolution;
	private GameObject labelWarning;


	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		else
			if (isShow)
				Get.Show(isShow);
	}

	public static UISkillEvolution Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillEvolution;

			return instance;
		}
	}

	protected override void InitCom() {
		goEvolution = GameObject.Find(UIName + "/Window/Center/View/RightPart");
		labelWarning = GameObject.Find(UIName + "/Window/Center/View/RightPart/WarningLabel");
		labelPrice = GameObject.Find(UIName + "/Window/DemountBtn/BtnLabel/PriceLabel").GetComponent<UILabel>();
		for(int i=0; i<skillCards.Length; i++) {
			skillCards[i] = new TActiveSkillCard();
			skillCards[i].Init(GameObject.Find(UIName + "/Window/Center/View/LeftPart/ItemSkillCard"+i.ToString()));
			skillCardValues[i] = new TSkillCardValue();
			skillCardValues[i].Init(GameObject.Find(UIName + "/Window/Center/View/LeftPart/ReinforceInfo"+i.ToString()).transform);
		}

		skillCardMaterial = new TSkillCardMaterial();
		skillCardMaterial.Init(GameObject.Find(UIName + "/Window/Center/View/RightPart/Evolution"));

		SetBtnFun(UIName + "/Window/Center/View/RightPart/Evolution/ElementSlot0/View/MaterialItem", OnSearchMaterial1);
		SetBtnFun(UIName + "/Window/Center/View/RightPart/Evolution/ElementSlot1/View/MaterialItem", OnSearchMaterial2);
		SetBtnFun(UIName + "/Window/Center/View/RightPart/Evolution/ElementSlot2/View/MaterialItem", OnSearchMaterial3);
		SetBtnFun(UIName + "/Window/DemountBtn", OnEvolution);
		SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnClose);

		labelWarning.SetActive(false);
	}

	public void OnSearchMaterial1 () {
		TMaterialSkillCard materialSkillCard = new TMaterialSkillCard();
		int count = GameData.Team.FindMaterialSkillCard(GameData.DSkillData[mSkill.ID].Material1, ref materialSkillCard);
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			if(count < GameData.DSkillData[mSkill.ID].MaterialNum1) {
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material1)) {
					UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material1], enable => {if(enable) UIShow(false);});
				}
			}
		}
	}

	public void OnSearchMaterial2 () {
		TMaterialSkillCard materialSkillCard = new TMaterialSkillCard();
		int count = GameData.Team.FindMaterialSkillCard(GameData.DSkillData[mSkill.ID].Material2, ref materialSkillCard);
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			if(count < GameData.DSkillData[mSkill.ID].MaterialNum2) {
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material2)) {
					UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material2], enable => {if(enable) UIShow(false);});
				}
			}
		}
	}

	public void OnSearchMaterial3 () {
		TMaterialSkillCard materialSkillCard = new TMaterialSkillCard();
		int count = GameData.Team.FindMaterialSkillCard(GameData.DSkillData[mSkill.ID].Material3, ref materialSkillCard);
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			if(count < GameData.DSkillData[mSkill.ID].MaterialNum3) {
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material3)) {
					UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material3], enable => {if(enable) UIShow(false);});
				}
			}
		}
	}

	public void Show (int index, TSkill currentSkill, TSkill nextSkill, bool isAlreadyEquip) {
		UIShow(true);
		mSkill = currentSkill;
		skillIndex = index;
		isEquiped = isAlreadyEquip;

		labelWarning.SetActive((currentSkill.ID == nextSkill.ID));
		goEvolution.SetActive(!labelWarning.activeInHierarchy);
		if(GameData.DSkillData.ContainsKey(currentSkill.ID)) {
			evolutionPrice = GameData.DSkillData[currentSkill.ID].EvolutionMoney;
			labelPrice.text = evolutionPrice.ToString();
		}
		
		skillCards[0].UpdateViewFormation(currentSkill, false);
		skillCards[1].UpdateViewFormation(nextSkill, false);
		skillCardValues[0].UpdateView(currentSkill);
		skillCardValues[1].UpdateView(nextSkill);
		skillCardMaterial.UpdateView(currentSkill);

	}

	public void OnEvolution () {
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			if(mSkill.Lv >= GameData.DSkillData[mSkill.ID].MaxStar) {
				if(skillCardMaterial.IsEnoughMaterial) {
					if(CheckMoney(evolutionPrice)) {
						if(isEquiped) {
							
						} else {
							
						}
							
					} else
						UIHint.Get.ShowHint(TextConst.S(7653), Color.red);
				} else
					UIHint.Get.ShowHint(TextConst.S(7652), Color.red);
			} else
				UIHint.Get.ShowHint(TextConst.S(7651), Color.red);				
		}
	}

	public void OnClose () {
		UIShow(false);
	}
	/// <summary>
	/// Sends the reinforce.
	/// </summary>
//	public void SendEvolution() {
//		WWWForm form = new WWWForm();
//		form.AddField("RemoveIndex", skillIndex);
//		SendHttp.Get.Command(URLConst.EvolutionSkillcard, waitEvolutionSkillcard, form);
//	}
//
//	public void SendEvolutionPlayer() {
//		WWWForm form = new WWWForm();
//		form.AddField("RemoveIndex", skillIndex);
//		SendHttp.Get.Command(URLConst.EvolutionPlayerSkillcard, waitEvolutionPlayer, form);
//	}
//
//	private void waitEvolutionSkillcard(bool ok, WWW www) {
//		if (ok) {
//			TEvolution result = JsonConvert.DeserializeObject <TEvolution>(www.text); 
//			GameData.Team.SkillCards = result.SkillCards;
//			GameData.Team.InitSkillCardCount();
//			SetMoney(result.Money);
//			UIMainLobby.Get.UpdateUI();
//
//			if(UISkillFormation.Visible)
//				UISkillFormation.Get.RefreshAddCard();
//
//			mSkill = findNewSkillFromTeam(mSkill);
//			awakeRunExp();
//
//		} else {
//			Debug.LogError("text:"+www.text);
//		} 
//	}
//
//	private TSkill findNewSkillFromTeam(TSkill skill) {
//		for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
//			if(GameData.Team.SkillCards[i].SN == skill.SN){
//				skillIndex = i;
//				return GameData.Team.SkillCards[i];
//			}
//		}
//
//		return skill;
//	}
//
//	private void waitEvolutionPlayer(bool ok, WWW www) {
//		if (ok) {
//			TEvolution result = JsonConvert.DeserializeObject <TEvolution>(www.text); 
//			GameData.Team.SkillCards = result.SkillCards;
//			GameData.Team.Player.SkillCards = result.PlayerCards;
//			GameData.Team.InitSkillCardCount();
//			SetMoney(result.Money);
//			UIMainLobby.Get.UpdateUI();
//
//			if(UISkillFormation.Visible)
//				UISkillFormation.Get.RefreshAddCard();
//
//			mSkill = findNewSkillFromPlayer(mSkill);
//			awakeRunExp();
//
//		} else {
//			Debug.LogError("text:"+www.text);
//		} 
//	}
//
//	private TSkill findNewSkillFromPlayer(TSkill skill) {
//		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
//			if(GameData.Team.Player.SkillCards[i].SN == skill.SN){
//				skillIndex = i;
//				return GameData.Team.Player.SkillCards[i];
//			}
//		}
//
//		return skill;
//	}
}
