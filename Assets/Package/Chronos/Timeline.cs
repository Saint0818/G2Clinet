using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chronos
{
	/// <summary>
	/// Determines what type of clock a timeline observes. 
	/// </summary>
	public enum TimelineMode
	{
		/// <summary>
		/// The timeline observes a LocalClock attached to the same GameObject.
		/// </summary>
		Local,

		/// <summary>
		/// The timeline observes a GlobalClock referenced by globalClockKey. 
		/// </summary>
		Global
	}

	/// <summary>
	/// A component that combines timing measurements from an observed LocalClock or GlobalClock and any AreaClock within which it is. This component should be attached to any GameObject that should be affected by Chronos. 
	/// </summary>
	[AddComponentMenu("Time/Timeline")]
	[DisallowMultipleComponent]
	public class Timeline : MonoBehaviour
	{
		public Timeline()
		{
			areaClocks = new HashSet<IAreaClock>();
			occurrences = new HashSet<Occurrence>();
			handledOccurrences = new HashSet<Occurrence>();
			previousDeltaTimes = new Queue<float>();
			timeScale = lastTimeScale = 1;
		}

		protected virtual void Awake()
		{
			CacheComponents();
		}

		protected virtual void Start()
		{
			timeScale = lastTimeScale = clock.timeScale;
		}

		protected virtual void Update()
		{
			lastTimeScale = timeScale;

			TriggerEvents();

			timeScale = clock.timeScale; // Start with the time scale from local / global clock

			foreach (IAreaClock areaClock in areaClocks) // Blend it with the time scale of each area clock
			{
				if (areaClock != null && areaClock.ContainsPoint(transform.position))
				{
					float areaClockTimeScale = areaClock.TimeScale(this);

					if (areaClock.innerBlend == ClockBlend.Multiplicative)
					{
						timeScale *= areaClockTimeScale;
					}
					else // if (areaClock.innerBlend == ClockBlend.Additive)
					{
						timeScale += areaClockTimeScale;
					}
				}
			}

			if (timeScale != lastTimeScale)
			{
				AdjustComponents();
			}

			deltaTime = Time.unscaledDeltaTime * timeScale;
			fixedDeltaTime = Time.fixedDeltaTime * timeScale;
			time += deltaTime;
			unscaledTime += Time.unscaledDeltaTime;

			RecordSmoothing();

			FixAnimators();

			FixParticles();

			if (timeScale > 0)
			{
				TriggerForwardOccurrences();
			}
			else if (timeScale < 0)
			{
				TriggerBackwardOccurrences();
			}
		}

		#region Fields

		protected Animator animator;
		protected new Animation animation;
		protected new ParticleSystem particleSystem;
		protected AudioSource audioSource;
		protected NavMeshAgent navigator;
		protected WindZone windZone;
		protected float lastTimeScale;
		protected float particleTime;
		protected Queue<float> previousDeltaTimes;
		protected HashSet<Occurrence> occurrences;
		protected HashSet<Occurrence> handledOccurrences;
		protected Occurrence nextForwardOccurrence;
		protected Occurrence nextBackwardOccurrence;
		protected internal HashSet<IAreaClock> areaClocks;

		#endregion

		#region Properties

		[SerializeField]
		private TimelineMode _mode;
		/// <summary>
		/// Determines what type of clock the timeline observes. 
		/// </summary>
		public TimelineMode mode
		{
			get { return _mode; }
			set { _mode = value; _clock = null; }
		}

		[SerializeField]
		private string _globalClockKey;
		/// <summary>
		/// The key of the GlobalClock that is observed by the timeline. This value is only used for the Global mode. 
		/// </summary>
		public string globalClockKey
		{
			get { return _globalClockKey; }
			set { _globalClockKey = value; _clock = null; }
		}

		private Clock _clock;
		/// <summary>
		/// The clock observed by the timeline. 
		/// </summary>
		public Clock clock
		{
			get
			{
				if (_clock == null)
				{
					_clock = FindClock();
				}

				return _clock;
			}
		}

		/// <summary>
		/// The time scale of the timeline, computed from all observed clocks. For more information, see Clock.timeScale. 
		/// </summary>
		public float timeScale { get; protected set; }

		/// <summary>
		/// The delta time of the timeline, computed from all observed clocks. For more information, see Clock.deltaTime. 
		/// </summary>
		public float deltaTime { get; protected set; }

		/// <summary>
		/// The fixed delta time of the timeline, computed from all observed clocks. For more information, see Clock.fixedDeltaTime. 
		/// </summary>
		public float fixedDeltaTime { get; protected set; }

		/// <summary>
		/// A smoothed out delta time. Use this value if you need to avoid spikes and fluctuations in delta times. The amount of frames over which this value is smoothed can be adjusted via smoothingDeltas. 
		/// </summary>
		public float smoothDeltaTime
		{
			get
			{
				return (deltaTime + previousDeltaTimes.Sum()) / (previousDeltaTimes.Count + 1);
			}
		}

		/// <summary>
		/// The amount of frames over which smoothDeltaTime is smoothed. 
		/// </summary>
		public static int smoothingDeltas = 5;

		/// <summary>
		/// The time in seconds since the creation of this timeline, computed from all observed clocks. For more information, see Clock.time. 
		/// </summary>
		public float time { get; protected internal set; }

		/// <summary>
		/// The unscaled time in seconds since the creation of this timeline. For more information, see Clock.unscaledTime. 
		/// </summary>
		public float unscaledTime { get; protected set; }

		/// <summary>
		/// Indicates the state of the timeline. 
		/// </summary>
		public TimeState state
		{
			get
			{
				return Timekeeper.GetTimeState(timeScale);
			}
		}

		#endregion

		#region Timing

		protected virtual Clock FindClock()
		{
			if (mode == TimelineMode.Local)
			{
				LocalClock localClock = GetComponent<LocalClock>();

				if (localClock == null)
				{
					throw new ChronosException(string.Format("Missing local clock."));
				}

				return localClock;
			}
			else if (mode == TimelineMode.Global)
			{
				GlobalClock oldGlobalClock = _clock as GlobalClock;

				if (oldGlobalClock != null)
				{
					oldGlobalClock.Unregister(this);
				}

				if (!Timekeeper.instance.HasClock(globalClockKey))
				{
					throw new ChronosException(string.Format("Missing global clock: '{0}'.", globalClockKey));
				}

				GlobalClock globalClock = Timekeeper.instance.Clock(globalClockKey);

				globalClock.Register(this);

				return globalClock;
			}
			else
			{
				throw new ChronosException(string.Format("Unknown timeline mode: '{0}'.", mode));
			}
		}

		/// <summary>
		/// Releases the timeline from the specified area clock's effects. 
		/// </summary>
		public virtual void ReleaseFrom(IAreaClock areaClock)
		{
			areaClock.Release(this);
		}

		/// <summary>
		/// Releases the timeline from the effects of all the area clocks within which it is. 
		/// </summary>
		public virtual void ReleaseFromAll()
		{
			foreach (IAreaClock areaClock in areaClocks.Where(ac => ac != null).ToArray())
			{
				areaClock.Release(this);
			}

			areaClocks.Clear();
		}

		protected virtual void TriggerEvents()
		{
			if (lastTimeScale != 0 && timeScale == 0)
			{
				SendMessage("OnStartPause", SendMessageOptions.DontRequireReceiver);
			}

			if (lastTimeScale == 0 && timeScale != 0)
			{
				SendMessage("OnStopPause", SendMessageOptions.DontRequireReceiver);
			}

			if (lastTimeScale >= 0 && timeScale < 0)
			{
				SendMessage("OnStartRewind", SendMessageOptions.DontRequireReceiver);
			}

			if (lastTimeScale < 0 && timeScale >= 0)
			{
				SendMessage("OnStopRewind", SendMessageOptions.DontRequireReceiver);
			}

			if (lastTimeScale <= 0 && lastTimeScale >= 1 && timeScale > 0 && timeScale < 1)
			{
				SendMessage("OnStartSlowDown", SendMessageOptions.DontRequireReceiver);
			}

			if (lastTimeScale > 0 && lastTimeScale < 1 && timeScale <= 0 && timeScale >= 1)
			{
				SendMessage("OnStopSlowDown", SendMessageOptions.DontRequireReceiver);
			}

			if (lastTimeScale <= 1 && timeScale > 1)
			{
				SendMessage("OnStartFastForward", SendMessageOptions.DontRequireReceiver);
			}

			if (lastTimeScale > 1 && timeScale <= 1)
			{
				SendMessage("OnStopFastForward", SendMessageOptions.DontRequireReceiver);
			}
		}

		protected virtual void RecordSmoothing()
		{
			if (deltaTime != 0)
			{
				previousDeltaTimes.Enqueue(deltaTime);
			}

			if (previousDeltaTimes.Count > smoothingDeltas)
			{
				previousDeltaTimes.Dequeue();
			}
		}

		#endregion

		#region Occurrences

		protected void TriggerForwardOccurrences()
		{
			handledOccurrences.Clear();

			while (nextForwardOccurrence != null && nextForwardOccurrence.time <= time)
			{
				nextForwardOccurrence.Forward();

				handledOccurrences.Add(nextForwardOccurrence);

				nextBackwardOccurrence = nextForwardOccurrence;

				nextForwardOccurrence = OccurrenceAfter(nextForwardOccurrence.time, handledOccurrences);
			}
		}

		protected void TriggerBackwardOccurrences()
		{
			handledOccurrences.Clear();

			while (nextBackwardOccurrence != null && nextBackwardOccurrence.time >= time)
			{
				nextBackwardOccurrence.Backward();

				if (nextBackwardOccurrence.repeatable)
				{
					handledOccurrences.Add(nextBackwardOccurrence);

					nextForwardOccurrence = nextBackwardOccurrence;
				}
				else
				{
					occurrences.Remove(nextBackwardOccurrence);
				}

				nextBackwardOccurrence = OccurrenceBefore(nextBackwardOccurrence.time, handledOccurrences);
			}
		}

		protected Occurrence OccurrenceAfter(float time, params Occurrence[] ignored)
		{
			return OccurrenceAfter(time, (IEnumerable<Occurrence>)ignored);
		}

		protected Occurrence OccurrenceAfter(float time, IEnumerable<Occurrence> ignored)
		{
			Occurrence after = null;

			foreach (Occurrence occurrence in occurrences)
			{
				if (occurrence.time >= time &&
					!ignored.Contains(occurrence) &&
					(after == null || occurrence.time < after.time))
				{
					after = occurrence;
				}
			}

			return after;
		}

		protected Occurrence OccurrenceBefore(float time, params Occurrence[] ignored)
		{
			return OccurrenceBefore(time, (IEnumerable<Occurrence>)ignored);
		}

		protected Occurrence OccurrenceBefore(float time, IEnumerable<Occurrence> ignored)
		{
			Occurrence before = null;

			foreach (Occurrence occurrence in occurrences)
			{
				if (occurrence.time <= time &&
					!ignored.Contains(occurrence) &&
					(before == null || occurrence.time > before.time))
				{
					before = occurrence;
				}
			}

			return before;
		}

		protected virtual void PlaceOccurence(Occurrence occurrence, float time)
		{
			if (time == this.time)
			{
				if (timeScale >= 0)
				{
					occurrence.Forward();
					nextBackwardOccurrence = occurrence;
				}
				else
				{
					occurrence.Backward();
					nextForwardOccurrence = occurrence;
				}
			}
			else if (time > this.time)
			{
				if (nextForwardOccurrence == null ||
					nextForwardOccurrence.time > time)
				{
					nextForwardOccurrence = occurrence;
				}
			}
			else if (time < this.time)
			{
				if (nextBackwardOccurrence == null ||
					nextBackwardOccurrence.time < time)
				{
					nextBackwardOccurrence = occurrence;
				}
			}
		}

		/// <summary>
		/// Schedules an occurrence at a specified absolute time in seconds on the timeline. 
		/// </param>
		public virtual Occurrence Schedule(float time, bool repeatable, Occurrence occurrence)
		{
			occurrence.time = time;
			occurrence.repeatable = repeatable;
			occurrences.Add(occurrence);
			PlaceOccurence(occurrence, time);
			return occurrence;
		}

		/// <summary>
		/// Executes an occurrence now and places it on the schedule for rewinding. 
		/// </summary>
		public Occurrence Do(bool repeatable, Occurrence occurrence)
		{
			return Schedule(time, repeatable, occurrence);
		}

		/// <summary>
		/// Plans an occurrence to be executed in the specified delay in seconds. 
		/// </summary>
		public Occurrence Plan(float delay, bool repeatable, Occurrence occurrence)
		{
			if (delay <= 0)
			{
				throw new ChronosException("Planned occurrences must be in the future.");
			}

			return Schedule(time + delay, repeatable, occurrence);
		}

		/// <summary>
		/// Creates a "memory" of an occurrence at a specified "past-delay" in seconds. This means that the occurrence will only be executed if time is rewound, and that it will be executed backward first. 
		/// </summary>
		public Occurrence Memory(float delay, bool repeatable, Occurrence occurrence)
		{
			if (delay >= 0)
			{
				throw new ChronosException("Memory occurrences must be in the past.");
			}

			return Schedule(time - delay, repeatable, occurrence);
		}

		public Occurrence Schedule(float time, bool repeatable, ForwardAction forward, BackwardAction backward)
		{
			return Schedule(time, repeatable, new DelegateOccurrence(forward, backward));
		}

		public Occurrence Do(bool repeatable, ForwardAction forward, BackwardAction backward)
		{
			return Do(repeatable, new DelegateOccurrence(forward, backward));
		}

		public Occurrence Plan(float delay, bool repeatable, ForwardAction forward, BackwardAction backward)
		{
			return Plan(delay, repeatable, new DelegateOccurrence(forward, backward));
		}

		public Occurrence Memory(float delay, bool repeatable, ForwardAction forward, BackwardAction backward)
		{
			return Memory(delay, repeatable, new DelegateOccurrence(forward, backward));
		}

		public Occurrence Schedule(float time, ForwardOnlyAction forward)
		{
			return Schedule(time, false, new ForwardDelegateOccurrence(forward));
		}

		public Occurrence Plan(float delay, ForwardOnlyAction forward)
		{
			return Plan(delay, false, new ForwardDelegateOccurrence(forward));
		}

		public Occurrence Memory(float delay, ForwardOnlyAction forward)
		{
			return Memory(delay, false, new ForwardDelegateOccurrence(forward));
		}

		/// <summary>
		/// Removes the specified occurrence from the timeline. 
		/// </summary>
		public void Cancel(Occurrence occurrence)
		{
			if (!occurrences.Contains(occurrence))
			{
				throw new ChronosException("Occurrence to cancel not found on timeline.");
			}
			else
			{
				if (occurrence == nextForwardOccurrence)
				{
					nextForwardOccurrence = OccurrenceAfter(occurrence.time, occurrence);
				}

				if (occurrence == nextBackwardOccurrence)
				{
					nextBackwardOccurrence = OccurrenceBefore(occurrence.time, occurrence);
				}

				occurrences.Remove(occurrence);
			}
		}

		/// <summary>
		/// Removes the specified occurrence from the timeline and returns true if it is found. Otherwise, returns false. 
		/// </summary>
		public bool TryCancel(Occurrence occurrence)
		{
			if (!occurrences.Contains(occurrence))
			{
				return false;
			}
			else
			{
				Cancel(occurrence);
				return true;
			}
		}

		/// <summary>
		/// Change the absolute time in seconds of the specified occurrence on the timeline.
		/// </summary>
		public void Reschedule(Occurrence occurrence, float time)
		{
			occurrence.time = time;
			PlaceOccurence(occurrence, time);
		}

		/// <summary>
		/// Moves the specified occurrence forward on the timeline by the specified delay in seconds.
		/// </summary>
		public void Postpone(Occurrence occurrence, float delay)
		{
			Reschedule(occurrence, time + delay);
		}

		/// <summary>
		/// Moves the specified occurrence backward on the timeline by the specified delay in seconds.
		/// </summary>
		public void Prepone(Occurrence occurrence, float delay)
		{
			Reschedule(occurrence, time - delay);
		}

		#endregion

		#region Coroutines

		/// <summary>
		/// Suspends the coroutine execution for the given amount of seconds. This method should only be used with a yield statement in coroutines. 
		/// </summary>
		public Coroutine WaitForSeconds(float seconds)
		{
			return StartCoroutine(WaitingForSeconds(seconds));
		}

		protected IEnumerator WaitingForSeconds(float seconds)
		{
			float start = time;

			while (time < start + seconds)
			{
				yield return null;
			}
		}

		#endregion

		#region Components

		/// <summary>
		/// The speed that is applied to animators before time effects. Use this property instead of Animator.speed, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float animatorSpeed { get; set; }

		/// <summary>
		/// The speed that is applied to animations before time effects. Use this property instead of AnimationState.speed, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float animationSpeed { get; set; }

		/// <summary>
		/// The speed that is applied to particles before time effects. Use this property instead of ParticleSystem.playbackSpeed, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float particleSpeed { get; set; }

		/// <summary>
		/// The speed that is applied to audio before time effects. Use this property instead of AudioSource.pitch, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float audioSpeed { get; set; }

		/// <summary>
		/// The speed that is applied to navigation before time effects. Use this property instead of NavMeshAgent.speed, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float navigationSpeed { get; set; }

		/// <summary>
		/// The angular speed that is applied to navigation before time effects. Use this property instead of NavMeshAgent.angularSpeed, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float navigationAngularSpeed { get; set; }

		/// <summary>
		/// The speeds that are applied to the wind zone before time effects. Use this property instead of WindZone.wind*, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public WindZoneSpeeds windZoneSpeeds { get; set; }

		/// <summary>
		/// The components used by the timeline are cached for performance optimization. If you add or remove the Animator, Animation, ParticleSystem, NavMeshAgent, AudioSource or WindZone on the GameObject, you need to call this method to update the timeline accordingly. 
		/// </summary>
		public virtual void CacheComponents()
		{
			// For each component, we first check whether it was
			// present on the GameObject before this call. If it 
			// wasn't and now is, we copy its properties. This way,
			// if we call CacheComponents after already having the component,
			// the properties won't be overwritten.

			// Animator

			bool hadAnimator = animator != null;

			animator = GetComponent<Animator>();

			if (!hadAnimator && animator != null)
			{
				animatorSpeed = animator.speed;
			}

			// Animation

			bool hadAnimation = animation != null;

			animation = GetComponent<Animation>();

			if (!hadAnimation && animation != null)
			{
				// Animations can have different speeds per state, but Chronos
				// doesn't support that yet. Warn if different speeds are found.

				float firstAnimationStateSpeed = 1;
				bool found = false;

				foreach (AnimationState animationState in animation)
				{
					if (found && firstAnimationStateSpeed != animationState.speed)
					{
						Debug.LogWarning("Different animation speeds per state are not supported.");
					}

					firstAnimationStateSpeed = animationState.speed;
					found = true;
				}

				animationSpeed = firstAnimationStateSpeed;
			}

			// Particle System

			bool hadParticleSystem = particleSystem != null;

			particleSystem = GetComponent<ParticleSystem>();

			if (!hadParticleSystem && particleSystem != null)
			{
				particleSpeed = particleSystem.playbackSpeed;
				particleTime = 0;

				if (particleSystem.randomSeed == 0)
				{
					particleSystem.randomSeed = (uint)Random.Range(1, int.MaxValue);
				}
			}

			// Audio Source

			bool hadAudioSource = audioSource != null;

			audioSource = GetComponent<AudioSource>();

			if (!hadAudioSource && audioSource != null)
			{
				audioSpeed = audioSource.pitch;
			}

			// Navigator

			bool hadNavigator = navigator != null;

			navigator = GetComponent<NavMeshAgent>();

			if (!hadNavigator && navigator != null)
			{
				navigationSpeed = navigator.speed;
				navigationAngularSpeed = navigator.angularSpeed;
			}

			// WindZone

			bool hadWindZone = windZone != null;

			windZone = GetComponent<WindZone>();

			if (!hadWindZone && windZone != null)
			{
				windZoneSpeeds = new WindZoneSpeeds()
				{
					main = windZone.windMain,
					turbulence = windZone.windTurbulence,
					pulseFrequency = windZone.windPulseFrequency,
					pulseMagnitude = windZone.windPulseMagnitude
				};
			}
		}

		protected virtual void AdjustComponents()
		{
			float timeScale = this.timeScale;

			if (animator != null)
			{
				animator.speed = animatorSpeed * timeScale;

				// Unity 5.1:
				// animator.SetFloat("TimeScale", timeScale);
			}

			if (animation != null)
			{
				foreach (AnimationState animationState in animation)
				{
					animationState.speed = animationSpeed * timeScale;
				}
			}

			if (audioSource != null)
			{
				audioSource.pitch = audioSpeed * timeScale;
			}

			if (navigator != null)
			{
				navigator.speed = navigationSpeed * timeScale;
				navigator.angularSpeed = navigationAngularSpeed * timeScale;
			}

			if (windZone != null)
			{
				WindZoneSpeeds speeds = windZoneSpeeds;

				windZone.windTurbulence = speeds.turbulence * timeScale * Mathf.Abs(timeScale);
				windZone.windPulseFrequency = speeds.pulseFrequency * timeScale;
				windZone.windPulseMagnitude = speeds.pulseMagnitude * Mathf.Sign(timeScale);
			}
		}

		// Unlike Animations, Animators cannot play at negative normalized times.
		// If their current clip is at a time of less than zero, loop it from the end.
		// Unity 5.1: Not necessary anymore
		protected virtual void FixAnimators()
		{
			if (animator != null && timeScale < 0)
			{
				for (int i = 0; i < animator.layerCount; i++)
				{
					AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(i);

					if (state.normalizedTime < 0)
					{
						animator.Play(state.fullPathHash, i, 1);
					}
				}
			}
		}

		// Known issue: low time scales / speed will cause stutter
		// Reported here: http://fogbugz.unity3d.com/default.asp?694191_dso514lin4rf5vbg
		protected virtual void FixParticles()
		{
			if (particleSystem != null)
			{
				// TODO: Override isPlaying / pause / stop

				particleSystem.Simulate(0, true, true);

				if (time > 0)
				{
					particleSystem.Simulate(particleTime, true, false);
				}

				particleTime += deltaTime * particleSpeed;
			}
		}

		#endregion
	}
}
