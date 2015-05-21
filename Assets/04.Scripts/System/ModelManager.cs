using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStruct;
using RootMotion.FinalIK;

public class ModelManager : KnightSingleton<ModelManager> {
	public const string Name = "ModelManager";

	private GameObject DefPointObject = null;
	public GameObject PlayerInfoModel = null;
	public GameObject AnimatorCurveManager;

	private Dictionary<string, GameObject> bodyCache = new Dictionary<string, GameObject>();
	private Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
	private Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();

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

    public PlayerBehaviour CreateGamePlayer(int TeamIndex, TeamKind Team, Vector3 BornPos, TPlayer player, GameObject Res=null){
		if (Res == null)
			Res = new GameObject();

		SetAvatar (ref Res, player.Avatar, true);

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

		if(Team == TeamKind.Npc)
			Res.transform.localEulerAngles = new Vector3(0, 180, 0);

		return PB;
	}

	public void SetAvatarTexture(GameObject Player, GameStruct.TAvatar Attr, int BodyPart, int ModelPart, int TexturePart) {
		if (Player) {
			string bodyNumber = (Attr.Body / 1000).ToString();
			string mainBody = string.Format("PlayerModel_{0}", bodyNumber);
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

	/// <summary>
	/// c:Clothes, h:Hair, m:HandEquipment, p:Pants, s:Shoes, a:Headdress, z:BackbackEquipment
	/// </summary>
	public void SetAvatar(ref GameObject result, GameStruct.TAvatar attr, bool isUseRig = false) {
		try {
			string bodyNumber = (attr.Body / 1000).ToString();
			string mainBody = string.Format ("PlayerModel_{0}", bodyNumber);
			string[] avatarPart = new string[]{mainBody, "C", "H", "M", "P", "S", "A", "Z"};

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
			GameObject dummyCatch = null;
			GameObject headDress = null;
			GameObject backEquipment = null;
			GameObject bipGO = null;
			GameObject ikPin = null;
			GameObject ikAim = null;
			
			Transform[] hips;
			List<CombineInstance> combineInstances = new List<CombineInstance> ();
			List<Material> materials = new List<Material> ();
			List<Transform> bones = new List<Transform> ();
			
			for (int i = 0; i < avatarIndex.Length; i++) {
				if (avatarIndex [i] > 0) {
					string path = string.Empty;
					string materialPath = string.Format ("Character/Materials/Material_0");
					string texturePath = string.Empty;

					int avatarBody = avatarIndex[i] / 1000;
					int avatarBodyTexture = avatarIndex[i] % 10;
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
					GameObject resObj = Resources.Load (path) as GameObject;
					if (resObj) {
						try {
							Material matObj = loadMaterial (materialPath);
							Texture texture = loadTexture(texturePath);
							if(!texture) 
								loadTexture(texturePath);

							matObj.SetTexture("_MainTex", texture);
							avatarPartGO [i] = Instantiate (resObj) as GameObject;

							if (i < 6) {
								Transform tBipGo = result.transform.FindChild("Bip01");
								if(tBipGo == null) {
									if (bipGO == null) {
										bipGO = avatarPartGO [i].transform.FindChild ("Bip01").gameObject;

										if (bipGO)
											bipGO.transform.parent = result.transform;
									}
								} else {
									bipGO = tBipGo.gameObject;
								}
								
								if(dummyBall == null) {
									Transform tBall = result.transform.FindChild("DummyBall");
									if(tBall == null){
										if (dummyBall == null) {
											Transform t1 = avatarPartGO [i].transform.FindChild ("DummyBall");
											if (t1 != null)
												dummyBall = t1.gameObject;
										}
									} else {
										dummyBall = tBall.gameObject;
									}
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
								Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead");
								int count = t.childCount;
								if(count > 0) {
									for (int j=0; j<count; j++) {
										Destroy(t.GetChild(j).gameObject);
									}
								}
								headDress = avatarPartGO [i];
								headDress.GetComponent<MeshRenderer> ().material = matObj;
								headDress.GetComponent<MeshRenderer> ().material.name = avatarPart [i];
							} else 
							if (i == 7) {
								Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack");
								int count = t.childCount;
								if(count > 0) {
									for(int j=0; j<count; j++){
										Destroy(t.GetChild(j).gameObject);
									}
								}
								backEquipment = avatarPartGO [i];
								backEquipment.GetComponent<MeshRenderer> ().material = matObj;
								backEquipment.GetComponent<MeshRenderer> ().material.name = avatarPart [i];
							}
						} catch (UnityException e) {
							Debug.Log(e.ToString());
						}
					}
				} else {
					if(i == 6){
						Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead");
						int count = t.childCount;
						if(count > 0) {
							for (int j=0; j<count; j++) {
								Destroy(t.GetChild(j).gameObject);
							}
						}
					} else 
					if (i == 7) {
						Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack");
						int count = t.childCount;
						if(count > 0) {
							for(int j=0; j<count; j++){
								Destroy(t.GetChild(j).gameObject);
							}
						}
					}
				}
//				if(isUseRig){
//					if(ikPin == null) {
//						Transform tPin = result.transform.FindChild("Pin");
//						if(tPin == null) {
//							GameObject obj = new GameObject();
//							obj.name = "Pin";
//							obj.transform.parent = result.transform;
//							if(bodyNumber.Equals("0")) {
//								obj.transform.localPosition = new Vector3(0, 2.9f, 1.7f);
//							} else if(bodyNumber.Equals("1")) {
//								obj.transform.localPosition = new Vector3(0, 2.7f, 1.7f);
//							} else if(bodyNumber.Equals("2")) {
//								obj.transform.localPosition = new Vector3(0, 2, 1.7f);
//							}
//							obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
//						}
//					}
//					
//					if(ikAim == null) {
//						Transform tAimParent = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
//						Transform tAim = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Aim");
//						if(tAim == null) {
//							GameObject obj = new GameObject();
//							obj.name = "Aim";
//							obj.transform.parent = tAimParent;
//							obj.transform.localPosition = Vector3.zero;
//							obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
//						}
//					}
//				}

				if (dummyBall != null){
					dummyBall.transform.parent = result.transform;

					if(dummyBall.GetComponent<BlockCatchTrigger>() == null)
						dummyBall.gameObject.AddComponent<BlockCatchTrigger>();
				}

				//HeadDress
				if (dummyHead == null) {
					Transform t = result.transform.FindChild ("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead");
					if (t != null)
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
			resultSmr.gameObject.isStatic = true;
			clone.name = mainBody;
			clone.transform.parent = result.transform;
			clone.layer = LayerMask.NameToLayer ("Player");


			if (result.transform.FindChild("DummyCatch") == null) {
				dummyCatch = new GameObject();
			} else {
				dummyCatch = result.transform.FindChild("DummyCatch").gameObject ;
			}

			if (dummyCatch != null) {
				dummyCatch.name = "DummyCatch";
				dummyCatch.transform.parent = result.transform;
				dummyCatch.transform.localPosition = new Vector3(0, 1.5f, 1f);
			}
			
			//animator
			Animator aniControl = result.GetComponent<Animator>();
			if(aniControl == null)
				aniControl = result.AddComponent<Animator>();

			RuntimeAnimatorController runtimeAnimatorController = aniControl.runtimeAnimatorController;
			if(runtimeAnimatorController == null) {
				if(isUseRig)
					runtimeAnimatorController = Resources.Load(string.Format("Character/PlayerModel_{0}/AnimationControl", bodyNumber)) as RuntimeAnimatorController;
				else
					runtimeAnimatorController = Resources.Load(string.Format("Character/PlayerModel_{0}/AvatarControl", bodyNumber)) as RuntimeAnimatorController;

				aniControl.runtimeAnimatorController = runtimeAnimatorController;
				aniControl.applyRootMotion = false;
			}
			
			//collider
			CapsuleCollider collider = result.GetComponent<CapsuleCollider>();

			if(collider == null)
				collider = result.AddComponent<CapsuleCollider>();
			if(bodyNumber.Equals("0")) {
				collider.radius = 1;
				collider.height = 3.5f;
				collider.center = new Vector3 (0, collider.height / 2f, 0);
			} else 
			if(bodyNumber.Equals("1")) {
				collider.radius = 0.88f;
				collider.height = 3.2f;
				collider.center = new Vector3 (0, collider.height / 2f, 0);
			} else
			if(bodyNumber.Equals("2")) {
				collider.radius = 0.88f;
				collider.height = 3f;
				collider.center = new Vector3 (0, collider.height / 2f, 0);
			}

			//IK
//			if(isUseRig) {
//				addIK(result);
//			}
			
			//rig
			if(isUseRig){
				Rigidbody rig = result.GetComponent<Rigidbody> ();
				if (rig == null)
					rig = result.AddComponent<Rigidbody> ();

//				rig.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
				rig.freezeRotation = true;
			}
		} catch (UnityException e) {
			Debug.Log(e.ToString());
		}
	}

	private void addIK(GameObject result){
		Transform tAim1 = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/Aim");
		AimIK aimIK = result.GetComponent<AimIK> ();
		Transform tAimBone1 = result.transform.FindChild("Bip01/Bip01 Spine");
		Transform tAimBone2 = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1");
		Transform tAimBone3 = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck");
		Transform tAimBone4 = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
		if(aimIK == null) {
			aimIK = result.AddComponent<AimIK> ();
			aimIK.solver.transform = tAim1; 
			aimIK.solver.bones = new IKSolver.Bone[4];
			aimIK.solver.bones[0] = new IKSolver.Bone();
			aimIK.solver.bones[1] = new IKSolver.Bone();
			aimIK.solver.bones[2] = new IKSolver.Bone();
			aimIK.solver.bones[3] = new IKSolver.Bone();
			aimIK.solver.bones[0].transform = tAimBone1;
			aimIK.solver.bones[0].weight = 0.25f;
			aimIK.solver.bones[1].transform = tAimBone2;
			aimIK.solver.bones[1].weight = 0.25f;
			aimIK.solver.bones[2].transform = tAimBone3;
			aimIK.solver.bones[2].weight = 0.5f;
			aimIK.solver.bones[3].transform = tAimBone4;
			aimIK.solver.bones[3].weight = 0.5f;
		}

		RootMotion.FinalIK.RotationLimitAngle boneRotationLimit1 = tAimBone1.gameObject.GetComponent<RootMotion.FinalIK.RotationLimitAngle>();
		if(!boneRotationLimit1 )
			boneRotationLimit1 = tAimBone1.gameObject.AddComponent<RootMotion.FinalIK.RotationLimitAngle>();

		RootMotion.FinalIK.RotationLimitAngle boneRotationLimit2 = tAimBone2.gameObject.GetComponent<RootMotion.FinalIK.RotationLimitAngle>();
		if(!boneRotationLimit2 )
			boneRotationLimit2 = tAimBone2.gameObject.AddComponent<RootMotion.FinalIK.RotationLimitAngle>();

		RootMotion.FinalIK.RotationLimitAngle boneRotationLimit3 = tAimBone3.gameObject.GetComponent<RootMotion.FinalIK.RotationLimitAngle>();
		if(!boneRotationLimit3 )
			boneRotationLimit3 = tAimBone3.gameObject.AddComponent<RootMotion.FinalIK.RotationLimitAngle>();

		RootMotion.FinalIK.RotationLimitAngle boneRotationLimit4 = tAimBone4.gameObject.GetComponent<RootMotion.FinalIK.RotationLimitAngle>();
		if(!boneRotationLimit4 )
			boneRotationLimit4 = tAimBone4.gameObject.AddComponent<RootMotion.FinalIK.RotationLimitAngle>();
		boneRotationLimit1.axis = new Vector3(-1, 1, 0);
		boneRotationLimit1.limit = 20;
		boneRotationLimit1.twistLimit = 20;
		boneRotationLimit2.axis = new Vector3(-1, 1, 0);
		boneRotationLimit2.limit = 20;
		boneRotationLimit2.twistLimit = 20;
		boneRotationLimit3.axis = new Vector3(-1, 1, 0);
		boneRotationLimit3.limit = 20;
		boneRotationLimit3.twistLimit = 20;
		boneRotationLimit4.axis = new Vector3(-1, 1, 0);
		boneRotationLimit4.limit = 20;
		boneRotationLimit4.twistLimit = 20;

		FullBodyBipedIK fbbik = result.GetComponent<FullBodyBipedIK>();
		if(fbbik == null)
			fbbik = result.AddComponent<FullBodyBipedIK>();
		RootMotion.BipedReferences bipedRef = new RootMotion.BipedReferences();

		bipedRef.root = result.transform.FindChild("Bip01");
		bipedRef.pelvis = result.transform.FindChild("Bip01/Bip01 Pelvis");
		bipedRef.leftThigh = result.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 L Thigh");
		bipedRef.leftCalf = result.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 L Thigh/Bip01 L Calf");
		bipedRef.leftFoot = result.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 L Thigh/Bip01 L Calf/Bip01 L Foot");
		bipedRef.rightThigh = result.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 R Thigh");
		bipedRef.rightCalf = result.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 R Thigh/Bip01 R Calf");
		bipedRef.rightFoot  = result.transform.FindChild("Bip01/Bip01 Pelvis/Bip01 R Thigh/Bip01 R Calf/Bip01 R Foot");
		bipedRef.leftUpperArm = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 L Clavicle/Bip01 L UpperArm");
		bipedRef.leftForearm = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm");
		bipedRef.leftHand = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand/Bip01 L Finger2");
		bipedRef.rightUpperArm = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm");
		bipedRef.rightForearm = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm");
		bipedRef.rightHand = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 R Finger2");
		bipedRef.head = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
		bipedRef.spine = new Transform[2];
		bipedRef.spine[0] = result.transform.FindChild("Bip01/Bip01 Spine");
		bipedRef.spine[1] = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1");
		fbbik.SetReferences(bipedRef, result.transform.FindChild("Bip01/Bip01 Spine"));
		fbbik.solver.GetEffector(FullBodyBipedEffector.LeftHand).positionWeight = 0.8f;
		fbbik.solver.GetEffector(FullBodyBipedEffector.RightHand).positionWeight = 0.8f;
		fbbik.solver.pullBodyVertical = 0.2f;
		fbbik.solver.pullBodyHorizontal = 0.3f;
	}

	public void ChangeLayersRecursively(Transform trans, string name) {
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans) {            
			ChangeLayersRecursively(child, name);
		}
	}
}
