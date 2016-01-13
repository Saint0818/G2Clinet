using UnityEngine;
using GameStruct;
using GameItem;
using Newtonsoft.Json;

public class PVPPage1TopView
{
    private GameObject self;
    private UISprite PvPRankIcon;
    private UILabel RangeNameLabel;
    private UILabel NowRangeLabel;
    private UIButton MyRankBtn;
    private UILabel Award0;
    private UILabel Award1;
    private bool isInit = false;

    public void Init(GameObject go, EventDelegate myRankFunc)
    {
        if (go)
        {
            self = go;
            PvPRankIcon = self.transform.FindChild("NowRankGroup/PvPRankIcon").gameObject.GetComponent<UISprite>();
            RangeNameLabel = self.transform.FindChild("NowRankGroup/RangeNameLabel").gameObject.GetComponent<UILabel>();
            NowRangeLabel = self.transform.FindChild("NowRankGroup/NowRangeLabel").gameObject.GetComponent<UILabel>();
            MyRankBtn = self.transform.FindChild("MyRankBtn").gameObject.GetComponent<UIButton>();
            Award0 = self.transform.FindChild("AwardGroup/Award0/ValueLabel").gameObject.GetComponent<UILabel>();
            Award1 = self.transform.FindChild("AwardGroup/Award1/ValueLabel").gameObject.GetComponent<UILabel>();
            isInit = self && PvPRankIcon && RangeNameLabel && NowRangeLabel && MyRankBtn && Award0 && Award1;
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
                RangeNameLabel.text = team.LeagueName;
                NowRangeLabel.text = string.Format("{0}-{1}", GameData.DPVPData[lv].LowScore, GameData.DPVPData[lv].HighScore);

                Award0.text = GameData.DPVPData[lv].Money.ToString();
                Award1.text = GameData.DPVPData[lv].PVPCoin.ToString();
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

    public void Init(GameObject go, GameObject[] rank100, GameObject parent)
    {
        if (go)
        {
            self = go;
            myRankInfo = new TItemRankGroup();
            GameObject myrank = self.transform.FindChild("ItemRankGroup").gameObject;

            myRankInfo.Init(myrank, new EventDelegate(CloseMyRank));
            myRankInfo.Enable = false;

            ranks = new TItemRankGroup[rank100.Length];

            isInit = self && myrank;
           
            if (isInit)
                for (int i = 0; i < rank100.Length; i++)
                {
                    ranks[i] = new TItemRankGroup();
                    ranks[i].Init(rank100[i]);
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
            {
                ranks[i].UpdateView(data[i]);
            }
            else
                ranks[i].Enable = false;
        }
    }
}

public class PVPPage1
{
    private GameObject self;
    private PVPPage1TopView topView;
    private PVPPage1ListView listView;

    public void Init(GameObject go, GameObject[] rank100, GameObject parent)
    {
        if (go)
        {
            self = go;

            topView = new PVPPage1TopView();
            listView = new PVPPage1ListView();
            topView.Init(self.transform.Find("TopView").gameObject, new EventDelegate(myRankInfo));
            listView.Init(self.transform.Find("ListView").gameObject, rank100, parent);
        }
    }

    private void myRankInfo()
    {
        listView.MyRankEnable = true;
    }

    public void UpdateView(TTeamRank myRank, TTeamRank[] otherRank)
    {
        topView.UpdateView(GameData.Team);
        listView.UpdateView(myRank, otherRank);
    }
}

public class UIPVP : UIBase
{
    private static UIPVP instance = null;
    private const string UIName = "UIPVP";
    private int pageCount = 2;
    private UIButton[] tabs;
    private GameObject[] pages;
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
        page1 = new PVPPage1();

        for (int i = 0; i < pageCount; i++)
        {
            tabs[i] = GameObject.Find(string.Format(UIName + "/Center/Window/Tabs/{0}", i)).GetComponent<UIButton>(); 
            tabs[i].onClick.Add(new EventDelegate(OnPage));
            pages[i] = GameObject.Find(string.Format(UIName + "/Center/Window/Pages/{0}", i)); 

            switch (i)
            {
                case 0:
                    break;
                case 1:
                    GameObject go = Resources.Load("Prefab/UI/Items/ItemRankGroup") as GameObject;
                    GameObject parent = pages[i].transform.FindChild("ListView/ScrollView").gameObject;
                    if (go)
                    {
                        GameObject[] gos = new GameObject[GameConst.PVPMaxSort];

                        for (int j = 0; j < gos.Length; j++)
                            gos[j] = Instantiate(go) as GameObject;
                        
                        page1.Init(pages[i], gos, parent);
                    }
                    else
                    {
                        Debug.LogError("Resources.load error : ItemRankGroup");
                    }

                    SendPVPRank();    
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

    public void SendPVPRank()
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
            TTeamRank[] data = (TTeamRank[])JsonConvert.DeserializeObject(www.text, typeof(TTeamRank[]));
            UpdateRank(data);
        }
    }

    private void UpdateRank(TTeamRank[] otherRank)
    {
        page1.UpdateView(GameFunction.TTeamCoverTTeamRank(GameData.Team), otherRank);
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
    }

    public void OnReturn()
    {
        UIShow(false);
        UIMainLobby.Get.Show();
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
