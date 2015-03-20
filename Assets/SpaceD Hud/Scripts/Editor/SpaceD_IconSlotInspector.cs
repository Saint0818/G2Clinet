using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SpaceD_IconSlot))]
public class SpaceD_IconSlotInspector : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.labelWidth = 120f;
		EditorGUILayout.Space();
		
		DrawTargetSprite();
		DrawDragAndDrop();
		DrawBehaviour();
	}
	
	public void DrawTargetSprite()
	{
		this.serializedObject.Update();
		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("iconSprite"), new GUIContent("Icon Sprite"));
		this.serializedObject.ApplyModifiedProperties();
	}
	
	public void DrawDragAndDrop()
	{
		this.serializedObject.Update();
		
		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("dragAndDropEnabled"), new GUIContent("Drag and Drop"));
		
		// When drag and drop is enabled
		if (this.serializedObject.FindProperty("dragAndDropEnabled").boolValue)
		{
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("AllowThrowAway"), new GUIContent("Allow throw away"));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("IsStatic"), new GUIContent("Is Static"));
			
			if (this.serializedObject.FindProperty("IsStatic").boolValue)
				EditorGUILayout.HelpBox("Static slots are intended to be used for spell books and such because they will not be unassigned when drag is strated.", MessageType.Warning);
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	public void DrawBehaviour()
	{
		this.serializedObject.Update();
		
		if (NGUIEditorTools.DrawHeader("Behaviour"))
		{
			NGUIEditorTools.BeginContents();
			EditorGUIUtility.labelWidth = 150f;
			GUILayout.BeginVertical();
			
			// Hover Effect
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("hoverEffectType"), new GUIContent("Hover Effect Type"));
			
			SpaceD_IconSlot.HoverEffectType hoverEffectType = (SpaceD_IconSlot.HoverEffectType)this.serializedObject.FindProperty("hoverEffectType").enumValueIndex;
			
			if (hoverEffectType == SpaceD_IconSlot.HoverEffectType.Sprite)
			{
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("hoverEffectSprite"), new GUIContent("Hover Sprite"));
			}
			else if (hoverEffectType == SpaceD_IconSlot.HoverEffectType.Color)
			{
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("hoverEffectColor"), new GUIContent("Hover Color"));
			}
			
			// Hover effect speed value
			if (hoverEffectType != SpaceD_IconSlot.HoverEffectType.None)
			{
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("hoverEffectSpeed"), new GUIContent("Hover Tween Duration"));
			}
			
			GUILayout.Space(10f);
			GUILayout.EndVertical();
			
			// Press Effect
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pressEffectType"), new GUIContent("Press Effect Type"));
			
			SpaceD_IconSlot.PressEffectType pressEffectType = (SpaceD_IconSlot.PressEffectType)this.serializedObject.FindProperty("pressEffectType").enumValueIndex;EditorGUILayout.PropertyField(this.serializedObject.FindProperty("hoverEffectType"), new GUIContent("Hover Effect Type"));
			
			if (pressEffectType == SpaceD_IconSlot.PressEffectType.Sprite)
			{
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pressEffectSprite"), new GUIContent("Press Sprite"));
			}
			else if (pressEffectType == SpaceD_IconSlot.PressEffectType.Color)
			{
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pressEffectColor"), new GUIContent("Press Color"));
			}
			
			// Press effect speed value
			if (pressEffectType != SpaceD_IconSlot.PressEffectType.None)
			{
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pressEffectSpeed"), new GUIContent("Press Tween Duration"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pressEffectInstaOut"), new GUIContent("Press Tween Insta Out"));
			}
			
			NGUIEditorTools.EndContents();
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
}
