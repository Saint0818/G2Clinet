using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 球員要跑回 Home Position.
    /// </summary>
    public class PlayerReturnToHomeState : State<EPlayerAIState, EGameMsg>
    {
        public override EPlayerAIState ID
        {
            get { return EPlayerAIState.ReturnToHome; }
        }

        private readonly PlayerAI mPlayerAI;
        private readonly PlayerBehaviour mPlayer;

        public PlayerReturnToHomeState([NotNull]PlayerAI playerAI, [NotNull]PlayerBehaviour player)
        {
            mPlayerAI = playerAI;
            mPlayer = player;
        }

        public override void Enter(object extraInfo)
        {
//            Debug.LogFormat("PlayerReturnToHomeState.Enter, Player:{0}", mPlayer.name);

            moveToHomePosition();
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
//            Parent.ChangeState(EPlayerAIState.Defense);
            mPlayer.AniState(EPlayerState.Idle);
            Vector3 shootPoint = mPlayerAI.Team.GetShootPoint();
            mPlayer.RotateTo(shootPoint.x, shootPoint.z);
        }

        public override void Exit()
        {
//            Debug.LogFormat("PlayerReturnToHomeState.Exit, Player:{0}", mPlayer.name);
        }

        public override void Update()
        {
            if(mPlayer.TargetPosNum <= 0)
                moveToHomePosition();
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    }
}