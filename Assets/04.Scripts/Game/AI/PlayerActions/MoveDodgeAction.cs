namespace AI
{
    public class MoveDodgeAction : PlayerAIAction
    {
        public MoveDodgeAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
			get { return Player.PlayerSkillController.MoveDodgeRate; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            var hasDefPlayer = PlayerAI.HasDefPlayer(GameConst.ThreatDistance, GameConst.ThreatAngle);
            return isAnimationValid() && GameController.Get.CoolDownCrossover <= 0 && hasDefPlayer;
        }

        private bool isAnimationValid()
        {
            return Player.CanUseState(EPlayerState.MoveDodge0) ||
                   Player.CanUseState(EPlayerState.MoveDodge1);
        }

        public override void Do()
        {
            GameController.Get.AIPass(Player);
            GameController.Get.PassCD.StartAgain();
        }
    }
}