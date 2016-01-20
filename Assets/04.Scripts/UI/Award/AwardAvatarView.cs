using GameStruct;
using UnityEngine;

public class AwardAvatarView : MonoBehaviour {
	public GameObject EXP;
	public GameObject Money;
	public GameObject Gem;

	public GameObject EquipmentStar;

	public UISprite QualitySquare;
	public UISprite ItemPic;
	public GameObject[] AvatarStars;
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

	private void hideAll () {
		ItemPic.gameObject.SetActive(false);
		EXP.SetActive(false);
		Money.SetActive(false);
		Gem.SetActive(false);
		EquipmentStar.SetActive(false);
		
		for (int i=0; i<AvatarStars.Length; i++)
			AvatarStars[i].SetActive(false);
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
		QualitySquare.spriteName = "Equipment_" + itemData.Quality.ToString();

		if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(itemData.Atlas))) {
			ItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(itemData.Atlas)];
		}

		ItemPic.gameObject.SetActive(true);
		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;

		if(QualityBG != null)
			QualityBG.color = TextConst.ColorBG(itemData.Quality);

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
