using AI;

public class InitShowControlState : State<EGameSituation>, ITelegraph<EGameSituation>
{
    public override void EnterImpl()
    {
        //        isSkip = false;
        foreach(PlayerBehaviour player in GameController.Get.GamePlayerList)
        {
            ModelManager.Get.ChangeAnimator(player.AnimatorControl,
                                            player.Attribute.BodyType.ToString(),
                                            EanimatorType.ShowControl);
        }
        UISkip.UIShow(true, ESkipSituation.Game);
    }

    public override void Update()
    {
        foreach(PlayerBehaviour player in GameController.Get.GamePlayerList)
        {
            if(player.ShowPos != -1 /*&& isSkip == false*/)
            {
                player.gameObject.transform.position = CameraMgr.Get.CharacterPos[player.ShowPos].transform.position;
                player.gameObject.transform.eulerAngles = CameraMgr.Get.CharacterPos[player.ShowPos].transform.eulerAngles;
            }
        }
    }

    public override void Exit()
    {
    }

    public void HandleMessage(Telegram<EGameSituation> msg)
    {
        
    }
}
