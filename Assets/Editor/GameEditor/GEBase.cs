using UnityEngine;
using UnityEditor;
using System.Collections;

public class GEBase : EditorWindow {
	private static Vector2 minWindow = new Vector2(600, 600);
	protected const float Height_Line = 24;
	protected const float Weight_Button = 120;
	public GUIStyle StyleLabel;
	public GUIStyle StyleEdit;
	public GUIStyle StyleButton;

	public virtual void SetStyle() {
		if (StyleLabel == null) {
			StyleLabel = new GUIStyle(EditorStyles.label);
			StyleLabel.fontSize = 18;
			StyleLabel.normal.textColor = Color.white;
		}

		if (StyleEdit == null) {
			StyleEdit = new GUIStyle(EditorStyles.textField);
			StyleEdit.fontSize = 16;
			StyleEdit.normal.textColor = Color.white;
		}
		
		if (StyleButton == null) {
			StyleButton = new GUIStyle(EditorStyles.miniButton);
			StyleButton.fontSize = 16;
		}
		
		this.minSize = minWindow;
	}
}
