﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelManager : MonoBehaviour {
	private const int DRESS_NONE = 0;

	private GameObject DefPointObject = null;
	public static ModelManager Get;
	public GameObject PlayerInfoModel = null;

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
		loadAllBody("Character/PlayerModel_2/Model");
		loadAllBody("Character/PlayerModel_3/Model");
		string materialPath = "Character/Materials";
		Object[] resourceMaterial = Resources.LoadAll (materialPath);
		for (int i=0; i<resourceMaterial.Length; i++) {
			Material material = resourceMaterial[i] as Material;
			string path = string.Format("{0}/{1}",materialPath, material.name);
			materialCache.Add(path, material);
		}
		loadAllTexture("Character/PlayerModel_2/Texture");
		loadAllTexture("Character/PlayerModel_3/Texture");

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

	private void loadAllBody(string path) {
		Object[] resourceBody = Resources.LoadAll (path, typeof(GameObject));
		for (int i=0; i<resourceBody.Length; i++) {
			if(!resourceBody[i].name.Contains("PlayerModel")){
				GameObject obj = resourceBody[i] as GameObject;
				string keyPath = string.Format("{0}/{1}",path, obj.name);
				bodyCache.Add(keyPath, obj);
			}
		}
	}

	private void loadAllTexture(string path) {
		Object[] resourceTexture = Resources.LoadAll (path);
		for (int i=0; i<resourceTexture.Length; i++) {
			Texture texture = resourceTexture[i] as Texture;
			textureCache.Add(texture.name, texture);
		}
	}

	private GameObject loadBody(string path) {
		if (bodyCache.ContainsKey(path))
			return bodyCache[path];
		else {
			GameObject obj = Resources.Load(path) as GameObject;
			if (obj) {
				string keyPath = string.Format("{0}/{1}",path, obj.name);
				bodyCache.Add(keyPath, obj);
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

	public void CreateStorePlayer(GameObject Player, GameStruct.TAvatar Attr, GameStruct.TAvatarTexture AttrTexture){
		SetAvatar (ref Player, Attr, AttrTexture, false);
	}

	public PlayerBehaviour CreateGamePlayer(GameStruct.TAvatar Attr, GameStruct.TAvatarTexture AttrTexture, int Index, TeamKind Team, Vector3 BornPos, GamePostion Postion, int AILevel = 0, GameObject Res=null){
		if (Res == null)
			Res = new GameObject();

		SetAvatar (ref Res, Attr, AttrTexture, true);

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
	public void SetAvatarTexture(GameObject Player, GameStruct.TAvatar Attr, int BodyPart, int ModelPart, int TexturePart) {
		if (Player) {
			string mainBody = string.Format("PlayerModel_{0}", Attr.Body);
			string[] strPart = new string[]{"B", "C", "H", "M", "P", "S", "A", "Z"};
			if(BodyPart < 6) {
				GameObject obj = Player.transform.FindChild(mainBody).gameObject;
				if(obj) {
					string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}",Attr.Body, strPart[BodyPart], ModelPart, TexturePart);
					string namePath = string.Format("{0}_{1}_{2}_{3}",Attr.Body, strPart[BodyPart], ModelPart, TexturePart);
					Texture texture = loadTexture(namePath);
					if(!texture)
						texture = loadTexture(path);
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
					string namePath = string.Format("{0}_{1}_{2}_{3}",Attr.Body, strPart[BodyPart], ModelPart, TexturePart);
					Texture texture = loadTexture(namePath);
					if(!texture)
						texture = loadTexture(path);
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
					string namePath = string.Format("{0}_{1}_{2}_{3}",Attr.Body, strPart[BodyPart], ModelPart, TexturePart);
					Texture texture = loadTexture(namePath);
					if(!texture)
						texture = loadTexture(path);
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
	public void SetAvatar(ref GameObject result, GameStruct.TAvatar attr, GameStruct.TAvatarTexture AttrTexture, bool isUseRig = false)
	{
		try {
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
				if (avatarIndex [i] > 0) {
					string path = string.Empty;
					string materialPath = string.Format ("Character/Materials/Material_0");

					if (i == 0) 
						path = string.Format ("Character/PlayerModel_{0}/Model/{1}", avatarIndex [i], mainBody);
					else 
					if (i < 6) 
						path = string.Format ("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", attr.Body, avatarPart [i], avatarIndex [i]);
					else //it maybe A or Z
						path = string.Format ("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", "3", avatarPart [i], avatarIndex [i]);

					Object resObj = Resources.Load (path);
					if (resObj) {
						try {
							Material matObj = loadMaterial (materialPath);
							Texture texture = loadTexture(avatarPartTexture [i]);
							if(texture) 
								matObj.SetTexture("_MainTex", texture);

							avatarPartGO [i] = Instantiate (resObj) as GameObject;

							if (dummyBall == null) {
								Transform t = avatarPartGO [i].transform.FindChild ("DummyBall");
								if (t != null)
									dummyBall = t.gameObject;
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
							} else 
							if (i == 6) {
								headDress = avatarPartGO [i];
								headDress.GetComponent<MeshRenderer> ().material = matObj;
								headDress.GetComponent<MeshRenderer> ().material.name = avatarPart [i];
							} else 
							if (i == 7) {
								backEquipment = avatarPartGO [i];
								backEquipment.GetComponent<MeshRenderer> ().material = matObj;
								backEquipment.GetComponent<MeshRenderer> ().material.name = avatarPart [i];
							}
						} catch (UnityException e) {
							Debug.Log(e.ToString());
						}
					}
				}
			}

			if (dummyBall != null)
				dummyBall.transform.parent = result.transform;

			//HeadDress
			if (dummyHead == null) {
				Transform t = result.transform.FindChild ("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead");
				if (tag != null)
					dummyHead = t.gameObject;
			}

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
			
			GameObject clone = GameObject.Find(result.name + "/" + mainBody);
			if (clone == null)
				clone = new GameObject();

			SkinnedMeshRenderer resultSmr = clone.gameObject.GetComponent<SkinnedMeshRenderer>();
			if (resultSmr == null)
				resultSmr = clone.gameObject.AddComponent<SkinnedMeshRenderer>();

			if (resultSmr.sharedMesh == null)
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
		} catch (UnityException e) {
			Debug.Log(e.ToString());
		}
	}


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
