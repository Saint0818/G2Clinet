using UnityEngine;
using System.Collections;
using GameStruct;

public class PlayerStats : MonoBehaviour {
	public GameObject[] PlayerNameView = new GameObject[6];
	public UILabel[] PlayerName = new UILabel[6];
	public GameObject[] AddFriendBtn = new GameObject[3];
	public GameObject[] PlayerInGameBtn = new GameObject[6];
	public UISprite[] PlayerIcon = new UISprite[6];

	void Awake () {
		for(int i=0; i<AddFriendBtn.Length; i++) {
			AddFriendBtn[i].SetActive(false);
		}
	}

	public void ShowAddFriendBtn (int index) {
		if(index >= 0 && index < 3)
			AddFriendBtn[index].SetActive(true);
	}

	public void SetPlayerName (int index, string name) {
		if(index >= 0 && index < 6)
			PlayerName[index].text = name;
	}

	public void SetPlayerIcon (int index, int position) {
		if(index >= 0 && index < 6)
			PlayerIcon[index].spriteName = getPosition(position) + "s";
	}

	private string getPosition (int position) {
		switch (position) {
		case 0:
			return "11800";
		case 1:
			return "12100";
		case 2:
			return "13000";
		default:
			return "11800";
		}
	}
}
