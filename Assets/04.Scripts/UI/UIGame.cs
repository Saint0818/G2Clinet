using UnityEngine;
using System.Collections;

public class UIGame : UIBase {
	private static UIGame instance = null;
	private const string UIName = "UIGame";
	private GameJoystick Joystick = null;
	private GameController Game;
	
	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if(instance)
			instance.Show(isShow);
		else
			if(isShow)
				Get.Show(isShow);
	}
	
	public static UIGame Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGame;
			
			return instance;
		}
	}
	
	protected override void InitCom() {
		Game = gameObject.AddComponent<GameController>();
		Joystick = GameObject.Find (UIName + "/GameJoystick").GetComponent<GameJoystick>();
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public PlayerBehaviour targetPlayer{
		set{
			if(Joystick != null)
				Joystick.targetPlayer = value;
		}
	}
}

