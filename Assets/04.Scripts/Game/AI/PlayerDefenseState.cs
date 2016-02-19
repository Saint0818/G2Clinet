using JetBrains.Annotations;

namespace AI
{
    public class PlayerDefenseState : State<EPlayerAIState, EGameMsg>
    {
        public override EPlayerAIState ID
        {
            get { return EPlayerAIState.Defense; }
        }

        private readonly PlayerBehaviour mPlayer;
//        private AISkillJudger mSkillJudger;

        private readonly StartSkillAction mStartSkillAction;

        private readonly ActionRandomizer mActions = new ActionRandomizer();

        public PlayerDefenseState([NotNull]PlayerAI playerAI, [NotNull] PlayerBehaviour player)
        {
            mPlayer = player;

            mStartSkillAction = new StartSkillAction(mPlayer);

            mActions.Add(new CloseDefPlayerAction(playerAI, mPlayer));
            mActions.Add(new PushAction(playerAI, mPlayer));
            mActions.Add(new StealAction(playerAI, mPlayer));
        }

        public void Init(PlayerBehaviour[] players)
        {
//            mSkillJudger = new AISkillJudger(mPlayer, false, players);
            mStartSkillAction.Init(players, false);
        }

        public override void Enter(object extraInfo)
        {
            mPlayer.ResetMove();
//			if (mSkillJudger != null && mPlayer.Attribute.ActiveSkills.Count > 0)
//            {
//                if (GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkills[0].ID))
//                {
//                    TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkills[0].ID];
//                    mSkillJudger.SetNewCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
//                }
//            }
        }

        public override void Exit()
        {
        }

        public override void UpdateAI()
        {
            if(!mPlayer.AIing)
                return;

//            if(mPlayer.Attribute.ActiveSkills.Count > 0)
//            {
//				if(mSkillJudger != null && mSkillJudger.IsMatchCondition() && 
//                   mPlayer.CanUseActiveSkill(mPlayer.Attribute.ActiveSkills[0]))
//				{
//				    if(GameController.Get.DoSkill(mPlayer, mPlayer.Attribute.ActiveSkills[0]))
//                        return; // 真的有做主動技, 才真的結束 AI 的判斷.
//				}
//            }

            if(mStartSkillAction.Do())
                return; // 真的有做主動技, 會結束此次的 AI 判斷.

            doDefenseAction();

//            GameController.Get.MoveDefPlayer(mPlayer.DefPlayer);
        }

        public override void Update()
        {
        }

        private void doDefenseAction()
        {
            if(GameController.Get.IsDunk || GameController.Get.IsShooting)
                return;

            mActions.Do();
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    } // end of the class PlayerDefenseState.
} // end of the namespace AI.
