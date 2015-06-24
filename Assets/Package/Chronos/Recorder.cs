using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace Chronos
{
	public interface IRecorder
	{
		void SetRecording(float duration, float interval);
		int EstimateMemoryUsage();
	}

	/// <summary>
	/// An abstract base component that saves snapshots at regular intervals to enable rewinding.
	/// </summary>
	/// <typeparam name="TSnapshot"></typeparam>
	public abstract class Recorder<TSnapshot> : MonoBehaviour, IRecorder
	{
		protected internal const float DefaultRecordingDuration = 30;
		protected internal const float DefaultRecordingInterval = 0.5f;

		public Recorder()
		{
			snapshots = new List<TSnapshot>();
			times = new List<float>();
		}

		protected virtual void Awake()
		{
			CacheComponents();
		}

		protected virtual void Start()
		{
			Reset();
		}

		protected virtual void Update()
		{
			float timeScale = timeline.timeScale;

			if (lastTimeScale >= 0 && timeScale < 0) // Started rewind
			{
				laterSnapshot = CopySnapshot();
				laterTime = timeline.time;
				canRewind = TryFindEarlierSnapshot(false);
			}

			if (timeScale > 0)
			{
				Progress();
			}
			else if (timeScale < 0)
			{
				Rewind();
			}

			lastTimeScale = timeScale;
		}

		#region Fields

		protected Timeline timeline;
		protected List<TSnapshot> snapshots;
		protected List<float> times;
		protected int capacity;
		protected float recordingTimer;
		protected float lastTimeScale = 1;
		protected bool canRewind;
		protected TSnapshot laterSnapshot;
		protected float laterTime;
		protected TSnapshot earlierSnapshot;
		protected float earlierTime;
		protected TSnapshot interpolatedSnapshot;

		#endregion

		#region Properties

		[SerializeField]
		private float _recordingDuration = DefaultRecordingDuration;
		/// <summary>
		/// The maximum duration in seconds during which snapshots will be recorded. Higher values offer more rewind time but require more memory. 
		/// </summary>
		public float recordingDuration
		{
			get { return _recordingDuration; }
			protected set { _recordingDuration = value; }
		}

		[SerializeField]
		private float _recordingInterval = DefaultRecordingInterval;
		/// <summary>
		/// The interval in seconds at which snapshots will be recorder. Lower values offer more rewind precision but require more memory. 
		/// </summary>
		public float recordingInterval
		{
			get { return _recordingInterval; }
			protected set { _recordingInterval = value; }
		}

		/// <summary>
		/// Indicates whether the recorder has exhausted its rewind capacity. 
		/// </summary>
		public bool exhaustedRewind
		{
			get { return !canRewind; }
		}

		#endregion

		#region Flow

		protected virtual void Progress()
		{
			if (recordingTimer >= recordingInterval)
			{
				Record();

				recordingTimer = 0;
			}

			recordingTimer += timeline.deltaTime;
		}

		protected virtual void Record()
		{
			if (snapshots.Count == capacity)
			{
				snapshots.RemoveAt(0);
				times.RemoveAt(0);
			}

			snapshots.Add(CopySnapshot());
			times.Add(timeline.time);

			canRewind = true;
		}

		protected virtual void Rewind()
		{
			if (canRewind)
			{
				if (timeline.time <= earlierTime)
				{
					canRewind = TryFindEarlierSnapshot(true);

					if (!canRewind)
					{
						// Make sure the last snapshot is perfectly in place
						interpolatedSnapshot = earlierSnapshot;
						ApplySnapshot(interpolatedSnapshot);

						SendMessage("OnExhaustRewind", SendMessageOptions.DontRequireReceiver);

						return;
					}
				}

				float t = (laterTime - timeline.time) / (laterTime - earlierTime);

				interpolatedSnapshot = LerpSnapshots(laterSnapshot, earlierSnapshot, t);

				ApplySnapshot(interpolatedSnapshot);
			}
		}

		protected virtual void OnExhaustRewind()
		{
			if (Timekeeper.instance.debug)
			{
				Debug.LogWarning("Reached rewind limit.");
			}
		}

		#endregion

		#region Snapshots

		protected abstract TSnapshot LerpSnapshots(TSnapshot from, TSnapshot to, float t);

		protected abstract TSnapshot CopySnapshot();

		protected abstract void ApplySnapshot(TSnapshot snapshot);

		protected bool TryFindEarlierSnapshot(bool pop)
		{
			if (pop)
			{
				if (snapshots.Count < 1)
				{
					return false;
				}

				laterSnapshot = snapshots[snapshots.Count - 1];
				laterTime = times[times.Count - 1];

				snapshots.RemoveAt(snapshots.Count - 1);
				times.RemoveAt(times.Count - 1);
			}

			if (snapshots.Count < 1)
			{
				return false;
			}

			earlierSnapshot = snapshots[snapshots.Count - 1];
			earlierTime = times[times.Count - 1];

			return true;
		}

		protected bool TryFindLaterSnapshot(bool pop)
		{
			if (pop)
			{
				if (snapshots.Count < 1)
				{
					return false;
				}

				earlierSnapshot = snapshots[0];
				earlierTime = times[0];

				snapshots.RemoveAt(0);
				times.RemoveAt(0);
			}

			if (snapshots.Count < 1)
			{
				return false;
			}

			laterSnapshot = snapshots[0];
			laterTime = times[0];

			return true;
		}

		/// <summary>
		/// Sets the recording duration and interval in seconds. 
		/// This will reset the saved snapshots.
		/// </summary>
		public void SetRecording(float duration, float interval)
		{
			recordingDuration = duration;
			recordingInterval = interval;

			Reset();
		}

		/// <summary>
		/// Resets the saved snapshots. 
		/// </summary>
		public virtual void Reset()
		{
			if (recordingDuration < recordingInterval)
			{
				throw new ChronosException("The recording duration must be longer than or equal to interval.");
			}

			if (recordingInterval <= 0)
			{
				throw new ChronosException("The recording interval must be positive.");
			}

			if (Application.isPlaying)
			{
				snapshots.Clear();
				times.Clear();

				capacity = Mathf.CeilToInt(recordingDuration / recordingInterval);
				snapshots.Capacity = capacity;
				times.Capacity = capacity;
				recordingTimer = 0;

				Record();
			}
		}

		#endregion

		#region Components
		
		public virtual void CacheComponents()
		{
			timeline = GetComponent<Timeline>();

			if (timeline == null)
			{
				throw new ChronosException(string.Format("Missing timeline for recorder."));
			}
		}

		#endregion

		/// <summary>
		/// Estimate the memory usage in bytes from the storage of snapshots for the current recording duration and interval. 
		/// </summary>
		public int EstimateMemoryUsage()
		{
			int structSize = Marshal.SizeOf(typeof(TSnapshot));
			int structAmount = Mathf.CeilToInt(recordingDuration / recordingInterval);
			int pointerAmount = 1; while (pointerAmount < structAmount) pointerAmount *= 2;
			int pointerSize = IntPtr.Size;

			return (structSize * structAmount) + (pointerSize * pointerAmount);
		}
	}
}