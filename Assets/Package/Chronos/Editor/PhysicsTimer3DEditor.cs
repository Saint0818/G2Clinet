using UnityEditor;
using UnityEngine;

namespace Chronos
{
	[CustomEditor(typeof(PhysicsTimer3D)), CanEditMultipleObjects]
	public class PhysicsTimer3DEditor : PhysicsTimerEditor<PhysicsTimer3D>
	{
		public override void OnEnable()
		{
			base.OnEnable();
			documentationClass = "PhysicsTimer";
		}

		protected override void CheckForComponents()
		{
			base.CheckForComponents();

			PhysicsTimer3D physicsTimer = (PhysicsTimer3D)serializedObject.targetObject;

			if (physicsTimer.GetComponent<Rigidbody>() == null)
			{
				EditorGUILayout.HelpBox("A physics timer requires a rigidbody component.", MessageType.Error);
			}
		}
	}
}
