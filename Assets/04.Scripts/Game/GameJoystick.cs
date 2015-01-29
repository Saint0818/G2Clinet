using UnityEngine;
using System.Collections;

public class GameJoystick: MonoBehaviour {
	public float MoveSpeed = 10;
	public PlayerBehaviour targetPlayer;

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
		targetPlayer.OnJoystickMove(move);
	}
	
	void On_JoystickMoveEnd (MovingJoystick move)
	{
		targetPlayer.OnJoystickMoveEnd (move);
	}
	
}
