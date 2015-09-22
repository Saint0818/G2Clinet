using UnityEngine;
using System.Collections;

public class PushSkillTrigger : MonoBehaviour {
	public PlayerBehaviour pusher;
	public float InRange = 5;
	public float DelayActivityTime;
	void OnEnable () {
		DelayActivityTime = 0.2f;
	}

	void FixedUpdate (){
		if(DelayActivityTime > 0) {
			DelayActivityTime -= Time.deltaTime * TimerMgr.Get.CrtTime;
			if(DelayActivityTime <= 0)
				StartSkill();
		}
	}

	public void StartSkill() {
		for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
			if(pusher != null && GameController.Get.GamePlayers[i].Team != pusher.Team) {
				if(GameController.Get.GetDis(new Vector2(GameController.Get.GamePlayers[i].transform.position.x, GameController.Get.GamePlayers[i].transform.position.z), 
				                             new Vector2(pusher.transform.position.x, pusher.transform.position.z)) <= InRange) {
					GameController.Get.GamePlayers[i].AniState(EPlayerState.Fall1, pusher.transform.position);
				} else {
					GameController.Get.GamePlayers[i].AniState(EPlayerState.Fall2, pusher.transform.position);
				}
			}
		}
	}
}
