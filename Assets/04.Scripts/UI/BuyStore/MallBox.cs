using UnityEngine;
using System;
using System.Collections;
using AnimationOrTween;

public class MallBox : MonoBehaviour {

	private Transform close;
	private Transform closeArrow;
	private Transform tween;

	private UIPlayTween uiplayTween;
	private UIPlayTween uiplayTweenClose;

	void Awake () {
//		DateTime departure = new DateTime(2016, 1, 1, 18, 32, 0);
//		DateTime arrival = new DateTime(2016, 1, 6, 22, 47, 0);
//		TimeSpan travelTime = DateTime.Now - departure;  
//		Debug.Log("travelTime Day: " + travelTime.Days);
//		Debug.Log("travelTime Hours: " + travelTime.Hours);
//		Debug.Log("travelTime Minutes: " + travelTime.Minutes);
//		Debug.Log("travelTime Seconds: " + travelTime.Seconds);

		close = transform.FindChild("Close");
		closeArrow = transform.FindChild("Close/ArrowIcon");
		closeArrow.transform.localScale = new Vector3(-1, 1, 1);
		tween = transform.FindChild("Tween");
		uiplayTween = transform.FindChild("MainBtn").GetComponent<UIPlayTween>();
		uiplayTweenClose = transform.FindChild("Close").GetComponent<UIPlayTween>();
		EventDelegate.Add(uiplayTween.onFinished, OnFinish, false);
		EventDelegate.Add(uiplayTweenClose.onFinished, OnFinish, false);
	}

	void Update () {
		close.transform.localPosition = new Vector3((tween.transform.localScale.x * 585f) + 200, 0, 0);
	} 

	void OnFinish () {
		if(tween.transform.localScale.x <= 0.01) 
			closeArrow.transform.localScale = new Vector3(-1, 1, 1);
		else 
			closeArrow.transform.localScale = new Vector3(1, 1, 1);
	}
}
