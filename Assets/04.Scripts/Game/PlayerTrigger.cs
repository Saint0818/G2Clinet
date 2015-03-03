using UnityEngine;
using System.Collections;

public class PlayerTrigger : MonoBehaviour {
	public int Direction = 0;

	void OnTriggerEnter(Collider other) {
		if (GameController.Visible){
			if (other.tag == "PlayerTrigger") 
			{
				GameController.Get.PlayerTouchPlayer(gameObject.transform.parent.gameObject, other.gameObject.transform.parent.gameObject, Direction);
			}
			else 
			if (other.tag == "RealBallTrigger") 
			{
				GameController.Get.BallTouchPlayer(gameObject.transform.parent.gameObject, Direction);
			} 
		}
	}

	void OnTriggerStay(Collider other) {
		if (GameController.Visible){
			if (other.tag == "RealBallTrigger") 
			{
				GameController.Get.BallTouchPlayer(gameObject.transform.parent.gameObject, Direction);
			} 
		}
	}
}
