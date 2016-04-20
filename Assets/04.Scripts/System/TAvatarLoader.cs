using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using ProMaterialCombiner;

public struct TLoadParameter {
    public ELayer Layer;
    public EAnimatorType AnimatorType;
    public string Name;
    public bool AsyncLoad;
    public bool Combine;
    public bool AddSpin;
    public bool AddEvent;
    public bool AddDummyBall;

    public TLoadParameter(ELayer layer, string name = "", bool combine = false, bool addSpin = false, bool addEvent = false, bool addDummyBall = false, bool asyncLoad = true, EAnimatorType animatorType = EAnimatorType.TalkControl) {
        Layer = layer;
        AnimatorType = animatorType;
        Name = name;
        AsyncLoad = asyncLoad;
        Combine = combine;
        AddSpin = addSpin;
        AddEvent = addEvent;
        AddDummyBall = addDummyBall;
    }
}

public class TAvatarLoader : MonoBehaviour {
    public int mBodyType = -1;
    public TAvatar mAvatars;
    public EAnimatorType mAnimatorType = EAnimatorType.None;
    public string mAnimationName = "";
    public Color32 mMaterialColor = Color.white;
    private SkinnedMeshRenderer mSkinnedMeshRenderer = null;
    private Animator aniController = null;
    private GameObject playerShadow;
    private List<GameObject> dressList = new List<GameObject>();
  
	private static string[] avatarPartStr = {"B", "H", "M", "C", "P", "S", "A", "Z"};
    private const string bip01Text = "Bip01";
    private const string avatarText = "PlayerModel";
	private static Material materialSource;
    private static GameObject playerShadowRes;
	private static Dictionary<string, GameObject> modelCache = new Dictionary<string, GameObject>();
	private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    private static Dictionary<string, RuntimeAnimatorController> controllorCache = new Dictionary<string, RuntimeAnimatorController>();
	
    public static TAvatarLoader Load(int bodyType, TAvatar avatars, ref GameObject avatarObj, GameObject anchorObj, TLoadParameter param) {
        TAvatarLoader avatarLoader = null;
        if (!avatarObj) {
            avatarObj = new GameObject(param.Name == "" ? "PlayerModel" : param.Name);
            LayerMgr.Get.SetLayer(avatarObj, param.Layer);

            if (param.AddEvent)
                avatarObj.AddComponent<SelectEvent>();

            if (param.AddSpin)
                avatarObj.AddComponent<SpinWithMouse>();

            if (param.AddSpin || param.AddEvent || param.Layer == ELayer.Player)
                initCapsuleCollider(ref avatarObj, bodyType);
            
            if (anchorObj) {
                avatarObj.transform.parent = anchorObj.transform;
                avatarObj.transform.localPosition = Vector3.zero;
                avatarObj.transform.localScale = Vector3.one;
                avatarObj.transform.localRotation = Quaternion.identity;
            }

            avatarLoader = avatarObj.AddComponent<TAvatarLoader>();
        } else
            avatarLoader = avatarObj.GetComponent<TAvatarLoader>();

        if (avatarLoader != null)
            avatarLoader.loadAvatar(avatarObj, bodyType, avatars, param.Layer, param.AnimatorType, param.Combine, param.AddDummyBall, param.AsyncLoad);

        return avatarLoader;
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
        mAnimationName  = "";
        clearLoaded();

        if (playerShadow)
            Destroy(playerShadow);
        
        Destroy(gameObject);
    }

    private void clearLoaded() {
        //mSkinnedMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (mSkinnedMeshRenderer) {
            Material[] mats = mSkinnedMeshRenderer.materials;
            for (int j = 0; j < mats.Length; j++) {
                Destroy(mats[j]);
                mats[j] = null;
            }

            mSkinnedMeshRenderer.materials = new Material[0];
        }

        clearDress();

        if (aniController != null) 
            aniController.runtimeAnimatorController = null;

        Transform t = gameObject.transform.Find(avatarText);
        if (t != null)
            Destroy(t.gameObject);

        t = gameObject.transform.FindChild(bip01Text);
        if (t != null) 
            Destroy(t.gameObject);
    }

    private void clearDress() {
        for (int i = 0; i < dressList.Count; i++) {
            MeshRenderer mr = dressList[i].GetComponent<MeshRenderer>();
            if (mr != null && mr.material != null) 
                Destroy(mr.material);

            Destroy(dressList[i]);
        }

        dressList.Clear();
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

    private IEnumerator AsyncSetAvatar(GameObject parentObj, int bodyType, TAvatar avatars, ELayer layer, EAnimatorType animatorType, bool combine, bool addDummyBall) {
        yield return new WaitForEndOfFrame();

        if (avatars.Body == 0)
            avatars.Body = 1;
        
		int[] avatarIndex = new int[] { avatars.Body, avatars.Hair, avatars.MHandDress, avatars.Cloth, avatars.Pants, avatars.Shoes, avatars.AHeadDress, avatars.ZBackEquip };
		string path = "";
        Material matObj = materialSource;
        GameObject avatarModel = null;
        Texture2D texture = null;

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Material> materials = new List<Material>();
        List<Transform> bones = new List<Transform>();
        Transform[] bodyHips = null;

        for (int i = 0; i < avatarIndex.Length; i++) {
			if (avatarIndex[i] > 0) {
				int avatarBody = avatarIndex[i] / 1000;
				int avatarBodyTexture = avatarIndex[i] % 1000;
                string texturePath = "";
                if (i == 0) { //body
                    path = string.Format("Character/PlayerModel_{0}/Model/PlayerModel_{1}", bodyType, bodyType);
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, "B", "0", avatarBodyTexture);
                } else
                if (i < 6) { //part of avatar
                    path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", bodyType, avatarPartStr[i], avatarBody);
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, avatarPartStr[i], avatarBody, avatarBodyTexture);
                } else { //glasses or hat
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

                        //add body bip to parent
                        if (i == 0) {
                            Transform avatarBip = parentObj.transform.FindChild(bip01Text);
                            if (avatarBip == null)
                                avatarBip = avatarPartObj.transform.FindChild(bip01Text);

                            if (avatarBip != null) {
                                avatarBip.parent = parentObj.transform;
                                avatarBip.localPosition = Vector3.zero;
                                avatarBip.localScale = Vector3.one;
                                bodyHips = avatarBip.gameObject.GetComponentsInChildren<Transform>();
                            }

                            if (addDummyBall) {
                                Transform t = parentObj.transform.FindChild("DummyBall");
                                if (t == null) {
                                    Transform t1 = avatarPartObj.transform.FindChild("DummyBall");
                                    if (t1 != null)
                                        t1.parent = parentObj.transform;
                                }
                            }
                        }

                        if (i < 6) {
                            SkinnedMeshRenderer avatarSMR = avatarPartObj.GetComponentInChildren<SkinnedMeshRenderer>();
                            if (avatarSMR != null) {
                                matObj.SetTexture ("_MainTex", texture);
                                avatarSMR.material = matObj;

                                CombineInstance ci = new CombineInstance();
                                ci.mesh = avatarSMR.sharedMesh;
                                combineInstances.Add(ci);
                                materials.AddRange(avatarSMR.materials);

                                if (bodyHips != null) {
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
                            }

                            Destroy(avatarPartObj);
                            avatarPartObj = null;
                        } else {
                            dressList.Add(avatarPartObj);
                            if (i == 6)
                                path = "Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead";
                            else
                                path = "Bip01/Bip01 Spine/Bip01 Spine1/DummyBack";
                                    
                            Transform t = parentObj.transform.FindChild(path);
                            if (t != null) {
                                MeshRenderer mr = avatarPartObj.GetComponent<MeshRenderer>();
                                if (mr != null) {
                                    Material matObj1 = Instantiate(materialSource);
                                    matObj1.SetTexture ("_MainTex", texture);
                                    mr.material = matObj1;
                                }

                                avatarPartObj.transform.parent = t;
                                avatarPartObj.transform.localPosition = Vector3.zero;
                                avatarPartObj.transform.localEulerAngles = Vector3.zero;
                                avatarPartObj.transform.localScale = Vector3.one;
                                LayerMgr.Get.SetLayer(avatarPartObj, layer);
                            }
                        }
					}
				}
            }
        }

        if (combineInstances.Count > 0) {
            GameObject clone = null;
            Transform dt = parentObj.transform.Find(avatarText);

            if (dt != null)
                clone = dt.gameObject;
            else
                clone = new GameObject();

            mSkinnedMeshRenderer = clone.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (mSkinnedMeshRenderer == null) 
                mSkinnedMeshRenderer = clone.gameObject.AddComponent<SkinnedMeshRenderer>();

            if (mSkinnedMeshRenderer.sharedMesh == null) {
                mSkinnedMeshRenderer.sharedMesh = new Mesh();
                mSkinnedMeshRenderer.gameObject.isStatic = true;
                mSkinnedMeshRenderer.receiveShadows = false;
                mSkinnedMeshRenderer.useLightProbes = false;
            }

            mSkinnedMeshRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
            mSkinnedMeshRenderer.materials = materials.ToArray();
            mSkinnedMeshRenderer.bones = bones.ToArray();
            if (mMaterialColor != Color.white)
                mSkinnedMeshRenderer.material.color = mMaterialColor;

            if (!combine)
            {
                clone.name = avatarText;
                clone.transform.parent = parentObj.transform;
                clone.transform.localPosition = Vector3.zero;
                clone.transform.localScale = Vector3.one;
                LayerMgr.Get.SetLayer(clone, layer);
            }
            else {
                MaterialCombiner materialCombiner = new MaterialCombiner(clone, true);
                GameObject cobbineObject = materialCombiner.CombineMaterial(matObj);
                cobbineObject.name = avatarText;
                cobbineObject.transform.parent = parentObj.transform;
                cobbineObject.transform.localPosition = Vector3.zero;
                cobbineObject.transform.localScale = Vector3.one;
                LayerMgr.Get.SetLayer(cobbineObject, layer);

                Destroy(clone);
                clone = null;
            }

            combineInstances.Clear();
            materials.Clear();
            bones.Clear();

            initAnimator(parentObj, bodyType, EAnimatorType.TalkControl);
            mBodyType = bodyType;
            mAvatars = avatars;
        }
	}

    private void SetAvatar(GameObject parentObj, int bodyType, TAvatar avatars, ELayer layer, EAnimatorType animatorType, bool combine, bool addDummyBall) {
        if (avatars.Body == 0)
            avatars.Body = 1;

        int[] avatarIndex = new int[] { avatars.Body, avatars.Hair, avatars.MHandDress, avatars.Cloth, avatars.Pants, avatars.Shoes, avatars.AHeadDress, avatars.ZBackEquip };
        string path = "";
        Material matObj = materialSource;
        GameObject avatarModel = null;
        Texture2D texture = null;

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Material> materials = new List<Material>();
        List<Transform> bones = new List<Transform>();
        Transform[] bodyHips = null;

        for (int i = 0; i < avatarIndex.Length; i++) {
            if (avatarIndex[i] > 0) {
                int avatarBody = avatarIndex[i] / 1000;
                int avatarBodyTexture = avatarIndex[i] % 1000;
                string texturePath = "";
                if (i == 0) { //body
                    path = string.Format("Character/PlayerModel_{0}/Model/PlayerModel_{1}", bodyType, bodyType);
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, "B", "0", avatarBodyTexture);
                } else
                if (i < 6) { //part of avatar
                    path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", bodyType, avatarPartStr[i], avatarBody);
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, avatarPartStr[i], avatarBody, avatarBodyTexture);
                } else { //glasses or hat
                    path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", "3", avatarPartStr[i], avatarBody);
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", avatarPartStr[i], avatarBody, avatarBodyTexture);
                }

                if (modelCache.ContainsKey(path))
                    avatarModel = modelCache[path];
                else {
                    avatarModel = Resources.Load(path) as GameObject;
                    if (!modelCache.ContainsKey(path))
                        modelCache.Add(path, avatarModel);
                }

                if (avatarModel != null) {
                    texture = null;
                    if (textureCache.ContainsKey(texturePath))
                        texture = textureCache[texturePath];
                    else {
                        texture = Resources.Load(texturePath) as Texture2D;
                        if (!textureCache.ContainsKey(texturePath))
                            textureCache.Add (texturePath, texture);
                    }

                    if (texture != null) {
                        GameObject avatarPartObj = Instantiate(avatarModel) as GameObject;

                        //add body bip to parent
                        if (i == 0) {
                            Transform avatarBip = parentObj.transform.FindChild(bip01Text);
                            if (avatarBip == null)
                                avatarBip = avatarPartObj.transform.FindChild(bip01Text);

                            if (avatarBip != null) {
                                avatarBip.parent = parentObj.transform;
                                avatarBip.localPosition = Vector3.zero;
                                avatarBip.localScale = Vector3.one;
                                bodyHips = avatarBip.gameObject.GetComponentsInChildren<Transform>();
                            }

                            if (addDummyBall) {
                                Transform t = parentObj.transform.FindChild("DummyBall");
                                if (t == null) {
                                    Transform t1 = avatarPartObj.transform.FindChild("DummyBall");
                                    if (t1 != null)
                                        t1.parent = parentObj.transform;
                                }
                            }
                        }

                        if (i < 6) {
                            SkinnedMeshRenderer avatarSMR = avatarPartObj.GetComponentInChildren<SkinnedMeshRenderer>();
                            if (avatarSMR != null) {
                                matObj.SetTexture ("_MainTex", texture);
                                avatarSMR.material = matObj;

                                CombineInstance ci = new CombineInstance();
                                ci.mesh = avatarSMR.sharedMesh;
                                combineInstances.Add(ci);
                                materials.AddRange(avatarSMR.materials);

                                if (bodyHips != null) {
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
                            }

                            Destroy(avatarPartObj);
                            avatarPartObj = null;
                        } else {
                            dressList.Add(avatarPartObj);
                            if (i == 6)
                                path = "Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead";
                            else
                                path = "Bip01/Bip01 Spine/Bip01 Spine1/DummyBack";

                            Transform t = parentObj.transform.FindChild(path);
                            if (t != null) {
                                MeshRenderer mr = avatarPartObj.GetComponent<MeshRenderer>();
                                if (mr != null) {
                                    Material matObj1 = Instantiate(materialSource);
                                    matObj1.SetTexture ("_MainTex", texture);
                                    mr.material = matObj1;
                                }

                                avatarPartObj.transform.parent = t;
                                avatarPartObj.transform.localPosition = Vector3.zero;
                                avatarPartObj.transform.localEulerAngles = Vector3.zero;
                                avatarPartObj.transform.localScale = Vector3.one;
                                LayerMgr.Get.SetLayer(avatarPartObj, layer);
                            }
                        }
                    }
                }
            }
        }

        if (combineInstances.Count > 0) {
            GameObject clone = null;
            Transform dt = parentObj.transform.Find(avatarText);

            if (dt != null)
                clone = dt.gameObject;
            else
                clone = new GameObject();

            mSkinnedMeshRenderer = clone.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (mSkinnedMeshRenderer == null) 
                mSkinnedMeshRenderer = clone.gameObject.AddComponent<SkinnedMeshRenderer>();

            if (mSkinnedMeshRenderer.sharedMesh == null) {
                mSkinnedMeshRenderer.sharedMesh = new Mesh();
                mSkinnedMeshRenderer.gameObject.isStatic = true;
                mSkinnedMeshRenderer.receiveShadows = false;
                mSkinnedMeshRenderer.useLightProbes = false;
            }

            mSkinnedMeshRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
            mSkinnedMeshRenderer.materials = materials.ToArray();
            mSkinnedMeshRenderer.bones = bones.ToArray();

            if (!combine)
            {
                clone.name = avatarText;
                clone.transform.parent = parentObj.transform;
                clone.transform.localPosition = Vector3.zero;
                clone.transform.localScale = Vector3.one;
                LayerMgr.Get.SetLayer(clone, layer);
            }
            else {
                MaterialCombiner materialCombiner = new MaterialCombiner(clone, true);
                GameObject cobbineObject = materialCombiner.CombineMaterial(matObj);
                cobbineObject.name = avatarText;
                cobbineObject.transform.parent = parentObj.transform;
                cobbineObject.transform.localPosition = Vector3.zero;
                cobbineObject.transform.localScale = Vector3.one;
                LayerMgr.Get.SetLayer(cobbineObject, layer);

                Destroy(clone);
                clone = null;
            }

            combineInstances.Clear();
            materials.Clear();
            bones.Clear();

            initAnimator(parentObj, bodyType, animatorType);
            mBodyType = bodyType;
            mAvatars = avatars;
        }
    }

    private void loadAvatar(GameObject parentObj, int bodyType, TAvatar avatars, ELayer layer, EAnimatorType animatorType, bool combine, bool addDummyBall, bool asyncLoad) {
        if (mBodyType >= 0) {
            if (mBodyType != bodyType) 
                clearLoaded();
            else
                clearDress();
        }

        if (layer == ELayer.Player && GameData.Setting.Quality == 1) {
            if (!playerShadowRes)
                playerShadowRes = Resources.Load("Prefab/Player/PlayerShadow") as GameObject;

            playerShadow = Instantiate(playerShadowRes) as GameObject;
            playerShadow.transform.parent = parentObj.transform;
        }

        if (asyncLoad)
            StartCoroutine (AsyncSetAvatar (parentObj, bodyType, avatars, layer, animatorType, combine, addDummyBall));
        else
            SetAvatar (parentObj, bodyType, avatars, layer, animatorType, combine, addDummyBall);
    }

    private void initAnimator(GameObject obj, int bodyType, EAnimatorType animatorType)
    {
        bool flag = aniController == null;
        if (flag)
            aniController = obj.AddComponent<Animator>();

        aniController.applyRootMotion = false;
        aniController.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        if (aniController.runtimeAnimatorController == null || mAnimatorType != animatorType || mBodyType != bodyType) {
            aniController.runtimeAnimatorController = loadController(bodyType, animatorType);
            aniController.parameters.Initialize();
        }

        if (flag && mAnimationName != "")
            aniController.SetTrigger(mAnimationName);

        mAnimatorType = animatorType;
    }

    private static void initCapsuleCollider(ref GameObject obj, int bodyType)
    {
        CapsuleCollider collider = obj.GetComponent<CapsuleCollider>();
        if (collider == null)
            collider = obj.AddComponent<CapsuleCollider>();

        switch (bodyType)
        {
            case 1:
                collider.radius = 0.9f;
                collider.height = 3.2f;
                break;

            case 2:
                collider.radius = 0.8f;
                collider.height = 3f;
                break;
            default: 
                collider.radius = 1;
                collider.height = 3.5f;
                break;
        }

        collider.center = new Vector3(0, collider.height / 2f, 0);
    }

    public void SetTrigger(string name) {
        mAnimationName = name;
        if (aniController != null) 
            aniController.SetTrigger(name);
    }

    public void Play(string name) {
        mAnimationName = name;
        if (aniController != null) 
            aniController.Play(name);
    }

    public Color32 MaterialColor {
        set {
            mMaterialColor = value;
            if (mSkinnedMeshRenderer != null && mSkinnedMeshRenderer.materials != null) {
                for (int i = 0; i < mSkinnedMeshRenderer.materials.Length; i++)
                    mSkinnedMeshRenderer.materials[i].color = value;
            }
        }
    }
}
