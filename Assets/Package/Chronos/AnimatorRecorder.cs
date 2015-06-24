using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Chronos
{
	/// <summary>
	/// A component that enables rewinding the state of the Animator on the GameObject via recorded snapshots of its parameters.
	/// </summary>
	public class AnimatorRecorder : Recorder<AnimatorRecorder.Snapshot>
	{
		public struct Snapshot
		{
			public Dictionary<int, float> floatParameters;
			public Dictionary<int, int> intParameters;
			public Dictionary<int, bool> boolParameters;

			public void Initialize()
			{
				floatParameters = new Dictionary<int, float>();
				intParameters = new Dictionary<int, int>();
				boolParameters = new Dictionary<int, bool>();
			}

			public static Snapshot Lerp(IEnumerable<AnimatorControllerParameter> parameters, Snapshot from, Snapshot to, float t)
			{
				Snapshot snapshot = new Snapshot();
				snapshot.Initialize();

				foreach (AnimatorControllerParameter parameter in parameters)
				{
					int hash = parameter.nameHash;

					switch (parameter.type)
					{
						case AnimatorControllerParameterType.Float:
							snapshot.floatParameters.Add(hash, Mathf.Lerp(from.floatParameters[hash], to.floatParameters[hash], t));
							break;

						case AnimatorControllerParameterType.Int:
							snapshot.intParameters.Add(hash, from.intParameters[hash]);
							break;

						case AnimatorControllerParameterType.Bool:
							snapshot.boolParameters.Add(hash, from.boolParameters[hash]);
							break;
					}
				}

				return snapshot;
			}
		}

		protected Animator animator;
		protected AnimatorControllerParameter[] parameters;

		protected override Snapshot LerpSnapshots(Snapshot from, Snapshot to, float t)
		{
			return Snapshot.Lerp(parameters, from, to, t);
		}

		protected override Snapshot CopySnapshot()
		{
			Snapshot snapshot = new Snapshot();
			snapshot.Initialize();

			foreach (AnimatorControllerParameter parameter in parameters)
			{
				int hash = parameter.nameHash;

				switch (parameter.type)
				{
					case AnimatorControllerParameterType.Float:
						snapshot.floatParameters.Add(hash, animator.GetFloat(hash));
						break;

					case AnimatorControllerParameterType.Int:
						snapshot.intParameters.Add(hash, animator.GetInteger(hash));
						break;

					case AnimatorControllerParameterType.Bool:
						snapshot.boolParameters.Add(hash, animator.GetBool(hash));
						break;
				}
			}

			return snapshot;
		}

		protected override void ApplySnapshot(Snapshot snapshot)
		{
			foreach (KeyValuePair<int, float> floatParameter in snapshot.floatParameters)
			{
				animator.SetFloat(floatParameter.Key, floatParameter.Value);
			}

			foreach (KeyValuePair<int, int> intParameter in snapshot.intParameters)
			{
				animator.SetInteger(intParameter.Key, intParameter.Value);
			}

			foreach (KeyValuePair<int, bool> boolParameter in snapshot.boolParameters)
			{
				animator.SetBool(boolParameter.Key, boolParameter.Value);
			}
		}

		#region Components

		/// <summary>
		/// The components used by the animator recorder are cached for performance optimization. If you add or remove the Timeline or Animator on the GameObject, you need to call this method to update the animator recorder accordingly. 
		/// </summary>
		public override void CacheComponents()
		{
			base.CacheComponents();

			animator = GetComponent<Animator>();

			if (animator == null)
			{
				throw new ChronosException("Missing animator for animator recorder.");
			}

			Debug.LogWarning("Rewinding animator states will be buggy until Unity 5.1.");

			parameters = animator.parameters;
		}

		#endregion
	}
}
