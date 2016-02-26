using UnityEngine;
using System.Collections;
using GameStruct;
using Newtonsoft.Json;
using System.Collections.Generic;

public class UISkillEvolution { 
	public UISkillReinforce mSelf;

	private int skillIndex;
	private int[] materialIndexs;
	private int[] skillIndexs;

	private List<int> tempSkillIndexs = new List<int>();

	private TActiveSkillCard[] skillCards = new TActiveSkillCard[2];
	private TSkillCardValue[] skillCardValues = new TSkillCardValue[2];
	private TSkillCardMaterial skillCardMaterial;
	private GameObject skillCanEvolution;
	private GameObject skillArrow;

	private UILabel labelPrice;

	private TSkill nextSkill;
	private TSkill mSkill;
	private int evolutionPrice;
	private bool isEquiped;

	private GameObject goEvolution;
	private GameObject labelWarning;

	public void InitCom(UISkillReinforce skillReinforce, string UIName) {
		mSelf = skillReinforce;
		goEvolution = GameObject.Find(UIName + "/Window2/Center/View/RightPart/Evolution");
		labelWarning = GameObject.Find(UIName + "/Window2/Center/View/RightPart/WarningLabel");

		labelPrice = GameObject.Find(UIName + "/Window2/Center/View/RightPart/DemountBtn/BtnLabel/PriceLabel").GetComponent<UILabel>();
		for(int i=0; i<skillCards.Length; i++) {
			skillCards[i] = new TActiveSkillCard();
			skillCards[i].Init(GameObject.Find(UIName + "/Window2/Center/View/LeftPart/ItemSkillCard"+i.ToString()));
			skillCardValues[i] = new TSkillCardValue();
			skillCardValues[i].Init(GameObject.Find(UIName + "/Window2/Center/View/LeftPart/ReinforceInfo"+i.ToString()).transform);
		}

		skillCanEvolution = GameObject.Find(UIName + "/Window2/Center/View/LeftPart/CanEvolution");
		skillArrow = GameObject.Find(UIName + "/Window2/Center/View/LeftPart/Arrow");

		skillCardMaterial = new TSkillCardMaterial();
		skillCardMaterial.Init(GameObject.Find(UIName + "/Window2/Center/View/RightPart/Evolution"));
		materialIndexs = new int[3];//目前訂三種
		refreshMaterialIndex ();

		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot0/View/MaterialItem", OnSearchMaterial1);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot1/View/MaterialItem", OnSearchMaterial2);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot2/View/MaterialItem", OnSearchMaterial3);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot0/View/SkillCardItem", OnSearchMaterial1);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot1/View/SkillCardItem", OnSearchMaterial2);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot2/View/SkillCardItem", OnSearchMaterial3);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/DemountBtn", OnEvolution);

		labelWarning.SetActive(false);
	}

	public void ShowView (int index, TSkill currentSkill, bool isAlreadyEquip) {
		skillIndex = index;
		isEquiped = isAlreadyEquip;
		Refresh (currentSkill);
	}

	private void refreshMaterialIndex () {
		for (int i=0 ;i< materialIndexs.Length; i++)
			materialIndexs[i] = -1;
	}


	private void search(int itemID) {
		if(GameData.DItemData.ContainsKey(itemID)) {
			if(GameData.DItemData[itemID].Kind == 19) {
				UIItemSource.Get.ShowMaterial(GameData.DItemData[itemID], enable => {if(enable){ 
						mSelf.OnSearch(); 
						UISkillFormation.Visible = false;
						UISkillInfo.Visible = false;
					}
				});
			} else if(GameData.DItemData[itemID].Kind == 21) {
				UIItemSource.Get.ShowSkill(GameData.DItemData[itemID], enable => {if(enable){ 
						mSelf.OnSearch(); 
						UISkillFormation.Visible = false;
						UISkillInfo.Visible = false;
					}
				});
			}
		}
	}

	public void OnSearchMaterial1 () {
		if(mSelf.IsCanClick) {
			TMaterialItem materialSkillCard = new TMaterialItem();
			if(GameData.DSkillData.ContainsKey(mSkill.ID)) 
				if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum1) 
					search(GameData.DSkillData[mSkill.ID].Material1);
		}
	}

	public void OnSearchMaterial2 () {
		if(mSelf.IsCanClick) {
			TMaterialItem materialSkillCard = new TMaterialItem();
			if(GameData.DSkillData.ContainsKey(mSkill.ID)) 
				if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum2) 
					search(GameData.DSkillData[mSkill.ID].Material2);
		}
	}

	public void OnSearchMaterial3 () {
		if(mSelf.IsCanClick) {
			TMaterialItem materialSkillCard = new TMaterialItem();
			if(GameData.DSkillData.ContainsKey(mSkill.ID)) 
				if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum3) 
					search(GameData.DSkillData[mSkill.ID].Material3);
		}
	}

	public void RefreshReinForce (TSkill skill, int index) {
		mSkill = skill;
		skillIndex = index;
	}

	public void RefreshPriceUI () {
		labelPrice.text = evolutionPrice.ToString();
		labelPrice.color = GameData.CoinEnoughTextColor(GameData.Team.CoinEnough(1,evolutionPrice),1);
	}

	public void Refresh (TSkill skill) {
		mSkill = skill;
		skillIndexs = new int[0];
		refreshMaterialIndex ();
		skillCanEvolution.SetActive(GameData.Team.IsEnoughMaterial(skill));
		skillArrow.SetActive(!skillCanEvolution.activeSelf);
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			nextSkill = new TSkill();
			if(GameData.DSkillData[mSkill.ID].EvolutionSkill != 0) {
				nextSkill.ID = GameData.DSkillData[mSkill.ID].EvolutionSkill;
				nextSkill.Lv = 0;
				nextSkill.Exp = 0;
				labelWarning.SetActive(false);
				evolutionPrice = GameData.DSkillData[mSkill.ID].EvolutionMoney;
			} else {
				nextSkill = mSkill;
				labelWarning.SetActive(true);
				evolutionPrice = 0;
			}

			goEvolution.SetActive(!labelWarning.activeInHierarchy);
			RefreshPriceUI ();

			skillCards[0].UpdateViewFormation(mSkill, false);
			skillCards[1].UpdateViewFormation(nextSkill, false);
			skillCardValues[0].UpdateView(mSkill, mSkill);
			skillCardValues[1].UpdateView(mSkill, nextSkill);
			skillCardMaterial.UpdateView(mSkill);
		}
	}

	//Kind 0:裝在卡牌上isEquiped(Player.SkillCard) 1:未安裝卡牌notEquiped(Team.SkillCard)
	public void OnEvolution () {
		if(mSelf.IsCanClick) {
			if(GameData.DSkillData.ContainsKey(mSkill.ID) && GameData.DSkillData[mSkill.ID].EvolutionSkill != 0) {
				//星等判斷
				if(mSkill.Lv >= GameData.DSkillData[mSkill.ID].MaxStar) {
					//材料判斷
					if(GameData.Team.IsEnoughMaterial(mSkill)) {
						if(skillCardMaterial.material1index != -1) {
							//材料
							materialIndexs[0] = skillCardMaterial.material1index;
						} else {
							//卡牌
							if(skillCardMaterial.skill1.Count > 0){
								for(int i=0; i<skillCardMaterial.skill1.Count; i++) {
									if(i < GameData.DSkillData[mSkill.ID].MaterialNum1){
										tempSkillIndexs.Add(skillCardMaterial.skill1[i].Index);
									}
								}
							}
						}

						if(skillCardMaterial.material2index != -1) {
							//材料
							materialIndexs[1] = skillCardMaterial.material2index;
						} else {
							//卡牌
							if(skillCardMaterial.skill2.Count > 0){
								for(int i=0; i<skillCardMaterial.skill2.Count; i++) {
									if(i < GameData.DSkillData[mSkill.ID].MaterialNum2){
										tempSkillIndexs.Add(skillCardMaterial.skill2[i].Index);
									}
								}
							}
						}

						if(skillCardMaterial.material3index != -1) {
							//材料
							materialIndexs[2] = skillCardMaterial.material3index;
						} else {
							//卡牌
							if(skillCardMaterial.skill3.Count > 0){
								for(int i=0; i<skillCardMaterial.skill3.Count; i++) {
									if(i < GameData.DSkillData[mSkill.ID].MaterialNum3){
										tempSkillIndexs.Add(skillCardMaterial.skill3[i].Index);
									}
								}
							}
						}
							
						skillIndexs = new int[tempSkillIndexs.Count];

						for(int i=0; i<tempSkillIndexs.Count; i++)
							skillIndexs[i] = tempSkillIndexs[i];

						//由小排到大
						if(skillIndexs.Length > 1) {
							for(int i=0; i<skillIndexs.Length; i++) {
								for (int j=i+1; j<skillIndexs.Length; j++){
									if (skillIndexs[i] >= skillIndexs[j]){
										int temp = skillIndexs[i];
										skillIndexs[i] = skillIndexs[j];
										skillIndexs[j] = temp;
									}
								}
							}
						}

						//金額判斷
						if(mSelf.CheckMoney(evolutionPrice, true)) {
							if(isEquiped) {
								mSelf.SendEvolution(0);
							} else {
								mSelf.SendEvolution(1);
							}
						} else {
							AudioMgr.Get.PlaySound (SoundType.SD_Prohibit);
							UIHint.Get.ShowHint(TextConst.S(7653), Color.red);
						}
					} else
						UIHint.Get.ShowHint(TextConst.S(7652), Color.red);
				} else
					UIHint.Get.ShowHint(TextConst.S(7651), Color.red);				
			}
		}
	}

	public void OnClose () {
		if(mSelf.IsCanClick) 
			mSelf.OnClose();
	}

	public int SkillIndex {
		get {return skillIndex;}
	}

	public int[] MaterialIndexs {
		get {return materialIndexs;}
	}

	public int[] SkillIndexs {
		get {return skillIndexs;}
	}

	public TSkill NextSkill {
		get {return nextSkill;}
	}

	public TSkill MySkill {
		get {return mSkill;}
	}
}
