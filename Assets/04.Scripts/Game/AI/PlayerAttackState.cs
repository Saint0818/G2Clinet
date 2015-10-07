using System;
using System.Collections.Generic;
using AI;
using G2;
using GamePlayEnum;
using GamePlayStruct;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    /// <summary>
    /// 球員在進攻狀態.
    /// </summary>
    public class PlayerAttackState : State<EPlayerAIState, EGameMsg>
    {
        public override EPlayerAIState ID
        {
            get { return EPlayerAIState.Attack; }
        }

        public enum EPlayerAttackState
        {
            None, 
            FastBreak, // 快攻.
            General // 尚未準確分類...
        }

        //    private TTacticalData mTactical;
        private readonly PlayerBehaviour mPlayer;
        //    private AISkillJudger mSkillJudger;

        //    /// <summary>
        //    /// 進攻狀態會做的行為. 這會根據情況, 用亂數的方式找出一個來做.
        //    /// </summary>
        //    private enum EAction
        //    {
        //        None, Dunk, Shoot2, Shoot3, Pass, Push, Elbow, FakeShoot,
        //        MoveDodge // 轉身運球(會閃過對手).
        //    }
        //    private readonly WeightedRandomizer<EAction> mRandomizer = new WeightedRandomizer<EAction>();
        //    /// <summary>
        //    /// 發生某件事情時, 對應的動作.
        //    /// </summary>
        //    private readonly Dictionary<EAction, CommonDelegateMethods.Action> mProbabilityActions = new Dictionary<EAction, CommonDelegateMethods.Action>();

        private readonly PlayerAI mPlayerAI;

        private readonly StateMachine<EPlayerAttackState, EGameMsg> mFSM = new StateMachine<EPlayerAttackState, EGameMsg>();

        public PlayerAttackState([NotNull]PlayerAI playerAI, [NotNull] PlayerBehaviour player)
        {
            mFSM.AddState(new PlayerAttackNoneState());
            mFSM.AddState(new PlayerAttackGeneralState(playerAI, player));
            mFSM.AddState(new PlayerAttackFastBreakState());
            mFSM.ChangeState(EPlayerAttackState.None);

            mPlayer = player;
            mPlayerAI = playerAI;

            //        mProbabilityActions.Add(EAction.Dunk, doShoot);
            //        mProbabilityActions.Add(EAction.Shoot2, doShoot);
            //        mProbabilityActions.Add(EAction.Shoot3, doShoot);
            //        mProbabilityActions.Add(EAction.Pass, doPass);
            ////        mProbabilityActions.Add(EProbability.Push, doPush);
            //        mProbabilityActions.Add(EAction.Elbow, doElbow);
            //        mProbabilityActions.Add(EAction.FakeShoot, doFakeShoot);
            //        mProbabilityActions.Add(EAction.MoveDodge, doMoveDodge);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="players"> 該場比賽中, 全部的球員. </param>
        public void Init([NotNull]PlayerBehaviour[] players)
        {
//            mSkillJudger = new AISkillJudger(mPlayer, players, true);
            var general = (PlayerAttackGeneralState)mFSM[EPlayerAttackState.General];
            general.Init(players);
        }

        public override void Enter(object extraInfo)
        {
            //        if(GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkill.ID))
            //        {
            //            TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkill.ID];
            //            mSkillJudger.SetCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
            //        }
            mFSM.ChangeState(EPlayerAttackState.General);
        }

        public override void Exit()
        {
            mFSM.ChangeState(EPlayerAttackState.None);
        }

        public override void Update()
        {
            mFSM.Update();

            //        if(!mPlayer.AIing)
            //            return;
            //
            //        if(mSkillJudger.IsMatchCondition() && mPlayer.CanUseActiveSkill)
            //        {
            //            GameController.Get.DoSkill(mPlayer);
            //            return;
            //        }
            //
            //        if(!mPlayer.IsAllShoot)
            //        {
            ////            bool isDoingAction;
            //            if(isBallOwner())
            ////                isDoingAction = tryDoShooting();
            //                tryDoShooting();
            //            else
            ////                isDoingAction = tryDoPush();
            //                tryDoPush();
            //
            ////            GameController.Get.AIAttack(mPlayer);
            ////            if(!isDoingAction)
            //            moveByTactical(mPlayer, ref mTactical);
            //        }
        }

        //    private bool isBallOwner()
        //    {
        //        return GameController.Get.BallOwner == mPlayer;
        //    }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
            mFSM.CurrentState.HandleMessage(msg);

//            if (msg.Msg == EGameMsg.CoachOrderAttackTactical)
//            {
//                var tactical = (TTacticalData)msg.ExtraInfo;
//                
//    //            Debug.LogFormat("HandleMessage, Tactical:{0}", mTactical);
//            }
        }

        //    private bool tryDoShooting()
        //    {
        //        if(!GameController.Get.BallOwner)
        //            return false;
        //
        //        var probability = randomProbability();
        //        if(probability == EAction.None)
        //            return false;
        //
        //        mProbabilityActions[probability]();
        //
        //        return true;
        //    }
        //
        //    private EAction randomProbability()
        //    {
        //        mRandomizer.Clear();
        //
        //        float shootPointDis = MathUtils.Find2DDis(mPlayer.transform.position,
        //                                    CourtMgr.Get.ShootPoint[mPlayer.Team.GetHashCode()].transform.position);
        //
        //        var threat = mPlayerAI.Team.HasDefPlayer(mPlayerAI, GameConst.ThreatDistance, GameConst.ThreatAngle);
        //
        //        // 是否可以灌籃.
        //        if (shootPointDis <= GameConst.DunkDistance &&
        //            mPlayer.CheckAnimatorSate(EPlayerState.HoldBall) /*&& Team.IsInUpfield(mPlayer)*/)
        //            mRandomizer.AddOrUpdate(EAction.Dunk, mPlayer.Attr.DunkRate);
        //
        //        // 是否可以投 2 分球.
        //        if (shootPointDis <= GameConst.TwoPointDistance &&
        //                threat != Team.EFindPlayerResult.InFront &&
        //                mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)
        //                /*Team.IsInUpfield(mPlayer)*/)
        //            mRandomizer.AddOrUpdate(EAction.Shoot2, mPlayer.Attr.PointRate2);
        //
        //        // 是否可以投 3 分球.
        //        if(shootPointDis <= GameConst.TreePointDistance + 1 &&
        //                threat != Team.EFindPlayerResult.InFront &&
        //                mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)
        //                /*Team.IsInUpfield(mPlayer)*/)
        //            mRandomizer.AddOrUpdate(EAction.Shoot3, mPlayer.Attr.PointRate3);
        //
        //        // 是否可以做假動作
        //        if(shootPointDis <= GameConst.TreePointDistance + 1 &&
        //           !mPlayer.CheckAnimatorSate(EPlayerState.HoldBall) && 
        //           threat == Team.EFindPlayerResult.InFront)
        //        {
        //            mRandomizer.AddOrUpdate(EAction.FakeShoot, GameConst.FakeShootRate);
        //        }
        //
        //        // 是否可以用 Elbow 攻擊對方.
        //        PlayerAI defPlayer;
        //        var stealThreat = mPlayerAI.Team.FindDefPlayer(mPlayerAI, GameConst.StealBallDistance, 90, out defPlayer);
        //        if(Team.IsInUpfield(mPlayer) &&
        //           stealThreat != Team.EFindPlayerResult.CannotFound && 
        //           defPlayer.GetComponent<PlayerBehaviour>().CheckAnimatorSate(EPlayerState.Idle) &&
        //           mPlayer.CoolDownElbow == 0 && !mPlayer.CheckAnimatorSate(EPlayerState.Elbow))
        //        {
        //            mRandomizer.AddOrUpdate(EAction.Elbow, mPlayer.Attr.ElbowingRate);
        //        }
        //
        //        // 是否可以傳球.
        //        if ((mPlayer.CheckAnimatorSate(EPlayerState.HoldBall)) &&
        //           GameController.Get.coolDownPass == 0 && /*!IsShooting && !IsDunk &&*/
        //           !mPlayer.CheckAnimatorSate(EPlayerState.Elbow))
        //            mRandomizer.AddOrUpdate(EAction.Pass, mPlayer.Attr.PassRate);
        //
        //        // 是否可以轉身運球過人.
        //        if (mPlayer.IsHaveMoveDodge && GameController.Get.CoolDownCrossover == 0 && mPlayer.CanMove &&
        //            threat != Team.EFindPlayerResult.CannotFound)
        //        {
        //            mRandomizer.AddOrUpdate(EAction.MoveDodge, mPlayer.MoveDodgeRate);
        //        }
        //
        //        if(mRandomizer.IsEmpty())
        //        {
        ////            Debug.Log("Randomizer is empty!");
        //            return EAction.None;
        //        }
        //        return mRandomizer.GetNext();
        //    }
        //
        //    private bool tryDoPush()
        //    {
        //        // 參數 player 並未持球, 所以只能做 Push 被動技.
        //        // 這裡的企劃規則是, 附近的敵對球員必須是 Idle 狀態時, 才會真的執行推人行為.
        //        var nearPlayer = mPlayerAI.Team.FindNearestOpponentPlayer(mPlayerAI.transform.position);
        //        if(nearPlayer == null)
        //            return false;
        //
        //        bool pushRate = Random.Range(0, 100) < mPlayer.Attr.PushingRate;
        //        bool isClose = MathUtils.Find2DDis(nearPlayer.transform.position, mPlayerAI.transform.position) <= GameConst.StealBallDistance;
        //        if(isClose && pushRate && Math.Abs(mPlayer.CoolDownPush) < float.Epsilon &&
        //           nearPlayer.GetComponent<PlayerBehaviour>().CheckAnimatorSate(EPlayerState.Idle))
        //        {
        //            if(mPlayer.DoPassiveSkill(ESkillSituation.Push0, nearPlayer.transform.position))
        //            {
        //                mPlayer.CoolDownPush = Time.time + GameConst.CoolDownPushTime;
        //                return true;
        //            }
        //        }
        //
        //        return false;
        //    }
        //
        //    private void doShoot()
        //    {
        //        GameController.Get.DoShoot();
        //    }
        //
        //    private void doFakeShoot()
        //    {
        //        mPlayer.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint[mPlayer.Team.GetHashCode()].transform.position);
        //    }
        //
        //    private void doElbow()
        //    {
        ////        PlayerAI defPlayer;
        ////        GameController.Get.HaveDefPlayer(mPlayer, GameConst.StealBallDistance, 90, out defPlayer);
        ////        mPlayerAI.Team.FindDefPlayer(mPlayerAI, GameConst.StealBallDistance, 90, out defPlayer);
        ////        if(mPlayer.DoPassiveSkill(ESkillSituation.Elbow, defPlayer.transform.position))
        //        if(mPlayer.DoPassiveSkill(ESkillSituation.Elbow))
        //        {
        //            GameController.Get.coolDownPass = 0;
        //            mPlayer.CoolDownElbow = Time.time + 3;
        //            GameController.Get.RealBallFxTime = 1f;
        //            CourtMgr.Get.RealBallFX.SetActive(true);
        //        }
        //    }
        //
        ////    private void doPush()
        ////    {
        ////    }
        //
        //    private void doPass()
        //    {
        //        GameController.Get.AIPass(mPlayer);
        //    }
        //
        //    private void doMoveDodge()
        //    {
        //        mPlayer.DoPassiveSkill(ESkillSituation.MoveDodge);
        //    }
        //
        //    public void moveByTactical([NotNull] PlayerBehaviour someone, ref TTacticalData tacticalData)
        //    {
        //        if(GameController.Get.BallOwner == null)
        //        {
        //            // 沒人有球, 所以要開始搶球. 最接近球的球員去撿球就好.
        //            if (!GameController.Get.Passer)
        //            {
        //                if (GameController.Get.Shooter == null)
        //                {
        //                    GameController.Get.NearestBallPlayerDoPickBall(someone);
        //                    if (someone.DefPlayer != null)
        //                        GameController.Get.NearestBallPlayerDoPickBall(someone.DefPlayer);
        //                }
        //                else
        //                {
        //                    // 投籃者出手, 此時球在空中飛行.
        //                    if ((GameController.Get.Situation == EGameSituation.AttackA && someone.Team == ETeamKind.Self) ||
        //                       (GameController.Get.Situation == EGameSituation.AttackB && someone.Team == ETeamKind.Npc))
        //                    {
        //                        if (!someone.IsShoot)
        //                            GameController.Get.NearestBallPlayerDoPickBall(someone);
        //                        if (someone.DefPlayer != null)
        //                            GameController.Get.NearestBallPlayerDoPickBall(someone.DefPlayer);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // 有人有球.
        //            if(someone.CanMove && someone.TargetPosNum == 0)
        //            {
        //                // 該球員目前沒有任何移動位置.
        //                for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
        //                {
        //                    if (GameController.Get.GamePlayers[i].Team == someone.Team && GameController.Get.GamePlayers[i] != someone &&
        //                       tacticalData.FileName != string.Empty && GameController.Get.GamePlayers[i].TargetPosName != tacticalData.FileName)
        //                        GameController.Get.GamePlayers[i].ResetMove();
        //                }
        //
        //                if (tacticalData.FileName != string.Empty)
        //                {
        //                    var tacticalActions = tacticalData.GetActions(someone.Postion.GetHashCode());
        //
        //                    if (tacticalActions != null)
        //                    {
        //                        for (int i = 0; i < tacticalActions.Length; i++)
        //                        {
        //                            TMoveData moveData = new TMoveData();
        //                            moveData.Clear();
        //                            moveData.Speedup = tacticalActions[i].Speedup;
        //                            moveData.Catcher = tacticalActions[i].Catcher;
        //                            moveData.Shooting = tacticalActions[i].Shooting;
        //                            int z = 1;
        //
        //                            if (GameStart.Get.CourtMode == ECourtMode.Full && someone.Team != ETeamKind.Self)
        //                                z = -1;
        //
        //                            moveData.Target = new Vector2(tacticalActions[i].x, tacticalActions[i].z * z);
        //                            if (GameController.Get.BallOwner != null && GameController.Get.BallOwner != someone)
        //                                moveData.LookTarget = GameController.Get.BallOwner.transform;
        //
        //                            moveData.TacticalName = tacticalData.FileName;
        //                            moveData.MoveFinish = GameController.Get.DefMove;
        //                            someone.TargetPos = moveData;
        //                        }
        //
        //                        GameController.Get.DefMove(someone);
        //                    }
        //                }
        //            }
        //
        //            if (someone.WaitMoveTime != 0 && GameController.Get.BallOwner != null && someone == GameController.Get.BallOwner)
        //                someone.AniState(EPlayerState.Dribble0);
        //        }
        //    }
    } // end of the class PlayerAttackState.

} // end of the namespace AI.


