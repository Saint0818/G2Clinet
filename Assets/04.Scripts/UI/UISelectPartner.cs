using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameEnum;
using GameStruct;

public class TMember{
	public string TeamName;
	public int Index;
	public TPlayer Player;
	public GameObject Item;
	public UILabel LabelTeamName;
	public UILabel LabelPower;
	public UISprite SpriteFace;
}

public class UISelectPartner : UIBase {
	private static UISelectPartner instance = null;
	private const string UIName = "UISelectPartner";

	private List<TMember> memberList = new List<TMember>();
	private GameObject itemMember;
	private UIScrollView scrollView;
	private int selectIndex;

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if(instance) {
			if (!isShow) 
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
		if(isShow)
			Get.Show(isShow);
	}
	
	public static UISelectPartner Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISelectPartner;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		SetBtnFun (UIName + "/Center/Close", OnClose);

		itemMember = Resources.Load("Prefab/UI/Items/ItemSelectPartner") as GameObject;
		scrollView = GameObject.Find(UIName + "/Center/ScrollView").GetComponent<UIScrollView>();
	}

	public void OnClose(){
		UIShow(false);
	}

	public void InitMemberList(ref List<TPlayer> playerList, int index) {
		selectIndex = index;
		for (int i = 0; i < memberList.Count; i ++)
			memberList[i].Item.SetActive(false);

		for (int i = 0; i < playerList.Count; i++)
			addMember(i, playerList[i]);
	}

	private void addMember(int index, TPlayer player) {
		if (index >= memberList.Count) {
			TMember team = new TMember();
			team.Item = Instantiate(itemMember, Vector3.zero, Quaternion.identity) as GameObject;
			team.Item.name = index.ToString();
			UIButton btn = team.Item.GetComponent<UIButton>();
			SetBtnFun(ref btn, OnSelectPartner);
			SetBtnFun(UIName + "/PlayerInGameBtn", OnPlayerInfo);
			team.Item.GetComponent<UIDragScrollView>().scrollView = scrollView;
			team.LabelTeamName = GameObject.Find(team.Item.name + "/PlayerName/NameLabel").GetComponent<UILabel>();
			team.LabelPower = GameObject.Find(team.Item.name + "/CombatGroup/CombatValueLabel").GetComponent<UILabel>();
			team.SpriteFace = GameObject.Find(team.Item.name + "/PlayerInGameBtn").GetComponent<UISprite>();
		
			int a = index / 2;
			int b = index % 2;
			team.Item.transform.parent = scrollView.gameObject.transform;
			team.Item.transform.localPosition = new Vector3(170 - b * 350, 40 - a * 140, 0);
			team.Item.transform.localScale = Vector3.one;
			memberList.Add(team);
			index = memberList.Count-1;
		}
		
		memberList[index].Index = index;
		memberList[index].Player = player;
		
		memberList[index].Item.SetActive(true);
		memberList[index].LabelTeamName.text = player.Name;
		memberList[index].LabelPower.text = string.Format(TextConst.S(9509), + player.Power());
		memberList[index].SpriteFace.spriteName = player.FacePicture; 
	}

	public void OnSelectPartner() {
		if (UISelectRole.Visible) {
			int index = -1;
			if (int.TryParse(UIButton.current.name, out index)) {
				UISelectRole.Get.SetPlayerAvatar(selectIndex, index);
			}
		}
	}

	public void OnPlayerInfo() {
		OnSelectPartner();
	}
}

