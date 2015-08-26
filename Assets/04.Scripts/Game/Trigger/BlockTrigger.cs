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
							faller.SetAnger(GameConst.DelAnger_Blocked);
							blocker.SetAnger(GameConst.AddAnger_Block, faller.gameObject);

							if(faller.IsDunk) {
								if(faller.IsCanBlock && !faller.IsTee) {
									GameController.Get.SetBall();
									CourtMgr.Get.SetBallState(EPlayerState.Block, blocker);
//									faller.AniState(EPlayerState.Fall1);
									faller.AniState(EPlayerState.KnockDown0);
									gameObject.SetActive (false);
								}
							} else {
//								faller.AniState(EPlayerState.Fall1);
								faller.AniState(EPlayerState.KnockDown1);
							
//								gameObject.SetActive (false);
							}
						}
				}
			}
		} else 
		if (GameController.Visible && other.gameObject.CompareTag ("RealBall")) {
			faller = GameController.Get.Shooter;

			if (faller) {
				faller.SetAnger(GameConst.DelAnger_Blocked);
				blocker.SetAnger(GameConst.AddAnger_Block, faller.gameObject);
			} else
				blocker.SetAnger(GameConst.AddAnger_Block);

			if (other.gameObject.transform.parent && other.gameObject.transform.parent.transform.parent) {
				faller = other.gameObject.transform.parent.transform.parent.GetComponent<PlayerBehaviour>();
				if(faller && faller.IsDunk) {
					if(faller.IsCanBlock && !faller.IsTee) {
						GameController.Get.SetBall();
						CourtMgr.Get.SetBallState(EPlayerState.Block, blocker);
						faller.AniState(EPlayerState.Fall1);
						gameObject.SetActive (false);
					}
				}
			} else {
				blocker.IsPerfectBlockCatch = true;
				CourtMgr.Get.SetBallState(EPlayerState.Block, blocker);
			}

			blocker.GameRecord.Block++;
			if (faller)
				faller.GameRecord.BeBlock++;
		}
	}
}
