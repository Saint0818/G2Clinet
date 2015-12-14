
namespace AI
{
    public class PassAction : PlayerAIAction
    {
        public PassAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return Player.Attr.PassRate; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            return isAnimationValid() && GameController.Get.PassCD.IsTimeUp();
        }

        private bool isAnimationValid()
        {
            return Player.CanUseState(EPlayerState.Pass0) ||
                   Player.CanUseState(EPlayerState.Pass1) ||
                   Player.CanUseState(EPlayerState.Pass2) ||
                   Player.CanUseState(EPlayerState.Pass3) ||
                   Player.CanUseState(EPlayerState.Pass4) ||
                   Player.CanUseState(EPlayerState.Pass5) ||
                   Player.CanUseState(EPlayerState.Pass6) ||
                   Player.CanUseState(EPlayerState.Pass7) ||
                   Player.CanUseState(EPlayerState.Pass8) ||
                   Player.CanUseState(EPlayerState.Pass9) ||
                   Player.CanUseState(EPlayerState.Pass50);
        }

        public override void Do()
        {
            GameController.Get.AIPass(Player);
            GameController.Get.PassCD.StartAgain();
        }
    }
}