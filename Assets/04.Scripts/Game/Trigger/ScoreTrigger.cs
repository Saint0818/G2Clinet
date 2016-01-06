using UnityEngine;
using System.Collections;
using GameEnum;

public class ScoreTrigger : MonoBehaviour
{
	public int Team;
	public int IntTrigger;
	public bool Into = false;
	
	private Animator animator;
	private Transform dummyHoop;

	private void shoot (){
		if(!dummyHoop)
			dummyHoop = CourtMgr.Get.BasketHoopDummy[Team];
		if(!animator)
			animator = CourtMgr.Get.BasketHoopAnimator[Team];

		if(!GameController.Get.IsDunk && !GameController.Get.IsAlleyoop && !GameController.Get.IsPassing &&
		   GameController.Get.BasketSituation != EBasketSituation.AirBall) {
			if (GameController.Visible) {
				if(IntTrigger == 0 && !Into){
					Into = true;
					CourtMgr.Get.RealBallTrigger.IsAutoRotate = false;
					CourtMgr.Get.RealBallDoMoveFinish();
					switch (GameController.Get.BasketSituation) {
					case EBasketSituation.Swish:
						if(GameStart.Get.IsDebugAnimation){
							Debug.LogWarning("RealBall Swish IN:"+ Time.time);
							GameController.Get.shootSwishTimes++;
						}
						GameController.Get.IsSwishIn = true;
						CourtMgr.Get.SetBasketState(EPlayerState.BasketActionSwish, dummyHoop, Team);
						break;
					case EBasketSituation.Score:
					case EBasketSituation.NoScore:
						if(GameStart.Get.IsDebugAnimation) {
							Debug.LogWarning("RealBall IN:"+ GameController.Get.BasketAnimationName);
							string[] nameSplit = GameController.Get.BasketAnimationName.Split("_"[0]);
							if(int.Parse(nameSplit[1]) < 100)
								GameController.Get.shootTimes ++ ;
						}
						
						CourtMgr.Get.SetBasketState(EPlayerState.BasketAnimationStart, dummyHoop, Team);
						if(animator != null ){
							if(GameController.Get.BasketAnimationName != string.Empty)
								animator.SetTrigger(GameController.Get.BasketAnimationName);
						}
						break;
					default:
						CourtMgr.Get.SetBasketState(EPlayerState.BasketActionSwish, dummyHoop, Team);
						break;
					}
				}else
				if(IntTrigger == 1 && CourtMgr.Get.BasketEntra[Team, 0].Into && !CourtMgr.Get.BasketEntra[Team, 1].Into) {
					Into = true;
					if(GameController.Get.BasketSituation == EBasketSituation.Swish){
						CourtMgr.Get.PlayShoot(Team, 0);
						CourtMgr.Get.SetBasketState(EPlayerState.BasketActionSwishEnd, dummyHoop, Team);
					}
				}
			}
		}
	}

    void OnTriggerEnter(Collider c) {
		if(c.CompareTag("RealBall"))
			shoot();

    }
}
