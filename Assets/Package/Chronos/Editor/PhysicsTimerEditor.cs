using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chronos
{
	public abstract class PhysicsTimerEditor<TPhysicsTimer> : RecorderEditor<TPhysicsTimer> where TPhysicsTimer : Component, IRecorder
	{

	}
}