using GameStruct;
using UnityEngine;

public class AwardAvatarView : MonoBehaviour {
	public GameObject EXP;
	public GameObject Money;
	public GameObject Gem;
	public Transform PVPCoin;

	//for gameresult
	private UILabel labelItemName;

	public GameObject EquipmentStar;

	public UISprite QualitySquare;
	public UISprite ItemPic;
	public GameObject[] EmptyStars;
	public GameObject[] AvatarStars;
	public UILabel AmountLabel;
	private Transform goQuality;
	public UISprite QualityBG;

	private Transform specialEffect;

	private GameObject mGameObject;
	private void Awake()
	{
		if(QualityBG == null)
			goQuality = GameFunction.FindQualityBG(transform);
		if(goQuality != null)
			QualityBG = goQuality.GetComponent<UISprite>();
		if(specialEffect == null)
			specialEffect = transform.Find("ItemView/SpecialEffect");
		if(PVPCoin == null)
			PVPCoin = transform.Find("ItemView/PVPCoin");
		if(labelItemName == null) {
			Transform t = transform.Find("ItemLabel");
			if(t != null)
				labelItemName = t.GetComponent<UILabel>();
		}

		mGameObject = gameObject;
		Hide();
	}

	private void hideAll () {
		ItemPic.gameObject.SetActive(false);
		EXP.SetActive(false);
		Money.SetActive(false);
		Gem.SetActive(false);
		if(PVPCoin != null)
			PVPCoin.gameObject.SetActive(false);
		EquipmentStar.SetActive(false);
		
		for (int i=0; i<AvatarStars.Length; i++)
			AvatarStars[i].SetActive(false);

		if(EmptyStars != null)
			for (int i=0; i<EmptyStars.Length; i++) 
				EmptyStars[i].SetActive(false);
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
		if(EmptyStars != null)
			GameFunction.ShowInlay(ref EmptyStars, ref AvatarStars, GameData.Team.Player, itemData.Kind);
		else 
			Debug.LogError("EmptyStars not setting");
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

		if(specialEffect != null)
			specialEffect.gameObject.SetActive((itemData.Flag == 1));

		if(itemData.Kind == 31 || itemData.Kind == 32 || itemData.Kind == 33) {
			AmountLabel.text = itemData.Value.ToString();
			if(labelItemName != null) 
				labelItemName.text = string.Format(itemData.Name, itemData.Value);
		} else {
			AmountLabel.text = "";
			if(labelItemName != null) 
				labelItemName.text = itemData.Name;
		}
	}

	/// <summary>
	/// kind 1:Money 2:Gem 3:EXP 4:PVP
	/// </summary>
	/// <param name="kind">Kind.</param>
	/// <param name="value">Value.</param>
	public void UpdateOther (int kind, int value) {
		hideAll () ;
		Show ();
		switch(kind) {
		case 1:
			Money.SetActive(true);
			break;
		case 2:
			Gem.SetActive(true);
			break;
		case 3:
			EXP.SetActive(true);
			break;
		case 4:
			if(PVPCoin != null)
				PVPCoin.gameObject.SetActive(true);
			break;
		}
		AmountLabel.text = value.ToString();
		if(QualityBG != null)
			QualityBG.color = TextConst.ColorBG(1);
		if(specialEffect != null)
			specialEffect.gameObject.SetActive(false);
	}

}
