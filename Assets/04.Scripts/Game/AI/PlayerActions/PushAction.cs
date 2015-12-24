using G2;
using GamePlayEnum;
using UnityEngine;

namespace AI
{
    public class PushAction : PlayerAIAction
    {
        private Vector3 mNearPlayerPos;

        public PushAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return Player.Attr.PushingRate; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            // 參數 player 並未持球, 所以只能做 Push 被動技.
            // 這裡的企劃規則是, 附近的敵對球員必須是 Idle 狀態時, 才會真的執行推人行為.
            var nearPlayer = PlayerAI.FindNearestOpponentPlayer();
            if(nearPlayer == null)
                return false;

            mNearPlayerPos = nearPlayer.transform.position;

            bool isClose = MathUtils.Find2DDis(nearPlayer.transform.position, PlayerAI.transform.position) <= GameConst.StealPushDistance;

            return isAnimationValid() && isClose && Player.PushCD.IsTimeUp() &&
                nearPlayer.GetComponent<PlayerBehaviour>().CheckAnimatorSate(EPlayerState.Idle);
        }

        private bool isAnimationValid()
        {
            return Player.CanUseState(EPlayerState.Push0) ||
                   Player.CanUseState(EPlayerState.Push1) ||
                   Player.CanUseState(EPlayerState.Push2) ||
                   Player.CanUseState(EPlayerState.Push20);
        }

        public override void Do()
        {
			if(Player.PlayerSkillController.DoPassiveSkill(ESkillSituation.Push0, mNearPlayerPos))
                Player.PushCD.StartAgain();
        }
    }
}