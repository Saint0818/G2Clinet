using UnityEngine;
using System.Collections;

public class GameButton : ETCButton {

	public void AttachButton () {
		onDown.AddListener(OnPressDown);
		onPressed.AddListener(OnPress);
		onPressedValue.AddListener(OnPressValue);
		onUp.AddListener(OnUp);
	}

	public void OnPressDown () {
		Debug.Log ("PressDown");
	}

	public void OnPress () {
		Debug.Log ("Press");
	}

	public void OnPressValue (float value) {
		Debug.Log("OnPressValue:" + value);
	}

	public void OnUp () {
		Debug.Log("OnUp:");
	}
}
