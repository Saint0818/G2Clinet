using GameEnum;
using GameStruct;
using JetBrains.Annotations;

namespace AI
{
    public class PlayerAttackGeneralState : State<PlayerAttackState.EPlayerAttackState, EGameMsg>
    {
        private TTacticalData mTactical;
        private readonly PlayerAI mPlayerAI;
        private readonly PlayerBehaviour mPlayer;
//        private AISkillJudger mSkillJudger;

        /// <summary>
        /// 下一個要發的主動技.
        /// </summary>
//        private int mNextSkillIndex;

        private readonly StartSkillAction mStartSkillAction;

        /// <summary>
        /// 持球行為選擇.
        /// </summary>
        private readonly ActionRandomizer mBallActions = new ActionRandomizer();

        /// <summary>
        /// 未持球行為選擇.
        /// </summary>
        private readonly ActionRandomizer mNoBallActions = new ActionRandomizer();

        public override PlayerAttackState.EPlayerAttackState ID
        {
            get { return PlayerAttackState.EPlayerAttackState.General;}
        }

        public PlayerAttackGeneralState([NotNull]PlayerAI playerAI, [NotNull]PlayerBehaviour player)
        {
            mPlayerAI = playerAI;
            mPlayer = player;

            mStartSkillAction = new StartSkillAction(mPlayer);

            mBallActions.Add(new MoveDodgeAction(mPlayerAI, mPlayer));
            mBallActions.Add(new TacticalAction(mPlayerAI, mPlayer));
            mBallActions.Add(new IdleAction(mPlayerAI, mPlayer, EPlayerState.Dribble0));
            mBallActions.Add(new PassAction(mPlayerAI, mPlayer));
            mBallActions.Add(new Shoot2Action(mPlayerAI, mPlayer));
            mBallActions.Add(new Shoot3Action(mPlayerAI, mPlayer));
            mBallActions.Add(new ElbowAction(mPlayerAI, mPlayer));
            mBallActions.Add(new FakeShootAction(mPlayerAI, mPlayer));

            mNoBallActions.Add(new TacticalAction(mPlayerAI, mPlayer));
            mNoBallActions.Add(new IdleAction(mPlayerAI, mPlayer, EPlayerState.Idle));
            mNoBallActions.Add(new PushAction(mPlayerAI, mPlayer));
        }

        public override void Enter(object extraInfo)
        {
//            mPlayer.ResetMove();
//            Debug.LogFormat("PlayerAttackGeneralState.Enter, Player:{0}", mPlayerAI.name);

//			if(mSkillJudger != null && mPlayer.Attribute.ActiveSkills.Count > 0) 
//			{
//				if(GameData.DSkillData.ContainsKey(mPlayer.Attribute.ActiveSkills[0].ID))
//				{
//					TSkillData skill = GameData.DSkillData[mPlayer.Attribute.ActiveSkills[0].ID];
//					mSkillJudger.SetCondition(skill.Situation, mPlayer.Attribute.AISkillLv);
//				}
//			}

//            mStartSkillAction.UpdateCondition();
        }

        public override void Exit()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
            if(msg.Msg == EGameMsg.CoachOrderAttackTactical)
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
//            mSkillJudger = new AISkillJudger(mPlayer, players, true);
            mStartSkillAction.Init(players, true);
        }

        public override void UpdateAI()
        {
            if(!mPlayer.AIing)
                return;

            // 魔術數字 1.0 的目的是希望球員可以盡可能的變成快攻狀態.
            if(GameController.Get.BallOwner == mPlayer &&
               mPlayerAI.Team.IsAllOpponentsBehindMe(mPlayerAI.transform.position, GameConst.AIFastBreakOffset))
            {
                // 我是持球者, 而且我前方沒有任何人.
                Parent.ChangeState(PlayerAttackState.EPlayerAttackState.FastBreak);
                return;
            }

            if(mStartSkillAction.Do())
                return; // 主動技真的放成功, 結束這次 AI 的判斷.
//          嘗試放主動技.
//			if(mPlayer.Attribute.ActiveSkills.Count > 0)
//			{
//				if(mSkillJudger != null && mSkillJudger.IsMatchCondition() && 
//                   mPlayer.CanUseActiveSkill(mPlayer.Attribute.ActiveSkills[mNextSkillIndex]))
//				{
//					if(GameController.Get.DoSkill(mPlayer, mPlayer.Attribute.ActiveSkills[mNextSkillIndex]))
//					{
//                        // 現在規則是主動技會按順序發動, 第 1 招發完, 下一次會放第 2 招.
//					    ++mNextSkillIndex;
//					    if(mNextSkillIndex >= mPlayer.Attribute.ActiveSkills.Count)
//					        mNextSkillIndex = 0;
//
//                        return; // 主動技真的放成功, 才真的會結束 AI 的判斷.
//                    }
//				}
//			}

            if(isBallOwner())
                mBallActions.Do();
			else if(!mPlayer.IsCatch)
                mNoBallActions.Do();

            if(GameController.Get.HasBallOwner)
                updateTactical(); // 一直指定戰術給球員.
            else
                tryDoPickBall(); // 其實應該要寫一個 PickBallState, 但目前暫時沒寫.
        }

        public override void Update()
        {
        }

        private bool isBallOwner()
        {
            return GameController.Get.BallOwner == mPlayer;
        }

        /// <summary>
        /// 當戰術跑完時, 會通知 AIController, 並會亂數再指定 1 個戰術. 這是避免 Idle 太久.
        /// 目前遊戲的 Idle 看起來會很呆.
        /// </summary>
        public void updateTactical()
        {
            if(mPlayer.TargetPosNum > 0 || !mTactical.IsValid)
                return;

            // 以下會將戰術的每個跑位點都設定 PlayerBehavior.
            var tacticalActions = mTactical.GetActions(mPlayer.Index);
            for(int i = 0; i < tacticalActions.Length; i++)
            {
                TMoveData moveData = new TMoveData();
                moveData.Clear();
                moveData.Speedup = tacticalActions[i].Speedup;
                moveData.Catcher = tacticalActions[i].Catcher;
                moveData.Shooting = tacticalActions[i].Shooting;

                int signZ = 1;
                if(GameStart.Get.CourtMode == ECourtMode.Full && mPlayer.Team == ETeamKind.Npc)
                    signZ = -1;

                moveData.SetTarget(tacticalActions[i].X, tacticalActions[i].Z * signZ);

                if(GameController.Get.BallOwner != mPlayer)
                    moveData.LookTarget = GameController.Get.BallOwner.transform;

                moveData.TacticalName = mTactical.Name;
                moveData.MoveFinish = oneMoveDone;
                mPlayer.TargetPos = moveData;
            }

//            GameController.Get.MoveDefPlayer(mPlayer);
        }

        private void oneMoveDone(PlayerBehaviour player, bool speedup)
        {
            GameController.Get.MoveDefPlayer(player, speedup);

            if(mPlayer.TargetPosNum == 0)
                // 全部的戰術都跑完了.
                GameMsgDispatcher.Ins.SendMesssage(EGameMsg.PlayerTacticalDone, mPlayer.Index);
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
                    if((GameController.Get.Situation == EGameSituation.GamerAttack && mPlayer.Team == ETeamKind.Self) ||
                       (GameController.Get.Situation == EGameSituation.NPCAttack && mPlayer.Team == ETeamKind.Npc))
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


