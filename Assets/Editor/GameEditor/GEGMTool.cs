using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameStruct;

public class GEGMTool : GEBase
{
	private int options = 0;
	private string[] optionsTitle = new string[3]{"物品", "關卡", "戰鬥"};
	
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
					ItemHandle ();
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
