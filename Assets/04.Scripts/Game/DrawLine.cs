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
	private bool isShow = true;

	public GameObject[] UIs = new GameObject[3];
	private List<TLineVector> targets = new List<TLineVector>();
	private VectorLine line;
	// Use this for initialization
	void Start () {
		line = new VectorLine("Line", new List<Vector2>(), null, lineWidth);
		line.color = Color.red;
		line.capLength = lineWidth*0.5f;

		for (int i = 0; i < UIs.Length; i ++) {
			if (UIs[i] && UIs[i].activeInHierarchy) {
				GameObject obj = GameObject.Find("PlayerInfoModel/" + i.ToString());
				if (obj)
					AddTarget(UIs[i], obj);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isShow) {
			for (var i = 0; i < targets.Count; i++) {
				Vector2 screenPoint = CameraMgr.Get.CourtCamera.WorldToScreenPoint (targets[i].Target.transform.position);
				line.points2[i*2] = new Vector2(screenPoint.x, screenPoint.y);
				screenPoint = Camera.main.WorldToScreenPoint(targets[i].Source.transform.position);
				line.points2[i*2 + 1] = new Vector2(screenPoint.x, screenPoint.y);
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
//			line.Draw();
			line.active = isShow;
		}
	}
}
