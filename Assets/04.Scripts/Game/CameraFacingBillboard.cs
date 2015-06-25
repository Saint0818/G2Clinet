using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {
	public Camera referenceCamera;
	private Transform mTransform;
	void Awake() {
		mTransform = transform;
	}

	void Update(){
		mTransform.LookAt(transform.position + referenceCamera.transform.rotation * Vector3.forward,
		                 referenceCamera.transform.rotation * Vector3.up);
	}

	public Camera SetCamera{
		get { return referenceCamera;}
		set { referenceCamera = value;}
	}
}
