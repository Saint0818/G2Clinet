using System.Collections.Generic;
using GameStruct;
using UnityEngine;

namespace AI
{
    public class PlayerDebugAlleyoop : State<EPlayerAIState, EGameMsg>
    {
        private readonly List<Vector2> mMovingPoints = new List<Vector2>();

        private readonly PlayerBehaviour mPlayer;

        public PlayerDebugAlleyoop(PlayerBehaviour player)
        {
            mPlayer = player;
        }

        public override EPlayerAIState ID
        {
            get { return EPlayerAIState.DebugAlleyoop; }
        }

        public void AddPosition(float x, float z)
        {
            mMovingPoints.Add(new Vector2(x, z));
        }

        public override void Enter(object extraInfo)
        {
            setMoveData();
        }

        public override void Exit()
        {
        }

        public override void UpdateAI()
        {
            if(GameController.Get.BallOwner == mPlayer)
                GameController.Get.TryPass(GameController.Get.Joysticker);

            if (mPlayer.TargetPosNum == 0)
                setMoveData();
        }

        private void onMoveFinish(PlayerBehaviour player, bool speedUp)
        {
            if(mPlayer.TargetPosNum == 0)
                setMoveData();
        }

        private void setMoveData()
        {
            mPlayer.ResetMove();
            foreach(Vector2 point in mMovingPoints)
            {
                var moveData = new TMoveData();
                moveData.SetTarget(point.x, point.y);
                moveData.MoveFinish = onMoveFinish;

                mPlayer.TargetPos = moveData;
            } 
        }

        public override void Update()
        {
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
        }
    }
}