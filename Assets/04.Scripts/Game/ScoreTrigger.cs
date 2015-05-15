using UnityEngine;
using System.Collections;

public class ScoreTrigger : MonoBehaviour
{
	public int Team;
	public int IntTrigger;
	public bool Into = false;
	
	private Animator animator;
	private Transform dummyHoop;
	
    void OnTriggerEnter(Collider c) {
		if(!dummyHoop)
			dummyHoop = CourtMgr.Get.BasketHoopDummy[Team];
		if(!animator)
			animator = CourtMgr.Get.BasketHoopAnimator[Team];

		if (c.tag == "RealBall") {
			if(!GameController.Get.IsDunk && (GameController.Get.situation == GameSituation.AttackA || GameController.Get.situation == GameSituation.AttackB)) {
				if (GameController.Visible) {
					if(IntTrigger == 0 && !Into){
						Into = true;
						switch (GameController.Get.BasketSituationType) {
						case BasketSituation.Swich:
							CourtMgr.Get.SetBasketBallState(PlayerState.BasketActionSwish, dummyHoop);
							break;
						case BasketSituation.Score:
						case BasketSituation.NoScore:
							CourtMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
							if(animator != null ){
								if(GameController.Get.BasketAnimationName.Length > 0)
									animator.SetTrigger(GameController.Get.BasketAnimationName);
							}
							break;
						}
					}else
					if(IntTrigger == 1 && CourtMgr.Get.BasketEntra[Team, 0].Into && !CourtMgr.Get.BasketEntra[Team, 1].Into) {
						Into = true;
						GameController.Get.PlusScore(Team);
						CourtMgr.Get.PlayShoot(Team);
						if(GameController.Get.BasketSituationType == BasketSituation.Swich)
							CourtMgr.Get.SetBasketBallState(PlayerState.BasketActionSwishEnd, dummyHoop);
					}
				}
			}
		}
    }
}
