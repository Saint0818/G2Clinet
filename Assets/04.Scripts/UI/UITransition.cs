﻿using UnityEngine;

public class UITransition : UIBase {
	private static UITransition instance = null;
	private const string UIName = "UITransition";

	private Animator transitionAnimator;
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
		
		set {
			if (instance)
				instance.Show(value);
			else
			if (value)
				Get.Show(value);
		}
	}
	
	public static UITransition Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UITransition;
			
			return instance;
		}
	}
	public static void UIShow(bool isShow){
		if (instance) {
			instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}
	
	protected override void InitCom() {
		transitionAnimator = GameObject.Find (UIName).GetComponent<Animator>();
	}

	public void SelfAttack (){
		Show(true);
		transitionAnimator.SetTrigger("AwayOffense");
	}

	public void SelfOffense (){
		Show(true);
		transitionAnimator.SetTrigger("HomeOffense");
	}
}
