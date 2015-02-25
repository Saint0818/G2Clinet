using UnityEngine;
using System.Collections;

public class GameJoystick: MonoBehaviour {
	public float MoveSpeed = 10;
	public EasyJoystick Joystick;

	void OnEnable(){
		EasyJoystick.On_JoystickMove += On_JoystickMove;	
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;
	}
	
	void OnDisable(){
		EasyJoystick.On_JoystickMove -= On_JoystickMove;	
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
	}
	
	void OnDestroy(){
		EasyJoystick.On_JoystickMove -= On_JoystickMove;	
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
	}

	void On_JoystickMove( MovingJoystick move){
		GameController.Get.OnJoystickMove(move);
	}
	
	void On_JoystickMoveEnd (MovingJoystick move)
	{
		GameController.Get.OnJoystickMoveEnd (move);
	}

	public bool Visible{
		get{return new Vector2(Joystick.JoystickAxis.x, Joystick.JoystickAxis.y) != Vector2.zero;}
	}
	
}
