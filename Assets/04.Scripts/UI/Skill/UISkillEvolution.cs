using UnityEngine;
using System.Collections;
using GameStruct;
using Newtonsoft.Json;

public struct TEvolution {
	public int Money;
	public TSkill[] SkillCards;
	public TSkill[] PlayerSkillCards;
	public TMaterialItem[] MaterialItems;
	public TTeamRecord LifetimeRecord;
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

	public void HideAll () {
		for(int i=0; i<AttrView.Length; i++) 
			AttrView[i].SetActive(false);	
	}

	public void UpdateView(TSkill skill) {
		HideAll ();
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
	public int material1index;
	public int material2index;
	public int material3index;
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
		material1index = -1;
		material2index = -1;
		material3index = -1;
		mSkill = skill;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].Material1 != 0 && GameData.DSkillData[skill.ID].MaterialNum1 != 0) {
				mMaterial[0].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material1)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Atlas))) {
						ElementPic[0].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Atlas)];
					}
					ElementPic[0].spriteName = "Item_" + GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Icon;
					NameLabel[0].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material1].Name;
					
					TMaterialItem materialSkillCard = new TMaterialItem();
					material1index = GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material1, ref materialSkillCard);
					
					if(material1index != -1)
						AmountLabel[0].text = materialSkillCard.Num + "/" + GameData.DSkillData[skill.ID].MaterialNum1.ToString();
					else 
						AmountLabel[0].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum1.ToString();
					
					material1count = materialSkillCard.Num;
				}
			}

			if(GameData.DSkillData[skill.ID].Material2 != 0 && GameData.DSkillData[skill.ID].MaterialNum2 != 0) {
				mMaterial[1].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material2)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Atlas))) {
						ElementPic[1].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Atlas)];
					}
					ElementPic[1].spriteName = "Item_" + GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Icon;
					NameLabel[1].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material2].Name;

					TMaterialItem materialSkillCard = new TMaterialItem();
					material2index = GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material2, ref materialSkillCard);

					if(material2index != -1)
						AmountLabel[1].text = materialSkillCard.Num + "/" + GameData.DSkillData[skill.ID].MaterialNum2.ToString();
					else 
						AmountLabel[0].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum2.ToString();

					material2count = materialSkillCard.Num;
				}
			}

			if(GameData.DSkillData[skill.ID].Material3 != 0 && GameData.DSkillData[skill.ID].MaterialNum3 != 0) {
				mMaterial[2].SetActive(true);
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[skill.ID].Material3)) {
					if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Atlas))) {
						ElementPic[2].atlas = GameData.DItemAtlas[GameData.AtlasName(GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Atlas)];
					}
					ElementPic[2].spriteName = "Item_" + GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Icon;
					NameLabel[2].text = GameData.DItemData[GameData.DSkillData[skill.ID].Material3].Name;

					TMaterialItem materialSkillCard = new TMaterialItem();
					material3index = GameData.Team.FindMaterialItem(GameData.DSkillData[skill.ID].Material3, ref materialSkillCard);

					if(material3index != -1)
						AmountLabel[2].text = materialSkillCard.Num + "/" + GameData.DSkillData[skill.ID].MaterialNum3.ToString();
					else 
						AmountLabel[0].text = "0/" + GameData.DSkillData[skill.ID].MaterialNum3.ToString();

					material3count = materialSkillCard.Num;
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
	private int[] materialIndexs;

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
		materialIndexs = new int[3];//目前訂三種

		SetBtnFun(UIName + "/Window/Center/View/RightPart/Evolution/ElementSlot0/View/MaterialItem", OnSearchMaterial1);
		SetBtnFun(UIName + "/Window/Center/View/RightPart/Evolution/ElementSlot1/View/MaterialItem", OnSearchMaterial2);
		SetBtnFun(UIName + "/Window/Center/View/RightPart/Evolution/ElementSlot2/View/MaterialItem", OnSearchMaterial3);
		SetBtnFun(UIName + "/Window/DemountBtn", OnEvolution);
		SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnClose);

		labelWarning.SetActive(false);
	}

	public void OnSearchMaterial1 () {
		TMaterialItem materialSkillCard = new TMaterialItem();
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum1) {
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material1)) {
					UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material1], enable => {if(enable) UIShow(false);});
				}
			}
		}
	}

	public void OnSearchMaterial2 () {
		TMaterialItem materialSkillCard = new TMaterialItem();
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum2) {
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material2)) {
					UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material2], enable => {if(enable) UIShow(false);});
				}
			}
		}
	}

	public void OnSearchMaterial3 () {
		TMaterialItem materialSkillCard = new TMaterialItem();
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum3) {
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material3)) {
					UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material3], enable => {if(enable) UIShow(false);});
				}
			}
		}
	}

	private void refresh () {
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			TSkill nextSkill = new TSkill();
			if(GameData.DSkillData[mSkill.ID].EvolutionSkill != 0) {
				nextSkill.ID = GameData.DSkillData[mSkill.ID].EvolutionSkill;
				nextSkill.Lv = 1;
				nextSkill.Exp = 0;
				labelWarning.SetActive(false);
				evolutionPrice = GameData.DSkillData[mSkill.ID].EvolutionMoney;
			} else {
				nextSkill = mSkill;
				labelWarning.SetActive(true);
				evolutionPrice = 0;
			}

			goEvolution.SetActive(!labelWarning.activeInHierarchy);
			labelPrice.text = evolutionPrice.ToString();

			skillCards[0].UpdateViewFormation(mSkill, false);
			skillCards[1].UpdateViewFormation(nextSkill, false);
			skillCardValues[0].UpdateView(mSkill);
			skillCardValues[1].UpdateView(nextSkill);
			skillCardMaterial.UpdateView(mSkill);
		}
	}

	public void Show (int index, TSkill currentSkill, bool isAlreadyEquip) {
		UIShow(true);
		mSkill = currentSkill;
		skillIndex = index;
		isEquiped = isAlreadyEquip;
		refresh ();
	}

	//Kind 0:裝在卡牌上isEquiped(Player.SkillCard) 1:未安裝卡牌notEquiped(Team.SkillCard)
	public void OnEvolution () {
		if(GameData.DSkillData.ContainsKey(mSkill.ID) && GameData.DSkillData[mSkill.ID].EvolutionSkill != 0) {
			//星等判斷
			if(mSkill.Lv >= GameData.DSkillData[mSkill.ID].MaxStar) {
				//材料判斷
				if(skillCardMaterial.IsEnoughMaterial) {
					materialIndexs[0] = skillCardMaterial.material1index;
					materialIndexs[1] = skillCardMaterial.material2index;
					materialIndexs[2] = skillCardMaterial.material3index;
					//金額判斷
					if(CheckMoney(evolutionPrice, true)) {
						if(isEquiped) {
							SendEvolution(0);
						} else {
							SendEvolution(1);
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
	public void SendEvolution(int kind) {
		WWWForm form = new WWWForm();
		form.AddField("RemoveIndex", skillIndex);
		form.AddField("Kind", kind);
		form.AddField("MaterialIndexs", JsonConvert.SerializeObject(materialIndexs));
		SendHttp.Get.Command(URLConst.EvolutionSkillcard, waitEvolutionSkillcard, form);
	}

	private void waitEvolutionSkillcard(bool ok, WWW www) {
		if (ok) {
			TEvolution result = JsonConvert.DeserializeObject <TEvolution>(www.text); 
			GameData.Team.SkillCards = result.SkillCards;
			GameData.Team.Player.SkillCards = result.PlayerSkillCards;
			GameData.Team.MaterialItems = result.MaterialItems;
			GameData.Team.LifetimeRecord = result.LifetimeRecord;
			GameData.Team.InitSkillCardCount();
			SetMoney(result.Money);
			UIMainLobby.Get.UpdateUI();

			if(UISkillFormation.Visible)
				UISkillFormation.Get.RefreshAddCard();

			mSkill = findNewSkill(mSkill);
			refresh ();

		} else {
			Debug.LogError("text:"+www.text);
		} 
	}

	private TSkill findNewSkill(TSkill skill) {
		for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
			if(GameData.Team.SkillCards[i].SN == skill.SN){
				skillIndex = i;
				return GameData.Team.SkillCards[i];
			}
		}

		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
			if(GameData.Team.Player.SkillCards[i].SN == skill.SN){
				skillIndex = i;
				return GameData.Team.Player.SkillCards[i];
			}
		}
		return skill;
	}
}
