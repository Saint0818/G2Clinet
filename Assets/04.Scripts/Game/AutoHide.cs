using UnityEngine;
using System.Collections;

public class AutoHide : MonoBehaviour {
	public float HideTime = 0;
	void Update() {
		HideTime -= Time.deltaTime;
		if (HideTime <= 0)
			gameObject.SetActive(false);
	}
}
