using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerTrigger : MonoBehaviour {
	public int Direction = 0;
	public PlayerBehaviour Player;

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("PlayerTrigger"))
		{
			PlayerTrigger obj = other.gameObject.GetComponent<PlayerTrigger>();
			if (obj)
				GameController.Get.PlayerTouchPlayer(Player, obj.Player, Direction);
		}
	}
}
