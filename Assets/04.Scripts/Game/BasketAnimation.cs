using UnityEngine;
using System.Collections;

public class BasketAnimation : MonoBehaviour {
	public int Team;

	public void AnimationEvent(string animationName) {
		CourtMgr.Get.RealBallPath(Team, animationName);
	}

	public void PlaySound(string soundName)
	{
		AudioMgr.Get.PlaySound (soundName);
	}
}
