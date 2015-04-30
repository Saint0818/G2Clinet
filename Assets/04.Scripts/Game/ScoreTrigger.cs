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
		dummyHoop = SceneMgr.Get.BasketHoopDummy[Team];
		animator = SceneMgr.Get.BasketHoopAni[Team];

		if (c.tag == "RealBall") {
			if(!GameController.Get.IsDunk) {
				if (GameController.Visible) {
						if(IntTrigger == 0 && !Into){
							Into = true;
							if(GameController.Get.IsScore) {
							string basketAniName = GameController.Get.BasketScoreAnimationState[Random.Range(0, GameController.Get.BasketScoreAnimationState.Length)];
							if(GameController.Get.IsSwich) {
									SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwish, dummyHoop);
								} else {
									SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
									if(animator != null)
										animator.SetTrigger(basketAniName);
								}
							} else {
							string basketAniName = GameController.Get.BasketScoreNoneAnimationState[Random.Range(0, GameController.Get.BasketScoreNoneAnimationState.Length)];
								if(!GameController.Get.IsAirBall) {
									SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
									if(animator != null)
										animator.SetTrigger(basketAniName);
								}
							}
						}else
						if(IntTrigger == 1 && SceneMgr.Get.BasketEntra[Team, 0].Into && !SceneMgr.Get.BasketEntra[Team, 1].Into) {
							Into = true;
							GameController.Get.PlusScore(Team);
							SceneMgr.Get.PlayShoot(Team);
							if(GameController.Get.IsSwich)
								SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwishEnd, dummyHoop);
						}
				}
			}
		}
    }
}
