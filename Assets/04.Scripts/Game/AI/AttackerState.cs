using System.Collections.Generic;

namespace AI
{
    /// <summary>
    /// 進攻方相同的邏輯.
    /// </summary>
    /// 實作子類別:
    /// <list type="number">
    /// <item> 要初始化 Players property. 目前認為最好的時機點是在 State.Enter 設定. </item>
    /// <item> 要設定 Tactical property. </item>
    /// </list>
    public abstract class AttackerState : State<EGameSituation, EGameMsg>
    {
        protected List<PlayerBehaviour> Players { get {return mPlayers;} }
        private readonly List<PlayerBehaviour> mPlayers = new List<PlayerBehaviour>();

        /// <summary>
        /// 進攻要跑的戰術類型.
        /// </summary>
        public ETacticalAuto Tactical { get; set; }

        /// <summary>
        /// 哪位球員的戰術跑完.
        /// </summary>
        private readonly Dictionary<EPlayerPostion, bool> mTacticalDones = new Dictionary<EPlayerPostion, bool>
        {
            {EPlayerPostion.G, false},
            {EPlayerPostion.F, false},
            {EPlayerPostion.C, false}
        };

        private void randomTacticalToPlayer()
        {
            if(GameController.Get.HasBallOwner)
            {
                TTacticalData tactical;
                bool isFound = AITools.RandomTactical(Tactical, 
                    GameController.Get.BallOwner.Index, out tactical);

                if(isFound)
                    GameMsgDispatcher.Ins.SendMesssage(EGameMsg.CoachOrderAttackTactical, tactical);
            }
        }

        private bool isAllPlayerTacticalDone()
        {
            foreach(KeyValuePair<EPlayerPostion, bool> pair in mTacticalDones)
            {
                if(!pair.Value)
                    return false;
            }

            return true;
        }

        private void clearPlayerTacticals()
        {
            mTacticalDones[EPlayerPostion.G] = false;
            mTacticalDones[EPlayerPostion.F] = false;
            mTacticalDones[EPlayerPostion.C] = false;

            for(var i = 0; i < Players.Count; i++)
            {
                Players[i].ResetMove();
            }
        }

        public override void Enter(object extraInfo)
        {
            clearPlayerTacticals();
        }

        public override void HandleMessage(Telegram<EGameMsg> msg)
        {
            if(msg.Msg == EGameMsg.PlayerTacticalDone)
            {
                // 以下的邏輯是當全部球員的戰術跑完時, 全部的球員指定新的戰術.
                // 目前的邏輯是 3 位球員跑戰術, 但是先跑完戰術的球員會先自行更換新戰術.
                // (戰術是在 UpdateAI 不斷的送新的給球員)
                // 當全部球員的戰術跑完, 才會全部球員的戰術重置, 全部的球員重跑一次新戰術.
                mTacticalDones[(EPlayerPostion)msg.ExtraInfo] = true;

                if(isAllPlayerTacticalDone())
                {
                    clearPlayerTacticals();
                    randomTacticalToPlayer();
                }
            }
        }

        public override void UpdateAI()
        {
            if(GameController.Get.GamePlayers.Count <= 0)
                return;

            randomTacticalToPlayer();
        }
    }
}