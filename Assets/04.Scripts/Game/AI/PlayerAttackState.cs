using AI;

public class PlayerAttackState : State<EPlayerAIState>
{
    public override EPlayerAIState ID
    {
        get { return EPlayerAIState.Attack; }
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
