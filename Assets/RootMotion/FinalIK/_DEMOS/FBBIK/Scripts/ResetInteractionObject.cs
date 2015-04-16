using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// Resets an interaction object to it's initial position and rotation after "resetDelay" from interaction trigger.
	/// </summary>
	public class ResetInteractionObject : MonoBehaviour {

		public float resetDelay = 0.3f; // Time since interaction trigger to reset this Transform

		private Vector3 defaultPosition;
		private Quaternion defaultRotation;

		void Start() {
			// Store the defaults
			defaultPosition = transform.position;
			defaultRotation = transform.rotation;
		}

		// Interaction has been triggered, start the enumerator
		void OnInteractionTrigger(Transform t) {
			StopAllCoroutines();
			StartCoroutine(ResetObject(Time.time + resetDelay));
		}

		// Reset after a certain delay
		private IEnumerator ResetObject(float resetTime) {
			while (Time.time < resetTime) yield return null;

			transform.parent = null;
			transform.position = defaultPosition;
			transform.rotation = defaultRotation;
		}
	}
}
