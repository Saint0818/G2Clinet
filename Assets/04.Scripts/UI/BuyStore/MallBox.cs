using UnityEngine;

public class MallBox : MonoBehaviour {

	private Transform close;
	private Transform closeArrow;
	private Transform tween;

	private UIPlayTween uiplayTween;
	private UIPlayTween uiplayTweenClose;

	void Awake () {
		close = transform.Find("Close");
		closeArrow = transform.Find("Close/ArrowIcon");
		closeArrow.transform.localScale = new Vector3(-1, 1, 1);
		tween = transform.Find("Tween");
		uiplayTween = transform.Find("MainBtn").GetComponent<UIPlayTween>();
		uiplayTweenClose = transform.Find("Close").GetComponent<UIPlayTween>();
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
