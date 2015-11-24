﻿using UnityEngine;
using System.Collections;
using GameStruct;

public class UIItemHint : UIBase {
	private static UIItemHint instance = null;
	private const string UIName = "UIItemHint";
	
	private UILabel uiLabelName;
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
		uiLabelExplain = GameObject.Find (UIName + "/Window/Center/HintView/ExplainLabel").GetComponent<UILabel>();
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

	public void OnShowAvatar () {
		UIShow(true);
		hintAvatarView.Show();
//		uiLabelName.text = ;
//		uiLabelExplain.text = ;
//		hintAvatarView.UpdateUI
	}

	public void OnShowInlay () {
		UIShow(true);
		hintInlayView.Show();
//		uiLabelName.text = ;
//		uiLabelExplain.text = ;
//		hintInlayView.UpdateUI
	}

	public void OnShowSkill(TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			UIShow(true);
			hintSkillView.Show();
			uiLabelName.text = GameData.DSkillData[skill.ID].Name;
			uiLabelExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[skill.ID].Explain, skill.ID, skill.Lv);
			hintSkillView.UpdateUI(skill);
		}
	}

	public void OnClose () {
		hideAll ();
		UIShow(false);
	}
}
