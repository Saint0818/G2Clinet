using UnityEngine;
using System.Collections;
using GameStruct;

public class AwardAvatarView : MonoBehaviour {
	public GameObject EXP;
	public GameObject Money;
	public GameObject Gem;

	public GameObject EquipmentStar;

	public UISprite QualitySquare;
	public UISprite ItemPic;
	public GameObject[] AvatarStars;
	public UILabel AmountLabel;

	private GameObject mGameObject;
	private void Awake()
	{
		mGameObject = gameObject;
		Hide();
	}

	private void hideAll () {
		ItemPic.gameObject.SetActive(false);
		EXP.SetActive(false);
		Money.SetActive(false);
		Gem.SetActive(false);
		EquipmentStar.SetActive(false);
	}

	public void Show () {
		mGameObject.SetActive(true);
	}

	public void Hide () {
		mGameObject.SetActive(false);
	}

	public void UpdateUI (TItemData itemData) {
		hideAll ();
		Show ();
		QualitySquare.spriteName = "Equipment_"+ Mathf.Clamp(itemData.Quality, 1, 5).ToString();
		ItemPic.gameObject.SetActive(true);
		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;

		if(itemData.LV > 0) {
			EquipmentStar.SetActive(true);
			for (int i=0; i<AvatarStars.Length; i++)
				AvatarStars[i].SetActive((i < itemData.LV));
		} else 
			EquipmentStar.SetActive(false);
		AmountLabel.text = "";
	}

	public void UpdateExp (int value) {
		hideAll () ;
		Show ();
		EXP.SetActive(true);
		AmountLabel.text = value.ToString();
	}
	
	public void UpdateMoney (int value) {
		hideAll () ;
		Show ();
		Money.SetActive(true);
		AmountLabel.text = value.ToString();
	}
	
	public void UpdateGem (int value) {
		hideAll () ;
		Show ();
		Gem.SetActive(true);
		AmountLabel.text = value.ToString();
	}
}
