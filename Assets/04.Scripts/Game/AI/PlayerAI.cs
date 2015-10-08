using JetBrains.Annotations;
using UnityEngine;

namespace AI
{
    public enum EPlayerAIState
    {
        None,
        Attack,
        Defense,
        ReturnToHome
    }

    /// <summary>
    /// 每一位球員身上都會掛上此 Component.
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(PlayerBehaviour))]
    public class PlayerAI : MonoBehaviour, ITelegraph<EGameMsg>
    {
        public Team Team { set; get; }

        private StateMachine<EPlayerAIState, EGameMsg> mFSM;

        [UsedImplicitly]
        private void Awake()
        {
            mFSM = new StateMachine<EPlayerAIState, EGameMsg>();
            mFSM.AddState(new PlayerNoneState());
            mFSM.AddState(new PlayerAttackState(this, GetComponent<PlayerBehaviour>()));
            mFSM.AddState(new PlayerDefenseState(GetComponent<PlayerBehaviour>()));
            mFSM.AddState(new PlayerReturnToHomeState(this, GetComponent<PlayerBehaviour>()));
            mFSM.ChangeState(EPlayerAIState.None);

            GameMsgDispatcher.Ins.AddListener(this, EGameMsg.GamePlayersCreated);
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            mFSM.Update();

//            if(GameController.Get.BallOwner == GetComponent<PlayerBehaviour>())
//            {
//                var isBehindMe = Team.IsAllOpponentsBehindMe(transform.position);
//                Debug.LogFormat("Name:{0}, IsBehindMe:{1}", name, isBehindMe);
//            }
        }

        public void ChangeState(EPlayerAIState newState, object extraInfo = null)
        {
            mFSM.ChangeState(newState, extraInfo);
        }

        public void HandleMessage(Telegram<EGameMsg> msg)
        {
            if (msg.Msg == EGameMsg.GamePlayersCreated)
            {
//            Debug.Log("Receiving [EGameMsg.GamePlayersCreated]...");

                PlayerBehaviour[] players = (PlayerBehaviour[])msg.ExtraInfo;
                var attack = (PlayerAttackState)(mFSM[EPlayerAIState.Attack]);
                attack.Init(players);

                var defense = (PlayerDefenseState)(mFSM[EPlayerAIState.Defense]);
                defense.Init(players);

                GameMsgDispatcher.Ins.AddListener(mFSM, EGameMsg.CoachOrderAttackTactical);

                GameMsgDispatcher.Ins.RemoveListener(this, EGameMsg.GamePlayersCreated);
            }
        }

        /// <summary>
        /// 是否球員是該隊最接近球的球員?
        /// </summary>
        /// <returns></returns>
        public bool isNearestBall()
        {
            return Team.FindNearBallPlayer() == this;
        }
    } // end of the class PlayerAI.
} // end of the namespace AI.


