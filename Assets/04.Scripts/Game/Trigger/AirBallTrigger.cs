using UnityEngine;
using System.Collections;
using GameEnum;

public class AirBallTrigger : MonoBehaviour {
	public int Team;
	public bool Into = false;
	void OnTriggerEnter(Collider c) {
				if((c.CompareTag("RealBall") || c.CompareTag("RealBallTrigger")) && !Into && GameController.Get.BasketSituation == EBasketSituation.AirBall) {
			Into = true;
			GameController.Get.ShowShootSate(false, Team);
		}
	}
}
