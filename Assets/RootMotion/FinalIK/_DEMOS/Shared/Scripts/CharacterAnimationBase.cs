using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// The base abstract class for all character animation controllers.
	/// </summary>
	public abstract class CharacterAnimationBase: MonoBehaviour {

		[SerializeField] protected CharacterBase character; // The character controller
		[SerializeField] CameraController cameraController; // The camera controller
		[SerializeField] bool smoothFollow = true;
		[SerializeField] float smoothFollowSpeed = 30f;
	
		public abstract void UpdateState(System.Object state);

		// Gets the rotation pivot of the character
		public abstract Vector3 GetPivotPoint();

		// Is the animator playing the grounded state?
		public bool animationGrounded { get; protected set; }

		// Gets angle around y axis from a world space direction
		public float GetAngleFromForward(Vector3 worldDirection) {
			Vector3 local = transform.InverseTransformDirection(worldDirection);
			return Mathf.Atan2 (local.x, local.z) * Mathf.Rad2Deg;
		}

		protected virtual void Start() {
			// Disable the camera controller so we can be sure to update it after the character position has been interpolated for each frame
			cameraController.enabled = false;
		}

		void LateUpdate() {
			if (smoothFollow) {
				// Interpolate the character's position and rotation
				transform.position = Vector3.Lerp(transform.position, character.transform.position, Time.deltaTime * smoothFollowSpeed);
				transform.rotation = Quaternion.Lerp(transform.rotation, character.transform.rotation, Time.deltaTime * smoothFollowSpeed);
			} else {
				transform.position = character.transform.position;
				transform.rotation = character.transform.rotation;
			}
			
			// Update camera
			cameraController.UpdateInput();
			cameraController.UpdateTransform();
		}
	}

}
