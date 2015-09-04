using AI;
using GamePlayStruct;
using UnityEngine;

public class PlayerAttackState : State<EPlayerAIState, EGameMsg>
{
    public override EPlayerAIState ID
    {
        get { return EPlayerAIState.Attack; }
    }

    private TTacticalData mTactical;
    private readonly PlayerBehaviour mPlayer;

    public PlayerAttackState(PlayerBehaviour player)
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
        {
            if(!GameController.Get.IsShooting || !mPlayer.IsAllShoot)
            {
                GameController.Get.AIAttack(mPlayer);
                GameController.Get.AIMove(mPlayer, ref mTactical);
            }
        }
    }

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
        if(msg.Msg == EGameMsg.CoachOrderAttackTactical)
        {
            mTactical = (TTacticalData)msg.ExtraInfo;
//            Debug.LogFormat("HandleMessage, Tactical:{0}", mTactical);
        }
    }
}
