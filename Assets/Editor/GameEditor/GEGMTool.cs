using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameStruct;

public struct TPlayerInfo
{
	public int Kind;
	public int Value;
}

public class GEGMTool : GEBase
{
	private int options = 0;
	private string[] optionsTitle = new string[4]{"物品", "關卡", "戰鬥", "人物資料"};
	
    void OnGUI()
    {
		if (EditorApplication.isPlaying) {
			options = GUILayout.Toolbar(options, optionsTitle);
			switch (options) {
				case 0:
					ItemHandle ();
					break;
				case 1:
					StageHandle ();
					break;
				case 2:
					BattleHandle ();
					break;
				case 3:
					PlayerInfoHandle ();
					break;
			}
		}
		else
			GUILayout.Label("想用？先執行遊戲再說"); 
    }

	private int addItemCount = 1;
	private int[] itemIds;
	private string mArea = "---------------------------------------------------------------------------------------------";
	private int countprekind = 1;
	private int position = 0;

	private void ItemHandle()
	{
		EditorGUILayout.LabelField(mArea);

		//Add Item
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("物品數量 : "); 
		addItemCount = EditorGUILayout.IntField (addItemCount, GUILayout.Width(100));
		if (GUILayout.Button ("設定", GUILayout.Width (200))) {
			itemIds = new int[addItemCount];
			for(int i = 0; i < itemIds.Length; i++)
				itemIds[i] = -1;
		}
		EditorGUILayout.EndHorizontal();

		if(itemIds != null && itemIds.Length > 0)
			for (int i = 0; i < itemIds.Length; i++) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label("物品編號 : "); 
				itemIds[i] = EditorGUILayout.IntField (itemIds[i], GUILayout.Width(100));
				EditorGUILayout.EndHorizontal ();
			}

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("AddItem", GUILayout.Width(200)))
		{
			if(itemIds != null && itemIds.Length > 0){
				WWWForm form = new WWWForm();
				form.AddField("AddIndexs", JsonConvert.SerializeObject(itemIds));
				SendHttp.Get.Command(URLConst.GMAddItem, waitGMAddItem, form);
			}
			else
				ShowHint("請設定Item數量");
		}

		if (GUILayout.Button("Remove", GUILayout.Width(200)))
		{
			if(itemIds != null && itemIds.Length > 0){
			WWWForm form = new WWWForm();
			form.AddField("RemoveIndexs", JsonConvert.SerializeObject(itemIds));
			SendHttp.Get.Command(URLConst.GMRemoveItem, waitGMAddItem, form);
			}
			else
				ShowHint("請設定Item數量");
		}

		if(GUILayout.Button("刪除背包", GUILayout.Width(200)))
		{
			WWWForm form = new WWWForm();
			form.AddField("RemoveAll", "true");
			SendHttp.Get.Command(URLConst.GMRemoveItem, waitGMAddItem, form);
		}
		EditorGUILayout.EndHorizontal();


		PrePartAddItem ();
		LimitPartAddItem();
	}

	private int AvatarPotential = 0;
	private int CrtAvatarPotential = 0;
	private int LvPotential = 0;
	private int CrtLvPotential = 0;

	private int playerlv = 0;
	private int avatarPotential = 0;
	private bool IsInitPlayerInfo = false;
	private int useLvPotential = 0;
	private int useAvatarPotential = 0;
	private int[] addPotential = new int[GameConst.PotentialCount];
	private int[] Potential = new int[GameConst.PotentialCount];
	
	private void InitPotentialPoint()
	{
		AvatarPotential = GameData.Team.AvatarPotential;
		avatarPotential = AvatarPotential;
		LvPotential = GameData.Team.Player.Lv * GameConst.PreLvPotential;
		CrtAvatarPotential = AvatarPotential;

		if (GameData.Team.PlayerBank != null && GameData.Team.PlayerBank.Length > 1) {
			for(int i = 0;i< GameData.Team.PlayerBank.Length; i++){
				if(GameData.Team.PlayerBank[i].RoleIndex != GameData.Team.Player.RoleIndex){
					CrtAvatarPotential -= GetUseAvatarPotentialFromBank(GameData.Team.PlayerBank[i]);
				}
			}
		}

		CrtAvatarPotential -= GetUseAvatarPotential (GameData.Team.Player);
		CrtLvPotential = GetCurrentLvPotential (GameData.Team.Player);
	}

	public int GetUseAvatarPotentialFromBank(TPlayerBank player)
	{
		int lvpoint = player.Lv * GameConst.PreLvPotential;
		int use = 0;

		for(int i = 0; i < player.Potential.Length; i++){
			use += player.Potential[i] * GameConst.PotentialRule[i]; 
		}

		if (use > lvpoint)
			return use - lvpoint;
		else
			return 0;
	}

	public int GetUseAvatarPotential(TPlayer player)
	{
		int lvpoint = player.Lv * GameConst.PreLvPotential;
		int use = 0;
		
		for(int i = 0; i < player.Potential.Length; i++){
			use += player.Potential[i] * GameConst.PotentialRule[i]; 
		}
		
		if (use > lvpoint)
			return use - lvpoint;
		else
			return 0;
	}

	public int GetCurrentLvPotential(TPlayer player)
	{
		int lvpoint = player.Lv * GameConst.PreLvPotential;
		int use = 0;
		
		for(int i = 0; i < player.Potential.Length; i++){
			use += player.Potential[i] * GameConst.PotentialRule[i]; 
		}

		if (lvpoint > use)
			return lvpoint - use;
		else
			return 0;
	}

	private void PlayerInfoHandle()
	{
		if (GUILayout.Button ("讀取資料", GUILayout.Width (200))) {
			playerlv = GameData.Team.Player.Lv;
			Potential = GameData.Team.Player.Potential;
			InitPotentialPoint();
			IsInitPlayerInfo = true; 
		}

		if (!IsInitPlayerInfo)
			return;

		AddPlayeLv ();
		AddAvatarPotential ();

		if(Potential.Length > 0)
			for (int i = 0; i < Potential.Length; i++) {
			EditorGUILayout.BeginHorizontal();

			GUILayout.Label(string.Format("Masteries{0} : {1} + {2}/100", i, GameData.Team.Player.Potential[i], addPotential[i])); 

			if (GUILayout.Button ("+", GUILayout.Width (200))) {
				if(CrtAvatarPotential > 0 &&  Potential[i] < 100)
				{
					if(CanUsePotential(i)){
						addPotential[i]++;
						CalculateAddPotential();
					}


				}
			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("取消配點", GUILayout.Width(50))){
			for(int i = 0;i < addPotential.Length; i++)
				addPotential[i] = 0;
			
			CalculateAddPotential();
		}

		if(GUILayout.Button("重置", GUILayout.Width(50))){
//			for(int i = 0;i < Potential.Length; i++)
//				Potential[i] = 0;
//
//			InitPotentialPoint();
		}

		if (GUILayout.Button ("存檔", GUILayout.Width (200))) {
			if(HaveChange()){
				WWWForm form = new WWWForm();
				int[] save = new int[GameConst.PotentialCount];

				for(int i = 0;i< save.Length; i++)
					save[i] = Potential[i] + addPotential[i];

				form.AddField("Potential", JsonConvert.SerializeObject(save));
				SendHttp.Get.Command(URLConst.GMSavePotential, waitSaveMasteries, form);
			}


		}
		EditorGUILayout.EndHorizontal();
	}

	private bool HaveChange()
	{
		for (int i = 0; i< addPotential.Length; i++)
			if (addPotential [i] > 0)
				return true;

		return false;
	}

	private bool CanUsePotential(int index)
	{
		return CrtAvatarPotential + CrtLvPotential >= useLvPotential + useAvatarPotential + GameConst.PotentialRule [index];
	}

	private void CalculateAddPotential()
	{
		int count = 0;
		for (int i = 0; i < addPotential.Length; i++) {
			count += addPotential[i] * GameConst.PotentialRule[i];
		}

		if (CrtLvPotential >= count) {
			useLvPotential = count;
			useAvatarPotential = 0;
		}
		else {
			useLvPotential = CrtLvPotential;
			useAvatarPotential = count - CrtLvPotential;
		}
	}

	private void AddPlayeLv()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(string.Format("等級潛能點 : {0} - {1} / {2}", CrtLvPotential, useLvPotential,LvPotential)); 
		GUILayout.Label(", 設定人物等級 : "); 
		playerlv = EditorGUILayout.IntField (playerlv, GUILayout.Width(100));
		
		if (GUILayout.Button ("設定", GUILayout.Width (200))) {
			if(playerlv != GameData.Team.Player.Lv){
				WWWForm form = new WWWForm();
				form.AddField("Lv", playerlv);
				SendHttp.Get.Command(URLConst.GMAddLv, waitGMPlayerInfo, form);
			}
			else
				ShowHint("請設定Player Lv");
		}
		EditorGUILayout.EndHorizontal();
	}

	private void AddAvatarPotential()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(string.Format("裝備潛能點 : {0} - {1} / {2}", CrtAvatarPotential, useAvatarPotential, AvatarPotential)); 
		GUILayout.Label(", 設定裝備潛能點 : "); 
		avatarPotential = EditorGUILayout.IntField (avatarPotential, GUILayout.Width(100));
		
		if (GUILayout.Button ("設定", GUILayout.Width (200))) {
			if(avatarPotential > 0){
				WWWForm form = new WWWForm();
				form.AddField("AvatarPotential", avatarPotential);
				SendHttp.Get.Command(URLConst.GMAddAvatarPotential, waitGMAddAvatarPotential, form);
			}
			else
				ShowHint("請設定AvatarPotential");
		}
		EditorGUILayout.EndHorizontal();
	}

	List<int> itemIds2 = new List<int>();

	//每部位加Item
	private void PrePartAddItem()
	{
		EditorGUILayout.LabelField(mArea);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("位置： : "); 
		position = EditorGUILayout.IntField (position, GUILayout.Width(30));
		GUILayout.Label ("(中鋒: 0 、前鋒：1、後衛：２)");
		
		GUILayout.Label("各部位＋ : "); 
		countprekind = EditorGUILayout.IntField (countprekind, GUILayout.Width(30));
		GUILayout.Label("個"); 
		
		if (GUILayout.Button("Add", GUILayout.Width(200)))
		{
			//note : Item data all kind 
			int findCount = 0;
			int currentkind = 0;
			itemIds2.Clear();

			foreach( KeyValuePair<int, TItemData> item in GameData.DItemData ){
				if(item.Value.Kind > 0){
					if(currentkind != item.Value.Kind){
						findCount = 0;
						currentkind = item.Value.Kind;
					}
					else{
						if(findCount < countprekind){
							if(item.Value.Kind < 6 && item.Value.Position == position){
								itemIds2.Add(item.Value.ID);
							}
							else
								itemIds2.Add(item.Value.ID);

							findCount++;
						}
					}
				}
			}
			
			if(itemIds2 != null && itemIds2.Count > 0){
				WWWForm form = new WWWForm();
				form.AddField("AddIndexs", JsonConvert.SerializeObject(itemIds2));
				SendHttp.Get.Command(URLConst.GMAddItem, waitGMAddItem, form);
			}
			else
				ShowHint("請設定Item數量");
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
	//指定部位加Item
	private int limitposition = 0;
	private int limitcountprekind = 1;
	private int limitItemkind = 0;
	private int[] itemIds3;

	private void LimitPartAddItem()
	{
		EditorGUILayout.LabelField(mArea);
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label("位置:"); 
		limitposition = EditorGUILayout.IntField (limitposition, GUILayout.Width(30));
		GUILayout.Label ("(中鋒: 0 、前鋒：1、後衛：２)");

		GUILayout.Label("部位:(Kind) "); 
		limitItemkind = EditorGUILayout.IntField (limitItemkind, GUILayout.Width(30));

		if (limitItemkind > 7)
			ShowHint ("Error Kind : " + limitItemkind);
		
		GUILayout.Label("各部位＋ : "); 
		limitcountprekind = EditorGUILayout.IntField (limitcountprekind, GUILayout.Width(30));
		GUILayout.Label("個"); 
		
		if (GUILayout.Button("Add", GUILayout.Width(200)))
		{
			int findCount;
//			itemIds3 = new int[limitcountprekind];
			findCount = 0;
			List<int> itemIds3 = new List<int>();
				
			foreach( KeyValuePair<int, TItemData> item in GameData.DItemData )
			{
				if(limitItemkind < 6)
				{
					if(item.Value.Kind == limitItemkind && item.Value.Position == limitposition)
					{
						if(findCount < countprekind){
							itemIds3.Add(item.Value.ID);
//							itemIds3[findCount] = item.Value.ID;
							findCount++;
						}
						else
							continue;
					}
				}
				else
				{
					if(item.Value.Kind == limitItemkind)
					{
						if(findCount < limitcountprekind){
							itemIds3.Add(item.Value.ID);
//							itemIds3[findCount] = item.Value.ID;
							findCount++;
						}
						else
							continue;
					}
				}
			}

			for(int i = 0; i < itemIds3.Count; i++)
			{
				Debug.LogError("Find : " + itemIds3[i]);
			}

			if(itemIds3 != null && itemIds3.Count > 0){
				WWWForm form = new WWWForm();
				form.AddField("AddIndexs", JsonConvert.SerializeObject(itemIds3));
				SendHttp.Get.Command(URLConst.GMAddItem, waitGMAddItem, form);
			}
			else
				ShowHint("請設定Item數量");
		}
		
		EditorGUILayout.EndHorizontal();
	}

	private void waitGMAddItem(bool ok, WWW www)
	{
		if(ok)
		{
//			ShowHint("Server Return : " + www.text);

			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Items = team.Items;
			if(team.Items.Length > 0)
				for(int i = 0; i < team.Items.Length; i++)
					if(GameData.DItemData.ContainsKey(team.Items[i].ID))
						Debug.Log("item : " + GameData.DItemData[team.Items[i].ID].Name);

			if(UIAvatarFitted.Visible)
				UIAvatarFitted.Get.UpdateAvatar(true);
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddItem);
	}
	
	private void waitGMPlayerInfo(bool ok, WWW www)
	{
		if(ok){
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			ShowHint("PlayerLv Upgrade " + GameData.Team.Player.Lv + " > " + team.Player.Lv);
			GameData.Team.Player.Lv = team.Player.Lv;
			GameData.Team.AvatarPotential = team.AvatarPotential;
			InitPotentialPoint();
		}else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddLv);
	}

	private void waitGMAddAvatarPotential(bool ok, WWW www)
	{
		if (ok) {
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.AvatarPotential = team.AvatarPotential;
			InitPotentialPoint();
		}
	}

	private void waitSaveMasteries(bool ok, WWW www)
	{
		if(ok){
			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			GameData.Team.Player.Potential = team.Player.Potential;
		}else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddLv);
	}

    private int mNextMainStageID = GameConst.Default_MainStageID;
	private void StageHandle()
	{
	    nextMainStageIDLabel();
        resetStageChallengeNums();
	}

    private void nextMainStageIDLabel()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("NextMainStageID: ");
        mNextMainStageID = EditorGUILayout.IntField(mNextMainStageID, GUILayout.Width(100));
        if(GUILayout.Button("設定", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            form.AddField("NextMainStageID", mNextMainStageID);
            SendHttp.Get.Command(URLConst.GMSetNextMainStageID, waitGMSetNextMainStageID, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMSetNextMainStageID(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMSetNextMainStageID, ok:{0}", ok);

        if(ok)
        {
            TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
            GameData.Team.Player.NextMainStageID = team.Player.NextMainStageID;
            updateUIMainStage();
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMSetNextMainStageID);
    }

    private void resetStageChallengeNums()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("每日關卡限制: ");
        if(GUILayout.Button("重置", GUILayout.Width(50)))
        {
            WWWForm form = new WWWForm();
            SendHttp.Get.Command(URLConst.GMResetStage, waitGMResetStage, form);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void waitGMResetStage(bool ok, WWW www)
    {
        Debug.LogFormat("waitGMResetStage, ok:{0}", ok);

        if(ok)
        {
            GameData.Team.Player.StageChallengeNums.Clear();
            updateUIMainStage();
        }
        else
            Debug.LogErrorFormat("Protocol:{0}", URLConst.GMSetNextMainStageID);
    }

    private void updateUIMainStage()
    {
        if(UIMainStage.Get.Visible)
        {
            UIMainStage.Get.Hide();
            UIMainStage.Get.Show();
        }
    }

    private void BattleHandle()
	{

	}

    private void ShowHint(string str)
    {
        this.ShowNotification(new GUIContent(str));
    }
}
