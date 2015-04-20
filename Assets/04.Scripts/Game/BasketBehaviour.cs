using UnityEngine;
using System.Collections;

public class BasketBehaviour : MonoBehaviour {
	public int Team;
	private Animator animator;
	private Transform dummyHoop;
	private float swishTime;

	void Awake(){
		animator = GetComponent<Animator>();
		dummyHoop = transform.FindChild("DummyHoop");
	}

	public void AnimationEvent(string animationName) {
		switch(animationName) {
		case "ActionEnd":
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionEnd, dummyHoop);
			break;
		case "BasketNetPlay":
			SceneMgr.Get.PlayShoot(Team);
			GameController.Get.PlusScore(Team);
			break;
		}
	}

	void Update(){
		if(swishTime > 0) {
			swishTime -= Time.deltaTime;
			if(swishTime <=0 ){
				swishTime = 0;
				SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwishEnd, dummyHoop);
			}
		}
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag == "RealBall") {
			if (GameController.Visible) {
				if(GameController.Get.IsScore) {
					if(GameController.Get.IsSwich) {
						swishTime = 0.3f;
						SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwish, dummyHoop);
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
