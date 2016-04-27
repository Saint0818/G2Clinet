using GameStruct;
using UnityEngine;

public class PersonalView
{
    private GameObject self;
    private UIButton changeHeadBtn;
    private UISprite headTex;
	private UISprite position;
    private UILabel lv;
    private UILabel name;
    private UISlider expBar;
    private UILabel expValue;
    //	private UISprite powerBar;
    private UILabel powerValue;
    //	private UIButton group;
    private UIButton playerName;
    //	private UILabel groupHead;
    //	private UILabel groupBody;
    public TValueAvater[] ValueItems = new TValueAvater[8];

    public void Init(GameObject obj, GameObject[] itemEquipmentBtn)
    {
        self = obj;

        if (self)
        {
            playerName = self.transform.Find("PlayerName").gameObject.GetComponent<UIButton>();
            changeHeadBtn = self.transform.Find("PlayerBt").gameObject.GetComponent<UIButton>();
            headTex = changeHeadBtn.transform.Find("PlayerIcon").gameObject.GetComponent<UISprite>();
			position = changeHeadBtn.transform.Find("PositionIcon").gameObject.GetComponent<UISprite>();
            lv = changeHeadBtn.transform.Find("LevelLabel").gameObject.GetComponent<UILabel>();
            name = self.transform.Find("PlayerName/NameLabel").gameObject.GetComponent<UILabel>();
            expBar = self.transform.Find("EXPView/ProgressBar").gameObject.GetComponent<UISlider>();
            expValue = self.transform.Find("EXPView/ExpLabel").gameObject.GetComponent<UILabel>();
//			powerBar = self.transform.Find("CombatView/CombatValue").gameObject.GetComponent<UISprite>();
            powerValue = self.transform.Find("CombatView/CombatLabel").gameObject.GetComponent<UILabel>();
//			group = self.transform.Find("PlayerLeague").gameObject.GetComponent<UIButton>();
//			groupHead = group.transform.Find("Label").gameObject.GetComponent<UILabel>();
//			groupBody = group.transform.Find("LeagueID").gameObject.GetComponent<UILabel>();
			UIEventListener.Get(self.transform.Find("CombatView").gameObject).onClick = OnClickPower;

            for (int i = 0; i < ValueItems.Length; i++)
            {
                GameObject go = self.transform.Find(string.Format("EquipmentView/PartSlot{0}/View", i)).gameObject;
                if ((ValueItems[i] == null || !ValueItems[i].IsInit) && itemEquipmentBtn[i] != null)
                {
                    ValueItems[i] = new TValueAvater();
                    ValueItems[i].Init(itemEquipmentBtn[i], go, i);
                }
            }
        }
    }

	public void OnClickPower(GameObject go) {
		UIOverallHint.Get.ShowView(powerValue.text);
	}

    public void InitBtttonFunction(EventDelegate changeHeadFunc, EventDelegate groupFunc, EventDelegate itemHint, EventDelegate changeName)
    {
        changeHeadBtn.onClick.Add(changeHeadFunc);
//		group.onClick.Add (groupFunc);
        playerName.onClick.Add(changeName);

        for (int i = 0; i < ValueItems.Length; i++)
            ValueItems[i].InitBtttonFunction(itemHint);
    }

    public void Update(ref TTeam team)
    {
        UpdatePlayerData(team.Player);
        UpdateValueItem(team.Player);
    }

    private void UpdateValueItem(TPlayer player)
    {
        int id = 0;
        int kind = 11;

        for (int i = 0; i < ValueItems.Length; i++)
            ValueItems[i].Enable = false;

        if (player.ValueItems != null)
        {
            for (int i = 0; i < ValueItems.Length; i++)
            {
                if (player.ValueItems.ContainsKey(kind + i))
                {
                    id = player.ValueItems[kind + i].ID;
                    if (id > 0)
                    {
                        TItemData data = GameData.DItemData[player.ValueItems[kind + i].ID];
                        ValueItems[i].Enable = true;
                        ValueItems[i].Name = data.Name;
                        ValueItems[i].Atlas = data.Atlas;
                        ValueItems[i].Pic = data.Icon;
                        ValueItems[i].Quality = data.Quality;
                        if (player.ValueItems != null && player.ValueItems.ContainsKey(kind + i) &&
                            player.ValueItems[kind + i].InlayItemIDs != null)
                        {
                            ValueItems[i].Starts = player.ValueItems[kind + i].InlayItemIDs.Length;
                            ValueItems[i].ID = player.ValueItems[kind + i].ID;
                        }
                    }
                }

            }
        }
    }

    private void UpdatePlayerData(TPlayer player)
    {
        headTex.spriteName = player.FacePicture;
		position.spriteName = player.PositionPicture;
        name.text = player.Name;
        lv.text = player.Lv.ToString();

        if (GameData.DExpData.ContainsKey(player.Lv))
        {
            expValue.text = string.Format("{0}/{1}", player.Exp, GameData.DExpData[player.Lv].LvUpExp);
            expBar.value = player.Exp / (float)GameData.DExpData[player.Lv].LvUpExp;
        }
        else
        {
            expValue.text = "Max";
            expBar.value = 1;
        }

        powerValue.text = player.CombatPower().ToString();
//		powerBar.fillAmount = player.Power() / 100;
    }

    public bool Enable{ set { self.SetActive(value); } }
}

public class AbilityView
{
    private GameObject self;
    private TAbilityItem[] abilitys = new TAbilityItem[12];
    private UIAttributes hexagon;

    public void Init(GameObject obj, GameObject hexgonObj, EventDelegate hitFunc)
    {
        self = obj;
        if (self)
        {
            GameObject go;
            for (int i = 0; i < abilitys.Length; i++)
            {
                abilitys[i] = new TAbilityItem();
                go = self.transform.Find(string.Format("AttrGroup/{0}", i)).gameObject;
                abilitys[i].Init(go, i, hitFunc);
            }

            GameObject hexagonCenter = GameObject.Find("AttributeHexagonCenter").gameObject;
            hexagon = hexgonObj.GetComponent<UIAttributes>();
            hexagon.transform.parent = hexagonCenter.transform;
            hexagon.transform.localPosition = Vector3.zero;
			hexagon.transform.localScale = Vector3.one;
            GameFunction.InitDefaultText(hexagon.gameObject);
        }
    }

    public void UpdateView(ref TTeam team)
    {
        float basic = 0;
		int add = 0;

        EAttribute kind;
        for (int i = 0; i < abilitys.Length; i++)
        {
            kind = GameFunction.GetAttributeKind(i);
            basic = 0;

//			if(team.Player.Potential.ContainsKey(kind))
//				add = team.Player.Potential[kind];

            switch (kind)
            {
                case EAttribute.Point2:
                    basic = team.Player.Point2;
                    break;
                case EAttribute.Point3:
                    basic = team.Player.Point3;
                    break;
                case EAttribute.Dunk:
                    basic = team.Player.Dunk;
                    break;
                case EAttribute.Rebound:
                    basic = team.Player.Rebound;
                    break;
                case EAttribute.Block:
                    basic = team.Player.Block;
                    break;
                case EAttribute.Steal:
                    basic = team.Player.Steal;
                    break;
                case EAttribute.Speed:
                    basic = team.Player.Speed;
                    break;
                case EAttribute.Stamina:
                    basic = team.Player.Stamina;
                    break;
                case EAttribute.Strength:
                    basic = team.Player.Strength;
                    break;
                case EAttribute.Defence:
                    basic = team.Player.Defence;
                    break;
                case EAttribute.Dribble:
                    basic = team.Player.Dribble;
                    break;
                case EAttribute.Pass:
                    basic = team.Player.Pass;
                    break; 
            }
            abilitys[i].Value.text = basic.ToString();
        }
        GameFunction.UpdateAttrHexagon(hexagon, team.Player);
    }

    public void HexagonEnable(bool enable)
    {
        hexagon.SetVisible(enable);
    }

    public bool Enable{ set { self.SetActive(value); } }
}

public class TSkillView
{
    private GameObject self;
    private TActiveSkillCard[] activeSkillCard;
    private TPassiveSkillCard[] passiveSkillCard;

    public void Init(GameObject go, TSkill[] skill, GameObject[] active, GameObject[] passive)
    {
        if (go)
        {
            self = go;	

            activeSkillCard = new TActiveSkillCard[active.Length];
            passiveSkillCard = new TPassiveSkillCard[passive.Length];

            for (int i = 0; i < active.Length; i++)
            {
                activeSkillCard[i] = new TActiveSkillCard();
                activeSkillCard[i].Init(active[i], new EventDelegate(OnActiveSkillHint));
            }

            GameObject passiveParent = self.transform.Find("PassiveCard/PassiveList").gameObject;

            for (int i = 0; i < passive.Length; i++)
            {
                passiveSkillCard[i] = new TPassiveSkillCard();
                passiveSkillCard[i].Init(passiveParent, passive[i], new EventDelegate(OnActiveSkillHint)); 
            }
        }
    }

    public bool Enable
    {
        set{ self.SetActive(value); }
    }

    public void OnActiveSkillHint()
    {
        int index;
        if (int.TryParse(UIButton.current.name, out index))
        {
            UIItemHint.Get.OnShowSkill(GameData.Team.Player.SkillCards[index]);
        }
    }

    private float[] sortY = new float[3]{ 70, 0, -70 };

    public void UpdateView(TSkill[] skill)
    {
        int activecount = 0;
        int passivecount = 0;

        for (int i = 0; i < activeSkillCard.Length; i++)
        {
            activeSkillCard[i].Enable = false;		
        }

        if (passiveSkillCard != null)
            for (int i = 0; i < passiveSkillCard.Length; i++)
            {
                passiveSkillCard[i].Enable = false;
            }

        for (int i = 0; i < skill.Length; i++)
        {
            if (GameFunction.IsActiveSkill(skill[i].ID))
            {
                if (activecount < GameConst.Max_ActiveSkill)
                {
                    activeSkillCard[activecount].Enable = true;
                    activeSkillCard[activecount].UpdateView(i, skill[i]);
                    activecount++;
                }
            }
            else
            {
                if (passivecount < passiveSkillCard.Length)
                {
                    passiveSkillCard[passivecount].Enable = true;
                    passiveSkillCard[passivecount].UpdateView(i, skill[i], new Vector3(-350 + (int)(passivecount / 3) * 300, sortY[passivecount % 3], 0));
                    passivecount++;
                }
            }
        }
    }
}

public class TStateView
{
    private GameObject self;
    private UILabel ContentsLabel;

    public void Init(GameObject go)
    {
        if (go)
        {
            self = go;	
            ContentsLabel = self.transform.Find("ShowPve/Contents/ContentsLabel").gameObject.GetComponent<UILabel>();
        }
    }

    public bool Enable
    {
        set{ self.SetActive(value); }
    }

    public void UpdateView()
    {
        ContentsLabel.text = GameData.Team.StatsText;
    }
}

[System.Serializable]
public class TValueAvater
{
    public int Index;
    public int ID;
    private GameObject self;
    public UISprite BG;
	public UISprite qualityBG;
    private UISprite pic;
    private UISprite redPoint;
    private UILabel name;
    public UIButton Btn;
    private UISprite[] stars = new UISprite[4];
    public GameObject Parent;
    public bool IsInit = false;
	public GameObject plusIcon;
	public UILabel amountLabel;

    public void Init(GameObject obj, GameObject parent, int index)
    {
        if (obj)
        {
            Index = index;
            self = obj;
            self.transform.parent = parent.transform;
            self.transform.localPosition = Vector3.zero;
            self.transform.localScale = Vector3.one;
            BG = self.GetComponent<UISprite>();

            pic = self.transform.Find("Icon").gameObject.GetComponent<UISprite>();
			qualityBG = self.transform.Find("QualityBG").gameObject.GetComponent<UISprite>();
            redPoint = self.transform.Find("RedPoint").gameObject.GetComponent<UISprite>();
            redPoint.enabled = false;
            name = self.transform.Find("NameLabel").gameObject.GetComponent<UILabel>();
			plusIcon =  self.transform.Find("PlustIcon").gameObject;
			amountLabel = self.transform.Find("AmountLabel").gameObject.GetComponent<UILabel>();
			plusIcon.SetActive(false);
			amountLabel.gameObject.SetActive(false);
            Btn = self.GetComponent<UIButton>();

            self.name = Index.ToString();

            for (int i = 0; i < stars.Length; i++)
                stars[i] = obj.transform.Find(string.Format("EquipmentStar/Empty{0}", i)).gameObject.GetComponent<UISprite>();

            IsInit = self || BG || Btn || stars[0] || stars[1] || stars[2] || stars[3] || pic;
        }
    }

    public void InitBtttonFunction(EventDelegate btnFunc)
    {
        if (Btn && btnFunc != null)
            Btn.onClick.Add(btnFunc);
    }

    public int Atlas
    {
        set{ pic.atlas = TextureManager.Get.ItemAtlas(value); }
    }

    public string Pic
    {

        set{ pic.spriteName = string.Format("Item_{0}", value); }
    }

    public string Name
    {
        set{ name.text = value; }
    }

    public int Quality
    {

        set{
			qualityBG.color = TextConst.ColorBG(value);
			BG.spriteName = string.Format("Equipment_{0}", value); }
    }

    public int Starts
    {
        set
        {
            for (int i = 0; i < stars.Length; i++)
                stars[i].enabled = i < value ? true : false;
        }
    }

    public bool Enable
    {
        set{ self.SetActive(value); }
    }
}

public class TAbilityItem
{
    private GameObject go;
    private UIButton btn;
    public int index;
    public UILabel Value;
    public bool IsInit = false;

    public void Init(GameObject obj, int index, EventDelegate hitFunc)
    {
        if (obj)
        {
            go = obj;
//			pic = go.GetComponent<UISprite> ();
//			pic.spriteName = string.Format ("AttrKind_{0}", index + 1);
            Value = go.transform.Find("ValueBaseLabel").gameObject.GetComponent<UILabel>();
            btn = go.GetComponent<UIButton>();
            IsInit = go && Value;

            if (IsInit)
            {
                btn.onClick.Add(hitFunc);
            }
        }
    }
}

public class UIPlayerInfo : UIBase
{

    private static UIPlayerInfo instance = null;
    private const string UIName = "UIPlayerInfo";
    private GameObject[] PageAy = new GameObject[3];
    private UIAttributes hexagon;

    //Page 0
    private PersonalView personalView = new PersonalView();
    private AbilityView abilityView = new AbilityView();

    //Page 1
    private TSkillView skillView = new TSkillView();

    //Page 2
    private TStateView stateView = new TStateView();

    //part4
    public UIButton SkillUp;
    public UIButton Back;
    public static TTeam teamdata;

    //Page 2
    private void Awake()
    {

    }

    public void UpdateAvatarModel(TItem[] items)
    {
		
    }

    public static bool Visible
    {
        get
        {
            if (instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }
    }

    public static UIPlayerInfo Get
    {
        get
        {
            if (!instance)
                instance = LoadUI(UIName) as UIPlayerInfo;
			
            return instance;
        }
    }

    public static void UIShow(bool isShow, ref TTeam team)
    {
		if (isShow) {
			team.PlayerInit();
            teamdata = team;

            Statistic.Ins.LogScreen(22);
        }

        if (instance)
        {
            if (!isShow)
            {
                RemoveUI(instance.gameObject);
                UIPlayerAvatar.Get.Enable = false;
            }
            else
                instance.Show(isShow);
        }
        else if (isShow)
            Get.Show(isShow);
    }

    protected override void InitCom()
    {
        GameObject obj;

        //P1
        for (int i = 0; i < PageAy.Length; i++)
            PageAy[i] = GameObject.Find(string.Format("Page{0}", i));

        GameObject[] itemEquipmentBtns = new GameObject[8];
        for (int i = 0; i < itemEquipmentBtns.Length; i++)
        {
            itemEquipmentBtns[i] = UIPrefabPath.LoadUI(UIPrefabPath.UIEquipItem);
        }

        obj = GameObject.Find(UIName + string.Format("/Window/Center/View/PersonalView"));
        personalView.Init(obj, itemEquipmentBtns);
        personalView.InitBtttonFunction(new EventDelegate(OnChangehead), new EventDelegate(OnGuild), new EventDelegate(OnAvatarItemHint), new EventDelegate(OnChangePlayerName));

        obj = GameObject.Find(UIName + string.Format("/Window/Center/View/AbilityView"));
        abilityView.Init(obj, Instantiate(Resources.Load("Prefab/UI/UIattributeHexagon")) as GameObject, new EventDelegate(OnAttributeHint));
        SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnReturn);
        for (int i = 0; i < 3; i++)
            SetBtnFun(UIName + string.Format("/Window/Center/View/TopTabs/{0}", i), OnPage);	

        //Skill page
        obj = GameObject.Find(UIName + string.Format("/Window/Center/View/SkillView"));

        GameObject[] actives = new GameObject[GameConst.Max_ActiveSkill];
        GameObject[] passives =	new GameObject[GameFunction.GetPassiveSkillCount(GameData.Team.Player.SkillCards)];
        GameObject clone = Resources.Load("Prefab/UI/Items/ItemCardEquipped") as GameObject;

        for (int i = 0; i < GameConst.Max_ActiveSkill; i++)
        {
            actives[i] = obj.transform.Find(string.Format("ActiveCard/ActiveCardBase{0}/ItemSkillCard", i)).gameObject;	
        }

        for (int i = 0; i < passives.Length; i++)
        {
            passives[i] = Instantiate(clone);		
        }

        skillView.Init(obj, GameData.Team.SkillCards, actives, passives);

        //StatePage
        obj = GameObject.Find(UIName + string.Format("/Window/Center/View/StateView"));
        stateView.Init(obj);
    }

    private void OnAttributeHint()
    {
        int index;
        EAttribute att = EAttribute.Point2;
            
        if (int.TryParse(UIButton.current.name, out index))
        {
            switch (index)
            {
                case 0:
                    att = EAttribute.Point2;
                    break;
                case 1:
                    att = EAttribute.Point3;
                    break;
                case 2:
                    att = EAttribute.Dunk;
                    break;
                case 3:
                    att = EAttribute.Rebound;
                    break;
                case 4:
                    att = EAttribute.Block;
                    break;
                case 5:
                    att = EAttribute.Steal;
                    break;
                case 6:
                    att = EAttribute.Speed;
                    break;
                case 7:
                    att = EAttribute.Stamina;
                    break;
                case 8:
                    att = EAttribute.Strength;
                    break;
                case 9:
                    att = EAttribute.Defence;
                    break;
                case 10:
                    att = EAttribute.Dribble;
                    break;
                case 11:
                    att = EAttribute.Pass;
                    break;
            }
            int kind = GameFunction.GetEBounsIndexByAttribute(att);
            UIAttributeHint.Get.UpdateView(kind);
        }
    }

    public void OnReturn()
    {
        UIShow(false, ref teamdata);
        UIMainLobby.Get.Show();
    }

//    public void OnUpgradingMasteries()
//    {
//        UIPlayerPotential.UIShow(true);
//    }

    public void UpdateHexagon(bool flag)
    {
        abilityView.HexagonEnable(flag);
    }

    public void OnSwitchPage()
    {

    }

    protected override void InitData()
    {
        UpdatePage(0);
    }

    public void OnPage()
    {
        int index;
        if (int.TryParse(UIButton.current.name, out index))
        {
            UpdatePage(index);
        }
    }

    public void UpdatePage(int index)
    {
        switch (index)
        {
            case 0:
                personalView.Update(ref teamdata);
                abilityView.UpdateView(ref teamdata);
                UIPlayerAvatar.Get.ShowUIPlayer(EUIPlayerMode.UIPlayerInfo, teamdata.Player.BodyType, teamdata.Player.Avatar);
                break;
            case 1:
                skillView.UpdateView(teamdata.Player.SkillCards);
                break;
            case 2:
                stateView.UpdateView();
                break;
        }

        personalView.Enable = index == 0 ? true : false;
        abilityView.Enable = index == 0 ? true : false;
        skillView.Enable = index == 1 ? true : false;
        stateView.Enable = index == 2 ? true : false;
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);
        if (isShow)
            UpdateHexagon(true);
    }

    public void OnChangehead()
    {
        UIHeadPortrait.UIShow(true);
    }

    public void OnGuild()
    {

    }

    public void OnAvatarItemHint()
    {
        int index;
        if (int.TryParse(UIButton.current.name, out index))
        if (index < GameData.Team.Player.Items.Length && index < personalView.ValueItems.Length)
        if (personalView.ValueItems[index].ID > 0 && GameData.DItemData.ContainsKey(personalView.ValueItems[index].ID))
            UIItemHint.Get.OnShow(personalView.ValueItems[index].ID);
    }

    public void OnChangePlayerName()
    {
        UINamed.UIShow(true);
    }
}
