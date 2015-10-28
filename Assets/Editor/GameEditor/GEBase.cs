using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class GEBase : EditorWindow {
	private static Vector2 minWindow = new Vector2(300, 600);
	protected const float Height_Line = 24;
	protected const float Weight_Button = 120;
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

	protected void SaveFile(string fileName, string Data) {
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
	
	protected string LoadFile(string fileName) {
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
