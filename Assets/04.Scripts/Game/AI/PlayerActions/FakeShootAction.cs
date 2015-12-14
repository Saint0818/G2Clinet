using G2;

namespace AI
{
    public class FakeShootAction : PlayerAIAction
    {
        public FakeShootAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return GameConst.FakeShootRate; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            float shootPointDis = MathUtils.Find2DDis(Player.transform.position,
                CourtMgr.Get.ShootPoint[Player.Team.GetHashCode()].transform.position);

            var hasDefPlayer = PlayerAI.HasDefPlayer(GameConst.ThreatDistance, GameConst.ThreatAngle);
            return isAnimationValid() &&
                   shootPointDis <= GameConst.Point3Distance + 1 && hasDefPlayer;
        }

        private bool isAnimationValid()
        {
            return Player.CanUseState(EPlayerState.FakeShoot);
        }

        public override void Do()
        {
            GameController.Get.DoShoot();
        }
    }
}