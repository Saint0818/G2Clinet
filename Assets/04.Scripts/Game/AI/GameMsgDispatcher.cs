using AI;

public class GameMsgDispatcher : MessageDispatcher<EGameMsg>
{
    public static GameMsgDispatcher Ins { get { return instance; } }

    private static readonly GameMsgDispatcher instance = new GameMsgDispatcher();

    private GameMsgDispatcher() { }
}
