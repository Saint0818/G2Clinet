using UnityEngine;

namespace Chronos
{
	public class ParticleSystemTimeline : ComponentTimeline<ParticleSystem>
	{
		/// <summary>
		/// The playback speed that is applied to the particle system before time effects. Use this property instead of ParticleSystem.playbackSpeed, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float playbackSpeed { get; set; }

		/// <summary>
		/// The playback time of the particle system. Use this property instead of ParticleSystem.time, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float time { get; set; }

		/// <summary>
		/// Indicates whether the particle system is playing. Use this property instead of ParticleSystem.isPlaying, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public bool isPlaying { get; protected set; }

		/// <summary>
		/// Indicates whether the particle system is paused. Use this property instead of ParticleSystem.isPaused, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public bool isPaused
		{
			get
			{
				return !isPlaying;
			}
		}

		/// <summary>
		/// Indicates whether the particle system is stopped. Use this property instead of ParticleSystem.isStopped, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public bool isStopped
		{
			get
			{
				return !isPlaying && time == 0;
			}
		}

		public ParticleSystemTimeline(Timeline timeline) : base(timeline) { }

		public override void CopyProperties(ParticleSystem source)
		{
			playbackSpeed = source.playbackSpeed;

			isPlaying = source.playOnAwake;

			time = 0;

			if (source.randomSeed == 0)
			{
				source.randomSeed = (uint)Random.Range(1, int.MaxValue);
			}
		}

		public override void Update()
		{
			// Known issue: low time scales / speed will cause stutter
			// Reported here: http://fogbugz.unity3d.com/default.asp?694191_dso514lin4rf5vbg

			component.Simulate(0, true, true);

			if (time > 0)
			{
				// Can be performance intensive at high times.
				// Limit it with a loop-multiple of its time (globally configurable)

				float maxLoops = Timekeeper.instance.maxParticleLoops;
				float adjustedTime = time;

				if (maxLoops > 0)
				{
					adjustedTime %= component.duration * maxLoops;
				}

				component.Simulate(adjustedTime, true, false);
			}

			if (isPlaying)
			{
				time += timeline.deltaTime * playbackSpeed;
			}
		}

		/// <summary>
		/// Plays the particle system. Use this property instead of ParticleSystem.Play, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public void Play()
		{
			isPlaying = true;
		}

		/// <summary>
		/// Pauses the particle system. Use this property instead of ParticleSystem.Pause, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public void Pause()
		{
			isPlaying = false;
		}

		/// <summary>
		/// Stops the particle system. Use this property instead of ParticleSystem.Stop, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public void Stop()
		{
			isPlaying = false;
			time = 0;
		}
	}
}