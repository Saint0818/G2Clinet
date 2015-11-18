using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 關卡頁面, 會顯示很多的小關卡.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示關卡. </item>
/// <item> Call Hide() 關閉關卡. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class UIMainStage : UIBase
{
    private static UIMainStage instance;
    private const string UIName = "UIMainStage";

    private int mCurrentStageID;

    private UIMainStageImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UIMainStageImpl>();
        mImpl.BackListener += goToGameLobby;
        mImpl.Info.StartListener += enterGame;
    }

    [UsedImplicitly]
    private void Start()
    {
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void Show()
    {
        Show(true);

        showMainStages();
    }

    private void enterGame(int stageID)
    {
        Debug.LogFormat("enterGame, StageID:{0}", stageID);

        if(StageTable.Ins.HasByID(stageID))
        {
            TStageData stageData = StageTable.Ins.GetByID(stageID);

            if(verifyPlayer(stageData))
                pveStart(stageID);
            else
                Debug.LogWarningFormat("Player can't enter game!");
        }
        else
            Debug.LogErrorFormat("StageID({0}) don't exist!", stageID);
    }

    /// <summary>
    /// 檢查玩家是否可以進入遊戲.
    /// </summary>
    /// <param name="stageData"></param>
    /// <returns></returns>
    private bool verifyPlayer(TStageData stageData)
    {
        switch(stageData.CostKind)
        {
            case TStageData.ECostKind.Stamina:
                if(GameData.Team.Power < stageData.CostValue)
                    return false;
                break;
            case TStageData.ECostKind.Activity:
            case TStageData.ECostKind.Challenger:
            default:
                throw new NotImplementedException();
        }

        if(GameData.Team.Player.Lv < stageData.LimitLevel)
            return false;

        if(findPlayerDailyCount(stageData) <= 0)
            return false;

        return true;
    }

    private void pveStart(int stageID)
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        mCurrentStageID = stageID;
        SendHttp.Get.Command(URLConst.PVEStart, waitPVEStart, form);
    }

    private void waitPVEStart(bool ok, WWW www)
    {
        Debug.LogFormat("waitPVEStart, ok:{0}", ok);

        if(ok)
        {
            var team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Power = team.Power;
//            GameData.Team.Player.StageChallengeNums = new Dictionary<int, int>(team.Player.StageChallengeNums);

            GameData.StageID = mCurrentStageID;
			SceneMgr.Get.ChangeLevel(ESceneName.SelectRole);

//            stageSurelyReward(mCurrentStageID);
//            stageRandomRewardStart(mCurrentStageID);

            Hide();
        }
        else
            UIHint.Get.ShowHint("Start PVE fail!", Color.red);
    }

    /// <summary>
    /// 顯示主線關卡.
    /// </summary>
    private void showMainStages()
    {
        // 1. 清空全部章節.
        mImpl.RemoveAllChapters();

        // for debug.
//        GameData.Team.Player.NextMainStageID = 0;

        // 2. 取出可顯示章節的全部關卡.
        int maxChapter = StageTable.Ins.MainStageMaxChapter;
        if(StageTable.Ins.HasByID(GameData.Team.Player.NextMainStageID))
            maxChapter = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID).Chapter;
        List<TStageData> allStageData = new List<TStageData>();
        // 主線關卡是從第一章開始顯示.
        StageTable.Ins.GetByChapterRange(1, maxChapter, ref allStageData);

        // 3. 設定每一個小關卡.
        foreach(TStageData data in allStageData)
        {
            showChapter(data.Chapter);
            
            if(data.ID <= GameData.Team.Player.NextMainStageID)
                showStage(data);
            else
                showStageLock(data);
        }

        setLastChapterLock();
    }

    private void showChapter(int chapter)
    {
        if(!ChapterTable.Ins.Has(chapter))
        {
            Debug.LogErrorFormat("Chapter({0}) don't exist!", chapter);
            return;
        }

        ChapterData data = ChapterTable.Ins.Get(chapter);
        mImpl.ShowChapter(chapter, data.Name);
    }

    private void showStage(TStageData stageData)
    {
        if(!verify(stageData))
            return;

        UIStageInfo.Data data = new UIStageInfo.Data
        {
            Name = stageData.Name,
            BgTextureName = stageData.KindTextIndex.ToString(),
            Description = stageData.Explain,
            KindSpriteName = stageData.KindTextIndex.ToString(),
            KindName = TextConst.S(stageData.KindTextIndex),
            Stamina = stageData.CostValue,
            ShowCompleted = stageData.ID < GameData.Team.Player.NextMainStageID,
            DailyCount = findPlayerDailyCount(stageData),
            StartEnable = verifyPlayer(stageData)
        };

        for(int i = 0; i < data.RewardSpriteNames.Length; i++)
        {
            if(stageData.Rewards.Length > i && GameData.DItemData.ContainsKey(stageData.Rewards[i]))
                data.RewardSpriteNames[i] = GameData.DItemData[stageData.Rewards[i]].Icon;
        }

        Vector3 localPos = new Vector3(stageData.PositionX, stageData.PositionY, 0);
        mImpl.ShowStage(stageData.Chapter, stageData.ID, localPos, data); 
    }

    /// <summary>
    /// 找出玩家該關卡還可以打幾次.
    /// </summary>
    /// <param name="stageData"></param>
    /// <returns></returns>
    private int findPlayerDailyCount(TStageData stageData)
    {
        int dailyCount = 3; // 目前企劃規定的是, 主線關卡最多只能打 3 次.
        if(GameData.Team.Player.StageChallengeNums.ContainsKey(stageData.ID))
            dailyCount = stageData.ChallengeNum - GameData.Team.Player.StageChallengeNums[stageData.ID];
        return dailyCount;
    }

    private void showStageLock(TStageData stageData)
    {
        if(!verify(stageData))
            return;

        Vector3 localPos = new Vector3(stageData.PositionX, stageData.PositionY, 0);
        mImpl.ShowStageLock(stageData.Chapter, stageData.ID, localPos, stageData.KindTextIndex.ToString());
    }

    private static bool verify(TStageData stageData)
    {
        if(!stageData.IsValid())
        {
            Debug.LogWarningFormat("Stage({0}) don't exist!", stageData.ID);
            return false;
        }

        int textIndex = stageData.KindTextIndex;
        if(string.IsNullOrEmpty(TextConst.S(textIndex)))
        {
            Debug.LogErrorFormat("TextConst({0}) don't exist!", textIndex);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 根據玩家的進度, 設定下一個章節為 lock 狀態.
    /// </summary>
    private void setLastChapterLock()
    {
        TStageData stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
        if(!stageData.IsValid())
            return;

        int nextChapter = stageData.Chapter + 1;
        string nextChapterTitle = "";
        if(ChapterTable.Ins.Has(nextChapter))
        {
            ChapterData chapterData = ChapterTable.Ins.Get(nextChapter);
            nextChapterTitle = chapterData.Name;
        }

        if(StageTable.Ins.HasByChapter(nextChapter))
            mImpl.ShowChapterLock(nextChapter, nextChapterTitle);
    }

    public void Hide()
    {
        RemoveUI(UIName);
    }

    private void goToGameLobby()
    {
        UIGameLobby.Get.Show();
        Hide();
    }

    /// <summary>
    /// 要求 Server 給必給獎勵.
    /// </summary>
    /// <param name="stageID"></param>
    private void stageSurelyReward(int stageID)
    {
        TStageData data = StageTable.Ins.GetByID(stageID);
        if(data.SurelyRewards == null || data.SurelyRewards.Length <= 0)
        {
            Debug.LogErrorFormat("StageID:{0}, Surely Reward is empty!", stageID);
            return;
        }

        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        mCurrentStageID = stageID;
        SendHttp.Get.Command(URLConst.StageSurelyReward, waitStageSurelyReward, form);
    }

    private void waitStageSurelyReward(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageSurelyReward, ok:{0}", ok);

        if (ok)
        {
            var team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Items = team.Items;
        }
        else
            UIHint.Get.ShowHint("Stage Surely Reward fail!", Color.red);
    }

    private void stageRandomRewardStart(int stageID)
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        mCurrentStageID = stageID;
        SendHttp.Get.Command(URLConst.StageRandomRewardStart, waitStageRandomRewardStart, form);
    }

    private void waitStageRandomRewardStart(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageRandomRewardStart, ok:{0}", ok);

        if(ok)
        {
            var team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Items = team.Items;

//            stageRandomRewardAgain(mCurrentStageID);
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    private void stageRandomRewardAgain(int stageID)
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        mCurrentStageID = stageID;
        SendHttp.Get.Command(URLConst.StageRandomRewardAgain, waitStageRandomRewardAgain, form);
    }

    private void waitStageRandomRewardAgain(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageRandomRewardAgain, ok:{0}", ok);

        if (ok)
        {
            var team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Items = team.Items;
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    public static UIMainStage Get
    {
        get
        {
            if(!instance)
            {
                UI2D.UIShow(true);
                instance = LoadUI(UIName) as UIMainStage;
            }
			
            return instance;
        }
    }
}