using GameStruct;

namespace AI
{
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
            if (mPlayer.Attribute.ActiveSkills.Count > 0)
            {
                if (GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkills[0].ID))
                {
                    TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkills[0].ID];
                    mSkillJudger.SetCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
                }
            }
            //        if (GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkill.ID))
            //        {
            //            TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkill.ID];
            //            mSkillJudger.SetCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
            //        }
        }

        public override void Exit()
        {
        }

        public override void UpdateAI()
        {
            if (!mPlayer.AIing)
                return;

            if (mPlayer.Attribute.ActiveSkills.Count > 0)
            {
                if (mSkillJudger.IsMatchCondition() && mPlayer.CanUseActiveSkill(mPlayer.Attribute.ActiveSkills[0]))
                {
                    GameController.Get.DoSkill(mPlayer, mPlayer.Attribute.ActiveSkills[0]);
                    return;
                }
            }

            //        if(mSkillJudger.IsMatchCondition() && mPlayer.CanUseActiveSkill)
            //        {
            //            GameController.Get.DoSkill(mPlayer);
            //            return;
            //        }

            GameController.Get.AIDefend(mPlayer);
        }

//        public override void Update()
//        {
//        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    } // end of the class PlayerDefenseState.
} // end of the namespace AI.
