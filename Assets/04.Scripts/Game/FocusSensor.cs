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
//		if (c.collider.gameObject.tag == "RealBall")
//		  CameraMgr.Inst.SetFocus (Mode, this.gameObject, c.collider.gameObject, true);
//		else if(c.collider.gameObject.tag == "HandBall") 
//		  CameraMgr.Inst.SetFocus (Mode, this.gameObject, c.collider.gameObject, false);
	}
}
