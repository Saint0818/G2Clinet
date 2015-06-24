using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chronos
{
	/// <summary>
	/// A global singleton tasked with keeping track of global clocks in the scene. One and only one Timekeeper is required per scene. 
	/// </summary>
	[AddComponentMenu("Time/Timekeeper"), DisallowMultipleComponent]
	public class Timekeeper : Singleton<Timekeeper>
	{
		public Timekeeper()
			: base(false, false)
		{
			clocks = new Dictionary<string, GlobalClock>();
		}

		protected virtual void Awake()
		{
			foreach (GlobalClock globalClock in GetComponents<GlobalClock>())
			{
				clocks.Add(globalClock.key, globalClock);
			}
		}

		#region Fields

		protected Dictionary<string, GlobalClock> clocks;

		#endregion

		#region Properties

		[SerializeField]
		private bool _debug = false;
		/// <summary>
		/// Determines whether Chronos should display debug messages and gizmos in the editor. 
		/// </summary>
		public bool debug
		{
			get { return _debug; }
			set { _debug = value; }
		}

		#endregion

		#region Clocks

		/// <summary>
		/// Determines whether the timekeeper has a global clock with the specified key. 
		/// </summary>
		public virtual bool HasClock(string key)
		{
			if (key == null) throw new ArgumentNullException("key");

			return clocks.ContainsKey(key);
		}

		/// <summary>
		/// Returns the global clock with the specified key. 
		/// </summary>
		public virtual GlobalClock Clock(string key)
		{
			if (key == null) throw new ArgumentNullException("key");

			if (!HasClock(key))
			{
				throw new ChronosException(string.Format("Unknown global clock '{0}'.", key));
			}

			return clocks[key];
		}

		/// <summary>
		/// Adds a global clock with the specified key and returns it.
		/// </summary>
		public virtual GlobalClock AddClock(string key)
		{
			if (key == null) throw new ArgumentNullException("key");

			if (HasClock(key))
			{
				throw new ChronosException(string.Format("Global clock '{0}' already exists.", key));
			}

			GlobalClock clock = gameObject.AddComponent<GlobalClock>();
			clock.key = key;
			return clock;
		}

		/// <summary>
		/// Removes the global clock with the specified key.
		/// </summary>
		public virtual void RemoveClock(string key)
		{
			if (key == null) throw new ArgumentNullException("key");

			if (!HasClock(key))
			{
				throw new ChronosException(string.Format("Unknown global clock '{0}'.", key));
			}

			clocks.Remove(key);
		}

		#endregion

		internal static TimeState GetTimeState(float timeScale)
		{
			if (timeScale < 0)
			{
				return TimeState.Reversed;
			}
			else if (timeScale == 0)
			{
				return TimeState.Paused;
			}
			else if (timeScale < 1)
			{
				return TimeState.Slowed;
			}
			else if (timeScale == 1)
			{
				return TimeState.Normal;
			}
			else // if (timeScale > 1)
			{
				return TimeState.Accelerated;
			}
		}
	}
}