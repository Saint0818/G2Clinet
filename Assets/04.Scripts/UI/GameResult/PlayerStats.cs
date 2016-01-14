﻿using UnityEngine;

public class PlayerStats : MonoBehaviour {
	private string[] positionPicName = {"IconCenter", "IconForward", "IconGuard"};
	public GameObject[] PlayerNameView = new GameObject[6];
	public UILabel[] PlayerName = new UILabel[6];
	public UIButton[] AddFriendBtn = new UIButton[3];
	public GameObject[] PlayerInGameBtn = new GameObject[6];
	public UISprite[] PlayerIcon = new UISprite[6];
	public UISprite[] PositionIcon = new UISprite[6];
	public string[] tempID = new string[6];

	public GameObject ViewAttrBtn;

	void Awake () {
		for(int i=0; i<AddFriendBtn.Length; i++) {
			AddFriendBtn[i].gameObject.SetActive(false);
		}

		UIEventListener.Get(ViewAttrBtn).onClick = OnOpenAttrbute;
	}

	public void ShowAddFriendBtn (int index, string id) {
		if(index >= 0 && index < 3) {
			tempID[index] = id;
			AddFriendBtn[index].gameObject.SetActive(true);
			AddFriendBtn[index].name = index.ToString();
		}
	}

	public void HideAddFriendBtn (int index) {
		if(index >= 0 && index < 3)
			AddFriendBtn[index].gameObject.SetActive(false);
	}

	public void SetPlayerName (int index, string name) {
		if(index >= 0 && index < 6)
			PlayerName[index].text = name;
	}

	public void SetPlayerIcon (int index, string name) {
		if(index >= 0 && index < 6)
			PlayerIcon[index].spriteName = name;
	}

	public void SetPositionIcon (int index, int bodyType) {
		if(index >= 0 && index < 6)
			PositionIcon[index].spriteName = positionPicName[bodyType];
	}


	public void OnOpenAttrbute (GameObject go) {
		UIAttributeExplain.UIShow(true);
	}
}
