using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class RPGMotor : MonoBehaviour {

	public float WalkSpeed = 2.0f;
	public float RunSpeed = 10.0f;
	public float StrafeSpeed = 10.0f;
	public float AirborneSpeed = 2.0f;
	public float RotatingSpeed = 2.5f;
	public float SprintSpeedMultiplier = 2.0f;
	public float BackwardsSpeedMultiplier = 0.2f;
	public float JumpHeight = 10.0f;
	public int AllowedAirborneMoves = 1;
	public float SlidingThreshold = 40.0f;
	public float FallingThreshold = 6.0f;
	public float Gravity = 20.0f;

	private CharacterController _characterController;
	private Animator _animator;
	private RPGCamera _rpgCamera;
	private MotionState _currentMotionState;

	private bool hasTarget = false;
	private Vector3 _moveTarget;
	// Local player direction
	private Vector3 _playerDirection;
	// Player direction in world coordinates
	private Vector3 _playerDirectionWorld;
	private float _localRotation;
	// True if the character should jump in the current frame
	private bool _jump = false;
	// True if currently shuffling with "Fire2" pressed
	private bool _shuffleViaFire2 = false;
	// True if the character should walk
	private bool _walking = false;
	// True if the character is sprinting
	private bool _sprinting = false;
	// True if the character is sliding
	private bool _sliding = false;
	private bool _allowAirborneMovement = false;
	// Allowed Moves while airborne
	private int _airborneMovesCount = 0;


	private void Awake() {
		_characterController = GetComponent<CharacterController>();
		_animator = GetComponent<Animator>();
		_rpgCamera = GetComponent<RPGCamera>();
		_characterController.slopeLimit = SlidingThreshold + 0.2f;
	}

	public void StartMotor() {

		if (_characterController.isGrounded) {
			// Reset the counter for the number of remaining moves while airborne
			_airborneMovesCount = 0;
			// Transform the local movement direction to world space
			_playerDirectionWorld = transform.TransformDirection(_playerDirection);
			// Normalize the player's movement direction
			if (_playerDirectionWorld.magnitude > 1) {
				_playerDirectionWorld = Vector3.Normalize(_playerDirectionWorld);
			}

			float resultingSpeed = 0f;
			// Compute the speed combined of strafe and run speed
			if (_playerDirection.x != 0 || _playerDirection.z != 0) {
				resultingSpeed = (StrafeSpeed * Mathf.Abs(_playerDirection.x)
						+ RunSpeed * Mathf.Abs(_playerDirection.z))
						/ (Mathf.Abs(_playerDirection.x) + Mathf.Abs(_playerDirection.z));
			}

			// Multiply with the sprint multiplier if sprinting is active
			if (_sprinting) {
				resultingSpeed *= SprintSpeedMultiplier;
			}
			// Adjust the speed if moving backwards
			if (_playerDirection.z < 0) {
				resultingSpeed *= BackwardsSpeedMultiplier;
			}
			// Adjust the speed if walking is enabled
			if (_walking) {
				resultingSpeed = WalkSpeed;
			}

			// Apply the resulting speed
			_playerDirectionWorld *= resultingSpeed;

			// Apply the falling threshold
			_playerDirectionWorld.y = -FallingThreshold;
			// Apply sliding
			ApplySliding();
			
			// Check if the character should jump this frame
			if (_jump) {
				_jump = false;
				if (!_sliding) {
					// Only jump if we are not sliding
					_playerDirectionWorld.y = JumpHeight;
				}
			}

		} else if (_allowAirborneMovement 
		           && _airborneMovesCount <= AllowedAirborneMoves
		           && transform.InverseTransformDirection(_playerDirectionWorld).z == 0) {
			// Allow slight movement while airborne after a standing jump
			Vector3 playerDirectionWorld = transform.TransformDirection(_playerDirection);
			// Normalize the player's movement direction
			if (_playerDirectionWorld.magnitude > 1) {
				playerDirectionWorld = Vector3.Normalize(playerDirectionWorld);
			}
			// Apply the airborne speed
			playerDirectionWorld *= AirborneSpeed;
			// Set the x and z direction in order to move the character continuously
			_playerDirectionWorld.x = playerDirectionWorld.x;
			_playerDirectionWorld.z = playerDirectionWorld.z;
		}

		// Apply gravity
		_playerDirectionWorld.y -= Gravity * Time.deltaTime;
		// Move the character
		_characterController.Move(_playerDirectionWorld * Time.deltaTime);
		// Rotate the character
		transform.Rotate(Vector3.up * _localRotation);

		// Determine the current motion state
		_currentMotionState = DetermineMotionState();
		if (_animator) {
			float transitionDamping = 10.0f;
			// Pass values important for animations to the animator
			int state = (int)_currentMotionState;
			_animator.SetInteger("MotionState", state);
			_animator.SetBool("Shuffle", _shuffleViaFire2 || _localRotation != 0);
			_animator.SetFloat("StrafeDirection X", _playerDirection.x, 1.0f, transitionDamping * Time.deltaTime);
			_animator.SetFloat("StrafeDirection Z", _playerDirection.z, 1.0f, transitionDamping * Time.deltaTime);
		}
	}

	/* Applies sliding to the character if it is standing on too steep terrain  */
	private void ApplySliding() {
		RaycastHit hitInfo;

		// Cast a ray down to the ground in order to get the ground's normal vector
		if (Physics.Raycast(transform.position, Vector3.down, out hitInfo)) {
			//Debug.DrawLine(transform.position, transform.position + hitInfo.normal);
			//Debug.DrawLine(transform.position, transform.position + slopeDirection);

			Vector3 hitNormal = hitInfo.normal;
			// Compute the slope in degrees
			float slope = Vector3.Angle(hitNormal, Vector3.up);
			// Compute the sliding direction
			Vector3 slidingDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
			// Normalize the sliding direction and make it orthogonal to the hit normal
			Vector3.OrthoNormalize(ref hitNormal, ref slidingDirection);
			// Check if the slope is too steep
			if (slope > SlidingThreshold) {
				_sliding = true;
				// Apply sliding
				_playerDirectionWorld = slidingDirection * slope * 0.5f;
			} else {
				_sliding = false;
			}
		}
	}

	/* Determines the current motion state of the character by using set variables */
	private MotionState DetermineMotionState() {
		MotionState result;

		if (_characterController.isGrounded) {
			if (_playerDirection.magnitude > 0) {
				if (_walking) {
					result = MotionState.Walking;
				} else if (_sprinting) {
					result = MotionState.Sprinting;
				} else {
					result = MotionState.Running;
				}
			} else if (_sliding) {
				result = MotionState.Falling;
			} else {
				result = MotionState.Standing;
			}
		} else {
			if (_playerDirectionWorld.y >= 0) {
				result = MotionState.Jumping;
			} else {
				result = MotionState.Falling;
			}
		}

		return result;
	}

	/* Lets the character jump in the current frame, only works when character is then grounded */		
	public void Jump() {
		if (_characterController.isGrounded) {
			// Only allow jumping when the character is grounded
			_jump = true;
		}
	}
	
	/* Enables/Disables sprinting */
	public void Sprint(bool on) {
		_sprinting = on;
	}

	/* Enables/Disables sprinting with speed "speed" */
	public void Sprint(bool on, float speed) {
		_sprinting = on;
		SprintSpeedMultiplier = speed;
	}
	
	/* Toggles walking */
	public void Walk(bool toggle) {
		if (toggle) {
			_walking = !_walking;
		}
	}

	public void SetPlayerDirection(Vector3 direction) {
		_playerDirection = direction;

		if (_rpgCamera) {
			// Align the camera with the character and report character movement (forwards/backwards/strafe)
			_rpgCamera.SetAlignCameraWithCharacter(_playerDirection.z != 0 || _playerDirection.x != 0);
		}
	}

	public void AllowAirborneMovement(bool on) {
		_allowAirborneMovement = false;
		// Allow airborne movement for the current frame and increase the airborne moves counter if we are not grounded
		if (!_characterController.isGrounded && on) {
			_allowAirborneMovement = true;
			_airborneMovesCount++;
		}
	}

	/* Set the local rotation and rotate the camera the same amount if "withCamera" is true */
	public void SetLocalRotation(float rotation, bool withCamera = false) {
		_localRotation = rotation * RotatingSpeed;

		if (_rpgCamera && withCamera) {
			// Rotate the camera too for the same amount
			_rpgCamera.Rotate(rotation * RotatingSpeed);
		}
	}

	/* Align the character with the camera if "align" is true and play a additionally shuffle animations if "fireAnimation" is true */
	public void AlignCharacterWithCamera(bool align, bool fireAnimation) {
		if (_rpgCamera) {
			bool cameraControlIsActivated = _rpgCamera.getActivateCameraControl();
			
			_shuffleViaFire2 = false;
			
			if (align && cameraControlIsActivated) {
				if (fireAnimation) {
					_shuffleViaFire2 = true;
				}
				// Set the characters Y rotation to the camera's Y rotation so that they look in the same direction
				transform.eulerAngles = new Vector3(transform.eulerAngles.x, _rpgCamera.getUsedCamera().transform.eulerAngles.y, transform.eulerAngles.z);
			}
		}
	}
	
	public bool getActivateCameraControl() {
		return _rpgCamera && _rpgCamera.getActivateCameraControl();
	}
	
	public bool getAlwaysRotateCamera() {
		return _rpgCamera && _rpgCamera.getAlwaysRotateCamera();
	}

	public void MoveTo() {
		if (hasTarget) {
			transform.LookAt (new Vector3(_moveTarget.x, transform.position.y, _moveTarget.z));
			float v = 0;
			float h = 0;
			float dx = Mathf.Abs(transform.position.x - _moveTarget.x);
			float dz = Mathf.Abs(transform.position.z - _moveTarget.z);
			
			if (dx > 0)
				h = dz / dx;
			else
				h = 0;
			
			/*if (transform.position.x < _moveTarget.x)
				v = 1;
			else
				v = -1;
			
			if (transform.position.z > _moveTarget.z)
				h = -h;
			*/
			_playerDirection = new Vector3(v, 0, h).normalized; 
		}
	}

	private bool onTarget() {
		return Vector3.Distance(transform.position, _moveTarget) <= 0.3f;
	}

	public Vector3 Target {
		get {return _moveTarget;}
		set {
			_moveTarget = new Vector3(value.x, transform.position.y, value.z);
			hasTarget = !onTarget();
			if (!hasTarget) {
				_playerDirection = Vector3.zero;
			}
		}
	}
}
