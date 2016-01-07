using System;
using System.Collections.Generic;
using AI;
using GameEnum;
using GameStruct;
using ProMaterialCombiner;
using UnityEngine;

public enum EAnimatorType
{
    AnimationControl,
    AvatarControl,
    ShowControl,
    TalkControl
}

public class ModelManager : KnightSingleton<ModelManager>
{
    public const string Name = "ModelManager";

    private GameObject DefPointObject = null;
    public GameObject PlayerInfoModel = null;
    public AniCurve AnimatorCurveManager;

    private Material materialSource;
    private Dictionary<string, GameObject> bodyCache = new Dictionary<string, GameObject>();
    private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    private Dictionary<string, RuntimeAnimatorController> controllorCache = new Dictionary<string, RuntimeAnimatorController>();

    void Awake()
    {
        PlayerInfoModel = new GameObject();
        PlayerInfoModel.name = "PlayerInfoModel";

        materialSource = Resources.Load("Character/Materials/Material_0") as Material;
        DefPointObject = Resources.Load("Character/Component/DefPoint") as GameObject;
        GameObject cloneObj = Instantiate(Resources.Load("Character/Component/AnimatorCurve")) as GameObject;
        AnimatorCurveManager = cloneObj.GetComponent<AniCurve>();
    }

    public void PreloadResource(TAvatar attr, int bodyType)
    {
        string bodyNumber = bodyType.ToString();
        string mainBody = string.Format("PlayerModel_{0}", bodyNumber);
        string[] avatarPart = new string[]{ mainBody, "C", "H", "M", "P", "S", "A", "Z" };
        int[] avatarIndex = new int[] { attr.Body, attr.Cloth, attr.Hair, attr.MHandDress, attr.Pants, attr.Shoes, attr.AHeadDress, attr.ZBackEquip };
        string path;
        string texturePath;
        for (int i = 0; i < avatarIndex.Length; i++)
        {
            if (avatarIndex[i] > 0)
            {
                int avatarBody = avatarIndex[i] / 1000;
                int avatarBodyTexture = avatarIndex[i] % 1000;
                if (i == 0)
                {
                    path = string.Format("Character/PlayerModel_{0}/Model/{1}", bodyNumber, mainBody); 
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyNumber, "B", "0", avatarBodyTexture);
                }
                else if (i < 6)
                {
                    path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", bodyNumber, avatarPart[i], avatarBody);
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyNumber, avatarPart[i], avatarBody, avatarBodyTexture);
                }
                else
                {//it maybe A or Z
                    path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", "3", avatarPart[i], avatarBody);
                    texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", avatarPart[i], avatarBody, avatarBodyTexture);
                }
				
                GameObject resObj = loadBody(path);
                if (resObj)
                    loadTexture(texturePath);
            }
        }
    }

    public void PreloadAnimator()
    {
        //load animator
        string path;

        for (int i = 0; i < 3; i++)
        {
            path = string.Format("Character/PlayerModel_{0}/{1}", i, EAnimatorType.AnimationControl.ToString());
            loadController(path, i, EAnimatorType.AnimationControl);
            path = string.Format("Character/PlayerModel_{0}/{1}", i, EAnimatorType.ShowControl.ToString());
            loadController(path, i, EAnimatorType.ShowControl);
        }
    }

    public void LoadAllSelectPlayer(ref int[] id)
    {
        for (int i = 0; i < id.Length; i++)
            if (GameData.DPlayers.ContainsKey(id[i]))
            {
                TAvatar avatar = new TAvatar(id[i]);
                PreloadResource(avatar, GameData.DPlayers[id[i]].BodyType);
            }
    }

    private GameObject loadBody(string path)
    {
        if (bodyCache.ContainsKey(path))
            return bodyCache[path];
        else
        {
            GameObject obj = Resources.Load(path) as GameObject;
            if (obj)
            {
                bodyCache.Add(path, obj);
                return obj;
            }
            else
            {
                //download form server
                return null;
            }
        }
    }

    private Texture2D loadTexture(string path)
    {
        if (textureCache.ContainsKey(path))
        {
            return textureCache[path];
        }
        else
        {
            Texture2D obj = Resources.Load(path) as Texture2D;
            if (obj)
            {
                textureCache.Add(path, obj);
                return obj;
            }
            else 
            {
                //download form server
                return null;
            }
        }
    }

    private string getAnimatorKey(int bodyNumber, EAnimatorType type)
    {
        string key = string.Empty;
        switch (bodyNumber)
        {
            case 0:
                key = string.Format("{0}Center", type);
                break;
            case 1:
                key = string.Format("{0}Forward", type);
                break;
            case 2:
                key = string.Format("{0}Defender", type);
                break;
        }

        return key;
    }


    private RuntimeAnimatorController loadController(string path, int bodyNumber, EAnimatorType type)
    {
        string key = getAnimatorKey(bodyNumber, type);
        if (key != string.Empty)
        {
            if (controllorCache.ContainsKey(key))
            {
                return controllorCache[key];
            }
            else
            {
                RuntimeAnimatorController obj = Resources.Load(path) as RuntimeAnimatorController;
                if (obj)
                {
                    controllorCache.Add(key, obj);
                    return obj;
                }
                else
                {
                    //download form server
                    return null;
                }
            }
        }
        return null;
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
    public PlayerBehaviour CreateGamePlayer(int teamIndex, ETeamKind team, Vector3 bornPos, TPlayer player, GameObject res = null)
    {
        if (GameData.DPlayers.ContainsKey(player.ID))
        {
            if (res == null)
                res = new GameObject();

            if (GameStart.Get.TestModel != EModelTest.None && GameStart.Get.TestMode != EGameTest.None)
                player.BodyType = (int)GameStart.Get.TestModel;

            SetAvatar(ref res, player.Avatar, player.BodyType, EAnimatorType.AnimationControl); //EAnimatorType.ShowControl); 
			InitRigbody(res);
			ETimerKind timeKey;
			if (team == ETeamKind.Self)
				timeKey = (ETimerKind)Enum.Parse(typeof(ETimerKind), string.Format("Self{0}", teamIndex));
			else
				timeKey = (ETimerKind)Enum.Parse(typeof(ETimerKind), string.Format("Npc{0}", teamIndex));
						
			TimerMgr.Get.SetTimerKey(timeKey, ref res);

            res.transform.parent = PlayerInfoModel.transform;
            res.transform.localPosition = bornPos;

            PlayerBehaviour playerBehaviour = res.AddComponent<PlayerBehaviour>();

            playerBehaviour.Team = team;
            playerBehaviour.MoveIndex = -1;
            playerBehaviour.Attribute = player;
            playerBehaviour.Index = (EPlayerPostion)teamIndex;
			playerBehaviour.TimerKind = timeKey;
           
            if ((EPlayerPostion)teamIndex == EPlayerPostion.C)
                playerBehaviour.Postion = EPlayerPostion.C;
            else if ((EPlayerPostion)teamIndex == EPlayerPostion.F)
                playerBehaviour.Postion = EPlayerPostion.F;
            else if ((EPlayerPostion)teamIndex == EPlayerPostion.G)
                playerBehaviour.Postion = EPlayerPostion.G;


            playerBehaviour.InitTrigger(DefPointObject);
            playerBehaviour.InitDoubleClick();
            playerBehaviour.InitAttr();
            res.name = team.ToString() + teamIndex.ToString();

            if (team == ETeamKind.Npc)
                res.transform.localEulerAngles = new Vector3(0, 180, 0);

            playerBehaviour.AI = res.AddComponent<PlayerAI>();

            return playerBehaviour;
        }
        else
        {
            Debug.LogError("Error : playerId is not exist in great player");
            return null;
        }
						
		
    }

    public void SetAvatarTexture(GameObject Player, GameStruct.TAvatar Attr, int bodyType, int BodyKind, int avatarNo)
    {
        int ModelPart = (int)(avatarNo / 1000);
        int TexturePart = avatarNo % 1000;
        SetAvatarTexture(Player, Attr, bodyType, BodyKind, ModelPart, TexturePart);
    }

    public string GetAvatarPartString(int index)
    {
        string result = string.Empty;

        switch (index)
        {
            case 0:
                result = "B";
                break;
            case 1:
                result = "H";
                break;
            case 2:
                result = "M";
                break;
            case 3:
                result = "C";
                break;
            case 4:
                result = "P";
                break;
            case 5:
                result = "S";
                break;
            case 6:
                result = "A";
                break;
            case 7:
                result = "Z";
                break;
        }

        return result;
    }

    public void SetAvatarTexture(GameObject Player, GameStruct.TAvatar Attr, int bodyType, int BodyPart, int ModelPart, int TexturePart)
    {
        if (Player)
        {
            string bodyNumber = bodyType.ToString();
            string mainBody = "PlayerModel";
            string strPart = GetAvatarPartString(BodyPart);

            if (strPart == string.Empty)
            {
                Debug.LogError("Avatar.BodyPart can't found");
                return;
            }

            if (BodyPart < 6)
            {
                Transform t = Player.transform.FindChild(mainBody);
                if (t != null)
                {
                    GameObject obj = t.gameObject;

                    if (obj)
                    {
                        string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyNumber, strPart, ModelPart, TexturePart);
                        Texture texture = loadTexture(path);

                        Renderer renderers = obj.GetComponent<Renderer>();
                        Material[] materials = renderers.materials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            if (materials[i].name.Equals(strPart + " (Instance)"))
                            {
                                if (texture)
                                    materials[i].mainTexture = texture;

                                break;
                            }
                        }
                    }
                }
            }
            else if (BodyPart == 6)
            {
                string bodyPath = string.Format("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead/3_{0}_{1}(Clone)", strPart, ModelPart);
                Transform t = Player.transform.Find(bodyPath);
                if (t != null)
                {
                    GameObject obj = t.gameObject;
                    if (obj)
                    {
                        string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", strPart, ModelPart, TexturePart);
                        Texture texture = loadTexture(path);
                        Renderer renderers = obj.GetComponent<Renderer>();
                        Material[] materials = renderers.materials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            if (materials[i].name.Equals(strPart))
                            {
                                if (texture)
                                    materials[i].mainTexture = texture;

                                break;
                            }
                        }
                    }
                }
            }
            else if (BodyPart == 7)
            {
                string bodyPath = string.Format("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack/3_{0}_{1}(Clone)", strPart, ModelPart);
                GameObject obj = Player.transform.Find(bodyPath).gameObject;
                if (obj)
                {
                    string path = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", strPart, ModelPart, TexturePart);
                    Texture texture = loadTexture(path);

                    Renderer renderers = obj.GetComponent<Renderer>();
                    Material[] materials = renderers.materials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i].name.Equals(strPart))
                        {
                            if (texture)
                                materials[i].mainTexture = texture;

                            break;
                        }
                    }
                }
            }
        }
    }

    public void SetAvatarByItem(ref GameObject result, TItem[] items, int bodyType, EAnimatorType animatorType, bool combine = true, bool Reset = false)
    {
        if (items.Length != 8)
        {
            Debug.LogError("Error : Item's Lenght");
            return;
        }

        TAvatar attr = new TAvatar();
        GameFunction.ItemIdTranslateAvatar(ref attr, items);
        SetAvatar(ref result, attr, bodyType, animatorType, combine, Reset);
    }

    public GameObject SetAvatar(ref GameObject result, TAvatar attr, int bodyType, EAnimatorType animatorType, bool combine = true, bool Reset = false)
    {
        try
        {
            Transform parent = result.transform.parent;
            Vector3 localposition = result.transform.localPosition;
            string mainBody = string.Format("PlayerModel_{0}", bodyType);
            int[] avatarIndex = new int[] { attr.Body, attr.Hair, attr.MHandDress, attr.Cloth, attr.Pants, attr.Shoes, attr.AHeadDress, attr.ZBackEquip };

            if (Reset)
            {
                Destroy(result);			
                result = new GameObject();
            }

            GameObject dummyBall = null;
            GameObject bipGO = null;
			
            Transform[] hips;
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            List<Material> materials = new List<Material>();
            List<Transform> bones = new List<Transform>();

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

            for (int i = 0; i < avatarIndex.Length; i++)
            {
                if (avatarIndex[i] > 0)
                {
                    int avatarBody = avatarIndex[i] / 1000;
                    int avatarBodyTexture = avatarIndex[i] % 1000;
                    string avatarPart = GetAvatarPartString(i);

                    if (i == 0)
                    {
                        path = string.Format("Character/PlayerModel_{0}/Model/{1}", bodyType, mainBody); 
                        texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, "B", "0", avatarBodyTexture);
                    }
                    else if (i < 6)
                    {

                        path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", bodyType, avatarPart, avatarBody);
                        texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", bodyType, avatarPart, avatarBody, avatarBodyTexture);
                    }
                    else
                    {//it maybe A or Z
                        path = string.Format("Character/PlayerModel_{0}/Model/{0}_{1}_{2}", "3", avatarPart, avatarBody);
                        texturePath = string.Format("Character/PlayerModel_{0}/Texture/{0}_{1}_{2}_{3}", "3", avatarPart, avatarBody, avatarBodyTexture);
                    }
					
                    GameObject resObj = loadBody(path);
                    if (resObj)
                    {
                        try
                        {
                            GameObject avatarPartGO = Instantiate(resObj) as GameObject;
                            Texture texture = loadTexture(texturePath);
                            matObj.SetTexture("_MainTex", texture);
							
                            if (i < 6)
                            {
                                Transform tBipGo = result.transform.FindChild("Bip01");
                                if (tBipGo == null)
                                {
                                    if (bipGO == null)
                                    {
                                        bipGO = avatarPartGO.transform.FindChild("Bip01").gameObject;
										
                                        if (bipGO)
                                            bipGO.transform.parent = result.transform;
                                    }
                                }
                                else
                                    bipGO = tBipGo.gameObject;
								
                                if (dummyBall == null)
                                {
                                    Transform t = result.transform.FindChild("DummyBall");
                                    if (t == null)
                                    {
                                        if (dummyBall == null)
                                        {
                                            Transform t1 = avatarPartGO.transform.FindChild("DummyBall");
                                            if (t1 != null)
                                                dummyBall = t1.gameObject;
                                        }
                                    }
                                    else
                                        dummyBall = t.gameObject;

                                    dummyBall.transform.parent = result.transform;
                                }
								
                                hips = bipGO.GetComponentsInChildren<Transform>();
                                if (hips.Length > 0)
                                {
                                    CombineInstance ci = new CombineInstance();
                                    SkinnedMeshRenderer smr = avatarPartGO.GetComponentInChildren<SkinnedMeshRenderer>();
                                    if (smr != null)
                                    {
                                        smr.material = matObj;
                                        smr.material.name = avatarPart;
                                        ci.mesh = smr.sharedMesh;
                                    }

                                    combineInstances.Add(ci);

                                    //sort new material
                                    materials.AddRange(smr.materials);
									
                                    //get same bip to create new bones  
                                    foreach (Transform bone in smr.bones)
                                    {
                                        foreach (Transform hip in hips)
                                        {
                                            if (hip.name != bone.name)
                                                continue;
											
                                            bones.Add(hip);
                                            break;
                                        }
                                    }
                                }

                                Destroy(avatarPartGO);
                                avatarPartGO = null;
                            }
                            else if (i == 6)
                            {
                                Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead");
                                Material matObj1 = Instantiate(materialSource);
                                if (t != null && matObj1 != null)
                                {
                                    for (int j = 0; j < t.childCount; j++)
                                        Destroy(t.GetChild(j).gameObject);

                                    MeshRenderer mr = avatarPartGO.GetComponent<MeshRenderer>();
                                    if (mr != null)
                                        mr.material = matObj1;

                                    avatarPartGO.transform.parent = t;
                                    avatarPartGO.transform.localPosition = Vector3.zero;
                                    avatarPartGO.transform.localEulerAngles = Vector3.zero;
                                    avatarPartGO.transform.localScale = Vector3.one;
                                }
                            }
                            else if (i == 7)
                            {
                                Transform t = result.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack");
                                Material matObj1 = Instantiate(materialSource);
                                if (t != null && matObj1 != null)
                                    for (int j = 0; j < t.childCount; j++)
                                        Destroy(t.GetChild(j).gameObject);

                                MeshRenderer mr = avatarPartGO.GetComponent<MeshRenderer>();
                                if (mr != null)
                                    mr.material = matObj1;

                                avatarPartGO.transform.parent = t;
                                avatarPartGO.transform.localPosition = Vector3.zero;
                                avatarPartGO.transform.localEulerAngles = Vector3.zero;
                                avatarPartGO.transform.localScale = Vector3.one;
                            }
                        }
                        catch (UnityException e)
                        {
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
			
            resultSmr.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
            resultSmr.bones = bones.ToArray();
            resultSmr.materials = materials.ToArray();
            resultSmr.gameObject.isStatic = true;
            resultSmr.receiveShadows = false;
            resultSmr.useLightProbes = false;

            GameObject cobbineObject = null;
            if (!combine)
            {
                clone.transform.parent = result.transform;
                LayerMgr.Get.SetLayer(clone, ELayer.Player);
                clone.name = "PlayerModel";
            }
            else
            {
                MaterialCombiner materialCombiner = new MaterialCombiner(clone, true);
                cobbineObject = materialCombiner.CombineMaterial(matObj);
                cobbineObject.transform.parent = result.transform;
                LayerMgr.Get.SetLayer(cobbineObject, ELayer.Player);
                cobbineObject.name = "PlayerModel";

                Destroy(clone);
                clone = null;
            }

            InitCapsuleCollider(result, bodyType);
            InitAnimator(result, bodyType, animatorType);
            result.transform.parent = parent;
            result.transform.localPosition = localposition;

            if(!combine)
                return clone;
            return cobbineObject;
        }
        catch (UnityException e)
        {
            Debug.Log(e.ToString());
            return null;
        }

        return null;
    }

    private void InitCapsuleCollider(GameObject obj, int bodyType)
    {
        //collider
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

    private void InitAnimator(GameObject obj, int bodyNumber, EAnimatorType animatorType)
    {
        Animator aniControl = obj.GetComponent<Animator>();
        if (aniControl == null)
            aniControl = obj.AddComponent<Animator>();
        aniControl.applyRootMotion = false;
        aniControl.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        ChangeAnimator(ref aniControl, bodyNumber, animatorType);
    }

	private void InitRigbody(GameObject obj)
	{
		Rigidbody rig = obj.AddComponent<Rigidbody>();
		rig.mass = 0.1f;
		rig.drag = 10f;
		rig.freezeRotation = true;
	}

    public void ChangeAnimator(ref Animator ani, int bodyNumber, EAnimatorType type)
    {
        string path = string.Format("Character/PlayerModel_{0}/{1}", bodyNumber, type.ToString());
        ani.runtimeAnimatorController = loadController(path, bodyNumber, type);
        ani.parameters.Initialize();
        ani.applyRootMotion = false;
    }
}
