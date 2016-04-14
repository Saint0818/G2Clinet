using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using GameEnum;
using GameStruct;
using Newtonsoft.Json;

public class GEStageAvatarCheck : GEBase {
	private int stageID = 101;
	private int tempStageIndex;
	public TAvatar[] Avatars = new TAvatar[6];
	private Dictionary<int, TStageData> DStageData = new Dictionary<int, TStageData>();
	private Dictionary<int, TGreatPlayer> DPlayers = new Dictionary<int, TGreatPlayer>();

	private List<int> recordIDs = new List<int>();
	private List<GameObject> recordPlayer = new List<GameObject>();

	private string stageText = string.Empty;
	private string greatPlayer = string.Empty;

	private readonly Vector3[] showPosition = //new Vector3[6]
	{
		new Vector3(-3.5f, 0, -3),
		new Vector3(0, 0, -1.2f), 
		new Vector3(3.5f, 0, -3), 
		new Vector3(3.5f, 0, 3), 
		new Vector3(0, 0, 4.8f), 
		new Vector3(-3.5f, 0, 3) 
	};

	void OnFocus () {
		if (EditorApplication.isPlaying)
			LoadData ();
	}

	void OnGUI () {
		if (EditorApplication.isPlaying)
		{
			InitUI();
		}
		else
			GUILayout.Label("先執行遊戲再說"); 
	}

	private void LoadData () {
		if(string.IsNullOrEmpty(stageText) || DStageData.Count == 0)
		{
			stageText = Resources.Load<TextAsset>("GameData/stage").text;
			parseStageData(stageText);
		}

		if(string.IsNullOrEmpty(greatPlayer) || DPlayers.Count == 0)
		{
			greatPlayer = Resources.Load<TextAsset>("GameData/greatplayer").text;
			parseGreatPlayerData(greatPlayer);
		}
	}

	private void parseStageData (string text) {
		TStageData[] data = JsonConvertWrapper.DeserializeObject<TStageData[]>(text);
		if(data != null) {
			for (int i = 0; i < data.Length; i++) 
				if (data[i].ID > 0 && !DPlayers.ContainsKey(data[i].ID)) {
					DStageData.Add(data[i].ID, data[i]);
					recordIDs.Add(data[i].ID);
				}
		}
	}

	private void parseGreatPlayerData (string text) {
		TGreatPlayer[] data = (TGreatPlayer[])JsonConvert.DeserializeObject<TGreatPlayer[]> (text);
		if (data != null) {
			for (int i = 0; i < data.Length; i++) 
				if (data[i].ID > 0 && !DPlayers.ContainsKey(data[i].ID))
					DPlayers.Add(data[i].ID, data[i]);
		}
	}

	private void InitUI () {
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Stage ID:");
		stageID = EditorGUILayout.IntField(stageID, GUILayout.Width(100));

		GUILayout.Label("Show Stage ID:" + recordIDs[tempStageIndex]);
		if (GUILayout.Button(" + 1", GUILayout.Width(30)))
		{
			if(tempStageIndex < recordIDs.Count - 1)
				tempStageIndex ++;
			
			if(tempStageIndex >= 0 && tempStageIndex<recordIDs.Count)
				create(recordIDs[tempStageIndex]);
			else
				Debug.LogError("index out");
		}
		if (GUILayout.Button(" - 1", GUILayout.Width(30)))
		{
			if(tempStageIndex > 0)
				tempStageIndex --;
			
			if(tempStageIndex >= 0 && tempStageIndex<recordIDs.Count)
				create(recordIDs[tempStageIndex]);
			else
				Debug.LogError("index out");
		}
		if (GUILayout.Button("Create", GUILayout.Width(200)))
		{
			if(recordIDs.Contains(stageID)) {
				tempStageIndex = recordIDs.IndexOf(stageID);
				create (stageID);
			}else 
				Debug.LogError("stageid wrong");	
		}
		EditorGUILayout.EndHorizontal();
	}

	void create (int id) {
		if(DStageData.ContainsKey(id)) {
			cleanAll ();
			int index = 0;
			int[] friends = DStageData[id].FriendID;
			int[] players = DStageData[id].PlayerID;
			if(friends != null) {
				for(int i=0; i<friends.Length; i++) {
					if(DPlayers.ContainsKey(friends[i])) { 
						TAvatar attr = new TAvatar(1);
						attr.Body = DPlayers[friends[i]].Body;
						attr.Hair = DPlayers[friends[i]].Hair;
						attr.Cloth = DPlayers[friends[i]].Cloth;
						attr.Pants = DPlayers[friends[i]].Pants;
						attr.Shoes = DPlayers[friends[i]].Shoes;
						attr.MHandDress = DPlayers[friends[i]].MHandDress;
						attr.AHeadDress = DPlayers[friends[i]].AHeadDress;
						attr.ZBackEquip = DPlayers[friends[i]].ZBackEquip;
						createPlayer(index.ToString(), attr, DPlayers[friends[i]].BodyType);
						index ++;
					}
				}
			}else
				Debug.LogError("no friends id");

			if(players != null) {
				for(int i=0; i<players.Length; i++) {
					if(DPlayers.ContainsKey(players[i])) { 
						TAvatar attr = new TAvatar(1);
						attr.Body = DPlayers[players[i]].Body;
						attr.Hair = DPlayers[players[i]].Hair;
						attr.Cloth = DPlayers[players[i]].Cloth;
						attr.Pants = DPlayers[players[i]].Pants;
						attr.Shoes = DPlayers[players[i]].Shoes;
						attr.MHandDress = DPlayers[players[i]].MHandDress;
						attr.AHeadDress = DPlayers[players[i]].AHeadDress;
						attr.ZBackEquip = DPlayers[players[i]].ZBackEquip;
						createPlayer(index.ToString(), attr, DPlayers[players[i]].BodyType);
						index ++;
					}
				}
			}else
				Debug.LogError("no players id");
		} else
			Debug.LogError("no stage id:"+ id);
	}

	void createPlayer(string name, TAvatar attr, int bodytype){
		if(Application.isPlaying) {
			if(GameObject.Find(name) == null) {
                GameObject obj = null;
                TAvatarLoader.Load(bodytype, attr, ref obj, null, new TLoadParameter(ELayer.Default, name));
				/*GameObject obj = new GameObject();
				obj.name = name;
				obj.AddComponent<DragRotateObject>();
				ModelManager.Get.SetAvatar(ref obj, attr, bodytype, EAnimatorType.AvatarControl, false);
				obj.AddComponent<AvatarAnimationTest>();
                */
				int result = 0;
				if(int.TryParse(name, out result)) {
					if(result >= 0 && result < showPosition.Length)
						obj.transform.position = showPosition[result];
				}
				recordPlayer.Add(obj);
			}
		}
	}

	private void cleanAll () {
		for (int i=0; i<recordPlayer.Count ;i++) 
			DestroyImmediate(recordPlayer[i]);
		recordPlayer.Clear();
	}
}
