using UnityEngine;
using UnityEditor;
using System.Collections;

public class SpaceD_SpellsEditor
{
	private static string GetSelectionFolder()
	{
		if (Selection.activeObject != null)
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
			
			if (!string.IsNullOrEmpty(path))
			{
				int dot = path.LastIndexOf('.');
				int slash = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
				if (slash > 0) return (dot > slash) ? path.Substring(0, slash + 1) : path + "/";
			}
		}
		return "Assets/";
	}
	
	[MenuItem("SpaceD/Create/SpaceD Spell Database")]
	public static void CreateDatabase()
	{
		// Get the currently selected asset directory
		string currentPath = GetSelectionFolder();
		
		// New asset name
		string assetName = "New Spell Database.asset";
		
		SpaceD_Spells asset = ScriptableObject.CreateInstance("SpaceD_Spells") as SpaceD_Spells;  //scriptable object
		AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(currentPath + assetName));
		AssetDatabase.Refresh();
	}
}