using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RPGViewFrustum))]

public class RPGCamera : MonoBehaviour {

	public Camera UsedCamera;
	public Vector3 CameraPivotLocalPosition = new Vector3(0, 1.1f, 0);
	public bool ActivateCameraControl = true;
	public bool AlwaysRotateCamera = false; 
	public CursorLockMode CursorLockMode = CursorLockMode.Confined;
	public bool HideCursorWhenPressed = true;
	public bool LockMouseX = false;
	public bool LockMouseY = false;
	public bool InvertMouseX = false;
	public bool InvertMouseY = true;
	public float MouseXSensitivity = 8.0f;
	public float MouseYSensitivity = 8.0f;
	public float MouseYMin = -89.5f;
	public float MouseYMax = 89.5f;
	public float MouseScrollSensitivity = 15.0f;
	public float MouseSmoothTime = 0.08f;
	public float MinDistance = 0;
	public float MaxDistance = 20.0f;
	public float DistanceSmoothTime = 0.7f;
	public float StartMouseX = 0;
	public float StartMouseY = 15.0f;
	public float StartDistance = 2.0f;
	public bool AlignCameraWhenMoving = true;
	public float AlignCameraSmoothTime = 0.2f;

	// Camera pivot position in world coordinates
	private Vector3 _cameraPivotPosition;
	// Used view frustum script for camera distance/constraints computations
	private RPGViewFrustum _rpgViewFrustum;
	// Desired camera position, can be unequal to the current position because of view occultation
	private Vector3 _desiredPosition;
	// Analogous to "_desiredPosition"
	private float _desiredDistance;
	private float _distanceSmooth = 0;
	private float _distanceCurrentVelocity;
	// If true, automatically align the camera with the character
	private bool _alignCameraWithCharacter = false;
	// Current mouse/camera X rotation
	private float _mouseX = 0;
	private float _mouseXSmooth = 0;
	private float _mouseXCurrentVelocity;
	// Current mouse/camera Y rotation
	private float _mouseY = 0;
	private float _mouseYSmooth = 0;
	private float _mouseYCurrentVelocity;
	// Desired mouse/camera Y rotation, as the Y rotation can be constrained by terrain
	private float _desiredMouseY = 0;


	private void Awake() {
		// Check if there is a prescribed camera to use
		if (UsedCamera == null) {
			// Create one for usage in the following code
			GameObject camObject = new GameObject(transform.name + transform.GetInstanceID() + " Camera");
			camObject.AddComponent<Camera>();
			camObject.AddComponent<FlareLayer>();
			UsedCamera = camObject.GetComponent<Camera>();
		}

		ResetView();

		_rpgViewFrustum = GetComponent<RPGViewFrustum>();		
	}

	private void LateUpdate() {
		// Make "AlwaysRotateCamera" and "AlignCameraWhenMoving" mutual exclusive
		if (AlwaysRotateCamera) {
			AlignCameraWhenMoving = false;
		}
	
		// Set the camera's pivot position in world coordinates
		_cameraPivotPosition = transform.position + transform.TransformVector(CameraPivotLocalPosition);
		
		// Check if the camera's Y rotation is contrained by terrain
		bool mouseYConstrained = false;
		OccultationHandling occultationHandling = _rpgViewFrustum.GetOccultationHandling();
		List<string> affectingTags = _rpgViewFrustum.GetAffectingTags();
		if (occultationHandling == OccultationHandling.AlwaysZoomIn || occultationHandling == OccultationHandling.TagDependent) {
			RaycastHit hitInfo;
			mouseYConstrained = Physics.Raycast(UsedCamera.transform.position, Vector3.down, out hitInfo, 1.0f);
			
			// mouseYConstrained = "Did the ray hit something?" AND "Was it terrain?" AND "Is the camera's Y position under that of the pivot?"
			mouseYConstrained = mouseYConstrained && hitInfo.transform.GetComponent<Terrain>() && UsedCamera.transform.position.y < _cameraPivotPosition.y;
			
			if (occultationHandling == OccultationHandling.TagDependent) {
				// Additionally take into account if the hit terrain has a camera affecting tag
				mouseYConstrained = mouseYConstrained && affectingTags.Contains(hitInfo.transform.tag);
			}
		}

		#region Get inputs

		float mouseYMinLimit = _mouseY;
		// Get mouse input
		if ((Input.GetButton("Fire1") || Input.GetButton("Fire2") || AlwaysRotateCamera) && ActivateCameraControl) {
			// Apply the prescribed cursor lock mode and visibility
			Cursor.lockState = CursorLockMode;
			Cursor.visible = !HideCursorWhenPressed;

			// Get mouse X axis input
			if (!LockMouseX) {
				if (InvertMouseX) {
					_mouseX -= Input.GetAxis("Mouse X") * MouseXSensitivity;
				} else {
					_mouseX += Input.GetAxis("Mouse X") * MouseXSensitivity;
				}
			}
			
			// Get mouse Y axis input
			if (!LockMouseY) {
				if (InvertMouseY) {
					_desiredMouseY -= Input.GetAxis("Mouse Y") * MouseYSensitivity;
				} else {
					_desiredMouseY += Input.GetAxis("Mouse Y") * MouseYSensitivity;
				}
			}
			
			// Check if the camera's Y rotation is constrained by terrain
			if (mouseYConstrained) {
				_mouseY = Mathf.Clamp(_desiredMouseY, Mathf.Max(mouseYMinLimit, MouseYMin), MouseYMax);
				// Set the desired mouse Y rotation to compute the degrees of looking up with the camera
				_desiredMouseY = Mathf.Max(_desiredMouseY, _mouseY - 90.0f);
			} else {
				// Clamp the mouse between the maximum values
				_mouseY = Mathf.Clamp(_desiredMouseY, MouseYMin, MouseYMax);
			}

			_desiredMouseY = Mathf.Min(_desiredMouseY, MouseYMax);
		} else {
			// Unlock the cursor and make it visible again
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		// Get scroll wheel input
		_desiredDistance = _desiredDistance - Input.GetAxis("Mouse ScrollWheel") * MouseScrollSensitivity;
		_desiredDistance = Mathf.Clamp(_desiredDistance, MinDistance, MaxDistance);
		// Check if one of the switch buttons is pressed
		if (Input.GetButton("First Person Zoom")) {
			_desiredDistance = MinDistance;
		} else if (Input.GetButton("Maximum Distance Zoom")) {
			_desiredDistance = MaxDistance;
		}
		// Align camera when moving forward or backwards, "_alignCameraWithCharacter" gets set via its Setter method
		if (AlignCameraWhenMoving && _alignCameraWithCharacter)
			AlignCameraWithCharacter();

		#endregion

		#region Smooth the inputs

		float smoothTime = MouseSmoothTime;
		if (AlignCameraWhenMoving && _alignCameraWithCharacter) {
			smoothTime = AlignCameraSmoothTime;
		}

		_mouseXSmooth = Mathf.SmoothDamp(_mouseXSmooth, _mouseX, ref _mouseXCurrentVelocity, smoothTime);
		_mouseYSmooth = Mathf.SmoothDamp(_mouseYSmooth, _mouseY, ref _mouseYCurrentVelocity, MouseSmoothTime);

		#endregion

		#region Compute the new camera position
		Vector3 newCameraPosition;
		// Compute the desired position
		_desiredPosition = GetCameraPosition(_mouseYSmooth, _mouseXSmooth, _desiredDistance);
		// Compute the closest possible camera distance by checking if there is something inside the view frustum
		float closestDistance = _rpgViewFrustum.CheckForOccultation(_desiredPosition, _cameraPivotPosition, UsedCamera);
		
		if (closestDistance != -1) {
			// Camera view is constrained => set the camera distance to the closest possible distance 
			closestDistance -= UsedCamera.nearClipPlane;
			if (_distanceSmooth < closestDistance) {
				// Smooth the distance if we move from a smaller constrained distance to a bigger constrained distance
				_distanceSmooth = Mathf.SmoothDamp(_distanceSmooth, closestDistance, ref _distanceCurrentVelocity, DistanceSmoothTime);
			} else {
				// Do not smooth if the new closest distance is smaller than the current distance
				_distanceSmooth = closestDistance;
			}
		
		} else {
			// The camera is not constrained (anymore) so smooth the distance change
			_distanceSmooth = Mathf.SmoothDamp(_distanceSmooth, _desiredDistance, ref _distanceCurrentVelocity, DistanceSmoothTime);
		}
		// Compute the new camera position
		newCameraPosition = GetCameraPosition(_mouseYSmooth, _mouseXSmooth, _distanceSmooth);
		
		#endregion

		#region Update the camera transform

		UsedCamera.transform.position = newCameraPosition;
		// Check if we are in third or first person and adjust the camera rotation behavior
		if (_distanceSmooth > 0.1f) {
			// In third person => orbit camera
			UsedCamera.transform.LookAt(_cameraPivotPosition);
		} else {
			// In first person => normal camera rotation
			UsedCamera.transform.eulerAngles = new Vector3(_mouseYSmooth, _mouseXSmooth, 0);
		}

		if (mouseYConstrained) {
			// Camera lies on terrain => enable looking up			
			float lookUpDegrees = _desiredMouseY - _mouseY;
			UsedCamera.transform.Rotate(Vector3.right, lookUpDegrees);
		}

		#endregion
	}
	
	private Vector3 GetCameraPosition(float xAxisDegrees, float yAxisDegrees, float distance) {
		Vector3 offset = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(xAxisDegrees, yAxisDegrees, 0);

		return _cameraPivotPosition + rotation * offset;
	}

	/* Resets the camera view behind the character + starting X rotation, starting Y rotation and starting distance "StartDistance" */
	public void ResetView() {
		_mouseX = transform.eulerAngles.y + StartMouseX;
		_mouseY = _desiredMouseY = StartMouseY;
		_desiredDistance = StartDistance;
	}

	/* Rotates the camera by "degree" degrees */
	public void Rotate(float degree) {
		_mouseX += degree;
	}
	
	/* Sets the private variable "_alignCameraWithCharacter" depending on if the character is in motion */
	public void SetAlignCameraWithCharacter(bool characterMoves) {
		// Check if camera controls are activated
		if (ActivateCameraControl) {
			// Align camera with character only when the character moves AND neither "Fire1" nor "Fire2" is pressed
			_alignCameraWithCharacter = characterMoves && !Input.GetButton("Fire1") && !Input.GetButton("Fire2");			
		} else {
			// Only align the camera with the character when the character moves
			_alignCameraWithCharacter = characterMoves;
		}
	}

	/* Align the camera with the character */
	private void AlignCameraWithCharacter() {
		float characterRotation = transform.eulerAngles.y;
		// Shift the character rotation offset so it fits the interval (-180,180]
		if (characterRotation > 180f) {
			characterRotation = characterRotation - 360f;
		}
		// Compute how many full rotations we have done with the camera and the offset to being behind the character
		float offsetToCameraRotation = CustomModulo(_mouseX, 360);
		float numberOfFullRotations = (_mouseX - offsetToCameraRotation) / 360;
		
		if (_mouseX < 0) {
			if (offsetToCameraRotation < -180 + characterRotation) {
				numberOfFullRotations--;
			}
		} else {
			if (offsetToCameraRotation > 180 + characterRotation) {
				// The shortest way to rotate behind the character is to fulfill the current rotation
				numberOfFullRotations++;
			}
		}

		_mouseX = numberOfFullRotations * 360 + characterRotation;
	}

	/* A custom modulo operation for calculating mod of a negative number */
	private float CustomModulo(float dividend, float divisor) {
		if (dividend < 0) {
			return dividend - divisor * Mathf.Ceil(dividend / divisor);	
		} else {
			return dividend - divisor * Mathf.Floor(dividend / divisor);
		}
	}

	public Camera getUsedCamera() {
		return UsedCamera;
	}
	
	public bool getActivateCameraControl() {
		return ActivateCameraControl;
	}
	
	public bool getAlwaysRotateCamera() {
		return AlwaysRotateCamera;
	}
	
	/* If Gizmos are turned on, this method draws the camera pivot at its position */
	private void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(transform.position + transform.TransformVector(CameraPivotLocalPosition), 0.1f);
	}
}
