using System.Collections;
using System.Collections.Generic;
using GameStruct;
using GameEnum;
using Newtonsoft.Json;
using UnityEngine;

public struct TMissionFinishResult {
    public int MissionID;
    public int Money;
    public int Diamond;
    public int PVPCoin;
    public int SocialCoin;
    public int Exp;
    public int Lv;
    public int ItemID;
    public int ItemNum;
    public TTeamRecord LifetimeRecord;
    public TItem[] Items;
    public TSkill[] SkillCards;
    public Dictionary<int, int> GotItemCount; //key: item id, value: got number
    public Dictionary<int, int> MissionLv; //key: mission id, value: lv
}

public class TMissionItem{
	public int Index;
	public TMission Mission;
	public GameObject Item;
	public GameObject UIFinished;
	public GameObject UIExp;
    public GameObject FXGetAward;
	public UIButton UIGetAwardBtn;

	public ItemAwardGroup AwardGroup;
	public UILabel LabelName;
	public UILabel LabelExplain;
	public UILabel LabelAwardDiamond;
	public UILabel LabelAwardExp;
    public UILabel LabelAwardMoney;
    public UILabel LabelExp;
    public UILabel LabelGot;
    public UILabel LabelScore;
    public UIButton ButtonGot;
    public UISlider SliderExp;
	public UISprite SpriteAwardDiamond;
	public UISprite SpriteColor;
	public UISprite[] SpriteLvs;
    public Animator[] AniLvs;
    public Animator AniFinish;
}

public class UIMission : UIBase {
    private static UIMission instance = null;
    private const string UIName = "UIMission";
    private const int pageNum = 4;
    private int nowPage = 1;
	private int totalScore;
	private int missionScore;
	private int missionLine;
    private int missionExp;
    private int finishID = -1;
    private int finishLv = -1;
    private bool waitForAnimator = false;

    private UILabel totalLabel;
    private UILabel labelStats;
    private GameObject itemMission;
    private GameObject[] redPoints = new GameObject[pageNum];
    private GameObject[] pageObjects = new GameObject[pageNum];
    private UIScrollView[] pageScrollViews = new UIScrollView[pageNum];
	private List<TMissionItem>[] missionList = new List<TMissionItem>[pageNum];

    public static bool Visible {
        get {
            if(instance)
                return instance.gameObject.activeInHierarchy;
            else
                return false;
        }

        set {
            if (instance) {
                if (value)
                    instance.Show(value);
                else
                    RemoveUI(UIName);
            } else
            if (value)
                Get.Show(value);
        }
    }

    public static UIMission Get
    {
        get {
            if (!instance) 
                instance = LoadUI(UIName) as UIMission;

            return instance;
        }
    }

    protected override void InitCom() {
        itemMission = Resources.Load("Prefab/UI/Items/ItemMission") as GameObject;
        totalLabel = GameObject.Find(UIName + "/Window/Center/Total").GetComponent<UILabel>();
        labelStats = GameObject.Find(UIName + "/Window/Center/Pages/4/ScrollView/Label").GetComponent<UILabel>();
        for (int i = 0; i < pageNum; i++) {
            redPoints[i] = GameObject.Find(UIName + "/Window/Center/Tabs/" + i.ToString() + "/RedPoint");
            pageObjects[i] = GameObject.Find(UIName + "/Window/Center/Pages/" + i.ToString());
            pageScrollViews[i] = GameObject.Find(UIName + "/Window/Center/Pages/" + i.ToString() + "/ScrollView").GetComponent<UIScrollView>();
            SetBtnFun(UIName + "/Window/Center/Tabs/" + i.ToString(), OnPage);

			redPoints[i].SetActive(false);
			pageObjects[i].SetActive(false);
        }

        SetBtnFun(UIName + "/Window/BottomLeft/BackBtn", OnClose);
    }

    protected override void InitData() {

    }

    protected override void OnShow(bool isShow) {
        if (isShow) {
            for (int i = 0; i < pageObjects.Length; i++)
                pageObjects[i].SetActive(false);
                
            initMissionList(nowPage);
            for (int i = 0; i < 4; i++)
                if (!redPoints[i].activeInHierarchy)
                    for (int j = 0; j < GameData.MissionData.Length; j++)
                        if (GameData.MissionData[j].TimeKind == i && GameData.Team.HaveMissionAward(ref GameData.MissionData[j])) {
                            redPoints[i].SetActive(true);
                            break;
                        }
        }

		base.OnShow(isShow);
    }

    public void OnPage() {
        if (waitForAnimator)
            return;

        for (int i = 0; i < pageObjects.Length; i++)
            pageObjects[i].SetActive(false);

        int index = -1;
        if (int.TryParse(UIButton.current.name, out index)) {
            pageObjects[index].SetActive(true);
            nowPage = index;

            if (index == 4)
                initStats();
            else
                initMissionList(index);
        }
    }

    public void OnClose() {
		Visible = false;
        UIMainLobby.Get.Show();
    }

    private void initStats() {
        labelStats.text = GameData.Team.StatsText;
    }

	private void initMissionList(int page) {
        if (page >= 0 && page < missionList.Length) {
			totalScore = 0;
			missionScore = 0;
			missionLine = 0;
			if (missionList[page] == null) {
				missionList[page] = new List<TMissionItem>();

				for (int i = 0; i < GameData.MissionData.Length; i++)
					if (GameData.MissionData[i].TimeKind == page)
						addMission(i, page, GameData.MissionData[i]);
			}

            redPoints[page].SetActive(false);
            if (page == 4)
                initStats();
            else {
                missionLine = 0;
                for (int i = 0; i < missionList[page].Count; i++) {
                    checkMission(missionList[page][i], missionList[page][i].Mission, true);
                }
            }
		}

		pageObjects[page].SetActive(true);
		totalLabel.text = missionScore.ToString() + "/" + totalScore.ToString();
	}

	private void addMission(int index, int page, TMission data) {
		TMissionItem mi = new TMissionItem();
		mi.Item = Instantiate(itemMission, Vector3.zero, Quaternion.identity) as GameObject;
		initDefaultText(mi.Item);
        string name = data.ID.ToString();
		mi.Item.name = name;
		mi.UIFinished = GameObject.Find(name + "/Window/CompletedLabel");
        mi.UIExp = GameObject.Find(name + "/Window/AwardExp");
        mi.FXGetAward = GameObject.Find(name + "/Window/GetBtn/FXGet");
		mi.SliderExp = GameObject.Find(name + "/Window/EXPView/ProgressBar").GetComponent<UISlider>();
		mi.LabelName = GameObject.Find(name + "/Window/TitleLabel").GetComponent<UILabel>();
		mi.LabelExplain = GameObject.Find(name + "/Window/ContentLabel").GetComponent<UILabel>();
        mi.LabelExp = GameObject.Find(name + "/Window/EXPView/ExpLabel").GetComponent<UILabel>();
		mi.LabelAwardDiamond = GameObject.Find(name + "/Window/AwardGroup/AwardDiamond").GetComponent<UILabel>();
		mi.LabelAwardExp = GameObject.Find(name + "/Window/AwardGroup/AwardExp").GetComponent<UILabel>();
        mi.LabelAwardMoney = GameObject.Find(name + "/Window/AwardGroup/AwardMoney").GetComponent<UILabel>();
        mi.LabelGot = GameObject.Find(name + "/Window/GetBtn/BtnLabel").GetComponent<UILabel>();
        mi.LabelScore = GameObject.Find(name + "/Window/AwardScore").GetComponent<UILabel>();
        mi.ButtonGot = GameObject.Find(name + "/Window/GetBtn").GetComponent<UIButton>();
		mi.SpriteAwardDiamond = GameObject.Find(name + "/Window/AwardGroup/AwardDiamond/Icon").GetComponent<UISprite>();
		mi.SpriteColor = GameObject.Find(name + "/Window/ObjectLevel").GetComponent<UISprite>();
        mi.AniFinish = GameObject.Find(name).GetComponent<Animator>();
		mi.SpriteLvs = new UISprite[5];
        mi.AniLvs = new Animator[5];
        for (int i = 0; i < mi.SpriteLvs.Length; i++) {
            mi.AniLvs[i] = GameObject.Find(name + "/Window/AchievementTarget/Target" + i.ToString()).GetComponent<Animator>();
			mi.SpriteLvs[i] = GameObject.Find(name + "/Window/AchievementTarget/Target" + i.ToString() + "/Get").GetComponent<UISprite>();
        }

		GameObject obj = GameObject.Find(name + "/Window/ItemAwardGroup");
		if (obj)
			mi.AwardGroup = obj.GetComponent<ItemAwardGroup>();

        mi.UIGetAwardBtn = GameObject.Find(name + "/Window/GetBtn").GetComponent<UIButton>();
        mi.UIGetAwardBtn.name = name;
        SetBtnFun(ref mi.UIGetAwardBtn, OnGetAward);

		mi.Index = missionList[page].Count;
		mi.Mission = data;
		mi.Item.transform.parent = pageScrollViews[page].gameObject.transform;
		if (GameData.DMissionData.ContainsKey(mi.Mission.PrivousID)) {
			for (int i = 0; i < missionList[page].Count; i++)
				if (missionList[page][i].Mission.ID == mi.Mission.PrivousID) {
					mi.Item.transform.localPosition = missionList[page][i].Item.transform.localPosition;
					break;
				}
		} else {
			mi.Item.transform.localPosition = new Vector3(0, 170 - missionLine * 160, 0);
			missionLine++;
		}

		mi.Item.transform.localScale = Vector3.one;
		missionList[page].Add(mi);
	}

    private void checkMission(TMissionItem missionItem, TMission missionData, bool resetPosition) {
		try {
            if (missionData.Lv == 0 || GameData.Team.Player.Lv >= missionData.Lv) {
                if (!GameData.DMissionData.ContainsKey(missionData.PrivousID) || 
                    GameData.Team.FindMissionLv(missionData.PrivousID, missionData.TimeKind) >= 
                    GameData.DMissionData[missionData.PrivousID].Value.Length) {
                    totalScore += missionData.Score;
                    int mLv = Mathf.Min(GameData.Team.FindMissionLv(missionData.ID, missionData.TimeKind), missionData.Value.Length);
                    if (mLv >= missionData.Value.Length)
                        missionScore += missionData.Score;
                    
                    if (missionData.Final >= 1 || mLv < missionData.Value.Length) {
            			missionItem.Item.SetActive(true);
                        missionItem.LabelName.text = missionData.Name;
                        missionItem.LabelName.color = TextConst.Color(missionData.Color);
                        missionItem.LabelExplain.text = missionData.Explain;
                        if (missionData.Score > 0)
                            missionItem.LabelScore.text = TextConst.S(3718) + " " + missionData.Score.ToString();
                        else
                            missionItem.LabelScore.text = "";
                        
                        if (missionData.Final > 0) {
                            if (mLv >= missionData.Value.Length) {
            					missionItem.UIFinished.SetActive(true);
            					missionItem.UIGetAwardBtn.gameObject.SetActive(false);
            				} else
            					missionItem.UIFinished.SetActive(false);
            			} else
            				missionItem.UIFinished.SetActive(false);

                        int lv = Mathf.Min(mLv, missionData.Value.Length-1);
                        if (missionData.AwardID != null) {
                            missionItem.UIExp.SetActive(false);
                            if (GameData.DItemData.ContainsKey(missionData.AwardID[lv]))
                                missionItem.AwardGroup.Show(GameData.DItemData[missionData.AwardID[lv]]);
                            else
                            if (missionData.Exp[lv] > 0)
                                missionItem.UIExp.SetActive(true);
                        } else {
                            if (missionData.Exp[lv] > 0)
                                missionItem.UIExp.SetActive(true);
                        }

                        if (missionData.Diamond[lv] > 0) {
                            missionItem.LabelAwardDiamond.text = missionData.Diamond[lv].ToString();
                            missionItem.SpriteAwardDiamond.spriteName = GameFunction.SpendKindTexture(missionData.SpendKind);
                        }

                        missionItem.LabelAwardMoney.text = NumFormater.Convert(missionData.Money[lv]);
                        missionItem.LabelAwardExp.text = missionData.Exp[lv].ToString();
                        int mValue = GameData.Team.GetMissionValue(missionData.Kind, missionData.TimeKind, missionData.TimeValue);
                        if (mValue >= missionData.Value[lv]) {
                            missionItem.FXGetAward.SetActive(true);
                            missionItem.LabelGot.text = TextConst.S(3706);
                            missionItem.ButtonGot.normalSprite = "button_orange1";
                            if (!redPoints[nowPage].activeInHierarchy &&mLv == lv)
                                redPoints[nowPage].SetActive(true);
                        } else {
                            missionItem.FXGetAward.SetActive(false);
                            missionItem.LabelGot.text = TextConst.S(3705);
                            missionItem.ButtonGot.normalSprite = "button_blue2";
                        }

                        if (missionData.Value[lv] > 0) {
                            missionItem.LabelExp.text = mValue + " / " + missionData.Value[lv];
                            float r =  (float)mValue / (float)missionData.Value[lv];
                            missionItem.SliderExp.value = r;
                            if (r > 1) {
                                r = 1;
                                missionItem.LabelExp.color = Color.green;
                            } else
                                missionItem.LabelExp.color = Color.white;
                        } else
                            missionItem.LabelExp.text = mValue.ToString();

                        missionItem.SpriteColor.spriteName = "MissionLv" + missionData.Color.ToString();
                        for (int i = 0; i < missionItem.SpriteLvs.Length; i++) {
                            if (i < missionData.Value.Length) {
                                if (missionItem.SpriteLvs[i].transform.parent)
                                    missionItem.SpriteLvs[i].transform.parent.gameObject.SetActive(true);

                                if (mLv > i) {
                                    missionItem.SpriteLvs[i].gameObject.SetActive(true);
                                    missionItem.SpriteLvs[i].spriteName = "MissionBall" + missionData.Color.ToString();
                                } else
                                    missionItem.SpriteLvs[i].gameObject.SetActive(false);
                            } else
                            if (missionItem.SpriteLvs[i].transform.parent)
                                missionItem.SpriteLvs[i].transform.parent.gameObject.SetActive(false);
                        }

                        if (resetPosition) {
                            missionItem.Item.transform.localPosition = new Vector3(0, 170 - missionLine * 160, 0);
                            missionLine++;
                        }
                    } else 
                        missionItem.Item.SetActive(false);
                } else
                    missionItem.Item.SetActive(false);
            } else
                missionItem.Item.SetActive(false);
        } catch (System.Exception e) {
            Debug.Log(missionData.ID.ToString());
        }
	}

    private float checkAnimator(int id, int lv) {
        for (int i = 0; i < missionList[nowPage].Count; i++) {
            TMissionItem item = missionList[nowPage][i];
            if (item.Mission.ID == id && lv < item.AniLvs.Length) {
                item.SpriteLvs[lv].gameObject.SetActive(true);
                item.SpriteLvs[lv].spriteName = "MissionBall" + item.Mission.Color.ToString();
                item.AniLvs[lv].SetTrigger("Target");
                if (item.Mission.Final == 0 && lv == item.Mission.Value.Length-1) {
                    TMission mission = item.Mission;
                    if (i+1 < missionList[nowPage].Count)
                        mission = missionList[nowPage][i+1].Mission;

                    StartCoroutine(waitFinish(item, mission));
                    return 1.5f;
                }

                return 0.8f;
            }
        }

        return 0;
    }

    IEnumerator waitFinish(TMissionItem item, TMission mission) {
        yield return new WaitForSeconds(0.5f);

        item.AniFinish.SetTrigger("Next");
        StartCoroutine(waitNextMission(item, mission));
    }

    IEnumerator waitNextMission(TMissionItem item, TMission mission) {
        yield return new WaitForSeconds(0.5f);

        mission.PrivousID = 0;
        checkMission(item, mission, false);
    }

    IEnumerator waitUpdateMission(TMissionFinishResult result, float sec) {
        yield return new WaitForSeconds(sec);
        waitForAnimator = false;
        GameData.Team.TeamRecord = result.LifetimeRecord;

        bool flag = false;
        TPlayer player = GameData.Team.Player;
        if (result.Lv > GameData.Team.Player.Lv)
            flag = true;

        GameData.Team.Player.Lv = result.Lv;
        if (flag) {
            UILevelUp.Get.Show(player, GameData.Team.Player);
            if (GameData.DExpData.ContainsKey(result.Lv) && LimitTable.Ins.HasOpenIDByLv(result.Lv))
                PlayerPrefs.SetInt (ESave.LevelUpFlag.ToString(), GameData.DExpData[result.Lv].UI);
        }

        if (GameData.DMissionData.ContainsKey(result.MissionID)) {
            switch (GameData.DMissionData[result.MissionID].TimeKind) {
                case 0:
                    GameData.Team.MissionLv = result.MissionLv;
                    break;
                case 1:
                    GameData.Team.DailyRecord.MissionLv = result.MissionLv;
                    break;
                case 2:
                    GameData.Team.WeeklyRecord.MissionLv = result.MissionLv;
                    break;
                case 3:
                    GameData.Team.MonthlyRecord.MissionLv = result.MissionLv;
                    break;
            }
        }

        if (result.SkillCards != null) {
            GameData.Team.SkillCards = result.SkillCards;
            GameData.Team.InitSkillCardCount();
        }

        if (result.Items != null) 
            GameData.Team.Items = result.Items;

        if (result.GotItemCount != null)
            GameData.Team.GotItemCount = result.GotItemCount;

        if (GameData.DItemData.ContainsKey(result.ItemID))
            UIGetItem.Get.AddItem(result.ItemID);

        int diamond = 0;
        if (GameData.Team.Diamond < result.Diamond) {
            diamond = result.Diamond - GameData.Team.Diamond;
            GameData.Team.Diamond = result.Diamond;
            UIGetItem.Get.AddExp(0, diamond);
        }

        int money = 0;
        if (GameData.Team.Money < result.Money) {
            money = result.Money - GameData.Team.Money;
            GameData.Team.Money = result.Money;
            UIGetItem.Get.AddExp(1, money);
        }

        int pvpCoin = 0;
        if (GameData.Team.PVPCoin < result.PVPCoin) {
            pvpCoin = result.PVPCoin - GameData.Team.PVPCoin;
            GameData.Team.PVPCoin = result.PVPCoin;
            UIGetItem.Get.AddExp(2, pvpCoin);
        }

        int socialCoin = 0;
        if (GameData.Team.SocialCoin < result.SocialCoin) {
            socialCoin = result.SocialCoin - GameData.Team.SocialCoin;
            GameData.Team.SocialCoin = result.SocialCoin;
            UIGetItem.Get.AddExp(3, socialCoin);
        }

        GameData.Team.Player.Exp = result.Exp;
        if (missionExp > 0)
            UIGetItem.Get.AddExp(4, missionExp);

        initMissionList(nowPage);
    }

    private void waitMissionFinish(bool ok, WWW www) {
        if (ok) {
            waitForAnimator = true;
            TMissionFinishResult result = JsonConvert.DeserializeObject <TMissionFinishResult>(www.text, SendHttp.Get.JsonSetting);
            float sec = checkAnimator(finishID, finishLv);
            StartCoroutine(waitUpdateMission(result, sec));
        } else
            UIHint.Get.ShowHint(TextConst.S(3715), Color.red);
    }

	public void OnGetAward() {
        if (waitForAnimator)
            return;

        finishID = -1;
        finishLv = -1;
        missionExp = 0;
        if (int.TryParse(UIButton.current.transform.name, out finishID)) {
            if (GameData.DMissionData.ContainsKey(finishID)) {
                TMission mission = GameData.DMissionData[finishID];
                finishLv = GameData.Team.FindMissionLv(mission.ID, mission.TimeKind);
                if (finishLv < mission.Value.Length) {
                    int mValue = GameData.Team.GetMissionValue(mission.Kind, mission.TimeKind, mission.TimeValue);
                    if (mValue >= mission.Value[finishLv]) {
                        missionExp = mission.Exp[finishLv];
                        WWWForm form = new WWWForm();
                        form.AddField("MissionID", mission.ID);
                        SendHttp.Get.Command(URLConst.MissionFinish, waitMissionFinish, form, true);
                    } else
                    if (mission.OpenUI != "") {
                        Visible = false;
                        UI2D.Get.OpenUI(mission.OpenUI);
                    }
                } else
                    UIHint.Get.ShowHint(TextConst.S(3714), Color.red);
            } else
                Debug.Log("No mission id " + finishID.ToString());
        }
	}
}
