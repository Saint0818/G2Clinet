using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SpaceD_SpellSlot))]
public class SpaceD_SpellSlotInspector : SpaceD_IconSlotInspector
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		EditorGUIUtility.labelWidth = 120f;
		SpaceD_SpellSlot mSlot = target as SpaceD_SpellSlot;
		
		EditorGUILayout.Space();
		
		SpaceD_Cooldown cooldown = EditorGUILayout.ObjectField("Cooldown Handle", mSlot.cooldownHandle, typeof(SpaceD_Cooldown), true) as SpaceD_Cooldown;
		
		if (mSlot.cooldownHandle != cooldown)
			mSlot.cooldownHandle = cooldown;
		
		DrawEvents();
	}
	
	public void DrawEvents()
	{
		SpaceD_SpellSlot mSlot = target as SpaceD_SpellSlot;
		
		EditorGUIUtility.labelWidth = 100f;
		
		NGUIEditorTools.DrawEvents("On Assign", mSlot, mSlot.onAssign);
		NGUIEditorTools.DrawEvents("On Unassign", mSlot, mSlot.onUnassign);
	}
}