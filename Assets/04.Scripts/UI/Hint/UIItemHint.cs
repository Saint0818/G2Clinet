using GameStruct;
using UnityEngine;

public class UIItemHint : UIBase {
	private static UIItemHint instance = null;
	private const string UIName = "UIItemHint";
    private TSellItem sellItemData;

    private EventDelegate.Callback callbackBuy;
    private GameObject uiBuy;
    private UIButton buttonBuy;
    private UISprite spriteCoin;
    private UILabel labelPrice;
    private UILabel labelCount;

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
	}
	
	public static UIItemHint Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIItemHint;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			instance.Show(isShow);
		} else
		if (isShow)
			Get.Show(isShow);
	}

    public void OpenBuyUI(TSellItem data, EventDelegate.Callback callback) {
        if (GameData.DItemData.ContainsKey(data.ID)) {
            UIShow(true);
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

		SetBtnFun (UIName + "/Window/Center/CoverBackground", OnClose);
		SetBtnFun (UIName + "/Window/Center/HintView/NoBtn", OnClose);
        SetBtnFun (UIName + "/Window/Center/BuyItem/Buy", OnBuy);
	}

	private void hideAll () {
		hintAvatarView.Hide ();
		hintInlayView.Hide ();
		hintSkillView.Hide ();
	}

	private void setHaveCount (int value) {
		uiLabelHave.text = string.Format(TextConst.S(6110), value);
	}

	//For First Get
	public void OnShow(int itemID) {
		if(GameData.DItemData.ContainsKey (itemID)) {
			uiBuy.SetActive(false);
			hideAll ();
			scrollViewExplain.ResetPosition();
			UIShow(true);
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
				uiLabelExplain.text = GameData.DItemData[itemID].Explain;
				if(GameData.DItemData[GameData.DItemData[itemID].ID].Potential > 0 && !GameData.Team.GotAvatar.ContainsKey(GameData.DItemData[itemID].ID))
					uiLabelExplain.text += "\n\n" + TextConst.S(3207) + TextConst.S(3202) + "+" + GameData.DItemData[GameData.DItemData[itemID].ID].Potential.ToString();
			}
			uiLabelName.text = GameData.DItemData[itemID].Name;
			uiLabelName.color = TextConst.Color(GameData.DItemData[itemID].Quality);
		}
	}

	public void OnShowForSuit(int itemID) {
		if(GameData.DItemData.ContainsKey (itemID)) {
			uiBuy.SetActive(false);
			hideAll ();
			scrollViewExplain.ResetPosition();
			UIShow(true);
			gameObject.transform.localPosition = new Vector3(0, 0, -10);
			if(GameData.DItemData[itemID].Kind == 21) { //技能卡
				//For First Get
				hintSkillView.Show();
				hintSkillView.UpdateUIForSuit(GameData.DItemData[itemID]);
				if(GameData.Team.SkillCardCounts == null)
					GameData.Team.InitSkillCardCount();
				if(GameData.Team.SkillCardCounts.ContainsKey(GameData.DItemData[itemID].Avatar))
					setHaveCount(GameData.Team.SkillCardCounts[GameData.DItemData[itemID].Avatar]);
				else
					setHaveCount(0);
				uiLabelExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[GameData.DItemData[itemID].Avatar].Explain, GameData.DItemData[itemID].Avatar, GameData.DItemData[itemID].LV);
			}
			uiLabelName.text = GameData.DItemData[itemID].Name;
			uiLabelName.color = TextConst.Color(GameData.DItemData[itemID].Quality);
		}
	}

	public void OnShowPartnerItem (int itemID, TPlayer player) {
		if(GameData.DItemData.ContainsKey (itemID)) {
			uiBuy.SetActive(false);
			hideAll ();
			scrollViewExplain.ResetPosition();
			UIShow(true);
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
			uiBuy.SetActive(false);
			hideAll ();
			UIShow(true);
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
		hideAll ();
		UIShow(false);
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
