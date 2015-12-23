﻿using UnityEngine;
using System.Collections;
using GameStruct;

public class AwardSkillView : MonoBehaviour {

	public UISprite QualityCards;
	public UISprite SkillItemPic;
	public SkillCardStar[] SkillStars;
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
		if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(itemData.Atlas))) {
			SkillItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(itemData.Atlas)];
		}

		if(GameData.DSkillData.ContainsKey(itemData.Avatar)) {
			QualityCards.spriteName = "cardlevel_" + GameData.DSkillData[itemData.Avatar].Quality.ToString();
			SkillItemPic.spriteName = GameData.DSkillData[itemData.Avatar].PictureNo + "s";
		}

		ShowStar(itemData.LV, itemData.Quality);
		AmountLabel.text = "";
	}

	public void ShowStar (int lv, int quality) {
		for (int i=0; i<SkillStars.Length; i++) {
			if(i < lv)
				SkillStars[i].Show();
			else 
				SkillStars[i].Hide();

			SkillStars[i].SetQuality(quality);
		}
	}
}
