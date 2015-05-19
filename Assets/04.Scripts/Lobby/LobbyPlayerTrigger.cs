﻿using UnityEngine;
using System.Collections;

public class LobbyPlayerTrigger : MonoBehaviour {

	private bool playerOnTarget(GameObject obj) {
		if (obj.transform.parent && obj.transform.parent.name == "Myself") {
			RPGMotor motor = obj.transform.parent.gameObject.GetComponent<RPGMotor>();
			gameObject.SetActive(false);
			if (motor) {
				motor.Target = motor.transform.position;
				return true;
			}
		}

		return false;
	}

	void OnTriggerEnter(Collider other) {
		playerOnTarget(other.gameObject);
	}
	
	void OnTriggerStay(Collider other) {	
		playerOnTarget(other.gameObject);
	}
}
