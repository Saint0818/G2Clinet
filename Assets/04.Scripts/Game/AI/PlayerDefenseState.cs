using AI;

public class PlayerDefenseState : State<EPlayerAIState, EGameMsg>
{
    public override EPlayerAIState ID
    {
        get { return EPlayerAIState.Defense; }
    }

    private readonly PlayerBehaviour mPlayer;

    public PlayerDefenseState(PlayerBehaviour player)
    {
        mPlayer = player;
    }

    public override void Enter(object extraInfo)
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if(mPlayer.AIing && !GameController.Get.DoSkill(mPlayer))
            GameController.Get.AIDefend(mPlayer);
    }

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
    }
}