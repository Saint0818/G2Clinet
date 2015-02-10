using UnityEngine;
using System.Collections;

public class UIGame : UIBase {
	private static UIGame instance = null;
	private const string UIName = "UIGame";
	public bool IsStart = true;

	private GameJoystick Joystick = null;
	public GameController Game;
	public GameObject[] ControlButtonGroup= new GameObject[2];
	
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
		Joystick.Joystick = GameObject.Find (UIName + "GameJoystick").GetComponent<EasyJoystick>();

		ControlButtonGroup [0] = GameObject.Find (UIName + "/Attack");
		ControlButtonGroup [1] = GameObject.Find (UIName + "/Defance");

		SetBtnFun (UIName + "Attack/ButtonA", DoPass);
		SetBtnFun (UIName + "Attack/ButtonB", DoShoot);
		SetBtnFun (UIName + "Attack/ButtonC", DoSkill);
		SetBtnFun (UIName + "Defance/ButtonA", DoSteal);
		SetBtnFun (UIName + "Defance/ButtonB", DoBlock);
		SetBtnFun (UIName + "Defance/ButtonC", DoSkill);
	}

	public void ChangeControl(bool IsAttack)
	{
		ControlButtonGroup [0].SetActive (IsAttack);
		ControlButtonGroup [1].SetActive (!IsAttack);
	}

	public void DoPass()
	{
		
	}

	public void DoShoot()
	{
		if(Game.ballController)
		{
			Vector3 pos = SceneMgr.Inst.ShootPoint[Game.ballController.Team.GetHashCode()].transform.position;
			Game.PlayerList [0].AniState (PlayerState.Shooting, true, pos.x, pos.z);
		}
	}

	public void DoJump()
	{
		targetPlayer.AniState (PlayerState.Jumper);
	}

	public void DoSteal()
	{
		if(Game.ballController)
			targetPlayer.AniState (PlayerState.Steal, true, Game.ballController.transform.position.x, Game.ballController.transform.position.z);
	}

	public void DoSkill()
	{

	}

	public void DoBlock()
	{
		bool isturn = false;
		Vector3 pos = Vector3.zero;
		if (Game.ballController) 
		{
			if(Vector3.Distance(targetPlayer.gameObject.transform.position, Game.ballController.gameObject.transform.position) < 5f)
			{
				isturn = true;
				pos = Game.ballController.gameObject.transform.position;

			}
		} else 
		{
			if(SceneMgr.Inst.RealBall.transform.position.y > 2 && Vector3.Distance(targetPlayer.gameObject.transform.position, SceneMgr.Inst.RealBall.transform.position) < 5)
			{
				isturn = true;
				pos = SceneMgr.Inst.RealBall.transform.position;
			}
		}

		if(isturn)
			Game.PlayerList [0].AniState (PlayerState.Block, isturn, pos.x, pos.z);
		else
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

