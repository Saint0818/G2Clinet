using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;
using GameStruct;
using GameEnum;
using ProMaterialCombiner;

public enum EanimatorType
{
	AnimationControl,
	AvatarControl,
	ShowControl
}

public class ModelManager : KnightSingleton<ModelManager> {
	public const string Name = "ModelManager";

	private GameObject DefPointObject = null;
	public GameObject PlayerInfoModel = null;
	public GameObject AnimatorCurveManager;

	private Material materialSource;
	private Dictionary<string, GameObject> bodyCache = new Dictionary<string, GameObject>();
	private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
	private Dictionary<string, RuntimeAnimatorController> controllorCache = new Dictionary<string, RuntimeAnimatorController>();

	void Awake() {
		PlayerInfoModel = new GameObject();
		PlayerInfoModel.name = "PlayerInfoModel";

		materialSource = Resources.Load("Character/Materials/Material_0") as Material;
		DefPointObject = Resources.Load("Character/Component/DefPoint") as GameObject;
		AnimatorCurveManager = Resources.Load("Character/Component/AnimatorCurve") as GameObject;
	}

	public void PreloadResource(TAvatar attr, int bodyType) {
		//load animator
		for (int i = 0; i < 3; i++) {
			loadController(string.Format("Character/PlayerModel_{0}/{1}", i, EanimatorType.AnimationControl.ToString()));
			loadController(string.Format("Character/PlayerModel_{0}/{1}", i, EanimatorType.AvatarControl.ToString()));
			loadController(string.Format("Character/PlayerModel_{0}/{1}", i, EanimatorType.ShowControl.ToString()));
		}

		string bodyNumber = bodyType.ToString();
		string mainBody = string.Format ("PlayerModel_{0}", bodyNumber);
		string[] avatarPart = new string[]{mainBody, "C", "H", "M", "P", "S", "A", "Z"};
		int[] avatarIndex = new int[] {attr.Body, attr.Cloth, attr.Hair, attr.MHandDress, attr.Pants, attr.Shoes, attr.AHeadDress, attr.ZBackEquip};
		string path;
		string texturePath;
		for (int i = 0; i < avatarIndex.Length; i++) {
			if (avatarIndex [i] > 0) {
				int avatarBody = avatarIndex[i] / 1000;
				int avatarBodyTexture = avatarIndex[i] % 1000;
				if (i == 0) {
					path = string.Format ("Character/PlayerModel_{0}/Model/{1}", bodyNumber, mainBody); 
					texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyNumber, "B", "0", avatarBodyTexture);
				} else 
				if (i < 6) {
					path = string.Format ("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", bodyNumber, avatarPart [i], avatarBody);
					texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyNumber, avatarPart [i], avatarBody, avatarBodyTexture);
				} else  {//it maybe A or Z
					path = string.Format ("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", "3", avatarPart [i], avatarBody);
					texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", avatarPart [i], avatarBody, avatarBodyTexture);
				}
				
				GameObject resObj = loadBody(path);
				if (resObj) 
					loadTexture(texturePath);
            }
        }
    }

	public void LoadAllSelectPlayer(int[] id) {
		for (int i = 0; i < id.Length; i++)
		if (GameData.DPlayers.ContainsKey(id[i])) {
			TAvatar avatar = new TAvatar(id[i]);
			PreloadResource(avatar, GameData.DPlayers[id[i]].BodyType);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="teamIndex"> 0:C, 1:F, 2:G </param>
    /// <param name="team"></param>
    /// <param name="bornPos"></param>
    /// <param name="player"></param>
    /// <param name="res"></param>
    /// <returns></returns>
    public PlayerBehaviour CreateGamePlayer(int teamIndex, ETeamKind team, Vector3 bornPos, TPlayer player, 
                                            GameObject res = null)
    {
		if (res == null)
			res = new GameObject();

		if (GameStart.Get.TestModel != EModelTest.None && GameStart.Get.TestMode != EGameTest.None)
			player.BodyType = (int)GameStart.Get.TestModel;

		SetAvatar(ref res, player.Avatar, player.BodyType, true, true); 

		res.transform.parent = PlayerInfoModel.transform;
		res.transform.localPosition = bornPos;
		res.AddComponent<SkillController>();

		PlayerBehaviour playerBehaviour = res.AddComponent<PlayerBehaviour>();

		playerBehaviour.Team = team;
		playerBehaviour.MoveIndex = -1;
		playerBehaviour.Attribute = player;
		playerBehaviour.Index = teamIndex;
		if(team == ETeamKind.Self)
			playerBehaviour.SetTimerKey((ETimerKind)Enum.Parse(typeof(ETimerKind), string.Format("Player{0}", teamIndex)));
		else
			playerBehaviour.SetTimerKey((ETimerKind)Enum.Parse(typeof(ETimerKind), string.Format("Player{0}", 3 +teamIndex)));

		if(teamIndex == 0)
			playerBehaviour.Postion = EPlayerPostion.C;
		else if(teamIndex == 1)
			playerBehaviour.Postion = EPlayerPostion.F;
		else if(teamIndex == 2)
			playerBehaviour.Postion = EPlayerPostion.G;
		
		playerBehaviour.InitTrigger(DefPointObject);
		playerBehaviour.InitCurve(AnimatorCurveManager);
		playerBehaviour.InitAttr();
		res.name = team.ToString() + teamIndex.ToString();

		if(team == ETeamKind.Npc)
			res.transform.localEulerAngles = new Vector3(0, 180, 0);

        // 目前 PlayerAI 必須要依賴 PlayerBehavior 才能做事情, 所以 PlayerAI 加到
        // GameObject 時, PlayerBehavior 必須要已經存在了.
        res.AddComponent<PlayerAI>();

        return playerBehaviour;
	}

	public void SetAvatarTexture(GameObject Player, GameStruct.TAvatar Attr, int bodyType, int BodyKind, int avatarNo) {
		int ModelPart = (int)(avatarNo / 1000);
		int TexturePart = avatarNo % 1000;
		int BodyPart = -1;

		switch (BodyKind) {
			case 1:
				BodyPart = 2; // H
				break;
				
			case 2:
				BodyPart = 3; // M
				break;
				
			case 3:
				BodyPart = 1; // C
				break;
				
			case 4:
				BodyPart = 4; //P
				break;
				
			case 5:
				BodyPart = 5;//S
				break;
				
			case 10:
				BodyPart = 6;//A
				break;
				
			case 11:
				BodyPart = 7; //Z
				break;	
		
		}

		if(BodyPart > 0)
			SetAvatarTexture(Player, Attr, bodyType, BodyPart, ModelPart, TexturePart);
	}

	public void SetAvatarTexture(GameObject Player, GameStruct.TAvatar Attr, int bodyType, int BodyPart, int ModelPart, int TexturePart) {
		if (Player) {
			string bodyNumber = bodyType.ToString();;
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

	public void SetAvatar(ref GameObject result, TAvatar attr, int bodyType, bool isUseRig, bool combine = true, bool Reset = false) {
		try {
			string bodyNumber = bodyType.ToString();
			string mainBody = string.Format ("PlayerModel_{0}", bodyNumber);
			string[] avatarPart = new string[]{mainBody, "C", "H", "M", "P", "S", "A", "Z"};
			int[] avatarIndex = new int[] {attr.Body, attr.Cloth, attr.Hair, attr.MHandDress, attr.Pants, attr.Shoes, attr.AHeadDress, attr.ZBackEquip};

			if(Reset){
				Destroy(result);			
				result = new GameObject();
			}

			GameObject dummyBall = null;
			GameObject bipGO = null;
			
			Transform[] hips;
			List<CombineInstance> combineInstances = new List<CombineInstance> ();
			List<Material> materials = new List<Material> ();
			List<Transform> bones = new List<Transform> ();

			string path = string.Empty;
			string texturePath = string.Empty;
			Material matObj = materialSource;

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
								Material matObj1 = Instantiate(materialSource);
								if (t != null && matObj1 != null) {
									for (int j=0; j<t.childCount; j++) 
										Destroy(t.GetChild(j).gameObject);

									MeshRenderer mr = avatarPartGO.GetComponent<MeshRenderer> ();
									if (mr != null)
										mr.material = matObj1;

									avatarPartGO.transform.parent = t;
									avatarPartGO.transform.localPosition = Vector3.zero;
									avatarPartGO.transform.localEulerAngles = Vector3.zero;
									avatarPartGO.transform.localScale = Vector3.one;
								}
							} else 
							if (i == 7) {
								Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack");
								Material matObj1 = Instantiate(materialSource);
								if (t != null && matObj1!= null) 
									for (int j=0; j<t.childCount; j++) 
										Destroy(t.GetChild(j).gameObject);

								MeshRenderer mr = avatarPartGO.GetComponent<MeshRenderer> ();
								if (mr != null)
									mr.material = matObj1;

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
			resultSmr.useLightProbes = false;

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
			
			switch (bodyType) {
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

			aniControl.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			if(aniControl.runtimeAnimatorController == null) {
				if(isUseRig)
					ChangeAnimator( aniControl, bodyNumber, EanimatorType.ShowControl);
				else
					ChangeAnimator( aniControl, bodyNumber, EanimatorType.AvatarControl);
			} else {
				if(!isUseRig)
					ChangeAnimator( aniControl, bodyNumber, EanimatorType.AvatarControl);
				else 
					aniControl.applyRootMotion = false;
			}
			
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

	public void ChangeAnimator(Animator ani,string bodyNumber, EanimatorType type) {
		RuntimeAnimatorController runtimeAnimatorController = loadController(string.Format("Character/PlayerModel_{0}/{1}", bodyNumber, type.ToString()));
		ani.runtimeAnimatorController = runtimeAnimatorController;
		ani.parameters.Initialize ();
		ani.applyRootMotion = false;
	}
}
