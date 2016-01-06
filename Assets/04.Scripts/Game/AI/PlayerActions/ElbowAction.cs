using GameEnum;

namespace AI
{
    public class ElbowAction : PlayerAIAction
    {
        public ElbowAction(PlayerAI playerAI, PlayerBehaviour player) : base(playerAI, player)
        {
        }

        public override float Probability
        {
            get { return Player.Attr.ElbowingRate; }
        }

        public override bool IsDone
        {
            get { return true; }
        }

        public override bool IsValid()
        {
            PlayerAI defPlayer;
            var stealThreat = PlayerAI.FindDefPlayer(GameConst.StealPushDistance, 160, out defPlayer);
            return isAnimationValid() && stealThreat && Player.ElbowCD.IsTimeUp();
        }

        private bool isAnimationValid()
        {
            return Player.CanUseState(EPlayerState.Elbow0) ||
                   Player.CanUseState(EPlayerState.Elbow1) ||
                   Player.CanUseState(EPlayerState.Elbow2) ||
                   Player.CanUseState(EPlayerState.Elbow20) ||
                   Player.CanUseState(EPlayerState.Elbow21);
        }

        public override void Do()
        {
			if(Player.PlayerSkillController.DoPassiveSkill(ESkillSituation.Elbow0))
            {
                Player.ElbowCD.StartAgain();
                CourtMgr.Get.ShowBallSFX(Player.Attr.PunishTime);
            }
        }
    }
}