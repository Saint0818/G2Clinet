using UnityEngine;
using System.Collections;

public class EnterPaint : MonoBehaviour {
	public int Team;
	void OnTriggerEnter(Collider c) {
		if (c.tag == "Player") {
			GameController.Get.PlayerEnterPaint(Team, c.gameObject);
		}
	}
}
