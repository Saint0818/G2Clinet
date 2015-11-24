using UnityEngine;
using System.Collections;
using GameStruct;

public class AwardAvatarView : MonoBehaviour {
	public UISprite ItemPic;
	public GameObject[] AvatarStars;
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

	}
}
