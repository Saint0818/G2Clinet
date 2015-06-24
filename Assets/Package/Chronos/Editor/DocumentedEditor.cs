using UnityEngine;
using UnityEditor;

public class DocumentedEditor : Editor
{
	const string documentationUrl = "http://ludiq.io/chronos/documentation";

	protected string documentationClass = null;
	
	protected virtual void DrawDocumentationIcon()
	{
		// Clicking the button triggers the header foldout... Making this useless.
		// Maybe a future Unity version will fix this.

		/*string className = documentationClass ?? target.GetType().Name;
		GUIContent content = EditorGUIUtility.IconContent("_Help");
		content.tooltip = string.Format("Open Chronos Reference for {0}.", className);
		Rect inspector = EditorGUILayout.GetControlRect(GUILayout.Height(0));
		Rect position = new Rect(inspector.xMax - 53, inspector.y - 16, 16, 16);
		GUIStyle style = new GUIStyle();

		if (GUI.Button(position, content, style))
		{
			Help.BrowseURL(string.Format("{0}#{1}", documentationUrl, className));
		}*/
	}
}