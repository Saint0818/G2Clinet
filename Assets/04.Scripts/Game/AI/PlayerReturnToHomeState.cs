using JetBrains.Annotations;
using UnityEngine;
using GameStruct;

namespace AI
{
    /// <summary>
    /// <para> 球員跑回 Home Position. </para>
    /// <para> 切換到此狀態時, 必須要傳遞 EPlayerAIState, 也就是跑到 Home Position 時, AI 要切換到什麼狀態. </para>
    /// </summary>
    public class PlayerReturnToHomeState : State<EPlayerAIState, EGameMsg>
    {
        public override EPlayerAIState ID
        {
            get { return EPlayerAIState.ReturnToHome; }
        }

        private readonly PlayerAI mPlayerAI;
        private readonly PlayerBehaviour mPlayer;

        /// <summary>
        /// 跑到 Home Position 時, 要切換到哪個狀態.
        /// </summary>
        private EPlayerAIState mFinishNextState = EPlayerAIState.None;

        public PlayerReturnToHomeState([NotNull]PlayerAI playerAI, [NotNull]PlayerBehaviour player)
        {
            mPlayerAI = playerAI;
            mPlayer = player;
        }

        public override void Enter(object extraInfo)
        {
//            Debug.LogFormat("PlayerReturnToHomeState.Enter, Player:{0}", mPlayer.name);

            moveToHomePosition();

            mFinishNextState = (EPlayerAIState)extraInfo;
        }

        private void moveToHomePosition()
        {
            mPlayer.ResetFlag();

            var homePos = mPlayerAI.Team.GetHomePosition(mPlayer.Postion);

            TMoveData moveData = new TMoveData();
            moveData.SetTarget(homePos.x, homePos.y);
            moveData.MoveFinish = onMoveFinish;
            mPlayer.TargetPos = moveData;
        }

        private void onMoveFinish(PlayerBehaviour player, bool speedup)
        {
            mPlayer.AniState(EPlayerState.Idle);
            Vector3 shootPoint = mPlayerAI.Team.GetShootPoint();
            mPlayer.RotateTo(shootPoint.x, shootPoint.z);

            Parent.ChangeState(mFinishNextState);
        }

        public override void Exit()
        {
//            Debug.LogFormat("PlayerReturnToHomeState.Exit, Player:{0}", mPlayer.name);
        }

        public override void UpdateAI()
        {
            if(mPlayer.TargetPosNum <= 0)
                moveToHomePosition();
        }

        public override void Update()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    } // end of the class.
} // end of the namespace.