using AI;

public class PlayerNoneState : State<EPlayerAIState>
{
    public override EPlayerAIState ID
    {
        get { return EPlayerAIState.None; }
    }

    public override void Enter(object extraInfo)
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }
}