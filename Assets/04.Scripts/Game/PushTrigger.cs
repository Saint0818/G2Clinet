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
				if(pusher && faller && pusher.Team != faller.Team){

					int rate = UnityEngine.Random.Range(0, 100);
					
					if(rate < faller.Attr.StrengthRate)
					{
						if(faller.AniState(PlayerState.Fall2, pusher.transform.position))
							faller.SetAnger(GameConst.DelAnger_Fall2);
					}
					else
					{
						if(faller.AniState(PlayerState.Fall1, pusher.transform.position))
							faller.SetAnger(GameConst.DelAnger_Fall1);
					}

					pusher.SetAnger(GameConst.AddAnger_Push);
					gameObject.SetActive(false);
				}
			}
		}
	}
}
