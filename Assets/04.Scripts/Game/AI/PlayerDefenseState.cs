using AI;
using GameStruct;

public class PlayerDefenseState : State<EPlayerAIState, EGameMsg>
{
    public override EPlayerAIState ID
    {
        get { return EPlayerAIState.Defense; }
    }

    private readonly PlayerBehaviour mPlayer;
    private AISkillJudger mSkillJudger;

    public PlayerDefenseState(PlayerBehaviour player)
    {
        mPlayer = player;
    }

    public void Init(PlayerBehaviour[] players)
    {
        mSkillJudger = new AISkillJudger(mPlayer, players, false);
    }

    public override void Enter(object extraInfo)
    {
        if (GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkill.ID))
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

        GameController.Get.AIDefend(mPlayer);
    }

    public override void HandleMessage(Telegram<EGameMsg> msg)
    {
    }
}