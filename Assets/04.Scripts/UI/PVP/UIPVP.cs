using UnityEngine;
using GameStruct;
using GameItem;
using Newtonsoft.Json;
using DG.Tweening;
using System;

public class UIPVP : UIBase
{
    private static UIPVP instance = null;
    private const string UIName = "UIPVP";
    private const int pageCount = 3;
    private const float tweenSpeed = 0.5f;
    public int pvpLv = 1;
	public int currentLv = 1;
    private int currecntPage = 0;
    private int shopIndex = -1;

    private GameObject[] pages = new GameObject[pageCount];
    private GameObject[] redPoints = new GameObject[pageCount];
    private UILabel labelPVPCoin;
	private TimeSpan checktime;
 
    //page0
    private TPVPLeagueGroup[] pvplvs = new TPVPLeagueGroup[GameData.DPVPData.Count];
    private GameObject uiSort;
    private UIButton buttonGetAward;
    private UIButton buttonNext;
    private UIButton buttonLeft;
    private UIButton buttonRight;
    private UILabel labelAward0;
    private UILabel labelAward1;
	private UILabel labelStatus;
	private UILabel labelStart;
    private UILabel labelTime;
	private UILabel labelRank;
	private UILabel labelExp;
	private UISprite spriteRank;
	private UISprite spriteTicket;
	private UISlider sliderExp;
    private PVPLvRangeItem lvRange;

	private GameObject itemPVPRankGroup;
    //page1
	private GameObject anchorLocalList;
	private TItemRankGroup[] leaderBoardLocalItems = null;

	//page2
	private GameObject anchorGlobalList;
	private TItemRankGroup[] leaderBoardGlobalItems = null;

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
        {
            UIMainLobby.Get.Hide(false);
            UIResource.Get.Show(2);

            Statistic.Ins.LogScreen(4);
        }
        else
        {
            UIMainLobby.Get.Hide(false);
            UIResource.Get.Show();
        }
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);
        if (isShow)
        {
            DoPage(0);
            //sendMyRank();
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
        SetBtnFun(UIName + "/Center/Window/Tabs/Shop", OnShop);

		itemPVPRankGroup = Resources.Load("Prefab/UI/Items/ItemRankGroup") as GameObject;
        GameObject itemPVPLeague = Resources.Load("Prefab/UI/Items/PvPLeagueGroup") as GameObject;

        uiSort = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/PvPLeagueBoard/ScrollView/Sort");
        labelPVPCoin = GameObject.Find(UIName + "/TopRight/PVPCoin/Label").GetComponent<UILabel>();
		labelStatus = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/NextBtn/StatusLabel").GetComponent<UILabel>();
		labelStart = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/NextBtn/Label").GetComponent<UILabel>();
		labelTime = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/Trim/TimesLabel").GetComponent<UILabel>();
		labelRank = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/EXPView/Rank").GetComponent<UILabel>();
		labelExp = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/EXPView/Label").GetComponent<UILabel>();
		labelAward0 = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/Award0/ValueLabel").GetComponent<UILabel>();
        labelAward1 = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/Award1/ValueLabel").GetComponent<UILabel>();
        lvRange = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/PvPLeagueSlider/PVPLvRangeItem").GetComponent<PVPLvRangeItem>();
		spriteRank = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/EXPView/Icon").GetComponent<UISprite>();
		spriteTicket = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/NextBtn/Icon").GetComponent<UISprite>();
		sliderExp = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/EXPView/ProgressBar").GetComponent<UISlider>();
		buttonGetAward = GameObject.Find(UIName + "/Center/Window/Pages/0/MainView/DailyAwardBtn").GetComponent<UIButton>();
		anchorLocalList = GameObject.Find(UIName + "/Center/Window/Pages/1/ListView/ScrollView");
		anchorGlobalList = GameObject.Find(UIName + "/Center/Window/Pages/2/ListView/ScrollView");

        GameObject[] pvplvBtns = new GameObject[GameData.DPVPData.Count];
        for (int i = 0; i < pvplvs.Length; i++)
        {
            pvplvBtns[i] = Instantiate(itemPVPLeague) as GameObject;
            pvplvBtns[i].name = (i + 1).ToString();

            pvplvs[i] = new TPVPLeagueGroup();
            pvplvs[i].Init(ref pvplvBtns[i], uiSort);
            pvplvs[i].UpdateView(i + 1);
            pvplvs[i].LoaclPosition = new Vector3(260 * i, 0, 0);
            pvplvs[i].LoacalScale = Vector3.one * 0.8f;
        }

        if (GameData.DPVPData.Count > 0 && lvRange != null)
            lvRange.InitData(7);
        
        for (int i = 0; i < pageCount; i++) {
            SetBtnFun(UIName + "/Center/Window/Tabs/" + i.ToString(), OnPage);
            redPoints[i] = GameObject.Find(string.Format(UIName + "/Center/Window/Tabs/{0}/RedPoint", i));
            redPoints[i].SetActive(false);
            pages[i] = GameObject.Find(UIName + "/Center/Window/Pages/" + i.ToString());
            pages[i].SetActive(false);
        }
    }

    protected override void InitData() {
		shopIndex = GetShopIndex();
		pvpLv = GameData.Team.PVPLv;
		currentLv = pvpLv;
        labelRank.text = string.Format(TextConst.S (9737), currentLv);
		spriteRank.spriteName = "IconRank" + currentLv.ToString ();
		updateUI ();

		if (GameData.DPVPData.ContainsKey (currentLv)) {
			int exp = GameData.Team.PVPIntegral;
			labelExp.text = exp.ToString () + " / " + GameData.DPVPData [currentLv].HighScore.ToString ();
			sliderExp.value  = exp / (float) GameData.DPVPData [currentLv].HighScore;
		}
    }

	public void initLeaderBoard(ref TTeamRank[] data, ref TItemRankGroup[] rankObjects, GameObject anchorObj)
	{
		if (rankObjects == null) {
			Array.Resize (ref rankObjects, data.Length);
			for (int i = 0; i < data.Length; i++) {
				GameObject item = Instantiate(itemPVPRankGroup) as GameObject;
				rankObjects [i] = new TItemRankGroup ();
				rankObjects [i].Init (ref item);
				rankObjects [i].SetParent (anchorObj);
				rankObjects [i].Enable = true;
				rankObjects [i].UpdateView (data [i]);
				rankObjects [i].LocalPosititon = new Vector3 (0, 40 -110 * i, 0);
			}
		}
	}

    void FixedUpdate()
    {
		if (currecntPage == 0 && GameData.Team.PVPTicket > 0 && 
			GameData.Team.PVPCD.ToUniversalTime ().CompareTo(DateTime.UtcNow) > 0)
			updateUI();
    }

	private void updateUI()
	{
		spriteTicket.spriteName = GameFunction.SpendKindTexture(0);
		labelTime.text = string.Format(TextConst.S(9728), GameData.Team.PVPTicket);
		labelStart.text = TextConst.S (9721);

		if (GameData.Team.PVPTicket > 0) {
			int sec = (int)GameData.Team.PVPCD.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds;
			if (sec > 0) {
				labelTime.text += " " + string.Format (TextConst.S (9729), TextConst.SecondString (sec));
				labelStart.text = TextConst.S (9738);
				labelStatus.text = GameConst.PVPCD_Price.ToString ();
				labelStatus.color = GameData.CoinEnoughTextColor (GameData.Team.Diamond >= GameConst.PVPCD_Price);
			} else {
				if (GameData.DPVPData.ContainsKey (pvpLv)) {
					labelStatus.color = GameData.CoinEnoughTextColor (GameData.Team.Money >= GameData.DPVPData [pvpLv].SearchCost, 1);
					spriteTicket.spriteName = GameFunction.SpendKindTexture (1);
					if (!string.IsNullOrEmpty (GameData.PVPEnemyMembers [0].Identifier)) 
						labelStatus.text = "0";
                    else 
					    labelStatus.text = GameData.DPVPData [pvpLv].SearchCost.ToString ();
				}
			}
		} else {
			if (shopIndex >= 0 && shopIndex < GameData.DShops.Length) {
				if (GameData.Team.DailyCount.BuyPVPTicketCount < GameData.DShops [shopIndex].Limit.Length) {
					int diamond = GameData.DShops [shopIndex].Limit[GameData.Team.DailyCount.BuyPVPTicketCount];
					labelStart.text = TextConst.S (9730);
					labelStatus.text = diamond.ToString ();
					labelStatus.color = GameData.CoinEnoughTextColor (GameData.Team.Diamond >= diamond);
				} else {
					labelStart.text = TextConst.S (9739);
					labelStatus.text = "";
					spriteTicket.spriteName = "";
				}
			} else {
				labelStatus.text = "";
				spriteTicket.spriteName = "";
			}
		}
	}

    private int GetShopIndex()
    {
        if (GameData.DShops != null)
            for (int i = 0; i < GameData.DShops.Length; i++)
                if (GameData.DShops[i].Kind == 2)
                    return i;

        return -1;
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

	private void DoPage(int index) {
		currecntPage = index;
		for (int i = 0; i < pages.Length; i++)
			pages[i].SetActive(false);

		pages[index].SetActive(true);
		switch (index) {
		case 0:
			buttonGetAward.isEnabled = GameFunction.CanGetPVPReward(ref GameData.Team);
			redPoints[0].SetActive(buttonGetAward.isEnabled);
			OnNowRank();
			break;
		case 1:
			sendPVPRank(pvpLv);
			break;
		case 2:
			sendPVPRank(-1);
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

    public void OnShop() {
        UIShop.Visible = true;
        UIShop.Get.OpenPage(1);
    }

    private void WaitPVPAward(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPReward data = JsonConvertWrapper.DeserializeObject <TPVPReward>(www.text);
            GameData.Team.DailyCount = data.DailyCount;
            UIGetItem.Get.AddExp(2, data.PVPCoin - GameData.Team.PVPCoin);
            UIGetItem.Get.SetTitle(TextConst.S(9707));
            GameData.Team.PVPCoin = data.PVPCoin;
            labelPVPCoin.text = GameData.Team.PVPCoin.ToString();
            buttonGetAward.isEnabled = false;
            redPoints[0].SetActive(false);
        }
    }

	public void OnStart()
	{
		if (GameData.Team.PVPTicket > 0) {
			int sec = (int)GameData.Team.PVPCD.ToUniversalTime ().Subtract (DateTime.UtcNow).TotalSeconds;
			if (sec <= 0) {
				if (!string.IsNullOrEmpty (GameData.PVPEnemyMembers [0].Identifier)) {
					UIShow (false);
					UISelectRole.Get.LoadStage (GameData.DPVPData [GameData.Team.PVPLv].Stage);
				} else
				    askSearchEnemy ();
			} else
				askBuyPVP (-1);
		} else {
			if (GameData.Team.DailyCount.BuyPVPTicketCount < GameData.DShops [shopIndex].Limit.Length)
				askBuyPVP (shopIndex);
			else
				UIHint.Get.ShowHint (TextConst.S(9739), Color.red);
		}
	}

	private void askSearchEnemy() {
		if (GameData.DPVPData.ContainsKey (pvpLv)) 
			CheckMoney(GameData.DPVPData [pvpLv].SearchCost, true,
				string.Format (TextConst.S (9749), GameData.DPVPData [pvpLv].SearchCost), 
				searchEnemy, updateUI);
			
	}

	private void searchEnemy()
	{
		WWWForm form = new WWWForm ();
		form.AddField ("Identifier", GameData.Team.Identifier);	
		form.AddField ("Kind", 1);
		SendHttp.Get.Command (URLConst.PVPGetEnemy, waitPVPGetEnemy, form, true);
        UIMessage.UIShow(false);
	}

	private void askBuyPVP(int kind) {
		if (kind < 0) {
			CheckDiamond (GameConst.PVPCD_Price, true, 
				string.Format(TextConst.S (9750), GameConst.PVPCD_Price), buyCD, updateUI);
		} else {
			int diamond = GameData.DShops [shopIndex].Limit [GameData.Team.DailyCount.BuyPVPTicketCount];
			CheckDiamond (diamond, true, string.Format(TextConst.S (9751), diamond), buyTicket, updateUI);
		}
	}

	private void buyCD() {
		sendBuyPVP (-1);
	}

	private void buyTicket() {
		sendBuyPVP (shopIndex);
	}

	private void sendBuyPVP(int kind)
	{
		WWWForm form = new WWWForm();
		form.AddField("ShopIndex", kind);
		SendHttp.Get.Command(URLConst.PVPBuyTicket, waitPVPBuyPVP, form, true);  
	}

	private void waitPVPBuyPVP(bool ok, WWW www)
	{
		if (ok)
		{
			if (SendHttp.Get.CheckServerMessage (www.text)) {
                TPVPBuyResult data = JsonConvertWrapper.DeserializeObject <TPVPBuyResult> (www.text);
				GameData.Team.Diamond = data.Diamond;
				GameData.Team.PVPTicket = data.PVPTicket;
				GameData.Team.PVPCD = data.PVPCD;
				GameData.Team.DailyCount = data.DailyCount;

				updateUI ();
			}
		}
	}

	private void openRecharge()
	{
		UIRecharge.UIShow(true);
	}
    
    private void waitPVPGetEnemy(bool ok, WWW www)
    {
        if (ok)
        {
            TPVPEnemyTeams data = JsonConvertWrapper.DeserializeObject <TPVPEnemyTeams>(www.text);
            GameData.Team.Money = data.Money;
            GameData.Team.PVPEnemyIntegral = data.PVPEnemyIntegral;
			pvpLv = GameData.Team.PVPLv;
   
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
			UISelectRole.Get.LoadStage(GameData.DPVPData[pvpLv].Stage);
        } else
            UIHint.Get.ShowHint(TextConst.S(255), Color.red);
    }

	private void sendMyRank()
	{
		if (GameData.Team.PVPIntegral > 0) {
			WWWForm form = new WWWForm ();
			form.AddField ("Language", GameData.Setting.Language.GetHashCode ());
			form.AddField ("PVPIntegral", GameData.Team.PVPIntegral);
			SendHttp.Get.Command (URLConst.PVPMyRank, waitMyRank, form, false);
		}
	}

	private void waitMyRank(bool ok, WWW www)
	{
		if (ok)
		{
            TTeamRank data = JsonConvertWrapper.DeserializeObject <TTeamRank>(www.text);
			lvRange.LabelMyRank.text = TextConst.S (9742) + data.Index.ToString();
		}
	}

	private void sendPVPRank(int lv)
    {
		if ((lv > -1 && leaderBoardLocalItems == null) || (leaderBoardGlobalItems == null))
        {
            WWWForm form = new WWWForm();
			form.AddField("PVPLv", lv);
            form.AddField("Language", GameData.Setting.Language.GetHashCode());
            SendHttp.Get.Command(URLConst.PVPRank, waitSendPVPRank, form, true);
        }
    }

    public void waitSendPVPRank(bool ok, WWW www)
    {
        if (ok)
        {
            TTeamRank[] data = JsonConvertWrapper.DeserializeObject <TTeamRank[]> (www.text);
			if (currecntPage == 1) {
				if (leaderBoardLocalItems == null) 
					initLeaderBoard (ref data, ref leaderBoardLocalItems, anchorLocalList);
			} else {
				if (leaderBoardGlobalItems == null) 
					initLeaderBoard (ref data, ref leaderBoardGlobalItems, anchorGlobalList);
			}
        }
    }
}