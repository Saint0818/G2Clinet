using UnityEngine;
using System.Collections;

public class BasketAnimation : MonoBehaviour {
	public int Team;

	public void AnimationEvent(string animationName) {
		switch(animationName) {
		case "ActionEnd":
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionEnd, SceneMgr.Get.BasketHoopAni[Team].transform.FindChild("DummyHoop"));
			break;
		case "ActionNoScoreEnd":
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionNoScoreEnd, SceneMgr.Get.BasketHoopAni[Team].transform.FindChild("DummyHoop"));
			break;
		case "BasketNetPlay":
			SceneMgr.Get.PlayShoot(Team);
			SceneMgr.Get.RealBallRigidbody.velocity = Vector3.zero;
			break;
		}
	}
}
