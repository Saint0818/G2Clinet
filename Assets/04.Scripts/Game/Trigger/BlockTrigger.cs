using UnityEngine;
using System.Collections;

public class BlockTrigger : MonoBehaviour {
	public int Direction = 0;
	private PlayerBehaviour blocker;
	private PlayerBehaviour faller;

	void Start()
	{
		blocker = gameObject.transform.parent.parent.gameObject.GetComponent<PlayerBehaviour>();
		MeshRenderer mr = gameObject.GetComponent<MeshRenderer> ();
		if (mr)
			mr.enabled = false;

		gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other) {
		if (!GameController.Get.IsStart)
			return;

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
//									GameController.Get.SetBall();
//									CourtMgr.Get.SetBallState(EPlayerState.Block0, blocker);
									faller.DoPassiveSkill(GamePlayEnum.ESkillSituation.KnockDown0);
									gameObject.SetActive (false);

									blocker.GameRecord.Block++;
									GameController.Get.IsGameFinish();
									GameController.Get.CheckConditionText();
									if(blocker == GameController.Get.Joysticker)
										GameController.Get.ShowWord(GamePlayEnum.EShowWordType.Block, 0, blocker.ShowWord);
									}
							} else {
								if(faller.IsBallOwner)
								{
									GameController.Get.SetBall();
									CourtMgr.Get.SetBallState(EPlayerState.Block0, blocker);
								}
								faller.DoPassiveSkill(GamePlayEnum.ESkillSituation.KnockDown0);
								gameObject.SetActive (false);
							}
						}
				}
			}
		} else 
		if (GameController.Visible && other.gameObject.CompareTag ("RealBall") && !CourtMgr.Get.IsRealBallActive) {
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
						CourtMgr.Get.SetBallState(EPlayerState.Block0, blocker);
						faller.AniState(EPlayerState.Fall1);
						gameObject.SetActive (false);
					}
				}
			} else {
				blocker.IsPerfectBlockCatch = true;
				CourtMgr.Get.SetBallState(EPlayerState.Block0, blocker);
			}

			blocker.GameRecord.Block++;
			GameController.Get.IsGameFinish();
			GameController.Get.CheckConditionText();
			if(blocker == GameController.Get.Joysticker)
				GameController.Get.ShowWord(GamePlayEnum.EShowWordType.Block, 0, blocker.ShowWord);
			if (faller)
				faller.GameRecord.BeBlock++;
		}
	}
}
