using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class Knight49Editor : EditorWindow
{
	[MenuItem ("GameEditor/BuildTool")]
    private static void BuildTool()
    {
		EditorWindow.GetWindowWithRect(typeof(Knight49Editor), new Rect(0, 0, 800, 400), true, "BuildTool").Show();
    }
    
    public float mVersion = 0.8f;
    public int mVersionCode = 1;
    public string mPass = "csharp2014";
    public string mPath;
    public int HeadItemIndex = 51;
    public int ShoesItemIndex = 52;
    public int PlayerIndex = 0;
    public int SkillKind = 0;
    public int Value = 0;
    public int LifeTime = 0;

	const string ClassName = "BundleVersion";
	const string TargetCodeFile = "Assets/04.Scripts/System/" + ClassName + ".cs";

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes [i] = EditorBuildSettings.scenes [i].path;
        }
        
        return scenes;
    }

    void OnGUI()
    {
		mVersion = EditorGUILayout.FloatField("Bundle Version", mVersion);
		mVersionCode = EditorGUILayout.IntField("Bundle mVersionCode", mVersionCode); 

		if (GUILayout.Button("Setting", GUILayout.Width(200)))
		{
		    PlayerSettings.bundleVersion = mVersion.ToString();
		    PlayerSettings.Android.bundleVersionCode = mVersionCode;
		    PlayerSettings.Android.keyaliasPass = mPass;
		    PlayerSettings.Android.keystorePass = mPass;

		    BundleVersionChecker();
		}
    }
	
    static void BundleVersionChecker () {
		float bundleVersion = float.Parse(PlayerSettings.bundleVersion);
		float lastVersion = BundleVersion.version;
		if (lastVersion != bundleVersion) {
            UnityEngine.Debug.Log ("Found new bundle version " + bundleVersion + " replacing code from previous version " + lastVersion +" in file \"" + TargetCodeFile + "\"");
            CreateNewBuildVersionClassFile (bundleVersion);
        }

        AssetDatabase.Refresh ();
    }
    
    static string CreateNewBuildVersionClassFile (float bundleVersion) {
        using (StreamWriter writer = new StreamWriter (TargetCodeFile, false)) {
            try {
                string code = GenerateCode (bundleVersion);
                writer.WriteLine ("{0}", code);
            } catch (System.Exception ex) {
                string msg = " threw:\n" + ex.ToString ();
                UnityEngine.Debug.LogError (msg);
                EditorUtility.DisplayDialog ("Error when trying to regenerate class", msg, "OK");
            }
        }
        return TargetCodeFile;
    }

    static string GenerateCode (float bundleVersion) {
        string code = "public static class " + ClassName + "\n{\n";
        code += System.String.Format ("\tpublic static readonly float version = {0}f;", bundleVersion);
        code += "\n}\n";
        return code;
    }

    private void ShowHint(string str)
    {
        this.ShowNotification(new GUIContent(str));
    }
}
