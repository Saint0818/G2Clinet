using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameStruct;

public struct TMissionFinishResult {
    public int MissionID;
    public int Money;
    public int Diamond;
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
	public UIButton UIGetAwardBtn;
	public ItemAwardGroup AwardGroup;
	public UILabel LabelName;
	public UILabel LabelExplain;
	public UILabel LabelAward1;
	public UILabel LabelAward2;
    public UILabel LabelExp;
    public UILabel LabelGot;
    public UISprite SpriteGot;
	public UISprite SpriteAward1;
	public UISprite SpriteAward2;
	public UISprite SpriteColor;
	public UISprite[] SpriteLvs;
	public UISlider SliderExp;
}

public class UIMission : UIBase {
    private static UIMission instance = null;
    private const string UIName = "UIMission";

    private int nowPage = 0;
	private int totalScore;
	private int missionScore;
	private int missionLine;
    private const int pageNum = 5;

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
            if (instance)
                instance.Show(value);
            else
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
			    for (int i = 0; i < missionList[page].Count; i++) 
				    checkMission(missionList[page][i]);
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
		mi.Item.GetComponent<UIDragScrollView>().scrollView = pageScrollViews[page];	
		mi.UIFinished = GameObject.Find(name + "/Window/CompletedLabel");
        mi.UIExp = GameObject.Find(name + "/Window/AwardExp");
		mi.UIGetAwardBtn = GameObject.Find(name + "/Window/GetBtn").GetComponent<UIButton>();
		SetBtnFun(ref mi.UIGetAwardBtn, OnGetAward);
		mi.SliderExp = GameObject.Find(name + "/Window/EXPView/ProgressBar").GetComponent<UISlider>();
		mi.LabelName = GameObject.Find(name + "/Window/TitleLabel").GetComponent<UILabel>();
		mi.LabelExplain = GameObject.Find(name + "/Window/ContentLabel").GetComponent<UILabel>();
        mi.LabelExp = GameObject.Find(name + "/Window/EXPView/ExpLabel").GetComponent<UILabel>();
		mi.LabelAward1 = GameObject.Find(name + "/Window/AwardGroup/Award0").GetComponent<UILabel>();
		mi.LabelAward2 = GameObject.Find(name + "/Window/AwardGroup/Award1").GetComponent<UILabel>();
        mi.LabelGot = GameObject.Find(name + "/Window/GetBtn/BtnLabel").GetComponent<UILabel>();
        mi.SpriteGot = GameObject.Find(name + "/Window/GetBtn").GetComponent<UISprite>();
		mi.SpriteAward1 = GameObject.Find(name + "/Window/AwardGroup/Award0/Icon").GetComponent<UISprite>();
		mi.SpriteAward2 = GameObject.Find(name + "/Window/AwardGroup/Award1/Icon").GetComponent<UISprite>();
		mi.SpriteColor = GameObject.Find(name + "/Window/ObjectLevel").GetComponent<UISprite>();
		mi.SpriteLvs = new UISprite[5];
		for (int i = 0; i < mi.SpriteLvs.Length; i++) 
			mi.SpriteLvs[i] = GameObject.Find(name + "/Window/AchievementTarget/Target" + i.ToString() + "/Get").GetComponent<UISprite>();
		
		GameObject obj = GameObject.Find(name + "/Window/ItemAwardGroup");
		if (obj)
			mi.AwardGroup = obj.GetComponent<ItemAwardGroup>();

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

	private void checkMission(TMissionItem missionItem) {
		totalScore += missionItem.Mission.Score;

        if (!GameData.DMissionData.ContainsKey(missionItem.Mission.PrivousID) || 
            GameData.Team.FindMissionLv(missionItem.Mission.PrivousID, missionItem.Mission.TimeKind) >= 
            GameData.DMissionData[missionItem.Mission.PrivousID].Value.Length) {
            int mLv = Mathf.Min(GameData.Team.FindMissionLv(missionItem.Mission.ID, missionItem.Mission.TimeKind), missionItem.Mission.Value.Length);
            if (mLv >= missionItem.Mission.Value.Length)
                missionScore += missionItem.Mission.Score;
            
            if (missionItem.Mission.Final >= 1 || mLv < missionItem.Mission.Value.Length) {
    			missionItem.Item.SetActive(true);
    			missionItem.LabelName.text = missionItem.Mission.Name;
    			missionItem.LabelExplain.text = missionItem.Mission.Explain;

    			if (missionItem.Mission.Final > 0) {
                    if (mLv >= missionItem.Mission.Value.Length) {
    					missionItem.UIFinished.SetActive(true);
    					missionItem.UIGetAwardBtn.gameObject.SetActive(false);
    				} else
    					missionItem.UIFinished.SetActive(false);
    			} else
    				missionItem.UIFinished.SetActive(false);

                int lv = Mathf.Min(mLv, missionItem.Mission.AwardID.Length-1);
                missionItem.UIExp.SetActive(false);
                if (GameData.DItemData.ContainsKey(missionItem.Mission.AwardID[lv]))
                    missionItem.AwardGroup.Show(GameData.DItemData[missionItem.Mission.AwardID[lv]]);
                else
                if (missionItem.Mission.Exp[lv] > 0)
                    missionItem.UIExp.SetActive(true);
    			
                if (missionItem.Mission.Diamond[lv] > 0) {
                    missionItem.LabelAward1.text = missionItem.Mission.Diamond[lv].ToString();
                    missionItem.SpriteAward1.spriteName = "Icon_Gem";
                } else
                if (missionItem.Mission.Money[mLv] > 0) {
                    missionItem.LabelAward1.text = missionItem.Mission.Money[lv].ToString();
                    missionItem.SpriteAward1.spriteName = "Icon_Coin";
                }

                missionItem.LabelAward2.text = missionItem.Mission.Exp[lv].ToString();
                int mValue = GameData.Team.GetMissionValue(missionItem.Mission.Kind, missionItem.Mission.TimeKind);
                if (mValue >= missionItem.Mission.Value[lv]) {
                    missionItem.LabelGot.text = TextConst.S(3706);
                    missionItem.SpriteGot.spriteName = "button_orange1";
                    if (!redPoints[nowPage].activeInHierarchy &&mLv == lv)
                        redPoints[nowPage].SetActive(true);
                } else {
                    missionItem.LabelGot.text = TextConst.S(3705);
                    missionItem.SpriteGot.spriteName = "button_blue2";
                }

                if (missionItem.Mission.Value[lv] > 0) {
                    missionItem.LabelExp.text = mValue + " / " + missionItem.Mission.Value[lv];
                    float r =  (float)mValue / (float)missionItem.Mission.Value[lv];
                    missionItem.SliderExp.value = r;
                    if (r > 1) {
                        r = 1;
                        missionItem.LabelExp.color = Color.green;
                    } else
                        missionItem.LabelExp.color = Color.white;
                } else
                    missionItem.LabelExp.text = mValue.ToString();

    			missionItem.SpriteColor.spriteName = "MissionLv" + missionItem.Mission.Color.ToString();
                for (int i = 0; i < missionItem.SpriteLvs.Length; i++) {
                    if (i < missionItem.Mission.Value.Length) {
                        if (mLv > i) {
                            missionItem.SpriteLvs[i].gameObject.SetActive(true);
        				    missionItem.SpriteLvs[i].spriteName = "MissionBall" + missionItem.Mission.Color.ToString();
                        } else
                            missionItem.SpriteLvs[i].gameObject.SetActive(false);
                    } else
                    if (missionItem.SpriteLvs[i].transform.parent)
                       missionItem.SpriteLvs[i].transform.parent.gameObject.SetActive(false);
                }

                missionItem.Item.transform.localPosition = new Vector3(0, 170 - missionLine * 160, 0);
                missionLine++;
            } else
                missionItem.Item.SetActive(false);
		} else
			missionItem.Item.SetActive(false);
	}

    private void waitMissionFinish(bool ok, WWW www) {
        if (ok) {
            TMissionFinishResult result = JsonConvert.DeserializeObject <TMissionFinishResult>(www.text, SendHttp.Get.JsonSetting);
            GameData.Team.TeamRecord = result.LifetimeRecord;
            GameData.Team.Player.Exp = result.Exp;
            GameData.Team.Player.Lv = result.Lv;

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

            if (GameData.Team.Money < result.Money)
                GameData.Team.Money = result.Money;

            if (GameData.Team.Diamond < result.Diamond)
                GameData.Team.Diamond = result.Diamond;

            if (result.SkillCards != null)
                GameData.Team.SkillCards = result.SkillCards;

            if (result.Items != null) 
                GameData.Team.Items = result.Items;
            
            if (result.GotItemCount != null) {
                GameData.Team.GotItemCount = result.GotItemCount;
                if (GameData.DItemData.ContainsKey(result.ItemID)) {
                    UIItemHint.Get.OnShow(GameData.DItemData[result.ItemID]);
                    UIHint.Get.ShowHint(string.Format(TextConst.S(3717), GameData.DItemData[result.ItemID].Name, result.ItemNum), Color.white);
                }
            }

            initMissionList(nowPage);
        } else
            UIHint.Get.ShowHint(TextConst.S(3715), Color.red);
    }

	public void OnGetAward() {
        int index = -1;
        if (UIButton.current.transform.parent != null && UIButton.current.transform.parent.parent != null &&
            int.TryParse(UIButton.current.transform.parent.parent.name, out index) && GameData.DMissionData.ContainsKey(index)) {
            TMission mission = GameData.DMissionData[index];
            int mLv = GameData.Team.FindMissionLv(mission.ID, mission.TimeKind);
            if (mLv < mission.Value.Length) {
                int mValue = GameData.Team.GetMissionValue(mission.Kind, mission.TimeKind);
                if (mValue >= mission.Value[mLv]) {
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
        }
	}
}
