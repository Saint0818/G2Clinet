using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {
	public enum Axis {up, down, left, right, forward, back};

	public Camera referenceCamera;
	public bool reverseFace = false; 
	public Axis axis = Axis.up; 
	private Transform mTransform;
	
	// return a direction based upon chosen axis
	public Vector3 GetAxis (Axis refAxis)
	{
		switch (refAxis)
		{
		case Axis.down:
			return Vector3.down; 
		case Axis.forward:
			return Vector3.forward; 
		case Axis.back:
			return Vector3.back; 
		case Axis.left:
			return Vector3.left; 
		case Axis.right:
			return Vector3.right; 
		}
		
		// default is Vector3.up
		return Vector3.up; 		
	}
	
	void  Awake ()
	{
		// if no camera referenced, grab the main camera
		if (!referenceCamera)
			referenceCamera = Camera.main; 

		mTransform = transform;
	}
	
	void  Update ()
	{
		// rotates the object relative to the camera
		Vector3 targetPos = mTransform.position + referenceCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back) ;
		Vector3 targetOrientation = referenceCamera.transform.rotation * GetAxis(axis);
		mTransform.LookAt (targetPos, targetOrientation);
	}

	public Camera SetCamera{
		get { return referenceCamera;}
		set { referenceCamera = value;}
	}
}
