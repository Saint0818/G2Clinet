using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 主線關卡介面.
/// </summary>
/// <remarks>
/// 使用方法:
/// <list type="number">
/// <item> 用 Get 取得 instance. </item>
/// <item> Call Show() 顯示關卡. </item>
/// <item> Call Hide() 關閉關卡. </item>
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

    private UIMainStageView mView;

    private void Awake()
    {
        mView = GetComponent<UIMainStageView>();
        mView.BackListener += goToGameLobby;
//        mMain.Info.StartListener += enterSelectRole;
//        mMain.Info.ShowListener += () => Statistic.Ins.LogScreen(7);
    }

    [UsedImplicitly]
    private void Start()
    {
        GameData.Team.OnPowerChangeListener += onPowerChange;
        GameData.Team.OnDiamondChangeListener += onDiamondChange;
    }

    private void onPowerChange(int power)
    {
//        if(mMain.Info.Visible)
//            Show(mMain.Info.StageID);
    }

    private void onDiamondChange(int diamond)
    {
//        if(mMain.Info.Visible)
//            Show(mMain.Info.StageID);
    }

    private void OnDestroy()
    {
        GameData.Team.OnPowerChangeListener -= onPowerChange;
        GameData.Team.OnDiamondChangeListener -= onDiamondChange;
    }

    public bool Visible { get { return gameObject.activeSelf; } }

    /// <summary>
    /// 預設顯示尚未過關關卡所在的章節.
    /// </summary>
    public void Show()
    {
        Show(true);

//        UIMainStageTools.SetDebugNewChapter();
//        UIMainStageTools.SetDebugNewStage();

        buildChapters();

        if(isNeedPlayUnlockAnimation())
            playUnlokAnimation();
        else
            selectChapter();

        Statistic.Ins.LogScreen(6);
    }

    public void Show(int stageID)
    {
        UIMainLobby.Get.Hide(false);
        UIResource.Get.Show();

        Show(true);

        buildChapters();
//        selectStage(stageID);

        Statistic.Ins.LogScreen(6);
    }

//    private void selectStage(int stageID)
//    {
//        if(!StageTable.Ins.HasByID(stageID))
//        {
//            Debug.LogErrorFormat("Can't find Stage({0})", stageID);
//            return;
//        }
//
//        TStageData stageData = StageTable.Ins.GetByID(stageID);
//        mMain.ShowStageInfo(stageData.Chapter, stageID);
//    }

    private void enterSelectRole(int stageID, UIStageVerification.EErrorCode errorCode, string errMsg)
    {
        TStageData stageData = StageTable.Ins.GetByID(stageID);
        switch(errorCode)
        {
            case UIStageVerification.EErrorCode.Pass:
//            UIMainStageDebug debug = new UIMainStageDebug();
//            debug.SendCommand(stageID);

                UIMainStageTools.Record(stageData.Chapter);
                UISelectRole.Get.LoadStage(stageID);
                Hide();
                break;
            case UIStageVerification.EErrorCode.NoPower:
                OnBuyPower();
                break;
            case UIStageVerification.EErrorCode.NoDailyChallenge:
                var protocol = new ResetStageDailyChallengeProtocol();
                protocol.Send(stageData.ID, ok => {if(ok) Show(stageData.ID);});
                break;
            default:
                Debug.LogWarning(errMsg);
                UIHint.Get.ShowHint(errMsg, Color.red);
                break;
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
                mView.PlayUnlockChapterAnimation(stageData.Chapter, stageData.ID);
        }
        else if(UIMainStageTools.HasNewStage())
        {
            TStageData stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
            if(stageData.IsValid())
            {
                mView.SelectChapter(stageData.Chapter);
                mView.GetChapter(stageData.Chapter).GetStageByID(stageData.ID).PlayUnlockAnimation();
            }
        }

        UIMainStageTools.ClearStageFlag();
    }

    /// <summary>
    /// 取出可顯示章節的全部關卡.
    /// </summary>
    private void buildChapters()
    {
        mView.RemoveAllChapters();

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
            addStageElement(data);
        }

        addLastLockChapter();
    }

    private void selectChapter()
    {
        if(UIMainStageTools.HasSelectChapter())
            mView.SelectChapter(UIMainStageTools.GetSelectChapter());
        else
        {
            // 切換到最新章節.
            TStageData stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageID);
            mView.SelectChapter(stageData.IsValid() ? stageData.Chapter : mView.ChapterCount);
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
        UIStageChapter stageChapter = mView.AddChapter(chapter, data.Name);
        stageChapter.Info.StartListener += enterSelectRole;
        stageChapter.Info.ShowListener += () => Statistic.Ins.LogScreen(7);
    }

    private void addStageElement(TStageData stageData)
    {
        if(!stageData.IsValid())
        {
            Debug.LogWarningFormat("Stage({0}) don't exist!", stageData.ID);
            return;
        }

        var infoData = UIMainStageBuilder.BuildInfo(stageData);
        var elementData = UIMainStageBuilder.BuildElement(stageData);
        Vector3 localPos = new Vector3(stageData.PositionX, stageData.PositionY, 0);

        if(stageData.Kind != 9)
            mView.AddStage(stageData.Chapter, stageData.ID, localPos, elementData, infoData);
        else
            mView.AddBossStage(stageData.Chapter, stageData.ID, localPos, elementData, infoData);
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
            mView.AddLockChapter(nextChapter, nextChapterTitle);
    }

    public void Hide()
    {
        GameData.Team.OnPowerChangeListener -= onPowerChange;

//        RemoveUI(instance.gameObject);
        Destroy(instance.gameObject);
    }

    private void goToGameLobby()
    {
        UIGameLobby.Get.Show();
        Hide();
    }

//    protected override void OnShow(bool isShow)
//    {
//        base.OnShow(isShow);
//
//        mMain.Info.Hide();
//    }

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