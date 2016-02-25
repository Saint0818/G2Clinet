using UnityEngine;
using System.Collections;
using System;

public class CamEvent : MonoBehaviour {

	public void TimeScale(AnimationEvent aniEvent) {
		float floatParam = aniEvent.floatParameter;

		if (floatParam != 1) 
		{
			TimerMgr.Get.CamEvent(1.2f, floatParam);			
		}
	}

	public void ZoomIn(float t) {
//		CameraMgr.Get.SkillShow (gameObject); 
		CameraMgr.Get.SetRoomMode (EZoomType.In, t); 
	}
	
	public void ZoomOut(float t) {
//		CameraMgr.Get.SkillShow (gameObject);
		CameraMgr.Get.SetRoomMode (EZoomType.Out, t); 
	}

	public void SetPlayerAni(AnimationEvent aniEvent)
	{
		int intParam = aniEvent.intParameter;
		string stringParam = aniEvent.stringParameter;

		GameController.Get.PlayShowAni (intParam, stringParam);
	}

}
