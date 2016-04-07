using GameStruct;
using UnityEngine;

public class AwardInlayView : MonoBehaviour {

	public UISprite QualityOctagon;
	public UISprite ItemPic;
	public UILabel AmountLabel;
	private Transform goQuality;
	public UISprite QualityBG;
	private Transform specialEffect;

	private GameObject mGameObject;

	//for gameresult
	private UILabel labelItemName;
	private void Awake()
	{
		if(QualityBG == null)
			goQuality = GameFunction.FindQualityBG(transform);
		if(goQuality != null)
			QualityBG = goQuality.GetComponent<UISprite>();
		if(specialEffect == null)
			specialEffect = transform.FindChild("ItemView/SpecialEffect");
		if(labelItemName == null) {
			Transform t = transform.Find("ItemLabel");
			if(t != null)
				labelItemName = t.GetComponent<UILabel>();
		}
		
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
		
		if(specialEffect != null)
			specialEffect.gameObject.SetActive((itemData.Flag == 1));

		AmountLabel.text = "";

		if(labelItemName != null) 
			labelItemName.text = itemData.Name;
	}
}
