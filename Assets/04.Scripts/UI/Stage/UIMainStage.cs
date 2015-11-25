using System;
using System.Collections.Generic;
using GameStruct;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 關卡介面, 會顯示很多的小關卡.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示關卡. </item>
/// <item> Call Hide() 關閉關卡. </item>
/// <item> Call ClearSelectChapter() 將之前選擇的章節記錄刪除. </item>
/// <item> (Optional)Visible 用來檢查關卡介面是否顯示. </item>
/// </list>
/// 
/// 實作細節:
/// <list type="number">
/// <item> 當關卡介面開啟時, 會根據情況切換到適當的頁面. 如果是從大廳進入關卡介面, 必須顯示
/// 最新進度的關卡; 如果是遊戲打完進入關卡介面, 則會選擇剛剛遊戲關卡的章節(這麼做是為了玩家方便刷獎勵,
/// 比如玩家的進度已經到第 9 章, 當他回到第 5 章刷獎勵時, 遊戲打完, 回到關卡介面會顯示第 5 章, 方便玩家刷獎勵). </item>
/// </list>
/// <item> 內部用 PlayerPrefs 記錄玩家的關卡章節. </item>
/// </remarks>
[DisallowMultipleComponent]
public class UIMainStage : UIBase
{
    private static UIMainStage instance;
    private const string UIName = "UIMainStage";

    private const string SelectChapterKey = "StageSelectChapter";

    private int mCurrentStageID;

    private UIMainStageImpl mMain;

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIMainStageImpl>();
        mMain.BackListener += goToGameLobby;
        mMain.Info.StartListener += enterGame;
    }

    [UsedImplicitly]
    private void Start()
    {
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    public void ClearSelectChapter()
    {
        if(PlayerPrefs.HasKey(SelectChapterKey))
            PlayerPrefs.DeleteKey(SelectChapterKey);
    }

    /// <summary>
    /// 預設顯示尚未過關關卡所在的章節.
    /// </summary>
    public void Show()
    {
        Show(true);

        buildMainStages();
    }

    private void enterGame(int stageID)
    {
//        Debug.LogFormat("enterGame, StageID:{0}", stageID);

        if(StageTable.Ins.HasByID(stageID))
        {
            TStageData stageData = StageTable.Ins.GetByID(stageID);

            if(verifyPlayer(stageData))
            {
                PlayerPrefs.SetInt(SelectChapterKey, mMain.CurrentChapter);
                pveStart(stageID);
            }
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

            GameData.StageID = mCurrentStageID;
			SceneMgr.Get.ChangeLevel(ESceneName.SelectRole);

//            pveEnd(mCurrentStageID);

            Hide();
        }
        else
            UIHint.Get.ShowHint("Start PVE fail!", Color.red);
    }

    /// <summary>
    /// 顯示主線關卡.
    /// </summary>
    private void buildMainStages()
    {
        // 1. 清空全部章節.
        mMain.RemoveAllChapters();

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
            addChapter(data.Chapter);
            
            if(data.ID <= GameData.Team.Player.NextMainStageID)
                addStage(data);
            else
                addLockStage(data);
        }

        addLastLockChapter();

        selectChapter();
    }

    private void selectChapter()
    {
        if(PlayerPrefs.HasKey(SelectChapterKey))
            mMain.SelectChapter(PlayerPrefs.GetInt(SelectChapterKey));
        else
        {
            TStageData stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
            if(stageData.IsValid())
                mMain.SelectChapter(stageData.Chapter);
            else
                mMain.SelectChapter(mMain.ChapterCount);
        }
    }

    private void addChapter(int chapter)
    {
        if(!ChapterTable.Ins.Has(chapter))
        {
            Debug.LogErrorFormat("Chapter({0}) don't exist!", chapter);
            return;
        }

        ChapterData data = ChapterTable.Ins.Get(chapter);
        mMain.AddChapter(chapter, data.Name);
    }

    private void addStage(TStageData stageData)
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
        mMain.AddStage(stageData.Chapter, stageData.ID, localPos, data); 
    }

    /// <summary>
    /// 找出玩家該關卡還可以打幾次.
    /// </summary>
    /// <param name="stageData"></param>
    /// <returns></returns>
    private static int findPlayerDailyCount(TStageData stageData)
    {
//        int dailyCount = 3; // 目前企劃規定的是, 主線關卡最多只能打 3 次.
//        if(GameData.Team.Player.StageChallengeNums.ContainsKey(stageData.ID))
//            dailyCount = stageData.ChallengeNum - GameData.Team.Player.StageChallengeNums[stageData.ID];
//        int dailyCount = stageData.ChallengeNum - GameData.Team.Player.GetStageChallengeNum(stageData.ID);
//        return dailyCount;

        return stageData.ChallengeNum - GameData.Team.Player.GetStageChallengeNum(stageData.ID);
    }

    private void addLockStage(TStageData stageData)
    {
        if(!verify(stageData))
            return;

        Vector3 localPos = new Vector3(stageData.PositionX, stageData.PositionY, 0);
        mMain.AddLockStage(stageData.Chapter, stageData.ID, localPos, stageData.KindTextIndex.ToString());
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
    private void addLastLockChapter()
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
            mMain.AddLockChapter(nextChapter, nextChapterTitle);
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

    private void stageRewardStart(int stageID)
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        mCurrentStageID = stageID;
        SendHttp.Get.Command(URLConst.StageRewardStart, waitStageRewardStart, form);
    }

    private void waitStageRewardStart(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageRewardStart, ok:{0}", ok);

        if(ok)
        {
            TStageReward reward = JsonConvert.DeserializeObject<TStageReward>(www.text);

            Debug.Log(reward);

            GameData.Team.Player = reward.Player;
            GameData.Team.Player.Init();
            GameData.Team.Items = reward.Items;

//            stageRewardAgain(mCurrentStageID);
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    private void stageRewardAgain(int stageID)
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        mCurrentStageID = stageID;
        SendHttp.Get.Command(URLConst.StageRewardAgain, waitStageRewardAgain, form);
    }

    private void waitStageRewardAgain(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageRewardAgain, ok:{0}", ok);

        if (ok)
        {
            TStageReward team = JsonConvert.DeserializeObject<TStageReward>(www.text);
            GameData.Team.Items = team.Items;
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    protected override void OnShow(bool isShow)
    {
        base.OnShow(isShow);

        mMain.Info.Hide();
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