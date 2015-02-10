using UnityEngine;
using System.Collections;

public class FollowGameObject : MonoBehaviour {
	private GameObject target;

	public void SetTarget(GameObject obj)
	{
		target = obj;
	}

	// Update is called once per frame
	void Update () {
		if (target) {
			gameObject.transform.position = new Vector3(target.transform.position.x , 0, target.transform.position.z);	
		}
	}
}
