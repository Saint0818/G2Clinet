using UnityEngine;
using System.Collections;

public class PlayerTrigger : MonoBehaviour {
	public int Direction = 0;
	public PlayerBehaviour Player;

	void OnTriggerEnter(Collider other) {
		if (GameController.Visible){
			if (other.tag == "PlayerTrigger") 
			{
				PlayerTrigger obj = other.gameObject.GetComponent<PlayerTrigger>();
				if (obj)
					GameController.Get.PlayerTouchPlayer(Player, obj.Player, Direction);
			}
			else 
			if (other.tag == "RealBallTrigger") 
			{
				GameController.Get.BallTouchPlayer(Player, Direction);
			} 
		}
	}

	void OnTriggerStay(Collider other) {
		if (GameController.Visible){
			if (other.tag == "RealBallTrigger") 
			{
				GameController.Get.BallTouchPlayer(Player, Direction);
			} 
		}
	}
}
