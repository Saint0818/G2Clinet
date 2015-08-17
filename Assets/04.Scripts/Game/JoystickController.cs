using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoystickController : MonoBehaviour {

	public enum EJoystickBtn
	{
		A = 0,
		B = 1,
		X = 2,
		Y = 3,
		L1 = 4,
		R1 = 5,
		L2 = 6,
		R2 = 7,
		Start = 10,
		Select =11
	}

	private string currentButton;
	private string currentAxis;
	private Vector2 axisInput;

	private bool x;
	private bool y;
	private bool isShowGUI = false;
	private bool isPressShoot = false;
	public bool IsUseControl = false;

	private string[] BtnsName = new string[20];
	private MovingJoystick move = new MovingJoystick();

	void Start () {
		for (int i = 0; i < BtnsName.Length; i++){
			BtnsName [i] = string.Format ("joystick button {0}", i);
		}
	}

	void FixedUpdate () 
	{
		getAxis();
		getButton();
	}

	void getAxis()
	{
		if (!GameController.Get.IsStart) {
			return;
		}

		axisInput.x = Input.GetAxisRaw("X axis");
		axisInput.y = Input.GetAxisRaw("Y axis");

		if(axisInput.x > 0.1f || axisInput.x < -0.1f)
		{
			IsUseControl = true;
		}
		
		if(axisInput.y > 0.1f || axisInput.y < -0.1f)
		{
			IsUseControl = true;
		}

		if (IsUseControl) {
			x = Input.GetAxisRaw("X axis") > 0.1f || Input.GetAxisRaw("X axis") < -0.1f;
			y = Input.GetAxisRaw("Y axis") > 0.1f || Input.GetAxisRaw("Y axis") < -0.1f;

			if(!x && !y)
			{
				move.joystickAxis.x = 0;
				move.joystickAxis.y = 0;
				
				GameController.Get.OnJoystickMoveEnd(move);
				IsUseControl = false;
			}
			else
			{
				move.joystickAxis.x = - 1 * axisInput.x;
				move.joystickAxis.y = axisInput.y;
				
				GameController.Get.OnJoystickMove(move);
			}
		}

		currentAxis = "result : " + "IsUseControl : " + IsUseControl;
//		if(Input.GetAxisRaw("3rd axis")> 0.3|| Input.GetAxisRaw("3rd axis") < -0.3)
//		{
//			currentAxis = "3rd axis";
//			axisInput = Input.GetAxisRaw("3rd axis");
//		}
//		
//		if(Input.GetAxisRaw("4th axis")> 0.3|| Input.GetAxisRaw("4th axis") < -0.3)
//		{
//			currentAxis = "4th axis";
//			axisInput = Input.GetAxisRaw("4th axis");
//		}
//		
//		if(Input.GetAxisRaw("5th axis")> 0.3|| Input.GetAxisRaw("5th axis") < -0.3)
//		{
//			currentAxis = "5th axis";
//			axisInput = Input.GetAxisRaw("5th axis");
//		}
//		
//		if(Input.GetAxisRaw("6th axis")> 0.3|| Input.GetAxisRaw("6th axis") < -0.3)
//		{
//			currentAxis = "6th axis";
//			axisInput = Input.GetAxisRaw("6th axis");
//		}
//		
//		if(Input.GetAxisRaw("7th axis")> 0.3|| Input.GetAxisRaw("7th axis") < -0.3)
//		{
//			currentAxis = "7th axis";
//			axisInput = Input.GetAxisRaw("7th axis");
//		}
//		
//		if(Input.GetAxisRaw("8th axis") > 0.3|| Input.GetAxisRaw("8th axis") < -0.3)
//		{
//			currentAxis = "8th axis";
//			axisInput = Input.GetAxisRaw("8th axis");
//		}
	}

	void getButton()
	{
		switch (SceneMgr.Get.CurrentScene) {

			case SceneName.Null:
			case SceneName.Lobby:
			case SceneName.Main:
				break;

			case SceneName.SelectRole:
//				SelectRoleFunction();
				break;

			default:
//				SelectRoleFunction();
				InGameBtnFunction();
				break;
		}   
	}



	private void InGameBtnFunction()
	{
		if (!GameController.Get.IsStart)
			return;

		switch (controllerState) {
			case EUIControl.AttackA:
				if(Input.GetButtonDown(BtnsName[EJoystickBtn.A.GetHashCode()]) && isPressShoot == false)
				{
					isPressShoot = true;
					UIGame.Get.DoShoot(null, true);
				}

				if(Input.GetButtonUp(BtnsName[EJoystickBtn.A.GetHashCode()]))
				{
					isPressShoot = false;
					UIGame.Get.DoShoot(null, false);
				}

				if(Input.GetButton(BtnsName[EJoystickBtn.X.GetHashCode()]))
					UIGame.Get.DoPassTeammateA(null, true);

				if(Input.GetButton(BtnsName[EJoystickBtn.Y.GetHashCode()]))
					UIGame.Get.DoPassTeammateB(null, true);

				if(Input.GetButton(BtnsName[EJoystickBtn.B.GetHashCode()]))
					UIGame.Get.DoPassNone();
				break;

			case EUIControl.AttackB:
				if(Input.GetButton(BtnsName[EJoystickBtn.A.GetHashCode()]))
			    	UIGame.Get.DoBlock();

				if(Input.GetButton(BtnsName[EJoystickBtn.B.GetHashCode()]))
					UIGame.Get.DoSteal();
				break;
		}

		if(Input.GetButton(BtnsName[EJoystickBtn.L1.GetHashCode()]))
			UIGame.Get.DoSkill();

		if(Input.GetButton(BtnsName[EJoystickBtn.R1.GetHashCode()]))
			UIGame.Get.DoAttack();

		if (Input.GetButton (BtnsName [EJoystickBtn.Select.GetHashCode ()]))
			isShowGUI = !isShowGUI;
	}

	private void SelectRoleFunction()
	{
		if(Input.GetButton("joystick button 0"))
			currentButton = "joystick button 0";
		
		if (Input.GetButton ("joystick button 1")) {
			currentButton = "joystick button 1";
		}
		
		if(Input.GetButton("joystick button 2"))
			currentButton = "joystick button 2";
		
		if(Input.GetButton("joystick button 3"))
			currentButton = "joystick button 3";
		
		if(Input.GetButton("joystick button 4"))
			currentButton = "joystick button 4";
		
		if(Input.GetButton("joystick button 5"))
			currentButton = "joystick button 5";
		
		if(Input.GetButton("joystick button 6"))
			currentButton = "joystick button 6";
		
		if(Input.GetButton("joystick button 7"))
			currentButton = "joystick button 7";
		
		if(Input.GetButton("joystick button 8"))
			currentButton = "joystick button 8";
		
		if(Input.GetButton("joystick button 9"))
			currentButton = "joystick button 9";
		
		if(Input.GetButton("joystick button 10"))
			currentButton = "joystick button 10";
		
		if(Input.GetButton("joystick button 11"))
			currentButton = "joystick button 11";
		
		if(Input.GetButton("joystick button 12"))
			currentButton = "joystick button 12";
		
		if(Input.GetButton("joystick button 13"))
			currentButton = "joystick button 13";
		
		if(Input.GetButton("joystick button 14"))
			currentButton = "joystick button 14";
		
		if(Input.GetButton("joystick button 15"))
			currentButton = "joystick button 15";
		
		if(Input.GetButton("joystick button 16"))
			currentButton = "joystick button 16";
		
		if(Input.GetButton("joystick button 17"))
			currentButton = "joystick button 17";
		
		if(Input.GetButton("joystick button 18"))
			currentButton = "joystick button 18";
		
		if(Input.GetButton("joystick button 19"))
			currentButton = "joystick button 19";
	}

	private EUIControl controllerState;

	public void UIMaskState(EUIControl state)
	{
		if (controllerState != state) {
			controllerState = state;
			isPressShoot = false;
		}
	}

	/// <summary>
	/// show the data onGUI
	/// </summary>
	void OnGUI()
	{
		if(isShowGUI){
			GUI.Toggle (new Rect (400, 50, 50, 50), IsUseControl, "Control");
			GUI.TextArea(new Rect(400, 100, 250, 50), "Current Button : " + currentButton);
			GUI.TextArea(new Rect(400, 250, 250, 50), "Axis Value : " +  axisInput);
			GUI.TextArea(new Rect(400, 150, 250, 50), "X : " +  x);
			GUI.TextArea(new Rect(400, 200, 250, 50), "Y: " +  y);
			GUI.TextArea(new Rect(400, 300, 250, 50), "Current : " + currentAxis);
		}
	}
}
