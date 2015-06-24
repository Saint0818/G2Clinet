using UnityEditor;
using UnityEngine;

namespace Chronos
{
	[CustomEditor(typeof(Timeline)), CanEditMultipleObjects]
	public class TimelineEditor : DocumentedEditor
	{
		protected SerializedProperty mode;
		protected SerializedProperty globalClockKey;

		public void OnEnable()
		{
			mode = serializedObject.FindProperty("_mode");
			globalClockKey = serializedObject.FindProperty("_globalClockKey");
		}

		public override void OnInspectorGUI()
		{
			DrawDocumentationIcon();

			serializedObject.Update();

			EditorGUILayout.PropertyField(mode, new GUIContent("Mode"));

			if (!mode.hasMultipleDifferentValues)
			{
				if (mode.enumValueIndex == (int)TimelineMode.Local)
				{
					if (!serializedObject.isEditingMultipleObjects)
					{
						Timeline timeline = (Timeline)serializedObject.targetObject;

						LocalClock localClock = timeline.GetComponent<LocalClock>();

						if (localClock == null || !localClock.enabled)
						{
							EditorGUILayout.HelpBox("A local timeline requires a local clock.", MessageType.Error);
						}
					}
				}
				else if (mode.enumValueIndex == (int)TimelineMode.Global)
				{
					TimekeeperEditor.GlobalClockKeyPopup("Global Clock", globalClockKey);

					if (!globalClockKey.hasMultipleDifferentValues &&
						string.IsNullOrEmpty(globalClockKey.stringValue))
					{
						EditorGUILayout.HelpBox("A global timeline requires a global clock reference.", MessageType.Error);
					}
				}
				else
				{
					EditorGUILayout.HelpBox("Unsupported timeline mode.", MessageType.Error);
				}
			}

			if (!serializedObject.isEditingMultipleObjects &&
				Application.isPlaying)
			{
				Timeline timeline = (Timeline)serializedObject.targetObject;
				EditorGUILayout.LabelField("Computed Time Scale", timeline.timeScale.ToString("0.00"));
				EditorGUILayout.LabelField("Computed Time", timeline.time.ToString("0.00"));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
