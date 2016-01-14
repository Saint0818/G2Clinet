using GameStruct;
using UnityEngine;

public class UIItemHint : UIBase {
	private static UIItemHint instance = null;
	private const string UIName = "UIItemHint";
	
    private EventDelegate.Callback callbackBuy;
    private GameObject uiBuy;
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
            OnShow(GameData.DItemData[data.ID]);
            uiBuy.SetActive(true);
            callbackBuy = callback;
            labelPrice.text = data.Price.ToString();
            labelCount.text = string.Format(TextConst.S(4513), data.Num);
            if (data.SpendKind == 0) {
                spriteCoin.spriteName = "Icon_Gem";
                labelPrice.color = new Color(255, 0, 255, 255);
            } else {
                spriteCoin.spriteName = "Icon_Coin";
                labelPrice.color = Color.white;
            }
        }
    }

	protected override void InitCom() {
        uiBuy = GameObject.Find (UIName + "/Window/Center/BuyItem");
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
	public void OnShow(TItemData itemData) {
        uiBuy.SetActive(false);
		hideAll ();
		scrollViewExplain.ResetPosition();
		UIShow(true);
		gameObject.transform.localPosition = new Vector3(0, 0, -10);
		if(itemData.Kind == 21) {
			//For First Get
			hintSkillView.Show();
			hintSkillView.UpdateUI(itemData);
			if(GameData.Team.SkillCardCounts == null)
				GameData.Team.InitSkillCardCount();
			if(GameData.Team.SkillCardCounts.ContainsKey(itemData.Avatar))
				setHaveCount(GameData.Team.SkillCardCounts[itemData.Avatar]);
			else
				setHaveCount(0);
			uiLabelExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[itemData.Avatar].Explain, itemData.Avatar, itemData.LV);
		} else if(itemData.Kind == 19) {
			hintInlayView.Show();
			hintInlayView.UpdateUI(itemData);
			if(GameData.Team.HasMaterialItem(GameData.Team.FindMaterialItemIndex(itemData.ID)))
				setHaveCount(GameData.Team.MaterialItems[GameData.Team.FindMaterialItemIndex(itemData.ID)].Num);
			else
				setHaveCount(0);
			uiLabelExplain.text = itemData.Explain;
		} else {
			hintAvatarView.Show();
			hintAvatarView.UpdateUI(itemData);
			//TODO : 等待嘉明的來源
			setHaveCount(0);
			uiLabelExplain.text = itemData.Explain;
		}
		uiLabelName.text = itemData.Name;
		uiLabelName.color = TextConst.Color(itemData.Quality);
	}
	
	public void OnShowSkill(TSkill skill) {
		UIShow(true);
		scrollViewExplain.ResetPosition();
		hintSkillView.Show();
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
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
			UIShow(false);
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
}
