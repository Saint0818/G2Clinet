﻿using UnityEngine;
using GameStruct;
using GameItem;
using Newtonsoft.Json;
using DG.Tweening;

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
            PvPRankIcon = self.transform.FindChild("NowRankGroup/PvPRankIcon").gameObject.GetComponent<UISprite>();
            RangeNameLabel = self.transform.FindChild("NowRankGroup/RangeNameLabel").gameObject.GetComponent<UILabel>();
            NowRangeLabel = self.transform.FindChild("NowRankGroup/NowRangeLabel").gameObject.GetComponent<UILabel>();

            NextRankGroup = self.transform.FindChild("NextRankGroup").gameObject;
            NextRangeNameLabel = NextRankGroup.transform.FindChild("RangeNameLabel").GetComponent<UILabel>();
            NextAward0 = NextRankGroup.transform.FindChild("Award0/ValueLabel").GetComponent<UILabel>();
            NextAward1 = NextRankGroup.transform.FindChild("Award1/ValueLabel").GetComponent<UILabel>();
            NextRankIcon = NextRankGroup.transform.FindChild("PvPRankIcon").gameObject.GetComponent<UISprite>();

            MyRankBtn = self.transform.FindChild("MyRankBtn").gameObject.GetComponent<UIButton>();
            NowAward0 = self.transform.FindChild("AwardGroup/Award0/ValueLabel").gameObject.GetComponent<UILabel>();
            NowAward1 = self.transform.FindChild("AwardGroup/Award1/ValueLabel").gameObject.GetComponent<UILabel>();
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
            int lv = GameFunction.GetPVPLv(team.PVPIntegral);
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
            GameObject myrank = self.transform.FindChild("ItemRankGroup").gameObject;

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
    private GameObject ThumbOffset;
    private GameObject NowRankOffset;
    private UILabel ThumbOffsetLabel;
    private UIButton nowRankBtn;
    private UIButton getRewardBtn;
	
    private bool isInit = false;

    public void Init(GameObject go, ref GameObject[] lvs, EventDelegate getEnemyFunc, EventDelegate lFunc, 
                   EventDelegate rFunc, EventDelegate nowRankFunc, EventDelegate getRewardFunc)
    {
        if (go)
        {
            self = go;
            nextBtn = self.transform.FindChild("NextBtn").gameObject.GetComponent<UIButton>();
            Lbtn = self.transform.FindChild("ButtonGroup/LButton").gameObject.GetComponent<UIButton>();
            Rbtn = self.transform.FindChild("ButtonGroup/RButton").gameObject.GetComponent<UIButton>();
            Sort = self.transform.FindChild("PvPLeagueBoard/ScrollView/Sort").gameObject;
            award0 = self.transform.FindChild("AwardGroup/Award0/ValueLabel").gameObject.GetComponent<UILabel>();
            award1 = self.transform.FindChild("AwardGroup/Award1/ValueLabel").gameObject.GetComponent<UILabel>();
            ThumbOffset = self.transform.FindChild("PvPLeagueSlider/Thumb/Offset").gameObject;
            ThumbOffsetLabel = ThumbOffset.transform.FindChild("ScoreLabel").gameObject.GetComponent<UILabel>();
            NowRankOffset = self.transform.FindChild("PvPLeagueSlider/NowRank/Offset").gameObject;
            nowRankBtn = NowRankOffset.GetComponent<UIButton>();
            getRewardBtn = self.transform.FindChild("DailyAwardBtn").gameObject.GetComponent<UIButton>();

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

            isInit = self && nextBtn && Sort && Lbtn && Rbtn && nowRankBtn && ThumbOffset && NowRankOffset && ThumbOffsetLabel;

            if (isInit)
            {
                nextBtn.onClick.Add(getEnemyFunc);
                Lbtn.onClick.Add(lFunc);
                Rbtn.onClick.Add(rFunc);
                nowRankBtn.onClick.Add(nowRankFunc);
                getRewardBtn.onClick.Add(getRewardFunc);
            }
        }
    }

    public void OnNowRank(int lv)
    {
        NowRankOffset.transform.localPosition = new Vector3((lv - 1) * 40, 0, 0);
//        SetOffset(lv);
    }

    public void SetOffset(int lv)
    {
        ThumbOffset.transform.DOLocalMoveX((lv - 1) * 40, tweenSpeed);
        if (GameData.DPVPData.ContainsKey(lv))
        {
            ThumbOffsetLabel.text = string.Format(TextConst.S(9741), GameData.DPVPData[lv].LowScore, GameData.DPVPData[lv].HighScore);
        }
    }

    public void UpdateView(ref TTeam team)
    {
        getRewardBtn.enabled = team.PVPDailyReaward == 0;
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
        SetOffset(currentIndex);
        Sort.transform.DOLocalMoveX((260 - (260 * (currentIndex - 1))), tweenSpeed);

        int ex = currentIndex - 1;
        int next = currentIndex + 1;

        if (ex >= 1 && ex <= GameConst.PVPMaxLv)
        {
            pvplvs[ex - 1].self.transform.DOScale(0.6f, tweenSpeed);
        }

        if (next >= 1 && next <= GameConst.PVPMaxLv)
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

    private TItemRankGroup[] EnemyItems;

    public void Init(GameObject go, GameObject[] enemys, EventDelegate close, EventDelegate resetFunc, EventDelegate startFunc)
    {
        if (go)
        {
            self = go;
            GameObject parent = self.transform.FindChild("EnemyList/ScrollView").gameObject;
            EnemyItems = new TItemRankGroup[enemys.Length];

            for (int i = 0; i < enemys.Length; i++)
            {
                enemys[i].transform.parent = parent.transform;
                EnemyItems[i] = new TItemRankGroup();
                EnemyItems[i].Init(ref enemys[i]);
            }
            
            NoBtn = self.transform.FindChild("NoBtn").gameObject.GetComponent<UIButton>();
            StartBtn = self.transform.FindChild("StartBtn").gameObject.GetComponent<UIButton>();
            ResetBtn = self.transform.FindChild("ResetBtn").gameObject.GetComponent<UIButton>();
            Combat1 = self.transform.FindChild("CombatGroup/CombatLabel0").gameObject.GetComponent<UILabel>();
            Combat2 = self.transform.FindChild("CombatGroup/CombatLabel1").gameObject.GetComponent<UILabel>();
            WinValueLabel = self.transform.FindChild("ScoreGroup/WinValueLabel").gameObject.GetComponent<UILabel>();
            LoseValueLabel = self.transform.FindChild("ScoreGroup/LoseValueLabel").gameObject.GetComponent<UILabel>();

            ResearchPrice = ResetBtn.transform.FindChild("PriceLabel").gameObject.GetComponent<UILabel>();
			
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
        int calculate = (int)(Mathf.Abs(GameData.Team.PVPIntegral - GameData.Team.PVPEnemyIntegral)/ GameData.DPVPData[GameData.Team.PVPLv].Calculate);
        if (GameData.Team.PVPIntegral > GameData.Team.PVPEnemyIntegral)
        {
            winpoint = (GameData.DPVPData[GameData.Team.PVPLv].BasicScore - calculate);
            lostpoint = (GameData.DPVPData[GameData.Team.PVPLv].BasicScore + calculate);
        }
        else if(GameData.Team.PVPIntegral < GameData.Team.PVPEnemyIntegral)
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

    public bool Enable
    {
        set
        { 
            self.SetActive(value);
        }
        get{ return self.activeSelf; } 
    }
}

public class PVPPage0
{
    private GameObject self;
    public PVPMainView mainview = new PVPMainView();
    private EnterView enterView = new EnterView();

    public void Init(GameObject go, ref GameObject[] pvplvBtns, GameObject[] itemRankGroups, EventDelegate getEnemyFunc,  
                     EventDelegate resetFunc, EventDelegate startFunc, EventDelegate lFunc, EventDelegate rFunc, EventDelegate nowFunc, EventDelegate getRewardFunc)
    {
        if (go)
        {
            self = go;
            mainview.Init(self.transform.FindChild("MainView").gameObject, ref pvplvBtns, getEnemyFunc, lFunc, rFunc, nowFunc, getRewardFunc);
            enterView.Init(self.transform.FindChild("EnterView").gameObject, itemRankGroups, 
                new EventDelegate(CloseEnterView),
                resetFunc,
                startFunc);

            EnableEnterView = false;


//            mainview.UpdateView();
        }
    }

    public void UpdateMainView(ref TTeam team)
    {
        mainview.UpdateView(ref team);
    }

    private void CloseEnterView()
    {
        EnableEnterView = false;
    }

    public void UpdateEnterView(ref TTeam[] team, int researchPrice)
    {
        enterView.UpdateView(ref team, researchPrice);
    }

    public bool EnableEnterView
    {
        set
        { 
            mainview.Enable = !value;
            enterView.Enable = value;
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
            TTeamRank data = (TTeamRank)JsonConvert.DeserializeObject(www.text, typeof(TTeamRank));
            listView.UpdateViewMyrank(data); 
        }
    }
}

public class UIPVP : UIBase
{
    private static UIPVP instance = null;
    private const string UIName = "UIPVP";
    private int pageCount = 2;
    private UIButton[] tabs;
    private GameObject[] pages;
    private PVPPage0 page0;
    private PVPPage1 page1;

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
        if (instance)
        {
            if (!isShow)
            {
                RemoveUI(UIName);
            }
            else
            {
                instance.Show(isShow);
            }
        }
        else if (isShow)
        {
            Get.Show(isShow);
        }
    }

    protected override void InitCom()
    {
        tabs = new UIButton[pageCount];
        pages = new GameObject[pageCount];
        page0 = new PVPPage0();
        page1 = new PVPPage1();
        currentLv = GameFunction.GetPVPLv(GameData.Team.PVPIntegral);
        for (int i = 0; i < pageCount; i++)
        {
            tabs[i] = GameObject.Find(string.Format(UIName + "/Center/Window/Tabs/{0}", i)).GetComponent<UIButton>(); 
            tabs[i].onClick.Add(new EventDelegate(OnPage));
            pages[i] = GameObject.Find(string.Format(UIName + "/Center/Window/Pages/{0}", i));

            UnityEngine.Object go;
            UnityEngine.Object itemRankgroupObj = Resources.Load("Prefab/UI/Items/ItemRankGroup");
            GameObject parent;
            GameObject[] gos;
            GameObject[] itemRankgroups;

            switch (i)
            {
                case 0:
                    go = Resources.Load("Prefab/UI/Items/PvPLeagueGroup");
                    GameObject[] pvplvBtns = new GameObject[GameConst.PVPMaxLv];

                    for (int j = 0; j < pvplvBtns.Length; j++)
                    {
                        pvplvBtns[j] = Instantiate(go) as GameObject;
                        pvplvBtns[j].name = (j + 1).ToString();
                    }

                    itemRankgroups = new GameObject[3];
                    for (int j = 0; j < itemRankgroups.Length; j++)
                    {
                        itemRankgroups[j] = Instantiate(itemRankgroupObj) as GameObject;
                    }

                    page0.Init(pages[i], ref pvplvBtns, itemRankgroups,
                        new EventDelegate(OnGetEnemy),
                        new EventDelegate(OnSearch),
                        new EventDelegate(OnPVPStart),
                        new EventDelegate(OnLeft),
                        new EventDelegate(OnRight),
                        new EventDelegate(OnNowRank),
                        new EventDelegate(OnAward));
                    break;
                case 1:
                    
                    parent = pages[i].transform.FindChild("ListView/ScrollView").gameObject;
                    if (itemRankgroupObj)
                    {
                        gos = new GameObject[GameConst.PVPMaxSort];

                        for (int j = 0; j < gos.Length; j++)
                            gos[j] = Instantiate(itemRankgroupObj) as GameObject;
                        
                        page1.Init(pages[i], ref gos, parent);
                    }
                    else
                    {
                        Debug.LogError("Resources.load error : ItemRankGroup");
                    }
                    break;
            }
        }
        SetBtnFun(UIName + "/BottomLeft/BackBtn", OnReturn);
    }

    public void OnPage()
    {
        int index;

        if (int.TryParse(UIButton.current.name, out index))
            DoPage(index);
    }

    public void OnGetEnemy()
    {
        bool isalreadyFind = true;

        for (var i = 0; i < GameData.PVPEnemyMembers.Length; i++)
        {
            if (GameData.PVPEnemyMembers[i].Identifier == null || GameData.PVPEnemyMembers[i].Identifier == string.Empty)
            {
                isalreadyFind = false;
                continue;
            }
        }

        if (isalreadyFind)
        {
            page0.EnableEnterView = true;
            int lv = GameFunction.GetPVPLv(GameData.Team.PVPIntegral);

            if (GameData.DPVPData.ContainsKey(lv))
            {
                page0.UpdateEnterView(ref GameData.PVPEnemyMembers, GameData.DPVPData[lv].SearchCost); 
            }
            else
                Debug.LogError("Error : not Found PVPData " + lv); 
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("Kind", 0);
            SendHttp.Get.Command(URLConst.PVPGetEnemy, WaitPVPGetEnemy, form, false); 
        }
    }

    public void OnSearch()
    {
        int lv = GameFunction.GetPVPLv(GameData.Team.PVPIntegral);

        if (GameData.DPVPData.ContainsKey(lv))
        {
            if (CheckMoney(GameData.DPVPData[lv].SearchCost))
            {
                WWWForm form = new WWWForm();
                form.AddField("Kind", 1);
                SendHttp.Get.Command(URLConst.PVPGetEnemy, WaitPVPGetEnemy, form, true);
            }
        }			
    }

    private void WaitPVPSrarch(bool ok, WWW www)
    {
        if (ok)
        {
            
        }
    }

    private void OnPVPStart()
    {
        int lv = GameFunction.GetPVPLv(GameData.Team.PVPIntegral);
       
        if (GameData.DPVPData.ContainsKey(lv))
        {
            //GameData.StageID = GameData.DPVPData[lv].Stage;
            UISelectRole.Get.LoadStage(GameData.DPVPData[lv].Stage);
        }
        //SceneMgr.Get.ChangeLevel(ESceneName.SelectRole);
    }

    //    public void WaitPVPStart(bool ok, WWW www)
    //    {
    //        if (ok)
    //        {
    //            TPVPStart data = (TPVPStart)JsonConvert.DeserializeObject(www.text, typeof(TPVPStart));
    //
    //            if (data.CanBattle)
    //            {
    //                //TODO:戰鬥
    //            }
    //        }
    //        else
    //        {
    //
    //        }
    //    }

    public int currentLv = 1;

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
        if (currentLv < GameConst.PVPMaxLv)
        {
            currentLv++;
            TurnLv(currentLv);
        } 
    }

    private void OnNowRank()
    {
        int lv = GameFunction.GetPVPLv(GameData.Team.PVPIntegral);
        currentLv = lv;
        page0.mainview.OnNowRank(lv);  
        TurnLv(lv);
    }

    private void OnAward()
    {
        if (GameData.Team.PVPDailyReaward == 0)
        {
            WWWForm form = new WWWForm();
            SendHttp.Get.Command(URLConst.PVPAward, WaitPVPAward, form, false); 
        }
    }

    private void WaitPVPAward(bool ok, WWW www)
    {
        if (ok)
        {
            //Update pvpcoin
        }
    }

    public void TurnLv(int i)
    {
        page0.mainview.DoTrun(i);
        //TODO:update data
    }

    public void WaitPVPGetEnemy(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPEnemyTeams data = JsonConvert.DeserializeObject <TPVPEnemyTeams>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.Money = data.Money;
            GameData.Team.PVPEnemyIntegral = data.PVPEnemyIntegral;
            UIMainLobby.Get.UpdateUI();

            page0.EnableEnterView = true;

            int lv = GameFunction.GetPVPLv(GameData.Team.PVPIntegral);

            if (GameData.DPVPData.ContainsKey(lv))
            {
                page0.UpdateEnterView(ref data.Teams, GameData.DPVPData[lv].SearchCost); 
            }
            else
                Debug.LogError("Error : not Found PVPData " + lv);
   
            if (data.Teams != null)
            {
                int num = Mathf.Min(data.Teams.Length, GameData.EnemyMembers.Length);
                for (int i = 0; i < num; i++)
                {
                    data.Teams[i].Init();
                    GameData.PVPEnemyMembers[i] = data.Teams[i];
                    GameData.EnemyMembers[i] = data.Teams[i];
                }
            }
        }
        else
        {	
        }
    }



    private void SendPVPRank()
    {
        WWWForm form = new WWWForm();
        form.AddField("PVPRankLv", GameFunction.GetPVPLv(GameData.Team.PVPIntegral));
        form.AddField("Language", GameData.Setting.Language.GetHashCode());
        SendHttp.Get.Command(URLConst.PVPRank, WaitSendPVPRank, form, false);
    }

    public void WaitSendPVPRank(bool ok, WWW www)
    {
        if (ok)
        {
            TTeamRank[] data = JsonConvert.DeserializeObject <TTeamRank[]>(www.text, SendHttp.Get.JsonSetting);
            //TODO:更新Rank資料：
            TTeamRank myrank = GameFunction.TTeamCoverTTeamRank(GameData.Team);
            page1.UpdateView(myrank, data);
        }
    }

    private void DoPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (index == i)
                pages[i].SetActive(true);
            else
                pages[i].SetActive(false);
        }

        switch (index)
        {
            case 0:
                page0.UpdateMainView(ref GameData.Team);
                OnNowRank();
                break;
            case 1:
                page1.TopView.UpdateView(GameData.Team);
                SendPVPRank();
                break;
        }
    }

    public void OnReturn()
    {
        UIShow(false);
        UIGameLobby.Get.Show();
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);
        if (isShow)
        {
            DoPage(0);
        }
    }
}
