using UnityEngine;
using System.Collections;

public class MallBox : MonoBehaviour {

	public Transform Pos;
	public Transform Tween;

	void Update () {
		Pos.transform.localPosition = new Vector3((Tween.transform.localScale.x * 585f) + 200, 0, 0);
	}
}
