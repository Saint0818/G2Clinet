using AI;

public class AIController : KnightSingleton<AIController>
{
    public static AIController Instance
    {
        get { return INSTANCE; }
    }
    private readonly static AIController INSTANCE = new AIController();

    private StateMachine<EGameSituation> mFSM;
//    private MessageDispatcher<EGameSituation> mMessageDispatcher = new MessageDispatcher<EGameSituation>();

    private void Awake()
    {
        mFSM = new StateMachine<EGameSituation>(new GameStateFactory());
    }

    private void Update()
    {
        mFSM.Update();
    }


}
