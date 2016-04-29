using System;
using System.Collections.Generic;
using GameStruct;
using UnityEngine;
using Newtonsoft.Json;

public struct TRentPlayerResult {
    public DateTime RentPlayerTime;
    public int Diamond;
}

public class TMember{
	public string TeamName;
	public int Index;
	public TPlayer Player;
	public GameObject Item;
    public GameObject UISelected;
    public GameObject UIEquiptment;
    public GameObject UILocked;
    public UILabel LabelSelected;
	public UILabel LabelTeamName;
    public UILabel LabelFightCount;
	public UILabel LabelPower;
    public UILabel LabelLv;
    public UIButton ButtonBG;
	public UISprite SpriteFace;
    public UISprite SpritePosition;
}

public class TRentMercenary {
    public GameObject Item;
    public UILabel LabelMercenary;
    public UILabel LabelRentDiamond;
    public UISprite SpriteRentBG;
}

public class UISelectPartner : UIBase {
	private static UISelectPartner instance = null;
	private const string UIName = "UISelectPartner";
    private const int pageNum = 1;

    private TPlayer[] tempSelectAy;

    private List<TMember>[] memberList = new List<TMember>[pageNum];
	private List<TPassiveSkillCard> skillList = new List<TPassiveSkillCard>();
    private List<GameObject> titleList = new List<GameObject>();
    private UILabel labelSelect;
    private GameObject itemMember;
	private GameObject itemSkill;
    private GameObject itemEquipt;
    private GameObject itemTitle;
    private GameObject itemRent;
    private GameObject[] partnerUI = new GameObject[pageNum];
    private UIScrollView[] partnerScrollView = new UIScrollView[pageNum];
	private UIScrollView skillScrollView;
    private TRentMercenary rentMercenaryDaily = new TRentMercenary();

	private int selectIndex;
    private int nowPage = 0;
    private int extandLine = 0;
    private bool rentExpire = false;

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
                    RemoveUI(instance.gameObject);
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

    void FixedUpdate() {
        if (rentMercenaryDaily.Item && rentMercenaryDaily.Item.activeInHierarchy)
            updateRentDate();
    }

    void OnDestroy() {
        for (int i = 0; i < memberList.Length; i++)
            for (int j = 0; j < memberList[i].Count; j++)
                if (memberList[i][j].Item)
                    Destroy(memberList[i][j].Item);
        
        for (int i = 0; i < skillList.Count; i++)
            if (skillList[i].item)
                Destroy(skillList[i].item);
        
        for (int i = 0; i < titleList.Count; i++)
            Destroy(titleList[i]);
    }
	
	protected override void InitCom() {
        SetBtnFun (UIName + "/Right/Window/Close", OnClose);
        for (int i = 0; i < pageNum; i++) {
            partnerUI[i] = GameObject.Find(UIName + "/Right/Window/Partner/Pages/" + i.ToString());
            partnerScrollView[i] = GameObject.Find(UIName + "/Right/Window/Partner/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIScrollView>();
        }

        itemTitle = Resources.Load("Prefab/UI/Items/ItemScrollViewLine") as GameObject;
        itemRent = Resources.Load("Prefab/UI/Items/ItemRentPartener") as GameObject;
        itemEquipt = Resources.Load("Prefab/UI/Items/UIEquipItem") as GameObject;
		itemMember = Resources.Load("Prefab/UI/Items/ItemSelectPartner") as GameObject;
		itemSkill = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;
        labelSelect = GameObject.Find(UIName + "/Right/Window/SelectPlayer").GetComponent<UILabel>();
        skillScrollView = GameObject.Find(UIName + "/Right/Window/Skill/ScrollView").GetComponent<UIScrollView>();
	}

	public void OnClose(){
		UISelectRole.Get.InitPartnerPosition();
        instance.Show(false);
	}

    private void addTitle(string text, int page=0) {
        GameObject obj = Instantiate(itemTitle) as GameObject;
        obj.name = "Title" + extandLine.ToString();
        obj.GetComponent<UILabel>().text = text;
        obj.GetComponent<UIDragScrollView>().scrollView = partnerScrollView[page];
        obj.transform.parent = partnerScrollView[page].gameObject.transform;
        obj.transform.localScale = Vector3.one;
        float y = memberList[page].Count * -185 - extandLine * 80 + 55;
        obj.transform.localPosition = new Vector3(0, y, 0);
        extandLine ++;
    }

    private void updateRentDate() {
        if (GameData.Team.RentExpire) {
            //rentMercenaryDaily.SpriteRentBG.color = Color.red;
            rentMercenaryDaily.LabelMercenary.text = string.Format(TextConst.S(9520), 1);
            if (!rentExpire)
                initRentPlayer();

            rentExpire = true;
        } else {
            rentExpire = false;
            //rentMercenaryDaily.SpriteRentBG.color = new Color32(0, 230, 255, 255);
            rentMercenaryDaily.LabelMercenary.text = string.Format(TextConst.S(9521), TextConst.SecondString(GameData.Team.RentPlayerTime.ToUniversalTime()));
        }
    }

    private void updateRentPrice() {
        rentMercenaryDaily.LabelRentDiamond.text = GameConst.Diamond_RentPlayer.ToString();
        rentMercenaryDaily.LabelRentDiamond.color = GameData.CoinEnoughTextColor(GameData.Team.Diamond >= GameConst.Diamond_RentPlayer);
    }

    private void addRent(int page) {
        if (!rentMercenaryDaily.Item) {
            rentMercenaryDaily.Item = Instantiate(itemRent) as GameObject;
            rentMercenaryDaily.Item.name = "RentMercenaryDaily";
            SetBtnFun(rentMercenaryDaily.Item.name + "/RentBtn", OnRent);
            rentMercenaryDaily.Item.GetComponent<UIDragScrollView>().scrollView = partnerScrollView[page];
            rentMercenaryDaily.SpriteRentBG = GameObject.Find(rentMercenaryDaily.Item.name + "/GroupBG").GetComponent<UISprite>();
            rentMercenaryDaily.LabelMercenary = GameObject.Find(rentMercenaryDaily.Item.name + "/Label").GetComponent<UILabel>();
            rentMercenaryDaily.LabelRentDiamond = GameObject.Find(rentMercenaryDaily.Item.name + "/RentBtn/PriceLabel").GetComponent<UILabel>();
            updateRentDate();
            updateRentPrice();

            rentMercenaryDaily.Item.transform.parent = partnerScrollView[page].gameObject.transform;
            rentMercenaryDaily.Item.transform.localScale = Vector3.one;
            float y = memberList[page].Count * -185 - extandLine * 80 + 50;
            rentMercenaryDaily.Item.transform.localPosition = new Vector3(0, y, 0);
            extandLine ++;
        }
    }

    private int getMemberIndex(int index) {
        for (int i = 0; i < memberList[nowPage].Count; i++)
            if (tempSelectAy[selectIndex].RoleIndex == memberList[nowPage][i].Player.RoleIndex)
                return i;

        return 0;
    }

	private void initSkillList(int index) {
        if (index < memberList[nowPage].Count) {
            labelSelect.text = memberList[nowPage][index].Player.Name;
    		for (int i = 0; i < memberList[nowPage][index].Player.SkillCards.Length; i++)
                addSkillItem(i, memberList[nowPage][index].Player.SkillCards[i]);

            for (int i = memberList[nowPage][index].Player.SkillCards.Length; i < skillList.Count; i++)
                skillList[i].Enable = false;
        }
	}

    private void initSkillList() {
        int index = getMemberIndex(selectIndex);
        initSkillList(index);
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
		skillList[index].UpdateView(index, skill, new Vector3(0, index * -70, 0));
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

    private void waitRent(bool ok, WWW www)
    {
        if(ok)
        {
            TRentPlayerResult result = JsonConvertWrapper.DeserializeObject<TRentPlayerResult>(www.text);

            if(GameData.IsMainStage)
                Statistic.Ins.LogEvent(56, GameData.Team.Diamond - result.Diamond);
            else if(GameData.IsInstance)
                Statistic.Ins.LogEvent(104, GameData.Team.Diamond - result.Diamond);

            GameData.Team.Diamond = result.Diamond;
            GameData.Team.RentPlayerTime = result.RentPlayerTime;
            updateRentDate();
            updateRentPrice();
        }
    }

    private void askRent() {
        SendHttp.Get.Command(URLConst.RentPlayer, waitRent);
    }

    public void OnRent() {
        CheckDiamond(GameConst.Diamond_RentPlayer, true, 
            string.Format(TextConst.S(9523), GameConst.Diamond_RentPlayer),
            askRent, updateRentPrice);
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

    public void InitMemberList(ref List<TPlayer> playerList, ref TPlayer[] selectAy, int index, bool isPVP = false) {
        tempSelectAy = selectAy;
        selectIndex = index;
        rentExpire = GameData.Team.RentExpire;

        if (memberList[0] == null) {
            for (int i = 0; i < memberList.Length; i++) {
                if (memberList[i] == null)
                    memberList[i] = new List<TMember>();
            }

            bool line1 = false;
            bool line2 = false;
            for (int i = 0; i < playerList.Count; i++) {
                if (playerList[i].FriendKind == EFriendKind.Friend || playerList[i].FriendKind == EFriendKind.Mercenary)
                    line1 = true;
                else
                    line2 = true;
            }

            if (line1) {
                addTitle(isPVP ? TextConst.S(9503): TextConst.S(9519));
                if (!isPVP)
                    addRent(0);
            }

            for (int i = 0; i < playerList.Count; i++) {
                if (playerList[i].FriendKind == EFriendKind.Friend || playerList[i].FriendKind == EFriendKind.Mercenary)
                    addMember(0, playerList[i]);
            }

            if (line2)
                addTitle(isPVP ? TextConst.S(9502): TextConst.S(9518));
    		
            for (int i = 0; i < playerList.Count; i++) {
                if (playerList[i].FriendKind != EFriendKind.Friend && playerList[i].FriendKind != EFriendKind.Mercenary)
                    addMember(0, playerList[i]);
            }

            checkSelected(ref selectAy);
            Invoke("initSkillList", 1f);
        } else {
            int i = getMemberIndex(selectIndex);
            initSkillList(i);
        }
	}

    private void initRentPlayer() {
        for (int i = 0; i < memberList[0].Count; i++) 
            if (memberList[0][i].Player.FriendKind == EFriendKind.Mercenary)
                memberList[0][i].UILocked.SetActive(GameData.Team.RentExpire);
            else
                memberList[0][i].UILocked.SetActive(false);
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
        item.ButtonBG = btn;
        item.SpriteFace = GameObject.Find(name + "/PlayerInGameBtn/PlayerPic").GetComponent<UISprite>();
        item.SpritePosition = GameObject.Find(name + "/PlayerInGameBtn/PlayerPic/PositionIcon").GetComponent<UISprite>();
        item.UISelected = GameObject.Find(name + "/UISelected");
        item.UIEquiptment = GameObject.Find(name + "/EquipmentGroup");
        item.UILocked = GameObject.Find(name + "/UILocked");
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
                        ei.Set(vd, false, false);
                        ei.OnClickListener += OnItemHint;
                        ep.transform.parent = item.UIEquiptment.transform;
                        ep.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

                        int x = count % 4;
                        int y = count / 4;
                        ep.transform.localPosition = new Vector3(x * 60, 2+ y * -60, 0);
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

        if (player.FriendKind == EFriendKind.Mercenary && GameData.Team.RentExpire)
            item.UILocked.SetActive(true);
        else
            item.UILocked.SetActive(false);
        
        if (player.FriendKind == EFriendKind.Friend || player.FriendKind == EFriendKind.Mercenary) {
            item.ButtonBG.defaultColor = new Color32(240, 203, 127, 255);
            item.ButtonBG.hover = new Color32(240, 203, 127, 255);
            item.ButtonBG.pressed = new Color32(240, 203, 127, 255);
        } else {
            item.ButtonBG.defaultColor = Color.white;
            item.ButtonBG.hover = Color.white;
            item.ButtonBG.pressed = Color.white;
        }

        if (player.Lv > 0) {
            item.LabelLv.text = player.Lv.ToString();
            item.LabelFightCount.text = TextConst.S(9516) + player.FightCount.ToString();
        } else {
            item.LabelLv.text = GameData.Team.Player.Lv.ToString();
            item.LabelFightCount.text = TextConst.S(9516) + "∞";
        }

        item.Item.transform.parent = partnerScrollView[page].gameObject.transform;
        item.Item.transform.localScale = Vector3.one;
        item.Item.transform.localPosition = new Vector3(0, index * -185 - extandLine*80, 0);

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
                        if (memberList[nowPage][index].Player.FriendKind == EFriendKind.Mercenary && GameData.Team.RentExpire)
                            UIHint.Get.ShowHint(TextConst.S(9522), Color.red);
                        else {
                            tempSelectAy[selectIndex].RoleIndex = memberList[nowPage][index].Player.RoleIndex;
                            checkSelected(ref UISelectRole.Get.playerData);
                            UISelectRole.Get.SelectPartner(selectIndex, memberList[nowPage][index].Player.RoleIndex);
                        }
    				}
    				
    				initSkillList(index);
                }
			}
		}
	}
}

