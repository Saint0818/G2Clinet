using UnityEngine;
using System.Collections;

public delegate void OnComplete();

public class DelayedExecutionMgr:KnightSingleton<DelayedExecutionMgr>{

//	private OnComplete onComplete;
//	private float time = 0;

//	void FixedUpdate (){
//		if(time > 0) {
//			time -= Time.deltaTime * TimerMgr.Get.CrtTime;
//			if(time <= 0 ) {
//				onComplete();
//			}
//		}
//	}
//
//	public void Execute (float delayTime, OnComplete callback) {
//		time = delayTime;
//		onComplete = callback;
//	}

	public IEnumerator Execute(float delayTime, OnComplete callback ) {
		float time = delayTime;
		
		while (time > 0) {
			time -= Time.deltaTime * TimerMgr.Get.CrtTime;
			yield return null;
		}
		callback();
	}

	public void StopExecute (){
		StopCoroutine("Execute");
	}
}
