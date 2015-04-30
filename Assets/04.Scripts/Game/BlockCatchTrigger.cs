using UnityEngine;
using System.Collections;

public class BlockCatchTrigger: MonoBehaviour {
	private PlayerBehaviour blocker;

	void Start()
	{
		blocker = gameObject.transform.parent.parent.gameObject.GetComponent<PlayerBehaviour>();
		gameObject.GetComponent<MeshRenderer> ().enabled = false;
		gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other) {
	 	if (GameController.Visible && other.gameObject.CompareTag ("RealBall")) {
			if(blocker)
			SceneMgr.Get.SetBallState(PlayerState.Block, blocker);
		}
	}
}
