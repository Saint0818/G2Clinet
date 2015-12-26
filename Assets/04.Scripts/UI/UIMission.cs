using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

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
	public UILabel LabelGot;
	public UISprite SpriteAward1;
	public UISprite SpriteAward2;
	public UISprite SpriteColor;
	public UISprite[] SpriteLvs;
	public UISlider SliderExp;
}

public class UIMission : UIBase {
    private static UIMission instance = null;
    private const string UIName = "UIMission";

	private int totalScore;
	private int missionScore;
	private int missionLine;
    private const int pageNum = 4;
    private GameObject itemMission;
    private UIScrollView[] pageScrollViews = new UIScrollView[pageNum];

    private UILabel totalLabel;
    private GameObject[] redPoints = new GameObject[pageNum];
    private GameObject[] pageObjects = new GameObject[pageNum];
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
		if (isShow)
			initMissionList(0);
		
		base.OnShow(isShow);
    }

    public void OnPage() {
        for (int i = 0; i < pageObjects.Length; i++)
            pageObjects[i].SetActive(false);

        int index = -1;
        if (int.TryParse(UIButton.current.name, out index))
            pageObjects[index].SetActive(true);
    }

    public void OnClose() {
		Visible = false;
        UIMainLobby.Get.Show();
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

			for (int i = 0; i < missionList[page].Count; i++) 
				checkMission(missionList[page][i]);
		}

		pageObjects[page].SetActive(true);
		totalLabel.text = missionScore.ToString() + "/" + totalScore.ToString();
	}

	private void addMission(int index, int page, TMission data) {
		TMissionItem mi = new TMissionItem();
		mi.Item = Instantiate(itemMission, Vector3.zero, Quaternion.identity) as GameObject;
		initDefaultText(mi.Item);
		string name = index.ToString();
		mi.Item.name = name;
		mi.Item.GetComponent<UIDragScrollView>().scrollView = pageScrollViews[page];	
		mi.UIFinished = GameObject.Find(name + "/Window/CompletedLabel");
		//mi.UIExp = GameObject.Find(name + "/Window/CompletedLabel");
		mi.UIGetAwardBtn = GameObject.Find(name + "/Window/GetBtn").GetComponent<UIButton>();
		SetBtnFun(ref mi.UIGetAwardBtn, OnGetAward);
		mi.SliderExp = GameObject.Find(name + "/Window/EXPView/ProgressBar").GetComponent<UISlider>();
		mi.LabelName = GameObject.Find(name + "/Window/TitleLabel").GetComponent<UILabel>();
		mi.LabelExplain = GameObject.Find(name + "/Window/ContentLabel").GetComponent<UILabel>();
		mi.LabelGot = GameObject.Find(name + "/Window/GetBtn/BtnLabel").GetComponent<UILabel>();
		mi.LabelAward1 = GameObject.Find(name + "/Window/AwardGroup/Award0").GetComponent<UILabel>();
		mi.LabelAward2 = GameObject.Find(name + "/Window/AwardGroup/Award1").GetComponent<UILabel>();
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

		if (!GameData.DMissionData.ContainsKey(missionItem.Mission.PrivousID) || GameData.Team.HaveAchievement(missionItem.Mission.PrivousID)) {
			missionItem.Item.SetActive(true);
			missionItem.LabelName.text = missionItem.Mission.Name;
			missionItem.LabelExplain.text = missionItem.Mission.Explain;
			if (missionItem.Mission.Final > 0) {
				if (GameData.Team.MissionLv(missionItem.Mission) >= missionItem.Mission.Value.Length) {
					missionItem.UIFinished.SetActive(true);
					missionItem.UIGetAwardBtn.gameObject.SetActive(false);
				} else
					missionItem.UIFinished.SetActive(false);
			}else
				missionItem.UIFinished.SetActive(false);

			if (GameData.DItemData.ContainsKey(missionItem.Mission.AwardID[0])) {
				missionItem.AwardGroup.Show(GameData.DItemData[missionItem.Mission.AwardID[0]]);
				//missionItem.UIExp.SetActive(false);
			} else
			if (missionItem.Mission.Exp[0] == 0) {
				//missionItem.UIExp.SetActive(false);
			}
			
			if (missionItem.Mission.Diamond[0] > 0) {
				missionItem.LabelAward1.text = missionItem.Mission.Diamond[0].ToString();
				missionItem.SpriteAward1.spriteName = "Icon_Gem";
			} else
			if (missionItem.Mission.Money[0] > 0) {
					missionItem.LabelAward1.text = missionItem.Mission.Money[0].ToString();
					missionItem.SpriteAward1.spriteName = "Icon_Coin";
			}

			if (missionItem.Mission.Exp[0] > 0) {
				missionItem.LabelAward2.text = missionItem.Mission.Exp[0].ToString();
				missionItem.SpriteAward2.spriteName = "Icon_Exp";
			} else {
				missionItem.LabelAward2.text = "";
				missionItem.SpriteAward2.spriteName = "";
			}

			missionItem.SpriteColor.spriteName = "MissionLv" + missionItem.Mission.Color.ToString();
			for (int i = 0; i < missionItem.SpriteLvs.Length; i++)
				missionItem.SpriteLvs[i].spriteName = "MissionBall" + missionItem.Mission.Color.ToString();
		} else
			missionItem.Item.SetActive(false);
	}

	public void OnGetAward() {
		
	}
}
