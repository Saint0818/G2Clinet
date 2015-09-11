
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{

    public class Team
    {
        private readonly ETeamKind mTeam;

        private readonly List<PlayerAI> mPlayers = new List<PlayerAI>();

        /// <summary>
        /// 代表的球員的某個隊伍. 目前只是放 utility method, 讓 Player AI 執行的時候使用.
        /// </summary>
        /// <param name="team"></param>
        public Team(ETeamKind team)
        {
            mTeam = team;
        }

        public void Clear()
        {
            mPlayers.Clear();
        }

        public void AddPlayer([NotNull]PlayerAI player)
        {
            mPlayers.Add(player);
            player.Parent = this;
        }

        public override string ToString()
        {
            return string.Format("Team:{0}", mTeam);
        }

        /// <summary>
        /// 找出某隊離球最近的球員.
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public PlayerAI FindNearBallPlayer()
        {
            float nearDis = float.MaxValue;
            PlayerAI nearBallPlayer = null;

            Vector2 ballPos = Vector2.zero;
            ballPos.Set(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);

            for(int i = 0; i < mPlayers.Count; i++)
            {
                Vector2 someonePos = Vector2.zero;
                someonePos.Set(mPlayers[i].transform.position.x, mPlayers[i].transform.position.z);

                var dis = Vector2.Distance(ballPos, someonePos);
                if(dis < nearDis)
                {
                    nearDis = dis;
                    nearBallPlayer = mPlayers[i];
                }
            }

            return nearBallPlayer;
        }

    } // end of the class Team.
} // end of the namespace AI.


