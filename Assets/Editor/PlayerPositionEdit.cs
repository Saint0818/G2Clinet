using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Newtonsoft.Json;

public class PlayerPositionEdit : EditorWindow {

	[MenuItem ("Player/PositionEdit")]
	private static void PositionEdit()
	{
		EditorWindow.GetWindowWithRect(typeof(PlayerPositionEdit), new Rect(0, 0, 800, 900), true, "PlayerPositionEdit").Show();
	}
	
	private bool ControlPlayer1 = true;
	private bool ControlPlayer2 = false;
	private bool ControlPlayer3 = false;
	private int PositionCount1 = 0;
	private int PositionCount2 = 0;
	private int PositionCount3 = 0;
	private int _newIdx = 0;
	private int _oldIdx = 0;
	private string FileName = "";
	private TActionPosition [] PosAy1 = new TActionPosition[0];
	private TActionPosition [] PosAy2 = new TActionPosition[0];
	private TActionPosition [] PosAy3 = new TActionPosition[0];		
	private string[] ArrayString = new string[0];

	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("Get File", GUILayout.Width (200))) {
			DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
			FileInfo[] info = dir.GetFiles("*.txt");
			if(info.Length > 0){
				ArrayString = new string[info.Length];
				int i = 0;
				foreach (FileInfo f in info){
					string[] strChars = f.Name.Split(new char[] {'.'});
					ArrayString[i] = strChars[0];
					i++;
				}
			}
		}

		if (ArrayString.Length > 0) {
			_newIdx = EditorGUILayout.Popup(_oldIdx, ArrayString);
			if (_newIdx != _oldIdx) {
				_oldIdx = _newIdx;
			}

			if (GUILayout.Button ("Load File", GUILayout.Width (200))) {
				if(_oldIdx >= 0 && _oldIdx < ArrayString.Length){
					string filedata = StringRead(Application.persistentDataPath + "/" + ArrayString[_oldIdx] + ".txt");
					TTactical saveData2 = new TTactical();
					GetJsonData(filedata, ref saveData2); 

					PositionCount1 = saveData2.PosAy1.Length;
					PosAy1 = new TActionPosition[PositionCount1];
					PosAy1 = saveData2.PosAy1;
					PositionCount2 = saveData2.PosAy2.Length;
					PosAy2 = new TActionPosition[PositionCount2];
					PosAy2 = saveData2.PosAy2;
					PositionCount3 = saveData2.PosAy3.Length;
					PosAy3 = new TActionPosition[PositionCount3];
					PosAy3 = saveData2.PosAy3;
					FileName = ArrayString[_oldIdx];
				}
			}
		}
		EditorGUILayout.EndHorizontal();
			

		PositionCount1 = EditorGUILayout.IntField("Position Count_1", PositionCount1);	
		PositionCount2 = EditorGUILayout.IntField("Position Count_2", PositionCount2);	
		PositionCount3 = EditorGUILayout.IntField("Position Count_3", PositionCount3);	

		if (GUILayout.Button("Array Setting", GUILayout.Width(200)))
		{
			if(PositionCount1 > 0 && PositionCount1 != PosAy1.Length)
				PosAy1 = new TActionPosition[PositionCount1];

			if(PositionCount2 > 0 && PositionCount2 != PosAy2.Length)
				PosAy2 = new TActionPosition[PositionCount2];

			if(PositionCount3 > 0 && PositionCount3 != PosAy3.Length)
				PosAy3 = new TActionPosition[PositionCount3];
		}

		if (GUILayout.Button("Move", GUILayout.Width(200)))
		{
			if (PosAy1.Length > 0) {
				for(int i = 0 ; i < PosAy1.Length; i++){
					GameController.Get.EditSetMove(PosAy1[i], 0);
				}
			}

			if (PosAy2.Length > 0) {
				for(int i = 0 ; i < PosAy2.Length; i++){
					GameController.Get.EditSetMove(PosAy2[i], 1);
				}
			}

			if (PosAy3.Length > 0) {
				for(int i = 0 ; i < PosAy3.Length; i++){
					GameController.Get.EditSetMove(PosAy3[i], 2);
				}
			}
		}

		EditorGUILayout.BeginHorizontal();
		FileName = EditorGUILayout.TextField("FileName", FileName);
		if (GUILayout.Button("Save", GUILayout.Width(200)))
		{
			TTactical saveData = new TTactical();
			saveData.PosAy1 = new TActionPosition[PosAy1.Length];
			saveData.PosAy1 = PosAy1;
			
			saveData.PosAy2 = new TActionPosition[PosAy2.Length];
			saveData.PosAy2 = PosAy2;
			
			saveData.PosAy3 = new TActionPosition[PosAy3.Length];
			saveData.PosAy3 = PosAy3;

			string aaa = GetJsonStr(saveData);
			StringWrite(Application.persistentDataPath + "/" + FileName + ".txt", aaa);
			Debug.Log(Application.persistentDataPath + "/" + FileName + ".txt");									
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		ControlPlayer1 = EditorGUILayout.Toggle("Player1", ControlPlayer1);
		if (ControlPlayer1) {
			ControlPlayer2 = false;
			ControlPlayer3 = false;
			if(GameController.Visible){
				GameController.Get.EditSetJoysticker(0);
			}
		}

		ControlPlayer2 = EditorGUILayout.Toggle("Player2", ControlPlayer2);
		if (ControlPlayer2) {
			ControlPlayer1 = false;
			ControlPlayer3 = false;
			if(GameController.Visible){
				GameController.Get.EditSetJoysticker(1);
			}
		}

		ControlPlayer3 = EditorGUILayout.Toggle("Player3", ControlPlayer3);
		if (ControlPlayer3) {
			ControlPlayer1 = false;
			ControlPlayer2 = false;
			if(GameController.Visible){
				GameController.Get.EditSetJoysticker(2);
			}
		}
		EditorGUILayout.EndHorizontal();
		
		if (PosAy1.Length > 0) {
			GUI.color = Color.yellow;   
			EditorGUILayout.LabelField("Player1");
			GUI.color = Color.white;   
			for(int i = 0 ; i < PosAy1.Length; i++){
				EditorGUILayout.BeginHorizontal();
					PosAy1[i].Position = EditorGUILayout.Vector3Field("(" + (i + 1).ToString() + ")", PosAy1[i].Position);
					if(GUILayout.Button("Capture Position_" + (i + 1).ToString(), GUILayout.Height(32))){
						Vector3 Res = GameController.Get.EditGetPosition(0);
						float x = Convert.ToSingle(Math.Round(Res.x, 2));
						float z = Convert.ToSingle(Math.Round(Res.z, 2));
						PosAy1[i].Position = new Vector3(x, 0, z);
					}					
				PosAy1[i].Speedup = EditorGUILayout.Toggle("Speedup", PosAy1[i].Speedup);
				EditorGUILayout.EndHorizontal();
			}				
		}

		if (PosAy2.Length > 0) {
			GUI.color = Color.green;   
			EditorGUILayout.LabelField("Player2");
			GUI.color = Color.white;   
			for(int i = 0 ; i < PosAy2.Length; i++){
				EditorGUILayout.BeginHorizontal();
				PosAy2[i].Position = EditorGUILayout.Vector3Field("(" + (i + 1).ToString() + ")", PosAy2[i].Position);
				if(GUILayout.Button("Capture Position_" + (i + 1).ToString(), GUILayout.Height(32))){
					Vector3 Res = GameController.Get.EditGetPosition(1);
					float x = Convert.ToSingle(Math.Round(Res.x, 2));
					float z = Convert.ToSingle(Math.Round(Res.z, 2));
					PosAy2[i].Position = new Vector3(x, 0, z);
				}					
				PosAy2[i].Speedup = EditorGUILayout.Toggle("Speedup", PosAy2[i].Speedup);
				EditorGUILayout.EndHorizontal();
			}				
		}

		if (PosAy3.Length > 0) {
			GUI.color = Color.red;   
			EditorGUILayout.LabelField("Player3");
			GUI.color = Color.white;   
			for(int i = 0 ; i < PosAy3.Length; i++){
				EditorGUILayout.BeginHorizontal();
				PosAy3[i].Position = EditorGUILayout.Vector3Field("(" + (i + 1).ToString() + ")", PosAy3[i].Position);
				if(GUILayout.Button("Capture Position_" + (i + 1).ToString(), GUILayout.Height(32))){
					Vector3 Res = GameController.Get.EditGetPosition(2);
					float x = Convert.ToSingle(Math.Round(Res.x, 2));
					float z = Convert.ToSingle(Math.Round(Res.z, 2));
					PosAy3[i].Position = new Vector3(x, 0, z);
				}		
				PosAy3[i].Speedup = EditorGUILayout.Toggle("Speedup", PosAy3[i].Speedup);
				EditorGUILayout.EndHorizontal();
			}				
		}
	}

	public static void StringWrite(string fileName, string Data)
	{
		FileStream myFile = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		StreamWriter myWriter = new StreamWriter(myFile);
		myWriter.Write(Data);
		myWriter.Close();
		myFile.Close();
	}

	public static string StringRead(string OpenFileName)
	{
		string InData = "";
		FileStream myFile = File.Open(OpenFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		StreamReader myReader = new StreamReader(myFile);
		InData = myReader.ReadToEnd();
		myReader.Close();
		myFile.Close();
		return InData;
	}

	// Json Encoding
	public static string GetJsonStr(object obj)
	{
		return JsonConvert.SerializeObject(obj);
	}
	
	// Json Decoding
	public static void GetJsonData<T>(string Str,ref T obj)
	{
		obj = JsonConvert.DeserializeObject <T>(Str);
	}
}
