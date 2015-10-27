using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System;

public class GEBuildTool : GEBase {
	public float mVersion = 0.131f;
    public int mVersionCode = 13;
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

//	static string[] appName = new string[2]{"Gang of Basketball.apk", "Gang of Basketball 2.apk"};

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes [i] = EditorBuildSettings.scenes [i].path;
        }
        
        return scenes;
    }

	public enum EAcouunt
	{
		nbaa,
		g2
	}
	
	private static string[] accounts = new string[2]{"nbaa", "g2"};
	private string[] appName = new string[2]{"Gang of Basketball", "Gang of Basketball 2"};
	private string companyName = "Nice Market";
	private string bundleId = "com.nicemarket.g2";
	private int selectedIndex = 1;
	private int[] sizes = new int[]{0,1};
	static string[] SCENES = FindEnabledEditorScenes();

	private static string[] FindEnabledEditorScenes() {
		List<string> EditorScenes = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if(!scene.enabled) continue;
			EditorScenes.Add(scene.path);
		}
		return EditorScenes.ToArray();
	}

	static void PerformAndroidBuild(string appname) {
		string target_dir = appname;
		string buildPath = System.IO.Directory.GetCurrentDirectory() + "/build-android";
		CreateDirectory(buildPath);
		GenericBuild(SCENES, buildPath + "/" +  target_dir, BuildTarget.Android, BuildOptions.None);
	}

	static private void CreateDirectory(string path){
		if(!System.IO.Directory.Exists (path))
			System.IO.Directory.CreateDirectory (path);
	}

	static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options) {
		EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
		string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
		if(res.Length > 0) {
			throw new Exception("BuildPlayer failure: " + res);
		}
	}

    void OnGUI()
    {
		selectedIndex = EditorGUILayout.IntPopup("Games Select: ", selectedIndex, accounts, sizes);
		mVersion = EditorGUILayout.FloatField("Bundle Version", mVersion);
		mVersionCode = EditorGUILayout.IntField("Bundle mVersionCode", mVersionCode); 

		if (GUILayout.Button("Build Android", GUILayout.Width(200)))
		{
			PlayerSettings.companyName = companyName;
			PlayerSettings.productName = appName[selectedIndex];
			PlayerSettings.bundleIdentifier = bundleId;
		    PlayerSettings.bundleVersion = mVersion.ToString();
		    PlayerSettings.Android.bundleVersionCode = mVersionCode;
		    PlayerSettings.Android.keyaliasPass = mPass;
			PlayerSettings.Android.keyaliasName = accounts [selectedIndex];
		    PlayerSettings.Android.keystorePass = mPass;

			BundleVersionChecker();
//			PerformAndroidBuild(appName[selectedIndex]);
		}
    }
	
    static void BundleVersionChecker () {
		float bundleVersion = float.Parse(PlayerSettings.bundleVersion);
		float lastVersion = BundleVersion.Version;
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
        code += System.String.Format ("\tpublic static readonly float Version = {0}f;", bundleVersion);
        code += "\n}\n";
        return code;
    }

    private void ShowHint(string str)
    {
        this.ShowNotification(new GUIContent(str));
    }
}
