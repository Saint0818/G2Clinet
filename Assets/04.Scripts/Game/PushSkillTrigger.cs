using UnityEngine;
using System.Collections;

public class PushSkillTrigger : MonoBehaviour {
	private PlayerBehaviour pusher;
	public float InRange = 5;
	public float DelayActivityTime = 2;
	void Start () {
		pusher = gameObject.transform.parent.gameObject.GetComponent<PlayerBehaviour>();
		Invoke("StartSkill", DelayActivityTime);
	}

	public void StartSkill() {
		for(int i=0; i<GameController.Get.GetAllPlayer.Count; i++) {
			if(GameController.Get.GetAllPlayer[i].Team != pusher.Team) {
				if(GameController.Get.getDis(new Vector2(GameController.Get.GetAllPlayer[i].transform.position.x, GameController.Get.GetAllPlayer[i].transform.position.z), 
				                             new Vector2(pusher.transform.position.x, pusher.transform.position.z)) <= InRange) {
					GameController.Get.GetAllPlayer[i].AniState(EPlayerState.Fall1, pusher.transform.position);
				} else {
					GameController.Get.GetAllPlayer[i].AniState(EPlayerState.Fall2, pusher.transform.position);
				}
			}
		}
	}

}
