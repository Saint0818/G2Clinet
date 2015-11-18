using UnityEngine;
using System.Collections;

public class PushSkillTrigger : MonoBehaviour {
	[HideInInspector]
	public PlayerBehaviour pusher;
	[HideInInspector]
	public float InRange = 5;
	public float DelayActivityTime = 0;
	private float delay;
	void OnEnable () {
		if (DelayActivityTime > 0)
			delay = DelayActivityTime;
		else
			delay = 0.05f;
	}

	void FixedUpdate (){
		if(delay > 0) {
			delay -= Time.deltaTime * TimerMgr.Get.CrtTime;
			if(delay <= 0)
				StartSkill();
		}
	}

	public void StartSkill() {
		if(pusher != null) {
			Debug.LogWarning(" Range:"+InRange);
			pusher.GameRecord.Push ++;
			GameController.Get.IsGameFinish();
			for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
				if(GameController.Get.GamePlayers[i].Team != pusher.Team) {
					if(GameController.Get.GetDis(new Vector2(GameController.Get.GamePlayers[i].transform.position.x, GameController.Get.GamePlayers[i].transform.position.z), 
					                             new Vector2(pusher.transform.position.x, pusher.transform.position.z)) <= InRange) {
						GameController.Get.GamePlayers[i].AniState(EPlayerState.Fall1, pusher.transform.position);
					} 
//					else {
//						GameController.Get.GamePlayers[i].AniState(EPlayerState.Fall2, pusher.transform.position);
//					}
				} 
			}
		}else 
			Debug.LogError("Pusher is null!! ");
	}
}
