using UnityEngine;
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
			GameFunction.ShowStar(ref SkillStars, itemData.LV, itemData.Quality, GameData.DSkillData[itemData.Avatar].MaxStar);
		}

		AmountLabel.text = "";
	}

	public void UpdateUI (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(21))) {
				SkillItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(21)];
			}

			QualityCards.spriteName = "cardlevel_" + GameData.DSkillData[skill.ID].Quality.ToString();
			SkillItemPic.spriteName = GameData.DSkillData[skill.ID].PictureNo + "s";
			GameFunction.ShowStar(ref SkillStars, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);
		}
		AmountLabel.text = "";
	}
}
