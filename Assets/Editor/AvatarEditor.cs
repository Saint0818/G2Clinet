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

	private int model = 2;
	private string[] strPart = new string[]{"B", "C", "H", "M", "P", "S", "A", "Z"};
	private int bodyPart = 0;
	private string bodyPartName = "";
	private string bodyTex = "";
	
	private GameStruct.TAvatar attr = new GameStruct.TAvatar(1);
	private GameStruct.TAvatarTexture attrTexture = new GameStruct.TAvatarTexture (1);

	public Vector2 scrollPosition = Vector2.zero;
	public Vector2 scrollPositionTexture = Vector2.zero;

	public List<UnityEngine.GameObject> allBody;
	public List<UnityEngine.Material> allMaterial;
	public List<UnityEngine.Texture> allTextures;

	public List<UnityEngine.GameObject> showBody = new List<GameObject> ();
	public List<UnityEngine.Texture> showBodyTexture = new List<Texture> ();

//	private int bodyModelCount = 0;
//	private int bodyTexturesCount = 0;
	

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
		allTextures = new List<UnityEngine.Texture> ();

		allBody.Clear ();
		allMaterial.Clear ();
		allTextures.Clear ();

		UnityEngine.Object[] obj = Resources.LoadAll ("Character/PlayerModel_2/Model", typeof(GameObject));
		for (int i=0; i<obj.Length; i++) {
			if(!obj[i].name.Contains("PlayerModel")){
				allBody.Add((UnityEngine.GameObject)obj[i]);
			}
		}
		UnityEngine.Object[] obj_3 = Resources.LoadAll ("Character/PlayerModel_3/Model", typeof(GameObject));
		for (int i=0; i<obj_3.Length; i++) {
			if(!obj_3[i].name.Contains("PlayerModel")){
				allBody.Add((UnityEngine.GameObject)obj_3[i]);
			}
		}
		UnityEngine.Object[] mat = Resources.LoadAll ("Character/Materials"); 
		for (int i=0; i<mat.Length; i++) {
			if(mat[i].GetType().Equals(typeof(UnityEngine.Material))){
				allMaterial.Add((UnityEngine.Material)mat[i]);
			}
		}
		UnityEngine.Object[] tex = Resources.LoadAll ("Character/PlayerModel_2/Texture"); 
		for (int i=0; i<tex.Length; i++) {
				allTextures.Add((UnityEngine.Texture)tex[i]);
		}
		UnityEngine.Object[] tex_3= Resources.LoadAll ("Character/PlayerModel_3/Texture"); 
		for (int i=0; i<tex_3.Length; i++) {
			allTextures.Add((UnityEngine.Texture)tex_3[i]);
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
		GUI.Label (new Rect(0, 80, 500, 50), "Choose Model");
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
		GUI.Label (new Rect(0, 150, 500, 50), "Choose Body");
		if (GUI.Button (new Rect(0, 170, 70, 50), "B")) {
			bodyPart = 0;
			bodyPartName = "";
			bodyTex = "";
			showBody.Clear ();
			judgeBodyTextureName("B","0");
		}
		if (GUI.Button (new Rect(75, 170, 70, 50), "C")) {
			bodyPart = 1;
			judgeBodyName("C");
		}
		if (GUI.Button (new Rect(150, 170, 70, 50), "H")) {
			bodyPart = 2;
			judgeBodyName("H");
		}
		if (GUI.Button (new Rect(225, 170, 70, 50), "M")) {
			bodyPart = 3;
			judgeBodyName("M");
		}
		if (GUI.Button (new Rect(300, 170, 70, 50), "P")) {
			bodyPart = 4;
			judgeBodyName("P");
		}
		if (GUI.Button (new Rect(375, 170, 70, 50), "S")) {
			bodyPart = 5;
			judgeBodyName("S");
		}
		if (GUI.Button (new Rect(450, 170, 70, 50), "A")) {
			bodyPart = 6;
			judgeBodyName("A");
		}
		if (GUI.Button (new Rect(525, 170, 70, 50), "Z")) {
			bodyPart = 7;
			judgeBodyName("Z");
		}
		//Body Part
		GUI.Label (new Rect(0, 220, 500, 50), "Change Body Part");
		scrollPosition = GUI.BeginScrollView (new Rect (0, 240, 600, 50), scrollPosition, new Rect (0, 0, showBody.Count * 100, 50));
		if (showBody.Count > 0) {
			for (int i=0; i<showBody.Count; i++) {
				if(GUI.Button(new Rect(120 * i, 0, 100, 50), showBody[i].name)) {
					bodyPartName = showBody[i].name;
					string[] name = showBody[i].name.Split("_"[0]);
					judgeBodyTextureName(name[1], name[2]);

					if(!name[1].Equals("A") && !name[1].Equals("Z"))
						attr.Body = int.Parse(name[0]);

					if(name[1].Equals("C")){
						attr.Cloth = int.Parse(name[2]);
						if(showBodyTexture.Count > 0) {
							attrTexture.CTexture = showBodyTexture[0].name;
						}
					} else if(name[1].Equals("H")){
						attr.Hair = int.Parse(name[2]);
						if(showBodyTexture.Count > 0) {
							attrTexture.HTexture = showBodyTexture[0].name;
						}
					} else if(name[1].Equals("M")){
						attr.MHandDress = int.Parse(name[2]);
						if(showBodyTexture.Count > 0) {
							attrTexture.MTexture = showBodyTexture[0].name;
						}
					} else if(name[1].Equals("P")){
						attr.Pants = int.Parse(name[2]);
						if(showBodyTexture.Count > 0) {
							attrTexture.PTexture = showBodyTexture[0].name;
						}
					} else if(name[1].Equals("S")){
						attr.Shoes = int.Parse(name[2]);
						if(showBodyTexture.Count > 0) {
							attrTexture.STexture = showBodyTexture[0].name;
						}
					} else if(name[1].Equals("A")){
						attr.AHeadDress = int.Parse(name[2]);
						if(showBodyTexture.Count > 0) {
							attrTexture.ATexture = showBodyTexture[0].name;
						}
					} else if(name[1].Equals("Z")){
						attr.ZBackEquip = int.Parse(name[2]);
						if(showBodyTexture.Count > 0) {
							attrTexture.ZTexture = showBodyTexture[0].name;
						}
					}

					GameController.Get.ChangePlayer(attr, attrTexture);
				}
			}
		}	
		GUI.EndScrollView ();

		//Body Texture
		GUI.Label (new Rect(0, 300, 500, 50), "Change Body Texture");
		scrollPositionTexture = GUI.BeginScrollView (new Rect (0, 320, 600, 50), scrollPositionTexture, new Rect (0, 0, showBody.Count * 100, 50));
		if (showBodyTexture.Count > 0) {
			for (int i=0; i<showBodyTexture.Count; i++) {
				if(GUI.Button(new Rect(120 * i, 0, 100, 50), showBodyTexture[i].name)) {
					bodyTex = showBodyTexture[i].name;
					string[] name = showBodyTexture[i].name.Split("_"[0]);
					if(name[1].Equals("B")){
						attrTexture.BTexture = showBodyTexture[i].name;
					} else if(name[1].Equals("C")){
						attrTexture.CTexture = showBodyTexture[i].name;
					} else if(name[1].Equals("H")){
						attrTexture.HTexture = showBodyTexture[i].name;
					} else if(name[1].Equals("M")){
						attrTexture.MTexture = showBodyTexture[i].name;
					} else if(name[1].Equals("P")){
						attrTexture.PTexture = showBodyTexture[i].name;
					} else if(name[1].Equals("S")){
						attrTexture.STexture = showBodyTexture[i].name;
					} else if(name[1].Equals("A")){
						attrTexture.ATexture = showBodyTexture[i].name;
					} else if(name[1].Equals("Z")){
						attrTexture.ZTexture = showBodyTexture[i].name;
					}
					bodyPart = Array.IndexOf(strPart, name[1]);
					GameController.Get.ChangeTexture(attr, bodyPart, int.Parse(name[2]), int.Parse(name[3])); 
				}
			}
		}	
		GUI.EndScrollView ();

	}
	void judgeBodyName(string body){
		showBody.Clear ();
		showBodyTexture.Clear ();
		bodyPartName = "";
		bodyTex = "";
		for (int i=0; i<allBody.Count; i++) {
			string[] name = allBody[i].name.Split("_"[0]);
			if(name[1].Equals(body)) {
				showBody.Add(allBody[i]);
			}
		}
	}
	void judgeBodyTextureName(string body, string _bodyPart){
		showBodyTexture.Clear ();
		bodyTex = "";
		for (int i=0; i<allTextures.Count; i++) {
			string[] name = allTextures[i].name.Split("_"[0]);
			if(name[1].Equals(body)) {
				if(name[2].Equals(_bodyPart)) {
					showBodyTexture.Add(allTextures[i]);
				}
			}
		}
	}

}
