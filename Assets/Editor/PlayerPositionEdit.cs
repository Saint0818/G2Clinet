using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Newtonsoft.Json;
using GamePlayStruct;

public class PlayerPositionEdit : EditorWindow
{

    [MenuItem ("GameEditor/PlayerMoves")]
    private static void PositionEdit()
    {
        EditorWindow.GetWindowWithRect(typeof(PlayerPositionEdit), new Rect(0, 0, 1200, 800), true, "PlayerMoves").Show();
        FileName = Application.dataPath + "/Resources/GameData/tactical.json";
		BackupFileName = Application.dataPath + "/Resources/GameData/Backup/tactical_" + DateTime.Now.ToString("MM-dd-yy") + ".json";
		BornAy [0] = new Vector2 (0, 0);
		BornAy [1] = new Vector2 (5, -2);
		BornAy [2] = new Vector2 (-5, -2);
    }

    private float x;
    private float z;
    private bool ControlPlayer1 = true;
    private bool ControlPlayer2 = false;
    private bool ControlPlayer3 = false;
    private int DataCount = 0;
    private int EditIndex = -1;
    private int OldIndex = -1;
    private int PositionCount1 = 0;
    private int PositionCount2 = 0;
    private int PositionCount3 = 0;
    private int _newIdx = 0;
    private int _oldIdx = 0;
    private string tacticalName = "";
    private static string FileName = "";
	private static string BackupFileName = "";
    private TActionPosition[] PosAy1 = new TActionPosition[0];
    private TActionPosition[] PosAy2 = new TActionPosition[0];
    private TActionPosition[] PosAy3 = new TActionPosition[0];
    private string[] ArrayString = new string[0];
    private TTacticalData[] TacticalData = new TTacticalData[0];
	private static Vector2 [] BornAy = new Vector2[3];

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load", GUILayout.Width(200))) 
            OnLoad();

        if (ArrayString.Length > 0)
        {
            _newIdx = EditorGUILayout.Popup(_oldIdx, ArrayString);
            if (_newIdx != _oldIdx) 
                _oldIdx = _newIdx;

            if (GUILayout.Button("Load Tactical", GUILayout.Width(200))) 
                OnLoadTactical();
        }

        EditorGUILayout.EndHorizontal();
            
        DataCount = EditorGUILayout.IntField("DataCount", DataCount);
        PositionCount1 = EditorGUILayout.IntField("Position Count_1", PositionCount1);  
        PositionCount2 = EditorGUILayout.IntField("Position Count_2", PositionCount2);  
        PositionCount3 = EditorGUILayout.IntField("Position Count_3", PositionCount3);  

        if (GUILayout.Button("Data Size", GUILayout.Width(200)))
            ResetTacticalArray(DataCount);

        if (GUILayout.Button("Array Setting", GUILayout.Width(200)))
            ResetPositionArraySize();

        if (GUILayout.Button("Move", GUILayout.Width(200)))
        {
            if (PosAy1.Length > 0)
            {
                for (int i = 0; i < PosAy1.Length; i++)
                {
                    GameController.Get.EditSetMove(PosAy1 [i], 0);
                }
            }

            if (PosAy2.Length > 0)
            {
                for (int i = 0; i < PosAy2.Length; i++)
                {
                    GameController.Get.EditSetMove(PosAy2 [i], 1);
                }
            }

            if (PosAy3.Length > 0)
            {
                for (int i = 0; i < PosAy3.Length; i++)
                {
                    GameController.Get.EditSetMove(PosAy3 [i], 2);
                }
            }
        }

		if (GUILayout.Button("BornPosition", GUILayout.Width(200)))
		{
			TActionPosition aa = new TActionPosition();
			GameController.Get.ResetAll();
			for(int i = 0; i < 3; i++)
			{
				aa.x = BornAy[i].x;
				aa.z = BornAy[i].y;
				GameController.Get.EditSetMove(aa, i);
			}
		}

        EditorGUILayout.BeginHorizontal();
        tacticalName = EditorGUILayout.TextField("FileName", tacticalName);
        EditIndex = EditorGUILayout.IntField("Save Index(0~" + (TacticalData.Length - 1).ToString() + ")", EditIndex);

        if (OldIndex != EditIndex)
        {
            if (TacticalData != null && TacticalData.Length > 0 && EditIndex >= 0 && EditIndex < TacticalData.Length)
                tacticalName = TacticalData [EditIndex].FileName;
            else
                tacticalName = "";
            OldIndex = EditIndex;
        }

        if (GUILayout.Button("Save", GUILayout.Width(200)))
            OnSave();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        ControlPlayer1 = EditorGUILayout.Toggle("Player1", ControlPlayer1);
        if (ControlPlayer1)
        {
            ControlPlayer2 = false;
            ControlPlayer3 = false;
            if (GameController.Visible)
            {
                GameController.Get.EditSetJoysticker(0);
            }
        }

        ControlPlayer2 = EditorGUILayout.Toggle("Player2", ControlPlayer2);
        if (ControlPlayer2)
        {
            ControlPlayer1 = false;
            ControlPlayer3 = false;
            if (GameController.Visible)
            {
                GameController.Get.EditSetJoysticker(1);
            }
        }

        ControlPlayer3 = EditorGUILayout.Toggle("Player3", ControlPlayer3);
        if (ControlPlayer3)
        {
            ControlPlayer1 = false;
            ControlPlayer2 = false;
            if (GameController.Visible)
            {
                GameController.Get.EditSetJoysticker(2);
            }
        }

        EditorGUILayout.EndHorizontal();
        
        DoEditPosition();
    }

    private void ResetTacticalArray(int newSize)
    {
        if (newSize > 0)
        {
            int c1 = TacticalData.Length;
            Array.Resize(ref TacticalData, newSize);

            if (newSize > c1)
            {
                for (int i = c1; i < newSize; i++)
                    TacticalData [i] = new TTacticalData();
            }

            FlashTacticalName();
            Debug.Log(TacticalData.Length);
        }
    }
    
    private void ResetPositionArraySize()
    {
        if (PositionCount1 > 0 && PositionCount1 != PosAy1.Length)
            Array.Resize(ref PosAy1, PositionCount1);
        
        if (PositionCount2 > 0 && PositionCount2 != PosAy2.Length)
            Array.Resize(ref PosAy2, PositionCount2);
        
        if (PositionCount3 > 0 && PositionCount3 != PosAy3.Length)
            Array.Resize(ref PosAy3, PositionCount3);
    }

    private void FlashTacticalName()
    {
        DataCount = TacticalData.Length;
        ArrayString = new string[TacticalData.Length];
        for (int i = 0; i < TacticalData.Length; i++)
            ArrayString [i] = TacticalData [i].FileName;
    }

    private void OnLoad()
    {
        if (File.Exists(FileName))
        {
            TextAsset tx = Resources.Load("GameData/tactical") as TextAsset;
            if (tx)
            {
                TacticalData = (TTacticalData[])JsonConvert.DeserializeObject(tx.text, typeof(TTacticalData[]));
                FlashTacticalName();
            } 
        }
    }

    private void OnLoadTactical()
    {
        if (_oldIdx >= 0 && _oldIdx < ArrayString.Length)
        {
            PositionCount1 = TacticalData [_oldIdx].PosAy1.Length;
            PosAy1 = new TActionPosition[PositionCount1];
            Array.Copy(TacticalData [_oldIdx].PosAy1, PosAy1, PositionCount1);
            
            PositionCount2 = TacticalData [_oldIdx].PosAy2.Length;
            PosAy2 = new TActionPosition[PositionCount2];
            Array.Copy(TacticalData [_oldIdx].PosAy2, PosAy2, PositionCount2);
            
            PositionCount3 = TacticalData [_oldIdx].PosAy3.Length;
            PosAy3 = new TActionPosition[PositionCount3];
            Array.Copy(TacticalData [_oldIdx].PosAy3, PosAy3, PositionCount3);
            
            tacticalName = ArrayString [_oldIdx];
            EditIndex = _oldIdx;
            OldIndex = EditIndex;
        }
    }
    
    private void OnSave()
    {
        if (TacticalData != null && TacticalData.Length > 0 && EditIndex >= 0 && EditIndex < TacticalData.Length)
        {
            if (tacticalName != string.Empty)
            {
                int i = EditIndex;
                TacticalData [i].FileName = tacticalName;
                TacticalData [i].PosAy1 = new TActionPosition[PosAy1.Length];
                Array.Copy(PosAy1, TacticalData [i].PosAy1, PosAy1.Length);
                
                TacticalData [i].PosAy2 = new TActionPosition[PosAy2.Length];
                Array.Copy(PosAy2, TacticalData [i].PosAy2, PosAy2.Length);
                
                TacticalData [i].PosAy3 = new TActionPosition[PosAy3.Length];
                Array.Copy(PosAy3, TacticalData [i].PosAy3, PosAy3.Length);
                
                for (int j = 0; j < TacticalData[i].PosAy1.Length; j ++)
                {
                    TacticalData [i].PosAy1 [j].x = (float)System.Math.Round(TacticalData [i].PosAy1 [j].x, 2);
                    TacticalData [i].PosAy1 [j].z = (float)System.Math.Round(TacticalData [i].PosAy1 [j].z, 2);
                }
                
                for (int j = 0; j < TacticalData[i].PosAy2.Length; j ++)
                {
                    TacticalData [i].PosAy2 [j].x = (float)System.Math.Round(TacticalData [i].PosAy2 [j].x, 2);
                    TacticalData [i].PosAy2 [j].z = (float)System.Math.Round(TacticalData [i].PosAy2 [j].z, 2);
                }
                
                for (int j = 0; j < TacticalData[i].PosAy3.Length; j ++)
                {
                    TacticalData [i].PosAy3 [j].x = (float)System.Math.Round(TacticalData [i].PosAy3 [j].x, 2);
                    TacticalData [i].PosAy3 [j].z = (float)System.Math.Round(TacticalData [i].PosAy3 [j].z, 2);
                }
                
                SaveFile(FileName, JsonConvert.SerializeObject(TacticalData));
				SaveFile(BackupFileName, JsonConvert.SerializeObject(TacticalData));
                FlashTacticalName();
                Debug.Log(FileName);
				Debug.Log(BackupFileName);
            } else
                Debug.LogError("FileName is empty");
        } else
            Debug.LogError("EditIndex error");
    }
      
    private void DoEditPosition()
    {
        if (PosAy1.Length > 0)
        {
            GUI.color = Color.yellow;   
            EditorGUILayout.LabelField("Player1(Center)");
            GUI.color = Color.white;   
            for (int i = 0; i < PosAy1.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Vector3 v = EditorGUILayout.Vector3Field("(" + (i + 1).ToString() + ")", new Vector3(PosAy1 [i].x, 0, PosAy1 [i].z));
                PosAy1 [i].x = v.x;
                PosAy1 [i].z = v.z;

				if (GUILayout.Button("MoveTo_" + (i + 1).ToString(), GUILayout.Height(32)))
				{
					GameController.Get.EditSetMove(new Vector2(PosAy1 [i].x, PosAy1 [i].z), 0);
				}

                if (GUILayout.Button("Capture Position_" + (i + 1).ToString(), GUILayout.Height(32)))
                {
                    Vector3 Res = GameController.Get.EditGetPosition(0);
                    PosAy1 [i].x = Convert.ToSingle(Math.Round(Res.x, 2));
                    PosAy1 [i].z = Convert.ToSingle(Math.Round(Res.z, 2));
                }                   
                PosAy1 [i].Speedup = EditorGUILayout.Toggle("Speedup", PosAy1 [i].Speedup);
				PosAy1 [i].Catcher = EditorGUILayout.Toggle("Catcher", PosAy1 [i].Catcher);
				PosAy1 [i].Shooting = EditorGUILayout.Toggle("Shooting", PosAy1 [i].Shooting);
                EditorGUILayout.EndHorizontal();
            }               
        }
        
        if (PosAy2.Length > 0)
        {
            GUI.color = Color.green;   
            EditorGUILayout.LabelField("Player2(Forward)");
            GUI.color = Color.white;   
            for (int i = 0; i < PosAy2.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Vector3 v = EditorGUILayout.Vector3Field("(" + (i + 1).ToString() + ")", new Vector3(PosAy2 [i].x, 0, PosAy2 [i].z));
                PosAy2 [i].x = v.x;
                PosAy2 [i].z = v.z;

				if (GUILayout.Button("MoveTo_" + (i + 1).ToString(), GUILayout.Height(32)))
				{
					GameController.Get.EditSetMove(new Vector2(PosAy2 [i].x, PosAy2 [i].z), 1);
				}

                if (GUILayout.Button("Capture Position_" + (i + 1).ToString(), GUILayout.Height(32)))
                {
                    Vector3 Res = GameController.Get.EditGetPosition(1);
                    PosAy2 [i].x = Convert.ToSingle(Math.Round(Res.x, 2));
                    PosAy2 [i].z = Convert.ToSingle(Math.Round(Res.z, 2));
                }                   
                PosAy2 [i].Speedup = EditorGUILayout.Toggle("Speedup", PosAy2 [i].Speedup);
				PosAy2 [i].Catcher = EditorGUILayout.Toggle("Catcher", PosAy2 [i].Catcher);
				PosAy2 [i].Shooting = EditorGUILayout.Toggle("Shooting", PosAy2 [i].Shooting);
                EditorGUILayout.EndHorizontal();
            }               
        }
        
        if (PosAy3.Length > 0)
        {
            GUI.color = Color.red;   
            EditorGUILayout.LabelField("Player3(Guard)");
            GUI.color = Color.white;   
            for (int i = 0; i < PosAy3.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Vector3 v = EditorGUILayout.Vector3Field("(" + (i + 1).ToString() + ")", new Vector3(PosAy3 [i].x, 0, PosAy3 [i].z));
                PosAy3 [i].x = v.x;
                PosAy3 [i].z = v.z;

				if (GUILayout.Button("MoveTo_" + (i + 1).ToString(), GUILayout.Height(32)))
				{
					GameController.Get.EditSetMove(new Vector2(PosAy3 [i].x, PosAy3 [i].z), 2);
				}

                if (GUILayout.Button("Capture Position_" + (i + 1).ToString(), GUILayout.Height(32)))
                {
                    Vector3 Res = GameController.Get.EditGetPosition(2);
                    PosAy3 [i].x = Convert.ToSingle(Math.Round(Res.x, 2));
                    PosAy3 [i].z = Convert.ToSingle(Math.Round(Res.z, 2));
                }       
                PosAy3 [i].Speedup = EditorGUILayout.Toggle("Speedup", PosAy3 [i].Speedup);
				PosAy3 [i].Catcher = EditorGUILayout.Toggle("Catcher", PosAy3 [i].Catcher);
				PosAy3 [i].Shooting = EditorGUILayout.Toggle("Shooting", PosAy3 [i].Shooting);
                EditorGUILayout.EndHorizontal();
            }               
        }
    }

    public void SaveFile(string fileName, string Data)
    {
        if (File.Exists(fileName))
            File.WriteAllText(fileName, string.Empty);

        using (FileStream myFile = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            using (StreamWriter myWriter = new StreamWriter(myFile))
            {
                myWriter.Write(Data);
                myWriter.Close();
            }

            myFile.Close();
        }
    }

    public string LoadFile(string OpenFileName)
    {
        string InData = "";
        using (FileStream myFile = File.Open(OpenFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            using (StreamReader myReader = new StreamReader(myFile))
            {
                InData = myReader.ReadToEnd();
                myReader.Close();
            }
            myFile.Close();
        }

        return InData;
    }
}
