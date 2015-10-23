using System.Collections.Generic;
using JetBrains.Annotations;
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
    /// <para> 這是 Stage.Kind 參數的對照表. </para>
    /// <para> 目前有一個特殊的假設, 對照表找出的數值, 就是關卡的圖片, 也就是該關卡的類型文字(比如:傳統, 計時賽等等). </para>
    /// </summary>
    private readonly Dictionary<int, int> mStageKindMapping = new Dictionary<int, int>
    {
        {1, 2000001},
        {2, 2000002},
        {3, 2000003},
        {4, 2000004},
        {9, 2000005}
    };

    private UIMainStageImpl mImpl;

    [UsedImplicitly]
    private void Awake()
    {
        mImpl = GetComponent<UIMainStageImpl>();
        mImpl.BackListener += goToGameLobby;
    }

    public void Show()
    {
        Show(true);

        // 這是和企劃表非常有關係的設計. 主線關卡的 ID 範圍是 101 ~ 2000,
        // 所以這樣判斷就可以知道玩家是否有打過任何關卡.
//        if(GameData.Team.Player.NextMainStageSchedule <= 100)
//            showStartChapter(); // 未打過任何關卡, 所以顯示第一章的資訊.
//        else
            showChapters();
    }

//    /// <summary>
//    /// 顯示新手出現的關卡頁面.
//    /// </summary>
//    private void showStartChapter()
//    {
//        mImpl.HideAllChapters();

//        mImpl.ShowChapter(1);
//        mImpl.ShowChapterLock(2);

//        showStage(StageTable.Ins.GetByID(1));

//        List<Stage> stageData = StageTable.Ins.GetByChapter(1);
//        int minID = int.MaxValue;
//        foreach(Stage data in stageData)
//        {
//            if(minID > data.ID && data.Order >= 1)
//                minID = data.ID;
//        }
//        showStage(StageTable.Ins.GetByID(minID));
//    }

    private void showChapters()
    {
        mImpl.HideAllChapters();
        for(int id = StageTable.MinMainStageID; id <= GameData.Team.Player.NextMainStageSchedule; id++)
        {
            Stage stage = StageTable.Ins.GetByID(id);
            showStage(stage);
        }

        setLastChapterLock();
    }

    private void showStage(Stage stageData)
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
//        mImpl.SetStage(stageData.ID, textIndex.ToString());
        mImpl.ShowStage(stageData.ID, "Iconface"); // todo 因為暫時還沒有圖檔, 所以暫時用別的圖片代替.
    }

    /// <summary>
    /// 根據玩家的進度, 設定下一個章節為 lock 狀態.
    /// </summary>
    private void setLastChapterLock()
    {
        Stage stageData = StageTable.Ins.GetByID(GameData.Team.Player.NextMainStageSchedule);
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