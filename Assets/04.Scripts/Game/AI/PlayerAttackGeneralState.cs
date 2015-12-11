using System.Collections.Generic;
using G2;
using GamePlayEnum;
using GameStruct;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    public class PlayerAttackGeneralState : State<PlayerAttackState.EPlayerAttackState, EGameMsg>
    {
        private TTacticalData mTactical;
        private readonly PlayerAI mPlayerAI;
        private readonly PlayerBehaviour mPlayer;
        private AISkillJudger mSkillJudger;

        private readonly CountDownTimer mTimer = new CountDownTimer(2);

        /// <summary>
        /// 進攻狀態會做的行為. 這會根據情況, 用亂數的方式找出一個來做.
        /// </summary>
        private enum EAction
        {
            None, Dunk, Shoot2, Shoot3, Pass, Push, Elbow, FakeShoot,
            MoveDodge // 轉身運球(會閃過對手).
        }
        private readonly WeightedRandomizer<EAction> mRandomizer = new WeightedRandomizer<EAction>();
        /// <summary>
        /// 發生某件事情時, 對應的動作.
        /// </summary>
        private readonly Dictionary<EAction, CommonDelegateMethods.Action> mProbabilityActions = new Dictionary<EAction, CommonDelegateMethods.Action>();

        public override PlayerAttackState.EPlayerAttackState ID
        {
            get { return PlayerAttackState.EPlayerAttackState.General;}
        }

        public PlayerAttackGeneralState([NotNull]PlayerAI playerAI, [NotNull]PlayerBehaviour player)
        {
            mPlayerAI = playerAI;
            mPlayer = player;

            mProbabilityActions.Add(EAction.Dunk, doShoot);
            mProbabilityActions.Add(EAction.Shoot2, doShoot);
            mProbabilityActions.Add(EAction.Shoot3, doShoot);
            mProbabilityActions.Add(EAction.Pass, doPass);
//        mProbabilityActions.Add(EProbability.Push, doPush);
            mProbabilityActions.Add(EAction.Elbow, doElbow);
            mProbabilityActions.Add(EAction.FakeShoot, doFakeShoot);
            mProbabilityActions.Add(EAction.MoveDodge, doMoveDodge);
        }

        public override void Enter(object extraInfo)
        {
//            Debug.LogFormat("PlayerAttackGeneralState.Enter, Player:{0}", mPlayerAI.name);

			if(mSkillJudger != null && mPlayer.Attribute.ActiveSkills.Count > 0) 
			{
				if(GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkills[0].ID))
				{
					TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkills[0].ID];
					mSkillJudger.SetCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
				}
			}

            mTimer.StartAgain();
        }

        public override void Exit()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
            if (msg.Msg == EGameMsg.CoachOrderAttackTactical)
            {
                mTactical = (TTacticalData)msg.ExtraInfo;
//            Debug.LogFormat("HandleMessage, Tactical:{0}", mTactical);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="players"> 該場比賽中, 全部的球員. </param>
        public void Init([NotNull]PlayerBehaviour[] players)
        {
            mSkillJudger = new AISkillJudger(mPlayer, players, true);
        }

        public override void UpdateAI()
        {
            if(!mPlayer.AIing)
                return;

            mTimer.Update(Time.deltaTime);

            if(GameController.Get.BallOwner == mPlayer &&
               mPlayerAI.Team.IsAllOpponentsBehindMe(mPlayerAI.transform.position))
            {
                // 我是持球者, 而且我前方沒有任何人.
                Parent.ChangeState(PlayerAttackState.EPlayerAttackState.FastBreak);
                return;
            }

            // 嘗試放主動技.
			if(mPlayer.Attribute.ActiveSkills.Count > 0) 
			{
				if(mSkillJudger != null && mSkillJudger.IsMatchCondition() && 
                   mPlayer.CanUseActiveSkill(mPlayer.Attribute.ActiveSkills[0]))
				{
					if(GameController.Get.DoSkill(mPlayer, mPlayer.Attribute.ActiveSkills[0]))
					    return; // 主動技真的放成功, 才真的會結束 AI 的判斷.
				}
			}
			
            if(!mPlayer.IsAllShoot)
            {
                if(isBallOwner())
                    tryDoShooting();
                else
                    tryDoPush();

                if(GameController.Get.HasBallOwner)
                    tryMoveByTactical();
                else
                    tryDoPickBall(); // 其實應該要寫一個 PickBallState, 但目前暫時沒寫.
            }
        }

        public override void Update()
        {
        }

        private bool isBallOwner()
        {
            return GameController.Get.BallOwner == mPlayer;
        }

        private bool tryDoShooting()
        {
            if (!GameController.Get.BallOwner)
                return false;

            var probability = randomProbability();
            if(probability == EAction.None)
            {
                if(mTimer.IsTimeUp()) // 是否持球太久, 持球太久, 強制做某個動作.
                {
                    probability = randomSpecialProbability();
//                    Debug.LogFormat("randomSpecialProbability:{0}", probability);
                }
                else 
                    return false;
            }

            mProbabilityActions[probability]();
            mTimer.StartAgain();

            return true;
        }

        private EAction randomProbability()
        {
            mRandomizer.Clear();

            float shootPointDis = MathUtils.Find2DDis(mPlayer.transform.position,
                                        CourtMgr.Get.ShootPoint[mPlayer.Team.GetHashCode()].transform.position);

            var hasDefPlayer = mPlayerAI.Team.HasDefPlayer(mPlayerAI, GameConst.ThreatDistance, 
                                                           GameConst.ThreatAngle);

            // 是否可以灌籃.
            if (shootPointDis <= GameConst.DunkDistance &&
                mPlayer.CheckAnimatorSate(EPlayerState.HoldBall))
                mRandomizer.AddOrUpdate(EAction.Dunk, mPlayer.Attr.DunkRate);

            // 是否可以投 2 分球.
            if(shootPointDis <= GameConst.Point2Distance && !hasDefPlayer &&
               mPlayer.CheckAnimatorSate(EPlayerState.HoldBall))
                mRandomizer.AddOrUpdate(EAction.Shoot2, mPlayer.Attr.PointRate2);

            // 是否可以投 3 分球.(判斷距離 +1 的目的是確保球員真的在三分線外投籃)
            if(shootPointDis <= GameConst.Point3Distance + 1 && !hasDefPlayer &&
               mPlayer.CheckAnimatorSate(EPlayerState.HoldBall))
                mRandomizer.AddOrUpdate(EAction.Shoot3, mPlayer.Attr.PointRate3);

            // 是否可以做假動作
            if(shootPointDis <= GameConst.Point3Distance + 1 && hasDefPlayer &&
               !mPlayer.CheckAnimatorSate(EPlayerState.HoldBall))
            {
                mRandomizer.AddOrUpdate(EAction.FakeShoot, GameConst.FakeShootRate);
            }

            // 是否可以用 Elbow 攻擊對方.(對方必須是 Idle 動作時, 才可以做 Elbow, 主要是避免打不到對方)
            PlayerAI defPlayer;
            var stealThreat = mPlayerAI.Team.FindDefPlayer(mPlayerAI, GameConst.StealPushDistance, 160, out defPlayer);
            if(stealThreat &&
               defPlayer.GetComponent<PlayerBehaviour>().CheckAnimatorSate(EPlayerState.Idle) &&
               mPlayer.ElbowCD.IsTimeUp() && !mPlayer.IsElbow)
            {
                mRandomizer.AddOrUpdate(EAction.Elbow, mPlayer.Attr.ElbowingRate);
            }

            // 是否可以傳球.
            if(mPlayer.CheckAnimatorSate(EPlayerState.HoldBall) &&
//               GameController.Get.CoolDownPass <= 0)
               GameController.Get.PassCD.IsTimeUp())
                mRandomizer.AddOrUpdate(EAction.Pass, mPlayer.Attr.PassRate);

            // 是否可以轉身運球過人.
            if(mPlayer.IsHaveMoveDodge && GameController.Get.CoolDownCrossover <= 0 && mPlayer.CanMove &&
                hasDefPlayer)
            {
                mRandomizer.AddOrUpdate(EAction.MoveDodge, mPlayer.MoveDodgeRate);
            }

//            Debug.Log(mRandomizer);

            if(mRandomizer.IsEmpty())
            {
//            Debug.Log("Randomizer is empty!");
                return EAction.None;
            }
            return mRandomizer.GetNext();
        }

        private EAction randomSpecialProbability()
        {
            mRandomizer.Clear();

            float shootPointDis = MathUtils.Find2DDis(mPlayer.transform.position,
                                        CourtMgr.Get.ShootPoint[mPlayer.Team.GetHashCode()].transform.position);

            // 是否可以灌籃.
            if(shootPointDis <= GameConst.DunkDistance)
                mRandomizer.AddOrUpdate(EAction.Dunk, mPlayer.Attr.DunkRate);

            // 是否可以投 2 分球.
            if(shootPointDis <= GameConst.Point2Distance)
                mRandomizer.AddOrUpdate(EAction.Shoot2, mPlayer.Attr.PointRate2);

            // 是否可以投 3 分球.(判斷距離 +1 的目的是確保球員真的在三分線外投籃)
            if(shootPointDis <= GameConst.Point3Distance + 1)
                mRandomizer.AddOrUpdate(EAction.Shoot3, mPlayer.Attr.PointRate3);

            // 是否可以做假動作
            if(shootPointDis <= GameConst.Point3Distance + 1)
                mRandomizer.AddOrUpdate(EAction.FakeShoot, GameConst.FakeShootRate);

//            mRandomizer.AddOrUpdate(EAction.Elbow, mPlayer.Attr.ElbowingRate);
            mRandomizer.AddOrUpdate(EAction.Pass, mPlayer.Attr.PassRate);
//            GameController.Get.CoolDownPass = 0;
            GameController.Get.PassCD.StartAgain();

            EAction action = mRandomizer.GetNext();

//            Debug.LogFormat("randomSpecialProbability:{0}, Action:{1}", mRandomizer, action);

            return action;
        }

        private bool tryDoPush()
        {
            // 參數 player 並未持球, 所以只能做 Push 被動技.
            // 這裡的企劃規則是, 附近的敵對球員必須是 Idle 狀態時, 才會真的執行推人行為.
            var nearPlayer = mPlayerAI.Team.FindNearestOpponentPlayer(mPlayerAI.transform.position);
            if (nearPlayer == null)
                return false;

            bool pushRate = Random.Range(0, 100) < mPlayer.Attr.PushingRate;
            bool isClose = MathUtils.Find2DDis(nearPlayer.transform.position, mPlayerAI.transform.position) <= GameConst.StealPushDistance;
            if (isClose && pushRate && mPlayer.PushCD.IsTimeUp() &&
               nearPlayer.GetComponent<PlayerBehaviour>().CheckAnimatorSate(EPlayerState.Idle))
            {
                if(mPlayer.DoPassiveSkill(ESkillSituation.Push0, nearPlayer.transform.position))
                {
                    mPlayer.PushCD.StartAgain();
                    return true;
                }
            }

            return false;
        }

        private void doShoot()
        {
            GameController.Get.DoShoot();
        }

        private void doFakeShoot()
        {
            mPlayer.AniState(EPlayerState.FakeShoot, CourtMgr.Get.ShootPoint[mPlayer.Team.GetHashCode()].transform.position);
        }

        private void doElbow()
        {
            if(mPlayer.DoPassiveSkill(ESkillSituation.Elbow0))
            {
                mPlayer.ElbowCD.StartAgain();
//                CourtMgr.Get.ShowBallSFX(GameConst.BallSFXTime);
                CourtMgr.Get.ShowBallSFX(mPlayer.Attr.PunishTime);
            }
        }

//    private void doPush()
//    {
//    }

        private void doPass()
        {
            GameController.Get.AIPass(mPlayer);
            GameController.Get.PassCD.StartAgain();
        }

        private void doMoveDodge()
        {
            mPlayer.DoPassiveSkill(ESkillSituation.MoveDodge);
        }

        public void tryMoveByTactical()
        {
            if(mPlayer.CanMove && mPlayer.TargetPosNum == 0)
            {
                // 該球員目前沒有任何移動位置.

                // 對同隊的其它隊友重置位置, 也就是同隊隊友若是目前有移動路徑, 會清掉.
                // 但為什麼要這樣做呢? 應該是當某位球員的戰術跑完後, 其它全部的人即使還未跑完,
                // 也必須要將戰術位置清掉, 重新指定新的戰術位置.
                for(int i = 0; i < GameController.Get.GamePlayers.Count; i++)
                {
                    if(GameController.Get.GamePlayers[i].Team == mPlayer.Team && 
                        GameController.Get.GamePlayers[i] != mPlayer &&
                        mTactical.FileName != string.Empty && 
                        GameController.Get.GamePlayers[i].TargetPosName != mTactical.FileName)
                        GameController.Get.GamePlayers[i].ResetMove();
                }

                // 以下會將戰術的每個跑位點都設定 PlayerBehavior.
                if(mTactical.FileName != string.Empty)
                {
                    var tacticalActions = mTactical.GetActions(mPlayer.Postion);

                    if(tacticalActions != null)
                    {
                        for (int i = 0; i < tacticalActions.Length; i++)
                        {
                            TMoveData moveData = new TMoveData();
                            moveData.Clear();
                            moveData.Speedup = tacticalActions[i].Speedup;
                            moveData.Catcher = tacticalActions[i].Catcher;
                            moveData.Shooting = tacticalActions[i].Shooting;
                            int z = 1;

                            if(GameStart.Get.CourtMode == ECourtMode.Full && mPlayer.Team != ETeamKind.Self)
                                z = -1;

                            moveData.SetTarget(tacticalActions[i].x, tacticalActions[i].z * z);

                            if(GameController.Get.BallOwner != mPlayer)
                                moveData.LookTarget = GameController.Get.BallOwner.transform;

                            moveData.TacticalName = mTactical.FileName;
                            moveData.MoveFinish = GameController.Get.MoveDefPlayer;
                            mPlayer.TargetPos = moveData;
                        }

                        GameController.Get.MoveDefPlayer(mPlayer);
                    }
                }
            }

            if(mPlayer.CantMoveTimer.IsOn()&&
                mPlayer == GameController.Get.BallOwner)
                mPlayer.AniState(EPlayerState.Dribble0);
        }

        private void tryDoPickBall()
        {
            // 沒人有球, 所以要開始搶球. 最接近球的球員去撿球就好.
            if(!GameController.Get.Passer)
            {
                if(GameController.Get.Shooter == null)
                {
                    GameController.Get.NearestBallPlayerDoPickBall(mPlayer);
                    if(mPlayer.DefPlayer != null)
                        GameController.Get.NearestBallPlayerDoPickBall(mPlayer.DefPlayer);
                }
                else
                {
                    // 投籃者出手, 此時球在空中飛行.
                    if((GameController.Get.Situation == EGameSituation.AttackGamer && mPlayer.Team == ETeamKind.Self) ||
                       (GameController.Get.Situation == EGameSituation.AttackNPC && mPlayer.Team == ETeamKind.Npc))
                    {
                        if(!mPlayer.IsShoot)
                            GameController.Get.NearestBallPlayerDoPickBall(mPlayer);
                        if(mPlayer.DefPlayer != null)
                            GameController.Get.NearestBallPlayerDoPickBall(mPlayer.DefPlayer);
                    }
                }
            }
        }
    } // end of the class PlayerAttackGeneralState.

} // end of the namespace AI.


