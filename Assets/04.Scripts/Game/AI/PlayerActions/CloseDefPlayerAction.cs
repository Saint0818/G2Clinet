using UnityEngine;

namespace AI
{
    /// <summary>
    /// 接近對位球員.
    /// </summary>
    public class CloseDefPlayerAction : PlayerAIAction
    {
        /// <summary>
        /// 戰術要跑多久, 單位:秒.
        /// </summary>
        private float mDoneTime;

        public CloseDefPlayerAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return GameConst.AICloseDefPlayerRate; }
        }

        public override bool IsDone
        {
            get
            {
                return IsValid() && Time.time >= mDoneTime;
            }
        }

        public override bool IsValid()
        {
            return Player.CanMove;
        }

        public override void Do()
        {
            GameController.Get.MoveDefPlayer(Player.DefPlayer);
            mDoneTime = Time.time + Random.Range(GameConst.AIMinCloseDefPlayerTime, GameConst.AIMaxCloseDefPlayerTime);
        }
    }
}