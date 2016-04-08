using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using ProMaterialCombiner;

public class TAvatarLoader : MonoBehaviour {
    public int mBodyType;
    public TAvatar mAvatars;
    public EAnimatorType mAnimatorType = EAnimatorType.None;

	private static string[] avatarPartStr = {"B", "H", "M", "C", "P", "S", "A", "Z"};
	private static Material materialSource;
	private static Dictionary<string, GameObject> modelCache = new Dictionary<string, GameObject>();
	private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    private static Dictionary<string, RuntimeAnimatorController> controllorCache = new Dictionary<string, RuntimeAnimatorController>();
	
    public static GameObject CreateAvatarLoader(int bodyType, TAvatar avatars,  GameObject avatarObj,  GameObject anchorObj = null, ELayer layer = ELayer.Default) {
        if (avatarObj != null) {
            Destroy(avatarObj);
            avatarObj = null;
        }

        GameObject obj = new GameObject("PlayerModel");
        TAvatarLoader avatarLoader = obj.AddComponent<TAvatarLoader>();
        if (anchorObj) {
            obj.transform.parent = anchorObj.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
        }

        avatarLoader.LoadAvatar(obj, bodyType, avatars, layer);
        return obj;
    }

    public static void ReleaseCache() {
        modelCache.Clear();
        textureCache.Clear();
    }

    void Awake () {
		if (materialSource == null)
			materialSource = Resources.Load("Character/Materials/Material_0") as Material;
	}

    void OnDestroy() {
        Destroy(gameObject);
        Resources.UnloadUnusedAssets();
    }

    private static RuntimeAnimatorController loadController(int bodyNumber, EAnimatorType type) {
        string key = type.ToString() + bodyNumber.ToString();
        if (key != string.Empty) {
            if (controllorCache.ContainsKey(key))
                return controllorCache[key];
            else {
                string path = string.Format("Character/PlayerModel_{0}/{1}", bodyNumber, type.ToString());

                RuntimeAnimatorController obj = Resources.Load(path) as RuntimeAnimatorController;
                if (obj) {
                    controllorCache.Add(key, obj);
                    return obj;
                }
                else {
                    //download form server
                    return null;
                }
            }
        }
        return null;
    }

    private IEnumerator AsyncSetAvatar(GameObject resObj, int bodyType, TAvatar avatars, ELayer layer) {
        yield return new WaitForEndOfFrame();

		int[] avatarIndex = new int[] { avatars.Body, avatars.Hair, avatars.MHandDress, avatars.Cloth, avatars.Pants, avatars.Shoes, avatars.AHeadDress, avatars.ZBackEquip };
		string texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, "B", "0", avatars.Body);
        string path = string.Format("Character/PlayerModel_{0}/Model/PlayerModel_{1}", bodyType, bodyType);
        Material matObj = materialSource;
        GameObject avatarModel = null;
        Texture2D texture = null;
        GameObject bipGO = null;

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Material> materials = new List<Material>();
        List<Transform> bones = new List<Transform>();

        for (int i = 0; i < avatarIndex.Length; i++) {
			if (avatarIndex[i] > 0) {
				int avatarBody = avatarIndex[i] / 1000;
				int avatarBodyTexture = avatarIndex[i] % 1000;
                texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, avatarPartStr[i], avatarBody, avatarBodyTexture);
                if (i == 0)
                    string.Format("Character/PlayerModel_{0}/Model/PlayerModel_{1}", bodyType, bodyType);
                else
                if (i < 6)
                    path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", bodyType, avatarPartStr[i], avatarBody);
                else {//it maybe A or Z
                    path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", "3", avatarPartStr[i], avatarBody);
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", avatarPartStr[i], avatarBody, avatarBodyTexture);
                }

                if (modelCache.ContainsKey(path))
                    avatarModel = modelCache[path];
				else {
					ResourceRequest request = Resources.LoadAsync(path);
					yield return request;
					avatarModel = request.asset as GameObject;
                    if (!modelCache.ContainsKey(path))
                        modelCache.Add(path, avatarModel);
				}

                if (avatarModel != null) {
					texture = null;
					if (textureCache.ContainsKey(texturePath))
						texture = textureCache[texturePath];
					else {
						ResourceRequest request = Resources.LoadAsync(texturePath);
						yield return request;
						texture = request.asset as Texture2D;
                        if (!textureCache.ContainsKey(texturePath))
						    textureCache.Add (texturePath, texture);
					}

					if (texture != null) {
                        GameObject avatarPartObj = Instantiate(avatarModel) as GameObject;
                        if (i < 6) {
                            Transform tBipGo = resObj.transform.FindChild("Bip01");
                            if (tBipGo == null) {
                                if (bipGO == null) {
                                    bipGO = avatarPartObj.transform.FindChild("Bip01").gameObject;

                                    if (bipGO)
                                        bipGO.transform.parent = resObj.transform;
                                }
                            } else
                                bipGO = tBipGo.gameObject;
                            
                            SkinnedMeshRenderer avatarSMR = avatarPartObj.GetComponentInChildren<SkinnedMeshRenderer>();
                            if (avatarSMR != null) {
                                matObj.SetTexture ("_MainTex", texture);
                                avatarSMR.material = matObj;

                                CombineInstance ci = new CombineInstance();
                                ci.mesh = avatarSMR.sharedMesh;
                                combineInstances.Add(ci);
                                materials.AddRange(avatarSMR.materials);

                                Transform[] bodyHips = bipGO.GetComponentsInChildren<Transform>();
                                foreach (Transform bone in avatarSMR.bones)
                                {
                                    foreach (Transform hip in bodyHips)
                                    {
                                        if (hip.name != bone.name)
                                            continue;

                                        bones.Add(hip);
                                        break;
                                    }
                                }
                            }
                        } else {
                            Transform t = resObj.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead");
                            Material matObj1 = Instantiate(materialSource);
                            MeshRenderer mr = avatarPartObj.GetComponent<MeshRenderer>();
                            if (mr != null)
                                mr.material = matObj1;

                            avatarPartObj.transform.parent = t;
                            avatarPartObj.transform.localPosition = Vector3.zero;
                            avatarPartObj.transform.localEulerAngles = Vector3.zero;
                            avatarPartObj.transform.localScale = Vector3.one;
                            if (layer != ELayer.Default)
                                LayerMgr.Get.SetLayer(avatarPartObj, layer);
                        }

                        Destroy(avatarPartObj);
                        avatarPartObj = null;
					}
				}
            }
        }

        if (combineInstances.Count > 0) {
            GameObject clone = new GameObject();
            SkinnedMeshRenderer resultSmr = clone.gameObject.AddComponent<SkinnedMeshRenderer>();
            resultSmr.sharedMesh = new Mesh();
            resultSmr.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
            resultSmr.materials = materials.ToArray();
            resultSmr.bones = bones.ToArray();
            resultSmr.gameObject.isStatic = true;
            resultSmr.receiveShadows = false;
            resultSmr.useLightProbes = false;
            resultSmr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            MaterialCombiner materialCombiner = new MaterialCombiner(clone, true);
            GameObject cobbineObject = materialCombiner.CombineMaterial(matObj);
            cobbineObject.transform.parent = resObj.transform;
            cobbineObject.name = "PlayerModel";

            if (layer != ELayer.Default)
                LayerMgr.Get.SetLayer(cobbineObject, layer);

            if (layer != ELayer.Player) {
                //resObj.AddComponent<SelectEvent>();
                //resObj.AddComponent<SpinWithMouse>();
            }

            combineInstances.Clear();
            materials.Clear();
            bones.Clear();
            Destroy(clone);

            InitAnimator(resObj, bodyType, EAnimatorType.TalkControl);
        }
	}

    private void LoadAvatar(GameObject obj, int bodyType, TAvatar avatars, ELayer layer) {
        mBodyType = bodyType;
        mAvatars = avatars;
        StartCoroutine (AsyncSetAvatar (obj, bodyType, avatars, layer));
    }

    private void InitAnimator(GameObject obj, int bodyNumber, EAnimatorType animatorType)
    {
        Animator aniControl = obj.AddComponent<Animator>();

        aniControl.applyRootMotion = false;
        aniControl.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        aniControl.runtimeAnimatorController = loadController(bodyNumber, animatorType);
        aniControl.parameters.Initialize();
        aniControl.applyRootMotion = false;

        mAnimatorType = animatorType;
    }
}
