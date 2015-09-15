using UnityEngine;
using UnityEditor;
using System.Collections;

public class GMTool : EditorWindow
{
	[MenuItem ("GameEditor/GMTool")]
	private static void GMToolWindow()
    {
		EditorWindow.GetWindowWithRect(typeof(GMTool), new Rect(0, 0, 600, 400), true, "GMTool").Show();
    }

	private int options = 0;
	private string[] optionsTitle = new string[3]{"物品", "關卡", "戰鬥"};
	
    void OnGUI()
    {
		options = GUILayout.Toolbar(options, optionsTitle);
		switch (options)
		{
			case 0:
				ItemHandel();
				break;
			case 1:
				SteageHandel();
				break;
			case 2:
				BattleHandel();
				break;
			case 3:
				ItemHandel();
				break;
		}
    }

	private int addItemId = 0;
	private int delItemId = 0;
	private string mArea = "---------------------------------------------------------------------------------------------";

	private void ItemHandel()
	{
		EditorGUILayout.LabelField(mArea);

		//Add Item
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("物品編號 : "); 
		addItemId = EditorGUILayout.IntField (addItemId, GUILayout.Width(100));

		if (GUILayout.Button("AddItem", GUILayout.Width(200)))
		{
			if(addItemId >0){
				WWWForm form = new WWWForm();
				form.AddField("RemoveIndexs", addItemId);
				form.AddField("AddIndexs", 0);

				SendHttp.Get.Command(URLConst.GMAddItem, waitGMAddItem, form);
			}
			else
				ShowHint("請填物品編號");
		}
		EditorGUILayout.EndHorizontal();

		//Del Item
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("物品編號 : "); 
		delItemId = EditorGUILayout.IntField (delItemId, GUILayout.Width(100));
		
		if (GUILayout.Button("DeleteItem", GUILayout.Width(200)))
		{
			if(delItemId >0){
				WWWForm form = new WWWForm();
				form.AddField("RemoveIndexs", delItemId);

				SendHttp.Get.Command(URLConst.GMAddItem, waitGMAddItem, form);
			}
			else
				ShowHint("請填物品編號");
		}
		EditorGUILayout.EndHorizontal();
	}

	private void waitGMAddItem(bool ok, WWW www)
	{
		if(ok)
		{
			ShowHint("Server Return : " + www.text);
//			TPlayerBank[] playerBank = JsonConvert.DeserializeObject<TPlayerBank[]>(www.text);
//			
//			foreach(var bank in playerBank)
//			{
//				Debug.Log(bank);
//			}
//			Visible = false;
//			UICreateRole.Get.ShowFrameView(playerBank);
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
