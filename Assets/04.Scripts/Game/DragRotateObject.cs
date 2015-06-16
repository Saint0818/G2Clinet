using UnityEngine;
using System.Collections;

public class DragRotateObject : MonoBehaviour {
	private bool onDrag = false;
	public float speed = 6f;
	private float tempSpeed;
	private float axisX;
//	private float axisY;
	void OnMouseDrag ()
	{
		onDrag = true;
		axisX=-Input.GetAxis ("Mouse X");
//		axisY=Input.GetAxis ("Mouse Y");
	}
	
	float Rigid ()
	{
		if (onDrag) {
			if (tempSpeed < speed) {
				tempSpeed += speed * Time.deltaTime*5;
			}else{
				tempSpeed=speed;
			}
		} else {
			if(tempSpeed>0){
				tempSpeed-=speed * Time.deltaTime;
			}else{
				tempSpeed = 0;
			}
		}
		return tempSpeed;
	}
	
	void Update ()
	{
		this.transform.Rotate (new Vector3 (0, axisX, 0) * Rigid (), Space.Self);
		if (!Input.GetMouseButton (0)) {
			onDrag = false;
		}
	}
}
