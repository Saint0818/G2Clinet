using UnityEngine;
using System.Collections;
using GameStruct;
using Newtonsoft.Json;

public class UISkillEvolution { 
	public UISkillReinforce mSelf;

	private int skillIndex;
	private int[] materialIndexs;

	private TActiveSkillCard[] skillCards = new TActiveSkillCard[2];
	private TSkillCardValue[] skillCardValues = new TSkillCardValue[2];
	private TSkillCardMaterial skillCardMaterial;

	private UILabel labelPrice;

	private TSkill nextSkill;
	private TSkill mSkill;
	private int evolutionPrice;
	private bool isEquiped;

	private GameObject goEvolution;
	private GameObject labelWarning;

	public void InitCom(UISkillReinforce skillReinforce, string UIName) {
		mSelf = skillReinforce;
		goEvolution = GameObject.Find(UIName + "/Window2/Center/View/RightPart");
		labelWarning = GameObject.Find(UIName + "/Window2/Center/View/RightPart/WarningLabel");

		labelPrice = GameObject.Find(UIName + "/Window2/Center/View/RightPart/DemountBtn/BtnLabel/PriceLabel").GetComponent<UILabel>();
		for(int i=0; i<skillCards.Length; i++) {
			skillCards[i] = new TActiveSkillCard();
			skillCards[i].Init(GameObject.Find(UIName + "/Window2/Center/View/LeftPart/ItemSkillCard"+i.ToString()));
			skillCardValues[i] = new TSkillCardValue();
			skillCardValues[i].Init(GameObject.Find(UIName + "/Window2/Center/View/LeftPart/ReinforceInfo"+i.ToString()).transform);
		}

		skillCardMaterial = new TSkillCardMaterial();
		skillCardMaterial.Init(GameObject.Find(UIName + "/Window2/Center/View/RightPart/Evolution"));
		materialIndexs = new int[3];//目前訂三種

		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot0/View/MaterialItem", OnSearchMaterial1);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot1/View/MaterialItem", OnSearchMaterial2);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/Evolution/ElementSlot2/View/MaterialItem", OnSearchMaterial3);
		mSelf.SetBtn(UIName + "/Window2/Center/View/RightPart/DemountBtn", OnEvolution);

		labelWarning.SetActive(false);
	}

	public void Show (int index, TSkill currentSkill, bool isAlreadyEquip) {
		skillIndex = index;
		isEquiped = isAlreadyEquip;
		Refresh (currentSkill);
	}

	public void OnSearchMaterial1 () {
		TMaterialItem materialSkillCard = new TMaterialItem();
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) 
			if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum1) 
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material1)) 
			UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material1], enable => {if(enable) mSelf.OnClose(); UISkillFormation.Visible = false;UISkillInfo.Visible = false;});
		
	}

	public void OnSearchMaterial2 () {
		TMaterialItem materialSkillCard = new TMaterialItem();
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) 
			if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum2) 
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material2)) 
			UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material2], enable => {if(enable) mSelf.OnClose();UISkillFormation.Visible = false;UISkillInfo.Visible = false;});

	}

	public void OnSearchMaterial3 () {
		TMaterialItem materialSkillCard = new TMaterialItem();
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) 
			if(materialSkillCard.Num < GameData.DSkillData[mSkill.ID].MaterialNum3) 
				if(GameData.DItemData.ContainsKey(GameData.DSkillData[mSkill.ID].Material3)) 
			UIItemSource.Get.ShowMaterial(GameData.DItemData[GameData.DSkillData[mSkill.ID].Material3], enable => {if(enable) mSelf.OnClose();UISkillFormation.Visible = false;UISkillInfo.Visible = false;});
		
	}

	public void RefreshReinForce (TSkill skill, int index) {
		mSkill = skill;
		skillIndex = index;
	}

	public void Refresh (TSkill skill) {
		mSkill = skill;
		if(GameData.DSkillData.ContainsKey(mSkill.ID)) {
			nextSkill = new TSkill();
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
					if(mSelf.CheckMoney(evolutionPrice, true)) {
						if(isEquiped) {
							mSelf.SendEvolution(0);
						} else {
							mSelf.SendEvolution(1);
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
		mSelf.OnClose();
	}

	public int SkillIndex {
		get {return skillIndex;}
	}

	public int[] MaterialIndexs {
		get {return materialIndexs;}
	}

	public TSkill NextSkill {
		get {return nextSkill;}
	}

	public TSkill MySkill {
		get {return mSkill;}
	}

//	public void SendEvolution(int kind) {
//		WWWForm form = new WWWForm();
//		form.AddField("RemoveIndex", skillIndex);
//		form.AddField("Kind", kind);
//		form.AddField("MaterialIndexs", JsonConvert.SerializeObject(materialIndexs));
//		SendHttp.Get.Command(URLConst.EvolutionSkillcard, waitEvolutionSkillcard, form);
//	}
//
//	private void waitEvolutionSkillcard(bool ok, WWW www) {
//		if (ok) {
//			TEvolution result = JsonConvert.DeserializeObject <TEvolution>(www.text); 
//			GameData.Team.SkillCards = result.SkillCards;
//			GameData.Team.Player.SkillCards = result.PlayerSkillCards;
//			GameData.Team.MaterialItems = result.MaterialItems;
//			GameData.Team.LifetimeRecord = result.LifetimeRecord;
//			GameData.Team.InitSkillCardCount();
//			mSelf.SetMoney(result.Money);
//			UIMainLobby.Get.UpdateUI();
//			UILevelUp.Get.ShowSkill(mSkill, nextSkill);
//
//			if(UISkillFormation.Visible)
//				UISkillFormation.Get.RefreshAddCard();
//		
//			mSkill = findNewSkill(mSkill);
//			Refresh ();
//
//		} else {
//			Debug.LogError("text:"+www.text);
//		} 
//	}
//
//	private TSkill findNewSkill(TSkill skill) {
//		for(int i=0; i<GameData.Team.SkillCards.Length; i++) {
//			if(GameData.Team.SkillCards[i].SN == skill.SN){
//				skillIndex = i;
//				return GameData.Team.SkillCards[i];
//			}
//		}
//
//		for(int i=0; i<GameData.Team.Player.SkillCards.Length; i++) {
//			if(GameData.Team.Player.SkillCards[i].SN == skill.SN){
//				skillIndex = i;
//				return GameData.Team.Player.SkillCards[i];
//			}
//		}
//		return skill;
//	}
}
