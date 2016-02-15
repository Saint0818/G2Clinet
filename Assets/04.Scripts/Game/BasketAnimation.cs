using UnityEngine;
using System.Collections;

public delegate void BasketDelegate(int Team, AnimationEvent aniEvent);
public class BasketAnimation : MonoBehaviour {
	public int Team;
	public BasketDelegate AnimationEventDel = null;
	public BasketDelegate PlayEffectDel = null;
	public BasketDelegate PlayShakeDel = null;
	public BasketDelegate PlayActionSoundDel = null;

	public void AnimationEvent(AnimationEvent aniEvent) {
		if(AnimationEventDel != null)
			AnimationEventDel(Team, aniEvent);
	}

	public void PlayEffect(AnimationEvent aniEvent) {
		if(PlayEffectDel != null)
			PlayEffectDel(Team, aniEvent);
	}

	public void PlayShake (AnimationEvent aniEvent) {
		if(PlayShakeDel != null)
			PlayShakeDel(Team, aniEvent);
	}

	public void PlayActionSound (AnimationEvent aniEvent) {
		if(PlayActionSoundDel != null)
			PlayActionSoundDel(Team, aniEvent);
	}
}
