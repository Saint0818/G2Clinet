using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class AvatarEditor :  EditorWindow{

	[MenuItem ("Avatar/AvatarSystem")]
	private static void BuildTool() {
		Rect wr = new Rect(0, 0, 500, 500);
		AvatarEditor window = (AvatarEditor)EditorWindow.GetWindowWithRect(typeof(AvatarEditor), wr, true, "AvatarEditor");
		window.Show();
	}
		
	public int Body = 2;
	public int Hair = 2;
	public int AHead = 1;
	public int Cloth = 5;
	public int Pants = 6;
	public int Shoes = 1;
	public int MHandDress = 2;
	public int ZBackEquip = 1;

	void OnGUI() {
		Body = EditorGUILayout.IntField ("Body ", Body);
		Hair = EditorGUILayout.IntField ("Hair ", Hair);
		AHead = EditorGUILayout.IntField ("Hair Dress", AHead);
		Cloth = EditorGUILayout.IntField ("Cloth", Cloth);
		Pants = EditorGUILayout.IntField ("Pants", Pants);
		Shoes = EditorGUILayout.IntField ("Shoes", Shoes);
		MHandDress = EditorGUILayout.IntField ("Hand Dress", MHandDress);
		ZBackEquip = EditorGUILayout.IntField ("Back Equip", ZBackEquip);

		if (GUILayout.Button("Change Avatar", GUILayout.Width(200))) {
			GameStruct.TPlayerAttribute attr = new GameStruct.TPlayerAttribute();
			attr.Body = Body;
			attr.Hair = Hair;
			attr.AHead = AHead;
			attr.Cloth = Cloth;
			attr.Pants = Pants;
			attr.Shoes = Shoes;
			attr.MHeadDress = MHandDress;
			attr.ZBackEquip = ZBackEquip;
			GameController.Get.ChangePlayer(attr);
		}
	}


	
}
