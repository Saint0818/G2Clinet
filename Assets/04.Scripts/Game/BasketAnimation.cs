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

	public void PlayEffect(AnimationEvent aniEvent) 
	{
		float duration = aniEvent.floatParameter;
		int eventKind = aniEvent.intParameter;
		string effectName = aniEvent.stringParameter;
		CourtMgr.Get.PlayDunkEffect(Team, effectName, eventKind, duration);
	}
}
