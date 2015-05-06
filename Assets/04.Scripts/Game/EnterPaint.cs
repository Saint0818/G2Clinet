using UnityEngine;
using System.Collections;

public class EnterPaint : MonoBehaviour {
	public int Team;
	private float timer;

	void OnTriggerEnter(Collider c) {
		if (c.tag == "Player") {
			timer = 0;
			GameController.Get.PlayerEnterPaint(Team, c.gameObject);
		}
	}

	void OnTriggerStay(Collider c) {
		if (timer >= 2 && c.tag == "Player") {
			timer = 0;
			GameController.Get.PlayerEnterPaint(Team, c.gameObject);
		}
	}

	void Update() {
		timer += Time.deltaTime;
	}
}
