using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// Third person character controller. This class is based on the ThirdPersonCharacter.cs of the Unity Exmaple Assets.
	/// </summary>
	public class CharacterThirdPerson : CharacterBase {

		// Animation state
		public struct AnimState {
			public float forward; // the forward speed
			public bool jump; // should the character be jumping?
			public bool crouch; // should the character be crouching?
			public bool onGround; // is the character grounded
			public float yVelocity; // y velocity of the character
		}

		[System.Serializable]
		public class AdvancedSettings
		{
			public float stationaryTurnSpeedMlp = 1f;			// additional turn speed added when the player is stationary (added to animation root rotation)
			public float movingTurnSpeed = 5f;					// additional turn speed added when the player is moving (added to animation root rotation)
			public float headLookResponseSpeed = 2;				// speed at which head look follows its target
			public float jumpRepeatDelayTime = 0.25f;			// amount of time that must elapse between landing and being able to jump again
			public float groundStickyEffect = 5f;				// power of 'stick to ground' effect - prevents bumping down slopes.
			public float platformFriction = 7f;					// the acceleration of adapting the velocities of moving platforms
			public float maxVerticalVelocityOnGround = 3f;		// the maximum y velocity while the character is grounded
			public float crouchCapsuleScaleMlp = 0.6f;			// the capsule collider scale multiplier while crouching
			public float velocityToGroundTangentWeight = 1f;	// the weight of rotating character velocity vector to the ground tangent
		}

		[SerializeField] CharacterAnimationBase characterAnimation; // the animation controller
		[SerializeField] protected Grounder grounder; // reference to the Grounder
		[SerializeField] float forwardAcceleration = 3f; // The acceleration of the speed of the character
		[SerializeField] float airSpeed = 6; // determines the max speed of the character while airborne
		[SerializeField] float airControl = 2; // determines the response speed of controlling the character while airborne
		[SerializeField] float jumpPower = 12; // determines the jump force applied when jumping (and therefore the jump height)
		[Range(1,4)] [SerializeField] float gravityMultiplier = 2;	// gravity modifier - often higher than natural gravity feels right for game characters
	
		[SerializeField] AdvancedSettings advancedSettings; // Container for the advanced settings class , thiss allows the advanced settings to be in a foldout in the inspector

		protected UserControlThirdPerson.State inputState = new UserControlThirdPerson.State();
		protected float forward; // The current forward speed of the character (interpolating this between the states)
		protected Vector3 lookPosSmooth;
		protected bool onGround;

		private Animator animator;
		private Vector3 lastForward, normal, platformVelocity;
		private RaycastHit hit;
		private float jumpLeg, jumpEndTime, forwardMlp, groundDistance, lastAirTime, stickyForce;

		// Use this for initialization
		protected override void Start () {
			base.Start();

			animator = GetComponent<Animator>();

			lookPosSmooth = transform.position + transform.forward * 10f;
		}

		void OnAnimatorMove() {
			Move (animator.deltaPosition, animator.deltaRotation);
		}

		// When the Animator moves
		public override void Move(Vector3 deltaPosition, Quaternion deltaRotation) {
			GetComponent<Rigidbody>().rotation *= deltaRotation;

			Vector3 velocity = deltaPosition / Time.deltaTime;

			// Add velocity of the rigidbody the character is standing on
			velocity += new Vector3(platformVelocity.x, 0f, platformVelocity.z);

			if (onGround) {
				// Rotate velocity to ground tangent
				if (velocity.y > 0f && advancedSettings.velocityToGroundTangentWeight > 0f) {
					Quaternion r = Quaternion.FromToRotation(Vector3.up, normal);
					velocity = Quaternion.Lerp(Quaternion.identity, r, advancedSettings.velocityToGroundTangentWeight) * velocity;
				}

			} else {

				// Air move
				Vector3 airMove = new Vector3 (inputState.move.x * airSpeed, GetComponent<Rigidbody>().velocity.y, inputState.move.z * airSpeed);
				velocity = Vector3.Lerp(GetComponent<Rigidbody>().velocity, airMove, Time.deltaTime * airControl);
			}

			// Limit the vertical velocity
			velocity.y = Mathf.Clamp(GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.y, onGround? advancedSettings.maxVerticalVelocityOnGround: GetComponent<Rigidbody>().velocity.y);

			velocity += Vector3.down * stickyForce * Time.deltaTime;
			

			GetComponent<Rigidbody>().velocity = velocity;

			// Dampering forward speed on the slopes
			float slopeDamper = !onGround? 1f: GetSlopeDamper(-deltaPosition / Time.deltaTime, normal);
			forwardMlp = Mathf.Lerp(forwardMlp, slopeDamper, Time.deltaTime * 5f);
		}

		public override void UpdateState(System.Object _inputState) {
			this.inputState = (UserControlThirdPerson.State)_inputState;

			AnimState animState = new AnimState();

			// Updating the forward speed of the character
			forward = Mathf.Lerp(forward, inputState.move.magnitude, Time.deltaTime * forwardAcceleration);

			Look();
			GroundCheck (); // detect and stick to ground

			// Friction
			if (inputState.move == Vector3.zero && groundDistance < airborneThreshold * 0.5f) HighFriction();
			else ZeroFriction();

			if (onGround) {

				// Jumping
				animState.jump = Jump();
			} else {

				// Additional gravity
				GetComponent<Rigidbody>().AddForce((Physics.gravity * gravityMultiplier) - Physics.gravity);
			}

			// Scale the capsule colllider while crouching
			ScaleCapsule(inputState.crouch? advancedSettings.crouchCapsuleScaleMlp: 1f);

			animState.forward = forward * forwardMlp;
			animState.crouch = inputState.crouch;
			animState.onGround = onGround;
			animState.yVelocity = GetComponent<Rigidbody>().velocity.y;

			// Update animation
			characterAnimation.UpdateState(animState);
		}

		private void Look() {
			// update the current head-look position
			lookPosSmooth = Vector3.Lerp(lookPosSmooth, inputState.lookPos, Time.deltaTime * advancedSettings.headLookResponseSpeed);

			bool isMoving = inputState.move != Vector3.zero;

			Vector3 lookDirection = isMoving? inputState.move: inputState.lookPos - GetComponent<Rigidbody>().position;

			float angle = GetAngleFromForward(lookDirection);
			
			if (!isMoving) angle *= (1.01f - (Mathf.Abs(angle) / 180f)) * advancedSettings.stationaryTurnSpeedMlp;
			
			// Rotating the character
			RigidbodyRotateAround(characterAnimation.GetPivotPoint(), transform.up, angle * Time.deltaTime * advancedSettings.movingTurnSpeed);
		}

		private bool Jump() {
			// check whether conditions are right to allow a jump:
			if (!inputState.jump) return false;
			if (inputState.crouch) return false;
			if (!characterAnimation.animationGrounded) return false;
			if (Time.time < lastAirTime + advancedSettings.jumpRepeatDelayTime) return false;

			// Jump
			onGround = false;
			jumpEndTime = Time.time + 0.1f;
			
			Vector3 jumpVelocity = inputState.move * airSpeed;
			GetComponent<Rigidbody>().velocity = new Vector3(jumpVelocity.x, jumpPower, jumpVelocity.z);

			return true;
		}

		// Is the character grounded?
		private void GroundCheck () {
			Vector3 platformVelocityTarget = Vector3.zero;
			float stickyForceTarget = 0f;

			// Spherecasting
			if (grounder != null && grounder.enabled && grounder.solver.quality == Grounding.Quality.Best) hit = grounder.solver.rootHit;
			else hit = GetSpherecastHit();

			normal = hit.normal;
			groundDistance = GetComponent<Rigidbody>().position.y - hit.point.y;

			Debug.DrawLine(GetComponent<Rigidbody>().position, hit.point, Color.red);
			
			// if not jumping...
			if (Time.time > jumpEndTime && GetComponent<Rigidbody>().velocity.y < jumpPower * 0.5f) {
				bool g = onGround;
				onGround = false;

				// The distance of considering the character grounded
				float groundHeight = !g? airborneThreshold * 0.5f: airborneThreshold;
				Vector3 horizontalVelocity = GetComponent<Rigidbody>().velocity;
				horizontalVelocity.y = 0f;
				
				float velocityF = horizontalVelocity.magnitude;

				if (groundDistance < groundHeight) {
					// Force the character on the ground
					stickyForceTarget = advancedSettings.groundStickyEffect * velocityF * groundHeight;

					// On moving platforms
					if (hit.rigidbody != null) platformVelocityTarget = hit.rigidbody.GetPointVelocity(hit.point);

					// Flag the character grounded
					onGround = true;
				}
			}

			// Interpolate the additive velocity of the platform the character might be standing on
			platformVelocity = Vector3.Lerp(platformVelocity, platformVelocityTarget, Time.deltaTime * advancedSettings.platformFriction);

			stickyForce = Mathf.Lerp(stickyForce, stickyForceTarget, Time.deltaTime * 5f);
			
			// remember when we were last in air, for jump delay
			if (!onGround) lastAirTime = Time.time;
		}
	}
}
