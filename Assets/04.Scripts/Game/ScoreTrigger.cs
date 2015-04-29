using UnityEngine;
using System.Collections;

public class ScoreTrigger : MonoBehaviour
{
	public int Team;
	public int IntTrigger;
	public bool Into = false;
	
	private Animator animator;
	private Transform dummyHoop;
	private bool isBesideBasket = false;

	void Update(){
		if(isBesideBasket) {
			float dis = Vector3.Distance(transform.position, SceneMgr.Get.RealBall.transform.position);
			if(dis >= 0.5f && dis < 1f) {
				SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwishEnd, dummyHoop);
			} else
			if(dis >= 1.5f) {
				isBesideBasket = false;
			}
			
		}
	}
	
    void OnTriggerEnter(Collider c) {
		if(dummyHoop == null)
			dummyHoop = SceneMgr.Get.BasketHoopAni[Team].transform.FindChild("DummyHoop");
		if(animator == null)
			animator = SceneMgr.Get.BasketHoopAni[Team].GetComponent<Animator>();
		if (c.tag == "RealBall") {
			if(!GameController.Get.IsDunk) {
				if (GameController.Visible) {
						if(IntTrigger == 0 && !Into){
							Into = true;
							if(GameController.Get.IsScore) {
							string basketAniName = GameController.Get.BasketScoreAnimationState[Random.Range(0, GameController.Get.BasketScoreAnimationState.Length)];
							if(GameController.Get.IsSwich) {
									SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwish, dummyHoop);
								} else {
									SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
									if(animator != null)
										animator.SetTrigger(basketAniName);
								}
							} else {
							string basketAniName = GameController.Get.BasketScoreNoneAnimationState[Random.Range(0, GameController.Get.BasketScoreNoneAnimationState.Length)];
								if(!GameController.Get.IsAirBall) {
									SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
									if(animator != null)
										animator.SetTrigger(basketAniName);
								}
							}
						}else
						if(IntTrigger == 1 && SceneMgr.Get.BasketEntra[Team, 0].Into && !SceneMgr.Get.BasketEntra[Team, 1].Into) {
							Into = true;
							GameController.Get.PlusScore(Team);
							SceneMgr.Get.PlayShoot(Team);
							if(GameController.Get.IsSwich)
								SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwishEnd, dummyHoop);
						}
				}
			}
		}
    }
}
