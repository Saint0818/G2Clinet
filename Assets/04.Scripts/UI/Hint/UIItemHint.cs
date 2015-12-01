using UnityEngine;
using System.Collections;
using GameStruct;

public class UIItemHint : UIBase {
	private static UIItemHint instance = null;
	private const string UIName = "UIItemHint";
	
	private UILabel uiLabelName;
	private UIScrollView scrollViewExplain;
	private UILabel uiLabelExplain;

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
	
	protected override void InitCom() {
		uiLabelName = GameObject.Find (UIName + "/Window/Center/HintView/NameLabel").GetComponent<UILabel>();
		uiLabelExplain = GameObject.Find (UIName + "/Window/Center/HintView/Explain/ExplainLabel").GetComponent<UILabel>();
		scrollViewExplain = GameObject.Find (UIName + "/Window/Center/HintView/Explain").GetComponent<UIScrollView>();
		hintAvatarView = GameObject.Find (UIName + "/Window/Center/HintView/ItemGroup0").GetComponent<HintAvatarView>();
		hintInlayView = GameObject.Find (UIName + "/Window/Center/HintView/ItemGroup1").GetComponent<HintInlayView>();
		hintSkillView = GameObject.Find (UIName + "/Window/Center/HintView/ItemGroup2").GetComponent<HintSkillView>();

		SetBtnFun (UIName + "/Window/Center/CoverBackground", OnClose);
		SetBtnFun (UIName + "/Window/Center/HintView/NoBtn", OnClose);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {

	}

	private void hideAll () {
		hintAvatarView.Hide ();
		hintInlayView.Hide ();
		hintSkillView.Hide ();
	}

	public void OnShow(TItemData itemData) {
		hideAll ();
		scrollViewExplain.ResetPosition();
		UIShow(true);
		if(itemData.Kind == 21) {
			//For First Get
			hintSkillView.Show();
			hintSkillView.UpdateUI(itemData);
			uiLabelExplain.text = GameFunction.GetStringExplain(itemData.Explain, itemData.Avatar, itemData.LV);
		} else if(itemData.Kind == 19) {
			hintInlayView.Show();
			hintInlayView.UpdateUI(itemData);
			uiLabelExplain.text = itemData.Explain;
		} else {
			hintAvatarView.Show();
			hintAvatarView.UpdateUI(itemData);
			uiLabelExplain.text = itemData.Explain;
		}
		uiLabelName.text = itemData.Name;
	}
	
	public void OnShowSkill(TSkill skill) {
		UIShow(true);
		scrollViewExplain.ResetPosition();
		hintSkillView.Show();
		uiLabelName.text = GameData.DSkillData[skill.ID].Name;
		uiLabelExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[skill.ID].Explain, skill.ID, skill.Lv);
		hintSkillView.UpdateUI(skill);
	}

	public void OnClose () {
		hideAll ();
		UIShow(false);
	}
}
