using UnityEngine;
using System.Collections;

public class BasketBehaviour : MonoBehaviour {
	public int Team;
	private Animator animator;
	private Transform dummyHoop;
	private bool isBesideBasket = false;

	void Awake(){
	}
	void Update(){
		if(isBesideBasket) {
			float dis = Vector3.Distance(transform.position, SceneMgr.Get.RealBall.transform.position);
			if(dis >= 0.5f && dis < 1f) {
				SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwishEnd, dummyHoop);
			} else
			if(dis >= 1.5f) {
				GameController.Get.IsAnimationEnd = true;
				isBesideBasket = false;
			}

		}
	}

	void OnTriggerEnter(Collider c) {
		if(dummyHoop == null)
			dummyHoop = SceneMgr.Get.BasketHoopAni[Team].transform.FindChild("DummyHoop");
		if(animator == null)
			animator = SceneMgr.Get.BasketHoopAni[Team].GetComponent<Animator>();
		if (c.tag == "RealBall") {
			if(!GameController.Get.IsDunk) {
				if (GameController.Visible) {
					if(GameController.Get.IsAnimationEnd) {
						if(GameController.Get.IsScore) {
							if(GameController.Get.IsSwich) {
								isBesideBasket = true;//avoid ball touch basket
								SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwish, dummyHoop);
								SceneMgr.Get.PlayShoot(Team);
								GameController.Get.PlusScore(Team);
							} else {
								SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
								if(animator != null)
									animator.SetTrigger(GameController.Get.BasketScoreAnimationState[Random.Range(0, GameController.Get.BasketScoreAnimationState.Length)]);
							}
						} else {
							if(!GameController.Get.IsAirBall) {
								SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
								if(animator != null)
									animator.SetTrigger(GameController.Get.BasketScoreNoneAnimationState[Random.Range(0, GameController.Get.BasketScoreNoneAnimationState.Length)]);
							}
						}
						GameController.Get.IsAnimationEnd = false;
					}
				}
			}
		}
	}
}
