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
		QualityOctagon.spriteName = "Patch" + itemData.Quality.ToString();
		ItemPic.spriteName = "item_" + itemData.Kind;
		AmountLabel.text = "";
	}
}
