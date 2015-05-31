using UnityEngine;
using System.Collections;

public class LobbyPlayerTrigger : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (LobbyStart.Visible)
			LobbyStart.Get.PlayerOnTarget(other.gameObject, gameObject);
	}
	
	void OnTriggerStay(Collider other) {	
		if (LobbyStart.Visible)
			LobbyStart.Get.PlayerOnTarget(other.gameObject, gameObject);
	}
}
