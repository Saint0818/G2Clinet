﻿using UnityEngine;
using System.Collections;
using GameStruct;

public class HintAvatarView : MonoBehaviour {

	public UISprite[] AttrKindsIcon;
	public UILabel[] ValueLabels;
	public UISprite QualitySquare;
	public UISprite ItemPic;
	public GameObject[] AvatarStars;
	public UILabel AmountLabel;

	public GameObject Money;
	public GameObject EXP;
	public GameObject Gem;

	private GameObject mGameObject;
	private void Awake()
	{
		mGameObject = gameObject;
		Money.SetActive(false);
		EXP.SetActive(false);
		Gem.SetActive(false);
		
		for (int i=0; i<AttrKindsIcon.Length; i++){
			AttrKindsIcon[i].gameObject.SetActive(false);
		}

		for (int i=0; i<AvatarStars.Length; i++)
			AvatarStars[i].SetActive(false);
		Hide();
	}
	
	public void Show()
	{
		mGameObject.SetActive(true);
	}
	
	public void Hide()
	{
		mGameObject.SetActive(false);
	}


	public void UpdateUI(TItemData itemData)
	{
		QualitySquare.spriteName = "Equipment_" + Mathf.Clamp(itemData.Quality, 1, 5).ToString();

		for (int i=0; i<itemData.Bonus.Length; i++) {
			AttrKindsIcon[i].gameObject.SetActive(true);
			AttrKindsIcon[i].spriteName = "AttrKind_" + itemData.Bonus[i].GetHashCode();
			if(itemData.Bonus[i].GetHashCode() > 0 && itemData.Bonus[i].GetHashCode() < 18)
				AttrKindsIcon[i].gameObject.SetActive(false);
		}

		for (int i=0; i<itemData.BonusValues.Length; i++) {
			ValueLabels[i].text = itemData.BonusValues[i].ToString();
			if(itemData.BonusValues[i] == 0)
				ValueLabels[i].gameObject.SetActive(false);
		}

		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;

		AmountLabel.text = "";
	}
}
