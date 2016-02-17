using GameStruct;
using UnityEngine;

public class AwardSkillView : MonoBehaviour {

	public UISprite QualityCards;
	public UISprite SkillItemPic;
	public SkillCardStar[] SkillStars;
	public UILabel AmountLabel;

	private Transform goQuality;
	private Transform specialEffect;
	public UISprite QualityBG;
	private GameObject mGameObject;
	private void Awake()
	{
		if(QualityBG == null)
			goQuality = GameFunction.FindQualityBG(transform);
		if(goQuality != null)
			QualityBG = goQuality.GetComponent<UISprite>();
		if(specialEffect == null)
			specialEffect = transform.FindChild("ItemView/SpecialEffect");


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
			QualityCards.spriteName = GameFunction.CardLevelName(itemData.Avatar);
			SkillItemPic.spriteName = GameData.DSkillData[itemData.Avatar].PictureNo + "s";
			GameFunction.ShowStar(ref SkillStars, itemData.LV, GameData.DSkillData[itemData.Avatar].Quality, GameData.DSkillData[itemData.Avatar].MaxStar);
		}
		if(QualityBG != null)
			QualityBG.color = TextConst.ColorBG(itemData.Quality);

		if(specialEffect != null)
			specialEffect.gameObject.SetActive((itemData.Flag == 1));
		
		AmountLabel.text = "";
	}

	public void UpdateUI (TSkill skill) {
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(21))) {
				SkillItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(21)];
			}

			QualityCards.spriteName = GameFunction.CardLevelName(skill.ID);
			SkillItemPic.spriteName = GameData.DSkillData[skill.ID].PictureNo + "s";
			GameFunction.ShowStar(ref SkillStars, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);
		}
		if(QualityBG != null)
			QualityBG.color = TextConst.ColorBG(GameData.DSkillData[skill.ID].Quality);
		
		AmountLabel.text = "";
	}
}
