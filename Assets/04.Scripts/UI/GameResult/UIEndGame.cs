using UnityEngine;
using System.Collections;

public class UIEndGame : UIBase {
	private static UIEndGame instance = null;
	private const string UIName = "UIEndGame";

	private UILabel labelEndGame;
	private Animator animator;

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
		labelEndGame = GameObject.Find(UIName + "/Center/Window/EndGameLabel").GetComponent<UILabel>();
	}

	public void ShowView () {
		Visible = true;
		animator.SetTrigger("EndGame");
	}
}

