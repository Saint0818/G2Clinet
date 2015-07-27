using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BallSoundTrigger : MonoBehaviour
{
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Floor")) 
		{
			AudioMgr.Get.PlaySound (SoundType.SD_dribble);
		} 
	}
}