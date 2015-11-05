using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class GEBase : EditorWindow {
	private static Vector2 minWindow = new Vector2(300, 600);
	protected const float Height_Line = 24;
	protected const float Weight_Button = 80;
	protected GUIStyle StyleLabel;
	protected GUIStyle StyleEdit;
	protected GUIStyle StyleButton;

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

	protected void GUILabel(string text) {
		GUILabel(text, Color.white);
	}

	protected virtual void GUILabel(string text, Color color) {
		StyleLabel.normal.textColor = color;
		GUILayout.Label(text, StyleLabel, GUILayout.Height(Height_Line));
		StyleLabel.normal.textColor = Color.white;
	}

	protected virtual int GUIIntEdit(int input, string title = "") {
		GUILabel(title);
		return EditorGUILayout.IntField(input, StyleEdit, GUILayout.Width(Weight_Button / 2), GUILayout.Height(Height_Line));
	}

	protected virtual float GUIFloatEdit(float input, string title = "") {
		GUILabel(title);
		return EditorGUILayout.FloatField(input, StyleEdit, GUILayout.Width(Weight_Button / 2), GUILayout.Height(Height_Line));
	}

	protected virtual bool GUIToggle(bool input, string title = "") {
		GUILabel(title);
		return EditorGUILayout.Toggle(input, GUILayout.Width(Weight_Button / 2), GUILayout.Height(Height_Line));
	}

	protected bool GUIButton(string title) {
		return GUIButton(title, Color.white);
	}

	protected virtual bool GUIButton(string title, Color color) {
		StyleButton.normal.textColor = color;
		bool clicked = GUILayout.Button(title, StyleButton, GUILayout.Width(Weight_Button), GUILayout.Height(Height_Line));
		StyleButton.normal.textColor = Color.white;
		return clicked;
	}

	protected virtual int GUIPopup(int select, string[] displayOptions, string title = "") {
		if (!string.IsNullOrEmpty(title))
			GUILabel(title);

		return EditorGUILayout.Popup(select, displayOptions, StyleButton, GUILayout.Width(Weight_Button * 2), GUILayout.Height(Height_Line));
	}

	protected static void SaveFile(string fileName, string Data) {
		if (File.Exists(fileName))
			File.WriteAllText(fileName, string.Empty);
		
		using (FileStream myFile = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
			using (StreamWriter myWriter = new StreamWriter(myFile)) {
				myWriter.Write(Data);
				myWriter.Close();
			}
			
			myFile.Close();
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
