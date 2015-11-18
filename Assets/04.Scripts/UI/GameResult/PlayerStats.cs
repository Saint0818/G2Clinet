using UnityEngine;
using System.Collections;
using GameStruct;

public class PlayerStats : MonoBehaviour {
	public GameObject[] PlayerNameView = new GameObject[6];
	public UILabel[] PlayerName = new UILabel[6];
	public GameObject[] AddFriendBtn = new GameObject[3];
	public GameObject[] Player = new GameObject[6];

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

	public void ShowSelf () {
		gameObject.SetActive(true);

		for(int i=0; i<Player.Length; i++) {
			if(i < 3) {
				Player[i].SetActive(true);
				PlayerNameView[i].SetActive(true);
			} else {
				Player[i].SetActive(false);
				PlayerNameView[i].SetActive(false);
			}
		}
	}

	public void ShowNPC () {
		gameObject.SetActive(true);
		for(int i=0; i<Player.Length; i++) {
			if(i >= 3) {
				Player[i].SetActive(true);
				PlayerNameView[i].SetActive(true);
			} else {
				Player[i].SetActive(false);
				PlayerNameView[i].SetActive(false);
			}
		}
	}

	public void Hide (){
		gameObject.SetActive(false);
	}
}
