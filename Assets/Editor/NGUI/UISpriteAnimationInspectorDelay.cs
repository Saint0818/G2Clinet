//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

/// <summary>
/// Inspector class used to edit UISpriteAnimations.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UISpriteAnimationDelay))]
public class UISpriteAnimationInspectorDelay : Editor
{
	/// <summary>
	/// Draw the inspector widget.
	/// </summary>
	
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(3f);
		NGUIEditorTools.SetLabelWidth(80f);
		serializedObject.Update();
		
		NGUIEditorTools.DrawProperty("Framerate", serializedObject, "mFPS");
		NGUIEditorTools.DrawProperty("Name Prefix", serializedObject, "mPrefix");
		NGUIEditorTools.DrawProperty("Delay Time", serializedObject, "mDelayTime");
		NGUIEditorTools.DrawProperty("Loop", serializedObject, "mLoop");
		NGUIEditorTools.DrawProperty("Pixel Snap", serializedObject, "mSnap");
		
		serializedObject.ApplyModifiedProperties();
	}
}
