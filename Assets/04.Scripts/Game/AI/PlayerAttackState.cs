using AI;
using GamePlayStruct;
using GameStruct;
using JetBrains.Annotations;

public class PlayerAttackState : State<EPlayerAIState, EGameMsg>
{
    public override EPlayerAIState ID
    {
        get { return EPlayerAIState.Attack; }
    }

    private TTacticalData mTactical;
    private readonly PlayerBehaviour mPlayer;
    private AISkillJudger mSkillJudger;

    public PlayerAttackState(PlayerBehaviour player)
    {
        mPlayer = player;
    }

    public void Init(PlayerBehaviour[] players)
    {
        mSkillJudger = new AISkillJudger(mPlayer, players, true);
    }

    public override void Enter(object extraInfo)
    {
        if(GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkill.ID))
        {
            TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkill.ID];
            mSkillJudger.SetCondition(skill.Situation, 0);
        }
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if(!mPlayer.AIing)
            return;

        if(mSkillJudger.IsMatchCondition() && mPlayer.CanUseActiveSkill)
        {
            GameController.Get.DoSkill(mPlayer);
            return;
        }

        if(!GameController.Get.IsShooting || !mPlayer.IsAllShoot)
        {
            GameController.Get.AIAttack(mPlayer);
            GameController.Get.AIMove(mPlayer, ref mTactical);
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
