using UnityEngine;
using System.Collections;

public class ModelManager : MonoBehaviour {
	public static ModelManager Get;
	private GameObject PlayerModule = null; 
	public GameObject PlayerInfoModel = null;

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

	public PlayerBehaviour CreatePlayer(int Index, TeamKind Team, BodyType Body){
		GameObject Res = Instantiate(PlayerModule) as GameObject;
		Res.transform.parent = PlayerInfoModel.transform;
		PlayerBehaviour PB = Res.AddComponent<PlayerBehaviour>();
		PB.Team = Team;
		PB.Body = Body;
		Res.name = Index.ToString();
		return PB;
	}
}
