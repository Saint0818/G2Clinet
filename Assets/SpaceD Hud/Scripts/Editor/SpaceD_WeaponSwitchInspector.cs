using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SpaceD_WeaponSwitch))]
public class SpaceD_WeaponSwitchInspector : Editor
{
	protected SpaceD_WeaponSwitch mInstance;
	
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.labelWidth = 100f;
		mInstance = target as SpaceD_WeaponSwitch;

		if (NGUIEditorTools.DrawHeader("Controls"))
		{
			NGUIEditorTools.BeginContents();
			GUILayout.BeginVertical();
			
			UIScrollView scrollView = EditorGUILayout.ObjectField("Viewport", mInstance.scrollView, typeof(UIScrollView), true) as UIScrollView;
			Transform container = EditorGUILayout.ObjectField("Container", mInstance.container, typeof(Transform), true) as Transform;
			
			if (mInstance.scrollView != scrollView || mInstance.container != container)
			{
				mInstance.scrollView = scrollView;
				mInstance.container = container;
			}
			
			GUILayout.EndVertical();
			NGUIEditorTools.EndContents();
		}

		if (NGUIEditorTools.DrawHeader("Animations"))
		{
			NGUIEditorTools.BeginContents();
			GUILayout.BeginVertical();
			EditorGUIUtility.labelWidth = 140f;
			
			float scaleLerp = EditorGUILayout.FloatField("Scale Speed", mInstance.scaleLerp);

			if (mInstance.scaleLerp != scaleLerp)
				mInstance.scaleLerp = scaleLerp;
			
			float activeScale = EditorGUILayout.FloatField("Active Scale", mInstance.activeScale);

			if (mInstance.activeScale != activeScale)
				mInstance.activeScale = activeScale;
			
			float inactiveScale = EditorGUILayout.FloatField("Inactive Scale", mInstance.inactiveScale);

			if (mInstance.inactiveScale != inactiveScale)
				mInstance.inactiveScale = inactiveScale;
			
			GUILayout.Space(10f);
			
			float positionLerp = EditorGUILayout.FloatField("Position Speed", mInstance.positionLerp);

			if (mInstance.positionLerp != positionLerp)
				mInstance.positionLerp = positionLerp;
			
			float activePositionY = EditorGUILayout.FloatField("Active Position Y", mInstance.activePositionY);

			if (mInstance.activePositionY != activePositionY)
				mInstance.activePositionY = activePositionY;
			
			float inactivePositionY = EditorGUILayout.FloatField("Inactive Position Y", mInstance.inactivePositionY);

			if (mInstance.inactivePositionY != inactivePositionY)
				mInstance.inactivePositionY = inactivePositionY;
			
			GUILayout.Space(10f);
			
			float alphaLerp = EditorGUILayout.FloatField("Opacity Speed", mInstance.alphaLerp);

			if (mInstance.alphaLerp != alphaLerp)
				mInstance.alphaLerp = alphaLerp;
			
			float activeAlpha = EditorGUILayout.FloatField("Active Opacity", mInstance.activeAlpha);

			if (mInstance.activeAlpha != activeAlpha)
				mInstance.activeAlpha = activeAlpha;
			
			float inactiveAlpha = EditorGUILayout.FloatField("Inactive Opacity", mInstance.inactiveAlpha);

			if (mInstance.inactiveAlpha != inactiveAlpha)
				mInstance.inactiveAlpha = inactiveAlpha;
			
			GUILayout.EndVertical();
			NGUIEditorTools.EndContents();
		}

		if (NGUIEditorTools.DrawHeader("Auto Hiding"))
		{
			NGUIEditorTools.BeginContents();
			GUILayout.BeginVertical();
			EditorGUIUtility.labelWidth = 140f;
			
			bool autoHideEnable = EditorGUILayout.Toggle("Enable", mInstance.autoHideEnable);

			if (mInstance.autoHideEnable != autoHideEnable)
				mInstance.autoHideEnable = autoHideEnable;
			
			float autoHideAfter = EditorGUILayout.FloatField("Hide After", mInstance.autoHideAfter);

			if (mInstance.autoHideAfter != autoHideAfter)
				mInstance.autoHideAfter = autoHideAfter;
			
			float hideDuration = EditorGUILayout.FloatField("FadeOut Duration", mInstance.hideDuration);

			if (mInstance.hideDuration != hideDuration)
				mInstance.hideDuration = hideDuration;
			
			GUILayout.EndVertical();
			NGUIEditorTools.EndContents();
		}

		if (NGUIEditorTools.DrawHeader("Auto Layout"))
		{
			NGUIEditorTools.BeginContents();
			GUILayout.BeginVertical();
			EditorGUIUtility.labelWidth = 140f;
			
			float cellWidth = EditorGUILayout.FloatField("Cell Width", mInstance.cellWidth);

			if (mInstance.cellWidth != cellWidth)
				mInstance.cellWidth = cellWidth;
			
			GUILayout.EndVertical();
			NGUIEditorTools.EndContents();
		}
	}
}
