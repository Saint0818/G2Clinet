﻿using GameStruct;
using UnityEngine;

public class HintInlayView : MonoBehaviour {
	
	public UISprite[] AttrKindsIcon;
	private UISprite[] attrKindsIcon;
	public UILabel[] ValueLabels;
	public UISprite QualityOctagon;
	public UISprite ItemPic;
	public UILabel AmountLabel;
	public UILabel[] KindLabels;

	private Transform goQuality;
	public UISprite QualityBG;
    private void Awake()
	{
		if(QualityBG == null)
			goQuality = GameFunction.FindQualityBG(transform);
		if(goQuality != null)
			QualityBG = goQuality.GetComponent<UISprite>();

	    for (int i=0; i<AttrKindsIcon.Length; i++){
			AttrKindsIcon[i].gameObject.SetActive(false);
		}
		Hide();
	}
	
	public void Show()
	{
		gameObject.SetActive(true);
	}
	
	public void Hide()
	{
		gameObject.SetActive(false);
	}
	
	public void UpdateUI(TItemData itemData)
	{
		if(KindLabels == null || KindLabels.Length ==0 ) {
			KindLabels = new UILabel[AttrKindsIcon.Length];
			for (int i=0; i<AttrKindsIcon.Length; i++){
				KindLabels[i] = AttrKindsIcon[i].transform.FindChild("KindLabel").GetComponent<UILabel>();
			}
		}

		QualityOctagon.spriteName = "Patch" + Mathf.Clamp(itemData.Quality, 1, 5).ToString();
		
		for(int i=0; i < itemData.Bonus.Length; i++)
        {
			AttrKindsIcon[i].gameObject.SetActive(true);
			AttrKindsIcon[i].spriteName = "AttrKind_" + itemData.Bonus[i].GetHashCode();
			KindLabels[i].text = TextConst.S(10500 + itemData.Bonus[i].GetHashCode());
		}
		
		for (int i=0; i<itemData.BonusValues.Length; i++) {
			ValueLabels[i].text = itemData.BonusValues[i].ToString();
			if(itemData.BonusValues[i] == 0 && i < AttrKindsIcon.Length)
				AttrKindsIcon[i].gameObject.SetActive(false);
		}

		if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(itemData.Atlas))) {
			ItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(itemData.Atlas)];
		}

		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;

		if(QualityBG != null)
			QualityBG.color = TextConst.ColorBG(itemData.Quality);
		
		AmountLabel.text = "";
	}
}
