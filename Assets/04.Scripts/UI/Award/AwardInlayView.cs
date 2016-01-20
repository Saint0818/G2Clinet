using GameStruct;
using UnityEngine;

public class AwardInlayView : MonoBehaviour {

	public UISprite QualityOctagon;
	public UISprite ItemPic;
	public UILabel AmountLabel;
	private Transform goQuality;
	public UISprite QualityBG;

	private GameObject mGameObject;
	private void Awake()
	{
		if(QualityBG == null)
			goQuality = GameFunction.FindQualityBG(transform);
		if(goQuality != null)
			QualityBG = goQuality.GetComponent<UISprite>();
		
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

		if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(itemData.Atlas))) {
			ItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(itemData.Atlas)];
		}
		QualityOctagon.spriteName = "Patch" + Mathf.Clamp(itemData.Quality, 1, 5).ToString();

		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;

		if(QualityBG != null)
			QualityBG.color = TextConst.ColorBG(itemData.Quality);
		AmountLabel.text = "";
	}
}
