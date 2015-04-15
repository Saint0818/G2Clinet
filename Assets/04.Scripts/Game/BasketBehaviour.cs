﻿using UnityEngine;
using System.Collections;

public class BasketBehaviour : MonoBehaviour {
	public int Team;
	private Animator animator;
	private Transform dummyHoop;

	void Awake(){
		animator = GetComponent<Animator>();
		dummyHoop = transform.FindChild("DummyHoop");
	}

	public void AnimationEvent(string animationName) {
		switch(animationName) {
		case "Action0_End":
			GameController.Get.PlusScore(Team);
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketAction0End, dummyHoop);
			break;
		case "Action1_End":
			GameController.Get.PlusScore(Team);
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketAction1End, dummyHoop);
			break;
		case "Action2_End":
			SceneMgr.Get.SetBasketBallState(PlayerState.BasketAction2End, dummyHoop);
			break;
		}
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag == "RealBall") {
			if (GameController.Visible) {
				SceneMgr.Get.SetBasketBallState(PlayerState.BasketAnimationStart, dummyHoop);
				for(int i=0; i<GameController.Get.BaskAnimationState.Length; i++) {
					if(GameController.Get.BaskAnimationState[i] == GameController.Get.BasketAnimation) 
						animator.SetTrigger("BasketballAction_"+i);
				}
			}
		}
	}
}
