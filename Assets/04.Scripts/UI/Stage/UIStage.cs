using UnityEngine;
using System.Collections;
using GamePlayEnum;
using GameEnum;

public class UIStage : UIBase {
	private static UIStage instance = null;
	private const string UIName = "UIStage";

//	private GameObject offsetStage;
//	private UIDraggableCamera cameraStage;
	private Camera cameraScrollView;

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static UIStage Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIStage;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		}
		else
		if (isShow)
			Get.Show(isShow);
	}

	protected override void InitCom() {
		SetBtnFun(UIName + "/Center/ButtonCloseStage", OnClose);

		GameObject obj = Resources.Load("Prefab/UI/Items/ItemJoinStage") as GameObject;
		if (obj) {
//			offsetStage = GameObject.Find(UIName + "/Center/StageInfo/View/Anchor/Offset");
//			cameraStage = GameObject.Find(UIName + "/Center/StageInfo/View/ViewCamera").GetComponent<UIDraggableCamera>();
			cameraScrollView = GameObject.Find(UIName + "/Center/StageInfo/View/ViewCamera").GetComponent<Camera>();

//			if (GameData.StageData != null && GameData.StageData.Length > 0) {
//				for (int i = 0; i < GameData.StageData.Length; i ++) {
//					GameObject item = Instantiate(obj) as GameObject;
//					item.name = "Stage" + GameData.StageData[i].ID.ToString();
//					item.GetComponent<UILabel>().text = GameData.StageData[i].Name;
//					SetBtnFun(item.name, OnJoinStage);
//					item.transform.parent = offsetStage.transform;
//					item.transform.localScale = Vector3.one;
//					item.transform.localPosition = Vector3.zero;
//					item.GetComponent<UIDragCamera>().draggableCamera = cameraStage;
//					
//					item.transform.localPosition = new Vector3(0, -i * 80, 0);
//				}
//			}
			
			cameraScrollView.transform.localPosition = new Vector3(-20, 67, 0);
		}
	}

	public void OnClose()
    {
		UIShow(false);
        UIMainLobby.Get.Show();
	}

	public void OnJoinStage() {
//		int id = -1;
//		if (int.TryParse(UIButton.current.name.Substring(5, UIButton.current.name.Length - 5), out id) && 
//		    GameData.DStageData.ContainsKey(id) )
//        {
//			GameData.StageID = id;
//
//			GameStart.Get.CourtMode =  (ECourtMode)GameData.DStageData[id].CourtMode;
//			GameStart.Get.WinMode =  (EWinMode)GameData.DStageData[id].WinMode;
//
//			if (GameData.DStageData[id].WinValue > 0)
//				GameStart.Get.GameWinValue =  GameData.DStageData[id].WinValue;
//
//			if (GameData.DStageData[id].FriendNumber > 0)
//				GameStart.Get.FriendNumber =  GameData.DStageData[id].FriendNumber;
//
//			UIShow(false);
//			SceneMgr.Get.ChangeLevel(ESceneName.SelectRole);
//		}
	}
}
