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

	private GameObject mGameObject;
	private void Awake()
	{
		mGameObject = gameObject;
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
		QualitySquare.spriteName = "Patch" + itemData.Quality.ToString();
		ItemPic.spriteName = "Item_" + itemData.Kind;
		for (int i=0; i<AvatarStars.Length; i++)
			AvatarStars[i].SetActive((i < itemData.LV));

		AmountLabel.text = "";
	}
}
