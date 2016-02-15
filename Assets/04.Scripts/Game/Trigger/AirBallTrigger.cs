using UnityEngine;
using System.Collections;
using GameEnum;

public delegate void AirBallDelegate(int Team);
public class AirBallTrigger : MonoBehaviour {
	public int Team;
	public bool Into = false;
	public AirBallDelegate AirBallDel;

	void OnTriggerEnter(Collider c) {
		if((c.CompareTag("RealBall") || c.CompareTag("RealBallTrigger")) && !Into && GameController.Get.BasketSituation == EBasketSituation.AirBall) {
			if(AirBallDel != null)
				AirBallDel(Team);
		}
	}
}
