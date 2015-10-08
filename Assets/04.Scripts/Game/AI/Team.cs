
using System.Collections.Generic;
using G2;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 設計:
    /// <list type="number">
    /// <item> 不要用 ETeamKind 來記錄球員, 分開兩者比較好. 因為這樣邏輯比較清晰, 不會被 ETeamKind 的值混淆. </item>
    /// </list>
    /// </remarks>
    public class Team
    {
        public List<PlayerAI> Players
        {
            get { return mPlayers; }
        }

        public List<PlayerAI> OpponentPlayers
        {
            get { return mOpponentPlayers; }
        }

        private readonly ETeamKind mTeamKind;

        /// <summary>
        /// 該隊球員.
        /// </summary>
        private readonly List<PlayerAI> mPlayers = new List<PlayerAI>();

        /// <summary>
        /// 敵對球員.
        /// </summary>
        private readonly List<PlayerAI> mOpponentPlayers = new List<PlayerAI>();

        private readonly Dictionary<EPlayerPostion, Vector2> mHomePositions;

        /// <summary>
        /// 代表的球員的某個隊伍. 目前只是放 utility method, 讓 Player AI 執行的時候使用.
        /// </summary>
        /// <param name="teamKind"></param>
        /// <param name="homePositions"></param>
        public Team(ETeamKind teamKind, Dictionary<EPlayerPostion, Vector2> homePositions)
        {
            mTeamKind = teamKind;

            mHomePositions = new Dictionary<EPlayerPostion, Vector2>(homePositions);
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
            return string.Format("Team:{0}", mTeamKind);
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
                var newDis = MathUtils.Find2DDis(mOpponentPlayers[i].transform.position, position);
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

        public enum EFindPlayerResult
        {
            CannotFound, // 找不到防守球員.
            InFront, // 防守球員在前方.
            InBack // 防守球員在後方.
        }
        /// <summary>
        /// 某位球員在某個距離和角度內, 是否有防守球員?
        /// </summary>
        /// <param name="player"></param>
        /// <param name="dis"></param>
        /// <param name="angle"></param>
        /// <returns> 0: 找不到防守球員; 1: 有找到, 防守球員在前方; 2: 有找到, 防守球員在後方. </returns>
        public EFindPlayerResult HasDefPlayer([NotNull]PlayerAI player, float dis, float angle)
        {
            PlayerAI defPlayer;
            return FindDefPlayer(player, dis, angle, out defPlayer);
        }

        /// <summary>
        /// 某位球員在某個距離和角度內, 是否有防守球員?
        /// </summary>
        /// <param name="player"></param>
        /// <param name="dis"></param>
        /// <param name="angle"></param>
        /// <param name="defPlayer"></param>
        /// <returns> 0: 找不到防守球員; 1: 有找到, 防守球員在前方; 2: 有找到, 防守球員在後方. </returns>
        public EFindPlayerResult FindDefPlayer([NotNull]PlayerAI player, float dis, float angle, 
                                               out PlayerAI defPlayer)
        {
            defPlayer = null;

            for (int i = 0; i < mOpponentPlayers.Count; i++)
            {
                float realAngle = MathUtils.GetAngle(player.transform, mOpponentPlayers[i].transform);

                if (MathUtils.Find2DDis(player.transform.position, mOpponentPlayers[i].transform.position) <= dis)
                {
                    if(realAngle >= 0 && realAngle <= angle)
                    {
                        defPlayer = mOpponentPlayers[i];
                        return EFindPlayerResult.InFront;
                    }
                    if(realAngle <= 0 && realAngle >= -angle)
                    {
                        defPlayer = mOpponentPlayers[i];
                        return EFindPlayerResult.InBack;
                    }
                }
            }

            return EFindPlayerResult.CannotFound;
        }

        [CanBeNull]
        public PlayerAI RandomSameTeamPlayer([NotNull] PlayerAI exceptPlayer)
        {
            if(exceptPlayer.GetComponent<PlayerBehaviour>().Team != mTeamKind)
                Debug.LogWarningFormat("Except Player isn't same team player");

            List<PlayerAI> otherPlayers = new List<PlayerAI>();
            foreach(PlayerAI playerAI in mPlayers)
            {
                if(playerAI == exceptPlayer)
                    continue;
                otherPlayers.Add(playerAI);
            }

            if(otherPlayers.Count == 0)
                return null;

            var randomIndex = Random.Range(0, otherPlayers.Count);
            return otherPlayers[randomIndex];
        }

        public bool IsAllOpponentsBehindMe(Vector3 position)
        {
            var targetPos = CourtMgr.Get.ShootPoint[(int)mTeamKind].transform.position;
            var meDis = MathUtils.Find2DDis(targetPos, position);
            for(int i = 0; i < mOpponentPlayers.Count; i++)
            {
                if(MathUtils.Find2DDis(mOpponentPlayers[i].transform.position, targetPos) < meDis)
                    return false;
            }

            return true;
        }

        public Vector3 GetShootPoint()
        {
            return CourtMgr.Get.GetShootPointPosition(mTeamKind);
        }

        public Vector2 GetHomePosition(EPlayerPostion pos)
        {
            return mHomePositions[pos];
        }
    } // end of the class Team.
} // end of the namespace AI.


