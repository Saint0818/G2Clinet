using UnityEngine;

namespace Chronos
{
	public class AudioSourceTimeline : ComponentTimeline<AudioSource>
	{
		/// <summary>
		/// The pitch that is applied to the audio source before time effects. Use this property instead of AudioSource.pitch, which will be overwritten by the timeline at runtime. 
		/// </summary>
		public float pitch { get; set; }

		public AudioSourceTimeline(Timeline timeline) : base(timeline) { }

		public override void CopyProperties(AudioSource source)
		{
			pitch = source.pitch;
		}

		public override void AdjustProperties(float timeScale)
		{
			component.pitch = pitch * timeScale;
		}
	}
}
