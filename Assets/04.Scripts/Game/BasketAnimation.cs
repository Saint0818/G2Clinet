using UnityEngine;
using System.Collections;

public class BasketAnimation : MonoBehaviour {
	public int Team;
	private bool isBesideBasket = false;
	void Update(){
		if(isBesideBasket) {
			if(Vector3.Distance(transform.position, SceneMgr.Get.RealBall.transform.position) >= 2.5f) {
				GameController.Get.IsAnimationEnd = true;
				isBesideBasket = false;
			}
		}
	}
	public void AnimationEvent(string animationName) {
		switch(animationName) {
		case "ActionEnd":
			isBesideBasket = true;
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionEnd, SceneMgr.Get.BasketHoopAni[Team].transform.FindChild("DummyHoop"));
			break;
		case "BasketNetPlay":
			SceneMgr.Get.PlayShoot(Team);
			GameController.Get.PlusScore(Team);
			break;
		}
	}
}
