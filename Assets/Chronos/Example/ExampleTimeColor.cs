using UnityEngine;

namespace Chronos.Example
{
	// Utility script to change the color of a game object based
	// on the time scale of its timeline.
	public class ExampleTimeColor : MonoBehaviour
	{
		// The state colors
		Color rewind = Color.magenta;
		Color pause = Color.red;
		Color slow = Color.yellow;
		Color play = Color.green;
		Color accelerate = Color.blue;

		// The time scales at which to apply colors
		float slowTimeScale = 0.5f;
		float rewindTimeScale = -1f;
		float accelerateTimeScale = 2f;

		public GlobalClock test;

		void Update()
		{
			Color color = Color.white;

			// Get the timeline in the ancestors
			Timeline time = GetComponentInParent<Timeline>();

			if (time != null)
			{
				float timeScale = time.timeScale;

				// Color lerping magic :)

				if (timeScale < 0)
				{
					color = Color.Lerp(pause, rewind, Mathf.Max(rewindTimeScale, timeScale) / rewindTimeScale);
				}
				else if (timeScale < slowTimeScale)
				{
					color = Color.Lerp(pause, slow, timeScale / slowTimeScale);
				}
				else if (timeScale < 1)
				{
					color = Color.Lerp(slow, play, (timeScale - slowTimeScale) / (1 - slowTimeScale));
				}
				else
				{
					color = Color.Lerp(play, accelerate, (timeScale - 1) / (accelerateTimeScale - 1));
				}
			}

			Renderer renderer = GetComponent<Renderer>();
			ParticleSystem particles = GetComponent<ParticleSystem>();

			// Apply the color to the renderer (if any)
			if (renderer != null)
			{
				foreach (Material material in GetComponent<Renderer>().materials)
				{
					material.color = color;
				}
			}

			// Apply the color to the particle system (if any)
			if (particles != null)
			{
				particles.startColor = color;
			}
		}
	}
}