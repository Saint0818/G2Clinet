using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// Contols animation for a third person person controller.
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class CharacterAnimationEthan: CharacterAnimationBase {

		[SerializeField] float turnSensitivity = 0.2f; // Animator turning sensitivity
		[SerializeField]  float turnSpeed = 5f; // Animator turning interpolation speed
		[SerializeField]  float runCycleLegOffset = 0.2f; // The offset of leg positions in the running cycle
		[Range(0.1f,3f)] [SerializeField] float animSpeedMultiplier = 1; // How much the animation of the character will be multiplied by

		private Animator animator;
		private Vector3 lastForward;

		protected override void Start() {
			base.Start();

			animator = GetComponent<Animator>();

			lastForward = transform.forward;
		}

		public override Vector3 GetPivotPoint() {
			return animator.pivotPosition;
		}

		// Update the Animator with the current state of the character controller
		public override void UpdateState(System.Object _state) {
			if (Time.deltaTime == 0f) return;

			CharacterThirdPerson.AnimState state = (CharacterThirdPerson.AnimState)_state;

			// Is the Animator playing the grounded animations?
			animationGrounded = animator.GetCurrentAnimatorStateInfo (0).IsName ("Grounded");

			// Jumping
			if (state.jump) {
				float runCycle = Mathf.Repeat (animator.GetCurrentAnimatorStateInfo (0).normalizedTime + runCycleLegOffset, 1);
				float jumpLeg = (runCycle < 0 ? 1 : -1) * state.forward;
				
				animator.SetFloat ("JumpLeg", jumpLeg);
			}

			// Calculate the angular delta in character rotation
			float angle = -GetAngleFromForward(lastForward);
			lastForward = transform.forward;
			angle *= turnSensitivity * 0.01f;
			angle = Mathf.Clamp(angle / Time.deltaTime, -1f, 1f);

			// Update Animator params
			animator.SetFloat("Turn", Mathf.Lerp(animator.GetFloat("Turn"), angle, Time.deltaTime * turnSpeed));
			animator.SetFloat("Forward", state.forward);
			animator.SetBool ("Crouch", state.crouch);
			animator.SetBool ("OnGround", state.onGround);

			if (!state.onGround) {
				animator.SetFloat ("Jump", state.yVelocity);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector
			if (state.onGround && state.forward > 0f) {
				animator.speed = animSpeedMultiplier;
			} else {
				// but we don't want to use that while airborne
				animator.speed = 1;
			}
		}

		// Call OnAnimatorMove manually on the character controller because it doesn't have the Animator component
		void OnAnimatorMove() {
			character.Move(animator.deltaPosition, animator.deltaRotation);
		}
	}
}
