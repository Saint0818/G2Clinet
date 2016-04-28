using UnityEngine;
using System.Collections;

public class UIEndGame : UIBase {
	private static UIEndGame instance = null;
	private const string UIName = "UIEndGame";

	private Animator animator;

	private bool isShowOneMinute = true;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
					RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else
			if (isShow)
				Get.Show(isShow);
	}

	public static UIEndGame Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIEndGame;

			return instance;
		}
	}

	protected override void InitCom() {
		animator = gameObject.GetComponent<Animator>();

		isShowOneMinute = true;
	}

	public void ShowFinish () {
		animator.SetTrigger("TimeUp");
	}

	public void ShowOneMinute () {
		if(isShowOneMinute) {
			animator.SetTrigger("OneMinute");
			isShowOneMinute = false;
		}
	}

	public void ShowOverTime () {
		animator.SetTrigger("Overtime");
	}
}

