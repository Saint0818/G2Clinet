using UnityEngine;
using System.Collections;

public delegate void OnComplete();

public class DelayedExecutionMgr:KnightSingleton<DelayedExecutionMgr>{

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
