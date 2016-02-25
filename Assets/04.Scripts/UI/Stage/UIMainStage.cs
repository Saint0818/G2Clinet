using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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

    private UIMainStageMain mMain;

    private void Awake()
    {
        mMain = GetComponent<UIMainStageMain>();
        mMain.BackListener += goToGameLobby;
        mMain.Info.StartListener += enterSelectRole;
    }

    private void OnPowerChange(int power)
    {
        if(mMain.Info.Visible)
            Show(mMain.Info.StageID);
    }

    [UsedImplicitly]
    private void Start()
    {
        GameData.Team.OnPowerChangeListener += OnPowerChange;
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    /// <summary>
    /// 預設顯示尚未過關關卡所在的章節.
    /// </summary>
    public void Show()
    {
        Show(true);

//        UIMainStageTools.SetDebugNewChapter();
        buildChapters();

        if(isNeedPlayUnlockAnimation())
            playUnlokAnimation();
        else
            selectChapter();
    }

    public void Show(int stageID)
    {
        UIMainLobby.Get.Hide();
        Show(true);

        buildChapters();
        selectStage(stageID);
    }

    private void selectStage(int stageID)
    {
        if(!StageTable.Ins.HasByID(stageID))
        {
            Debug.LogErrorFormat("Can't find Stage({0})", stageID);
            return;
        }

        TStageData stageData = StageTable.Ins.GetByID(stageID);
        mMain.ShowStageInfo(stageData.Chapter, stageID);
    }

    private void enterSelectRole(int stageID)
    {
//        Debug.LogFormat("enterSelectRole, StageID:{0}", stageID);

        if(!StageTable.Ins.HasByID(stageID))
        {
            Debug.LogErrorFormat("StageID({0}) don't exist!", stageID);
            return;
        }

        TStageData stageData = StageTable.Ins.GetByID(stageID);
        if(!stageData.IsValid())
        {
            Debug.LogErrorFormat("StageID:{0}, StageData Error.", stageID);
            return;
        }

        // todo 購買體力流程, 暫時寫在這, 以後再改流程.
        if(!CheckPower(stageData.CostValue, true))
        {
            OnBuyPower();
            return;
        }

        string errMsg;
        if(UIStageTools.VerifyPlayer(stageData, out errMsg))
        {
//            UIMainStageDebug debug = new UIMainStageDebug();
//            debug.SendCommand(stageID);

            UIMainStageTools.Record(stageData.Chapter);
            UISelectRole.Get.LoadStage(stageID);

            Hide();
        }
        else
        {
            Debug.LogWarning(errMsg);
            UIHint.Get.ShowHint(errMsg, Color.green);
        }
    }

    private bool isNeedPlayUnlockAnimation()
    {
        return UIMainStageTools.HasNewChapter() || UIMainStageTools.HasNewStage();
    }

    private void playUnlokAnimation()
    {
        if(UIMainStageTools.HasNewChapter())
        {
            TStageData stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
            if(stageData.IsValid())
                mMain.PlayUnlockChapterAnimation(stageData.Chapter, stageData.ID);
        }
        else if(UIMainStageTools.HasNewStage())
        {
            TStageData stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
            if(stageData.IsValid())
            {
                mMain.SelectChapter(stageData.Chapter);
                mMain.GetChapter(stageData.Chapter).GetStageByID(stageData.ID).PlayUnlockAnimation();
            }
        }

        UIMainStageTools.ClearStageFlag();
    }

    /// <summary>
    /// 取出可顯示章節的全部關卡.
    /// </summary>
    private void buildChapters()
    {
        mMain.RemoveAllChapters();

        int maxChapter = StageTable.Ins.MainStageMaxChapter;
        if(StageTable.Ins.HasByID(GameData.Team.Player.NextMainStageID))
            maxChapter = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID).Chapter;
        List<TStageData> allMainStage = new List<TStageData>();
        // 主線關卡是從第一章開始顯示.
        StageTable.Ins.GetMainStageByChapterRange(1, maxChapter, ref allMainStage);

        // 3. 設定每一個小關卡.
        foreach(TStageData data in allMainStage)
        {
            addChapter(data.Chapter);
            addStage(data);
        }

        addLastLockChapter();
    }

    private void selectChapter()
    {
        if(UIMainStageTools.HasSelectChapter())
            mMain.SelectChapter(UIMainStageTools.GetSelectChapter());
        else
        {
            // 切換到最新章節.
            TStageData stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
            mMain.SelectChapter(stageData.IsValid() ? stageData.Chapter : mMain.ChapterCount);
        }
    }

    private void addChapter(int chapter)
    {
        if(!StageChapterTable.Ins.HasMain(chapter))
        {
            Debug.LogErrorFormat("Chapter({0}) don't exist!", chapter);
            return;
        }

        ChapterData data = StageChapterTable.Ins.GetMain(chapter);
        mMain.AddChapter(chapter, data.Name);
    }

    private void addStage(TStageData stageData)
    {
        if(!verify(stageData))
            return;

        UIStageInfo.Data infoData = new UIStageInfo.Data
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
            RemainDailyCount = String.Format(TextConst.S(9312), UIStageTools.FindPlayerRemainDailyCount(stageData)),
            StartEnable = UIStageTools.VerifyPlayer(stageData),
            RewardTitle = UIMainStageTools.FindRewardTitle(stageData)
        };

        infoData.RewardItems.AddRange(UIMainStageTools.FindRewardItems(stageData));

        Vector3 localPos = new Vector3(stageData.PositionX, stageData.PositionY, 0);

        UIStageElement.Data elementData = new UIStageElement.Data
        {
            IsSelected = stageData.ID == GameData.Team.Player.NextMainStageID
        };
        elementData.IsEnable = UIStageTools.VerifyPlayerProgress(stageData, out elementData.ErrMsg);
        if(elementData.IsEnable) // 再一次驗證關卡是不是只能打一次.
        {
            elementData.IsEnable = UIStageTools.VerifyPlayerChallengeOnlyOnce(stageData, out elementData.ErrMsg);
            elementData.ShowClear = !elementData.IsEnable;
        }

        if(stageData.Kind != 9)
        {
            elementData.BGNormalIcon = elementData.IsEnable ? "StageButton01" : "StageButton03";
            elementData.BGPressIcon = elementData.IsEnable ? "StageButton02" : "StageButton03";
            mMain.AddStage(stageData.Chapter, stageData.ID, localPos, elementData, infoData);
        }
        else
        {
            elementData.BGNormalIcon = elementData.IsEnable ? "2000009" : "StageButton09";
            elementData.BGPressIcon = elementData.IsEnable ? "StageButton08" : "StageButton09";
            mMain.AddBossStage(stageData.Chapter, stageData.ID, localPos, elementData, infoData);
        }
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
        if(StageChapterTable.Ins.HasMain(nextChapter))
        {
            ChapterData chapterData = StageChapterTable.Ins.GetMain(nextChapter);
            nextChapterTitle = chapterData.Name;
        }

        if(StageTable.Ins.HasMainStageByChapter(nextChapter))
            mMain.AddLockChapter(nextChapter, nextChapterTitle);
    }

    public void Hide()
    {
        GameData.Team.OnPowerChangeListener -= OnPowerChange;

        RemoveUI(UIName);
    }

    private void goToGameLobby()
    {
        UIGameLobby.Get.Show();
        Hide();
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

    public static bool verify(TStageData stageData)
    {
        if(!stageData.IsValid())
        {
            Debug.LogWarningFormat("Stage({0}) don't exist!", stageData.ID);
            return false;
        }

        return true;
    }
}