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

    private UIMainStageMain mMain;

    [UsedImplicitly]
    private void Awake()
    {
        mMain = GetComponent<UIMainStageMain>();
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

        if(!StageTable.Ins.HasByID(stageID))
        {
            Debug.LogErrorFormat("StageID({0}) don't exist!", stageID);
            return;
        }

        TStageData stageData = StageTable.Ins.GetByID(stageID);
        if(!string.IsNullOrEmpty(stageData.verify()))
        {
            Debug.LogErrorFormat(stageData.verify());
            return;
        }

        string errMsg;
        if(verifyPlayer(stageData, out errMsg))
        {
            PlayerPrefs.SetInt(SelectChapterKey, mMain.CurrentChapter);
            pveStart(stageID);
        }
        else
        {
            Debug.LogWarning(errMsg);
            UIHint.Get.ShowHint(errMsg, Color.green);
        }
    }

    private bool verifyPlayer(TStageData stageData)
    {
        string errMsg;
        return verifyPlayer(stageData, out errMsg);
    }

    /// <summary>
    /// 檢查玩家是否可以進入遊戲.
    /// </summary>
    /// <param name="stageData"></param>
    /// <param name="errMsg"></param>
    /// <returns></returns>
    private bool verifyPlayer(TStageData stageData, out string errMsg)
    {
        if(!verifyPlayerCost(stageData))
        {
            errMsg = TextConst.S(230);
            return false;
        }

        if(!verifyPlayerDailyCount(stageData))
        {
            errMsg = TextConst.S(231);
            return false;
        }

        if (!verifyPlayerLv(stageData))
        {
            errMsg = TextConst.S(232);
            return false;
        }

        errMsg = String.Empty;
        return true;
    }

    private static bool verifyPlayerCost(TStageData stageData)
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
        return true;
    }

    private static bool verifyPlayerDailyCount(TStageData stageData)
    {
        return findPlayerDailyCount(stageData) > 0;
    }

    private static bool verifyPlayerLv(TStageData stageData)
    {
        return GameData.Team.Player.Lv >= stageData.LimitLevel;
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
            GameData.Team.PowerCD = team.PowerCD;

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

        autoSelectChapter();
    }

    private void autoSelectChapter()
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
            Money = stageData.Money,
            ExpVisible = GameData.Team.Player.NextMainStageID <= stageData.ID,
            Exp = stageData.Exp,
            Stamina = stageData.CostValue,
            ShowCompleted = stageData.ID < GameData.Team.Player.NextMainStageID,
            DailyCount = string.Format(TextConst.S(9312), findPlayerDailyCount(stageData)),
            StartEnable = verifyPlayer(stageData)
        };

        // 和企劃約定好, 僅顯示前 3 個.
        for(int i = 0; i < 3; i++)
        {
			if(stageData.Rewards != null && stageData.Rewards.Length > i && GameData.DItemData.ContainsKey(stageData.Rewards[i]))
                data.RewardItems.Add(GameData.DItemData[stageData.Rewards[i]]);
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
        int remainDailyCount = stageData.ChallengeNum - GameData.Team.Player.GetStageChallengeNum(stageData.ID);
        return Mathf.Max(0, remainDailyCount); // 強迫數值大於等於 0.
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

//        int textIndex = stageData.KindTextIndex;
//        if(string.IsNullOrEmpty(TextConst.S(textIndex)))
//        {
//            Debug.LogErrorFormat("TextConst({0}) don't exist!", textIndex);
//            return false;
//        }
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

    private void waitPVEEnd(bool ok, WWW www)
    {
        Debug.LogFormat("waitPVEEnd, ok:{0}", ok);

        if(ok)
        {
            TTeam team = JsonConvert.DeserializeObject<TTeam>(www.text);
            GameData.Team.Player = team.Player;
            GameData.Team.Player.Init();

//            stageRewardStart(mCurrentStageID);
        }
        else
            UIHint.Get.ShowHint("PVE End fail!", Color.red);
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
            TStageRewardStart reward = JsonConvert.DeserializeObject<TStageRewardStart>(www.text);

            Debug.LogFormat("waitStageRewardStart:{0}", reward);

            GameData.Team.Money = reward.Money;
            GameData.Team.Diamond = reward.Diamond;
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

        if(ok)
        {
            var reward = JsonConvert.DeserializeObject<TStageRewardAgain>(www.text);
            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Items = reward.Items;

            Debug.LogFormat("waitStageRewardAgain:{0}", reward);

//            stageRewardAgain2(mCurrentStageID);
        }
        else
            UIHint.Get.ShowHint("Stage Reward fail!", Color.red);
    }

    private void stageRewardAgain2(int stageID)
    {
        WWWForm form = new WWWForm();
        form.AddField("StageID", stageID);
        mCurrentStageID = stageID;
        SendHttp.Get.Command(URLConst.StageRewardAgain, waitStageRewardAgain2, form);
    }

    private void waitStageRewardAgain2(bool ok, WWW www)
    {
        Debug.LogFormat("waitStageRewardAgain2, ok:{0}", ok);

        if (ok)
        {
            var reward = JsonConvert.DeserializeObject<TStageRewardAgain>(www.text);

            Debug.LogFormat("waitStageRewardAgain2:{0}", reward);

            GameData.Team.Diamond = reward.Diamond;
            GameData.Team.Items = reward.Items;
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