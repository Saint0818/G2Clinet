using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelManager : KnightSingleton<ModelManager>
{
	public int clothAndhanditemLength = 4;
    public Dictionary<string, AnimationClip> DataAy = new Dictionary<string, AnimationClip> ();
	public Dictionary<string, AnimationClip> SkillAniData = new Dictionary<string, AnimationClip> ();

	public Shader TransparentDiffuseShader;
	public Shader DefaultShader;
	public Shader TransparentShader;
	public Shader UnlitTransparentCutoutShader;

	public GameObject Test;
	public GameObject Test1;
	public GameObject StorePlayerInfoModel;
	public GameObject PlayerInfoModel;
	private GameObject[] PlayerModules = new GameObject[6];

	private GameObject playerName;
	private GameObject effectSelect;
	private GameObject useSkill;
	private GameObject Triggers;
	private GameObject BlockHand;
	private GameObject HandBall;
	private static string[] avatarPart = {"Body", "Head", "Hair", "Cloth", "HandItem", "HeadItem", "LegItem", "Shoes"};

	public Dictionary<string, Object> HeadItemDataAy = new Dictionary<string, Object> ();
	public Dictionary<string, Object> ShoesItemDataAy = new Dictionary<string, Object> ();
	public string[] TrainingAnimationAy = new string[]{"Idle","Idle3", "Idle4", "Idle5", "Idle6", "Idle7", "Idle9", "Idle10", "GetCharacter", "GetCharacter2"};
	public AnimationClip GameResultAni;
	public AnimationClip GetStarAni;

	void Awake()
	{
        AnimationClip[] ani = Resources.LoadAll<AnimationClip>("FBX/Animation");

		playerName = Resources.Load ("FBX/UIPlayer/PlayerName") as GameObject;
		effectSelect = Resources.Load ("FBX/GamePlayer/EffectSelect") as GameObject;
		useSkill = Resources.Load ("FBX/GamePlayer/UseSkill") as GameObject;
		Triggers = Resources.Load ("FBX/GamePlayer/Triggers") as GameObject;
		BlockHand = Resources.Load ("FBX/GamePlayer/BlockHand") as GameObject;
		HandBall = Resources.Load ("FBX/GamePlayer/HandBall") as GameObject;
		Object[] headItemAy = Resources.LoadAll("Prefab/PlayerItem/Head");
		Object[] ShoesItemAy = Resources.LoadAll("Prefab/PlayerItem/Shoes");

		DefaultShader = Shader.Find("Toon/Basic");
		TransparentShader = Shader.Find("Toon/BasicTransparent");
		UnlitTransparentCutoutShader = Shader.Find("Unlit/Transparent Cutout");
		TransparentDiffuseShader = Shader.Find("Transparent/Diffuse");

		GameResultAni = Resources.Load("Prefab/UI/Animation/UInewGameResult") as AnimationClip;
		GetStarAni =  Resources.Load("Prefab/UI/Animation/GetStar") as AnimationClip;

		PlayerInfoModel = GameObject.Find("PlayerInfoModel");
		if (!PlayerInfoModel) {
			PlayerInfoModel = new GameObject();
			PlayerInfoModel.name = "PlayerInfoModel";
			UIPanel up = PlayerInfoModel.AddComponent<UIPanel>();
			up.depth = 2;
		}

		StorePlayerInfoModel = GameObject.Find("StorePlayerInfoModel");
		if (!StorePlayerInfoModel) {
			StorePlayerInfoModel = new GameObject();
			StorePlayerInfoModel.name = "StorePlayerInfoModel";
			UIPanel up = StorePlayerInfoModel.AddComponent<UIPanel>();
			up.depth = 1;
			StorePlayerInfoModel.SetActive(false);
		}

		if (ani.Length > 0) {
			for(int i = 0; i < ani.Length; i++)
			{
				string keyname = ani[i].name.Replace(" (UnityEngine.AnimationClip)", "");
					if(!DataAy.ContainsKey(keyname))
						DataAy.Add(keyname, ani[i]);
			}
		}

		if (headItemAy.Length > 0) {
			for(int i = 0; i < headItemAy.Length; i++)
			{
				string keyname = headItemAy[i].name.Replace(" (UnityEngine.GameObject)", "");
				if(!HeadItemDataAy.ContainsKey(keyname))
					HeadItemDataAy.Add(keyname, headItemAy[i]);
			}
		}

		if (ShoesItemAy.Length > 0) {
			for(int i = 0; i < ShoesItemAy.Length; i++)
			{
				string keyname = ShoesItemAy[i].name.Replace(" (UnityEngine.GameObject)", "");
				if(!ShoesItemDataAy.ContainsKey(keyname))
					ShoesItemDataAy.Add(keyname, ShoesItemAy[i]);
			}
		}

		for (int j = 0; j < PlayerModules.Length; j++) 
		{
			name = string.Format("Prefab/PlayerTrain_{0}", j);
			PlayerModules[j] = Resources.Load(name) as GameObject;
		}
	}

	public void AddPlayerModule(int kind, int index, ref GameObject result, Vector3 pos, Quaternion rotate) {
		if(index < PlayerModules.Length && PlayerModules[index])
		{
			result = Instantiate(PlayerModules[index], pos, rotate) as GameObject;
			result.layer = LayerMask.NameToLayer("Player");
			result.tag = "Player";

			ClearAllAnimation (result);

			GameObject clone;
			GameObject dummyBall = result.transform.FindChild("DummyBall").gameObject;
	
			switch(kind)
			{
				case 0: //UIplayer
					clone = Instantiate(playerName) as GameObject;
					clone.name = "PlayerName";
					clone.transform.parent = result.transform;
					clone.transform.localPosition = new Vector3(0, -0.06f, 0.4f);
					clone.transform.localEulerAngles = new Vector3(0, 90, 0);
					clone.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
					
					result.animation.cullingType = AnimationCullingType.BasedOnRenderers;
					for (int i = 0; i < TrainingAnimationAy.Length; i++) {
						if (DataAy.ContainsKey (TrainingAnimationAy [i])) {
							result.animation.AddClip (DataAy [TrainingAnimationAy [i]], TrainingAnimationAy [i]);
							result.animation.clip = DataAy [TrainingAnimationAy [i]];
						}
					}
				break;
				case 1: //GamePlayer
					clone = Instantiate(effectSelect) as GameObject;
					clone.name = "EffectSelect";
					clone.transform.parent = result.transform;
					clone.transform.localPosition = Vector3.zero;
					
					clone = Instantiate(useSkill) as GameObject;
					clone.name = "UseSkill";
					clone.transform.parent = result.transform;
					clone.transform.localPosition = Vector3.zero;
					
					clone = Instantiate(Triggers) as GameObject;
					clone.name = "Triggers";
					clone.transform.parent = result.transform;
					clone.transform.localPosition = Vector3.zero;
					
					clone = Instantiate(BlockHand) as GameObject;
					clone.name = "BlockHand";
					if(dummyBall)
					{
						clone.transform.parent = dummyBall.transform;
						clone.transform.localPosition = Vector3.zero;
					}
					
					clone = Instantiate(HandBall) as GameObject;
					clone.name = "HandBall";
					if(dummyBall)
					{
						clone.transform.parent = dummyBall.transform;
						clone.transform.localPosition = Vector3.zero;
					}
					
					result.AddComponent<PlayerBehaviour>();
					result.AddComponent<Rigidbody>();
					result.rigidbody.mass = 100;
					result.rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
					
					foreach (KeyValuePair<string, AnimationClip> item in DataAy) {
						result.animation.AddClip (item.Value, item.Key);
						result.animation.clip = item.Value;
					}
				break;
			case 2:
				clone = Instantiate(playerName) as GameObject;
				clone.name = "PlayerName";
				clone.transform.parent = result.transform;
				clone.transform.localPosition = new Vector3(0, -0.06f, 0.4f);
				clone.transform.localEulerAngles = new Vector3(0, 90, 0);
				clone.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);

				clone = Instantiate(HandBall) as GameObject;
				clone.name = "HandBall";

				if(dummyBall)
				{
					clone.transform.parent = dummyBall.transform;
					clone.transform.localPosition = Vector3.zero;
				}

//				result.AddComponent<StorePlayerBehaviour>();

				foreach (KeyValuePair<string, AnimationClip> item in DataAy) {
					result.animation.AddClip (item.Value, item.Key);
					result.animation.clip = item.Value;
				}
				break;
			case 3://TalkMan
				clone = Instantiate(playerName) as GameObject;
				clone.name = "PlayerName";
				clone.transform.parent = result.transform;
				clone.transform.localPosition = new Vector3(0, -0.06f, 0.4f);
				clone.transform.localEulerAngles = new Vector3(0, 90, 0);
				clone.transform.localScale = new Vector3(0.003f, 0.003f, 0.003f);

				foreach (KeyValuePair<string, AnimationClip> item in DataAy) {
					result.animation.AddClip (item.Value, item.Key);
					result.animation.clip = item.Value;
				}
				break;
			}
		}
	}

	public void RandomPlayAnimation(GameObject player)
	{
		Animation ani = player.GetComponent<Animation>();
		List<string> test = new List<string>();

		foreach (AnimationState item in ani) {
			test.Add(item.name);	
		}

		int index = Random.Range(0, test.Count);

		if(test[index] == "GetCharacter" || test[index] == "GetCharacter2")
			RandomPlayAnimation(player);
		else
			player.animation.Play (test[index]);
	}

	public void RandomPlayAnimationByStorePlayer(GameObject player)
	{
		Animation ani = player.GetComponent<Animation>();
		List<string> test = new List<string>();
		
		foreach (AnimationState item in ani) {
			test.Add(item.name);	
		}

		player.animation.Play("StayIdle");
	}

	public void ClearAllAnimation(GameObject player)
	{
		List<string> test = new List<string>();

		foreach(AnimationState animState in player.animation)
			test.Add(animState.name);
		
		if(test.Count > 0)
			for(int i = 0; i < test.Count; i++)
				player.animation.RemoveClip(test[i]);
	}

	public void AddHeadItem(GameObject player, int index)
	{
		if (player) {
			GameObject headBip;

			Vector3 size = player.transform.localScale; 

			string fileName = string.Format("HeadItem_{0}", index);
	
			string path = "Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Neck/Bip01 Head";

			headBip = player.transform.FindChild(path).gameObject;

			GameObject ItemGp = null;

			if(headBip.transform.childCount > 1)
			{
				foreach (Transform child in headBip.transform)
					if(child.name == "HeadItem")
						ItemGp = child.gameObject;
			}

			if(ItemGp)
			{
				bool sameObject = false;

				foreach (Transform child in ItemGp.transform)
				{
					if(child.name == fileName)
						sameObject = true;
				}

				if(!sameObject)
				{
					Destroy(ItemGp);
					ItemGp = null;
				}
			}

			if(index > 50 && !ItemGp && headBip && HeadItemDataAy.ContainsKey(fileName))
			{
				GameObject gp = new GameObject();
				gp.name = "HeadItem";
				gp.transform.parent = headBip.transform;
				gp.transform.localPosition = Vector3.zero;
				gp.transform.localEulerAngles = new Vector3(-74.30341f,-90, 180);

				GameObject clone = Instantiate(HeadItemDataAy[fileName]) as GameObject;
				clone.transform.parent = gp.transform;
				clone.transform.localPosition = Vector3.zero;
				clone.transform.localEulerAngles = new Vector3(-90, 0, 0);
				clone.transform.localScale = size;

				if(player.transform.FindChild("HeadItem"))
					player.transform.FindChild("HeadItem").renderer.enabled = false;
			}
		}
	}

	public void AddShoesItem(GameObject player, int index)
	{
		if (player) {
			GameObject shoes = null;
			string pathR = "Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 R Thigh/Bip01 R Calf/Bip01 R Foot/ShoesRPos";
			string pathL = "Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 L Thigh/Bip01 L Calf/Bip01 L Foot/ShoesLPos";
			Vector3 size = player.transform.localScale; 
			string fileName = string.Format("ShoesItem_{0}", index);
			GameObject ItemGp = null;

			for(int i = 0; i < 2; i++){
				if(i == 0) {
					Transform t = player.transform.FindChild(pathR);
					if (t)
						shoes = t.gameObject;
				} else {
					Transform t = player.transform.FindChild(pathL);
					if (t)
						shoes = t.gameObject;
				}

				if(shoes && shoes.transform.childCount > 0)
				{
					foreach (Transform child in shoes.transform)
						if(child.name == "ShoesItem")
							ItemGp = child.gameObject;
				}

				if(ItemGp)
				{
					bool sameObject = false;
					
					foreach (Transform child in ItemGp.transform)
					{
						if(child.name == fileName)
							sameObject = true;
					}
					
					if(!sameObject)
					{
						Destroy(ItemGp);
						ItemGp = null;
					}
				}

				if(index > 0 && !ItemGp && shoes && ShoesItemDataAy.ContainsKey(fileName))
				{
					GameObject gp = new GameObject();
					gp.name = "ShoesItem";
					gp.transform.parent = shoes.transform;
					gp.transform.localPosition = Vector3.zero;
					gp.transform.localEulerAngles = Vector3.zero;
					gp.transform.localScale = Vector3.one;

					GameObject clone = Instantiate(ShoesItemDataAy[fileName]) as GameObject;
					clone.name = fileName;
					clone.transform.parent = gp.transform;
					clone.transform.localPosition = Vector3.zero;
					clone.transform.localEulerAngles = new Vector3(-90, 0, 0);
					clone.transform.localScale = size;

					string texName = clone.renderer.material.mainTexture.name;

					if(texName == string.Empty || texName != fileName)
					{
						//TODO:Close
//						if(TextureManager.PlayerTexutres.ContainsKey(fileName))
//							clone.gameObject.renderer.material.mainTexture = TextureManager.PlayerTexutres[fileName];
//						else {
//
//							string path = string.Format("FBX/PlayerItem/Texture/{0}", fileName);
//							Texture2D texture = Resources.Load(path) as Texture2D;
//							if (texture) {
//								clone.gameObject.renderer.material.mainTexture = texture;
//								TextureManager.PlayerTexutres.Add(fileName, texture);
//							} 
//						}
					}
				}
			}

			Transform ts = player.transform.FindChild("Shoes");
			if (ts) {
				GameObject go = ts.gameObject;;

				if(go)
					go.renderer.enabled = index <= 50;
			}
		}
	}
	
	public void AddHandItem(GameObject player, int index)
	{
		//only modle 0
		if (player){
			GameObject[] hand = new GameObject[2]; 

			for(int i = 0; i < hand.Length; i++)
			{
				Transform t = player.transform.FindChild(string.Format("HandItem_{0}", i));
				if (t) 
					hand[i] = t.gameObject;
			}

			if(hand[0])
				hand[0].gameObject.renderer.enabled = index <= 50;

			if(hand[1])
				hand[1].gameObject.renderer.enabled = index > 50;
		}
	}

	public void AddClothItem(GameObject player, int index)
	{
		//only modle 0
		GameObject cloth;

		if (player){
			GameObject[] cloths = new GameObject[2]; 
			
			for(int i = 0; i < cloths.Length; i++)
			{
				Transform t = player.transform.FindChild(string.Format("Cloth_{0}", i));
				if (t)
					cloths[i] = t.gameObject;
			}
			
			if(cloths[0])
				cloths[0].gameObject.SetActive(index <= 50);
			
			if(cloths[1])
				cloths[1].gameObject.SetActive(index > 50);

			if(cloths[0] && cloths[1] && index > 50)
				cloth = cloths[1];
			else
				cloth = cloths[0];
		}
	}

    public void AddSkillAnimation(GameObject player, int skillindex)
    {
		string name = string.Format("Skill{0}", skillindex);

		AddSkillAnimation(player, name);
    }

    public void AddSkillAnimation(GameObject player, string anistr)
    {
		bool ishave = false;
		Animation ani = player.GetComponent<Animation>();
		string path;
		
		foreach (AnimationState item in ani) {
			if(item.name == anistr)
				ishave = true;
		}
		
		if (!ishave)
		{
			if(SkillAniData.ContainsKey(anistr))
			{
				player.animation.AddClip(SkillAniData[anistr], anistr);
				player.animation.clip = SkillAniData[anistr];
			}
			else
			{
				path = string.Format("FBX/SkillAnimation/{0}", anistr);
				AnimationClip skillAni = Resources.Load<AnimationClip>(path);
				if(skillAni)
				{
					SkillAniData.Add(anistr, skillAni);
					player.animation.AddClip(SkillAniData[anistr], anistr);
					player.animation.clip = SkillAniData[anistr];
				}
				else
					Debug.LogError("Can't find animation at the path : " + path);
			}
		}
    }


	public void InitTest()
	{
		AddPlayerModule(0, 2, ref Test, Vector3.zero, Quaternion.Euler(new Vector3(10, 270, 0)));
		AddHeadItem(Test, 52);
		AddShoesItem(Test, 52);
		AddHandItem(Test, 52);
	}

	public void ChangeLayersRecursively(Transform trans, string name)
	{
		trans.gameObject.layer = LayerMask.NameToLayer(name);
		foreach(Transform child in trans)
		{            
			ChangeLayersRecursively(child, name);
		}
	}

	public Transform[] GetAvatarPartAndEnable(int[] avatarIndex, int modelindex, GameObject player)
	{
		Transform[] objs = new Transform[8]; 
		return objs;
	}
}
