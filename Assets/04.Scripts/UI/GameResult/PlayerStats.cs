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

	public UILabel[] FollowLabel = new UILabel[6];
	public GameObject ViewAttrBtn;

	private int result = 0;
	void Awake () {
		for(int i=0; i<AddFriendBtn.Length; i++) {
			AddFriendBtn[i].gameObject.name = i.ToString();
			UIEventListener.Get(AddFriendBtn[i].gameObject).onClick = OnMakeFriend;
			AddFriendBtn[i].gameObject.SetActive(false);
		}
		UIEventListener.Get(ViewAttrBtn).onClick = OnOpenAttrbute;

		for(int i=0; i<FollowLabel.Length; i++)
			FollowLabel[i].text = TextConst.S(5023);
	}

	public void PlayerViewVisible(int index, bool isShow) {
		PlayerNameView[index].SetActive(isShow);
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
			if(!string.IsNullOrEmpty(tempID[i])) {
				if(GameData.Team.CheckFriend(tempID[i]) || tempID[i].Equals(GameData.Team.Identifier))
					AddFriendBtn[i].gameObject.SetActive(false);
				else {
					AddFriendBtn[i].gameObject.SetActive(true);
					AddFriendBtn[i].normalSprite = "IconLike";
				}
			} else 
				AddFriendBtn[i].gameObject.SetActive(false);
		}
	}

	public void AddFriendSuccess () {
		UIHint.Get.ShowHint(string.Format(TextConst.S(5027), PlayerName[result].text), Color.red, true);
		CheckFriend ();
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

	public void OnMakeFriend(GameObject go) {
		result = 0;
		if(int.TryParse(go.name, out result)) {
			if(!string.IsNullOrEmpty(tempID[result])) {
				SendHttp.Get.MakeFriend(AddFriendSuccess, tempID[result]);
			} 
		}
	}
}
