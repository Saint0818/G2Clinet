using UnityEngine;
using System.Collections;

public class UIGame : UIBase {
	private static UIGame instance = null;
	private const string UIName = "UIGame";
	private GameJoystick Joystick = null;
	public GameController Game;
	public bool IsStart = true;
	
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
		SetBtnFun (UIName + "/ButtonA", DoSteal);
		SetBtnFun (UIName + "/ButtonB", DoJump);
		SetBtnFun (UIName + "/ButtonC", DoSkill);
	}

	public void DoJump()
	{
		Game.PlayerList [0].AniState (PlayerState.Jumper);
	}

	public void DoSteal()
	{
		Game.PlayerList [0].AniState (PlayerState.Steal);
	}

	public void DoSkill()
	{
		Game.PlayerList [0].DoDunk ();
	}

	public void DoBlock()
	{
		Game.PlayerList [0].AniState (PlayerState.Block);
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
		get{
			if(Joystick != null)
				return Joystick.targetPlayer;
			else
				return null;
		}
	}

	private MovingJoystick Move = new MovingJoystick();
	private bool IsUseKeyboard = false;

	void Update()
	{
		if (Input.GetKey(KeyCode.W)) {
			IsUseKeyboard = true;
			Move.joystickAxis.y = 1;
			Move.joystickValue.y = 10;
		} else if (Input.GetKey (KeyCode.D)) {
			IsUseKeyboard = true;
			Move.joystickAxis.y = -1;
			Move.joystickValue.y = -10;
		} else {
			Move.joystickAxis.y = 0;
			Move.joystickValue.y = 0;
		}
		
		if (Input.GetKey (KeyCode.A)) {
			IsUseKeyboard = true;
			Move.joystickAxis.x = -1;
			Move.joystickValue.x = -10;
		} else
		if (Input.GetKey (KeyCode.S)) {
			IsUseKeyboard = true;
			Move.joystickAxis.x = 1;
			Move.joystickValue.x = 10;
		} else {
			Move.joystickValue.x = 0;
			Move.joystickAxis.x = 0;
		}

		if(IsUseKeyboard)
			Game.PlayerList [0].OnJoystickMove(Move);

		IsUseKeyboard = false;
	}
}

