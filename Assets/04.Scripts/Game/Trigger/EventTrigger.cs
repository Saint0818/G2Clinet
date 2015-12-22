﻿using UnityEngine;
using System.Collections;

public class EventTrigger : MonoBehaviour {
	public int NextEventID;
	private float timer;

	private void onTouch(GameObject obj) {
		timer = 0;
		if (GamePlayTutorial.Get.CheckTriggerEvent(NextEventID, obj))
			gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider c) {
		if (c.CompareTag("Player")) {
			onTouch(c.gameObject);
		}
	}

	void OnTriggerStay(Collider c) {
		if (timer >= 1 && c.CompareTag("Player")) {
			onTouch(c.gameObject);
		}
	}

	void FixedUpdate() {
		timer += Time.deltaTime;
	}
}