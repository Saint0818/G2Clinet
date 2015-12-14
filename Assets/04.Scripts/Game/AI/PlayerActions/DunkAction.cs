
namespace AI
{
    public class DunkAction : PlayerAIAction
    {
        public DunkAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return Player.Attr.DunkRate; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            return Player.CanUseState(EPlayerState.Dunk0) ||
                Player.CanUseState(EPlayerState.Dunk1) ||
                Player.CanUseState(EPlayerState.Dunk2) ||
                Player.CanUseState(EPlayerState.Dunk3) ||
                Player.CanUseState(EPlayerState.Dunk4) ||
                Player.CanUseState(EPlayerState.Dunk5) ||
                Player.CanUseState(EPlayerState.Dunk6) ||
                Player.CanUseState(EPlayerState.Dunk7) ||
                Player.CanUseState(EPlayerState.Dunk20) ||
                Player.CanUseState(EPlayerState.Dunk21) ||
                Player.CanUseState(EPlayerState.Dunk22);
        }

        public override void Do()
        {
            GameController.Get.DoShoot();
        }
    }
}