﻿using GameStruct;
using UnityEngine;

public class UIItemHint : UIBase {
	private static UIItemHint instance = null;
	private const string UIName = "UIItemHint";
    private TSellItem sellItemData;
	private int mItemId;

    private EventDelegate.Callback callbackBuy;
    private GameObject uiBuy;
    private UIButton buttonBuy;
    private UISprite spriteCoin;
    private UILabel labelPrice;
    private UILabel labelCount;

	private UIButton buttonSearch;

	private UILabel uiLabelName;
	private UIScrollView scrollViewExplain;
	private UILabel uiLabelExplain;
	private UILabel uiLabelHave;

	private HintAvatarView hintAvatarView;
	private HintInlayView hintInlayView;
	private HintSkillView hintSkillView;
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
		set
		{
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
	
	public static UIItemHint Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIItemHint;
			
			return instance;
		}
	}

    public void OpenBuyUI(TSellItem data, EventDelegate.Callback callback) {
        if (GameData.DItemData.ContainsKey(data.ID)) {
            Visible = true;
            LayerMgr.Get.SetLayerAllChildren(gameObject, ELayer.UI);
            OnShow(data.ID);
            uiBuy.SetActive(true);
            callbackBuy = callback;
            sellItemData = data;
            labelPrice.text = NumFormater.Convert(data.Price);
            labelCount.text = string.Format(TextConst.S(4513), data.Num);
            spriteCoin.spriteName = GameFunction.SpendKindTexture(data.SpendKind);
            FreshUI();
        }
    }

	protected override void InitCom() {
        uiBuy = GameObject.Find (UIName + "/Window/Center/BuyItem");
        buttonBuy = GameObject.Find (UIName + "/Window/Center/BuyItem/Buy").GetComponent<UIButton>();
        spriteCoin = GameObject.Find (UIName + "/Window/Center/BuyItem/SpendKind").GetComponent<UISprite>();
        labelPrice = GameObject.Find (UIName + "/Window/Center/BuyItem/Price").GetComponent<UILabel>();
        labelCount = GameObject.Find (UIName + "/Window/Center/BuyItem/Count").GetComponent<UILabel>();

		uiLabelName = GameObject.Find (UIName + "/Window/Center/HintView/NameLabel").GetComponent<UILabel>();
		uiLabelExplain = GameObject.Find (UIName + "/Window/Center/HintView/Explain/ExplainLabel").GetComponent<UILabel>();
		uiLabelHave = GameObject.Find (UIName + "/Window/Center/HintView/Have").GetComponent<UILabel>();
		scrollViewExplain = GameObject.Find (UIName + "/Window/Center/HintView/Explain").GetComponent<UIScrollView>();
		hintAvatarView = GameObject.Find (UIName + "/Window/Center/HintView/ItemGroup0").GetComponent<HintAvatarView>();
		hintInlayView = GameObject.Find (UIName + "/Window/Center/HintView/ItemGroup1").GetComponent<HintInlayView>();
		hintSkillView = GameObject.Find (UIName + "/Window/Center/HintView/ItemGroup2").GetComponent<HintSkillView>();

		buttonSearch = GameObject.Find(UIName + "/Window/Center/HintView/SourceButton").GetComponent<UIButton>();

		SetBtnFun (UIName + "/Window/Center/CoverBackground", OnClose);
		SetBtnFun (UIName + "/Window/Center/HintView/NoBtn", OnClose);
        SetBtnFun (UIName + "/Window/Center/BuyItem/Buy", OnBuy);
		SetBtnFun (ref buttonSearch, OnSource);
	}

	private void hideAll () {
		hintAvatarView.Hide ();
		hintInlayView.Hide ();
		hintSkillView.Hide ();
	}

	private void setHaveCount (int value) {
		uiLabelHave.text = string.Format(TextConst.S(6110), value);
	}

	public void OnSource () {
		if(GameData.DItemData.ContainsKey(mItemId)) {			
			if(GameData.DItemData[mItemId].Kind == 19) {
				UIItemSource.Get.ShowMaterial(GameData.DItemData[mItemId], enable => {if(enable){ 
						closeViewBySource ();
					}
				});
			} else if(GameData.DItemData[mItemId].Kind == 21) {
				UIItemSource.Get.ShowSkill(GameData.DItemData[mItemId], enable => {if(enable){ 
						closeViewBySource ();
					}
				});
			} else {
				UIItemSource.Get.ShowAvatar(GameData.DItemData[mItemId], enable => {if(enable){ 
						closeViewBySource ();
					}
				});
			}
		}
	}

	private void closeViewBySource () {
		if(UISuitAvatar.Visible)
			UISuitAvatar.Visible = false;

		if(UIAvatarFitted.Visible)
			UIAvatarFitted.UIShow(false);

		if(UISkillInfo.Visible)
			UISkillInfo.Visible = false;

		if(UISkillFormation.Visible)
			UISkillFormation.Visible = false;

		if(UIMall.Visible) {
			if(UIShop.Visible) 
				UIShop.Visible = false;

			if(UIPlayerAvatar.Visible)
				UIPlayerAvatar.Visible = false;
		}

		UIMainLobby.Get.Hide();
		Visible = false;
	}

	//For First Get
	public void OnShow(int itemID) {
		if(GameData.DItemData.ContainsKey (itemID)) {
			mItemId = itemID;
			buttonSearch.gameObject.SetActive(false);
			uiBuy.SetActive(false);
			hideAll ();
			scrollViewExplain.ResetPosition();
            Visible = true;
			gameObject.transform.localPosition = new Vector3(0, 0, -10);
			if(GameData.DItemData[itemID].Kind == 21) { //技能卡
				//For First Get
				hintSkillView.Show();
				hintSkillView.UpdateUI(GameData.DItemData[itemID]);
				if(GameData.Team.SkillCardCounts == null)
					GameData.Team.InitSkillCardCount();
				
				if(GameData.Team.SkillCardCounts.ContainsKey(GameData.DItemData[itemID].Avatar))
					setHaveCount(GameData.Team.SkillCardCounts[GameData.DItemData[itemID].Avatar]);
				else
					setHaveCount(0);
				
				if(GameData.DSkillData.ContainsKey(GameData.DItemData[itemID].Avatar)) 
					uiLabelExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[GameData.DItemData[itemID].Avatar].Explain, GameData.DItemData[itemID].Avatar, GameData.DItemData[itemID].LV);
				else
					uiLabelExplain.text = GameFunction.GetStringExplain(GameData.DItemData[itemID].Explain, GameData.DItemData[itemID].Avatar, GameData.DItemData[itemID].LV);
				
			} else if(GameData.DItemData[itemID].Kind == 19) {//材料
				hintInlayView.Show();
				hintInlayView.UpdateUI(GameData.DItemData[itemID]);
				if(GameData.Team.HasMaterialItem(GameData.Team.FindMaterialItemIndex(GameData.DItemData[itemID].ID)))
					setHaveCount(GameData.Team.MaterialItems[GameData.Team.FindMaterialItemIndex(GameData.DItemData[itemID].ID)].Num);
				else
					setHaveCount(0);
				
				uiLabelExplain.text = GameData.DItemData[itemID].Explain;
			} else {// Avatar
				hintAvatarView.Show();
				hintAvatarView.UpdateUI(GameData.DItemData[itemID]);
				//TODO : 等待來源
				setHaveCount(GameData.Team.GetAvatarCount(GameData.DItemData[itemID].ID));
				if(GameData.DItemData[itemID].Kind == 31 || GameData.DItemData[itemID].Kind == 32 || GameData.DItemData[itemID].Kind == 33) {
					uiLabelHave.gameObject.SetActive(false);
				}

				uiLabelExplain.text = GameData.DItemData[itemID].Explain;
				if(GameData.DItemData[GameData.DItemData[itemID].ID].Potential > 0 && !GameData.Team.GotAvatar.ContainsKey(GameData.DItemData[itemID].ID))
					uiLabelExplain.text += "\n\n" + TextConst.S(3207) + TextConst.S(3202) + "+" + GameData.DItemData[GameData.DItemData[itemID].ID].Potential.ToString();
			}

			uiLabelName.text = GameData.DItemData[itemID].Name;
			
			uiLabelName.color = TextConst.Color(GameData.DItemData[itemID].Quality);
		}
	}

//		93110 金幣
//		93201 寶石
//		93301 EXP
//		93901 PVP
//		94001 Social
	/// <summary>
	/// kind  1:Money 2:Gem 3:EXP 4:PVP
	/// </summary>
	/// <param name="kind">Kind.</param>
	/// <param name="value">Value.</param>
	public void OnShowOther (int kind, int value = 0) {
		int id = 0;
		int textid = 0;
		buttonSearch.gameObject.SetActive(false);
		uiBuy.SetActive(false);
		hideAll ();
		scrollViewExplain.ResetPosition();
        Visible = true;
		gameObject.transform.localPosition = new Vector3(0, 0, -10);
		hintAvatarView.Show();
		if(kind == 1){
			id = 93110;
			textid = 90001;
		}else if(kind == 2){
			id = 93201;
			textid = 90002;
		}else if(kind == 3){
			id = 93301;
			textid = 90003;
		}else if(kind == 4){
			id = 93901;
			textid = 90004;
		}else if(kind == 5){
			id = 94001;
			textid = 90005;
		}
		uiLabelHave.gameObject.SetActive(false);
		hintAvatarView.UpdateUI(GameData.DItemData[id]);
		uiLabelName.color = TextConst.Color(GameData.DItemData[id].Quality);
		if(value != 0) {
			uiLabelName.text = string.Format(TextConst.S(textid), value);
			uiLabelExplain.text = string.Format(TextConst.S(textid + 5), value);
		} else {
			uiLabelName.text = GameData.DItemData[id].Name;
			uiLabelExplain.text = GameData.DItemData[id].Explain;
		}
	}

	public void OnShowForSuit(int itemID, int lv = -1) {
		if(GameData.DItemData.ContainsKey (itemID)) {
			mItemId = itemID;
			buttonSearch.gameObject.SetActive(true);
			uiBuy.SetActive(false);
			hideAll ();
			scrollViewExplain.ResetPosition();
            Visible = true;
			gameObject.transform.localPosition = new Vector3(0, 0, -10);
			if(GameData.DItemData[itemID].Kind == 21) { //技能卡
				//For First Get
				hintSkillView.Show();
				hintSkillView.UpdateUIForSuit(GameData.DItemData[itemID], lv);
				if(GameData.Team.SkillCardCounts == null)
					GameData.Team.InitSkillCardCount();
				if(GameData.Team.SkillCardCounts.ContainsKey(GameData.DItemData[itemID].Avatar))
					setHaveCount(GameData.Team.SkillCardCounts[GameData.DItemData[itemID].Avatar]);
				else
					setHaveCount(0);
				uiLabelExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[GameData.DItemData[itemID].Avatar].Explain, GameData.DItemData[itemID].Avatar, GameData.DItemData[itemID].LV);
			} else if(GameData.DItemData[itemID].Kind == 19) {//材料
				hintInlayView.Show();
				hintInlayView.UpdateUI(GameData.DItemData[itemID]);
				if(GameData.Team.HasMaterialItem(GameData.Team.FindMaterialItemIndex(GameData.DItemData[itemID].ID)))
					setHaveCount(GameData.Team.MaterialItems[GameData.Team.FindMaterialItemIndex(GameData.DItemData[itemID].ID)].Num);
				else
					setHaveCount(0);

				uiLabelExplain.text = GameData.DItemData[itemID].Explain;
			} else {// Avatar
				hintAvatarView.Show();
				hintAvatarView.UpdateUI(GameData.DItemData[itemID]);
				//TODO : 等待來源
				setHaveCount(GameData.Team.GetAvatarCount(GameData.DItemData[itemID].ID));
				if(GameData.DItemData[itemID].Kind == 31 || GameData.DItemData[itemID].Kind == 32 || GameData.DItemData[itemID].Kind == 33) {
					uiLabelHave.gameObject.SetActive(false);
				}

				uiLabelExplain.text = GameData.DItemData[itemID].Explain;
				if(GameData.DItemData[GameData.DItemData[itemID].ID].Potential > 0 && !GameData.Team.GotAvatar.ContainsKey(GameData.DItemData[itemID].ID))
					uiLabelExplain.text += "\n\n" + TextConst.S(3207) + TextConst.S(3202) + "+" + GameData.DItemData[GameData.DItemData[itemID].ID].Potential.ToString();
			}
			uiLabelName.text = GameData.DItemData[itemID].Name;
			uiLabelName.color = TextConst.Color(GameData.DItemData[itemID].Quality);
		}
	}

	public void OnShowPartnerItem (int itemID, TPlayer player) {
		if(GameData.DItemData.ContainsKey (itemID)) {
			mItemId = itemID;
			buttonSearch.gameObject.SetActive(false);
			uiBuy.SetActive(false);
			hideAll ();
			scrollViewExplain.ResetPosition();
            Visible = true;
			gameObject.transform.localPosition = new Vector3(0, 0, -10);

			hintAvatarView.Show();
			hintAvatarView.UpdateUI(GameData.DItemData[itemID], player);
			//TODO : 等待來源
			setHaveCount(GameData.Team.GetAvatarCount(GameData.DItemData[itemID].ID));
			uiLabelExplain.text = GameData.DItemData[itemID].Explain;
			if(GameData.DItemData[GameData.DItemData[itemID].ID].Potential > 0 && !GameData.Team.GotAvatar.ContainsKey(GameData.DItemData[itemID].ID))
				uiLabelExplain.text += "\n\n" + TextConst.S(3207) + TextConst.S(3202) + "+" + GameData.DItemData[GameData.DItemData[itemID].ID].Potential.ToString();


			uiLabelName.text = GameData.DItemData[itemID].Name;
			uiLabelName.color = TextConst.Color(GameData.DItemData[itemID].Quality);
		}
	}
	
	public void OnShowSkill(TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			mItemId = skill.ID + 20000;
			buttonSearch.gameObject.SetActive(false);
			uiBuy.SetActive(false);
			hideAll ();
            Visible = true;
			scrollViewExplain.ResetPosition();
			hintSkillView.Show();
			uiLabelName.text = GameData.DSkillData[skill.ID].Name;
			uiLabelName.color = TextConst.Color(GameData.DSkillData[skill.ID].Quality);
			if(GameData.Team.SkillCardCounts == null)
				GameData.Team.InitSkillCardCount();
			if(GameData.Team.SkillCardCounts.ContainsKey(skill.ID))
				setHaveCount(GameData.Team.SkillCardCounts[skill.ID]);
			else
				setHaveCount(0);
			uiLabelExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[skill.ID].Explain, skill.ID, skill.Lv);
			hintSkillView.UpdateUI(skill);
		} else {
			Debug.LogError("no id:"+ skill.ID);
		}
	}

	public void OnClose () {
        Visible = false;
	}

    public void OnBuy() {
        if (callbackBuy != null)
            callbackBuy();
    }

    public void FreshUI() {
        bool flag = GameData.Team.CoinEnough(sellItemData.SpendKind, sellItemData.Price);
        labelPrice.color = GameData.CoinEnoughTextColor(flag, sellItemData.SpendKind);
        buttonBuy.normalSprite = GameData.CoinEnoughSprite(flag, 1);
    }
}
