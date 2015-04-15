using UnityEngine;
using System.Collections;

public class ScoreTrigger : MonoBehaviour
{
	public int Team;
	public int IntTrigger;
	public bool Into = false;
	
    void OnTriggerEnter(Collider c) {
//		if (c.tag == "RealBall") {
//			if (GameController.Visible) {
//				Debug.Log("IntTrigger:"+IntTrigger);
//				if(IntTrigger == 0)
//					Into = true;
//				else
//				if(IntTrigger == 1 && SceneMgr.Get.BasketEntra[Team, 0].Into && !SceneMgr.Get.BasketEntra[Team, 1].Into) {
//					Into = true;
//					GameController.Get.PlusScore(Team);
//				}
//			}
//		}
    }
}
