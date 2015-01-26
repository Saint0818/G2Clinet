using UnityEngine;
using System.Collections;

public class ScoreTrigger : MonoBehaviour
{
	// Use this for initialization
	public int Team;
    public int intTrigger;
	public bool trigger0 = false;
	public bool trigger1 = false;
    public AudioClip netAudio;
	
    void OnTriggerEnter(Collider c) {
		if (c.collider.gameObject.tag == "RealBall") {
//			if (UIGame.Visible) 
//			{
//				if(intTrigger == 0)
//				{
//					UIGame.Get.Game.ShootInto0 = true;
//					UIGame.Get.Game.ShootInto1 = false;
//				}
//				else if(intTrigger == 1)
//				{
//					UIGame.Get.Game.ShootInto1 = true;
//
//					if(UIGame.Get.Game.ShootInto0 == true && UIGame.Get.Game.ShootInto1 == true)
//					{
//						audio.clip = netAudio;
//						audio.Play();
//						UIGame.Get.Game.PlusScore(Team);
//					}
//				}
//			} else 
//			if(UIRecordGame.Visible)
//			{
//				if(intTrigger == 0)
//				{
//					UIRecordGame.Get.Game.ShootInto0 = true;
//					UIRecordGame.Get.Game.ShootInto1 = false;
//				}
//				else if(intTrigger == 1)
//				{
//					UIRecordGame.Get.Game.ShootInto1 = true;
//					
//					if(UIRecordGame.Get.Game.ShootInto0 && UIRecordGame.Get.Game.ShootInto1)
//					{
//						audio.clip = netAudio;
//						audio.Play();
//						UIRecordGame.Get.Game.PlusScore(Team);
//					}
//				}
//			}
		}
    }
	
	void OnTriggerExit(Collider c) 
	{
//		if (c.collider.gameObject.tag == "RealBall") {
//			if (UIGame.Visible && UIGame.Get.Game.situation >= 1 && UIGame.Get.Game.situation <= 2) 
//				UIGame.Get.Game.RushToBall(1);
//			else 
//			if(UIRecordGame.Visible)
//				UIRecordGame.Get.Game.RushToBall(1);
//		}
    }
}
