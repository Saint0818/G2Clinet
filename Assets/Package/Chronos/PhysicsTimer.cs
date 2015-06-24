using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Chronos
{
	public interface IPhysicsTimer
	{
		bool isKinematic { get; set; }
		float mass { get; set; }
		float drag { get; set; }
		float angularDrag { get; set; }
	}

	public abstract class PhysicsTimer<TSnapshot> : Recorder<TSnapshot>, IPhysicsTimer
	{
		protected override void Update()
		{
			float timeScale = timeline.timeScale;

			if (lastTimeScale != 0 && timeScale == 0) // Arrived at halt
			{
				if (lastTimeScale > 0)
				{
					zeroSnapshot = CopySnapshot();
				}
				else if (lastTimeScale < 0)
				{
					zeroSnapshot = interpolatedSnapshot;
				}
			}

			if (lastTimeScale >= 0 && timeScale <= 0) // Started pause or rewind
			{
				if (timeScale < 0) // Started rewind
				{
					laterSnapshot = CopySnapshot();
					laterTime = timeline.time;
					canRewind = TryFindEarlierSnapshot(false);
				}

				bodyIsKinematic = true;
			}
			else if (lastTimeScale <= 0 && timeScale > 0) // Stopped pause or rewind
			{
				bodyIsKinematic = isKinematic;

				if (lastTimeScale == 0) // Stopped pause
				{
					ApplySnapshot(zeroSnapshot);
				}
				else if (lastTimeScale < 0) // Stopped rewind
				{
					ApplySnapshot(interpolatedSnapshot);
				}

				WakeUp();

				Record();
			}

			if (timeScale > 0 && timeScale != lastTimeScale && !bodyIsKinematic) // Slowed down or accelerated
			{
				float modifier = timeScale / lastPositiveTimeScale;

				bodyVelocity *= modifier;
				bodyAngularVelocity *= modifier;
				bodyDrag *= modifier;
				bodyAngularDrag *= modifier;

				WakeUp();
			}

			if (timeScale > 0)
			{
				bodyIsKinematic = isKinematic;

				Progress();
			}
			else if (timeScale < 0)
			{
				Rewind();
			}

			lastTimeScale = timeScale;

			if (timeScale > 0)
			{
				lastPositiveTimeScale = timeScale;
			}
		}

		#region Fields

		protected float lastPositiveTimeScale = 1;
		protected TSnapshot zeroSnapshot;

		#endregion

		#region Rigidbody

		protected abstract bool bodyIsKinematic { get; set; }
		protected abstract float bodyMass { get; set; }
		protected abstract Vector3 bodyVelocity { get; set; }
		protected abstract Vector3 bodyAngularVelocity { get; set; }
		protected abstract float bodyDrag { get; set; }
		protected abstract float bodyAngularDrag { get; set; }
		protected abstract bool IsSleeping();
		protected abstract void WakeUp();

		/// <summary>
		/// Determines whether the rigidbody is kinematic before time effects. Use this property instead of Rigidbody.isKinematic, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public bool isKinematic { get; set; }

		/// <summary>
		/// The mass of the rigidbody before time effects. Use this property instead of Rigidbody.mass, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public float mass
		{
			get { return bodyMass; }
			set { AssertForwardProperty(); bodyMass = value; }
		}

		/// <summary>
		/// The drag of the rigidbody before time effects. Use this property instead of Rigidbody.drag, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public float drag
		{
			get { return bodyDrag / timeline.timeScale; }
			set { AssertForwardProperty(); bodyDrag = value * timeline.timeScale; }
		}

		/// <summary>
		/// The angular drag of the rigidbody before time effects. Use this property instead of Rigidbody.angularDrag, which will be overwritten by the physics timer at runtime. 
		/// </summary>
		public float angularDrag
		{
			get { return bodyAngularDrag / timeline.timeScale; }
			set { AssertForwardProperty(); bodyAngularDrag = value * timeline.timeScale; }
		}

		protected virtual float AdjustForce(float force)
		{
			return force * timeline.timeScale;
		}

		protected virtual Vector2 AdjustForce(Vector2 force)
		{
			return force * timeline.timeScale;
		}

		protected virtual Vector3 AdjustForce(Vector3 force)
		{
			return force * timeline.timeScale;
		}

		protected virtual void AssertForwardProperty()
		{
			if (timeline.timeScale <= 0)
			{
				throw new ChronosException("Cannot change the properties of the rigidbody while time is paused or rewinding.");
			}
		}

		protected virtual void AssertForwardForce()
		{
			if (timeline.timeScale <= 0)
			{
				throw new ChronosException("Cannot apply a force to the rigidbody while time is paused or rewinding.");
			}
		}

		#endregion
	}
}
