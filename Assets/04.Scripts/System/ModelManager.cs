using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStruct;
using ProMaterialCombiner;

public class ModelManager : KnightSingleton<ModelManager> {
	public const string Name = "ModelManager";

	private GameObject DefPointObject = null;
	public GameObject PlayerInfoModel = null;
	public GameObject AnimatorCurveManager;

	private Dictionary<string, GameObject> bodyCache = new Dictionary<string, GameObject>();
	private Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
	private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
	private Dictionary<string, RuntimeAnimatorController> controllorCache = new Dictionary<string, RuntimeAnimatorController>();

	void Awake() {
		PlayerInfoModel = new GameObject();
		PlayerInfoModel.name = "PlayerInfoModel";
		//UIPanel up = PlayerInfoModel.AddComponent<UIPanel>();
		//up.depth = 2;

		DefPointObject = Resources.Load("Character/Component/DefPoint") as GameObject;
		AnimatorCurveManager = Resources.Load("Character/Component/AnimatorCurve") as GameObject;
	}

	private void loadAllBody(string path) {
		GameObject[] resourceBody = Resources.LoadAll<GameObject> (path);
		if (resourceBody != null) {
			for (int i=0; i<resourceBody.Length; i++) {
				if(!resourceBody[i].name.Contains("PlayerModel")){
					string keyPath = string.Format("{0}/{1}",path, resourceBody[i].name);
					loadBody(keyPath);
				}
			}
		}
	}

	private void loadAllTexture(string path) {
		Texture[] resourceTexture = Resources.LoadAll<Texture> (path);
		if (resourceTexture != null) {
			for (int i=0; i<resourceTexture.Length; i++) {
				string keyPath = string.Format("{0}/{1}",path, resourceTexture[i].name);
				loadTexture(keyPath);
			}
		}
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

	private Texture2D loadTexture(string path) {
		if (textureCache.ContainsKey(path)) {
			return textureCache [path];
		}else {
			Texture2D obj = Resources.Load(path) as Texture2D;
			if (obj) {
				textureCache.Add(path, obj);
				return obj;
			} else {
				//download form server
				return null;
			}
		}
	}

	private RuntimeAnimatorController loadController(string path) {
		if (controllorCache.ContainsKey(path)) {
			return controllorCache [path];
		}else {
			RuntimeAnimatorController obj = Resources.Load(path) as RuntimeAnimatorController;
			if (obj) {
				controllorCache.Add(path, obj);
				return obj;
			} else {
				//download form server
				return null;
			}
		}
	}

    public PlayerBehaviour CreateGamePlayer(int TeamIndex, ETeamKind Team, Vector3 BornPos, TPlayer player, GameObject Res=null){
		if (Res == null)
			Res = new GameObject();

		SetAvatar (ref Res, player.Avatar, player.BodyType, true, false); 

		Res.transform.parent = PlayerInfoModel.transform;
		Res.transform.localPosition = BornPos;

		PlayerBehaviour PB = Res.AddComponent<PlayerBehaviour>();

		PB.Team = Team;
		PB.MoveIndex = -1;
		PB.Player = player;
		PB.Index = TeamIndex;
		
		PB.InitTrigger (DefPointObject);
		PB.InitCurve (AnimatorCurveManager);
		PB.InitAttr ();
		Res.name = Team.ToString() + TeamIndex.ToString();

		if(Team == ETeamKind.Npc)
			Res.transform.localEulerAngles = new Vector3(0, 180, 0);

		return PB;
	}

	public void SetAvatarTexture(GameObject Player, GameStruct.TAvatar Attr, int bodyType, int BodyPart, int ModelPart, int TexturePart) {
		if (Player) {
//			string bodyNumber = (Attr.Body / 1000).ToString();
			string bodyNumber = bodyType.ToString();;
//			string mainBody = string.Format("PlayerModel_{0}", bodyNumber);
			string mainBody = "PlayerModel";
			string[] strPart = new string[]{"B", "C", "H", "M", "P", "S", "A", "Z"};
			if(BodyPart < 6) {
				Transform t = Player.transform.FindChild(mainBody);
				if (t != null) {
					GameObject obj = t.gameObject;
					if(obj) {
						string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}",bodyNumber, strPart[BodyPart], ModelPart, TexturePart);
						string namePath = string.Format("{0}_{1}_{2}_{3}",bodyNumber, strPart[BodyPart], ModelPart, TexturePart);
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
				}
			} else if(BodyPart == 6){
				string bodyPath = string.Format("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead/3_{0}_{1}(Clone)", strPart[BodyPart], ModelPart);
				Transform t = Player.transform.Find(bodyPath);
				if (t != null) {
					GameObject obj = t.gameObject;
					if(obj) {
						string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", strPart[BodyPart], ModelPart, TexturePart);
						string namePath = string.Format("{0}_{1}_{2}_{3}",bodyNumber, strPart[BodyPart], ModelPart, TexturePart);
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
			} else if (BodyPart == 7){
				string bodyPath = string.Format("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack/3_{0}_{1}(Clone)", strPart[BodyPart], ModelPart);
				GameObject obj = Player.transform.Find(bodyPath).gameObject;
				if(obj) {
					string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", strPart[BodyPart], ModelPart, TexturePart);
					string namePath = string.Format("{0}_{1}_{2}_{3}",bodyNumber, strPart[BodyPart], ModelPart, TexturePart);
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

	public void SetAvatar(ref GameObject result, GameStruct.TAvatar attr, int bodyType, bool isUseRig, bool combine = true) {
		try {
//			string bodyNumber = (attr.Body / 1000).ToString();
//			Debug.Log("bodyType:"+ bodyType);
			string bodyNumber = bodyType.ToString();
			string mainBody = string.Format ("PlayerModel_{0}", bodyNumber);
			string[] avatarPart = new string[]{mainBody, "C", "H", "M", "P", "S", "A", "Z"};
			int[] avatarIndex = new int[] {attr.Body, attr.Cloth, attr.Hair, attr.MHandDress, attr.Pants, attr.Shoes, attr.AHeadDress, attr.ZBackEquip};

			GameObject dummyBall = null;
			GameObject bipGO = null;
			
			Transform[] hips;
			List<CombineInstance> combineInstances = new List<CombineInstance> ();
			List<Material> materials = new List<Material> ();
			List<Transform> bones = new List<Transform> ();

			string path = string.Empty;
			string texturePath = string.Empty;
			Material matObj = loadMaterial ("Character/Materials/Material_0");

			Transform dt = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead");
			if (dt != null) 
				for (int j = 0; j < dt.childCount; j++) 
					Destroy(dt.GetChild(j).gameObject);
		
			dt = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack");
			if (dt != null) 
				for (int j = 0; j < dt.childCount; j++) 
					Destroy(dt.GetChild(j).gameObject);

			for (int i = 0; i < avatarIndex.Length; i++) {
				if (avatarIndex [i] > 0) {
					int avatarBody = avatarIndex[i] / 1000;
					int avatarBodyTexture = avatarIndex[i] % 1000;
					if (i == 0) {
						path = string.Format ("Character/PlayerModel_{0}/Model/{1}", bodyNumber, mainBody); 
						texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyNumber, "B", "0", avatarBodyTexture);
					}else 
					if (i < 6) {
						path = string.Format ("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", bodyNumber, avatarPart [i], avatarBody);
						texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyNumber, avatarPart [i], avatarBody, avatarBodyTexture);
					} else  {//it maybe A or Z
						path = string.Format ("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", "3", avatarPart [i], avatarBody);
						texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", avatarPart [i], avatarBody, avatarBodyTexture);
					}
					
					GameObject resObj = loadBody(path);
					if (resObj) {
						try {
							GameObject avatarPartGO = Instantiate (resObj) as GameObject;
							Texture texture = loadTexture(texturePath);
							matObj.SetTexture("_MainTex", texture);
							
							if (i < 6) {
								Transform tBipGo = result.transform.FindChild("Bip01");
								if(tBipGo == null) {
									if (bipGO == null) {
										bipGO = avatarPartGO.transform.FindChild ("Bip01").gameObject;
										
										if (bipGO)
											bipGO.transform.parent = result.transform;
									}
								} else 
									bipGO = tBipGo.gameObject;
								
								if(dummyBall == null) {
									Transform t = result.transform.FindChild("DummyBall");
									if(t == null){
										if (dummyBall == null) {
											Transform t1 = avatarPartGO.transform.FindChild ("DummyBall");
											if (t1 != null)
												dummyBall = t1.gameObject;
										}
									} else 
										dummyBall = t.gameObject;

									dummyBall.transform.parent = result.transform;
								}
								
								hips = bipGO.GetComponentsInChildren<Transform> ();
								if (hips.Length > 0) {
									CombineInstance ci = new CombineInstance ();
									SkinnedMeshRenderer smr = avatarPartGO.GetComponentInChildren<SkinnedMeshRenderer> ();
									if (smr != null) {
										smr.material = matObj;
										if (i == 0) 
											smr.material.name = "B";
										else
											smr.material.name = avatarPart [i];
										ci.mesh = smr.sharedMesh;
									}

									combineInstances.Add (ci);

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

								Destroy (avatarPartGO);
								avatarPartGO = null;
							} else 
							if (i == 6) {
								Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead");
								if (t != null) {
									for (int j=0; j<t.childCount; j++) 
										Destroy(t.GetChild(j).gameObject);

									MeshRenderer mr = avatarPartGO.GetComponent<MeshRenderer> ();
									if (mr != null)
										mr.material = matObj;

									avatarPartGO.transform.parent = t;
									avatarPartGO.transform.localPosition = Vector3.zero;
									avatarPartGO.transform.localEulerAngles = Vector3.zero;
									avatarPartGO.transform.localScale = Vector3.one;
								}
							} else 
							if (i == 7) {
								Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack");
								if (t != null) 
									for (int j=0; j<t.childCount; j++) 
										Destroy(t.GetChild(j).gameObject);

								MeshRenderer mr = avatarPartGO.GetComponent<MeshRenderer> ();
								if (mr != null)
									mr.material = matObj;

								avatarPartGO.transform.parent = t;
								avatarPartGO.transform.localPosition = Vector3.zero;
								avatarPartGO.transform.localEulerAngles = Vector3.zero;
								avatarPartGO.transform.localScale = Vector3.one;
							}
						} catch (UnityException e) {
							Debug.Log(e.ToString());
						}
					}
				}
			}

			GameObject clone = null;
			dt = result.transform.Find("PlayerModel");
			if (dt != null)
				clone = dt.gameObject;
			else
				clone = new GameObject();
			
			SkinnedMeshRenderer resultSmr = clone.gameObject.GetComponent<SkinnedMeshRenderer>();
			if (resultSmr == null)
				resultSmr = clone.gameObject.AddComponent<SkinnedMeshRenderer>();
			
			if (resultSmr.sharedMesh == null)
				resultSmr.sharedMesh = new Mesh();
			
			resultSmr.sharedMesh.CombineMeshes(combineInstances.ToArray() , false , false);
			resultSmr.bones = bones.ToArray();
			resultSmr.materials = materials.ToArray();
			resultSmr.gameObject.isStatic = true;
			resultSmr.receiveShadows = false;

			if (!combine) {
				clone.transform.parent = result.transform;
				clone.layer = LayerMask.NameToLayer ("Player");
				clone.name = "PlayerModel";
			} else {
				MaterialCombiner materialCombiner = new MaterialCombiner(clone, true);
				GameObject cobbineObject = materialCombiner.CombineMaterial (matObj);
				cobbineObject.transform.parent = result.transform;
				cobbineObject.layer = LayerMask.NameToLayer ("Player");
				cobbineObject.name = "PlayerModel";

				Destroy(clone);
				clone = null;
			}

			//collider
			CapsuleCollider collider = result.GetComponent<CapsuleCollider>();
			
			if(collider == null)
				collider = result.AddComponent<CapsuleCollider>();
			
			switch (bodyType)
			{
				case 0:
					collider.radius = 1;
					collider.height = 3.5f;
					break;
					
				case 1:
					collider.radius = 0.88f;
		            collider.height = 3.2f;
		            break;
		            
		        case 2:
		            collider.radius = 0.88f;
		            collider.height = 3f;
		            break;
				default: 
					collider.radius = 1;
					collider.height = 3.5f;
				break;
            }
            
            collider.center = new Vector3 (0, collider.height / 2f, 0);

			//animator
			Animator aniControl = result.GetComponent<Animator>();
			if(aniControl == null)
				aniControl = result.AddComponent<Animator>();
			
			RuntimeAnimatorController runtimeAnimatorController = aniControl.runtimeAnimatorController;
			if(runtimeAnimatorController == null) {
				if(isUseRig)
					runtimeAnimatorController = loadController(string.Format("Character/PlayerModel_{0}/AnimationControl", bodyNumber));
				else
					runtimeAnimatorController = loadController(string.Format("Character/PlayerModel_{0}/AvatarControl", bodyNumber));
				aniControl.runtimeAnimatorController = runtimeAnimatorController;
				aniControl.applyRootMotion = false;

			} else {
				if(!isUseRig)
					runtimeAnimatorController = loadController(string.Format("Character/PlayerModel_{0}/AvatarControl", bodyNumber));
				aniControl.runtimeAnimatorController = runtimeAnimatorController;
				aniControl.applyRootMotion = false;
			}

			
			//rig
			if(isUseRig){
				Rigidbody rig = result.GetComponent<Rigidbody> ();
				if (rig == null)
					rig = result.AddComponent<Rigidbody> ();
				
				rig.freezeRotation = true;
			}
		} catch (UnityException e) {
			Debug.Log(e.ToString());
		}
	}

	public void ChangeLayersRecursively(Transform trans, string name) {
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans) {            
			ChangeLayersRecursively(child, name);
		}
	}
}
