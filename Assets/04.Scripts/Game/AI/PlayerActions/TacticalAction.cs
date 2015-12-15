using UnityEngine;

namespace AI
{
    /// <summary>
    /// 球員跑戰術.
    /// </summary>
    public class TacticalAction : PlayerAIAction
    {
        /// <summary>
        /// 戰術要跑多久, 單位:秒.
        /// </summary>
        private float mDoneTime;

        public TacticalAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return GameConst.AITacticalRate; }
        }

        public override bool IsDone
        {
            get
            {
                if(!IsValid())
                    return true;

                return Time.time >= mDoneTime;
            }
        }

        public override bool IsValid()
        {
            return Player.CanMove;
        }

        public override void Do()
        {
            mDoneTime = Time.time + Random.Range(GameConst.AIMinTacticalTime, GameConst.AIMaxTacticalTime);
        }
    }
}