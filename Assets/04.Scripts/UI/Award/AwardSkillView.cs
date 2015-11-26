using UnityEngine;
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
		SkillItemPic.spriteName = itemData.Icon;
		SkillStar.spriteName = "Staricon" + itemData.LV;
		AmountLabel.text = "";
	}
}
