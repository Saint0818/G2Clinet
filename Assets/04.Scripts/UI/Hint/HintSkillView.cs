using UnityEngine;
using System.Collections;
using GameStruct;

public class HintSkillView : MonoBehaviour {

	public UILabel SkillKindLabel;
	public UISprite QualityCards;
	public UISprite SkillItemPic;
	public UISprite SkillStar;
	public UISprite SkillLevel;
	public UILabel AmountLabel;
	
	private GameObject mGameObject;
	private void Awake()
	{
		mGameObject = gameObject;
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

	public void UpdateUI(TSkill skill)
	{
		if(skill.ID >= GameConst.ID_LimitActive)
			SkillKindLabel.text = TextConst.S(7111);
		else 
			SkillKindLabel.text = TextConst.S(7112);

		SkillLevel.spriteName = "Cardicon" + Mathf.Clamp(skill.Lv, 1, 5).ToString();
		QualityCards.spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[skill.ID].Quality, 1, 3).ToString();
		SkillItemPic.spriteName = GameData.DSkillData[skill.ID].PictureNo + "s";
		SkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[skill.ID].Star , 1, 5).ToString();
		AmountLabel.text = GameData.Team.Player.GetSkillCount(skill.ID).ToString();
	}

	public void UpdateUI(TItemData itemData)
	{
		if(itemData.Avatar >= GameConst.ID_LimitActive)
			SkillKindLabel.text = TextConst.S(7111);
		else 
			SkillKindLabel.text = TextConst.S(7112);
		
		SkillLevel.spriteName = "Cardicon" + Mathf.Clamp(itemData.LV, 1, 5).ToString();
		QualityCards.spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[itemData.Avatar].Quality, 1, 3).ToString();
		SkillItemPic.spriteName = GameData.DSkillData[itemData.Avatar].PictureNo + "s";
		SkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[itemData.Avatar].Star , 1, 5).ToString();
		AmountLabel.text = GameData.Team.Player.GetSkillCount(itemData.Avatar).ToString();
	}
}
