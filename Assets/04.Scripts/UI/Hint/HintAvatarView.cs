using GameStruct;
using UnityEngine;

public class HintAvatarView : MonoBehaviour {

	public UISprite[] AttrKindsIcon;
	public UILabel[] ValueLabels;
	public UISprite QualitySquare;
	public UISprite ItemPic;
	public GameObject[] AvatarStars;
	public UILabel AmountLabel;
	public UILabel ItemKindLabel;

	public GameObject Money;
	public GameObject EXP;
	public GameObject Gem;

	private Transform goQuality;
	public UISprite QualityBG;
	private GameObject mGameObject;
	private bool isHaveValue = false;
	private void Awake()
	{
		if(QualityBG == null)
			goQuality = GameFunction.FindQualityBG(transform);
		if(goQuality != null)
			QualityBG = goQuality.GetComponent<UISprite>();
		
		mGameObject = gameObject;

        if(Money != null)
		    Money.SetActive(false);

        if(EXP != null)
		    EXP.SetActive(false);

        if(Gem != null)
		    Gem.SetActive(false);
		
		for (int i=0; i<AttrKindsIcon.Length; i++){
			AttrKindsIcon[i].gameObject.SetActive(false);
		}

		for (int i=0; i<AvatarStars.Length; i++)
			AvatarStars[i].SetActive(false);
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


	public void UpdateUI(TItemData itemData)
	{
		isHaveValue = false;
		QualitySquare.spriteName = "Equipment_" + Mathf.Clamp(itemData.Quality, 1, 5).ToString();

		for (int i=0; i<itemData.Bonus.Length; i++) {
			AttrKindsIcon[i].gameObject.SetActive(true);
			AttrKindsIcon[i].spriteName = "AttrKind_" + itemData.Bonus[i].GetHashCode();
			if(itemData.BonusValues[i] == 0)
				ValueLabels[i].text = "";
			else
				ValueLabels[i].text = itemData.BonusValues[i].ToString();
			if(itemData.Kind >=0 && itemData.Kind <= 7){
				AttrKindsIcon[i].gameObject.SetActive(false);
				ValueLabels[i].gameObject.SetActive(false);
			} else {
				AttrKindsIcon[i].gameObject.SetActive(true);
				ValueLabels[i].gameObject.SetActive(true);
				isHaveValue = true;
			}
		}

		ItemKindLabel.gameObject.SetActive(!isHaveValue);
		if(!isHaveValue) {
			ItemKindLabel.text = TextConst.S(13000 + itemData.Kind);
		} 

		if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(itemData.Atlas))) {
			ItemPic.atlas = GameData.DItemAtlas[GameData.AtlasName(itemData.Atlas)];
		}

		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;
		
		if(QualityBG != null)
			QualityBG.color = TextConst.ColorBG(itemData.Quality);

		AmountLabel.text = "";
	}
}
