using UnityEngine;
using System.Collections;
using GamePlayEnum;

public class AirBallTrigger : MonoBehaviour {
	public int Team;
	public bool Into = false;
	void OnTriggerEnter(Collider c) {
		if((c.tag == "RealBall" || c.tag == "RealBallTrigger") && !Into && GameController.Get.BasketSituation == EBasketSituation.AirBall) {
			Into = true;
			GameController.Get.ShowShootSate(false, Team);
		}
	}
}
