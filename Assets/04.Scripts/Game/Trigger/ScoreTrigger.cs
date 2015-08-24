using UnityEngine;
using System.Collections;
using GamePlayEnum;

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

		if (c.tag == "RealBall" && 
		   !GameController.Get.IsDunk && !GameController.Get.IsAlleyoop && !GameController.Get.IsPassing &&
		    GameController.Get.BasketSituation != EBasketSituation.AirBall && 
		   (GameController.Get.Situation == EGameSituation.AttackA || GameController.Get.Situation == EGameSituation.AttackB)) {
			if (GameController.Visible) {
				if(IntTrigger == 0 && !Into){
					Into = true;
					switch (GameController.Get.BasketSituation) {
					case EBasketSituation.Swish:
						CourtMgr.Get.SetBasketBallState(EPlayerState.BasketActionSwish, dummyHoop);
						break;
					case EBasketSituation.Score:
					case EBasketSituation.NoScore:
						GameController.Get.ShowShootSate(GameController.Get.BasketSituation != EBasketSituation.NoScore, Team);
						CourtMgr.Get.SetBasketBallState(EPlayerState.BasketAnimationStart, dummyHoop);
						if(animator != null ){
							if(GameController.Get.BasketAnimationName != string.Empty)
								animator.SetTrigger(GameController.Get.BasketAnimationName);
						}
						break;
					default:
						CourtMgr.Get.SetBasketBallState(EPlayerState.BasketActionSwish, dummyHoop);
						break;
					}
				}else
				if(IntTrigger == 1 && CourtMgr.Get.BasketEntra[Team, 0].Into && !CourtMgr.Get.BasketEntra[Team, 1].Into) {
					Into = true;
					GameController.Get.PlusScore(Team, false, true);
					if(GameController.Get.BasketSituation == EBasketSituation.Swish){
						CourtMgr.Get.PlayShoot(Team, 0);
						CourtMgr.Get.SetBasketBallState(EPlayerState.BasketActionSwishEnd, dummyHoop);
					}
				}
			}
		}
    }
}
