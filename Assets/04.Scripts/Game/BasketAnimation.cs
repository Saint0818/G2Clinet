using UnityEngine;
using System.Collections;

public class BasketAnimation : MonoBehaviour {
	public int Team;

	public void AnimationEvent(string animationName) {
		SceneMgr.Get.RealBallPath(Team, animationName);
	}
}
