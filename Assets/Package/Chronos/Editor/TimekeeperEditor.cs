using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chronos
{
	[CustomEditor(typeof(Timekeeper))]
	public class TimekeeperEditor : DocumentedEditor
	{
		protected SerializedProperty debug;

		public virtual void OnEnable()
		{
			debug = serializedObject.FindProperty("_debug");
		}

		public override void OnInspectorGUI()
		{
			DrawDocumentationIcon();

			serializedObject.Update();

			Timekeeper timekeeper = (Timekeeper)serializedObject.targetObject;

			EditorGUILayout.PropertyField(debug, new GUIContent("Debug Mode"));

			EditorGUILayout.HelpBox("Add global clocks to this object to configure each clock individually.", MessageType.Info);

			string[] duplicates = timekeeper.GetComponents<GlobalClock>()
				.Select(gc => gc.key)
				.Where(k => !string.IsNullOrEmpty(k))
				.GroupBy(k => k)
				.Where(g => g.Count() > 1)
				.Select(y => y.Key)
				.ToArray();

			if (duplicates.Length > 0)
			{
				EditorGUILayout.HelpBox("The following global clocks have identical keys:\n" + string.Join("\n", duplicates.Select(d => "    - " + d).ToArray()), MessageType.Error);
			}

			serializedObject.ApplyModifiedProperties();
		}

		public static void GlobalClockKeyPopup(string label, SerializedProperty property, IEnumerable<Clock> exclude = null)
		{
			List<string> options = new List<string>();
			List<string> labels = new List<string>();

			if (exclude == null) exclude = new Clock[0];

			if (property.hasMultipleDifferentValues)
			{
				options.Add(null);
				labels.Add("—");
			}

			options.Add(null);
			labels.Add("(None)");

			foreach (string option in Timekeeper.instance
				.GetComponents<GlobalClock>()
				.Where(gc => !exclude.Contains(gc))
				.Select(gc => gc.key)
				.Where(k => !string.IsNullOrEmpty(k)))
			{
				options.Add(option);
				labels.Add(option);
			}

			int selectedIndex = -1;

			if (property.hasMultipleDifferentValues)
			{
				selectedIndex = 0;
			}
			else if (string.IsNullOrEmpty(property.stringValue))
			{
				selectedIndex = property.hasMultipleDifferentValues ? 1 : 0;
			}
			else if (options.Contains(property.stringValue))
			{
				selectedIndex = options.IndexOf(property.stringValue);
			}
			else
			{
				options.Add(property.stringValue);
				labels.Add(string.Format("{0} (Missing)", property.stringValue));
				selectedIndex = options.Count - 1;
			}

			int newIndex = EditorGUILayout.Popup(label, selectedIndex, labels.ToArray());

			if (!property.hasMultipleDifferentValues || newIndex != 0)
			{
				Undo.RecordObjects(property.serializedObject.targetObjects, property.name);
				property.stringValue = newIndex < 0 ? null : options[newIndex];
			}
		}

		[MenuItem("GameObject/Timekeeper", false, 12)]
		private static void MenuCommand(MenuCommand menuCommand)
		{
			if (GameObject.FindObjectOfType<Timekeeper>() != null)
			{
				EditorUtility.DisplayDialog("Chronos", "The scene already contains a timekeeper.", "OK");
				return;
			}

			GameObject go = new GameObject("Timekeeper");
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
			Timekeeper timekeeper = go.AddComponent<Timekeeper>();
			timekeeper.AddClock("Root");
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeObject = go;
		}
	}
}
