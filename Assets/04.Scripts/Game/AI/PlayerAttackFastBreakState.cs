using G2;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 前方沒人時, 會不斷的往籃框跑, 然後灌籃 or 上籃.
    /// </summary>
    public class PlayerAttackFastBreakState : State<PlayerAttackState.EPlayerAttackState, EGameMsg>
    {
        public override PlayerAttackState.EPlayerAttackState ID
        {
            get { return PlayerAttackState.EPlayerAttackState.FastBreak;}
        }

        private readonly PlayerAI mPlayerAI;
        private readonly PlayerBehaviour mPlayer;

        public PlayerAttackFastBreakState([NotNull] PlayerAI playerAI, [NotNull] PlayerBehaviour player)
        {
            mPlayerAI = playerAI;
            mPlayer = player;
        }

        public override void Enter(object extraInfo)
        {
//            Debug.LogFormat("PlayerAttackFastBreakState.Enter, Player:{0}, TargetPos:{1}", mPlayerAI.name, mPlayerAI.Team.GetShootPoint());

            mPlayer.ResetFlag();
            mPlayer.ResetMove();

            Vector3 shootPoint = mPlayerAI.Team.GetShootPoint();
            TMoveData moveData = new TMoveData();
            moveData.SetTarget(shootPoint.x, shootPoint.y);
            mPlayer.TargetPos = moveData;
        }

        public override void Exit()
        {
//            Debug.LogFormat("PlayerAttackFastBreakState.Exit, Player:{0}", mPlayerAI.name);
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }

        public override void Update()
        {
            if(GameController.Get.BallOwner == mPlayer &&
               !mPlayerAI.Team.IsAllOpponentsBehindMe(mPlayerAI.transform.position))
            {
                // 我是持球者, 而且我前方有任何人.
                Parent.ChangeState(PlayerAttackState.EPlayerAttackState.General);
                return;
            }

            Vector3 shootPoint = mPlayerAI.Team.GetShootPoint();
            if(MathUtils.Find2DDis(shootPoint, mPlayerAI.transform.position) <= GameConst.DunkDistance)
            {
                GameController.Get.DoShoot();
                Parent.ChangeState(PlayerAttackState.EPlayerAttackState.General);
            }
        }
    }
}