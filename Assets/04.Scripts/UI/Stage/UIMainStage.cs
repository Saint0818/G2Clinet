using System;
using System.Collections.Generic;
using GamePlayEnum;
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
//        initChapters();
    }

//    private void initChapters()
//    {
//        foreach(UIStageChapter chapter in mImpl.Chapters)
//        {
//            if(!ChapterTable.Ins.Has(chapter.Chapter))
//            {
//                Debug.LogErrorFormat("Chapter({0}) don't exist!", chapter.Chapter);
//                continue;
//            }
//
//            ChapterData data = ChapterTable.Ins.Get(chapter.Chapter);
//            chapter.ChapterName = data.Name;
//            chapter.ChapterValue = data.Chapter;
//        }
//    }

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
            StageData stageData = StageTable.Ins.GetByID(stageID);

            if(verifyPlayer(stageData.CostKind, stageData.CostValue, stageData.LimitLevel))
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
    /// <param name="costKind"></param>
    /// <param name="costValue"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    private bool verifyPlayer(StageData.ECostKind costKind, int costValue, int level)
    {
        switch(costKind)
        {
            case StageData.ECostKind.Stamina:
                if(GameData.Team.Power < costValue)
                    return false;
                break;
            case StageData.ECostKind.Activity:
            case StageData.ECostKind.Challenger:
            default:
                throw new NotImplementedException();
        }

        if(GameData.Team.Player.Lv < level)
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

    [UsedImplicitly]
    private class PVEInfo
    {
        [UsedImplicitly]
        public int NewPower;
    }

    private void waitPVEStart(bool ok, WWW www)
    {
        Debug.LogFormat("waitPVEStart, ok:{0}", ok);

        if(ok)
        {
            var info = JsonConvert.DeserializeObject<PVEInfo>(www.text);
            GameData.Team.Power = info.NewPower;

            GameData.StageID = mCurrentStageID;
            var stageData = StageTable.Ins.GetByID(mCurrentStageID);

            GameStart.Get.CourtMode = (ECourtMode)stageData.CourtMode;
            GameStart.Get.WinMode = stageData.WinMode;

            if (stageData.WinValue > 0)
                GameStart.Get.GameWinValue = stageData.WinValue;

            if (stageData.FriendNumber > 0)
                GameStart.Get.FriendNumber = stageData.FriendNumber;

            SceneMgr.Get.ChangeLevel(ESceneName.SelectRole);

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
        List<StageData> allStageData = new List<StageData>();
        // 主線關卡是從第一章開始顯示.
        StageTable.Ins.GetByChapterRange(1, maxChapter, ref allStageData);

        // 3. 設定每一個小關卡.
        foreach(StageData data in allStageData)
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

    private void showStage(StageData stageData)
    {
        if(!verify(stageData))
            return;

        UIStageInfo.Data data = new UIStageInfo.Data
        {
            Name = stageData.Name,
            Description = stageData.Explain,
            KindSpriteName = stageData.KindTextIndex.ToString(),
            KindName = TextConst.S(stageData.KindTextIndex),
            RewardSpriteName = "GoldCoin",
            RewardName = "",
            Stamina = stageData.CostValue
        };
        Vector3 localPos = new Vector3(stageData.PositionX, stageData.PositionY, 0);
        mImpl.ShowStage(stageData.Chapter, stageData.ID, localPos, data); 
    }

    private void showStageLock(StageData stageData)
    {
        if(!verify(stageData))
            return;

        Vector3 localPos = new Vector3(stageData.PositionX, stageData.PositionY, 0);
        mImpl.ShowStageLock(stageData.Chapter, stageData.ID, localPos, stageData.KindTextIndex.ToString());
    }

    private static bool verify(StageData stageData)
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
        StageData stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
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