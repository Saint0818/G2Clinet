﻿using UnityEngine;
using System.Collections;

public class ModelManager : MonoBehaviour {
	public static ModelManager Get;
	private GameObject PlayerModule = null; 
	public GameObject PlayerInfoModel = null;
	private const int avatartCount = 3;
	private string[] avatarPartName = new string[]{"Body", "Cloth", "Shoes"};

	public static void Init(){
		GameObject gobj = new GameObject(typeof(ModelManager).Name);
		DontDestroyOnLoad(gobj);
		Get = gobj.AddComponent<ModelManager>();
	}

	void Awake(){
		PlayerInfoModel = new GameObject();
		PlayerInfoModel.name = "PlayerInfoModel";
		UIPanel up = PlayerInfoModel.AddComponent<UIPanel>();
		up.depth = 2;

		PlayerModule = Resources.Load("Prefab/Player/PlayerModel_0") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public PlayerBehaviour CreatePlayer(int Index, TeamKind Team, Vector3 BornPos, Vector2 [] RunPosAy, MoveType MoveKind, GamePostion Postion){
		GameObject Res = Instantiate(PlayerModule) as GameObject;
		Res.transform.parent = PlayerInfoModel.transform;
		Res.transform.localPosition = BornPos;
		if(Team == TeamKind.Npc)
			Res.transform.localEulerAngles = new Vector3(0, 180, 0);
		PlayerBehaviour PB = Res.AddComponent<PlayerBehaviour>();
		PB.Team = Team;
		PB.MoveKind = MoveKind;
		PB.MoveIndex = -1;
		PB.Postion = Postion;
		PB.RunPosAy = RunPosAy;
		Res.name = Index.ToString();

		GameStruct.TPlayerAttribute attr = new GameStruct.TPlayerAttribute();

		if ((int)Team == 1) {
			attr.Cloth = 1;
			SetAvatar (ref Res, ref attr);
		} 
		else
			SetAvatar (ref Res, ref attr);

		return PB;
	}

	public void SetAvatar(ref GameObject playerModel, ref GameStruct.TPlayerAttribute attr) {
		if (playerModel) {

			GameObject[] avatarPart =  new GameObject[avatartCount];
			int[] avatarIndex = new int[avatartCount];

			avatarIndex[0] = attr.Face; 		//Body
			avatarIndex[1] = attr.Cloth;  		//Cloth
			avatarIndex[2] = attr.Shoes;		//Shoes

			for(int i = 0; i < avatarPart.Length; i ++) {
				avatarPart[i] = playerModel.transform.FindChild(avatarPartName[i]).gameObject;

				if(avatarPart[i])
				{
					string path = "Character/PlayerModel_0/Texture/" + avatarPartName[i] + "_" + avatarIndex[i];
					Texture2D texture = Resources.Load(path) as Texture2D;
					if (texture) {
						avatarPart[i].gameObject.renderer.material.mainTexture = texture;
					}
				}
			}
		}
	}
}
