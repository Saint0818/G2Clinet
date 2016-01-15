using System.Collections.Generic;
using GameStruct;
using UnityEngine;

public class TMember{
	public string TeamName;
	public int Index;
	public TPlayer Player;
	public GameObject Item;
    public GameObject UISelected;
    public UILabel LabelSelected;
	public UILabel LabelTeamName;
	public UILabel LabelPower;
    public UILabel LabelLv;
	public UISprite SpriteFace;
    public UISprite SpritePosition;
}

public class UISelectPartner : UIBase {
	private static UISelectPartner instance = null;
	private const string UIName = "UISelectPartner";

    private List<TPlayer> tempPlayerList;
    private TPlayer[] tempSelectAy;
    private int tempIndex;

	private List<TMember> memberList = new List<TMember>();
	private List<TPassiveSkillCard> skillList = new List<TPassiveSkillCard>();
	private GameObject itemMember;
	private GameObject itemSkill;
    private UIPanel partnerPanel;
	private UIScrollView partnerScrollView;
	private UIScrollView skillScrollView;
	private int selectIndex;
    private int nowPage = 0;

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
                Get.Show(isShow);
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
        SetBtnFun (UIName + "/Right/Window/Close", OnClose);
        for (int i = 0; i < 2; i++)
            SetBtnFun (UIName + "/Right/Window/Tabs/" + i.ToString(), OnPage);

		itemMember = Resources.Load("Prefab/UI/Items/ItemSelectPartner") as GameObject;
		itemSkill = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;
        partnerPanel = GameObject.Find(UIName + "/Right/Window/Partner/ScrollView").GetComponent<UIPanel>();
		partnerScrollView = GameObject.Find(UIName + "/Right/Window/Partner/ScrollView").GetComponent<UIScrollView>();
        skillScrollView = GameObject.Find(UIName + "/Right/Window/Skill/ScrollView").GetComponent<UIScrollView>();
	}

	public void OnClose(){
        UISelectRole.Get.InitPlayer();
		UIShow(false);
	}

    public void OnPage() {
        int i = -1;
        if (int.TryParse(UIButton.current.name, out i)) {
            nowPage = i;
            InitMemberList(ref tempPlayerList, ref tempSelectAy, tempIndex);
            partnerScrollView.transform.localPosition = Vector3.zero;
            partnerPanel.clipOffset = Vector2.zero;
        }
    }

	private void initSkillList(int index) {
		for (int i = 0; i < skillList.Count; i++)
			skillList[i].Enable = false;

		for (int i = 0; i < memberList[index].Player.SkillCards.Length; i++)
			addSkillItem(i, memberList[index].Player.SkillCards[i]); 
	}

	private void addSkillItem(int index, TSkill skill) {
		if (index >= skillList.Count) {
			TPassiveSkillCard sc = new TPassiveSkillCard();
			GameObject obj = Instantiate(itemSkill) as GameObject;
			sc.Init(skillScrollView.gameObject, obj, new EventDelegate(OnSkillHint));

			obj.name = skill.ID.ToString();
			LayerMgr.Get.SetLayer(obj, ELayer.TopUI);
			obj.GetComponent<UIDragScrollView>().scrollView = skillScrollView;
			skillList.Add(sc);
		}

		skillList[index].Enable = true;
		skillList[index].UpdateView(index, skill, new Vector3(0, 145-index * 70, 0));
	}

	public void OnSkillHint() {
		int index = -1;
		if (int.TryParse(UIButton.current.name, out index) && index >= -1 && index < skillList.Count)
            UISkillInfo.Get.ShowFromNewCard(skillList[index].Skill);
	}

    private void checkSelected(ref TPlayer[] selectAy) {
        for (int i = 0; i < memberList.Count; i ++) {
            memberList[i].LabelSelected.text = "";
            memberList[i].UISelected.SetActive(false);
        }

        for (int i = 0; i < memberList.Count; i++)
            for (int j = 0; j < selectAy.Length; j++) 
                if (memberList[i].Player.RoleIndex == selectAy[j].RoleIndex) {
                    memberList[i].UISelected.SetActive(true);
                    memberList[i].LabelSelected.text = TextConst.S(9511+j);
                }
    }

	public void InitMemberList(ref List<TPlayer> playerList, ref TPlayer[] selectAy, int index) {
        tempPlayerList = playerList;
        tempSelectAy = selectAy;
        tempIndex = index;

		selectIndex = index;
		for (int i = 0; i < memberList.Count; i ++) {
            memberList[i].LabelSelected.text = "";
            memberList[i].UISelected.SetActive(false);
			memberList[i].Item.SetActive(false);
		}

        int count = 0;
        for (int i = 0; i < playerList.Count; i++) {
            if (nowPage == 1) { //friend
                if (playerList[i].FriendKind == EFriendKind.Friend) {
    			    addMember(count, playerList[i]);
                    count++;
                }
            } else {
                if (playerList[i].FriendKind != EFriendKind.Friend) {
                    addMember(count, playerList[i]);
                    count++;
                }
            }
        }
		
        checkSelected(ref selectAy);
        
		//if (index >= 0 && index < selectAy.Length)
		//	initSkillList(selectAy[index].RoleIndex);
	}

	private void addMember(int index, TPlayer player) {
		if (index >= memberList.Count) {
			TMember team = new TMember();
			team.Item = Instantiate(itemMember, Vector3.zero, Quaternion.identity) as GameObject;
			team.Item.name = index.ToString();
			UIButton btn = team.Item.GetComponent<UIButton>();
			SetBtnFun(ref btn, OnSelectPartner);
            SetLabel(team.Item.name + "/CombatGroup/Label", TextConst.S(3019));
			team.Item.GetComponent<UIDragScrollView>().scrollView = partnerScrollView;
			team.LabelTeamName = GameObject.Find(team.Item.name + "/PlayerName/NameLabel").GetComponent<UILabel>();
			team.LabelPower = GameObject.Find(team.Item.name + "/CombatGroup/CombatValueLabel").GetComponent<UILabel>();
            team.LabelLv = GameObject.Find(team.Item.name + "/PlayerInGameBtn/LevelGroup").GetComponent<UILabel>();
            team.SpriteFace = GameObject.Find(team.Item.name + "/PlayerInGameBtn/PlayerPic").GetComponent<UISprite>();
            team.SpritePosition = GameObject.Find(team.Item.name + "/PlayerInGameBtn/PlayerPic/PositionIcon").GetComponent<UISprite>();
            team.UISelected = GameObject.Find(team.Item.name + "/UISelected");
            team.LabelSelected = GameObject.Find(team.Item.name + "/UISelected/Label").GetComponent<UILabel>();
			GameObject obj = GameObject.Find(team.Item.name + "/UISelected/Label");
			if (obj) {
				UILabel lab = obj.GetComponent<UILabel>();
				if (lab)
					lab.text = TextConst.S(9510);
			}

			team.Item.transform.parent = partnerScrollView.gameObject.transform;
			team.Item.transform.localScale = Vector3.one;
			memberList.Add(team);
			index = memberList.Count-1;
		}
		
		memberList[index].Index = index;
		memberList[index].Player = player;
		
		memberList[index].Item.SetActive(true);
        memberList[index].LabelSelected.text = "";
        memberList[index].UISelected.SetActive(false);
		memberList[index].LabelTeamName.text = player.Name;
        memberList[index].LabelPower.text = ((int) player.CombatPower()).ToString();
        memberList[index].LabelLv.text = player.Lv.ToString();
		memberList[index].SpriteFace.spriteName = player.FacePicture;
        memberList[index].Item.transform.localPosition = new Vector3(0, 120 - index * 130, 0);
        if (GameData.DPlayers.ContainsKey(player.ID)) {
            if (GameData.DPlayers[player.ID].Body == 0)
                memberList[index].SpritePosition.spriteName = "IconCenter";
            else
            if (GameData.DPlayers[player.ID].Body == 1)
                memberList[index].SpritePosition.spriteName = "IconForward";
            else
                memberList[index].SpritePosition.spriteName = "IconGuard";
        }
	}

	public void OnSelectPartner() {
		if (UISelectRole.Visible) {
			int index = -1;
			if (int.TryParse(UIButton.current.name, out index)) {
				int i = UISelectRole.Get.GetSelectedListIndex(selectIndex);
				int j = -1;
				if (selectIndex == 1)
				 	j = UISelectRole.Get.GetSelectedListIndex(2);
				else
					j = UISelectRole.Get.GetSelectedListIndex(1);

                if (i != memberList[index].Player.RoleIndex && j != memberList[index].Player.RoleIndex) {
                    tempSelectAy[selectIndex].RoleIndex = memberList[index].Player.RoleIndex;
                    checkSelected(ref tempSelectAy);
                    UISelectRole.Get.SetPlayerAvatar(selectIndex, memberList[index].Player.RoleIndex);
				}
				
				initSkillList(index);
			}
		}
	}
}

