using UnityEngine;
using System.Collections;

public class AutoFollowGameObject : MonoBehaviour 
{
	public GameObject RefGameObject;
	public GameObject Target;
	public bool FollowPosition_X = false;
	public bool FollowPosition_Y = false;
	public bool FollowPosition_Z = false;
	public bool FollowRotation_X = false;
	public bool FollowRotation_Y = false;
	public bool FollowRotation_Z = false;
	private Vector3 followPos;
	private Vector3 followRot;
	// Use this for initialization
	void Start () 
	{
		RefGameObject = gameObject;
		followPos = new Vector3 (0, 0.08f, 0);
		followRot = gameObject.transform.localEulerAngles;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Target)
		{
			if(FollowPosition_X)
				followPos.x = Target.transform.position.x;

			if(FollowPosition_Y)
				followPos.y = Target.transform.position.y;

			if(FollowPosition_Z)
				followPos.z = Target.transform.position.z;

			if(FollowRotation_X)
				followRot.x = Target.transform.localEulerAngles.x;

			if(FollowRotation_Y)
				followRot.y = Target.transform.localEulerAngles.y;

			if(FollowRotation_Z)
				followRot.z = Target.transform.localEulerAngles.z;

			gameObject.transform.position = followPos;
			gameObject.transform.localEulerAngles = followRot;
		}
	}

	public void SetTarget(GameObject obj)
	{
		if (obj) 
			Target = obj;
	}
}
