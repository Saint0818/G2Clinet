using UnityEngine;
using System.Collections;
using GameStruct;

public class HintInlayView : MonoBehaviour {
	
	public GameObject[] AttrKinds;
	public UILabel[] ValueLabels;
	public UISprite QualityOctagon;
	public UISprite ItemPic;
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
		QualityOctagon.spriteName = "Equipment_" + itemData.Kind;
		ItemPic.spriteName = "Item_" + itemData.Kind;
		
		AmountLabel.text = "";
	}
}
