using UnityEngine;
using System.Collections;

public enum ECameraType
{
	UI,
	Game
}

public class Billboard : MonoBehaviour {
	private Camera referenceCamera;
	private Transform mTransform;
	public ECameraType TypeOfCamera = ECameraType.Game;

	void Awake() {

		if(TypeOfCamera == ECameraType.Game)
			referenceCamera = CameraMgr.Get.CourtCamera;

		mTransform = transform;
	}

	void Update(){
		if (referenceCamera) {
			mTransform.LookAt (transform.position + referenceCamera.transform.rotation * Vector3.forward,
	         referenceCamera.transform.rotation * Vector3.up);
		} else {
			referenceCamera = CameraMgr.Get.CourtCamera;
		}
	}

	public Camera SetCamera{
		get { return referenceCamera;}
		set { referenceCamera = value;}
	}
}
