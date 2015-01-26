using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class Knight49Editor : EditorWindow
{
    [MenuItem ("Knight49/Build Tool")]
    private static void BuildTool()
    {
        Rect wr = new Rect(0, 0, 300, 300);
        Knight49Editor window = (Knight49Editor)EditorWindow.GetWindowWithRect(typeof(Knight49Editor), wr, true, "Knight49Editor");
        window.Show();
    }
    
    public string mVersion = "0.0.0";
    public int mVersionCode = 1;
    public string mPass = "csharp2014";
    public string mPath;
    private int skillTeam = 0;
    private int optinonsIndex = 0;
    public int HeadItemIndex = 51;
    public int ShoesItemIndex = 52;
    public int PlayerIndex = 0;
    public int SkillKind = 0;
    public int Value = 0;
    public int LifeTime = 0;
    private string[] tabOptions = new string[3]{"GameTools", "SkillTools", "Avatar"};
    private string[] skillANos = new string[5];
    private string[] skillAEffects = new string[5];
    private string[] skillBNos = new string[5];
    private string[] skillCNos = new string[5];
    private string AllSkillANo = "0";
    private string AllSkillAEffect = "0";
    private string AllSkillBNo = "0";
    private string AllSkillCNo = "0";

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
		mVersion = EditorGUILayout.TextField("Bundle Version", mVersion);
		mVersionCode = EditorGUILayout.IntField("Bundle mVersionCode", mVersionCode); 

		if (GUILayout.Button("Setting", GUILayout.Width(200)))
		{
		    PlayerSettings.bundleVersion = mVersion;
		    PlayerSettings.Android.bundleVersionCode = mVersionCode;
		    PlayerSettings.Android.keyaliasPass = mPass;
		    PlayerSettings.Android.keystorePass = mPass;

		    BundleVersionChecker();
		}
    }
	
    static void BundleVersionChecker () {
        string bundleVersion = "Ver " + PlayerSettings.bundleVersion;
		string lastVersion = BundleVersion.version;
		if (lastVersion != bundleVersion) {
            UnityEngine.Debug.Log ("Found new bundle version " + bundleVersion + " replacing code from previous version " + lastVersion +" in file \"" + TargetCodeFile + "\"");
            CreateNewBuildVersionClassFile (bundleVersion);
        }

        AssetDatabase.Refresh ();
    }
    
    static string CreateNewBuildVersionClassFile (string bundleVersion) {
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

    static string GenerateCode (string bundleVersion) {
        string code = "public static class " + ClassName + "\n{\n";
        code += System.String.Format ("\tpublic static readonly string version = \"{0}\";", bundleVersion);
        code += "\n}\n";
        return code;
    }

    private void ShowHint(string str)
    {
        this.ShowNotification(new GUIContent(str));
    }
}
