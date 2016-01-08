using GameStruct;
using UnityEngine;

public class HintSkillView : MonoBehaviour {

	public UILabel SkillKindLabel;
	public UISprite QualityCards;
	public UISprite SkillItemPic;
	public SkillCardStar[] SkillStar;
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
		if(GameFunction.IsActiveSkill(skill.ID))
			SkillKindLabel.text = TextConst.S(7111);
		else 
			SkillKindLabel.text = TextConst.S(7112);
		
		if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(21))) {
			SkillItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(21)];
		}
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			QualityCards.spriteName = "cardlevel_" + GameData.DSkillData[skill.ID].Quality.ToString();
			SkillItemPic.spriteName = GameData.DSkillData[skill.ID].PictureNo + "s";
			GameFunction.ShowStar(ref SkillStar, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);
		}

//		AmountLabel.text = GameData.Team.Player.GetSkillCount(skill.ID).ToString();
		AmountLabel.text = "";
	}

	public void UpdateUI(TItemData itemData)
	{
		if(GameFunction.IsActiveSkill(itemData.Avatar))
			SkillKindLabel.text = TextConst.S(7111);
		else 
			SkillKindLabel.text = TextConst.S(7112);

		if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(itemData.Atlas))) {
			SkillItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(itemData.Atlas)];
		}

		if(GameData.DSkillData.ContainsKey(itemData.Avatar)) {
			SkillItemPic.spriteName = GameData.DSkillData[itemData.Avatar].PictureNo + "s";
			QualityCards.spriteName = "cardlevel_" + GameData.DSkillData[itemData.Avatar].Quality.ToString();
			GameFunction.ShowStar(ref SkillStar, itemData.LV, GameData.DSkillData[itemData.Avatar].Quality, GameData.DSkillData[itemData.Avatar].MaxStar);
		}
		
//		AmountLabel.text = GameData.Team.Player.GetSkillCount(itemData.Avatar).ToString();
		AmountLabel.text = "";
	}
}
