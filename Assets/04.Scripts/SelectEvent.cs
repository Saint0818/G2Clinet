using UnityEngine;
using System.Collections;

public class SelectEvent : MonoBehaviour {
	public void AnimationEvent(string animationName)
	{
	}

	public void EffectEvent(string name){}

	public void PlaySound(string name)
	{
		AudioMgr.Get.PlaySound (name);
	}
}
