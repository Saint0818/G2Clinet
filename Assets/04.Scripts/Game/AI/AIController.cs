using AI;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// <para> 這就像是一個神物件, 對遊戲中的全部球員下達命令. 比如要球員執行什麼戰術, 並根據情況命令球員要做什麼動作
/// (傳球, 投籃等等). </para>
/// <para> It's like a god of the game. For example: AIController order all players to execute tactics
/// under the circumstances(passing, shooting, etc.). </para>
/// <para></para>
/// </summary>
/// <remarks>
/// How to add State:
/// <list type="number">
/// <item> inherit AI.State. </item>
/// <item> call StateMachine.AddState() in setup StateMachine. </item>
/// </list>
/// </remarks>
[DisallowMultipleComponent]
public class AIController : KnightSingleton<AIController>
{
    public static AIController Instance
    {
        get { return INSTANCE; }
    }
    private readonly static AIController INSTANCE = new AIController();

    private StateMachine<EGameSituation, EGameMsg> mFSM;

    [UsedImplicitly]
    private void Awake()
    {
        mFSM = new StateMachine<EGameSituation, EGameMsg>(new MessageDispatcher<EGameMsg>());
        mFSM.AddState(new NullState());
        mFSM.AddState(new PresentationState());
        mFSM.AddState(new CameraMovementState());
        mFSM.AddState(new InitCourtState());
        mFSM.AddState(new OpeningState());
        mFSM.AddState(new APickBallAfterScoreState());
        mFSM.AddState(new InboundsAState());
        mFSM.AddState(new BPickBallAfterScoreState());
        mFSM.AddState(new InboundsBState());
        mFSM.AddState(new SpecialActionState());
        mFSM.ChangeState(EGameSituation.None);
    }

    [UsedImplicitly]
    private void FixedUpdate()
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

    public void SendMesssage(EGameMsg msg, ITelegraph<EGameSituation> sender = null,
                             ITelegraph<EGameSituation> receiver = null, Object extraInfo = null)
    {
        mFSM.Dispatcher.SendMesssage(msg);
    }
}
