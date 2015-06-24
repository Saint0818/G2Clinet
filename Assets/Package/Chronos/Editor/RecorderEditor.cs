using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chronos
{
	public abstract class RecorderEditor<TRecorder> : DocumentedEditor where TRecorder : Component, IRecorder
	{
		protected SerializedProperty recordingDuration;
		protected SerializedProperty recordingInterval;

		public virtual void OnEnable()
		{
			recordingDuration = serializedObject.FindProperty("_recordingDuration");
			recordingInterval = serializedObject.FindProperty("_recordingInterval");
		}

		public override void OnInspectorGUI()
		{
			DrawDocumentationIcon();

			serializedObject.Update();

			if (!serializedObject.isEditingMultipleObjects)
			{
				CheckForComponents();
			}

			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			{
				EditorGUILayout.PropertyField(recordingDuration, new GUIContent("Recording Duration"));
				EditorGUILayout.PropertyField(recordingInterval, new GUIContent("Recording Interval"));
			}
			EditorGUI.EndDisabledGroup();

			float estimate = serializedObject.targetObjects.OfType<TRecorder>().Select(pt => pt.EstimateMemoryUsage()).Sum() / 1024;

			string summary;

			if (!recordingDuration.hasMultipleDifferentValues &&
				!recordingInterval.hasMultipleDifferentValues)
			{
				summary = string.Format("Rewind for up to {0:0.#} {1} at a {2:0.#} {3} per second precision.\n\nEstimated memory: {4} KiB.",
										recordingDuration.floatValue,
										recordingDuration.floatValue >= 2 ? "seconds" : "second",
										(1 / recordingInterval.floatValue),
										(1 / recordingInterval.floatValue) >= 2 ? "snapshots" : "snapshot",
										estimate);
			}
			else
			{
				summary = string.Format("Estimated memory: {0} KiB.", estimate);
			}

			EditorGUILayout.HelpBox(summary, MessageType.Info);

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void CheckForComponents()
		{
			TRecorder physicsTimer = (TRecorder)serializedObject.targetObject;

			Timeline timeline = physicsTimer.GetComponent<Timeline>();

			if (timeline == null)
			{
				EditorGUILayout.HelpBox("A recorder requires a timeline component.", MessageType.Error);
			}
		}
	}
}