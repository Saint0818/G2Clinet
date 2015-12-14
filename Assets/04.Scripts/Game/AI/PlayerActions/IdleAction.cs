using UnityEngine;

namespace AI
{
    /// <summary>
    /// 站在原地一段時間. 這可以吸引別人過來推我.
    /// </summary>
    public class IdleAction : PlayerAIAction
    {
        /// <summary>
        /// 完成的時間.
        /// </summary>
        private float mDoneTime;

        public IdleAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return GameConst.AIIdleRate; }
        }

        public override bool IsDone
        {
            get { return Time.time >= mDoneTime; }
        }

        public override bool IsValid()
        {
            return Player.CanUseState(EPlayerState.Idle);
        }

        public override void Do()
        {
            Player.AniState(EPlayerState.Idle);

            mDoneTime = Time.time + Random.Range(GameConst.AIMinIdleTime, GameConst.AIMaxIdleTime);
        }
    }
}