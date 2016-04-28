﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameEnum;
using GameStruct;
using UnityEngine;

public class UILoading : UIBase
{
    private static UILoading instance = null;
    private const string UIName = "UILoading";

    private GameObject windowLoading;
    private GameObject windowStage;
    private GameObject loadingPic;
    private GameObject buttonNext;
    private GameObject uiAbilityDisc;
    private GameObject uiSkillDisk;
    private UITexture uiBG;
    private UITexture uiLoadingProgress;
    private UISprite spriteTip;
    private UILabel labelLoading;
    private UILabel labelStageTitle;
    private UILabel labelStageExplain;
    private UILabel labelTip;

    private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    private UIStageHintTarget[] stageTargets;
    private TActiveSkillCard skillCard = new TActiveSkillCard();
    private TSkill skillData = new TSkill();
    private List<int> skillIDs = new List<int>();

    public static EventDelegate.Callback OpenUI = null;
    private ELoading loadingKind = ELoading.Null;
    private bool closeAfterFinished = false;
    public static int StageID = -1;
    private static int achievementTutorialID = -1;
    private float nowProgress;
    private float toProgress;
    private float startTimer = 0;
    private float loadingTimer = 0;
    private float textCount = 0;
    private string loadingText = "";

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

    public static UILoading Get
    {
        get
        {
            if (!instance)
                instance = LoadUI(UIName) as UILoading;
			
            return instance;
        }
    }

    public static void LvUpUI(int lv)
    {
        if (GameData.DExpData.ContainsKey(lv))
        {
            achievementTutorialID = GameData.DExpData[lv].TutorialID;
            switch (GameData.DExpData[lv].UI)
            {
                case 0:
                    OpenUI = OpenStageUI;
                    break;
                case 1:
                    OpenUI = OpenMainUI;
                    break;
                case 2:
                    OpenUI = OpenGameLobby;
                    break;
                case 3:
                    OpenUI = OpenInstanceUI;
                    break;
                case 4:
                    OpenUI = OpenPVPUI;
                    break;
                default:
                    OpenUI = null;
                    break;
            }
        } else
            OpenUI = OpenStageUI;
    }

    private static bool checkTutorialUI(int id)
    {
        OpenUI = null;
        if (GameData.DTutorial.ContainsKey(id * 100 + 1))
        {
            if (!GameData.Team.HaveTutorialFlag(id)) {
                UITutorial.Get.ShowTutorial(id, 1);
                achievementTutorialID = -1;
                return true;
            }
        }

        return false;
    }

    public static void OpenMainUI()
    {
        UIMainLobby.Get.Show();
        checkTutorialUI(achievementTutorialID);
    }

    public static void OpenGameLobby()
    {
        //UIMainLobby.Get.Hide();
        UIGameLobby.Get.Show();
        checkTutorialUI(achievementTutorialID);
    }

    //Open stage after battle
    public static void OpenStageUI()
    {
        UIMainStage.Get.Show();
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();

        if (!checkTutorialUI(achievementTutorialID))
            if (GameData.DTutorialStageEnd.ContainsKey(StageID) && checkTutorialUI(GameData.DTutorialStageEnd[StageID]))
                StageID = -1;
    }

    public static void OpenInstanceUI()
    {
        UIInstance.Get.ShowByPlayStageID();
        UIMainLobby.Get.Hide();
        UIResource.Get.Show();

        if (!checkTutorialUI(achievementTutorialID))
            if (GameData.DTutorialStageEnd.ContainsKey(StageID) && checkTutorialUI(GameData.DTutorialStageEnd[StageID]))
                StageID = -1;
    }

    public static void OpenPVPUI()
    {
        UIPVP.UIShow(true);

        UIMainLobby.Get.Hide();
        UIResource.Get.Show(2);

        if (!checkTutorialUI(achievementTutorialID))
            if (GameData.DTutorialStageEnd.ContainsKey(StageID) && checkTutorialUI(GameData.DTutorialStageEnd[StageID]))
                StageID = -1;
    }

    public static void OpenNotic()
    {
        if (GameData.Team.Player.Lv > 0)
        {
            int check = PlayerPrefs.GetInt(ESave.NoticDaily.ToString(), 0);
			
            if (check == 0)
                UINotic.Visible = true;
            else
            {
                int day = DateTime.Now.Day;
                int date = PlayerPrefs.GetInt(ESave.NoticDate.ToString(), -1);
                if (day != date)
                    UINotic.Visible = true;
            }

            if(!UITutorial.Visible && 
               (UIDailyLoginHelper.HasTodayDailyLoginReward() || UIDailyLoginHelper.HasLifetimeLoginReward()))
                UIDailyLogin.Get.Show();
        }
    }

    public void OnStageStart() {
        UIShow(false);
        if (GameController.Visible)
            GameController.Get.StageStart();
    }

    public static void UIShow(bool isShow, ELoading kind = ELoading.Null)
    {
        if (isShow)
        {
            Get.initLoadingPic(kind);
            Get.Show(true);

            Statistic.Ins.LogScreen(1);
        }
        else 
        if (instance)
        { 
            if (Get.LoadingFinished)
                Get.Show(false);
            else
                Get.closeAfterFinished = true;
        }
    }

    void FixedUpdate()
    {
        loadingTimer += Time.deltaTime;
        if (loadingTimer >= 0.1f)
        {
            loadingTimer = 0;
            textCount++;
            loadingText = "";
            for (int i = 0; i < textCount; i++)
                loadingText += ".";
			
            labelLoading.text = TextConst.S(10106) + loadingText;
			
            if (textCount > 2)
                textCount = 0;
        }

        if (!LoadingFinished)
        {
            if (nowProgress < toProgress)
            {
                nowProgress += Time.deltaTime;
                if (nowProgress > toProgress)
                    nowProgress = toProgress;

                uiLoadingProgress.fillAmount = nowProgress;
            }

            if (closeAfterFinished)
                UIShow(false);
        }
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);
        if (isShow)
            StartCoroutine(doLoading(loadingKind));
        else
        {
            toProgress = 0;
            nowProgress = 0;
            ProgressValue = 0;
            uiLoadingProgress.fillAmount = 0;
        }
    }

    private void initLoadingPic(ELoading kind)
    {
        loadingKind = kind;
        startTimer = Time.time;
        closeAfterFinished = false;

        if (kind == ELoading.Game)
        {
            windowStage.SetActive(true);
            windowLoading.SetActive(false);
            try
            {
                TStageData data = StageTable.Ins.GetByID(GameData.StageID);
                labelStageTitle.text = data.Name;
                labelStageExplain.text = data.Explain;
                string tipText = "";
                if (data.Tips != null && data.Tips.Length > 0)
                {
                    int index = UnityEngine.Random.Range(0, data.Tips.Length);
                    tipText = TextConst.S(data.Tips[index]);
                }
                else
                    tipText = TextConst.S(UnityEngine.Random.Range(301, 303));
            
                char[] c = {'='};
                string[] s = tipText.Split(c, 2);
                if (s.Length == 2) {
                    spriteTip.spriteName = s[0];
                    labelTip.text = s[1];
                } else 
                    labelTip.text = tipText;

                UIStageHintManager.UpdateHintNormal(GameData.StageID, ref stageTargets);

                if (GameData.SkillRecommends != null && GameData.SkillRecommends.Length > 0) {
                    skillIDs.Clear();
                
                    if (UnityEngine.Random.Range(0, 100) < GameData.SkillRecommends[0].Rate) {
                        for (int j = 0; j < GameData.SkillRecommends[0].IDs.Length; j++)
                            skillIDs.Add(GameData.SkillRecommends[0].IDs[j]);
                    } else {
                        for (int i = 0; i < GameData.SkillRecommends.Length; i++) {
                            if (data.HintBit[2] == i || data.HintBit[3] == i || 
                                (i == 8 && data.HintBit[1] == 2) || (i == 9 && data.HintBit[1] == 3)) {

                                if (GameData.SkillRecommends[i].IDs != null)
                                    for (int j = 0; j < GameData.SkillRecommends[i].IDs.Length; j++)
                                        skillIDs.Add(GameData.SkillRecommends[i].IDs[j]);
                            }
                        }
                    }

                    if (skillIDs.Count > 0)
                    {
                        int index = UnityEngine.Random.Range(0, skillIDs.Count);
                        skillData.ID = skillIDs[index];
                        if (GameData.DSkillData.ContainsKey(skillData.ID))
                        {
                            uiAbilityDisc.SetActive(true);
                            skillData.Lv = GameData.DSkillData[skillData.ID].MaxStar;
                            skillCard.UpdateView(0, skillData);
                        }
                    }
                }
            }
            catch
            {
                
            }
        }
        else
        {
            uiAbilityDisc.SetActive(false);
            windowStage.SetActive(false);
            windowLoading.SetActive(true);
        }
    }

    protected override void InitCom()
    {
        SetBtnFun(UIName + "/StageInfo/Right/Next", OnStageStart);
        uiAbilityDisc = GameObject.Find(UIName + "/ShowAbilityDisc");
        uiSkillDisk = GameObject.Find(UIName + "/ShowAbilityDisc/SkillCard");
        loadingPic = GameObject.Find(UIName + "/LoadingPic");
        uiLoadingProgress = GameObject.Find(UIName + "/LoadingPic/UIProgressBar").GetComponent<UITexture>();
        labelLoading = GameObject.Find(UIName + "/LoadingPic/UIWord").GetComponent<UILabel>();
        windowLoading = GameObject.Find(UIName + "/WindowLoading");
        windowStage = GameObject.Find(UIName + "/StageInfo");
        labelTip = GameObject.Find(UIName + "/StageInfo/Bottom/Tip").GetComponent<UILabel>();
        spriteTip = GameObject.Find(UIName + "/StageInfo/Bottom/Tip/Tip_Icon").GetComponent<UISprite>();
        labelStageTitle = GameObject.Find(UIName + "/StageInfo/Center/SingalStage/StageNameLabel").GetComponent<UILabel>();
        labelStageExplain = GameObject.Find(UIName + "/StageInfo/Center/SingalStage/StageExplainLabel").GetComponent<UILabel>();
        uiBG = GameObject.Find(UIName + "/StageInfo/Center/StageKindTexture").GetComponent<UITexture>();
        buttonNext = GameObject.Find(UIName + "/StageInfo/Right/Next");

        skillCard.Init(uiSkillDisk);
        GameObject obj = GameObject.Find(UIName + "/StageInfo/Center/StageHint");
        if (obj)
            stageTargets = obj.GetComponentsInChildren<UIStageHintTarget>();
        
        uiAbilityDisc.SetActive(false);
        buttonNext.SetActive(false);
        windowStage.SetActive(false);
        windowLoading.SetActive(false);
        loadingPic.SetActive(true);
    }

    IEnumerator doLoading(ELoading kind)
    {
        float minWait = 2;
        float maxWait = 4;
        float waitTime = 1;

        if (kind != ELoading.Login)
            ProgressValue = 0.3f;

        yield return new WaitForEndOfFrame();

        switch (kind) {
            case ELoading.Null:
                ProgressValue = 0.3f;
                yield return new WaitForSeconds(0.5f);

                break;
            case ELoading.Login:
                ProgressValue = 1;
                break;
            case ELoading.CreateRole:
                UICreateRole.Get.ShowPositionView();
                UI3DCreateRole.Get.PositionView.PlayDropAnimation();
                ProgressValue = 0.7f;

                yield return new WaitForSeconds(0.5f);
                UIShow(false);
				AudioMgr.Get.PlayMusic(EMusicType.MU_Create);
                break;
            case ELoading.Lobby:
                ProgressValue = 1;
			
                if (GameData.Team.Player.Lv == 0) {
                    UICreateRole.Get.ShowPositionView();
                    UI3DCreateRole.Get.PositionView.PlayDropAnimation();
                } else 
                if (OpenUI != null) {
                    OpenUI();
                    OpenUI = null;
                } else
                    UIMainLobby.Get.Show();

                if (UITutorial.Visible)
                    uiLoadingProgress.fillAmount = 1;
                
                UIShow(false);
				AudioMgr.Get.PlayMusic(EMusicType.MU_ThemeSong);
                break;
            case ELoading.Game:
                ProgressValue = 0.3f;
                yield return new WaitForSeconds(0.2f);
                ProgressValue = 1;

                TStageData stageData = StageTable.Ins.GetByID(GameData.StageID);
                CourtMgr.Get.InitCourtScene(stageData.CourtNo);

                yield return new WaitForSeconds(1);
                GameController.Get.ChangeSituation(EGameSituation.None);
                GameController.Get.LoadStage(GameData.StageID);

			    buttonNext.SetActive(true);
			    loadingPic.SetActive(false);
				if(GameData.IsPVP)
					AudioMgr.Get.PlayMusic(EMusicType.MU_BattlePVP);
				else
					AudioMgr.Get.PlayMusic(EMusicType.MU_BattleNormal);
                
                break;
            case ELoading.Stage:
                ProgressValue = 1;
                UISelectRole.Get.LoadStage(GameData.StageID);
                UIShow(false);

                break;
        }

        ProgressValue = 1;
    }

    public void UpdateProgress()
    {
        float b = FileManager.DownlandCount;
        float a = FileManager.AlreadyDownlandCount;
        ProgressValue = (float)(a / b);
    }

    private Texture2D loadTexture(string path)
    {
        if (textureCache.ContainsKey(path))
        {
            return textureCache[path];
        }
        else
        {
            Texture2D obj = Resources.Load(path) as Texture2D;
            if (obj)
            {
                textureCache.Add(path, obj);
                return obj;
            }
            else
            {
                //download form server
                return null;
            }
        }
    }

    private void loadSelectRole()
    {
        UISelectRole.Get.LoadStage(101);
        UIShow(false);
    }

    public float ProgressValue
    {
        get
        {
            return uiLoadingProgress.fillAmount;
        }
        set{ 
            if (toProgress < value)
                toProgress = value; 
        }
    }

    public bool LoadingFinished
    {
        get{ return uiLoadingProgress.fillAmount >= 1; }
    }
}
