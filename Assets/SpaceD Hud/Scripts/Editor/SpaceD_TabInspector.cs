using UnityEngine;
using UnityEditor;

#if UNITY_3_5
[CustomEditor(typeof(SpaceD_Tab))]
#else
[CustomEditor(typeof(SpaceD_Tab), true)]
#endif
public class SpaceD_TabInspector : Editor
{
	public override void OnInspectorGUI()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(100f);
		
		serializedObject.Update();
		NGUIEditorTools.DrawProperty("Target Sprite", serializedObject, "tabSprite");
		NGUIEditorTools.DrawProperty("Target Label", serializedObject, "tabLabel");
		NGUIEditorTools.DrawProperty("Target Content", serializedObject, "targetContent");
		NGUIEditorTools.DrawProperty("Link with Tab", serializedObject, "linkWith");

		DrawSprites();
		DrawColors();

		serializedObject.ApplyModifiedProperties();
	}

	protected void DrawSprites()
	{
		SpaceD_Tab tab = target as SpaceD_Tab;
		
		if (tab == null)
			return;
		
		if (NGUIEditorTools.DrawHeader("Appearance"))
		{
			NGUIEditorTools.BeginContents();
			
			if (tab.tabSprite != null)
			{
				EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
				{
					SerializedObject obj = new SerializedObject(tab.tabSprite);
					obj.Update();
					SerializedProperty atlas = obj.FindProperty("mAtlas");
					NGUIEditorTools.DrawSpriteField("Normal", obj, atlas, obj.FindProperty("mSpriteName"));
					NGUIEditorTools.DrawSpriteField("Hover", serializedObject, atlas, serializedObject.FindProperty("hoverSprite"), true);
					NGUIEditorTools.DrawSpriteField("Active", serializedObject, atlas, serializedObject.FindProperty("activeSprite"), true);
					obj.ApplyModifiedProperties();
				}
				EditorGUI.EndDisabledGroup();
			}

			NGUIEditorTools.EndContents();
		}
	}

	protected void DrawColors()
	{
		SpaceD_Tab tab = target as SpaceD_Tab;

		if (tab == null)
			return;

		if (NGUIEditorTools.DrawHeader("Label Colors"))
		{
			NGUIEditorTools.BeginContents();

			if (tab.tabLabel != null)
			{
				EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
				{
					SerializedObject obj = new SerializedObject(tab.tabLabel);
					obj.Update();
					NGUIEditorTools.DrawProperty("Normal", obj, "mColor");
					obj.ApplyModifiedProperties();
				}
				EditorGUI.EndDisabledGroup();
			}
			
			NGUIEditorTools.DrawProperty("Hover", serializedObject, "hoverLabelColor");
			NGUIEditorTools.DrawProperty("Active", serializedObject, "activeLabelColor");
			NGUIEditorTools.EndContents();
		}
	}
}
