using GamePlayEnum;
using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 球飛向誰的程式碼在 CourtMgr.SetBallState(EPlayerState.JumpBall).
    /// </summary>
    public class JumpBallState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.JumpBall; }
        }

        private readonly PlayerBehaviour[] mJumpBallPlayers = new PlayerBehaviour[2];

        /// <summary>
        /// 這是要接球的球員.
        /// </summary>
        [CanBeNull]
        private PlayerBehaviour mReceiveBallPlayer;

        public override void Enter(object extraInfo)
        {
            GameController.Get.IsStart = true;
            CourtMgr.Get.InitScoreboard(true);
            //        GameController.Get.setPassIcon(true);

            // 找出 2 位要跳球的球員.
            mJumpBallPlayers[0] = findJumpBallPlayer(ETeamKind.Self);
            if (mJumpBallPlayers[0] != null)
                mJumpBallPlayers[0].DoPassiveSkill(ESkillSituation.JumpBall, CourtMgr.Get.RealBall.transform.position);

            mJumpBallPlayers[1] = findJumpBallPlayer(ETeamKind.Npc);
            if (mJumpBallPlayers[1] != null)
                mJumpBallPlayers[1].DoPassiveSkill(ESkillSituation.JumpBall, CourtMgr.Get.RealBall.transform.position);
        }

        public override void UpdateAI()
        {
            if (GameController.Get.BallOwner == null && mReceiveBallPlayer != null)
            {
                GameController.Get.DoPickBall(mReceiveBallPlayer);
            }
        }

//        public override void Update()
//        {
//        }

        public override void Exit()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
            if (msg.Msg == EGameMsg.PlayerTouchBallWhenJumpBall)
            {
                var touchPlayer = randomTouchBallPlayer();
                if (touchPlayer != null)
                {
                    mReceiveBallPlayer = randomReceiveBallPlayer(touchPlayer);

                    // 要求籃球飛向 ReceiveBallPlayer.
                    CourtMgr.Get.SetBallState(EPlayerState.JumpBall, mReceiveBallPlayer);
                }
                else
                    Debug.LogWarning("Can't found any jump player!");
            }
        }

        [CanBeNull]
        private PlayerBehaviour findJumpBallPlayer(ETeamKind team)
        {
            int findIndex = team == 0 ? 0 : 3;
            PlayerBehaviour npc = null;
            for (int i = 0; i < GameController.Get.GamePlayers.Count; i++)
            {
                if (GameController.Get.GamePlayers[i].gameObject.activeInHierarchy &&
                   GameController.Get.GamePlayers[i].Team == team &&
                   GameController.Get.GamePlayers[i].IsJumpBallPlayer)
                {
                    findIndex = i;
                }
            }

            if (findIndex < GameController.Get.GamePlayers.Count)
                npc = GameController.Get.GamePlayers[findIndex];

            return npc;
        }

        [CanBeNull]
        private PlayerBehaviour randomReceiveBallPlayer([NotNull] PlayerBehaviour exceptPlayer)
        {
            var team = AIController.Get.GetTeam(exceptPlayer.Team);
            PlayerAI receivalBallPlayer = team.RandomSameTeamPlayer(exceptPlayer.GetComponent<PlayerAI>());

            if (receivalBallPlayer != null)
                return receivalBallPlayer.GetComponent<PlayerBehaviour>();
            return null;
        }

        [CanBeNull]
        private PlayerBehaviour randomTouchBallPlayer()
        {
            if (mJumpBallPlayers[0] == null && mJumpBallPlayers[1] == null)
                return null;

            if (mJumpBallPlayers[0] != null && mJumpBallPlayers[1] == null)
                return mJumpBallPlayers[0];

            if (mJumpBallPlayers[0] == null && mJumpBallPlayers[1] != null)
                return mJumpBallPlayers[1];

            WeightedRandomizer<int> randomizer = new WeightedRandomizer<int>();
            randomizer.AddOrUpdate(0, mJumpBallPlayers[0].Attribute.Rebound);
            randomizer.AddOrUpdate(1, mJumpBallPlayers[1].Attribute.Rebound);
            int index = randomizer.GetNext();
            return mJumpBallPlayers[index];
        }
    }
}

