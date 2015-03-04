using UnityEngine;
using System.Collections;

public class DunkTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if(other.tag == "DummyBall")
		{
			if(other.gameObject.transform.parent && other.gameObject.transform.parent.GetComponent<PlayerBehaviour>())
				GameController.Get.OnDunkInto(other.gameObject.transform.parent.GetComponent<PlayerBehaviour>());
		}
	}
}
