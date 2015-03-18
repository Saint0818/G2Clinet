using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public class AvatarEditor :  EditorWindow{

	[MenuItem ("Avatar/AvatarSystem")]
	private static void BuildTool() {

		EditorWindow.GetWindowWithRect(typeof(AvatarEditor), new Rect(0, 0, 600, 600), true, "AvatarEditor").Show();
	}

	private enum Tag{
		B, C, H, M, P, S, A, Z
	}

	private int model = 2;
	private string[] strPart = new string[]{"B", "C", "H", "M", "P", "S", "A", "Z"};
	private int bodyPart = 0;
	private string bodyPartName = "";
	private string bodyTex = "";

	public Vector2 scrollPosition = Vector2.zero;
	
	public List<UnityEngine.GameObject> allBody;
	public List<UnityEngine.Material> allMaterial;
	public List<UnityEngine.Texture2D> allTextures;

	private int bodyModelCount = 0;
	private int bodyTexturesCount = 0;
	

//	public int Body = 2;
//	public int Hair = 2;
//	public int AHead = 1;
//	public int Cloth = 5;
//	public int Pants = 6;
//	public int Shoes = 1;
//	public int MHandDress = 2;
//	public int ZBackEquip = 1;
//
//	public int BTexture = 0;
//	public int HTexture = 0;
//	public int ATexture = 0;
//	public int CTexture = 0;
//	public int PTexture = 0;
//	public int STexture = 0;
//	public int MTexture = 0;
//	public int ZTexture = 0;



	void init (){
		allBody = new List<UnityEngine.GameObject> ();
		allMaterial = new List<UnityEngine.Material> ();
		allTextures = new List<UnityEngine.Texture2D> ();

		allBody.Clear ();
		allMaterial.Clear ();
		allTextures.Clear ();

		UnityEngine.Object[] obj = Resources.LoadAll ("Character/PlayerModel_2");
//		Debug.Log ("obj length:"+ obj.Length);
		for (int i=0; i<obj.Length; i++) {
			Debug.Log ("name:"+obj[i].name);
			Debug.Log ("debug:"+obj[i].GetType());
			if(obj[i].GetType().ToString().Equals(typeof(UnityEngine.GameObject).ToString())){
				allBody.Add((UnityEngine.GameObject)obj[i]);
			}
		}
		UnityEngine.Object[] obj_3 = Resources.LoadAll ("Character/PlayerModel_3");
		for (int i=0; i<obj_3.Length; i++) {
			if(obj_3[i].GetType().Equals(typeof(UnityEngine.GameObject))){
				allBody.Add((UnityEngine.GameObject)obj_3[i]);
			}
		}
		UnityEngine.Object[] mat = Resources.LoadAll ("Character/PlayerModel_2/Materials"); 
		for (int i=0; i<mat.Length; i++) {
			if(mat[i].GetType().Equals(typeof(UnityEngine.Material))){
				allMaterial.Add((UnityEngine.Material)mat[i]);
			}
		}
		UnityEngine.Object[] mat_3 = Resources.LoadAll ("Character/PlayerModel_3/Materials"); 
			for (int i=0; i<mat_3.Length; i++) {
			if(mat_3[i].GetType().Equals(typeof(UnityEngine.Material))){
				allMaterial.Add((UnityEngine.Material)mat_3[i]);
			}
		}
		UnityEngine.Object[] tex = Resources.LoadAll ("Character/PlayerModel_2/Texture"); 
		for (int i=0; i<tex.Length; i++) {
			if(tex[i].GetType().Equals(typeof(UnityEngine.Texture2D))){
				allTextures.Add((UnityEngine.Texture2D)tex[i]);
			}
		}
		UnityEngine.Object[] tex_3= Resources.LoadAll ("Character/PlayerModel_3/Texture"); 
		for (int i=0; i<tex_3.Length; i++) {
			if(tex_3[i].GetType().Equals(typeof(UnityEngine.Texture2D))){
				allTextures.Add((UnityEngine.Texture2D)tex_3[i]);
			}
		}
	}

	void reSet(){
		init ();
		model = 2;
		bodyPart = 0;
		bodyPartName = "";
		bodyTex = "";
	}

	void OnGUI() {
//		if (GUI.Button (new Rect (500, 0, 100, 30), "Reset")) {
//			reSet();
//		}

		EditorGUILayout.LabelField ("Model:" + model);
		EditorGUILayout.LabelField ("Body:" + strPart[bodyPart]);
		EditorGUILayout.LabelField ("Body Part:" + bodyPartName);
		EditorGUILayout.LabelField ("Body Texture:" + bodyTex);
		//Model
		GUI.Label (new Rect(0, 80, 500, 50), "Model");
		if (GUI.Button (new Rect(0, 100, 200, 50), "PlayerMode_0")) {
			if(allBody == null || allBody.Count == 0) init();
//			model = 0;
		}
		if (GUI.Button (new Rect(200, 100, 200, 50), "PlayerMode_1")) {
			if(allBody== null || allBody.Count == 0) init();
//			model = 1;
		}
		if (GUI.Button (new Rect(400, 100, 200, 50), "PlayerMode_2")) {
			if(allBody== null || allBody.Count == 0) init();
			model = 2;
		}
		//Body
		GUI.Label (new Rect(0, 150, 500, 50), "Body");
		if (GUI.Button (new Rect(0, 170, 70, 50), "B")) {
			judgeName();
		}
		if (GUI.Button (new Rect(75, 170, 70, 50), "C")) {
		}
		if (GUI.Button (new Rect(150, 170, 70, 50), "H")) {
		}
		if (GUI.Button (new Rect(225, 170, 70, 50), "M")) {
		}
		if (GUI.Button (new Rect(300, 170, 70, 50), "P")) {
		}
		if (GUI.Button (new Rect(375, 170, 70, 50), "S")) {
		}
		if (GUI.Button (new Rect(450, 170, 70, 50), "A")) {
		}
		if (GUI.Button (new Rect(525, 170, 70, 50), "Z")) {
		}
		//Body Part
		GUI.Label (new Rect(0, 220, 500, 50), "Body Part");
		scrollPosition = GUI.BeginScrollView (new Rect (0, 240, 600, 50), scrollPosition, new Rect (0, 0, 1000, 50));
		if (bodyModelCount > 0) {

		}	
		GUI.EndScrollView ();

		//Body Texture
		GUI.Label (new Rect(0, 300, 500, 50), "Body Texture");


	}

	void chooseBodyPart(Tag tag){
		bodyTexturesCount = 0;
		switch(tag){
		case Tag.B:

			break;
		case Tag.C:

			break;
		case Tag.H:

			break;
		case Tag.M:

			break;
		case Tag.P:

			break;
		case Tag.S:

			break;
		case Tag.A:

			break;
		case Tag.Z:

			break;
		}
	}
	void judgeName(){
		Debug.Log("judgeName:"+allBody.Count);
		for (int i=0; i<allBody.Count; i++) {
			Debug.Log("!!!!!!:"+allBody[i].name);
			string[] name = allBody[i].name.Split("_"[0]);
			for(int j=0; j<name.Length; j++) {
				Debug.Log("name:"+name[j]);
			}
			Debug.Log("===============");
		}
	}





//			Body = EditorGUILayout.IntField ("Body ", Body);
//			Hair = EditorGUILayout.IntField ("Hair ", Hair);
//			AHead = EditorGUILayout.IntField ("Hair Dress", AHead);
//			Cloth = EditorGUILayout.IntField ("Cloth", Cloth);
//			Pants = EditorGUILayout.IntField ("Pants", Pants);
//			Shoes = EditorGUILayout.IntField ("Shoes", Shoes);
//			MHandDress = EditorGUILayout.IntField ("Hand Dress", MHandDress);
//			ZBackEquip = EditorGUILayout.IntField ("Back Equip", ZBackEquip);
//	
//			BTexture = EditorGUILayout.IntField ("Body Texture ", BTexture);
//			HTexture = EditorGUILayout.IntField ("Hair Texture ", HTexture);
//			ATexture = EditorGUILayout.IntField ("Hair Dress Texture ", ATexture);
//			CTexture = EditorGUILayout.IntField ("Cloth Texture ", CTexture);
//			PTexture = EditorGUILayout.IntField ("Pants Texture ", PTexture);
//			STexture = EditorGUILayout.IntField ("Shoes Texture ", STexture);
//			MTexture = EditorGUILayout.IntField ("Hand Dress Texture ", MTexture);
//			ZTexture = EditorGUILayout.IntField ("Back Texture ", ZTexture);
//	
//			if (GUILayout.Button("Change Avatar", GUILayout.Width(200))) {
//				GameStruct.TPlayerAttribute attr = new GameStruct.TPlayerAttribute();
//				attr.Body = Body;
//				attr.Hair = Hair;
//				attr.AHead = AHead;
//				attr.Cloth = Cloth;
//				attr.Pants = Pants;
//				attr.Shoes = Shoes;
//				attr.MHeadDress = MHandDress;
//				attr.ZBackEquip = ZBackEquip;
//	
//				attr.BTexture = BTexture;
//				attr.HTexture = HTexture;
//				attr.ATexture = ATexture;
//				attr.CTexture = CTexture;
//				attr.PTexture = PTexture;
//				attr.STexture = STexture;
//				attr.MTexture = MTexture;
//				attr.ZTexture = ZTexture;
//				GameController.Get.ChangePlayer(attr);
//			}
//			if (GUILayout.Button("Change Texture", GUILayout.Width(200))) {
//				GameStruct.TPlayerAttribute attr = new GameStruct.TPlayerAttribute();
//				attr.Body = Body;
//				attr.Hair = Hair;
//				attr.AHead = AHead;
//				attr.Cloth = Cloth;
//				attr.Pants = Pants;
//				attr.Shoes = Shoes;
//				attr.MHeadDress = MHandDress;
//				attr.ZBackEquip = ZBackEquip;
//				
//				attr.BTexture = BTexture;
//				attr.HTexture = HTexture;
//				attr.ATexture = ATexture;
//				attr.CTexture = CTexture;
//				attr.PTexture = PTexture;
//				attr.STexture = STexture;
//				attr.MTexture = MTexture;
//				attr.ZTexture = ZTexture;
//				GameController.Get.ChangeTexture(attr, 6, 1, 1);
//			}
}
