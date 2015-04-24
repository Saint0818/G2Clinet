﻿using UnityEngine;
using System.Collections;

public class BasketBehaviour : MonoBehaviour {
	public int Team;
	private Animator animator;
	private Transform dummyHoop;
	private bool isBesideBasket = false;

	void Awake(){
	}
	void Update(){
		if(isBesideBasket) {
			if(Vector3.Distance(transform.position, SceneMgr.Get.RealBall.transform.position) >= 0.5f) {
				GameController.Get.IsAnimationEnd = true;
				isBesideBasket = false;
				SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwishEnd, dummyHoop);
			}
		}
	}

	void OnTriggerEnter(Collider c) {
		dummyHoop = SceneMgr.Get.BasketHoopAni[Team].transform.FindChild("DummyHoop");
		animator = SceneMgr.Get.BasketHoopAni[Team].GetComponent<Animator>();
		if (c.tag == "RealBall") {
			if(!GameController.Get.IsDunk) {
				if (GameController.Visible) {
					if(GameController.Get.IsAnimationEnd) {
						if(GameController.Get.IsScore) {
							if(GameController.Get.IsSwich) {
								isBesideBasket = true;//avoid ball touch basket
								SceneMgr.Get.SetBasketBallState(PlayerState.BasketActionSwish, dummyHoop);
								SceneMgr.Get.PlayShoot(Team);
								GameController.Get.PlusScore(Team);
							} else {
								SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
								animator.SetTrigger(GameController.Get.BasketScoreAnimationState[Random.Range(0, GameController.Get.BasketScoreAnimationState.Length)]);
							}
						} else {
							if(!GameController.Get.IsAirBall) {
								SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
								animator.SetTrigger(GameController.Get.BasketScoreNoneAnimationState[Random.Range(0, GameController.Get.BasketScoreNoneAnimationState.Length)]);
							}
						}
						GameController.Get.IsAnimationEnd = false;
					}
				}
			}
		}
	}
}
