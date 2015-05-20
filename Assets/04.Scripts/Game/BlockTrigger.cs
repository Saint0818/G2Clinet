using UnityEngine;
using System.Collections;

public class BlockTrigger : MonoBehaviour {
	public int Direction = 0;
	private PlayerBehaviour blocker;
	private PlayerBehaviour faller;

	void Start()
	{
		blocker = gameObject.transform.parent.parent.gameObject.GetComponent<PlayerBehaviour>();
		gameObject.GetComponent<MeshRenderer> ().enabled = false;
		gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other) {
		if (GameController.Visible && other.gameObject.CompareTag ("PlayerTrigger")) {
			GameObject toucher = other.gameObject.transform.parent.parent.gameObject;
			if (toucher.layer == LayerMask.NameToLayer("Shooter") && blocker != null && blocker.gameObject != toucher) {
				int rate = Random.Range(0, 100);
				if(rate < blocker.Attr.BlockPushRate){
						faller = toucher.GetComponent<PlayerBehaviour> ();
						if (blocker.Team != faller.Team) {
							if(faller.crtState == PlayerState.Dunk || faller.crtState == PlayerState.DunkBasket)
							{
								if(faller.IsCanBlock && !faller.IsTee)	
								{
									GameController.Get.SetBall();
									CourtMgr.Get.SetBallState(PlayerState.Block, blocker);
									faller.AniState(PlayerState.Fall1);
									gameObject.SetActive (false);
								}
							}
							else{
								faller.AniState(PlayerState.Fall1);
								gameObject.SetActive (false);
							}
						}
				}
			}
		} else if (GameController.Visible && other.gameObject.CompareTag ("RealBall")) {
			if(other.gameObject.transform.parent && other.gameObject.transform.parent.transform.parent){
				faller = other.gameObject.transform.parent.transform.parent.GetComponent<PlayerBehaviour>();
				if(faller && faller.crtState == PlayerState.Dunk || faller.crtState == PlayerState.DunkBasket)
				{
					if(faller.IsCanBlock && !faller.IsTee){
						GameController.Get.SetBall();
						CourtMgr.Get.SetBallState(PlayerState.Block, blocker);
						faller.AniState(PlayerState.Fall1);
						gameObject.SetActive (false);
					}
				}
			}else
				CourtMgr.Get.SetBallState(PlayerState.Block, blocker);
		}
	}
}
