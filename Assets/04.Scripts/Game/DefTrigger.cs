using UnityEngine;
using System.Collections;

public class DefTrigger : MonoBehaviour {
	public int Direction = 0;
	public PlayerBehaviour Player;
	
	void OnTriggerEnter(Collider other) {
		if (GameController.Visible){
			if (other.gameObject.CompareTag("PlayerTrigger"))
			{
				PlayerTrigger obj = other.gameObject.GetComponent<PlayerTrigger>();
				if (obj){
					GameController.Get.DefRangeTouch(Player, obj.Player);
					obj.Player.IsTouchPlayerForCheckLayer(1);
				}
			}
			else if(other.gameObject.CompareTag ("RealBallTrigger")){
				GameController.Get.DefRangeTouchBall(Player);

			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (GameController.Visible) {
			if (other.gameObject.CompareTag("PlayerTrigger")){
				PlayerTrigger obj = other.gameObject.GetComponent<PlayerTrigger>();
				if (obj){
					obj.Player.IsTouchPlayerForCheckLayer(-1);
				}	
			}

		}
	}
}
