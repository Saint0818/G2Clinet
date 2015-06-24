using UnityEngine;

namespace Chronos
{
	/// <summary>
	/// A component that enables time effects on the non-kinematic 2D rigidbody attached to the same GameObject. 
	/// </summary>
	[AddComponentMenu("Time/Physics Timer 2D"), DisallowMultipleComponent]
	public class PhysicsTimer2D : PhysicsTimer<PhysicsTimer2D.Snapshot>
	{
		public struct Snapshot
		{
			public Vector2 position;
			public Quaternion rotation;
			public Vector2 velocity;
			public float angularVelocity;
			public float lastPositiveTimeScale;

			public static Snapshot Lerp(Snapshot from, Snapshot to, float t)
			{
				return new Snapshot()
				{
					position = Vector2.Lerp(from.position, to.position, t),
					rotation = Quaternion.Lerp(from.rotation, to.rotation, t),
					velocity = Vector2.Lerp(from.velocity, to.velocity, t),
					angularVelocity = Mathf.Lerp(from.angularVelocity, to.angularVelocity, t),
					lastPositiveTimeScale = Mathf.Lerp(from.lastPositiveTimeScale, to.lastPositiveTimeScale, t)
				};
			}
		}

		protected new Rigidbody2D rigidbody;

		protected override void Start()
		{
			base.Start();

			rigidbody.gravityScale = 0;
		}

		protected virtual void FixedUpdate()
		{
			if (gravityScale > 0 && !rigidbody.isKinematic && timeline.timeScale > 0)
			{
				velocity += (Physics2D.gravity * gravityScale * timeline.fixedDeltaTime);
			}
		}

		#region Snapshots

		protected override Snapshot LerpSnapshots(Snapshot from, Snapshot to, float t)
		{
			return Snapshot.Lerp(from, to, t);
		}

		protected override Snapshot CopySnapshot()
		{
			return new Snapshot()
			{
				position = transform.position,
				rotation = transform.rotation,
				velocity = rigidbody.velocity,
				angularVelocity = rigidbody.angularVelocity,
				lastPositiveTimeScale = lastPositiveTimeScale
			};
		}

		protected override void ApplySnapshot(Snapshot snapshot)
		{
			transform.position = snapshot.position;
			transform.rotation = snapshot.rotation;

			if (timeline.timeScale > 0)
			{
				rigidbody.velocity = snapshot.velocity;
				rigidbody.angularVelocity = rigidbody.angularVelocity;
			}

			lastPositiveTimeScale = snapshot.lastPositiveTimeScale;
		}

		#endregion

		#region Components

		/// <summary>
		/// The components used by the physics timer are cached for performance optimization. If you add or remove the Timeline or Rigidbody2D on the GameObject, you need to call this method to update the physics timer accordingly. 
		/// </summary>
		public override void CacheComponents()
		{
			base.CacheComponents();

			bool hadRigidbody = rigidbody != null;

			rigidbody = GetComponent<Rigidbody2D>();

			if (rigidbody == null)
			{
				throw new ChronosException("Missing rigidbody for physics timer.");
			}

			if (!hadRigidbody)
			{
				isKinematic = rigidbody.isKinematic;
				gravityScale = rigidbody.gravityScale;
			}
		}

		protected float bodyGravityScale
		{
			get { return rigidbody.gravityScale; }
			set { rigidbody.gravityScale = value; }
		}

		protected override bool bodyIsKinematic
		{
			get { return rigidbody.isKinematic; }
			set { rigidbody.isKinematic = value; }
		}

		protected override float bodyMass
		{
			get { return rigidbody.mass; }
			set { rigidbody.mass = value; }
		}

		protected override Vector3 bodyVelocity
		{
			get { return rigidbody.velocity; }
			set { rigidbody.velocity = value; }
		}

		protected override Vector3 bodyAngularVelocity
		{
			get { return rigidbody.angularVelocity * Vector3.one; }
			set { rigidbody.angularVelocity = value.x; }
		}

		protected override float bodyDrag
		{
			get { return rigidbody.drag; }
			set { rigidbody.drag = value; }
		}

		protected override float bodyAngularDrag
		{
			get { return rigidbody.angularDrag; }
			set { rigidbody.angularDrag = value; }
		}

		protected override bool IsSleeping()
		{
			return rigidbody.IsSleeping();
		}

		protected override void WakeUp()
		{
			rigidbody.WakeUp();
		}

		/// <summary>
		/// The gravity scale of the rigidbody. Use this property instead of Rigidbody2D.gravityScale, which will be overwritten by the physics timer at runtime.
		/// </summary>
		public float gravityScale { get; set; }

		/// <summary>
		/// The velocity of the rigidbody before time effects. Use this property instead of Rigidbody2D.velocity, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public Vector2 velocity
		{
			get { return bodyVelocity / timeline.timeScale; }
			set { AssertForwardProperty(); bodyVelocity = value * timeline.timeScale; }
		}

		/// <summary>
		/// The angular velocity of the rigidbody before time effects. Use this property instead of Rigidbody2D.angularVelocity, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public float angularVelocity
		{
			get { return bodyAngularVelocity.x / timeline.timeScale; }
			set { AssertForwardProperty(); bodyAngularVelocity = value * Vector3.one * timeline.timeScale; }
		}

		/// <summary>
		/// The equivalent of Rigidbody2D.AddForce adjusted for time effects.
		/// </summary>
		public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
		{
			AssertForwardForce();
			rigidbody.AddForce(AdjustForce(force), mode);
		}

		/// <summary>
		/// The equivalent of Rigidbody2D.AddRelativeForce adjusted for time effects.
		/// </summary>
		public void AddRelativeForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
		{
			AssertForwardForce();
			rigidbody.AddRelativeForce(AdjustForce(force), mode);
		}

		/// <summary>
		/// The equivalent of Rigidbody2D.AddForceAtPosition adjusted for time effects.
		/// </summary>
		public void AddForceAtPosition(Vector2 force, Vector2 position, ForceMode2D mode = ForceMode2D.Force)
		{
			AssertForwardForce();
			rigidbody.AddForceAtPosition(AdjustForce(force), position, mode);
		}

		/// <summary>
		/// The equivalent of Rigidbody2D.AddTorque adjusted for time effects.
		/// </summary>
		public void AddTorque(float torque, ForceMode2D mode = ForceMode2D.Force)
		{
			AssertForwardForce();
			rigidbody.AddTorque(AdjustForce(torque), mode);
		}

		#endregion
	}
}
