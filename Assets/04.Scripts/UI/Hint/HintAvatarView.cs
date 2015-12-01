using UnityEngine;
using System.Collections;
using GameStruct;

public class HintAvatarView : MonoBehaviour {

	public GameObject[] AttrKinds;
	public UILabel[] ValueLabels;
	public UISprite QualitySquare;
	public UISprite ItemPic;
	public GameObject[] AvatarStars;
	public UILabel AmountLabel;

	public GameObject Money;
	public GameObject EXP;
	public GameObject Gem;

	private GameObject mGameObject;
	private void Awake()
	{
		mGameObject = gameObject;
		Money.SetActive(false);
		EXP.SetActive(false);
		Gem.SetActive(false);
		for (int i=0; i<AttrKinds.Length; i++)
			AttrKinds[i].SetActive(false);
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
		QualitySquare.spriteName = "Patch" + Mathf.Clamp(itemData.Quality, 1, 5).ToString();
		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;

		for (int i=0; i<AvatarStars.Length; i++)
			AvatarStars[i].SetActive((i < itemData.LV));

		AmountLabel.text = "";
	}
}
