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
			dummyHoop = SceneMgr.Get.BasketHoopDummy[Team];
		if(!animator)
			animator = SceneMgr.Get.BasketHoopAni[Team];

		if (c.tag == "RealBall") {
			if(!GameController.Get.IsDunk && (GameController.Get.situation == GameSituation.AttackA || GameController.Get.situation == GameSituation.AttackB)) {
				if (GameController.Visible) {
					if(IntTrigger == 0 && !Into){
						Into = true;
						switch (GameController.Get.BasketSituationType) {
						case BasketSituation.Score:
							SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
							if(animator != null){
								animator.SetTrigger(GameController.Get.BasketAniName);
							}
							break;
						case BasketSituation.Swich:
							SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwish, dummyHoop);
							break;
						case BasketSituation.NoScore:
							SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
							if(animator != null ){
								animator.SetTrigger(GameController.Get.BasketAniName);
							}
							break;
						}
					}else
					if(IntTrigger == 1 && SceneMgr.Get.BasketEntra[Team, 0].Into && !SceneMgr.Get.BasketEntra[Team, 1].Into) {
						Into = true;
						GameController.Get.PlusScore(Team);
						SceneMgr.Get.PlayShoot(Team);
						if(GameController.Get.BasketSituationType == BasketSituation.Swich)
							SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwishEnd, dummyHoop);
					}
				}
			}
		}
    }
}
