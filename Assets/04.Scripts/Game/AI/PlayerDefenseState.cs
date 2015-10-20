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
            if (mPlayer.Attribute.ActiveSkills.Count > 0)
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
                if(mSkillJudger.IsMatchCondition() && 
                   mPlayer.CanUseActiveSkill(mPlayer.Attribute.ActiveSkills[0]))
                {
                    GameController.Get.DoSkill(mPlayer, mPlayer.Attribute.ActiveSkills[0]);
                    return;
                }
            }

//            GameController.Get.AIDefend(mPlayer);

            doDefenseAction();
        }

        private void doDefenseAction()
        {
//            if(mPlayer.IsSteal || mPlayer.IsPush || GameController.Get.IsDunk ||
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
//                player.CoolDownPush = Time.time + GameConst.CoolDownPushTime;
                mPlayer.PushCD.StartAgain();
            }
        }

        private void doSteal()
        {
            if(mPlayer.DoPassiveSkill(ESkillSituation.Steal0, GameController.Get.BallOwner.transform.position))
            {
//                player.CoolDownSteal = Time.time + GameConst.CoolDownStealTime;
//                mStealCDTimer.StartAgain();
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

//        public void AIDefend([NotNull] PlayerBehaviour player)
//        {
//            if(!player.IsSteal && !player.IsPush &&
//                BallOwner && !IsDunk && !IsShooting)
//            {
//                bool pushRate = Random.Range(0, 100) < player.Attr.PushingRate;
//                bool sucess = false;
//
//                TPlayerDisData[] disAy = findPlayerDisData(player);
//
//                for (int i = 0; i < disAy.Length; i++)
//                {
//                    if (disAy[i].Distance <= GameConst.StealPushDistance &&
//                        (disAy[i].Player.crtState == EPlayerState.Idle || disAy[i].Player.crtState == EPlayerState.Dribble0) &&
//                        pushRate && player.CoolDownPush == 0)
//                    {
//                        if (player.DoPassiveSkill(ESkillSituation.Push0, disAy[i].Player.transform.position))
//                        {
//                            player.CoolDownPush = Time.time + GameConst.CoolDownPushTime;
//                            sucess = true;
//
//                            break;
//                        }
//                    }
//                }
//
//                if (!sucess && disAy[0].Distance <= GameConst.StealPushDistance &&
//                    //                waitStealTime == 0 && 
//                    mStealCDTimer.IsTimeUp() &&
//                    BallOwner.Invincible.IsOff() &&
//                    player.CoolDownSteal == 0)
//                {
//                    if (Random.Range(0, 100) < player.Attr.StealRate)
//                    {
//                        if (player.DoPassiveSkill(ESkillSituation.Steal0, BallOwner.gameObject.transform.position))
//                        {
//                            player.CoolDownSteal = Time.time + GameConst.CoolDownStealTime;
//                            //						waitStealTime = Time.time + GameConst.WaitStealTime;
//                            mStealCDTimer.StartAgain();
//                        }
//                    }
//                }
//            }
//        }
    } // end of the class PlayerDefenseState.
} // end of the namespace AI.
