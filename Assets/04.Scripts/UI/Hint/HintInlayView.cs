using UnityEngine;
using System.Collections;
using GameStruct;

public class HintInlayView : MonoBehaviour {
	
	public UISprite[] AttrKindsIcon;
	private UISprite[] attrKindsIcon;
	public UILabel[] ValueLabels;
	public UISprite QualityOctagon;
	public UISprite ItemPic;
	public UILabel AmountLabel;

	private GameObject mGameObject;
	private void Awake()
	{
		mGameObject = gameObject;
		for (int i=0; i<AttrKindsIcon.Length; i++){
			AttrKindsIcon[i].gameObject.SetActive(false);
		}
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
		QualityOctagon.spriteName = "Patch" + Mathf.Clamp(itemData.Quality, 1, 5).ToString();

		UIValueItemData value = UIEquipUtility.Convert(itemData);
		
		for (int i=0; i<itemData.Bonus.Length; i++) {
			AttrKindsIcon[i].gameObject.SetActive(true);
			AttrKindsIcon[i].spriteName = "AttrKind_" + itemData.Bonus[i].GetHashCode();
		}
		
		for (int i=0; i<itemData.BonusValues.Length; i++) {
			ValueLabels[i].text = itemData.BonusValues[i].ToString();
		}


		if(string.IsNullOrEmpty(itemData.Icon))
			ItemPic.spriteName = "Item_999999";
		else
			ItemPic.spriteName = "Item_" + itemData.Icon;
		
		AmountLabel.text = "";
	}
}
