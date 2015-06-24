using UnityEditor;
using UnityEngine;

namespace Chronos
{
	[CustomEditor(typeof(PhysicsTimer2D)), CanEditMultipleObjects]
	public class PhysicsTimer2DEditor : PhysicsTimerEditor<PhysicsTimer2D>
	{
		public override void OnEnable()
		{
			base.OnEnable();
			documentationClass = "PhysicsTimer";
		}

		protected override void CheckForComponents()
		{
			base.CheckForComponents();

			PhysicsTimer2D physicsTimer = (PhysicsTimer2D)serializedObject.targetObject;

			if (physicsTimer.GetComponent<Rigidbody2D>() == null)
			{
				EditorGUILayout.HelpBox("A physics timer requires a rigidbody component.", MessageType.Error);
			}
		}
	}
}
