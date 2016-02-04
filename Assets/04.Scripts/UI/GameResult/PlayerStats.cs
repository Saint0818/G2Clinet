using UnityEngine;

public class PlayerStats : MonoBehaviour {
	private string[] positionPicName = {"IconCenter", "IconForward", "IconGuard"};
	public GameObject[] PlayerNameView = new GameObject[6];
	public UILabel[] PlayerName = new UILabel[6];
	public UIButton[] AddFriendBtn = new UIButton[6];
	public GameObject[] PlayerInGameBtn = new GameObject[6];
	public UISprite[] PlayerIcon = new UISprite[6];
	public UISprite[] PositionIcon = new UISprite[6];
	public string[] tempID = new string[6];

	public GameObject ViewAttrBtn;

	void Awake () {
		for(int i=0; i<AddFriendBtn.Length; i++) {
			UIEventListener.Get(AddFriendBtn[i].gameObject).onClick = OnOpenAttrbute;
			AddFriendBtn[i].gameObject.SetActive(false);
		}

		UIEventListener.Get(ViewAttrBtn).onClick = OnOpenAttrbute;
	}

	public void SetID (int index, string id) {
		if(index >= 0 && index < 6) 
			tempID[index] = id;
	}

	public void ShowAddFriendBtn (int index) {
		if(index >= 0 && index < 6) 
			AddFriendBtn[index].name = index.ToString();
	}

	public void CheckFriend () {
		for(int i=0; i<tempID.Length; i++) {
			if(string.IsNullOrEmpty(tempID[i])) {
				if(GameData.Team.CheckFriend(tempID[i]))
					AddFriendBtn[i].normalSprite = "IconDislike";
				else 
					AddFriendBtn[i].normalSprite = "IconLike";
			} else {
				AddFriendBtn[i].gameObject.SetActive(false);
			}
		}
	}

	public void HideAddFriendBtn (int index) {
		if(index >= 0 && index < 6)
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

	public void OnMakeFriend(int index) {
		if(!string.IsNullOrEmpty(tempID[index])) {
			SendHttp.Get.MakeFriend(CheckFriend, tempID[index]);
		} 
	}
}
