using System.Collections.Generic;
using G2;
using GamePlayEnum;
using GameStruct;
using JetBrains.Annotations;

namespace AI
{
    public class PlayerDefenseState : State<EPlayerAIState, EGameMsg>
    {
        public override EPlayerAIState ID
        {
            get { return EPlayerAIState.Defense; }
        }

        private enum EAction
        {
            None, Push, Steal
        }

        private readonly PlayerAI mPlayerAI;
        private readonly PlayerBehaviour mPlayer;
        private AISkillJudger mSkillJudger;

        private readonly WeightedRandomizer<EAction> mRandomizer = new WeightedRandomizer<EAction>();
        private readonly Dictionary<EAction, CommonDelegateMethods.Action> mActions = new Dictionary<EAction, CommonDelegateMethods.Action>();

        public PlayerDefenseState([NotNull]PlayerAI playerAI, [NotNull] PlayerBehaviour player)
        {
            mPlayerAI = playerAI;
            mPlayer = player;

            mActions.Add(EAction.Push, doPush);
            mActions.Add(EAction.Steal, doSteal);
        }

        public void Init(PlayerBehaviour[] players)
        {
            mSkillJudger = new AISkillJudger(mPlayer, players, false);
        }

        public override void Enter(object extraInfo)
        {
			if (mSkillJudger != null && mPlayer.Attribute.ActiveSkills.Count > 0)
            {
                if (GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkills[0].ID))
                {
                    TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkills[0].ID];
                    mSkillJudger.SetCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
                }
            }
        }

        public override void Exit()
        {
        }

        public override void UpdateAI()
        {
            if(!mPlayer.AIing)
                return;

            if(mPlayer.Attribute.ActiveSkills.Count > 0)
            {
				if(mSkillJudger != null && mSkillJudger.IsMatchCondition() && 
                   mPlayer.CanUseActiveSkill(mPlayer.Attribute.ActiveSkills[0]))
				{
				    if(GameController.Get.DoSkill(mPlayer, mPlayer.Attribute.ActiveSkills[0]))
                        return; // 真的有做主動技, 才真的結束 AI 的判斷.
				    
				}
            }

            doDefenseAction();
        }

        private void doDefenseAction()
        {
            if(GameController.Get.IsDunk || GameController.Get.IsShooting)
                return;

            var action = randomAction();
            if(action == EAction.None)
                return;

//            Debug.LogFormat("Name:{0}, Action:{1}", mPlayerAI.name, action);
            mActions[action]();
        }

        private void doPush()
        {
            var oppPlayerAI = mPlayerAI.FindNearestOpponentPlayer();
            if(mPlayer.DoPassiveSkill(ESkillSituation.Push0, oppPlayerAI.transform.position))
            {
                mPlayer.PushCD.StartAgain();
            }
        }

        private void doSteal()
        {
            if(mPlayer.DoPassiveSkill(ESkillSituation.Steal0, GameController.Get.BallOwner.transform.position))
            {
                mPlayer.StealCD.StartAgain();
            }
        }

        private EAction randomAction()
        {
            mRandomizer.Clear();

            // todo 這只是暫時的設定, 只是希望降低 Push, Steal 的發生機率.
            mRandomizer.AddOrUpdate(EAction.None, 30); 

            var oppPlayerAI = mPlayerAI.FindNearestOpponentPlayer();
            if(oppPlayerAI != null)
            {
                var oppPlayer = mPlayerAI.GetComponent<PlayerBehaviour>();
                
                if(/*(oppPlayer.IsIdle || oppPlayer.IsDribble) &&*/
                    oppPlayer.PushCD.IsTimeUp() &&
                   MathUtils.Find2DDis(mPlayerAI.transform.position, oppPlayerAI.transform.position) <= GameConst.StealPushDistance)
                {
                    mRandomizer.AddOrUpdate(EAction.Push, mPlayer.Attr.PushingRate);
                }
            }
            
            if(GameController.Get.BallOwner != null &&
               mPlayer.StealCD.IsTimeUp() &&
               MathUtils.Find2DDis(mPlayerAI.transform.position, GameController.Get.BallOwner.transform.position) <= GameConst.StealPushDistance)
            {
                mRandomizer.AddOrUpdate(EAction.Steal, mPlayer.Attr.StealRate);
            }

            if(mRandomizer.IsEmpty())
                return EAction.None;

            return mRandomizer.GetNext();
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    } // end of the class PlayerDefenseState.
} // end of the namespace AI.
