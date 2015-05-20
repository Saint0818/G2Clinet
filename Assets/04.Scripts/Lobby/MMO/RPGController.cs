using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(RPGMotor))]

public class RPGController : MonoBehaviour {

	/* Enum for controlling if the charater should be aligned to the camera, i.e. view in the same direction */
	public enum AlignCharacter {
		Never,				// Never align the character with the camera
		WhenFire2IsPressed,	// Only align when "Fire2" is pressed
		Always				// Always align the character with the camera
	};

	public AlignCharacter AlignCharacterWithCam = AlignCharacter.WhenFire2IsPressed;
	public bool AcceptInput = false;
	private RPGMotor _rpgMotor;
	private bool _autorun = false;

	private void Awake() {
		_rpgMotor = GetComponent<RPGMotor>();

		try {
			Input.GetButton("Horizontal Strafe");
			Input.GetButton("Autorun Toggle");
			Input.GetButton("Walk Toggle");
		} catch (UnityException e) {
			Debug.LogWarning(e.Message);
		}
	}

	private void Update() {

		#region Check inputs
		if (AcceptInput) {
			// Create the local movement direction
			float vertical = Input.GetAxisRaw("Vertical");
			// Check the autorun input
			if (_autorun && 
			    (Input.GetButtonDown("Vertical") || Input.GetButtonDown("Autorun Toggle"))) {
				_autorun = false;
			} else if (Input.GetButtonDown("Autorun Toggle")) {
				_autorun = true;
			}

			bool cameraControlIsActivated = _rpgMotor.getActivateCameraControl();

			if ((cameraControlIsActivated && Input.GetButton("Fire1") && Input.GetButton("Fire2")) 
			    || _autorun) {
			    // Camera controls are enable AND both "Fire1" and "Fire2" are pressed OR autorun is on
				vertical = 1.0f;
			}

			float horizontal = Input.GetAxisRaw("Horizontal");
			float horizontalStrafe = Input.GetAxisRaw("Horizontal Strafe");

			// Strafe if the right mouse button and the "Horizontal" input are pressed at once 
			if (cameraControlIsActivated && Input.GetButton("Fire2") && Input.GetAxisRaw("Horizontal") != 0) {
				horizontalStrafe = horizontal;
				horizontal = 0f;
			}
			// Create and set the player's direction inside the motor
			Vector3 playerDirection = new Vector3(horizontalStrafe, 0, vertical);
			_rpgMotor.SetPlayerDirection(playerDirection);

			// Allow movement while airborne if the player wants to move forward/backwards or strafe
			_rpgMotor.AllowAirborneMovement(Input.GetButtonDown("Vertical") || Input.GetButtonDown("Horizontal Strafe"));

			// Set the local y-axis rotation and if the camera rotates with the character
			_rpgMotor.SetLocalRotation(horizontal, !(cameraControlIsActivated && Input.GetButton("Fire1")));
			
			bool align = false;
			if (AlignCharacterWithCam == AlignCharacter.WhenFire2IsPressed) {
				// Align only when "Fire2" is pressed
				align = Input.GetButton("Fire2");
			} else if (AlignCharacterWithCam == AlignCharacter.Always) {
				// Align if "always rotate the camera" OR "Fire1" is pressed OR "Fire2" is pressed
				align = _rpgMotor.getAlwaysRotateCamera() || Input.GetButton("Fire1") || Input.GetButton("Fire2");
			}
			// Set the alignment of the character and if an shuffling animation should be fired
			_rpgMotor.AlignCharacterWithCamera(align, Input.GetAxis("Mouse X") != 0);

			// Enable sprinting in the motor if the sprint modifier is pressed down
			_rpgMotor.Sprint(Input.GetButton("Sprint"));
			
			// Toggle walking in the motor
			_rpgMotor.Walk(Input.GetButtonUp("Walk Toggle"));
			
			// Check if the jump button is pressed down
			if (Input.GetButtonDown("Jump")) {
				// Signalize the motor to jump
				_rpgMotor.Jump();
			}
		}
		#endregion
		
		// Start the motor
		_rpgMotor.MoveTo();
		_rpgMotor.StartMotor();
	}
}
