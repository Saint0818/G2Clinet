using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public class AvatarEditor :  EditorWindow{

	[MenuItem ("GameEditor/Avatar")]
	private static void BuildTool() {

		EditorWindow.GetWindowWithRect(typeof(AvatarEditor), new Rect(0, 0, 800, 800), true, "Avatar").Show();
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
		scrollPosition = GUI.BeginScrollView (new Rect (0, 240, 600, 50), scrollPosition, new Rect (0, 0, showBody.Count * 60, 50));
		if (showBody.Count > 0) {
			if(GUI.Button(new Rect(0, 0, 50, 30), "None")){
				chooseBodyPart(showBody[0].name, true);
				showBodyTexture.Clear();
			}
			for (int i=0; i<showBody.Count; i++) {
				if(GUI.Button(new Rect(60 * (i+1), 0, 50, 30), showBody[i].name)) {
					chooseBodyPart(showBody[i].name);
				}
			}
		}	
		GUI.EndScrollView ();

		//Body Texture
		GUI.Label (new Rect(0, 280, 500, 50), "Change Body Texture");
		scrollPositionTexture = GUI.BeginScrollView (new Rect (0, 300, 600, 50), scrollPositionTexture, new Rect (0, 0, showBody.Count * 70, 50));
		if (showBodyTexture.Count > 0) {
			for (int i=0; i<showBodyTexture.Count; i++) {
				if(GUI.Button(new Rect(70 * i, 0, 60, 30), showBodyTexture[i].name)) {
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

		//Random
		if(GUI.Button(new Rect(0, 350, 100, 50), "Random")){
			
			attr.Cloth = UnityEngine.Random.Range(5,7);
			attr.Hair = UnityEngine.Random.Range(2,4);
			attr.MHandDress = UnityEngine.Random.Range(2,4);
			attr.Pants = UnityEngine.Random.Range(6,8);
			attr.Shoes = UnityEngine.Random.Range(0,3);
			attr.AHeadDress = UnityEngine.Random.Range(0,3);
			attr.ZBackEquip = UnityEngine.Random.Range(0,3);
			
			//GameController.Get.ChangePlayer(attr, attrTexture);

			judgeBodyTextureName("B", UnityEngine.Random.Range(1,3).ToString());
			int numB = UnityEngine.Random.Range(0,2);
			string BTexName = string.Format("{0}_{1}_{2}_{3}", 2, "B", 0, numB);
			GameController.Get.ChangeTexture(attr, Array.IndexOf(strPart, "B"), 0, numB); 
			attrTexture.BTexture = BTexName;

			judgeBodyTextureName("C", attr.Cloth.ToString());
			if(showBody.Count > 0 && showBodyTexture.Count > 0) {
				int numC = UnityEngine.Random.Range(0,2);
				GameController.Get.ChangeTexture(attr, Array.IndexOf(strPart, "C"), attr.Cloth, numC); 
			}

			judgeBodyTextureName("H", attr.Hair.ToString());
			if(showBody.Count > 0 && showBodyTexture.Count > 0) {
				int numH = UnityEngine.Random.Range(0,2);
				GameController.Get.ChangeTexture(attr, Array.IndexOf(strPart, "H"), attr.Hair, numH); 
			}

			judgeBodyTextureName("M", attr.MHandDress.ToString());
			if(showBody.Count > 0 && showBodyTexture.Count > 0) {
				int numM = UnityEngine.Random.Range(0,2);
				GameController.Get.ChangeTexture(attr, Array.IndexOf(strPart, "M"), attr.MHandDress, numM); 
			}

			judgeBodyTextureName("P", attr.Pants.ToString());
			if(showBody.Count > 0 && showBodyTexture.Count > 0) {
				int numP = UnityEngine.Random.Range(0,2);
				GameController.Get.ChangeTexture(attr, Array.IndexOf(strPart, "P"), attr.Pants, numP); 
			}

			judgeBodyTextureName("S", attr.Pants.ToString());
			if(showBody.Count > 0 && showBodyTexture.Count > 0) {
				int numS = UnityEngine.Random.Range(0,2);
				GameController.Get.ChangeTexture(attr, Array.IndexOf(strPart, "S"), attr.Pants, numS);
			}

			judgeBodyTextureName("A", attr.AHeadDress.ToString());
			if(showBody.Count > 0 && showBodyTexture.Count > 0) {
				int numA = UnityEngine.Random.Range(0,2);
				GameController.Get.ChangeTexture(attr, Array.IndexOf(strPart, "A"), attr.AHeadDress, numA); 
			}

			judgeBodyTextureName("Z", attr.ZBackEquip.ToString());
			if(showBody.Count > 0 && showBodyTexture.Count > 0) {
				int numZ = UnityEngine.Random.Range(0,2);
				GameController.Get.ChangeTexture(attr, Array.IndexOf(strPart, "Z"), attr.ZBackEquip, numZ); 
			}



		}

	}

	void chooseBodyPart(string showBodyName, bool isNone = false){
		bodyPartName = showBodyName;
		string[] name = showBodyName.Split("_"[0]);
		judgeBodyTextureName(name[1], name[2]);
		
		if(!name[1].Equals("A") && !name[1].Equals("Z"))
			attr.Body = int.Parse(name[0]);
		
		if(name[1].Equals("C")){
			if(!isNone) {
				attr.Cloth = int.Parse(name[2]);
				if(showBodyTexture.Count > 0) {
					attrTexture.CTexture = showBodyTexture[0].name;
				}
			} else {
				attr.Cloth = 0;
			}
		} else if(name[1].Equals("H")){
			if(!isNone) {
				attr.Hair = int.Parse(name[2]);
				if(showBodyTexture.Count > 0) {
					attrTexture.HTexture = showBodyTexture[0].name;
				}
			} else {
				attr.Hair = 0;
			}
		} else if(name[1].Equals("M")){
			if(!isNone) {
				attr.MHandDress = int.Parse(name[2]);
				if(showBodyTexture.Count > 0) {
					attrTexture.MTexture = showBodyTexture[0].name;
				}
			} else {
				attr.MHandDress = 0;
			}
		} else if(name[1].Equals("P")){
			if(!isNone) {
				attr.Pants = int.Parse(name[2]);
				if(showBodyTexture.Count > 0) {
					attrTexture.PTexture = showBodyTexture[0].name;
				}
			} else {
				attr.Pants = 0;
			}
		} else if(name[1].Equals("S")){
			if(!isNone) {
				attr.Shoes = int.Parse(name[2]);
				if(showBodyTexture.Count > 0) {
					attrTexture.STexture = showBodyTexture[0].name;
				}
			} else {
				attr.Shoes = 0;
			}
		} else if(name[1].Equals("A")){
			if(!isNone) {
				attr.AHeadDress = int.Parse(name[2]);
				if(showBodyTexture.Count > 0) {
					attrTexture.ATexture = showBodyTexture[0].name;
				}
			} else {
				attr.AHeadDress = 0;
			}
		} else if(name[1].Equals("Z")){
			if(!isNone) {
				attr.ZBackEquip = int.Parse(name[2]);
				if(showBodyTexture.Count > 0) {
					attrTexture.ZTexture = showBodyTexture[0].name;
				}
			} else {
				attr.ZBackEquip = 0;
			}
		}
		
		//GameController.Get.ChangePlayer(attr, attrTexture);
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
