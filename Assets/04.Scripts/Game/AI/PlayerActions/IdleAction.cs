﻿using UnityEngine;
using GameEnum;

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

        private readonly EPlayerState mAnimation;

        public IdleAction(PlayerAI playerAI, PlayerBehaviour player, EPlayerState playerState) : base(playerAI, player)
        {
            mAnimation = playerState;
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
            return Player.CanUseState(mAnimation);
        }

        public override void Do()
        {
            Player.AniState(mAnimation);

            mDoneTime = Time.time + Random.Range(GameConst.AIMinIdleTime, GameConst.AIMaxIdleTime);
        }
    }
}