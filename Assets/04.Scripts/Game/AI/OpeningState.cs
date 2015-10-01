using AI;

public class OpeningState : State<EGameSituation, EGameMsg>
{
    public override EGameSituation ID
    {
        get { return EGameSituation.Opening; }
    }

    public override void Enter(object extraInfo)
    {
        foreach(var player in GameController.Get.GamePlayers)
        {
            player.GetComponent<PlayerAI>().ChangeState(EPlayerAIState.None);
        }

        //        setPassIcon(true);
        UIGame.UIShow(true);
        UIGame.Get.UIState(EUISituation.Opening);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
    }
}