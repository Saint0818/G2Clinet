using UnityEngine;

namespace AI
{
    /// <summary>
    /// <para> 這個狀態是用在球員得分後, 得分球員做的特寫 or 特殊動作. 比如 Jason Terry 得分後的招牌動作是滑翔翼. </para>
    /// </summary>
    public class SpecialActionState : State<EGameSituation, EGameMsg>
    {
        public override EGameSituation ID
        {
            get { return EGameSituation.SpecialAction; }
        }

        /// <summary>
        /// <para> 幾秒後會切換到下一個狀態. </para>
        /// <para> switch to the next state, after a few seconds. </para>
        /// </summary>
        /// <remarks>
        /// <para> 注意, 這不能設定為 0, 否則會發生 1 vs 1 的時候, 對方得分後, 沒人撿球的問題. </para>
        /// <para> Note: this can't be zero, otherwise nobody picking ball will occur when 1 vs 1. </para>
        /// </remarks>
        private const float NextStateTime = 2;

        private float mChangeStateTime;

        /// <summary>
        /// 時間到後, 切換的新狀態.
        /// </summary>
        private EGameSituation mNextState;

        public override void Enter(object extraInfo)
        {
            mChangeStateTime = Time.time + NextStateTime;

            mNextState = (EGameSituation)extraInfo;

            foreach(PlayerBehaviour player in GameController.Get.GamePlayers)
            {
                player.ResetFlag();
                player.ResetMove();
            }

            defensePlayerGoHomePosition();
        }

        private void defensePlayerGoHomePosition()
        {
            if(mNextState == EGameSituation.GamerPickBall)
            {
                foreach(PlayerAI playerAI in AIController.Get.GetTeam(ETeamKind.Npc).Players)
                {
                    playerAI.ChangeState(EPlayerAIState.ReturnToHome);
                }
            }
            else if(mNextState == EGameSituation.NPCPickBall)
            {
                foreach(PlayerAI playerAI in AIController.Get.GetTeam(ETeamKind.Self).Players)
                {
                    playerAI.ChangeState(EPlayerAIState.ReturnToHome);
                }
            }
            else
                Debug.LogWarning("Next State must be APickBallAfterScore or BPickBallAfterScore.");
        }

        public override void UpdateAI()
        {
            if(Time.time >= mChangeStateTime)
            {
                Parent.ChangeState(mNextState);
                GameController.Get.ChangeSituation(mNextState);
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
        }
    } // end of the class SpecialActionState.
} // end of the namespace AI.


