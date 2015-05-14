using UnityEngine;
using System.Collections;



public class GameJoystick: MonoBehaviour {
	public float MoveSpeed = 10;
	public EasyJoystick Joystick;
	private float rootWidth;
	private float rootHeight;

	public void CheckVirtualScreen() {
		GameObject obj = GameObject.Find("Singleton of VirtualScreen");
		if (obj) {
			VirtualScreen vs = obj.GetComponent<VirtualScreen>();
			if (vs) {
				vs.virtualWidth = rootWidth;
				vs.virtualHeight = rootHeight;
				vs.ComputeScreen();
			}
		}
	}

	void OnEnable(){
		EasyJoystick.On_JoystickMoveStart += On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove += On_JoystickMove;	
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;

		rootWidth = UI2D.Get.RootWidth;
		rootHeight = UI2D.Get.RootHeight;
	}

	void On_JoystickMoveStart(MovingJoystick move) {

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
