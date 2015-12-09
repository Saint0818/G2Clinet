using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class GEBase : EditorWindow {
	private static Vector2 minWindow = new Vector2(300, 600);
	protected const float Height_Line = 24;
	protected const float Width_Button = 80;
	protected GUIStyle StyleLayout;
	protected GUIStyle StyleLabel;
	protected GUIStyle StyleEdit;
	protected GUIStyle StyleButton;

	public virtual void SetStyle() {

		if (StyleLayout == null) {
			StyleLayout = new GUIStyle(EditorStyles.label);
			StyleLayout.fontSize = 18;
			StyleLayout.normal.textColor = Color.white;
			StyleLayout.alignment = TextAnchor.LowerLeft;
		}

		if (StyleLabel == null) {
			StyleLabel = new GUIStyle(EditorStyles.label);
			StyleLabel.fontSize = 18;
			StyleLabel.normal.textColor = Color.white;
			StyleLabel.alignment = TextAnchor.LowerLeft;
		}

		if (StyleEdit == null) {
			StyleEdit = new GUIStyle(EditorStyles.numberField);
			StyleEdit.fontSize = 16;
			StyleEdit.normal.textColor = Color.white;
			StyleEdit.alignment = TextAnchor.LowerLeft;
		}
		
		if (StyleButton == null) {
			StyleButton = new GUIStyle(EditorStyles.miniButton);
			StyleButton.fontSize = 16;
			StyleButton.alignment = TextAnchor.LowerLeft;
		}
		
		this.minSize = minWindow;
	}

	protected void GUILabel(string text) {
		GUILabel(text, Color.white);
	}

	protected virtual void GUILabel(string text, Color color) {
		StyleLabel.normal.textColor = color;
		GUILayout.Label(text, StyleLabel, GUILayout.Height(Height_Line));
		StyleLabel.normal.textColor = Color.white;
	}

	protected virtual int GUIIntEdit(int input, string title = "", float width = Width_Button / 2) {
		StyleLabel.alignment = TextAnchor.LowerRight;
		GUILabel(title);
		StyleLabel.alignment = TextAnchor.LowerLeft;
		return EditorGUILayout.IntField(input, StyleEdit, GUILayout.Width(width), GUILayout.Height(Height_Line));
	}

	protected virtual float GUIFloatEdit(float input, string title = "") {
		StyleLabel.alignment = TextAnchor.LowerRight;
		GUILabel(title);
		StyleLabel.alignment = TextAnchor.LowerLeft;
		return EditorGUILayout.FloatField(input, StyleEdit, GUILayout.Width(Width_Button), GUILayout.Height(Height_Line));
	}

	protected virtual bool GUIToggle(bool input, string title = "") {
		StyleLabel.alignment = TextAnchor.LowerRight;
		GUILabel(title);
		StyleLabel.alignment = TextAnchor.LowerLeft;
		return EditorGUILayout.Toggle(input, GUILayout.Width(Width_Button / 2), GUILayout.Height(Height_Line));
	}

	protected bool GUIButton(string title, float width = Width_Button) {
		return GUIButton(title, Color.white, width);
	}

	protected virtual bool GUIButton(string title, Color color, float width = Width_Button) {
		StyleButton.normal.textColor = color;
		bool clicked = GUILayout.Button(title, StyleButton, GUILayout.Width(width), GUILayout.Height(Height_Line));
		StyleButton.normal.textColor = Color.white;
		return clicked;
	}

	protected virtual int GUIPopup(int select, string[] displayOptions, string title = "") {
		if (!string.IsNullOrEmpty(title)) {
			StyleLabel.alignment = TextAnchor.LowerRight;
			GUILabel(title);
			StyleLabel.alignment = TextAnchor.LowerLeft;
		}

		return EditorGUILayout.Popup(select, displayOptions, StyleButton, GUILayout.Width(Width_Button * 2), GUILayout.Height(Height_Line));
	}

	protected static void SaveFile(string fileName, string data)
    {
		if(File.Exists(fileName))
			File.WriteAllText(fileName, string.Empty);

        using (FileStream file = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            using (StreamWriter myWriter = new StreamWriter(file))
            {
                myWriter.Write(data);
                myWriter.Close();
            }
            file.Close();
        }
	}
	
	protected static string LoadFile(string fileName) {
		if (File.Exists(fileName)) {
			string InData = "";
			using (FileStream myFile = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				using (StreamReader myReader = new StreamReader(myFile)) {
					InData = myReader.ReadToEnd();
					myReader.Close();
				}

				myFile.Close();
			}
			
			return InData;
		} else
			return "";
	}
}
