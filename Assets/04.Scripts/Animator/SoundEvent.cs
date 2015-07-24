using UnityEngine;
using System.Collections;

public class SoundEvent : MonoBehaviour {

	public void PlaySound(string name)
	{
		AudioMgr.Get.PlaySound (name);
	}
}
