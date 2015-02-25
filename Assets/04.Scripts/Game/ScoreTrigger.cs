using UnityEngine;
using System.Collections;

public class ScoreTrigger : MonoBehaviour
{
	public int Team;
    public int intTrigger;
	public bool trigger0 = false;
	public bool trigger1 = false;
    public AudioClip netAudio;
	
    void OnTriggerEnter(Collider c) {
		if (c.collider.gameObject.tag == "RealBall") {
			if (UIGame.Visible) 
			{
				if(intTrigger == 0)
				{
					GameController.Get.ShootInto0 = true;
					GameController.Get.ShootInto1 = false;
				}
				else if(intTrigger == 1)
				{
					GameController.Get.ShootInto1 = true;

					if(GameController.Get.ShootInto0 == true && GameController.Get.ShootInto1 == true)
					{
						audio.clip = netAudio;
						audio.Play();
						GameController.Get.PlusScore(Team);
					}
				}
			} 
		}
    }
	
	void OnTriggerExit(Collider c) 
	{

    }
}
