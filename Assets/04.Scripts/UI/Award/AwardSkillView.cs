﻿using UnityEngine;
using System.Collections;
using GameStruct;

public class AwardSkillView : MonoBehaviour {

	public UISprite QualityCards;
	public UISprite SkillItemPic;
	public UISprite SkillStar;
	public UILabel AmountLabel;

	private GameObject mGameObject;
	private void Awake()
	{
		mGameObject = gameObject;
		Hide();
	}
	
	public void Show () {
		mGameObject.SetActive(true);
	}
	
	public void Hide () {
		mGameObject.SetActive(false);
	}

	public void UpdateUI (TItemData itemData){
		QualityCards.spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[itemData.Avatar].Quality, 1, 3).ToString();
		if(GameData.DSkillData.ContainsKey(itemData.Avatar))
			SkillItemPic.spriteName = GameData.DSkillData[itemData.Avatar].PictureNo + "s";
		SkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[itemData.Avatar].Star , 1, 5).ToString();
		AmountLabel.text = "";
	}
}
