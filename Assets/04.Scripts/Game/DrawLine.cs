using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public struct TLineVector {
	public GameObject Source;
	public GameObject Target;

	public TLineVector(GameObject source, GameObject target) {
		Source = source;
		Target = target;
	}
}

public class DrawLine : MonoBehaviour {
	const int lineWidth = 3;
	private bool isShow = false;

	public GameObject[] UIs = new GameObject[0];
	private List<TLineVector> targets = new List<TLineVector>();
	private VectorLine line;

	public Vector2 OffsetFirst = new Vector2(-5,10);
	public Vector2 OffsetSecond = new Vector2(-20,15);

	// Use this for initialization

	public void Awake(){
		Material mat = Resources.Load("Materials/DrawLine") as Material;
		line = new VectorLine("Line", new List<Vector2>(), null, lineWidth);
		line.material = mat;
		line.capLength = lineWidth * 0.3f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isShow) {
			for (var i = 0; i < targets.Count; i++) {
				if (targets[i].Target && targets[i].Target.activeInHierarchy) {
					Vector2 screenPoint = CameraMgr.Get.CourtCamera.WorldToScreenPoint (targets[i].Target.transform.position);
					line.points2[i*2] = new Vector2(screenPoint.x + OffsetFirst.x, screenPoint.y + OffsetFirst.y);
					screenPoint = Camera.main.WorldToScreenPoint(targets[i].Source.transform.position);
					line.points2[i*2 + 1] = new Vector2(screenPoint.x + OffsetSecond.x, screenPoint.y + OffsetSecond.y);
				}
			}
		}
	}

	void LateUpdate () {
		if (isShow)
			line.Draw();
	}

	public void AddTarget(GameObject sourceUI, GameObject targerObject) {
		targets.Add(new TLineVector(sourceUI, targerObject));
		line.Resize (targets.Count * 2);
	}

	public void ClearTarget() {
		targets.Clear();
	}

	public void Show(bool show) {
		isShow = show;
		line.Draw();
	}

	public bool IsShow {
		get {return isShow;}
		set {
			isShow = value;
			line.active = isShow;
		}
	}
}
