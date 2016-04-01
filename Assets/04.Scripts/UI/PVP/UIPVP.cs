using UnityEngine;
using GameStruct;
using GameItem;
using Newtonsoft.Json;
using DG.Tweening;
using System;

public class PVPPage1TopView
{
    private GameObject self;
    private UISprite PvPRankIcon;
    private UILabel RangeNameLabel;
    private UILabel NowRangeLabel;
    private UIButton MyRankBtn;
    private UILabel NowAward0;
    private UILabel NowAward1;

    private UILabel NextRangeNameLabel;
    private UILabel NextNowRangeLabel;
    private UILabel NextAward0;
    private UILabel NextAward1;
    private UISprite NextRankIcon;
    private GameObject NextRankGroup;
    private bool isInit = false;

    public void Init(GameObject go, EventDelegate myRankFunc)
    {
        if (go)
        {
            self = go;
            PvPRankIcon = self.transform.Find("NowRankGroup/PvPRankIcon").gameObject.GetComponent<UISprite>();
            RangeNameLabel = self.transform.Find("NowRankGroup/RangeNameLabel").gameObject.GetComponent<UILabel>();
            NowRangeLabel = self.transform.Find("NowRankGroup/NowRangeLabel").gameObject.GetComponent<UILabel>();

            NextRankGroup = self.transform.Find("NextRankGroup").gameObject;
            NextRangeNameLabel = NextRankGroup.transform.Find("RangeNameLabel").GetComponent<UILabel>();
            NextAward0 = NextRankGroup.transform.Find("Award0/ValueLabel").GetComponent<UILabel>();
            NextAward1 = NextRankGroup.transform.Find("Award1/ValueLabel").GetComponent<UILabel>();
            NextRankIcon = NextRankGroup.transform.Find("PvPRankIcon").gameObject.GetComponent<UISprite>();

            MyRankBtn = self.transform.Find("MyRankBtn").gameObject.GetComponent<UIButton>();
            NowAward0 = self.transform.Find("AwardGroup/Award0/ValueLabel").gameObject.GetComponent<UILabel>();
            NowAward1 = self.transform.Find("AwardGroup/Award1/ValueLabel").gameObject.GetComponent<UILabel>();
            isInit = self && PvPRankIcon && RangeNameLabel && NowRangeLabel && MyRankBtn && NowAward0 && NowAward1;
            MyRankBtn.onClick.Add(myRankFunc);
        }
    }

    public bool Enable
    {
        set{ self.SetActive(value); }
        get{ return self.activeSelf; } 
    }

    public void UpdateView(TTeam team)
    {
        if (isInit)
        {
            int lv = team.PVPLv;
            if (GameData.DPVPData.ContainsKey(lv))
            {
                PvPRankIcon.spriteName = string.Format("IconRank{0}", lv);
                RangeNameLabel.text = GameData.DPVPData[lv].Name;
                NowRangeLabel.text = string.Format("{0}-{1}", GameData.DPVPData[lv].LowScore, GameData.DPVPData[lv].HighScore);

                NowAward0.text = GameData.DPVPData[lv].PVPCoin.ToString();
                NowAward1.text = GameData.DPVPData[lv].PVPCoinDaily.ToString();
            }

            if (GameData.DPVPData.ContainsKey(lv + 1))
            {
                NextRankGroup.SetActive(true);
                NextRankIcon.spriteName = string.Format("IconRank{0}", lv + 1);
                NextRangeNameLabel.text = GameData.DPVPData[lv + 1].Name;
                NextAward0.text = GameData.DPVPData[lv + 1].PVPCoin.ToString();
                NextAward1.text = GameData.DPVPData[lv + 1].PVPCoinDaily.ToString();
            }
            else
            {
                NextRankGroup.SetActive(false);
            }
           
        }
        else
        {
            Debug.LogError("You need Init");
        }
    }
}

public class PVPPage1ListView
{
    private GameObject self;
    private TItemRankGroup myRankInfo;
    private TItemRankGroup[] ranks;
    private bool isInit = false;

    public void Init(GameObject go, ref GameObject[] rank100, GameObject parent)
    {
        if (go)
        {
            self = go;
            myRankInfo = new TItemRankGroup();
            GameObject myrank = self.transform.Find("ItemRankGroup").gameObject;

            myRankInfo.Init(ref myrank, new EventDelegate(CloseMyRank));
            myRankInfo.Enable = false;

            ranks = new TItemRankGroup[rank100.Length];

            isInit = self && myrank;
           
            if (isInit)
                for (int i = 0; i < rank100.Length; i++)
                {
                    ranks[i] = new TItemRankGroup();
                    ranks[i].Init(ref rank100[i]);
                    ranks[i].SetParent(parent);
                    ranks[i].Enable = false;
                }
        }
    }

    private void CloseMyRank()
    {
        MyRankEnable = false; 
    }

    public bool MyRankEnable
    {
        set{ myRankInfo.Enable = value; }
        get { return myRankInfo.Enable; }
    }

    public bool Enable
    {
        set{ self.SetActive(value); }
        get{ return self.activeSelf; } 
    }

    public void UpdateView(TTeamRank team, TTeamRank[] data)
    {
        myRankInfo.UpdateView(team);

        for (int i = 0; i < ranks.Length; i++)
        {
            if (i < ranks.Length)
            { //防止Data大於實體物件
                if (i < data.Length)
                {
                    ranks[i].Enable = true;
                    ranks[i].UpdateView(data[i]);
                    ranks[i].LocalPosititon = new Vector3(0, -130 * i, 0);
                }
                else
                    ranks[i].Enable = false;
            }
        }
    }

    public void UpdateViewMyrank(TTeamRank data)
    {
        MyRankEnable = true;
        myRankInfo.UpdateView(data);
    }
}

public enum EPVPSituation
{
    CDIng = 0,
    CDEnd = 1,
    NoCountOrBuy = 2
}

public class PVPMainView
{
    private GameObject self;
    private TPvPLeagueGroup[] pvplvs;
    private UIButton nextBtn;
    private UIButton Lbtn;
    private UIButton Rbtn;
    private UILabel award0;
    private UILabel award1;
    private GameObject Sort;
    private UIButton getRewardBtn;
    private UISprite readPoint;
    private PVPLvRangeItem lvRange;
	
    private bool isInit = false;

    public void Init(GameObject go, ref GameObject[] lvs, EventDelegate getEnemyFunc, EventDelegate lFunc, 
                     EventDelegate rFunc, EventDelegate nowRankFunc, EventDelegate getRewardFunc)
    {
        if (go)
        {
            self = go;
            nextBtn = self.transform.Find("NextBtn").gameObject.GetComponent<UIButton>();
            Lbtn = self.transform.Find("ButtonGroup/LButton").gameObject.GetComponent<UIButton>();
            Rbtn = self.transform.Find("ButtonGroup/RButton").gameObject.GetComponent<UIButton>();
            Sort = self.transform.Find("PvPLeagueBoard/ScrollView/Sort").gameObject;
            award0 = self.transform.Find("AwardGroup/Award0/ValueLabel").gameObject.GetComponent<UILabel>();
            award1 = self.transform.Find("AwardGroup/Award1/ValueLabel").gameObject.GetComponent<UILabel>();
            getRewardBtn = self.transform.Find("DailyAwardBtn").gameObject.GetComponent<UIButton>();
            lvRange = self.transform.Find("PvPLeagueSlider/PVPLvRangeItem").gameObject.GetComponent<PVPLvRangeItem>();

            pvplvs = new TPvPLeagueGroup[lvs.Length];
            for (int i = 0; i < pvplvs.Length; i++)
            {
                lvs[i].transform.parent = Sort.transform;
                pvplvs[i] = new TPvPLeagueGroup();
                pvplvs[i].Init(ref lvs[i], Sort);
                pvplvs[i].UpdateView(i + 1);
                pvplvs[i].LoaclPosition = new Vector3(260 * i, 0, 0);
                pvplvs[i].LoacalScale = Vector3.one * 0.6f;
            }

            nextBtn.onClick.Add(getEnemyFunc);
            Lbtn.onClick.Add(lFunc);
            Rbtn.onClick.Add(rFunc);

            getRewardBtn.onClick.Add(getRewardFunc);

            if (GameData.DPVPData.Count > 0 && lvRange != null)
                lvRange.InitData(7);
        }
    }

    public void OnNowRank(int lv)
    {
        lvRange.SetNowRankOffset(lv);
    }

    public void UpdateView(ref TTeam team)
    {
        getRewardBtn.isEnabled = GameFunction.CanGetPVPReward(ref team);
    }

    public bool Enable
    {
        set
        { 
            self.SetActive(value);
        }
        get{ return self.activeSelf; } 
    }

    private float tweenSpeed = 0.5f;

    public void DoTrun(int currentIndex)
    {
        lvRange.SetOffset(currentIndex);
        Sort.transform.DOLocalMoveX((260 - (260 * (currentIndex - 1))), tweenSpeed);

        int ex = currentIndex - 1;
        int next = currentIndex + 1;

        if (ex >= 1 && ex < GameData.DPVPData.Count)
        {
            pvplvs[ex - 1].self.transform.DOScale(0.6f, tweenSpeed);
        }

        if (next >= 1 && next < GameData.DPVPData.Count)
        {
            pvplvs[next - 1].self.transform.DOScale(0.6f, tweenSpeed);
        }

        pvplvs[currentIndex - 1].self.transform.DOScale(1f, tweenSpeed);

        if (GameData.DPVPData.ContainsKey(currentIndex))
        {
            award0.text = GameData.DPVPData[currentIndex].PVPCoin.ToString();
            award1.text = GameData.DPVPData[currentIndex].PVPCoinDaily.ToString();
        }
    }

    public bool IsInit
    {
        get{ return isInit; }
    }
}

public class EnterView
{
    private GameObject self;
    private UIButton NoBtn;
    private UIButton StartBtn;
    private UIButton ResetBtn;
    private UILabel ResearchPrice;
    private GameObject parent;
    private UILabel Combat1;
    private UILabel Combat2;
    private UILabel WinValueLabel;
    private UILabel LoseValueLabel;
    private UILabel StatusLabel;
    private UILabel TimesLabel;
    private EPVPSituation situation = EPVPSituation.CDIng;
    private bool isInit = false;
    private  TimeSpan checktime;

    private TItemRankGroup[] EnemyItems;

    public void Init(GameObject go, GameObject[] enemys, EventDelegate close, EventDelegate resetFunc, EventDelegate startFunc)
    {
        if (go)
        {
            self = go;
            GameObject parent = self.transform.Find("EnemyList/ScrollView").gameObject;
            EnemyItems = new TItemRankGroup[enemys.Length];

            for (int i = 0; i < enemys.Length; i++)
            {
                enemys[i].transform.parent = parent.transform;
                EnemyItems[i] = new TItemRankGroup();
                EnemyItems[i].Init(ref enemys[i]);
            }
            
            NoBtn = self.transform.Find("NoBtn").gameObject.GetComponent<UIButton>();
            StartBtn = self.transform.Find("StartBtn").gameObject.GetComponent<UIButton>();
            TimesLabel = StartBtn.transform.Find("Icon/TimesLabel").gameObject.GetComponent<UILabel>();
            StatusLabel = StartBtn.transform.Find("Icon/StatusLabel").gameObject.GetComponent<UILabel>();

            ResetBtn = self.transform.Find("ResetBtn").gameObject.GetComponent<UIButton>();
            Combat1 = self.transform.Find("CombatGroup/CombatLabel0").gameObject.GetComponent<UILabel>();
            Combat2 = self.transform.Find("CombatGroup/CombatLabel1").gameObject.GetComponent<UILabel>();
            WinValueLabel = self.transform.Find("ScoreGroup/WinValueLabel").gameObject.GetComponent<UILabel>();
            LoseValueLabel = self.transform.Find("ScoreGroup/LoseValueLabel").gameObject.GetComponent<UILabel>();
            ResearchPrice = ResetBtn.transform.Find("PriceLabel").gameObject.GetComponent<UILabel>();

            isInit = NoBtn && StartBtn && ResetBtn && Combat1 && Combat2 && WinValueLabel && LoseValueLabel && TimesLabel && ResearchPrice;
			
            ResetBtn.onClick.Add(resetFunc);
            StartBtn.onClick.Add(startFunc);
            NoBtn.onClick.Add(close);
        }
    }

    public void UpdateView(ref TTeam[] teams, int researchPrice)
    {
        ResearchPrice.text = researchPrice.ToString();
        if (teams.Length != EnemyItems.Length)
        {
            Debug.LogError("Data Erro");
            return;
        }
            
        TTeamRank[] datas = new TTeamRank[teams.Length]; 
        float enemyCombatPowerTotal = 0;
        int PVPEnemyIntegralTotal = 0;

        for (int i = 0; i < datas.Length; i++)
        {
            teams[i].Init();
            datas[i] = GameFunction.TTeamCoverTTeamRank(teams[i]);
            EnemyItems[i].UpdateView(datas[i]);
            EnemyItems[i].LocalPosititon = new Vector3(0, -130 * i, 0);
            enemyCombatPowerTotal += teams[i].Player.CombatPower();
            PVPEnemyIntegralTotal += teams[i].PVPIntegral;
        }

        //計算積分
        int winpoint = 0;
        int lostpoint = 0;
        int calculate = (int)(Mathf.Abs(GameData.Team.PVPIntegral - GameData.Team.PVPEnemyIntegral) / GameData.DPVPData[GameData.Team.PVPLv].Calculate);
        if (GameData.Team.PVPIntegral > GameData.Team.PVPEnemyIntegral)
        {
            winpoint = (GameData.DPVPData[GameData.Team.PVPLv].BasicScore - calculate);
            lostpoint = (GameData.DPVPData[GameData.Team.PVPLv].BasicScore + calculate);
        }
        else if (GameData.Team.PVPIntegral < GameData.Team.PVPEnemyIntegral)
        {
            winpoint = GameData.DPVPData[GameData.Team.PVPLv].BasicScore + calculate;
            lostpoint = GameData.DPVPData[GameData.Team.PVPLv].BasicScore - calculate;
        }
        else
        {
            winpoint = GameData.DPVPData[GameData.Team.PVPLv].BasicScore;
            lostpoint = winpoint;
        }

        WinValueLabel.text = string.Format("+{0}", winpoint);
        LoseValueLabel.text = string.Format("-{0}", lostpoint);
        Combat1.text = (enemyCombatPowerTotal / 3).ToString();
        Combat2.text = GameData.Team.Player.CombatPower().ToString();
    }

    public bool IsInit
    {
        get{ return isInit; }
    }

    public bool Enable
    {
        set
        { 
            self.SetActive(value);
        }
        get{ return self.activeSelf; } 
    }

    public EPVPSituation PVPSituation
    {
        set
        {  
            if (situation != value || situation == EPVPSituation.CDIng)
            {
                situation = value;
                switch (situation)
                {
                    case EPVPSituation.CDIng:
                        checktime = GameData.Team.PVPCD.ToUniversalTime().Subtract(DateTime.UtcNow);
                        StatusLabel.text = string.Format(TextConst.S(9729), GameFunction.GetTimeString(checktime));
                        break;
                    case EPVPSituation.CDEnd:
                        StatusLabel.text = TextConst.S(9721);
                        break;
                    case EPVPSituation.NoCountOrBuy:
                        StatusLabel.text = TextConst.S(9730);
                        break;
                }
                UpdatePVPTicket();
            }
        }

        get{ return situation; }
    }

    public void UpdatePVPTicket()
    {
        TimesLabel.text = string.Format(TextConst.S(9728), GameData.Team.PVPTicket, GameConst.PVPMaxTickket);
    }
}

public class PVPPage0
{
    private GameObject self;
    public PVPMainView mainview = new PVPMainView();
    public EnterView EnterPage = new EnterView();

    public void Init(GameObject go, ref GameObject[] pvplvBtns, GameObject[] itemRankGroups, EventDelegate getEnemyFunc,  
                     EventDelegate resetFunc, EventDelegate startFunc, EventDelegate lFunc, EventDelegate rFunc, EventDelegate nowFunc, EventDelegate getRewardFunc)
    {
        if (go)
        {
            self = go;
            mainview.Init(self.transform.Find("MainView").gameObject, ref pvplvBtns, getEnemyFunc, lFunc, rFunc, nowFunc, getRewardFunc);
            /*EnterPage.Init(self.transform.Find("EnterView").gameObject, itemRankGroups, 
                new EventDelegate(CloseEnterView),
                resetFunc,
                startFunc);
           */
            EnableEnterView = false;
        }
    }

    private void CloseEnterView()
    {
        EnableEnterView = false;
    }

    public void UpdateEnterView(ref TTeam[] team, int researchPrice)
    {
        //EnterPage.UpdateView(ref team, researchPrice);
    }

    public bool EnableEnterView
    {
        set
        { 
            //mainview.Enable = !value;
            //EnterPage.Enable = value;
        }
    }

    public bool Enable
    {
        set
        { 
            self.SetActive(value); 
        }
        get{ return self.activeSelf; } 
    }
}

public class PVPPage1
{
    private GameObject self;
    public PVPPage1TopView TopView;
    private PVPPage1ListView listView;

    public void Init(GameObject go, ref GameObject[] rank100, GameObject parent)
    {
        if (go)
        {
            self = go;

            TopView = new PVPPage1TopView();
            listView = new PVPPage1ListView();
            TopView.Init(self.transform.Find("TopView").gameObject, new EventDelegate(myRankInfo));
            listView.Init(self.transform.Find("ListView").gameObject, ref rank100, parent);
        }
    }

    private void myRankInfo()
    {
        SendMyRank();
    }

    public void UpdateView(TTeamRank myRank, TTeamRank[] otherRank)
    {
        listView.UpdateView(myRank, otherRank);
    }

    private void SendMyRank()
    {
        WWWForm form = new WWWForm();
        form.AddField("Language", GameData.Setting.Language.GetHashCode());
        form.AddField("PVPIntegral", GameData.Team.PVPIntegral);
        SendHttp.Get.Command(URLConst.PVPMyRank, WaitMyRank, form, false);
    }

    private void WaitMyRank(bool ok, WWW www)
    {
        if (ok)
        {
            TTeamRank data = JsonConvert.DeserializeObject <TTeamRank>(www.text, SendHttp.Get.JsonSetting);
            listView.UpdateViewMyrank(data); 
        }
    }
}

public class UIPVP : UIBase
{
    private static UIPVP instance = null;
    private const string UIName = "UIPVP";
    private const int pageCount = 4;
    private const float tweenSpeed = 0.5f;
    public int currentLv = 1;
    private int currecntPage = 0;
    private int shopIndex = -1;

    private GameObject[] pages = new GameObject[pageCount];
    private GameObject[] redPoints = new GameObject[pageCount];
    private UILabel labelPVPCoin;
    private GameObject itemRankgroupObj;

    //page0
    private TPvPLeagueGroup[] pvplvs = new TPvPLeagueGroup[GameData.DPVPData.Count];
    private GameObject uiSort;
    private UIButton buttonGetAward;
    private UIButton buttonNext;
    private UIButton buttonLeft;
    private UIButton buttonRight;
    private UILabel labelAward0;
    private UILabel labelAward1;
    private UILabel labelTime;
    private PVPLvRangeItem lvRange;

    //page1
    private UISprite iconPVPRank;
    private UILabel labelRangeName;
    private UILabel labelNowRange;
    private UILabel labelNowAward0;
    private UILabel labelNowAward1;
    private UILabel labelNextRangeName;
    private UILabel labelNextAward0;
    private UILabel labelNextAward1;
    private UISprite iconNextRank;
    private GameObject nextRankGroup;
    private GameObject anchorMyRank;
    private TItemRankGroup myRankInfo;
    private TItemRankGroup[] rankGroups;
    private TTeamRank[] rankData;

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

    public static UIPVP Get
    {
        get
        {
            if (!instance)
                instance = LoadUI(UIName) as UIPVP;
			
            return instance;
        }
    }

    public static void UIShow(bool isShow)
    {
        if (instance) {
            if (!isShow)
                RemoveUI(instance.gameObject);
            else
                instance.Show(isShow);
        } else 
        if (isShow)
            Get.Show(isShow);

        if (isShow)
            UIMainLobby.Get.Hide(2, false);
        else
            UIMainLobby.Get.Hide(3, false);
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);
        if (isShow)
        {
            DoPage(0);
            labelPVPCoin.text = GameData.Team.PVPCoin.ToString();
        }
    }
        
    protected override void InitCom()
    {
        SetBtnFun(UIName + "/BottomLeft/BackBtn", OnReturn);
        SetBtnFun(UIName + "/Center/Window/Pages/0/MainView/NextBtn", OnStart);
        SetBtnFun(UIName + "/Center/Window/Pages/0/MainView/ButtonGroup/LButton", OnLeft);
        SetBtnFun(UIName + "/Center/Window/Pages/0/MainView/ButtonGroup/RButton", OnRight);
        SetBtnFun(UIName + "/Center/Window/Pages/0/MainView/DailyAwardBtn", OnAward);

        itemRankgroupObj = Resources.Load("Prefab/UI/Items/ItemRankGroup") as GameObject;
        GameObject itemPVPLeague = Resources.Load("Prefab/UI/Items/PvPLeagueGroup") as GameObject;

        uiSort = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/PvPLeagueBoard/ScrollView/Sort");
        labelPVPCoin = GameObject.Find(UIName + "/TopRight/PVPCoin/Label").GetComponent<UILabel>();
        labelTime = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/NextBtn/Trim/TimesLabel").GetComponent<UILabel>();
        labelAward0 = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/AwardGroup/Award0/ValueLabel").GetComponent<UILabel>();
        labelAward1 = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/AwardGroup/Award1/ValueLabel").GetComponent<UILabel>();
        lvRange = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/PvPLeagueSlider/PVPLvRangeItem").GetComponent<PVPLvRangeItem>();
        buttonGetAward = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/DailyAwardBtn").GetComponent<UIButton>();

        GameObject[] pvplvBtns = new GameObject[GameData.DPVPData.Count];
        for (int i = 0; i < pvplvs.Length; i++)
        {
            pvplvBtns[i] = Instantiate(itemPVPLeague) as GameObject;
            pvplvBtns[i].name = (i + 1).ToString();

            pvplvs[i] = new TPvPLeagueGroup();
            pvplvs[i].Init(ref pvplvBtns[i], uiSort);
            pvplvs[i].UpdateView(i + 1);
            pvplvs[i].LoaclPosition = new Vector3(260 * i, 0, 0);
            pvplvs[i].LoacalScale = Vector3.one * 0.6f;
        }

        if (GameData.DPVPData.Count > 0 && lvRange != null)
            lvRange.InitData(7);

        nextRankGroup = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/NextRankGroup");
        labelNextRangeName = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/NextRankGroup/RangeNameLabel").GetComponent<UILabel>();
        labelNextAward0 = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/NextRankGroup/Award0/ValueLabel").GetComponent<UILabel>();
        labelNextAward1 = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/NextRankGroup/Award1/ValueLabel").GetComponent<UILabel>();
        iconNextRank = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/NextRankGroup/PvPRankIcon").GetComponent<UISprite>();
        labelNowAward0 = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/AwardGroup/Award0/ValueLabel").GetComponent<UILabel>();
        labelNowAward1 = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/AwardGroup/Award1/ValueLabel").GetComponent<UILabel>();
        iconPVPRank = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/NowRankGroup/PvPRankIcon").GetComponent<UISprite>();
        labelRangeName = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/NowRankGroup/RangeNameLabel").GetComponent<UILabel>();
        labelNowRange = GameObject.Find(UIName + "/Center/Window/Pages/1/TopView/NowRankGroup/NowRangeLabel").GetComponent<UILabel>();
        anchorMyRank = GameObject.Find(UIName + "/Center/Window/Pages/1/ListView/ScrollView");

        for (int i = 0; i < pageCount; i++) {
            SetBtnFun(UIName + "/Center/Window/Tabs/" + i.ToString(), OnPage);
            redPoints[i] = GameObject.Find(string.Format(UIName + "/Center/Window/Tabs/{0}/RedPoint", i));
            redPoints[i].SetActive(false);
            pages[i] = GameObject.Find(UIName + "/Center/Window/Pages/" + i.ToString());
            pages[i].SetActive(false);
        }
    }

    protected override void InitData() {
        currentLv = GameData.Team.PVPLv;
        shopIndex = GetShopIndex();
    }

    void FixedUpdate()
    {
        //if (currecntPage == 0)
            //ComputingCD();
    }

    private int GetShopIndex()
    {
        for (int i = 0; i < GameData.DShops.Length; i++)
            if (GameData.DShops[i].Kind == 2)
                return i;

        return -1;
    }

    public void OnStart()
    {
        for (var i = 0; i < GameData.PVPEnemyMembers.Length; i++)
            if (!string.IsNullOrEmpty(GameData.PVPEnemyMembers[i].Identifier)) {
                UIShow(false);
                UISelectRole.Get.LoadStage(GameData.DPVPData[GameData.Team.PVPLv].Stage);
                return;
            }
        
        WWWForm form = new WWWForm();
        form.AddField("Identifier", GameData.Team.Identifier);	
        form.AddField("Kind", 0);
        SendHttp.Get.Command(URLConst.PVPGetEnemy, WaitPVPGetEnemy, form, false); 
    }

    public void OnSearch()
    {
        int lv = GameData.Team.PVPLv;

        if (GameData.DPVPData.ContainsKey(lv))
        {
            if (CheckMoney(GameData.DPVPData[lv].SearchCost))
            {
                WWWForm form = new WWWForm();
                form.AddField("Identifier", GameData.Team.Identifier);	
                form.AddField("Kind", 1);
                SendHttp.Get.Command(URLConst.PVPGetEnemy, WaitPVPGetEnemy, form, true);
            }
        }			
    }
    /*
    private void OnPVPStart()
    {
        if (page0.EnterPage.IsInit)
        {
            switch (page0.EnterPage.PVPSituation)
            {
                case EPVPSituation.CDIng:
                    if (CheckDiamond(GameConst.PVPCD_Price))
                    {
                        UIMessage.Get.ShowMessage(TextConst.S(256), string.Format(TextConst.S(9732), GameConst.PVPCD_Price), SendBuyPVPCD);
                    }
                    break;
                case EPVPSituation.NoCountOrBuy:
                    //詢問購買次數, itemid
                    if (shopIndex != -1)
                    {
                        if (shopIndex < GameData.DShops.Length)
                        {
                            if (GameData.DShops[shopIndex].Limit != null)
                            {
                                if (GameData.Team.DailyCount.BuyPVPTicketCount < GameData.DShops[shopIndex].Limit.Length)
                                {
                                    int price = GameData.DShops[shopIndex].Limit[GameData.Team.DailyCount.BuyPVPTicketCount];
                                    if (CheckMoney(price))
                                    {
                                        int id = GameData.DShops[shopIndex].ItemID;
                                        if (GameData.DItemData.ContainsKey(id))
                                            UIMessage.Get.ShowMessage(TextConst.S(256), string.Format(TextConst.S(9731), price, GameData.DItemData[id].Name), SendBuyPVPTicket);
                                    }
                                }
                                else
                                {
                                    Debug.LogError("超出使用次數"); 
                                }
                            }
                            else
                            {
                                Debug.LogError("Shop Limit Error");
                            }
                        }
                        else
                        {
                            Debug.LogError("shopIndex Error");
                        }
                    }
                    else
                    {
                        Debug.LogError("Shop Json Error");
                    }
                    break;
                case EPVPSituation.CDEnd:
                    //先設定PVP關卡，等到UISelectRole.DoStart按下之後，開啟PVP戰鬥
                    int lv = GameFunction.GetPVPLv(GameData.Team.PVPIntegral);

                    if (GameData.DPVPData.ContainsKey(lv))
                    {
                        UIShow(false);
                        UISelectRole.Get.LoadStage(GameData.DPVPData[lv].Stage);
                    }
                    break;
            } 
        }
    }*/

    private void SendBuyPVPCD()
    {
        WWWForm form = new WWWForm();
        form.AddField("ShopIndex", -1);
        SendHttp.Get.Command(URLConst.PVPBuyTicket, WaitPVPBuyPVPCD, form, true);  
    }

    private void WaitPVPBuyPVPCD(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPBuyResult data = JsonConvert.DeserializeObject <TPVPBuyResult>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Diamond = data.Diamond;
            GameData.Team.PVPTicket = data.PVPTicket;
            GameData.Team.PVPCD = data.PVPCD;
            GameData.Team.DailyCount = data.DailyCount;
        }
        else
        {
            UIMessage.Get.ShowMessage(TextConst.S(233), TextConst.S(238)); 
        }
    }

    private void SendBuyPVPTicket()
    {
        WWWForm form = new WWWForm();
        form.AddField("ShopIndex", shopIndex);
        SendHttp.Get.Command(URLConst.PVPBuyTicket, WaitPVPBuyTicket, form, true);	
    }

    private void WaitPVPBuyTicket(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPBuyResult data = JsonConvert.DeserializeObject <TPVPBuyResult>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Diamond = data.Diamond;
            GameData.Team.PVPTicket = data.PVPTicket;
            GameData.Team.PVPCD = data.PVPCD;
            GameData.Team.DailyCount = data.DailyCount;
            labelTime.text = string.Format(TextConst.S(9728), GameData.Team.PVPTicket, GameConst.PVPMaxTickket);
        }
    }

    private void openRecharge()
    {
        UIRecharge.UIShow(true);
    }

    private void OnLeft()
    {
        if (currentLv > 1)
        {
            currentLv--;
            TurnLv(currentLv);
        } 
    }

    private void OnRight()
    {
        if (currentLv < GameData.DPVPData.Count)
        {
            currentLv++;
            TurnLv(currentLv);
        } 
    }

    private void OnNowRank()
    {
        int lv = GameData.Team.PVPLv;
        currentLv = lv;
        lvRange.SetNowRankOffset(lv);  
        TurnLv(lv);
    }

    private void OnAward()
    {
        if (GameFunction.CanGetPVPReward(ref GameData.Team))
        {
            WWWForm form = new WWWForm();
            SendHttp.Get.Command(URLConst.PVPAward, WaitPVPAward, form, false); 
        }
    }

    private void WaitPVPAward(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPReward data = JsonConvert.DeserializeObject <TPVPReward>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.DailyCount = data.DailyCount;
            UIGetItem.Get.AddExp(2, data.PVPCoin - GameData.Team.PVPCoin);
            UIGetItem.Get.SetTitle(TextConst.S(9707));
            GameData.Team.PVPCoin = data.PVPCoin;
            labelPVPCoin.text = GameData.Team.PVPCoin.ToString();
            buttonGetAward.isEnabled = false;
            redPoints[0].SetActive(false);
        }
    }

    public void TurnLv(int currentIndex)
    {
        lvRange.SetOffset(currentIndex);
        uiSort.transform.DOLocalMoveX((260 - (260 * (currentIndex - 1))), tweenSpeed);

        int ex = currentIndex - 1;
        int next = currentIndex + 1;

        if (ex >= 1 && ex < GameData.DPVPData.Count)
        {
            pvplvs[ex - 1].self.transform.DOScale(0.6f, tweenSpeed);
        }

        if (next >= 1 && next < GameData.DPVPData.Count)
        {
            pvplvs[next - 1].self.transform.DOScale(0.6f, tweenSpeed);
        }

        pvplvs[currentIndex - 1].self.transform.DOScale(1f, tweenSpeed);

        if (GameData.DPVPData.ContainsKey(currentIndex))
        {
            labelAward0.text = GameData.DPVPData[currentIndex].PVPCoin.ToString();
            labelAward1.text = GameData.DPVPData[currentIndex].PVPCoinDaily.ToString();
        }
    }

    public void WaitPVPGetEnemy(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPEnemyTeams data = JsonConvert.DeserializeObject <TPVPEnemyTeams>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Money = data.Money;
            GameData.Team.PVPEnemyIntegral = data.PVPEnemyIntegral;
            int lv = GameData.Team.PVPLv;
   
            if (data.Teams != null)
            {
                int num = Mathf.Min(data.Teams.Length, GameData.EnemyMembers.Length);
                for (int i = 0; i < num; i++)
                {
                    data.Teams[i].PlayerInit();
                    GameData.PVPEnemyMembers[i] = data.Teams[i];
                    GameData.EnemyMembers[i] = data.Teams[i];
                }
            }

            UIShow(false);
            UISelectRole.Get.LoadStage(GameData.DPVPData[GameData.Team.PVPLv].Stage);
        } else
            UIHint.Get.ShowHint(TextConst.S(255), Color.red);
            //UIMessage.Get.ShowMessage(TextConst.S(256), TextConst.S(255));
    }

    private void SendPVPRank()
    {
        if (rankData == null || rankData.Length == 0)
        {
            WWWForm form = new WWWForm();
            form.AddField("PVPLv", GameData.Team.PVPLv);
            form.AddField("Language", GameData.Setting.Language.GetHashCode());
            SendHttp.Get.Command(URLConst.PVPRank, WaitSendPVPRank, form, true);
        }
    }



    public void WaitSendPVPRank(bool ok, WWW www)
    {/*
        if (ok)
        {
            rankdata = JsonConvert.DeserializeObject <TTeamRank[]>(www.text, SendHttp.Get.JsonSetting);
//            TTeamRank[] data = JsonConvert.DeserializeObject <TTeamRank[]>(www.text, SendHttp.Get.JsonSetting);
            //TODO:更新Rank資料：
            TTeamRank myrank = GameFunction.TTeamCoverTTeamRank(GameData.Team);
            page1.UpdateView(myrank, rankdata);
        }*/
    }

    private void DoPage(int index) {
        currecntPage = index;
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(false);

        pages[index].SetActive(true);
        switch (index)
        {
            case 0:
                buttonGetAward.isEnabled = GameFunction.CanGetPVPReward(ref GameData.Team);
                redPoints[0].SetActive(buttonGetAward.isEnabled);
                OnNowRank();
                break;
            case 1:
                //page1.TopView.UpdateView(GameData.Team);
                SendPVPRank();
                break;
        }
    }

    public void OnPage()
    {
        int index;

        if (int.TryParse(UIButton.current.name, out index))
            DoPage(index);
    }

    public void OnReturn()
    {
        UIShow(false);
        UIGameLobby.Get.Show();
    }

    private void ComputingCD()
    {/*
        if (GameData.Team.PVPTicket > 0)
        {
            if (page0.EnterPage.PVPSituation == EPVPSituation.CDIng || page0.EnterPage.PVPSituation == EPVPSituation.NoCountOrBuy)
            {
                int sec = (int)GameData.Team.PVPCD.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds;
                if (sec < 0)
                {
                    page0.EnterPage.PVPSituation = EPVPSituation.CDEnd;
                }
                else
                {
                    page0.EnterPage.PVPSituation = EPVPSituation.CDIng; 
                }
            }
        }
        else
        {
            page0.EnterPage.PVPSituation = EPVPSituation.NoCountOrBuy;
        }*/
    }
}
