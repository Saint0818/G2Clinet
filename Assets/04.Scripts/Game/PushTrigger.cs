using UnityEngine;
using System.Collections;

public class PushTrigger : MonoBehaviour {
	public int Direction = 0;
	private PlayerBehaviour pusher;
	private PlayerBehaviour faller;

	void Start()
	{
		pusher = gameObject.transform.parent.parent.gameObject.GetComponent<PlayerBehaviour>();
		gameObject.GetComponent<MeshRenderer> ().enabled = true;
		gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other) {
		if (GameController.Visible && other.gameObject.CompareTag("PlayerTrigger"))
		{
			GameObject toucher = other.gameObject.transform.parent.parent.gameObject;
			if(pusher != null && pusher.gameObject != toucher){
				faller = toucher.GetComponent<PlayerBehaviour>();
				if( pusher.Team != faller.Team){
					GameController.Get.OnFall(faller);
					gameObject.SetActive(false);
				}
			}
		}
	}
}
