using AI;

/// <summary>
/// 目前是負責處理遊戲比賽 AI 的行為(實際全部的工作都拆分到 State 了).
/// </summary>
/// <remarks>
/// 添加新的 State:
/// <list type="number">
/// <item> 繼承 AI.State. </item>
/// <item> 修改 GameStateFactory. </item>
/// </list>
/// </remarks>
public class AIController : KnightSingleton<AIController>
{
    public static AIController Instance
    {
        get { return INSTANCE; }
    }
    private readonly static AIController INSTANCE = new AIController();

    private StateMachine<EGameSituation> mFSM;

    private void Awake()
    {
        mFSM = new StateMachine<EGameSituation>(new GameStateFactory(), 
                                                new MessageDispatcher<EGameSituation>(), EGameSituation.None);
    }

    private void Update()
    {
        mFSM.Update();
    }

    public void ChangeState(EGameSituation newState)
    {
        mFSM.ChangeState(newState);
    }

    public void ChangeState(EGameSituation newState, object extraInfo)
    {
        mFSM.ChangeState(newState, extraInfo);
    }
}
