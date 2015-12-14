using G2;
using GamePlayEnum;

namespace AI
{
    public class StealAction : PlayerAIAction
    {
        public StealAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return Player.Attr.PushingRate; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            if(GameController.Get.BallOwner == null || !Player.StealCD.IsTimeUp() || !isAnimationValid())
                return false;

            bool isClose = MathUtils.Find2DDis(PlayerAI.transform.position, GameController.Get.BallOwner.transform.position) <= GameConst.StealPushDistance;

            return isClose;
        }

        private bool isAnimationValid()
        {
            return Player.CanUseState(EPlayerState.Steal0);
        }

        public override void Do()
        {
            if(Player.DoPassiveSkill(ESkillSituation.Steal0, GameController.Get.BallOwner.transform.position))
                Player.StealCD.StartAgain();
        }
    }
}