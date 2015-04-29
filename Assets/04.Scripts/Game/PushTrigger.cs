using UnityEngine;
using System.Collections;

public class PushTrigger : MonoBehaviour {
	public int Direction = 0;
	private PlayerBehaviour pusher;
	private PlayerBehaviour faller;

	void Start()
	{
		pusher = gameObject.transform.parent.parent.gameObject.GetComponent<PlayerBehaviour>();
		gameObject.GetComponent<MeshRenderer> ().enabled = false;
		gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other) {
		if (GameController.Visible && other.gameObject.CompareTag("PlayerTrigger"))
		{
			GameObject toucher = other.gameObject.transform.parent.parent.gameObject;
			if(pusher != null && pusher.gameObject != toucher){
				faller = toucher.GetComponent<PlayerBehaviour>();
				if(faller != null && pusher.Team != faller.Team){
					faller.AniState(PlayerState.Fall0);
					gameObject.SetActive(false);
				}
			}
		}
	}
}
