using G2;
using JetBrains.Annotations;
using UnityEngine;
using GameStruct;

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
//            mPlayer.ResetMove();

            updateTarget();
        }

        private void updateTarget()
        {
            if(mPlayer.TargetPosNum > 0)
                return;

            Vector3 shootPoint = mPlayerAI.Team.GetShootPoint();
            TMoveData moveData = new TMoveData();
            moveData.SetTarget(shootPoint.x, shootPoint.z);
            moveData.Speedup = true;
            mPlayer.TargetPos = moveData;
        }

        public override void Exit()
        {
//            Debug.LogFormat("PlayerAttackFastBreakState.Exit, Player:{0}", mPlayerAI.name);
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }

        public override void UpdateAI()
        {
            // 魔術數字 1.0 的目的是希望球員可以盡可能的變成快攻狀態.
            if(GameController.Get.BallOwner == mPlayer &&
               !mPlayerAI.Team.IsAllOpponentsBehindMe(mPlayerAI.transform.position, GameConst.AIFastBreakOffset))
            {
                // 我是持球者, 而且我前方有任何人.
                Parent.ChangeState(PlayerAttackState.EPlayerAttackState.General);
                return;
            }

            // 我發現目標位置會被重置, 暫時找不到原因, 所以這邊就會每次更新位置.
            updateTarget();

            // 靠近籃框, 要做投籃.
            Vector3 shootPoint = mPlayerAI.Team.GetShootPoint();
            if(MathUtils.Find2DDis(shootPoint, mPlayerAI.transform.position) <= GameConst.DunkDistance)
            {
                GameController.Get.DoShoot();
                Parent.ChangeState(PlayerAttackState.EPlayerAttackState.General);
            }
        }

        public override void Update()
        {
        }
    } // end of the class.
} // end of the namespace AI.