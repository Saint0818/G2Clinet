using UnityEngine;
using System.Collections;

public class AutoStickY : MonoBehaviour {
    private GameObject parentObject = null;
    private Vector3 initPosition;
	// Use this for initialization
	void Start () {
        initPosition = transform.localPosition;
        if (transform.parent.gameObject)
            parentObject = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (parentObject)
            transform.localPosition = new Vector3(initPosition.x, initPosition.y-parentObject.transform.localPosition.y, initPosition.z);
	}
}
