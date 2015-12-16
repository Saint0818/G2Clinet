using UnityEngine;
using System.Collections;
using GameStruct;

public class AwardInlayView : MonoBehaviour {

	public UISprite QualityOctagon;
	public UISprite ItemPic;
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

	public void UpdateUI (TItemData itemData) {
		QualityOctagon.spriteName = "Patch" + Mathf.Clamp(itemData.Quality, 1, 5).ToString();

		if(GameData.DItemAtlas.ContainsKey("AtlasItem_" + itemData.Kind)) {
			ItemPic.atlas = GameData.DItemAtlas["AtlasItem_" + itemData.Kind];
		}

		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;

		AmountLabel.text = "";
	}
}
