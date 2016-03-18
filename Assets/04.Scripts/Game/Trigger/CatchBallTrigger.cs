using UnityEngine;
using System.Collections;

public class CatchBallTrigger : MonoBehaviour {
	public PlayerBehaviour Player;
	
	void OnTriggerEnter(Collider other) {
		if (GameController.Visible){
			if (other.gameObject.CompareTag("RealBallTrigger"))
			{
//				PlayerTrigger obj = other.gameObject.GetComponent<PlayerTrigger>();
//				if (obj){
//					GameController.Get.DefRangeTouch(Player, obj.Player);
//				}
			}
		}
	}
}
