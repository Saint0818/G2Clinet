using UnityEngine;
using System.Collections;

public class BasketAnimation : MonoBehaviour {
	public int Team;

	public void AnimationEvent(AnimationEvent aniEvent) {
		string animationName = aniEvent.stringParameter;
		int index = aniEvent.intParameter;
		CourtMgr.Get.RealBallPath(Team, animationName, index);
	}

	public void PlayEffect(AnimationEvent aniEvent) 
	{
		float duration = aniEvent.floatParameter;
		int eventKind = aniEvent.intParameter;
		string effectName = aniEvent.stringParameter;
		CourtMgr.Get.PlayDunkEffect(Team, effectName, eventKind, duration);
	}

	public void PlayShake (string name) {
		CameraMgr.Get.PlayShake ();
	}
}
