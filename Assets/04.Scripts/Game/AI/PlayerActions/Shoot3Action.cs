
using G2;

namespace AI
{
    public class Shoot3Action : PlayerAIAction
    {
        public Shoot3Action(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return Player.Attr.PointRate3; }
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
                shootPointDis <= GameConst.Point3Distance + 1 && !hasDefPlayer;
        }

        private bool isAnimationValid()
        {
            return Player.CanUseState(EPlayerState.Shoot0) ||
                   Player.CanUseState(EPlayerState.Shoot1) ||
                   Player.CanUseState(EPlayerState.Shoot2) ||
                   Player.CanUseState(EPlayerState.Shoot3) ||
                   Player.CanUseState(EPlayerState.Shoot4) ||
                   Player.CanUseState(EPlayerState.Shoot5) ||
                   Player.CanUseState(EPlayerState.Shoot6) ||
                   Player.CanUseState(EPlayerState.Shoot7) ||
                   Player.CanUseState(EPlayerState.Shoot20);
        }

        public override void Do()
        {
            GameController.Get.DoShoot();
        }
    }
}

