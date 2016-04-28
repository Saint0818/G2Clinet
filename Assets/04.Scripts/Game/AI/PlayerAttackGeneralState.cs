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

        private readonly StartActiveSkillAction mStartActiveSkillAction;

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

            mStartActiveSkillAction = new StartActiveSkillAction(mPlayer);

//            mBallActions.Add(new MoveDodgeAction(mPlayerAI, mPlayer));
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

        public override string ToString()
        {
            if(isBallOwner())
                return string.Format("{0}, {1}", ID, mBallActions);
            return string.Format("{0}, {1}", ID, mNoBallActions);
        }

        public override void Enter(object extraInfo)
        {
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
            mStartActiveSkillAction.Init(players, true);
        }

        public override void UpdateAI()
        {
            if(!mPlayer.AIing)
                return;

            // 魔術數字 1.0 的目的是希望球員可以盡可能的變成快攻狀態.
            if(GameController.Get.BallOwner == mPlayer &&
               mPlayerAI.Team.IsAllOpponentsBehindMe(mPlayerAI.transform.position))
            {
                // 我是持球者, 而且我前方沒有任何人.
                Parent.ChangeState(PlayerAttackState.EPlayerAttackState.FastBreak);
                return;
            }

            if(mStartActiveSkillAction.Do())
                return; // 主動技真的放成功, 結束這次 AI 的判斷.

            if(isBallOwner())
            {
                mBallActions.Do();

//                Debug.LogFormat("{0}, {1}", mPlayer.name, mBallActions);
            }
			else if(!mPlayer.IsCatch)
			{
			    mNoBallActions.Do();

//                Debug.LogFormat("{0}, {1}", mPlayer.name, mNoBallActions);
            }

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
                if(CourtMgr.Get.CourtMode == ECourtMode.Full && mPlayer.Team == ETeamKind.Npc)
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
//            GameController.Get.MoveDefPlayer(player, speedup);

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


