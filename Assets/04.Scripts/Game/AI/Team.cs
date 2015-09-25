
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    public class Team
    {
        private readonly ETeamKind mTeam;

        /// <summary>
        /// 該隊球員.
        /// </summary>
        private readonly List<PlayerAI> mPlayers = new List<PlayerAI>();

        /// <summary>
        /// 敵對球員.
        /// </summary>
        private readonly List<PlayerAI> mOpponentPlayers = new List<PlayerAI>();

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
            mOpponentPlayers.Clear();
        }

        public void AddPlayer([NotNull]PlayerAI player)
        {
            mPlayers.Add(player);
            player.Team = this;
        }

        public void AddOpponentPlayer([NotNull] PlayerAI player)
        {
            mOpponentPlayers.Add(player);
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
            float nearestDis = float.MaxValue;
            PlayerAI nearestBallPlayer = null;

            Vector2 ballPos = Vector2.zero;
            ballPos.Set(CourtMgr.Get.RealBall.transform.position.x, CourtMgr.Get.RealBall.transform.position.z);

            for(int i = 0; i < mPlayers.Count; i++)
            {
                Vector2 playerPos = Vector2.zero;
                playerPos.Set(mPlayers[i].transform.position.x, mPlayers[i].transform.position.z);

                var dis = Vector2.Distance(ballPos, playerPos);
                if(dis < nearestDis)
                {
                    nearestDis = dis;
                    nearestBallPlayer = mPlayers[i];
                }
            }

            return nearestBallPlayer;
        }

        /// <summary>
        /// 從某點找出離該點最接近的敵方球員.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [CanBeNull]
        public PlayerAI FindNearestOpponentPlayer(Vector3 position)
        {
            PlayerAI nearPlayer = null;
            float nearDis = float.MaxValue;
            for(int i = 0; i < mOpponentPlayers.Count; i++)
            {
                var newDis = AITools.Find2DDis(mOpponentPlayers[i].transform.position, position);
                if(newDis < nearDis)
                {
                    nearDis = newDis;
                    nearPlayer = mOpponentPlayers[i];
                }
            }

            return nearPlayer;
        }

        /// <summary>
        /// 是否球員在前場.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool IsInUpfield([NotNull]PlayerBehaviour player)
        {
            if(player.Team == ETeamKind.Self && player.transform.position.z >= 15.5f && 
               player.transform.position.x <= 1 && player.transform.position.x >= -1)
                return false;
            if(player.Team == ETeamKind.Npc && player.transform.position.z <= -15.5f && 
               player.transform.position.x <= 1 && player.transform.position.x >= -1)
                return false;
            return true;
        }

    } // end of the class Team.
} // end of the namespace AI.


