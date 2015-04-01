using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// User input for a third person character controller.
	/// </summary>
	[RequireComponent(typeof(CharacterBase))]
	public class UserControlThirdPerson : MonoBehaviour {

		// Input state
		public struct State {
			public Vector3 move;
			public Vector3 lookPos;
			public bool crouch;
			public bool jump;
		}

		[SerializeField] bool walkByDefault = false;                  // toggle for walking state
		[SerializeField] bool lookInCameraDirection = true;           // should the character be looking in the same direction that the camera is facing
		[SerializeField] bool canCrouch = true;
		[SerializeField] bool canJump = true;

		private Transform cam;                              // A reference to the main camera in the scenes transform
		private CharacterBase character;             		// A reference to the character
		private State state = new State();					// The current state of the user input
		
		// Use this for initialization
		void Start ()
		{
			// get the transform of the main camera
			cam = Camera.main.transform;
			
			// get the third person character ( this should never be null due to require component )
			character = GetComponent<CharacterBase>();
		}
		
		// Fixed update is called in sync with physics
		void FixedUpdate ()
		{
			// read inputs
			state.crouch = canCrouch && Input.GetKey(KeyCode.C);
			state.jump = canJump && Input.GetButton("Jump");

			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			
			// calculate move direction to pass to character
			Vector3 camForward = Vector3.Scale (cam.forward, new Vector3(1,0,1)).normalized;
			state.move = (v * camForward + h * cam.right).normalized;	

			bool walkToggle = Input.GetKey(KeyCode.LeftShift);

			// We select appropriate speed based on whether we're walking by default, and whether the walk/run toggle button is pressed:
			float walkMultiplier = (walkByDefault ? walkToggle ? 1 : 0.5f : walkToggle ? 0.5f : 1);

			state.move *= walkMultiplier;
			
			// calculate the head look target position
			state.lookPos = lookInCameraDirection && cam != null
				? transform.position + cam.forward * 100
					: transform.position + transform.forward * 100;
			
			// pass all parameters to the character control script
			character.UpdateState(state);
		}
	}
}

