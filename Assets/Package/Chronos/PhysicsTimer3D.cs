using System;
using UnityEngine;

namespace Chronos
{
	/// <summary>
	/// A component that enables time effects on the non-kinematic 3D rigidbody attached to the same GameObject. 
	/// </summary>
	[AddComponentMenu("Time/Physics Timer 3D"), DisallowMultipleComponent]
	public class PhysicsTimer3D : PhysicsTimer<PhysicsTimer3D.Snapshot>
	{
		public struct Snapshot
		{
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 velocity;
			public Vector3 angularVelocity;
			public float lastPositiveTimeScale;

			public static Snapshot Lerp(Snapshot from, Snapshot to, float t)
			{
				return new Snapshot()
				{
					position = Vector3.Lerp(from.position, to.position, t),
					rotation = Quaternion.Lerp(from.rotation, to.rotation, t),
					velocity = Vector3.Lerp(from.velocity, to.velocity, t),
					angularVelocity = Vector3.Lerp(from.angularVelocity, to.angularVelocity, t),
					lastPositiveTimeScale = Mathf.Lerp(from.lastPositiveTimeScale, to.lastPositiveTimeScale, t)
				};
			}
		}

		protected override void Start()
		{
			base.Start();

			rigidbody.useGravity = false;
		}

		protected virtual void FixedUpdate()
		{
			if (useGravity && !rigidbody.isKinematic && timeline.timeScale > 0)
			{
				velocity += (Physics.gravity * timeline.fixedDeltaTime);
			}
		}

		protected new Rigidbody rigidbody;
		public bool shouldSleep;

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
		/// The components used by the physics timer are cached for performance optimization. If you add or remove the Timeline or Rigidbody on the GameObject, you need to call this method to update the physics timer accordingly. 
		/// </summary>
		public override void CacheComponents()
		{
			base.CacheComponents();

			bool hadRigidbody = rigidbody != null;

			rigidbody = GetComponent<Rigidbody>();

			if (rigidbody == null)
			{
				throw new ChronosException("Missing rigidbody for physics timer.");
			}

			if (!hadRigidbody)
			{
				isKinematic = rigidbody.isKinematic;
				useGravity = rigidbody.useGravity;
			}
		}

		protected bool bodyUseGravity
		{
			get { return rigidbody.useGravity; }
			set { rigidbody.useGravity = value; }
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
			get { return rigidbody.angularVelocity; }
			set { rigidbody.angularVelocity = value; }
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

		protected override void WakeUp()
		{
			rigidbody.WakeUp();
		}

		protected override bool IsSleeping()
		{
			return rigidbody.IsSleeping();
		}

		/// <summary>
		/// Determines whether the rigidbody uses gravity. Use this property instead of Rigidbody.useGravity, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public bool useGravity { get; set; }

		/// <summary>
		/// The velocity of the rigidbody before time effects. Use this property instead of Rigidbody.velocity, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public Vector3 velocity
		{
			get { return bodyVelocity / timeline.timeScale; }
			set { AssertForwardProperty(); bodyVelocity = value * timeline.timeScale; }
		}

		/// <summary>
		/// The angular velocity of the rigidbody before time effects. Use this property instead of Rigidbody.angularVelocity, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public Vector3 angularVelocity
		{
			get { return bodyAngularVelocity / timeline.timeScale; }
			set { AssertForwardProperty(); bodyAngularVelocity = value * timeline.timeScale; }
		}

		/// <summary>
		/// The equivalent of Rigidbody.AddForce adjusted for time effects.
		/// </summary>
		public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
		{
			AssertForwardForce();
			rigidbody.AddForce(AdjustForce(force), mode);
		}

		/// <summary>
		/// The equivalent of Rigidbody.AddRelativeForce adjusted for time effects.
		/// </summary>
		public void AddRelativeForce(Vector3 force, ForceMode mode = ForceMode.Force)
		{
			AssertForwardForce();
			rigidbody.AddRelativeForce(AdjustForce(force), mode);
		}

		/// <summary>
		/// The equivalent of Rigidbody.AddForceAtPosition adjusted for time effects.
		/// </summary>
		public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode = ForceMode.Force)
		{
			AssertForwardForce();
			rigidbody.AddForceAtPosition(AdjustForce(force), position, mode);
		}

		/// <summary>
		/// The equivalent of Rigidbody.AddRelativeForce adjusted for time effects.
		/// </summary>
		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0, ForceMode mode = ForceMode.Force)
		{
			AssertForwardForce();
			rigidbody.AddExplosionForce(AdjustForce(explosionForce), explosionPosition, explosionRadius, upwardsModifier, mode);
		}

		/// <summary>
		/// The equivalent of Rigidbody.AddTorque adjusted for time effects.
		/// </summary>
		public void AddTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
		{
			AssertForwardForce();
			rigidbody.AddTorque(AdjustForce(torque), mode);
		}

		/// <summary>
		/// The equivalent of Rigidbody.AddRelativeTorque adjusted for time effects.
		/// </summary>
		public void AddRelativeTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
		{
			AssertForwardForce();
			rigidbody.AddRelativeTorque(AdjustForce(torque), mode);
		}

		#endregion
	}
}
