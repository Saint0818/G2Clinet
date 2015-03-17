using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelManager : MonoBehaviour {
	private GameObject DefPointObject = null;
	public static ModelManager Get;
	private GameObject PlayerModule = null; 
	public GameObject PlayerInfoModel = null;
	private const int avatartCount = 3;
//	private string[] avatarPartName = new string[]{"Body", "Cloth", "Shoes"};
//	private GameObject bipGO;
	public Dictionary<string, AnimationClip> AniData = new Dictionary<string, AnimationClip> ();

	public static void Init(){
		GameObject gobj = new GameObject(typeof(ModelManager).Name);
		DontDestroyOnLoad(gobj);
		Get = gobj.AddComponent<ModelManager>();
	}

	void Awake(){
		AnimationClip[] ani = Resources.LoadAll<AnimationClip>("FBX/Animation");
		if (ani.Length > 0) {
			for(int i = 0; i < ani.Length; i++)
			{
				string keyname = ani[i].name.Replace(" (UnityEngine.AnimationClip)", "");
				if(!AniData.ContainsKey(keyname))
					AniData.Add(keyname, ani[i]);
			}
		}

		PlayerInfoModel = new GameObject();
		PlayerInfoModel.name = "PlayerInfoModel";
		UIPanel up = PlayerInfoModel.AddComponent<UIPanel>();
		up.depth = 2;

		PlayerModule = Resources.Load("Prefab/Player/PlayerModel_0") as GameObject;
		DefPointObject = Resources.Load("Character/Component/DefPoint") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public PlayerBehaviour CreatePlayer(GameObject Player, GameStruct.TPlayerAttribute Attr, int Index, TeamKind Team, Vector3 BornPos, Vector2 [] RunPosAy, MoveType MoveKind, GamePostion Postion){
		if (Player != null) {
			Destroy (Player);
		}
		GameObject Res = AddPlayer (Attr);
		Res.transform.parent = PlayerInfoModel.transform;
		Res.transform.localPosition = BornPos;
		GameObject DefPointCopy = Instantiate(DefPointObject) as GameObject;
		DefPointCopy.transform.parent = Res.transform;
		DefPointCopy.transform.localPosition = Vector3.zero;
		if(Team == TeamKind.Npc)
			Res.transform.localEulerAngles = new Vector3(0, 180, 0);
		PlayerBehaviour PB = Res.AddComponent<PlayerBehaviour>();
		PB.Team = Team;
		PB.MoveKind = MoveKind;
		PB.MoveIndex = -1;
		PB.Postion = Postion;
		PB.RunPosAy = RunPosAy;
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

//		GameStruct.TPlayerAttribute attr = new GameStruct.TPlayerAttribute();

//		if ((int)Team == 1) {
//			attr.Cloth = 1;
//			attr.Face = 1;
//			attr.Shoes = 1;
//			SetAvatar (ref Res, ref attr);
//		} 
//		else
//			SetAvatar (ref Res, ref attr);

		return PB;
	}

//	public void SetAvatar(ref GameObject playerModel, ref GameStruct.TPlayerAttribute attr) {
//		if (playerModel) {
//
//			GameObject[] avatarPart =  new GameObject[avatartCount];
//			int[] avatarIndex = new int[avatartCount];
//
//			avatarIndex[0] = attr.Face; 		//Body
//			avatarIndex[1] = attr.Cloth;  		//Cloth
//			avatarIndex[2] = attr.Shoes;		//Shoes
//
//			for(int i = 0; i < avatarPart.Length; i ++) {
//				avatarPart[i] = playerModel.transform.FindChild(avatarPartName[i]).gameObject;
//
//				if(avatarPart[i])
//				{
//					string path = "Character/PlayerModel_0/Texture/" + avatarPartName[i] + "_" + avatarIndex[i];
//					Texture2D texture = Resources.Load(path) as Texture2D;
//					if (texture) {
//						avatarPart[i].gameObject.GetComponent<Renderer>().material.mainTexture = texture;
//					}
//				}
//			}
//		}
//	}

	/// <summary>
	/// c:Clothes, h:Hair, m:HandEquipment, p:Pants, s:Shoes, a:Headdress, z:BackbackEquipment
	/// </summary>
	public GameObject AddPlayer(GameStruct.TPlayerAttribute attr)
	{
		string mainBody = string.Format("PlayerModel_{0}", attr.Body);
		string[] avatarPart = new string[]{mainBody, "C", "H", "M", "P", "S", "A", "Z"};
		int[] avatarIndex = new int[]{attr.Body, attr.Cloth, attr.Hair, attr.MHeadDress, attr.Pants, attr.Shoes, attr.AHead, attr.ZBackEquip};
		GameObject[] avatarPartGO = new GameObject[avatarIndex.Length];
		GameObject result = new GameObject();
		GameObject dummyBall = null;
		GameObject dummyHead = null;
		GameObject dummyBack = null;
		GameObject headDressObj = null;
		GameObject backEquipmentObj = null;

		GameObject bipGO = null;
		
		Transform[] hips;
		List<CombineInstance> combineInstances = new List<CombineInstance>();
		List<Material> materials = new List<Material>();
		List<Transform> bones = new List<Transform>();
		
		for (int i = 0; i < avatarIndex.Length; i++) {
			string path = string.Empty;
			string materialPath = string.Empty;
			
			if(i == 0){
				path = string.Format("Character/PlayerModel_{0}/{1}", avatarIndex[i], mainBody);
				materialPath = string.Format("Character/PlayerModel_{0}/Materials/{0}_{1}",attr.Body, "B");
			}else if(avatarIndex[i] > -1){
				path = string.Format("Character/PlayerModel_{0}/{0}_{1}_{2}", attr.Body, avatarPart[i], avatarIndex[i]);
				materialPath = string.Format("Character/PlayerModel_{0}/Materials/{0}_{1}",attr.Body, avatarPart[i]);
			}
			if(path != string.Empty)
			{
				Object resObj = Resources.Load(path);
				Material matObj = Resources.Load(materialPath) as Material;
				if(resObj != null)
				{
					avatarPartGO[i] = Instantiate(resObj) as GameObject;
					
					if(i == 0)
						dummyBall = avatarPartGO[i].transform.FindChild("DummyBall").gameObject;
				}
				
				
				if(i < 6) {
					if(bipGO == null)
					{
						bipGO = avatarPartGO[i].transform.FindChild("Bip01").gameObject;
						
						if(bipGO)
							bipGO.transform.parent = result.transform;
					}
					
					hips = bipGO.GetComponentsInChildren<Transform>();
					
					if(avatarPartGO[i] && hips.Length > 0)
					{
						SkinnedMeshRenderer smr = avatarPartGO[i].GetComponentInChildren<SkinnedMeshRenderer>();
						
						//ready combine mesh
						CombineInstance ci = new CombineInstance();
						ci.mesh = smr.sharedMesh;
						combineInstances.Add(ci);
						smr.material = matObj;
						smr.material.name = avatarPart[i];
						//sort new material
						materials.AddRange(smr.materials);
						
						//get same bip to create new bones  
						foreach(Transform bone in smr.bones){
							foreach(Transform hip in hips){
								
								if(hip.name != bone.name) continue;
								bones.Add(hip);
								break;
							}
						}
					}
					Destroy(avatarPartGO[i]);
				} else if(i == 6) {
					headDressObj = avatarPartGO[i];
				} else if(i == 7) {
					backEquipmentObj = avatarPartGO[i];
				}
			}
		}
		
		
		if (dummyBall != null)
			dummyBall.transform.parent = result.transform;
		
		GameObject clone = new GameObject();
		SkinnedMeshRenderer resultSmr = clone.gameObject.AddComponent<SkinnedMeshRenderer>();
		resultSmr.sharedMesh = new Mesh();
		resultSmr.sharedMesh.CombineMeshes(combineInstances.ToArray() , false , false);
		resultSmr.bones = bones.ToArray();
		resultSmr.materials = materials.ToArray();
		clone.name = mainBody;
		clone.transform.parent = result.transform;
		
		//HeadDress
		if (dummyHead == null)
			dummyHead = result.transform.FindChild ("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head/DummyHead").gameObject;
		if (headDressObj != null) {
			headDressObj.transform.parent = dummyHead.transform;
			headDressObj.transform.localPosition = Vector3.zero;
			headDressObj.transform.localEulerAngles = Vector3.zero;
		}
		
		//BackEquipment
		if (dummyBack == null)
			dummyBack = result.transform.FindChild ("Bip01/Bip01 Spine/Bip01 Spine1/DummyBack").gameObject;
		if (backEquipmentObj != null) {
			backEquipmentObj.transform.parent = dummyBack.transform;
			backEquipmentObj.transform.localPosition = Vector3.zero;
			backEquipmentObj.transform.localEulerAngles = Vector3.zero;
		}
		
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
		Rigidbody rig = result.GetComponent<Rigidbody> ();
		if (rig == null)
			rig = result.AddComponent<Rigidbody> ();
		rig.freezeRotation = true;

		//Layer
		result.tag = "Player";
		result.layer = LayerMask.NameToLayer ("Player");
		
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
