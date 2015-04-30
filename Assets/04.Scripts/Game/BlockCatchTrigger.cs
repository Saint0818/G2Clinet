using UnityEngine;
using System.Collections;

public class BlockCatchTrigger: MonoBehaviour {
	private PlayerBehaviour blocker;

	void Start()
	{
		blocker = gameObject.transform.parent.GetComponent<PlayerBehaviour>();
		BoxCollider boxcollider = gameObject.AddComponent<BoxCollider> ();
		boxcollider.size = Vector3.one * 2;
		boxcollider.isTrigger = true;
	}

	void OnTriggerEnter(Collider other) {
	 	if (GameController.Visible && other.gameObject.CompareTag ("RealBall")) {
			blocker.IsPerfectBlockCatch = true;
		}
	}
}
