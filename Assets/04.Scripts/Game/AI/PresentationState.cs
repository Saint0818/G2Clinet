using AI;
using UnityEngine;

public class PresentationState : State<EGameSituation, EGameMsg>, ITelegraph<EGameMsg>
{
    public override void EnterImpl(object extraInfo)
    {
        Dispatcher.AddListener(this, EGameMsg.UISkipClickOnGaming);

//        foreach(PlayerBehaviour player in GameController.Get.GamePlayerList)
//        {
//            ModelManager.Get.ChangeAnimator(player.AnimatorControl,
//                                            player.Attribute.BodyType.ToString(),
//                                            EanimatorType.ShowControl);
//        }
        UISkip.UIShow(true, ESkipSituation.Game);
    }

    public override void Exit()
    {
        Dispatcher.RemoveListener(this, EGameMsg.UISkipClickOnGaming);
    }

    public override void Update()
    {
        foreach(PlayerBehaviour player in GameController.Get.GamePlayers)
        {
            if(player.ShowPos != -1)
            {
                player.gameObject.transform.position = CameraMgr.Get.CharacterPos[player.ShowPos].transform.position;
                player.gameObject.transform.eulerAngles = CameraMgr.Get.CharacterPos[player.ShowPos].transform.eulerAngles;
            }
        }
    }

    public void HandleMessage(Telegram<EGameMsg> msg)
    {
        if(msg.Msg == EGameMsg.UISkipClickOnGaming)
        {
//            Debug.Log(msg);

            CourtMgr.Get.ShowEnd(true);
            GameController.Get.InitIngameAnimator();
            GameController.Get.SetBornPositions();
        }
    }
}
