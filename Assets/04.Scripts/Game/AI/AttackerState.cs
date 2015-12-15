using System.Collections.Generic;

namespace AI
{
    /// <summary>
    /// 進攻方相同的邏輯.
    /// </summary>
    /// 實作子類別:
    /// <list type="number">
    /// <item> 要指定 Players. 目前認為最好的時機點是在 State.Enter 設定. </item>
    /// <item> Call sendTactical() 將戰術傳遞給進攻方球員. </item>
    /// </list>
    public abstract class AttackerState : State<EGameSituation, EGameMsg>
    {
        protected List<PlayerBehaviour> Players { get {return mPlayers;} }
        private readonly List<PlayerBehaviour> mPlayers = new List<PlayerBehaviour>();

        /// <summary>
        /// 哪位球員的戰術跑完.
        /// </summary>
        private readonly Dictionary<EPlayerPostion, bool> mTacticalDones = new Dictionary<EPlayerPostion, bool>
        {
            {EPlayerPostion.G, false},
            {EPlayerPostion.F, false},
            {EPlayerPostion.C, false}
        };

        protected static void sendTactical()
        {
            TTacticalData tactical;
            if(GameController.Get.BallOwner != null)
            {
                switch(GameController.Get.BallOwner.Postion)
                {
                    case EPlayerPostion.C:
                        AITools.RandomTactical(ETacticalKind.AttackNormalC, out tactical);
                        break;
                    case EPlayerPostion.F:
                        AITools.RandomTactical(ETacticalKind.AttackNormalF, out tactical);
                        break;
                    case EPlayerPostion.G:
                        AITools.RandomTactical(ETacticalKind.AttackNormalG, out tactical);
                        break;
                    default:
                        AITools.RandomTactical(ETacticalKind.AttackNormal, out tactical);
                        break;
                }
            }
            else
                AITools.RandomTactical(ETacticalKind.AttackNormal, out tactical);

            GameMsgDispatcher.Ins.SendMesssage(EGameMsg.CoachOrderAttackTactical, tactical);
        }

        protected bool isAllPlayerTacticalDone()
        {
            foreach(KeyValuePair<EPlayerPostion, bool> pair in mTacticalDones)
            {
                if(!pair.Value)
                    return false;
            }

            return true;
        }

        protected void clearPlayerTacticals()
        {
            mTacticalDones[EPlayerPostion.G] = false;
            mTacticalDones[EPlayerPostion.F] = false;
            mTacticalDones[EPlayerPostion.C] = false;

            for(var i = 0; i < Players.Count; i++)
            {
                Players[i].ResetMove();
            }

//            for(int i = 0; i < GameController.Get.GamePlayers.Count; i++)
//            {
//                if(GameController.Get.GamePlayers[i].Team == ETeamKind.Self)
//                    GameController.Get.GamePlayers[i].ResetMove();
//            }
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
                    sendTactical();
                }
            }
        }

        public override void UpdateAI()
        {
            if(GameController.Get.GamePlayers.Count <= 0)
                return;

            sendTactical();
        }
    }
}