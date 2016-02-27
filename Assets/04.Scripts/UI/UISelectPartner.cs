using System.Collections.Generic;
using GameStruct;
using UnityEngine;

public class TMember{
	public string TeamName;
	public int Index;
	public TPlayer Player;
	public GameObject Item;
    public GameObject UISelected;
    public GameObject UIEquiptment;
    public UILabel LabelSelected;
	public UILabel LabelTeamName;
    public UILabel LabelFightCount;
	public UILabel LabelPower;
    public UILabel LabelLv;
	public UISprite SpriteFace;
    public UISprite SpritePosition;
}

public class UISelectPartner : UIBase {
	private static UISelectPartner instance = null;
	private const string UIName = "UISelectPartner";
    private const int pageNum = 2;

    private TPlayer[] tempSelectAy;

    private List<TMember>[] memberList = new List<TMember>[pageNum];
	private List<TPassiveSkillCard> skillList = new List<TPassiveSkillCard>();
	private GameObject itemMember;
	private GameObject itemSkill;
    private GameObject itemEquipt;
    private GameObject[] partnerUI = new GameObject[pageNum];
    private UIToggle[] pageToggle = new UIToggle[pageNum];
    private UIScrollView[] partnerScrollView = new UIScrollView[pageNum];
	private UIScrollView skillScrollView;
    private UIPanel skillPanel;
	private int selectIndex;
    private int nowPage = 0;

    public static bool Visible {
        get {
            if(instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }

        set {
            if (instance) {
                if (!value)
                    RemoveUI(UIName);
                else
                    instance.Show(value);
            } else
            if (value)
                Get.Show(value);
        }
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
        for (int i = 0; i < pageNum; i++) {
            SetBtnFun (UIName + "/Right/Window/Tabs/" + i.ToString(), OnPage);
            partnerUI[i] = GameObject.Find(UIName + "/Right/Window/Partner/Pages/" + i.ToString());
            partnerScrollView[i] = GameObject.Find(UIName + "/Right/Window/Partner/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIScrollView>();
            pageToggle[i] = GameObject.Find(UIName + "/Right/Window/Tabs/" + i.ToString()).GetComponent<UIToggle>();
        }

        itemEquipt = Resources.Load("Prefab/UI/Items/UIEquipItem") as GameObject;
		itemMember = Resources.Load("Prefab/UI/Items/ItemSelectPartner") as GameObject;
		itemSkill = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;
        skillPanel = GameObject.Find(UIName + "/Right/Window/Skill/ScrollView").GetComponent<UIPanel>();
        skillScrollView = GameObject.Find(UIName + "/Right/Window/Skill/ScrollView").GetComponent<UIScrollView>();
	}

	public void OnClose(){
        UISelectRole.Get.InitPartnerPosition();
        Visible = false;
	}

    public void OnPage() {
        int index = -1;
        if (int.TryParse(UIButton.current.name, out index)) {
            nowPage = index;

            for (int i = 0; i < partnerUI.Length; i++)
                partnerUI[i].SetActive(false);

            partnerUI[nowPage].SetActive(true);
        }
    }

	private void initSkillList(int index) {
		for (int i = 0; i < memberList[nowPage][index].Player.SkillCards.Length; i++)
            addSkillItem(i, memberList[nowPage][index].Player.SkillCards[i]);

        for (int i = memberList[nowPage][index].Player.SkillCards.Length; i < skillList.Count; i++)
            skillList[i].Enable = false;
        
        skillScrollView.gameObject.transform.localPosition = Vector3.zero;
        skillPanel.clipOffset = Vector2.zero;
	}

	private void addSkillItem(int index, TSkill skill) {
		if (index >= skillList.Count) {
			TPassiveSkillCard sc = new TPassiveSkillCard();
			GameObject obj = Instantiate(itemSkill) as GameObject;
			sc.Init(skillScrollView.gameObject, obj, new EventDelegate(OnSkillHint));

			obj.name = skill.ID.ToString();
			obj.GetComponent<UIDragScrollView>().scrollView = skillScrollView;
			skillList.Add(sc);
		}

		skillList[index].Enable = true;
		skillList[index].UpdateView(index, skill, new Vector3(0, 148 -index * 70, 0));
	}

	public void OnSkillHint() {
		int index = -1;
		if (int.TryParse(UIButton.current.name, out index) && index >= -1 && index < skillList.Count)
            UISkillInfo.Get.ShowFromNewCard(skillList[index].Skill);
	}

    public void OnItemHint() {
        if (UIButton.current.transform.parent && UIButton.current.transform.parent.parent) {
            char[] c = {'-'};
            string[] s = UIButton.current.transform.parent.parent.name.Split(c, 2);
            if (s.Length == 2) {
                int page = -1;
                int index = -1;

                if (int.TryParse(s[0], out page) && int.TryParse(s[1], out index)) {
                    int id = -1;
                    if (int.TryParse(UIButton.current.name, out id) && GameData.DItemData.ContainsKey(id)) {
                        int kind = GameData.DItemData[id].Kind;
                        if (memberList[page][index].Player.ValueItems != null && 
                            memberList[page][index].Player.ValueItems.ContainsKey(kind)) {
                            UIItemHint.UIShow(true);
							UIItemHint.Get.OnShowPartnerItem(id, memberList[page][index].Player);
                        }
                    }
                }
            }
        }
    }

    private void checkSelected(ref TPlayer[] selectAy) {
        for (int i = 0; i < memberList.Length; i++)
            for (int j = 0; j < memberList[i].Count; j++) {
                memberList[i][j].LabelSelected.text = "";
                memberList[i][j].UISelected.SetActive(false);
                for (int k = 0; k < selectAy.Length; k++) 
                    if (memberList[i][j].Player.RoleIndex == selectAy[k].RoleIndex) {
                        memberList[i][j].UISelected.SetActive(true);
                        memberList[i][j].LabelSelected.text = TextConst.S(9511+k);
                    }
            }
    }

	public void InitMemberList(ref List<TPlayer> playerList, ref TPlayer[] selectAy, int index) {
        tempSelectAy = selectAy;
        selectIndex = index;

        for (int i = 0; i < memberList.Length; i++) {
            if (memberList[i] == null)
                memberList[i] = new List<TMember>();
        
		    for (int j = 0; j < memberList[i].Count; j ++) {
                memberList[i][j].LabelSelected.text = "";
                memberList[i][j].UISelected.SetActive(false);
                memberList[i][j].Item.SetActive(false);
    		}
        }

        for (int i = 0; i < playerList.Count; i++) {
            if (playerList[i].FriendKind == EFriendKind.Friend)
			    addMember(1, playerList[i]);
            else
                addMember(0, playerList[i]);
        }
		
        checkSelected(ref selectAy);

        for (int i = 0; i < pageToggle.Length; i++)
            pageToggle[i].value = false;

        if (memberList[1].Count > 0) {
            partnerUI[0].SetActive(false);
            pageToggle[1].value = true;
            nowPage = 1;
        } else {
            partnerUI[1].SetActive(false);
            pageToggle[0].value = true;
            nowPage = 0;
        }
	}

	private void addMember(int page, TPlayer player) {
        int index = memberList[page].Count;
        TMember item = new TMember();
		item.Item = Instantiate(itemMember, Vector3.zero, Quaternion.identity) as GameObject;
        string name = page.ToString() + "-" + index.ToString();
        item.Item.name = name;
		UIButton btn = item.Item.GetComponent<UIButton>();
		SetBtnFun(ref btn, OnSelectPartner);
		item.Item.GetComponent<UIDragScrollView>().scrollView = partnerScrollView[page];
		item.LabelTeamName = GameObject.Find(name + "/PlayerName/NameLabel").GetComponent<UILabel>();
        item.LabelFightCount = GameObject.Find(name + "/FightCount").GetComponent<UILabel>();
		item.LabelPower = GameObject.Find(name + "/CombatGroup/CombatValueLabel").GetComponent<UILabel>();
        item.LabelLv = GameObject.Find(name + "/PlayerInGameBtn/LevelGroup").GetComponent<UILabel>();
        item.SpriteFace = GameObject.Find(name + "/PlayerInGameBtn/PlayerPic").GetComponent<UISprite>();
        item.SpritePosition = GameObject.Find(name + "/PlayerInGameBtn/PlayerPic/PositionIcon").GetComponent<UISprite>();
        item.UISelected = GameObject.Find(name + "/UISelected");
        item.UIEquiptment = GameObject.Find(name + "/EquipmentGroup");
        item.LabelSelected = GameObject.Find(name + "/UISelected/Label").GetComponent<UILabel>();
		GameObject obj = GameObject.Find(name + "/UISelected/Label");
		if (obj) {
			UILabel lab = obj.GetComponent<UILabel>();
			if (lab)
				lab.text = TextConst.S(9510);
		}

        if (player.ValueItems != null) {
            int count = 0;
            foreach (KeyValuePair<int, TValueItem> pair in player.ValueItems) {
                if (GameData.DItemData.ContainsKey(pair.Value.ID)) {
                    GameObject ep = Instantiate(itemEquipt);
                    ep.name = pair.Value.ID.ToString();
                    UIValueItemData vd = UIValueItemDataBuilder.Build(GameData.DItemData[pair.Value.ID], pair.Value.InlayItemIDs, pair.Value.Num);
                    UIEquipItem ei = ep.GetComponent<UIEquipItem>();
                    if (ei != null) {
                        ei.Set(vd, false);
                        ei.OnClickListener += OnItemHint;
                        ep.transform.parent = item.UIEquiptment.transform;
                        ep.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

                        int x = count % 4;
                        int y = count / 4;
                        ep.transform.localPosition = new Vector3(x * 60, y * -54, 0);
                        count ++;
                    }
                }
            }
        }

        item.Index = index;
        item.Player = player;
        item.Item.SetActive(true);
        item.LabelSelected.text = "";
        item.UISelected.SetActive(false);
        item.LabelTeamName.text = player.Name;
        item.LabelPower.text = ((int) player.CombatPower()).ToString();
        item.SpriteFace.spriteName = player.FacePicture;
        item.SpritePosition.spriteName = player.PositionPicture;

        if (player.Lv > 0) {
            item.LabelLv.text = player.Lv.ToString();
            item.LabelFightCount.text = TextConst.S(9516) + player.FightCount.ToString();
        } else {
            item.LabelLv.text = GameData.Team.Player.Lv.ToString();
            item.LabelFightCount.text = TextConst.S(9516) + "∞";
        }

        item.Item.transform.parent = partnerScrollView[page].gameObject.transform;
        item.Item.transform.localScale = Vector3.one;
        item.Item.transform.localPosition = new Vector3(0, index * -180, 0);

        memberList[page].Add(item);
	}

	public void OnSelectPartner() {
		if (UISelectRole.Visible) {
            char[] c = {'-'};
            string[] s = UIButton.current.name.Split(c, 2);
            if (s.Length == 2) {
                nowPage = -1;
                int index = -1;
			
                if (int.TryParse(s[0], out nowPage) && int.TryParse(s[1], out index)) {
    				int i = UISelectRole.Get.GetSelectedListIndex(selectIndex);
    				int j = -1;
    				if (selectIndex == 1)
    				 	j = UISelectRole.Get.GetSelectedListIndex(2);
    				else
    					j = UISelectRole.Get.GetSelectedListIndex(1);

                    if (i != memberList[nowPage][index].Player.RoleIndex && j != memberList[nowPage][index].Player.RoleIndex) {
                        tempSelectAy[selectIndex].RoleIndex = memberList[nowPage][index].Player.RoleIndex;
                        checkSelected(ref UISelectRole.Get.playerData);
                        UISelectRole.Get.SetPlayerAvatar(selectIndex, memberList[nowPage][index].Player.RoleIndex);
    				}
    				
    				initSkillList(index);
                }
			}
		}
	}
}

