using UnityEngine;
using System.Collections;
using GameStruct;

public class HintSkillView : MonoBehaviour {

	public UILabel SkillKindLabel;
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
			SkillKindLabel.text = TextConst.S(12101);
		else 
			SkillKindLabel.text = TextConst.S(12102);
		QualityCards.spriteName = "cardlevel_" + skill.Lv.ToString();
		SkillItemPic.spriteName = GameData.DSkillData[skill.ID].PictureNo + "s";
		if(GameData.DSkillData.ContainsKey(skill.ID))
			SkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[skill.ID].Star , 1 , 5).ToString();
		else {
			SkillStar.spriteName = "Staricon1";
			Debug.LogError("GameData.DSkillData is not ContainKey:" + skill.ID);
		}
		AmountLabel.text = GameData.Team.Player.GetSkillCount(skill.ID).ToString();
	}
}
