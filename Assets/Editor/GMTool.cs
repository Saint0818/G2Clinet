using UnityEngine;
using UnityEditor;
using System.Collections;
using Newtonsoft.Json;
using GameStruct;

public class GMTool : EditorWindow
{
	[MenuItem ("GameEditor/GMTool")]
	private static void GMToolWindow()
    {
		EditorWindow.GetWindowWithRect(typeof(GMTool), new Rect(0, 0, 600, 400), false, "GMTool").Show();
    }

	private int options = 0;
	private string[] optionsTitle = new string[3]{"物品", "關卡", "戰鬥"};
	
    void OnGUI()
    {
		if (EditorApplication.isPlaying) {
			options = GUILayout.Toolbar(options, optionsTitle);
			switch (options) {
				case 0:
					ItemHandel ();
					break;
				case 1:
					SteageHandel ();
					break;
				case 2:
					BattleHandel ();
					break;
				case 3:
					ItemHandel ();
					break;
			}
		}
		else
			GUILayout.Label("想用？先執行遊戲再說"); 
    }

	private int addItemCount = 1;
	private int[] itemIds;
	private string mArea = "---------------------------------------------------------------------------------------------";

	private void ItemHandel()
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
	}

	private void waitGMAddItem(bool ok, WWW www)
	{
		if(ok)
		{
			ShowHint("Server Return : " + www.text);

			TTeam team = (TTeam)JsonConvert.DeserializeObject(www.text, typeof(TTeam));
			
			if(team.Items.Length > 0)
				for(int i = 0; i < team.Items.Length; i++)
					if(GameData.DItemData.ContainsKey(team.Items[i].ID))
						Debug.Log("item : " + GameData.DItemData[team.Items[i].ID].Name);
		}
		else
			Debug.LogErrorFormat("Protocol:{0}", URLConst.GMAddItem);
	}

	private void SteageHandel()
	{

	}

	private void BattleHandel()
	{

	}

    private void ShowHint(string str)
    {
        this.ShowNotification(new GUIContent(str));
    }
}
