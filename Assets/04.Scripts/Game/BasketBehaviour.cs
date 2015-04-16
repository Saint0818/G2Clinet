using UnityEngine;
using System.Collections;

public class BasketBehaviour : MonoBehaviour {
	public int Team;
	private Animator animator;
	private Transform dummyHoop;

	void Awake(){
		animator = GetComponent<Animator>();
		dummyHoop = transform.FindChild("DummyHoop");
	}

	public void AnimationEvent(string animationName) {
		switch(animationName) {
		case "Action0_End":
			GameController.Get.PlusScore(Team);
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketAction0End, dummyHoop);
			break;
		case "Action1_End":
			GameController.Get.PlusScore(Team);
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketAction1End, dummyHoop);
			break;
		case "Action2_End":
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketAction2End, dummyHoop);
			break;
		case "BasketNetPlay":
			SceneMgr.Get.PlayShoot(Team);
			break;
		}
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag == "RealBall") {
			if (GameController.Visible) {
				if(GameController.Get.IsScore) {
					if(GameController.Get.IsSwich) {
						SceneMgr.Get.PlayShoot(Team);
						GameController.Get.PlusScore(Team);
					} else {
						SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
						animator.SetTrigger(GameController.Get.BasketScoreAnimationState[Random.Range(0, GameController.Get.BasketScoreAnimationState.Length)]);
					}
				} else {
					if(!GameController.Get.IsAirBall) {
						SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
						animator.SetTrigger(GameController.Get.BasketScoreNoneAnimationState[Random.Range(0, GameController.Get.BasketScoreNoneAnimationState.Length)]);
					}
				}
			}
		}
	}
}
