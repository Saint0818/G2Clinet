﻿using UnityEngine;
using System.Collections;
using System;

public class CamerEvent : MonoBehaviour {
	public void CamerTimeScale(AnimationEvent aniEvent) {
		float floatParam = aniEvent.floatParameter;

		//set all
		foreach (ETimerKind item in Enum.GetValues(typeof(ETimerKind)))
			TimerMgr.Get.ChangeTime (item, floatParam);
	}

	public void ZoomIn(float t) {
		CameraMgr.Get.SkillShow (gameObject); 
		CameraMgr.Get.SetRoomMode (EZoomType.In, t); 
	}
	
	public void ZoomOut(float t) {
		CameraMgr.Get.SkillShow (gameObject);
		CameraMgr.Get.SetRoomMode (EZoomType.Out, t); 
	}
}
