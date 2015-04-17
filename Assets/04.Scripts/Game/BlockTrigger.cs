using UnityEngine;
using System.Collections;

public class BlockTrigger : MonoBehaviour {
	public int Direction = 0;
	private PlayerBehaviour blocker;
	private PlayerBehaviour faller;

	void Start()
	{
		blocker = gameObject.transform.parent.parent.gameObject.GetComponent<PlayerBehaviour>();
		gameObject.GetComponent<MeshRenderer> ().enabled = true;
		gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other) {
		if (GameController.Visible && other.gameObject.CompareTag ("PlayerTrigger")) {
			GameObject toucher = other.gameObject.transform.parent.parent.gameObject;
			if (toucher.layer == LayerMask.NameToLayer("Shooter") && blocker != null && blocker.gameObject != toucher) {
					faller = toucher.GetComponent<PlayerBehaviour> ();
					if (blocker.Team != faller.Team) {
						GameController.Get.Fall (faller);
						gameObject.SetActive (false);
					}
			}
		} else if (GameController.Visible && other.gameObject.CompareTag ("RealBall")) {
			Debug.Log("Block ball");
			SceneMgr.Get.SetBallState(PlayerState.Block);
		}
	}
}
