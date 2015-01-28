using UnityEngine;
using System.Collections;

public class GameJoystick: MonoBehaviour {
	public float MoveSpeed = 10;
	public GameObject targetPlayer;

	void OnEnable(){
		EasyTouch.On_TouchDown += On_TouchDown;
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

	void On_TouchDown(Gesture gesture){
		//Manual mode
	}

	void On_JoystickMove( MovingJoystick move){
//		UIGame.Get.Game.OnJoystickMove(move);

		if (targetPlayer) {
			float angle = move.Axis2Angle(true);
			int a = 90;
			Vector3 rotation = new Vector3(0, angle + a, 0);
			targetPlayer.transform.rotation = Quaternion.Euler(rotation);
			Vector3 translate = Vector3.forward * Time.deltaTime * MoveSpeed;
			targetPlayer.transform.Translate(translate);	
		}
	}
	
	void On_JoystickMoveEnd (MovingJoystick move)
	{
//		UIGame.Get.Game.OnJoystickMoveEnd(move);
		Debug.Log("trun AiMode after 3s");
	}
	
}
