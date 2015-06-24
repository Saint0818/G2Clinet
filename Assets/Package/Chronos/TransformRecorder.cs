using UnityEngine;
using System.Collections;

namespace Chronos
{
	/// <summary>
	/// A component that enables rewinding the transform (position, rotation and scale) of the GameObject via recorded snapshots.
	/// </summary>
	public class TransformRecorder : Recorder<TransformRecorder.Snapshot>
	{
		public struct Snapshot
		{
			public Vector3 position;
			public Quaternion rotation;
			public Vector3 scale;

			public static Snapshot Lerp(Snapshot from, Snapshot to, float t)
			{
				return new Snapshot()
				{
					position = Vector3.Lerp(from.position, to.position, t),
					rotation = Quaternion.Lerp(from.rotation, to.rotation, t),
					scale = Vector3.Lerp(from.scale, to.scale, t)
				};
			}
		}

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
				scale = transform.localScale
			};
		}

		protected override void ApplySnapshot(Snapshot snapshot)
		{
			transform.position = snapshot.position;
			transform.rotation = snapshot.rotation;
			transform.localScale = snapshot.scale;
		}
	}
}