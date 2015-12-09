using JetBrains.Annotations;

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

        private readonly StateMachine<EPlayerAttackState, EGameMsg> mFSM = new StateMachine<EPlayerAttackState, EGameMsg>();

        public PlayerAttackState([NotNull]PlayerAI playerAI, [NotNull] PlayerBehaviour player)
        {
            mFSM.AddState(new PlayerAttackNoneState());
            mFSM.AddState(new PlayerAttackGeneralState(playerAI, player));
            mFSM.AddState(new PlayerAttackFastBreakState(playerAI, player));
            mFSM.ChangeState(EPlayerAttackState.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="players"> 該場比賽中, 全部的球員. </param>
        public void Init([NotNull]PlayerBehaviour[] players)
        {
            var general = (PlayerAttackGeneralState)mFSM[EPlayerAttackState.General];
            general.Init(players);
        }

        public EPlayerAttackState GetCurrentState()
        {
            return mFSM.CurrentState.ID;
        }

        public override void Enter(object extraInfo)
        {
            mFSM.ChangeState(EPlayerAttackState.General);
        }

        public override void Exit()
        {
            mFSM.ChangeState(EPlayerAttackState.None);
        }

        public override void UpdateAI()
        {
            mFSM.Update();
        }

        public override void Update()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
            mFSM.CurrentState.HandleMessage(msg);
        }
    } // end of the class PlayerAttackState.
} // end of the namespace AI.


