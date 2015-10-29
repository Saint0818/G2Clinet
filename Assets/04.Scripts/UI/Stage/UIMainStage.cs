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

    /// <summary>
    /// <para> 這是 Stage.Kind 參數的對照表. [Key:Kind, Value:TextConst Index]</para>
    /// <para> 目前的規則是對照表找出的數值, 就是關卡的圖片, 也就是該關卡的類型文字(比如:傳統, 計時賽等等). </para>
    /// </summary>
    private readonly Dictionary<int, int> mStageKindMapping = new Dictionary<int, int>
    {
        {1, 2000001},
        {2, 2000002},
        {3, 2000003},
        {4, 2000004},
        {9, 2000009}
    };

    private int mCurrentStageID;

    private UIMainStageImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UIMainStageImpl>();
        mImpl.BackListener += goToGameLobby;
        mImpl.Info.StartListener += enterGame;
//        mImpl.ChapterChangeListener.OnChangeListener += onChapterChange;
    }

    [UsedImplicitly]
    private void Start()
    {
        initChapters();
    }

    private void initChapters()
    {
        foreach(UIStageChapter chapter in mImpl.Chapters)
        {
            if(!ChapterTable.Ins.Has(chapter.Chapter))
            {
                Debug.LogErrorFormat("Chapter({0}) don't exist!", chapter.Chapter);
                continue;
            }

            ChapterData data = ChapterTable.Ins.Get(chapter.Chapter);
            chapter.ChapterName = data.Name;
            chapter.ChapterValue = data.Chapter;
        }
    }

//    private void onChapterChange(int chapterID)
//    {
////        Debug.LogFormat("onChapterChange, ChapterID:{0}", chapterID);
//
//        if(ChapterTable.Ins.Has(chapterID))
//        {
//            ChapterData data = ChapterTable.Ins.Get(chapterID);
//            mImpl.ChapterNum = data.Chapter;
//            mImpl.ChapterTitle = data.Name;
//        }
//        else
//            Debug.LogErrorFormat("Can't find ChapterID:{0}", chapterID);
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

            if(verifyPlayer(stageData.CostKind, stageData.CostValue))
                startPVE(stageID);
            else
                Debug.LogWarningFormat("Player can't enter game!");
        }
        else
            Debug.LogErrorFormat("StageID({0}) don't exist!", stageID);
    }

    private bool verifyPlayer(StageData.ECostKind costKind, int costValue)
    {
        if(costKind == StageData.ECostKind.Stamina)
            return GameData.Team.Power >= costValue;

        throw new NotImplementedException();
    }

    private void startPVE(int stageID)
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
            GameStart.Get.WinMode = (EWinMode)stageData.WinMode;

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
        mImpl.HideAllChapters();
        for(int id = StageTable.MinMainStageID; id <= GameData.Team.Player.NextMainStageID; id++)
        {
            StageData stage = StageTable.Ins.GetByID(id);
            showStage(stage);
        }

        setLastChapterLock();
    }

    private void showStage(StageData stageData)
    {
        if(!stageData.IsValid())
        {
            Debug.LogWarningFormat("Stage({0}) don't exist!", stageData.ID);
            return;
        }

        if(!mStageKindMapping.ContainsKey(stageData.Kind))
        {
            Debug.LogErrorFormat("StageID({0}), Kind({1}) don't exist!", stageData.ID, stageData.Kind);
            return;
        }

        int textIndex = mStageKindMapping[stageData.Kind];
        if(string.IsNullOrEmpty(TextConst.S(textIndex)))
        {
            Debug.LogErrorFormat("TextConst({0}) don't exist!", textIndex);
            return;
        }

        mImpl.ShowChapter(stageData.Chapter);

        UIStageInfo.Data data = new UIStageInfo.Data
        {
            Name = stageData.Name,
            Description = stageData.Explain,
            KindSpriteName = textIndex.ToString(),
            KindName = TextConst.S(textIndex),
            RewardSpriteName = "GoldCoin",
            RewardName = "",
            Stamina = stageData.CostValue
        };
        mImpl.ShowStage(stageData.ID, data); 
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
        if(StageTable.Ins.HasByChapter(nextChapter))
            mImpl.ShowChapterLock(nextChapter);
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