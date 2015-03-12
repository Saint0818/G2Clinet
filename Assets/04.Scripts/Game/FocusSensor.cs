using UnityEngine;
using System.Collections;

public class FocusSensor : MonoBehaviour 
{
	public FocusSensorMode Mode;

	public enum  FocusSensorMode
	{
		Center,
		CenterDown,
		CenterUp,
		LeftUp,
		LeftDown,
		Left,
		LeftTop,
		Right,
		RightUp,
		RightDown,
		RightTop
	}

	void OnTriggerEnter(Collider c) 
	{
		if (c.GetComponent<Collider>().gameObject.CompareTag("RealBall"))
			CameraMgr.Get.SetFocus (Mode, this.gameObject, c.GetComponent<Collider>().gameObject, true);
	}
}
