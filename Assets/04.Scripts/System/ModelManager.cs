using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelManager : MonoBehaviour {
	private const int DRESS_NONE = 0;

	private GameObject DefPointObject = null;
	public static ModelManager Get;
	public GameObject PlayerInfoModel = null;
	private const int avatartCount = 8;

	private Dictionary<string, GameObject> bodyCache = new Dictionary<string, GameObject>();
	private Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
	private Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();
	private Dictionary<string, AnimationClip> AniData = new Dictionary<string, AnimationClip> ();

	public static void Init(){
		GameObject gobj = new GameObject(typeof(ModelManager).Name);
		DontDestroyOnLoad(gobj);
		Get = gobj.AddComponent<ModelManager>();
	}

	void Awake(){
		//Cache
		string body_2Path = "Character/PlayerModel_2/Model";
		Object[] resourceBody_2 = Resources.LoadAll (body_2Path, typeof(GameObject));
		for (int i=0; i<resourceBody_2.Length; i++) {
			if(!resourceBody_2[i].name.Contains("PlayerModel")){
				GameObject obj = resourceBody_2[i] as GameObject;
				string path = string.Format("{0}/{1}",body_2Path, obj.name);
				bodyCache.Add(path, obj);
			}
		}
		string body_3Path = "Character/PlayerModel_3/Model";
		Object[] resourceBody_3 = Resources.LoadAll (body_3Path, typeof(GameObject));
		for (int i=0; i<resourceBody_3.Length; i++) {
			if(!resourceBody_3[i].name.Contains("PlayerModel")){
				GameObject obj = resourceBody_3[i] as GameObject;
				string path = string.Format("{0}/{1}",body_3Path, obj.name);
				bodyCache.Add(path, obj);
			}
		}
		string materialPath = "Character/Materials";
		Object[] resourceMaterial = Resources.LoadAll (materialPath);
		for (int i=0; i<resourceMaterial.Length; i++) {
			Material material = resourceMaterial[i] as Material;
			string path = string.Format("{0}/{1}",materialPath, material.name);
			materialCache.Add(path, material);
		}
		string texture_2Path = "Character/PlayerModel_2/Texture";
		Object[] resourceTexture_2 = Resources.LoadAll (texture_2Path);
		for (int i=0; i<resourceTexture_2.Length; i++) {
			Texture texture = resourceTexture_2[i] as Texture;
			string path = string.Format("{0}/{1}",texture_2Path, texture.name);
			textureCache.Add(texture.name, texture);
		}
		string texture_3Path = "Character/PlayerModel_3/Texture";
		Object[] resourceTexture_3 = Resources.LoadAll ("Character/PlayerModel_3/Texture");
		for (int i=0; i<resourceTexture_3.Length; i++) {
			Texture texture = resourceTexture_3[i] as Texture;
			string path = string.Format("{0}/{1}",texture_3Path, texture.name);
			textureCache.Add(texture.name, texture);
		}

		AnimationClip[] ani = Resources.LoadAll<AnimationClip>("FBX/Animation");
		if (ani.Length > 0) {
			for(int i = 0; i < ani.Length; i++) {
				string keyname = ani[i].name.Replace(" (UnityEngine.AnimationClip)", "");
				if(!AniData.ContainsKey(keyname))
					AniData.Add(keyname, ani[i]);
			}
		}

		PlayerInfoModel = new GameObject();
		PlayerInfoModel.name = "PlayerInfoModel";
		UIPanel up = PlayerInfoModel.AddComponent<UIPanel>();
		up.depth = 2;

		DefPointObject = Resources.Load("Character/Component/DefPoint") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private GameObject loadBody(string path) {
		if (bodyCache.ContainsKey(path))
			return bodyCache[path];
		else {
			GameObject obj = Resources.Load(path) as GameObject;
			if (obj) {
				bodyCache.Add(path, obj);
				return obj;
			} else {
				//download form server
				return null;
			}
		}
	}

	private Material loadMaterial(string path) {
		if (materialCache.ContainsKey(path)) {
			return materialCache [path];
		} else {
			Material obj = Resources.Load(path) as Material;
			if (obj) {
				materialCache.Add(path, obj);
				return obj;
			} else {
				//download form server
				return null;
			}
		}
	}

	private Texture loadTexture(string path) {
		if (textureCache.ContainsKey(path)) {
			return textureCache [path];
		}else {
			Texture obj = Resources.Load(path) as Texture;
			if (obj) {
				textureCache.Add(path, obj);
				return obj;
			} else {
				//download form server
				return null;
			}
		}
	}

	public PlayerBehaviour CreateStorePlayer(GameObject Player, GameStruct.TAvatar Attr, GameStruct.TAvatarTexture AttrTexture, Vector3 BornPos){
		if (Player != null) 
			Destroy (Player);

		GameObject Res = SetAvatar (Attr, AttrTexture, false);
		Res.transform.parent = PlayerInfoModel.transform;
		Res.transform.localPosition = BornPos;

		PlayerBehaviour PB = Res.AddComponent<PlayerBehaviour>();
		return PB;
	}


	public PlayerBehaviour CreateGamePlayer(GameStruct.TAvatar Attr, GameStruct.TAvatarTexture AttrTexture, int Index, TeamKind Team, Vector3 BornPos, GamePostion Postion, int AILevel = 0){
		GameObject Res = SetAvatar (Attr, AttrTexture, true);
		Res.transform.parent = PlayerInfoModel.transform;
		Res.transform.localPosition = BornPos;
		GameObject DefPointCopy = Instantiate(DefPointObject) as GameObject;
		DefPointCopy.transform.parent = Res.transform;
		DefPointCopy.transform.localPosition = Vector3.zero;
		if(Team == TeamKind.Npc)
			Res.transform.localEulerAngles = new Vector3(0, 180, 0);
		PlayerBehaviour PB = Res.AddComponent<PlayerBehaviour>();
		PB.Team = Team;
		PB.MoveIndex = -1;
		PB.Postion = Postion;
		PB.AILevel = AILevel;
		Res.name = Index.ToString();
		DefPointCopy.name = "DefPoint";
		PB.DefPointAy [DefPoint.Front.GetHashCode()] = DefPointCopy.transform.Find ("Front").gameObject.transform;
		PB.DefPointAy [DefPoint.Back.GetHashCode()] = DefPointCopy.transform.Find ("Back").gameObject.transform;
		PB.DefPointAy [DefPoint.Right.GetHashCode()] = DefPointCopy.transform.Find ("Right").gameObject.transform;
		PB.DefPointAy [DefPoint.Left.GetHashCode()] = DefPointCopy.transform.Find ("Left").gameObject.transform;
		PB.DefPointAy [DefPoint.FrontSteal.GetHashCode()] = DefPointCopy.transform.Find ("FrontSteal").gameObject.transform;
		PB.DefPointAy [DefPoint.BackSteal.GetHashCode()] = DefPointCopy.transform.Find ("BackSteal").gameObject.transform;
		PB.DefPointAy [DefPoint.RightSteal.GetHashCode()] = DefPointCopy.transform.Find ("RightSteal").gameObject.transform;
		PB.DefPointAy [DefPoint.LeftSteal.GetHashCode()] = DefPointCopy.transform.Find ("LeftSteal").gameObject.transform;

		return PB;
	}
	//BodyPart 
	public void SetAvatarTexture(GameObject Player, GameStruct.TAvatar Attr, int BodyPart, int ModelPart, int TexturePart) {
		if (Player) {
			string mainBody = string.Format("PlayerModel_{0}", Attr.Body);
			string[] strPart = new string[]{"B", "C", "H", "M", "P", "S", "A", "Z"};
			if(BodyPart < 6) {
				GameObject obj = Player.transform.FindChild(mainBody).gameObject;
				if(obj) {
					string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}",Attr.Body, strPart[BodyPart], ModelPart, TexturePart);
					Texture texture = loadTexture(path);
					Renderer renderers = obj.GetComponent<Renderer>();
					Material[] materials = renderers.materials;
					for(int i=0; i<materials.Length; i++){
						if(materials[i].name.Equals(strPart[BodyPart] + " (Instance)")) {
							if(texture)
								materials[i].mainTexture = texture;
							break;
						}
					}
				}
			} else if(BodyPart == 6){
				string bodyPath = string.Format("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead/3_{0}_{1}(Clone)", strPart[BodyPart], ModelPart);
				GameObject obj = Player.transform.Find(bodyPath).gameObject;
				if(obj) {
					string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", strPart[BodyPart], ModelPart, TexturePart);
					Texture texture = loadTexture(path);
					Renderer renderers = obj.GetComponent<Renderer>();
					Material[] materials = renderers.materials;
					for(int i=0; i<materials.Length; i++){
						if(materials[i].name.Equals(strPart[BodyPart])) {
							if(texture)
								materials[i].mainTexture = texture;
							break;
						}
					}
				}
			} else if (BodyPart == 7){
				string bodyPath = string.Format("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack/3_{0}_{1}(Clone)", strPart[BodyPart], ModelPart);
				GameObject obj = Player.transform.Find(bodyPath).gameObject;
				if(obj) {
					string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", strPart[BodyPart], ModelPart, TexturePart);
					Texture texture = loadTexture(path);
					Renderer renderers = obj.GetComponent<Renderer>();
					Material[] materials = renderers.materials;
					for(int i=0; i<materials.Length; i++){
						if(materials[i].name.Equals(strPart[BodyPart])) {
							if(texture)
								materials[i].mainTexture = texture;
							break;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// c:Clothes, h:Hair, m:HandEquipment, p:Pants, s:Shoes, a:Headdress, z:BackbackEquipment
	/// </summary>
	public GameObject SetAvatar(GameStruct.TAvatar attr, GameStruct.TAvatarTexture AttrTexture, bool isUseRig = false)
	{
		string mainBody = string.Format ("PlayerModel_{0}", attr.Body);
		string[] avatarPart = new string[]{mainBody, "C", "H", "M", "P", "S", "A", "Z"};
		string[] avatarPartTexture = new string[] {
			AttrTexture.BTexture,
			AttrTexture.CTexture,
			AttrTexture.HTexture,
			AttrTexture.MTexture,
			AttrTexture.PTexture,
			AttrTexture.STexture,
			AttrTexture.ATexture,
			AttrTexture.ZTexture
		};
		int[] avatarIndex = new int[] {
			attr.Body,
			attr.Cloth,
			attr.Hair,
			attr.MHandDress,
			attr.Pants,
			attr.Shoes,
			attr.AHeadDress,
			attr.ZBackEquip
		};
		GameObject[] avatarPartGO = new GameObject[avatarIndex.Length];
		GameObject result = new GameObject ();
		GameObject dummyBall = null;
		GameObject dummyHead = null;
		GameObject dummyBack = null;
		GameObject headDress = null;
		GameObject backEquipment = null;
		GameObject bipGO = null;
		
		Transform[] hips;
		List<CombineInstance> combineInstances = new List<CombineInstance> ();
		List<Material> materials = new List<Material> ();
		List<Transform> bones = new List<Transform> ();
		
		for (int i = 0; i < avatarIndex.Length; i++) {
			string path = string.Empty;
			string texturePath = string.Format ("Chatacter/PlayerModel_{0}/Texture/{1}", attr.Body, avatarPartTexture [i]);
			string materialPath = string.Format ("Character/Materials/Material_0");

			if (i == 0) {
				path = string.Format ("Character/PlayerModel_{0}/Model/{1}", avatarIndex [i], mainBody);
			} else if (avatarIndex [i] > DRESS_NONE) {
				path = string.Format ("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", attr.Body, avatarPart [i], avatarIndex [i]);
			}

			Object resObj = Resources.Load (path);
			if (!resObj && avatarIndex [i] > DRESS_NONE) {
				//it maybe A or F
				path = string.Format ("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", "3", avatarPart [i], avatarIndex [i]);
				resObj = Resources.Load (path);
			}

			if (resObj) {
				Material matObj = loadMaterial (materialPath);
				Texture texture = loadTexture(avatarPartTexture[i]);
				if(texture) {
					matObj.SetTexture("_MainTex", texture);
				}

				if (resObj != null) {
					avatarPartGO [i] = Instantiate (resObj) as GameObject;

					if (i == 0)
						dummyBall = avatarPartGO [i].transform.FindChild ("DummyBall").gameObject;
				}
				if (i < 6) {
					if (bipGO == null) {
						bipGO = avatarPartGO [i].transform.FindChild ("Bip01").gameObject;
						
						if (bipGO)
							bipGO.transform.parent = result.transform;
					}

					hips = bipGO.GetComponentsInChildren<Transform> ();
					
					if (avatarPartGO [i] && hips.Length > 0) {
						SkinnedMeshRenderer smr = avatarPartGO [i].GetComponentInChildren<SkinnedMeshRenderer> ();
						
						//ready combine mesh
						CombineInstance ci = new CombineInstance ();
						ci.mesh = smr.sharedMesh;
						combineInstances.Add (ci);
						smr.material = matObj;
						if (i == 0)
							smr.material.name = "B";
						else 
							smr.material.name = avatarPart [i];
						//sort new material
						materials.AddRange (smr.materials);
						
						//get same bip to create new bones  
						foreach (Transform bone in smr.bones) {
							foreach (Transform hip in hips) {
								
								if (hip.name != bone.name)
									continue;
								bones.Add (hip);
								break;
							}
						}
					}
					Destroy (avatarPartGO [i]);
				} else if (i == 6) {
					headDress = avatarPartGO [i];
					headDress.GetComponent<MeshRenderer> ().material = matObj;
					headDress.GetComponent<MeshRenderer> ().material.name = avatarPart [i];
				} else if (i == 7) {
					backEquipment = avatarPartGO [i];
					backEquipment.GetComponent<MeshRenderer> ().material = matObj;
					backEquipment.GetComponent<MeshRenderer> ().material.name = avatarPart [i];
				}
			}
		}
		
		if (dummyBall != null)
			dummyBall.transform.parent = result.transform;

		//HeadDress
		if (dummyHead == null)
			dummyHead = result.transform.FindChild ("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead").gameObject;
		if (headDress != null) {
			headDress.transform.parent = dummyHead.transform;
			headDress.transform.localPosition = Vector3.zero;
			headDress.transform.localEulerAngles = Vector3.zero;

		}
		
		//BackEquipment
		if (dummyBack == null)
			dummyBack = result.transform.FindChild ("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack").gameObject;
		if (backEquipment != null) {
			backEquipment.transform.parent = dummyBack.transform;
			backEquipment.transform.localPosition = Vector3.zero;
			backEquipment.transform.localEulerAngles = Vector3.zero;
		}
		
		GameObject clone = new GameObject();
		SkinnedMeshRenderer resultSmr = clone.gameObject.AddComponent<SkinnedMeshRenderer>();
		resultSmr.sharedMesh = new Mesh();
		resultSmr.sharedMesh.CombineMeshes(combineInstances.ToArray() , false , false);
		resultSmr.bones = bones.ToArray();
		resultSmr.materials = materials.ToArray();
		clone.name = mainBody;
		clone.transform.parent = result.transform;

		//animator
		Animator aniControl = result.AddComponent<Animator>();
		RuntimeAnimatorController test = Resources.Load(string.Format("Character/PlayerModel_{0}/AnimationControl", attr.Body)) as RuntimeAnimatorController;
		aniControl.runtimeAnimatorController = test;
		aniControl.applyRootMotion = false;
		
		//collider
		CapsuleCollider collider = result.AddComponent<CapsuleCollider>();
		collider.radius = 0.32f;
		collider.height = 3f;
		float testh = collider.height / 2f;
		collider.center = new Vector3 (0, testh, 0);
		
		//rig
		if (isUseRig) {
			Rigidbody rig = result.GetComponent<Rigidbody> ();
			if (rig == null)
				rig = result.AddComponent<Rigidbody> ();
			
			rig.freezeRotation = true;
		}
		
		return result;
	}
//	public GameObject AddPlayer(int model = 0, int c = -1, int h = -1, int m = -1, int p = -1, int s =-1, int a =-1, int z =-1)
//	{
//		string mainBody = string.Format("PlayerModel_{0}", model);
//		string[] avatarPart = new string[]{mainBody, "C", "H", "M", "P", "S", "A", "Z"};
//		int[] avatarIndex = new int[]{model, c, h, m, p, s, a, z};
//		GameObject[] avatarPartGO = new GameObject[avatarIndex.Length];
//		GameObject result = new GameObject();
//		GameObject dummyBall = null;
//		GameObject dummyHead = null;
//		GameObject dummyBack = null;
//		GameObject headDressObj = null;
//		GameObject backEquipmentObj = null;
//		
//		Transform[] hips;
//		List<CombineInstance> combineInstances = new List<CombineInstance>();
//		List<Material> materials = new List<Material>();
//		List<Transform> bones = new List<Transform>();
//		
//		for (int i = 0; i < avatarIndex.Length; i++) {
////			if(i > 6)
////				continue;
//			string path = string.Empty;
//			string materialPath = string.Empty;
//
//			if(i == 0){
//				path = string.Format("Character/PlayerModel_{0}/{1}", avatarIndex[i], mainBody);
//				materialPath = string.Format("Character/PlayerModel_{0}/Materials/{0}_{1}",model, "B");
//			}else if(avatarIndex[i] > -1){
//				path = string.Format("Character/PlayerModel_{0}/{0}_{1}_{2}", model,avatarPart[i],avatarIndex[i]);
//				materialPath = string.Format("Character/PlayerModel_{0}/Materials/{0}_{1}",model, avatarPart[i]);
//			}
//
//
//			if(path != string.Empty)
//			{
//				Object resObj = Resources.Load(path);
//				Material matObj = Resources.Load(materialPath) as Material;
//				if(resObj != null)
//				{
//					avatarPartGO[i] = Instantiate(resObj) as GameObject;
//
//					if(i == 0)
//						dummyBall = avatarPartGO[i].transform.FindChild("DummyBall").gameObject;
//				}
//
//			
//				if(i < 6) {
//					if(bipGO == null)
//					{
//						bipGO = avatarPartGO[i].transform.FindChild("Bip01").gameObject;
//						
//						if(bipGO)
//							bipGO.transform.parent = result.transform;
//					}
//					
//					hips = bipGO.GetComponentsInChildren<Transform>();
//					
//					if(avatarPartGO[i] && hips.Length > 0)
//					{
//						SkinnedMeshRenderer smr = avatarPartGO[i].GetComponentInChildren<SkinnedMeshRenderer>();
//						
//						//ready combine mesh
//						CombineInstance ci = new CombineInstance();
//						ci.mesh = smr.sharedMesh;
//						combineInstances.Add(ci);
//						smr.material = matObj;
//						smr.material.name = avatarPart[i];
//						//sort new material
//						materials.AddRange(smr.materials);
//
//						//get same bip to create new bones  
//						foreach(Transform bone in smr.bones){
//							foreach(Transform hip in hips){
//								
//								if(hip.name != bone.name) continue;
//								bones.Add(hip);
//								break;
//							}
//						}
//					}
//					Destroy(avatarPartGO[i]);
//				} else if(i == 6) {
//					headDressObj = avatarPartGO[i];
//				} else if(i == 7) {
//					backEquipmentObj = avatarPartGO[i];
//				}
//			}
//		}
//
//
//		if (dummyBall != null)
//			dummyBall.transform.parent = result.transform;
//		
//		GameObject clone = new GameObject();
//		SkinnedMeshRenderer resultSmr = clone.gameObject.AddComponent<SkinnedMeshRenderer>();
//		resultSmr.sharedMesh = new Mesh();
//		resultSmr.sharedMesh.CombineMeshes(combineInstances.ToArray() , false , false);
//		resultSmr.bones = bones.ToArray();
//		resultSmr.materials = materials.ToArray();
//		clone.name = mainBody;
//		clone.transform.parent = result.transform;
//
//		//HeadDress
//		if (dummyHead == null)
//			dummyHead = result.transform.FindChild ("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead").gameObject;
//		if (headDressObj != null) {
//			headDressObj.transform.parent = dummyHead.transform;
//			headDressObj.transform.localPosition = Vector3.zero;
//			headDressObj.transform.localEulerAngles = Vector3.zero;
//		}
//
//		//BackEquipment
//		if (dummyBack == null)
//			dummyBack = result.transform.FindChild ("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack").gameObject;
//		if (backEquipmentObj != null) {
//			backEquipmentObj.transform.parent = dummyBack.transform;
//			backEquipmentObj.transform.localPosition = Vector3.zero;
//			backEquipmentObj.transform.localEulerAngles = Vector3.zero;
//		}
//
//		//animator
//		Animator aniControl = result.AddComponent<Animator>();
//		RuntimeAnimatorController test = Resources.Load(string.Format("Character/PlayerModel_{0}/AnimationControl", model)) as RuntimeAnimatorController;
//		aniControl.runtimeAnimatorController = test;
//		aniControl.applyRootMotion = false;
//
//		//collider
//		CapsuleCollider collider = result.AddComponent<CapsuleCollider>();
//		collider.radius = 0.32f;
//		collider.height = 3f;
//		float testh = collider.height / 2f;
//		collider.center = new Vector3 (0, testh, 0);
//
//		//rig
//		Rigidbody rig = result.GetComponent<Rigidbody> ();
//		if (rig == null)
//			rig = result.AddComponent<Rigidbody> ();
//
//		rig.freezeRotation = true;
//
//		return result;
//	}

	public void AddAnimation(GameObject player, string anistr)
	{
		Animation ani;
		bool ishave = false;
		
		ani = player.GetComponent<Animation>();
		
		if (!ani)
			ani = player.AddComponent<Animation> ();
		
		foreach (AnimationState item in ani) {
			if(item.name == anistr)
				ishave = true;
		}
		
		if (!ishave)
		{
			if(!AniData.ContainsKey(anistr))
			{
				AnimationClip skillAni = Resources.Load<AnimationClip>(string.Format("FBX/SkillAnimation/{0}", anistr));
				AniData.Add(anistr, skillAni);
			}
				
			Animation addani = player.GetComponent<Animation>();

			if(addani)
			{
				addani.AddClip(AniData[anistr], anistr);
				addani.clip = AniData[anistr];
			}
		}
	}
}
