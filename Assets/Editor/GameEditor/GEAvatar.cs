using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public class GEAvatar : GEBase {
	private enum Flag {
		B,C,H,M,P,S,A,Z,NONE
	}

	private string[] strPart = new string[]{"B", "C", "H", "M", "P", "S", "A", "Z"};
	private int bodyPart = 0;
	private GameObject selectGameObject = null;
	
	private GameStruct.TAvatar attr = new GameStruct.TAvatar(1);

	public Vector2 scrollPosition = Vector2.zero;
	public Vector2 scrollPositionTexture = Vector2.zero;
	public Vector2 scrollPositionAnimation = Vector2.zero;

	public List<UnityEngine.GameObject> allBody = new List<UnityEngine.GameObject> ();
	public List<UnityEngine.Material> allMaterial = new List<UnityEngine.Material> ();
	public List<UnityEngine.Texture> allTextures = new List<UnityEngine.Texture> ();

	public List<UnityEngine.GameObject> showBody = new List<GameObject> ();
	public List<UnityEngine.Texture> showBodyTexture = new List<Texture> ();

	public bool isAvatar = false;
	public int chooseCount = 0;

	public int modelId = 0;
	public bool isModel0Choose = false;
	public bool isModel1Choose = false;
	public bool isModel2Choose = false;

	public bool isBChoose = false;
	public bool isCChoose = false;
	public bool isHChoose = false;
	public bool isMChoose = false;
	public bool isPChoose = false;
	public bool isSChoose = false;
	public bool isAChoose = false;
	public bool isZChoose = false;

	public string bodyPartText = "";
	public string bodyTextureText = "";
	private AvatarAnimationTest avatarAnimationTest;

	public UnityEditor.Animations.AnimatorController controller;
	public List<AnimationClip> allMotionAnimationClip = new List<AnimationClip>();
		
	void init (){
		if(allBody.Count == 0) {
			UnityEngine.Object[] obj_0 = Resources.LoadAll ("Character/PlayerModel_0/Model", typeof(GameObject));
			for (int i=0; i<obj_0.Length; i++) {
				if(!obj_0[i].name.Contains("PlayerModel")){
					allBody.Add((UnityEngine.GameObject)obj_0[i]);
				}
			}
			UnityEngine.Object[] obj_1 = Resources.LoadAll ("Character/PlayerModel_1/Model", typeof(GameObject));
			for (int i=0; i<obj_1.Length; i++) {
				if(!obj_1[i].name.Contains("PlayerModel")){
					allBody.Add((UnityEngine.GameObject)obj_1[i]);
				}
			}
			UnityEngine.Object[] obj_2 = Resources.LoadAll ("Character/PlayerModel_2/Model", typeof(GameObject));
			for (int i=0; i<obj_2.Length; i++) {
				if(!obj_2[i].name.Contains("PlayerModel")){
					allBody.Add((UnityEngine.GameObject)obj_2[i]);
				}
			}
			UnityEngine.Object[] obj_3 = Resources.LoadAll ("Character/PlayerModel_3/Model", typeof(GameObject));
			for (int i=0; i<obj_3.Length; i++) {
				if(!obj_3[i].name.Contains("PlayerModel")){
					allBody.Add((UnityEngine.GameObject)obj_3[i]);
				}
			}
		}
		if(allMaterial.Count == 0) {
			UnityEngine.Object[] mat = Resources.LoadAll ("Character/Materials"); 
			for (int i=0; i<mat.Length; i++) {
				if(mat[i].GetType().Equals(typeof(UnityEngine.Material))){
					allMaterial.Add((UnityEngine.Material)mat[i]);
				}
			}
		}
		if(allTextures.Count == 0) {
			UnityEngine.Object[] tex_0 = Resources.LoadAll ("Character/PlayerModel_0/Texture"); 
			for (int i=0; i<tex_0.Length; i++) {
				allTextures.Add((UnityEngine.Texture)tex_0[i]);
			}
			UnityEngine.Object[] tex_1 = Resources.LoadAll ("Character/PlayerModel_1/Texture"); 
			for (int i=0; i<tex_1.Length; i++) {
				allTextures.Add((UnityEngine.Texture)tex_1[i]);
			}
			UnityEngine.Object[] tex_2 = Resources.LoadAll ("Character/PlayerModel_2/Texture"); 
			for (int i=0; i<tex_2.Length; i++) {
				allTextures.Add((UnityEngine.Texture)tex_2[i]);
			}
			UnityEngine.Object[] tex_3= Resources.LoadAll ("Character/PlayerModel_3/Texture"); 
			for (int i=0; i<tex_3.Length; i++) {
				allTextures.Add((UnityEngine.Texture)tex_3[i]);
			}
		}
		isModel0Choose = false;
		isModel1Choose = false;
		isModel2Choose = false;
		judgeBody(Flag.NONE);
		showBody.Clear();
		showBodyTexture.Clear();
		bodyPartText = "";
		bodyTextureText = "";
	}
	
	void OnEnable(){
		init ();
		if(Selection.gameObjects.Length > 1) {
			chooseCount = 2;
			isAvatar = false;
		}else 
		if(Selection.gameObjects.Length == 0){
			chooseCount = 0;
			isAvatar = false;
		} else {
			chooseCount = 1;
			selectGameObject = Selection.gameObjects[0];
			Transform t = selectGameObject.transform.FindChild("DummyBall");
			if(t == null) 
				isAvatar = false;
			else {
				isAvatar = true;
				int count = selectGameObject.transform.childCount;
				for(int i=0; i<count; i++) {
					Transform childt = selectGameObject.transform.GetChild(i);
					string name = childt.name;
					if(name.Contains("PlayerModel")){
						Material[] materials = childt.GetComponent<SkinnedMeshRenderer>().materials;
						for(int j=0; j<materials.Length; j++) {
							string[] textureName = materials[j].mainTexture.name.Split("_"[0]);
							if(textureName[1].Equals("B")) {
								attr.Body = int.Parse(textureName[0]+"00"+textureName[3]);;
							} else 
							if(textureName[1].Equals("C")){
								attr.Cloth = int.Parse(textureName[2]+"00"+textureName[3]);
							} else 
							if(textureName[1].Equals("H")){
								attr.Hair = int.Parse(textureName[2]+"00"+textureName[3]);
							} else 
							if(textureName[1].Equals("M")){
								attr.MHandDress = int.Parse(textureName[2]+"00"+textureName[3]);
							} else 
							if(textureName[1].Equals("P")){
								attr.Pants = int.Parse(textureName[2]+"00"+textureName[3]);
							} else 
							if(textureName[1].Equals("S")){
								attr.Shoes = int.Parse(textureName[2]+"00"+textureName[3]);
							}
						}
					} else 
					if(name.Contains("Bip01")){
						int dummyCount_A = childt.FindChild("Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead").childCount;
						if(dummyCount_A == 0)
							attr.AHeadDress = 0;
						else {
							Material[] materials = childt.FindChild("Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead").GetChild(0).GetComponent<MeshRenderer>().materials;
							for(int j=0; j<materials.Length; j++) {
								if(materials[j].mainTexture != null){
									string[] textureName = materials[j].mainTexture.name.Split("_"[0]);
									if(textureName[1].Equals("A")) {
										attr.AHeadDress = int.Parse(textureName[2]+"00"+textureName[3]);
									}
								}
							}
						}

						int dummyCount_Z = childt.FindChild("Bip01 Spine/Bip01 Spine1/DummyBack").childCount;
						if(dummyCount_Z == 0)
							attr.ZBackEquip = 0;
						else {
							Material[] materials = childt.FindChild("Bip01 Spine/Bip01 Spine1/DummyBack").GetChild(0).GetComponent<MeshRenderer>().materials;
							for(int j=0; j<materials.Length; j++) {
								if(materials[j].mainTexture != null){
									string[] textureName = materials[j].mainTexture.name.Split("_"[0]);
									if(textureName[1].Equals("Z")) {
										attr.ZBackEquip = int.Parse(textureName[2]+"00"+textureName[3]);
									}
								}
							}
						}
					}
				}
				if(selectGameObject.name.Equals("0")) {
					modelId = 0;
					isModel0Choose = true;
				} else 
				if(selectGameObject.name.Equals("1")) {
					modelId = 1;
					isModel1Choose = true;
				} else 
				if(selectGameObject.name.Equals("2")) {
					modelId = 2;
					isModel2Choose = true;
				}
				getAllData();
			}
		}
	}

	void createPlayer(string name){
		if(Application.isPlaying) {
			if(GameObject.Find(name) == null) {
				GameObject obj = new GameObject();
				obj.name = name;
				obj.AddComponent<DragRotateObject>();
				ModelManager.Get.SetAvatar(ref obj, attr, int.Parse(name), EAnimatorType.AvatarControl, false);
				obj.AddComponent<AvatarAnimationTest>();
				if(name.Equals("0")) {
					obj.transform.position = new Vector3(2,0,0);
				} else 
				if(name.Equals("1")) {
					obj.transform.position = Vector3.zero;
				} else 
				if(name.Equals("2")) {
					obj.transform.position = new Vector3(-2,0,0);
				}
			}
		}
	}

	void OnGUI() {
		if(chooseCount == 0)
			GUILayout.Label("Need to choose one GameObject!");
		else if(chooseCount == 1)
			GUILayout.Label("Yes. One GameObject!");
		else
			GUILayout.Label("Too much GameObject!");
		if(isAvatar) 
			GUILayout.Label("it is a Avatar!");
		else
			GUILayout.Label("it is not a Avatar!");
		if(GUILayout.Button("Random Player")) {
			if(isAvatar && Application.isPlaying) 
				randomPlayer();
		}
		if(GUILayout.Button("Save(Not Open)")) {

		}

		//Model
		GUI.Label (new Rect(0, 80, 500, 50), "Model");
		if(isModel0Choose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(0, 100, 200, 50), "Center")) {
			if(!isModel0Choose){
				//Create New Model 0
				attr.Body = 0001;
				attr.Hair = 0;
				attr.Cloth = 0;
				attr.Pants = 0;
				attr.Shoes = 0;
				attr.MHandDress = 0;
				attr.AHeadDress = 0;
				attr.ZBackEquip = 0;
				createPlayer("0");
			}
		}
			
		if(isModel1Choose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(200, 100, 200, 50), "Forwards")) {
			if(!isModel1Choose) {
				//Create New Model 1
				attr.Body = 1001;
				attr.Hair = 0;
				attr.Cloth = 0;
				attr.Pants = 0;
				attr.Shoes = 0;
				attr.MHandDress = 0;
				attr.AHeadDress = 0;
				attr.ZBackEquip = 0;
				createPlayer("1");
			}
		}
			
		if(isModel2Choose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(400, 100, 200, 50), "Guard")) {
			if(!isModel2Choose) {
				//Create New Model 2
				attr.Body = 2001;
				attr.Hair = 0;
				attr.Cloth = 0;
				attr.Pants = 0;
				attr.Shoes = 0;
				attr.MHandDress = 0;
				attr.AHeadDress = 0;
				attr.ZBackEquip = 0;
				createPlayer("2");
			}
		}
		//Body
		if(isBChoose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;
		GUI.Label (new Rect(0, 150, 500, 50), "Choose Body");
		if (GUI.Button (new Rect(0, 170, 70, 50), "B")) {
			judgeBody(Flag.B);
			bodyPart = 0;
			showBody.Clear ();
			judgeBodyTextureName("B","0");
		}

		if(isCChoose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(75, 170, 70, 50), "C")) {
			judgeBody(Flag.C);
			bodyPart = 1;
			judgeBodyName("C");
		}

		if(isHChoose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(150, 170, 70, 50), "H")) {
			judgeBody(Flag.H);
			bodyPart = 2;
			judgeBodyName("H");
		}
		
		if(isMChoose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(225, 170, 70, 50), "M")) {
			judgeBody(Flag.M);
			bodyPart = 3;
			judgeBodyName("M");
		}
		
		if(isPChoose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(300, 170, 70, 50), "P")) {
			judgeBody(Flag.P);
			bodyPart = 4;
			judgeBodyName("P");
		}
		
		if(isSChoose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(375, 170, 70, 50), "S")) {
			judgeBody(Flag.S);
			bodyPart = 5;
			judgeBodyName("S");
		}
		
		if(isAChoose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(450, 170, 70, 50), "A")) {
			judgeBody(Flag.A);
			bodyPart = 6;
			judgeBodyName("A");
		}
		
		if(isZChoose)
			GUI.backgroundColor = Color.red;
		else 
			GUI.backgroundColor = Color.white;

		if (GUI.Button (new Rect(525, 170, 70, 50), "Z")) {
			judgeBody(Flag.Z);
			bodyPart = 7;
			judgeBodyName("Z");
		}
		GUI.backgroundColor = Color.white;
		//Body Part
		GUI.Label (new Rect(0, 220, 500, 50), "Change Body Part: " + bodyPartText);
		if(isAvatar) {
			scrollPosition = GUI.BeginScrollView (new Rect (0, 240, 600, 50), scrollPosition, new Rect (0, 0, showBody.Count * 100, 50));
			if (showBody.Count > 0) {
				if(GUI.Button(new Rect(0, 0, 50, 30), "None")){
					chooseBodyPart(showBody[0].name, true);
					bodyPartText = "None";
					bodyTextureText = "";
					showBodyTexture.Clear();
				}
				for (int i=0; i<showBody.Count; i++) {
					if(GUI.Button(new Rect(80 * (i+1), 0, 80, 30), showBody[i].name)) {
						chooseBodyPart(showBody[i].name);
					}
				}
			}	
			GUI.EndScrollView ();
		}
		
		//Body Texture
		GUI.Label (new Rect(0, 280, 500, 50), "Change Body Texture: "+ bodyTextureText);
		if(isAvatar) {
			scrollPositionTexture = GUI.BeginScrollView (new Rect (0, 300, 600, 50), scrollPositionTexture, new Rect (0, 0, showBody.Count * 100, 50));
			if (showBodyTexture.Count > 0) {
				for (int i=0; i<showBodyTexture.Count; i++) {
					if(GUI.Button(new Rect(90 * i, 0, 90, 30), showBodyTexture[i].name)) {
						bodyTextureText = showBodyTexture[i].name;
						string[] name = showBodyTexture[i].name.Split("_"[0]);
						string attrString = string.Empty;
						if(int.Parse(name[3]) < 10) 
							attrString = "00" + name[3];
						else if(int.Parse(name[3])>= 10 && int.Parse(name[3]) < 100)
							attrString = "0" + name[3];
						else if(int.Parse(name[3]) >= 100)
							attrString = name[3];
						if(name[1].Equals("B")){
							int bodyPartTemp = attr.Body / 1000;
							attr.Body = int.Parse(bodyPartTemp + attrString);
						} else if(name[1].Equals("C")){
							int bodyPartTemp = attr.Cloth / 1000;
							attr.Cloth = int.Parse(bodyPartTemp + attrString);
						} else if(name[1].Equals("H")){
							int bodyPartTemp = attr.Hair / 1000;
							attr.Hair = int.Parse(bodyPartTemp + attrString);
						} else if(name[1].Equals("M")){
							int bodyPartTemp = attr.MHandDress / 1000;
							attr.MHandDress = int.Parse(bodyPartTemp + attrString);
						} else if(name[1].Equals("P")){
							int bodyPartTemp = attr.Pants / 1000;
							attr.Pants = int.Parse(bodyPartTemp + attrString);
						} else if(name[1].Equals("S")){
							int bodyPartTemp = attr.Shoes / 1000;
							attr.Shoes = int.Parse(bodyPartTemp + attrString);
						} else if(name[1].Equals("A")){
							int bodyPartTemp = attr.AHeadDress / 1000;
							attr.AHeadDress = int.Parse(bodyPartTemp + attrString);
						} else if(name[1].Equals("Z")){
							int bodyPartTemp = attr.ZBackEquip / 1000;
							attr.ZBackEquip = int.Parse(bodyPartTemp + attrString);
						}
						bodyPart = Array.IndexOf(strPart, name[1]);
						ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
					}
				}
			}	
			GUI.EndScrollView ();
		}

		GUI.Label(new Rect(0, 340, 650, 100), "Body       : "+ attr.Body);
		GUI.Label(new Rect(0, 360, 650, 100), "Hair       : "+ attr.Hair);
		GUI.Label(new Rect(0, 380, 650, 100), "AHeadDress : "+ attr.AHeadDress);
		GUI.Label(new Rect(0, 400, 650, 100), "Cloth      : "+ attr.Cloth);
		GUI.Label(new Rect(0, 420, 650, 100), "Pants      : "+ attr.Pants);
		GUI.Label(new Rect(0, 440, 650, 100), "Shoes      : "+ attr.Shoes);
		GUI.Label(new Rect(0, 460, 650, 100), "MHandDress : "+ attr.MHandDress);
		GUI.Label(new Rect(0, 480, 650, 100), "ZBackEquip : "+ attr.ZBackEquip);

		
		//Play Animation
		if(isAvatar && Application.isPlaying) {
			avatarAnimationTest = Selection.gameObjects[0].GetComponent<AvatarAnimationTest>();
			if(avatarAnimationTest != null) {
				scrollPositionAnimation = GUI.BeginScrollView (new Rect (0, 500, 650, 550), scrollPositionAnimation, new Rect (0, 0, 750, 40 * (allMotionAnimationClip.Count / 5)));
				for(int i=0; i < allMotionAnimationClip.Count; i++) {
					if(GUI.Button(new Rect(130 * (i % 5), (i / 5) * 40, 120, 30), allMotionAnimationClip[i].name)) {
						avatarAnimationTest.Play(allMotionAnimationClip[i].name);
					}
				}
				GUI.EndScrollView ();
			}
		}
	}
	private void getAllData(){
		allMotionAnimationClip.Clear();
		controller = Resources.Load("Character/PlayerModel_"+modelId+"/AvatarControl") as UnityEditor.Animations.AnimatorController;
		AnimationClip[] animationClip = controller.animationClips;
		for(int i=0; i<animationClip.Length; i++) {
			if(!allMotionAnimationClip.Contains(animationClip[i]))
				allMotionAnimationClip.Add(animationClip[i]);
		}
		allMotionAnimationClip.Sort(
			delegate(AnimationClip i1, AnimationClip i2) { 
			return i1.name.CompareTo(i2.name); 
		}
		);
	}
	void randomPlayer(){
		//B
		bodyPart = 0;
		showBody.Clear ();
		judgeBodyTextureName("B","0");
		if (showBodyTexture.Count > 0) {
			string[] name = showBodyTexture[UnityEngine.Random.Range(0, showBodyTexture.Count)].name.Split("_"[0]);
			int bodyPartTemp = attr.Body / 1000;
			string attrString = string.Empty;
			if(int.Parse(name[3]) < 10) 
				attrString = "00" + name[3];
			else if(int.Parse(name[3]) >= 10 && int.Parse(name[3]) < 100)
				attrString = "0" + name[3];
			else if(int.Parse(name[3]) >= 100)
				attrString = name[3];
			attr.Body = int.Parse(bodyPartTemp + attrString);
			bodyPart = Array.IndexOf(strPart, name[1]);
			ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
		}

		//C
		bodyPart = 1;
		judgeBodyName("C");
		if (showBody.Count > 0) {
			int random = UnityEngine.Random.Range(0, showBody.Count + 1);
			if(random == showBody.Count) {
				chooseBodyPart(showBody[0].name, true);
				showBodyTexture.Clear();
			} else {
				chooseBodyPart(showBody[random].name);
				if (showBodyTexture.Count > 0) {
					string[] name = showBodyTexture[UnityEngine.Random.Range(0, showBodyTexture.Count)].name.Split("_"[0]);
					int bodyPartTemp = attr.Cloth / 1000;
					string attrString = string.Empty;
					if(int.Parse(name[3]) < 10) 
						attrString = "00" + name[3];
					else if(int.Parse(name[3]) >= 10 && int.Parse(name[3]) < 100)
						attrString = "0" + name[3];
					else if(int.Parse(name[3]) >= 100)
						attrString = name[3];
					attr.Cloth = int.Parse(bodyPartTemp + attrString);
					bodyPart = Array.IndexOf(strPart, name[1]);
					ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
				}
			}
		}
		//H
		bodyPart = 2;
		judgeBodyName("H");
		if (showBody.Count > 0) {
			int random = UnityEngine.Random.Range(0, showBody.Count + 1);
			if(random == showBody.Count) {
				chooseBodyPart(showBody[0].name, true);
				showBodyTexture.Clear();
			} else {
				chooseBodyPart(showBody[random].name);
				if (showBodyTexture.Count > 0) {
					string[] name = showBodyTexture[UnityEngine.Random.Range(0, showBodyTexture.Count)].name.Split("_"[0]);
					int bodyPartTemp = attr.Hair / 1000;
					string attrString = string.Empty;
					if(int.Parse(name[3]) < 10) 
						attrString = "00" + name[3];
					else if(int.Parse(name[3]) >= 10 && int.Parse(name[3]) < 100)
						attrString = "0" + name[3];
					else if(int.Parse(name[3]) >= 100)
						attrString = name[3];
					attr.Hair = int.Parse(bodyPartTemp + attrString);
					bodyPart = Array.IndexOf(strPart, name[1]);
					ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
				}
			}
		}

		//M
		bodyPart = 3;
		judgeBodyName("M");
		if (showBody.Count > 0) {
			int random = UnityEngine.Random.Range(0, showBody.Count + 1);
			if(random == showBody.Count) {
				chooseBodyPart(showBody[0].name, true);
				showBodyTexture.Clear();
			} else {
				chooseBodyPart(showBody[random].name);
				if (showBodyTexture.Count > 0) {
					string[] name = showBodyTexture[UnityEngine.Random.Range(0, showBodyTexture.Count)].name.Split("_"[0]);
					int bodyPartTemp = attr.MHandDress / 1000;
					string attrString = string.Empty;
					if(int.Parse(name[3]) < 10) 
						attrString = "00" + name[3];
					else if(int.Parse(name[3]) >= 10 && int.Parse(name[3]) < 100)
						attrString = "0" + name[3];
					else if(int.Parse(name[3]) >= 100)
						attrString = name[3];
					attr.MHandDress = int.Parse(bodyPartTemp + attrString);
					bodyPart = Array.IndexOf(strPart, name[1]);
					ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
				}
			}
		}

		//P
		bodyPart = 4;
		judgeBodyName("P");
		if (showBody.Count > 0) {
			int random = UnityEngine.Random.Range(0, showBody.Count + 1);
			if(random == showBody.Count) {
				chooseBodyPart(showBody[0].name, true);
				showBodyTexture.Clear();
			} else {
				chooseBodyPart(showBody[random].name);
				if (showBodyTexture.Count > 0) {
					string[] name = showBodyTexture[UnityEngine.Random.Range(0, showBodyTexture.Count)].name.Split("_"[0]);
					int bodyPartTemp = attr.Pants / 1000;
					string attrString = string.Empty;
					if(int.Parse(name[3]) < 10) 
						attrString = "00" + name[3];
					else if(int.Parse(name[3]) >= 10 && int.Parse(name[3]) < 100)
						attrString = "0" + name[3];
					else if(int.Parse(name[3]) >= 100)
						attrString = name[3];
					attr.Pants = int.Parse(bodyPartTemp + attrString);
					bodyPart = Array.IndexOf(strPart, name[1]);
					ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
				}
			}
		}

		//S
		bodyPart = 5;
		judgeBodyName("S");
		if (showBody.Count > 0) {
			int random = UnityEngine.Random.Range(0, showBody.Count + 1);
			if(random == showBody.Count) {
				chooseBodyPart(showBody[0].name, true);
				showBodyTexture.Clear();
			} else {
				chooseBodyPart(showBody[random].name);
				if (showBodyTexture.Count > 0) {
					string[] name = showBodyTexture[UnityEngine.Random.Range(0, showBodyTexture.Count)].name.Split("_"[0]);
					int bodyPartTemp = attr.Shoes / 1000;
					string attrString = string.Empty;
					if(int.Parse(name[3]) < 10) 
						attrString = "00" + name[3];
					else if(int.Parse(name[3]) >= 10 && int.Parse(name[3]) < 100)
						attrString = "0" + name[3];
					else if(int.Parse(name[3]) >= 100)
						attrString = name[3];
					attr.Shoes = int.Parse(bodyPartTemp + attrString);
					bodyPart = Array.IndexOf(strPart, name[1]);
					ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
				}
			}
		}

		//A
		bodyPart = 6;
		judgeBodyName("A");
		if (showBody.Count > 0) {
			int random = UnityEngine.Random.Range(0, showBody.Count + 1);
			if(random == showBody.Count) {
				chooseBodyPart(showBody[0].name, true);
				showBodyTexture.Clear();
			} else {
				chooseBodyPart(showBody[random].name);
				if (showBodyTexture.Count > 0) {
					string[] name = showBodyTexture[UnityEngine.Random.Range(0, showBodyTexture.Count)].name.Split("_"[0]);
					int bodyPartTemp = attr.AHeadDress / 1000;
					string attrString = string.Empty;
					if(int.Parse(name[3]) < 10) 
						attrString = "00" + name[3];
					else if(int.Parse(name[3]) >= 10 && int.Parse(name[3]) < 100)
						attrString = "0" + name[3];
					else if(int.Parse(name[3]) >= 100)
						attrString = name[3];
					attr.AHeadDress = int.Parse(bodyPartTemp + attrString);
					bodyPart = Array.IndexOf(strPart, name[1]);
					ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
				}
			}
		}

		//Z
		bodyPart = 7;
		judgeBodyName("Z");
		if (showBody.Count > 0) {
			int random = UnityEngine.Random.Range(0, showBody.Count + 1);
			if(random == showBody.Count) {
				chooseBodyPart(showBody[0].name, true);
				showBodyTexture.Clear();
			} else {
				chooseBodyPart(showBody[random].name);
				if (showBodyTexture.Count > 0) {
					string[] name = showBodyTexture[UnityEngine.Random.Range(0, showBodyTexture.Count)].name.Split("_"[0]);
					int bodyPartTemp = attr.ZBackEquip / 1000;
					string attrString = string.Empty;
					if(int.Parse(name[3]) < 10) 
						attrString = "00" + name[3];
					else if(int.Parse(name[3]) >= 10 && int.Parse(name[3]) < 100)
						attrString = "0" + name[3];
					else if(int.Parse(name[3]) >= 100)
						attrString = name[3];
					attr.ZBackEquip = int.Parse(bodyPartTemp + attrString);
					bodyPart = Array.IndexOf(strPart, name[1]);
					ModelManager.Get.SetAvatarTexture(Selection.gameObjects[0] ,attr, modelId, bodyPart, int.Parse(name[2]), int.Parse(name[3]));
				}
			}
		}
	}

	void chooseBodyPart(string showBodyName, bool isNone = false){
		bodyPartText = showBodyName;
		string[] name = showBodyName.Split("_"[0]);
		judgeBodyTextureName(name[1], name[2]);

		if(name[1].Equals("C")){
			if(!isNone) {
				attr.Cloth = int.Parse(name[2] + "001");
			} else {
				attr.Cloth = 0;
			}
		} else if(name[1].Equals("H")){
			if(!isNone) {
				attr.Hair = int.Parse(name[2] + "001");
			} else {
				attr.Hair = 0;
			}
		} else if(name[1].Equals("M")){
			if(!isNone) {
				attr.MHandDress = int.Parse(name[2] + "001");
			} else {
				attr.MHandDress = 0;
			}
		} else if(name[1].Equals("P")){
			if(!isNone) {
				attr.Pants = int.Parse(name[2] + "001");
			} else {
				attr.Pants = 0;
			}
		} else if(name[1].Equals("S")){
			if(!isNone) {
				attr.Shoes = int.Parse(name[2] + "001");
			} else {
				attr.Shoes = 0;
			}
		} else if(name[1].Equals("A")){
			if(!isNone) {
				attr.AHeadDress = int.Parse(name[2] + "001");
			} else {
				attr.AHeadDress = 0;
			}
		} else if(name[1].Equals("Z")){
			if(!isNone) {
				attr.ZBackEquip = int.Parse(name[2] + "001");
			} else {
				attr.ZBackEquip = 0;
			}
		}
		if(showBodyTexture.Count > 0)
			bodyTextureText = showBodyTexture[0].name;

		ModelManager.Get.SetAvatar(ref selectGameObject, attr, modelId, EAnimatorType.AvatarControl, false);
	}

	void judgeBodyName(string body){
		showBody.Clear ();
		showBodyTexture.Clear ();
		for (int i=0; i<allBody.Count; i++) {
			string[] name = allBody[i].name.Split("_"[0]);
			if(name[0].Equals(modelId.ToString()) || name[0].Equals("3")){
				if(name[1].Equals(body)) {
					showBody.Add(allBody[i]);
				}
			}
		}
	}
	void judgeBodyTextureName(string body, string _bodyPart){
		showBodyTexture.Clear ();
		for (int i=0; i<allTextures.Count; i++) {
			string[] name = allTextures[i].name.Split("_"[0]);
			if(name[0].Equals(modelId.ToString()) || name[0].Equals("3")){
				if(name[1].Equals(body)) {
					if(name[2].Equals(_bodyPart)) {
						showBodyTexture.Add(allTextures[i]);
					}
				}
			}
		}
	}

	void judgeBody(Flag flag){
		isBChoose = false;
		isCChoose = false;
		isHChoose = false;
		isMChoose = false;
		isPChoose = false;
		isSChoose = false;
		isZChoose = false;
		isAChoose = false;
		switch(flag){
		case Flag.B:
			isBChoose = true;
			break;
		case Flag.C:
			isCChoose = true;
			break;
		case Flag.H:
			isHChoose = true;
			break;
		case Flag.M:
			isMChoose = true;
			break;
		case Flag.P:
			isPChoose = true;
			break;
		case Flag.S:
			isSChoose = true;
			break;
		case Flag.A:
			isAChoose = true;
			break;
		case Flag.Z:
			isZChoose = true;
			break;
		}
	}
}
