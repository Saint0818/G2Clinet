using UnityEngine;
using System.Collections;

public class PushSkillTrigger : MonoBehaviour {
	public PlayerBehaviour pusher;
	public float InRange = 5;
	public float DelayActivityTime = 2;
	void Start () {
		StartCoroutine(DelayedExecutionMgr.Get.Execute(DelayActivityTime, StartSkill));
	}

	public void StartSkill() {
		for(int i=0; i<GameController.Get.GamePlayerList.Count; i++) {
			if(pusher != null && GameController.Get.GamePlayerList[i].Team != pusher.Team) {
				if(GameController.Get.getDis(new Vector2(GameController.Get.GamePlayerList[i].transform.position.x, GameController.Get.GamePlayerList[i].transform.position.z), 
				                             new Vector2(pusher.transform.position.x, pusher.transform.position.z)) <= InRange) {
					GameController.Get.GamePlayerList[i].AniState(EPlayerState.Fall1, pusher.transform.position);
				} else {
					GameController.Get.GamePlayerList[i].AniState(EPlayerState.Fall2, pusher.transform.position);
				}
			}
		}
	}
}
