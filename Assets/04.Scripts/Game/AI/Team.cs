
using System.Collections.Generic;
using System.Text;
using G2;
using JetBrains.Annotations;
using UnityEngine;
using GameEnum;

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

        private readonly StringBuilder mBuilder = new StringBuilder();
        public override string ToString()
        {
            mBuilder.Remove(0, mBuilder.Length);

            mBuilder.AppendLine();
            mBuilder.AppendFormat("Team:{0}, ", mTeamKind);
            mBuilder.AppendLine();

            for(int i = 0; i < mPlayers.Count; i++)
            {
                mBuilder.AppendFormat("{0}", mPlayers[i]);
                mBuilder.AppendLine();
            }

            return mBuilder.ToString();
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
            ballPos.Set(CourtMgr.Get.RealBallObj.transform.position.x, CourtMgr.Get.RealBallObj.transform.position.z);

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
        /// 是否球員在前場(接近進攻籃框); 後場(接近防守籃框).
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns> true: 在前場; false: 在後場. </returns>
        public bool IsInUpfield(Vector3 worldPos)
        {
            // 球場的大小是 (-12, -17) ~ (12, 17).
            // 畫面上的左是 +z 軸, 畫面上的右是 -z 軸.
            // 我為了判斷正確, 我就稍微加大數值.
            if(mTeamKind == ETeamKind.Self)
            {
                if(-15 <= worldPos.x && worldPos.x <= 15 &&
                   0 <= worldPos.z && worldPos.z <= 20.0f)
                    return true;
            }
            else if(mTeamKind == ETeamKind.Npc)
            {
                if(-15 <= worldPos.x && worldPos.x <= 15 &&
                   -20.0f <= worldPos.z && worldPos.z <= 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 某位球員在某個距離和角度內, 是否有防守球員?
        /// </summary>
        /// <param name="player"></param>
        /// <param name="dis"></param>
        /// <param name="angle"></param>
        /// <returns> 0: 找不到防守球員; 1: 有找到, 防守球員在前方; 2: 有找到, 防守球員在後方. </returns>
        public bool HasDefPlayer([NotNull]PlayerAI player, float dis, float angle)
        {
            PlayerAI defPlayer;
            return FindDefPlayer(player, dis, angle, out defPlayer);
        }

        /// <summary>
        /// 某位球員進攻時, 在某個距離和角度內, 是否有防守球員?
        /// </summary>
        /// <param name="player"></param>
        /// <param name="dis"> 進攻球員和防守球員的距離. </param>
        /// <param name="angle"> 進攻球員和防守球員的夾角, 單位:度. </param>
        /// <param name="defPlayer"></param>
        /// <returns> true:有找到防守球員. </returns>
        public bool FindDefPlayer([NotNull]PlayerAI player, float dis, float angle, 
                                  out PlayerAI defPlayer)
        {
            defPlayer = null;

            for(int i = 0; i < mOpponentPlayers.Count; i++)
            {
                float angleBetween = FindAttackAngle(player.transform.position,
                                                     mOpponentPlayers[i].transform.position);

                float disBetween = MathUtils.Find2DDis(player.transform.position, mOpponentPlayers[i].transform.position);
                if(disBetween <= dis && angleBetween <= angle)
                {
                    defPlayer = mOpponentPlayers[i];
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 找出進攻球員和防守球員的夾角.(和進攻藍框的位置有密切的關係)
        /// </summary>
        /// <param name="attackPlayerPos"></param>
        /// <param name="defPlayerPos"></param>
        /// <returns> 0 ~ 180, 單位:度. </returns>
        public float FindAttackAngle(Vector3 attackPlayerPos, Vector3 defPlayerPos)
        {
            Vector2 shootPoint = MathUtils.Convert2D(GetShootPoint());
            Vector2 srcPos = MathUtils.Convert2D(attackPlayerPos);
            Vector2 opponentPos = MathUtils.Convert2D(defPlayerPos);

            Vector2 playerToShootPoint = shootPoint - srcPos;
            Vector2 playerToOpp = opponentPos - srcPos;
            return Vector2.Angle(playerToShootPoint, playerToOpp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptPlayer"></param>
        /// <returns> null: 如果是 1 vs 1 的時候會發生. </returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsAllOpponentsBehindMe(Vector3 position)
        {
            var targetPos = CourtMgr.Get.ShootPoint[(int)mTeamKind].transform.position;
            var meDis = MathUtils.Find2DDis(targetPos, position);
            for(int i = 0; i < mOpponentPlayers.Count; i++)
            {
                float oppDis = MathUtils.Find2DDis(mOpponentPlayers[i].transform.position, targetPos);
                if(oppDis - meDis < GameConst.AIFastBreakThreshold)
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

        private const float NearHomePositionDis = 0.5f;

        public bool IsNearHomePosition(PlayerBehaviour player)
        {
            return IsNearHomePosition(player.Postion, player.transform.position);
        }

        public bool IsNearHomePosition(EPlayerPostion pos, Vector3 worldPos)
        {
            return Vector2.Distance(mHomePositions[pos], MathUtils.Convert2D(worldPos)) <= NearHomePositionDis;
        }
    } // end of the class Team.
} // end of the namespace AI.


