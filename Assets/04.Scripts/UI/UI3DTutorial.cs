using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class UI3DTutorial : UIBase {
	private static UI3DTutorial instance = null;
	private const string UIName = "UI3DTutorial";
	
	private const int manNum = 2;
	private GameObject[] manAnchor = new GameObject[manNum];
	private GameObject[] talkMan = new GameObject[manNum];
	private SkinnedMeshRenderer[] manRender = new SkinnedMeshRenderer[manNum];
	private Animator[] manAnimator = new Animator[manNum];
	private TAvatar[] manData = new TAvatar[manNum];
	private int[] manBodyType = new int[manNum];
	private int[] manID = new int[2];
	private int[] actionNo = new int[2];

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

	public static UI3DTutorial Get
	{
		get {
			if (!instance) {
				UI3D.UIShow(true);
				UI3D.Get.ShowCamera(false);
				instance = Load3DUI(UIName) as UI3DTutorial;
			}

			return instance;
		}
	}

	void OnDestroy() {
		releaseTalkMan();
	}

	private void releaseTalkMan() {
		for (int i = 0; i < manRender.Length; i++) 
			manRender[i] = null;

		for (int i = 0; i < talkMan.Length; i++) 
			Destroy(talkMan[i]);
	}

	public static void UIShow(bool isShow, bool closeTutorial){
		if (instance) {
			if (!isShow) {
				if (closeTutorial)
					Get.releaseTalkMan();

				Get.Show(isShow);
				//RemoveUI(UIName);
			} else
				instance.Show(isShow);
		} else
		if (isShow) {
			Get.Show(isShow);
		}
	}
	
	protected override void InitCom() {
		for (int i = 0; i < manNum; i++)
			manAnchor[i] = GameObject.Find(UIName + "/TalkView/Man" + i.ToString());
	}

	public void InitTalkMan(int talkL, int talkR) {
		manID[0] = talkL;
		manID[1] = talkR;

		for (int i = 0; i < manNum; i++) {
			if (!talkMan[i] && (GameData.DPlayers.ContainsKey(manID[i]) || manID[i] == -1)) {
				if (GameData.DPlayers.ContainsKey(manID[i])) {
					manData[i] = new TAvatar(manID[i]);
					manBodyType[i] = GameData.DPlayers[manID[i]].BodyType;
				} else 
				if (manID[i] == -1) {
					GameFunction.ItemIdTranslateAvatar(ref GameData.Team.Player.Avatar, GameData.Team.Player.Items);
					manData[i] = GameData.Team.Player.Avatar;
					manBodyType[i] = GameData.Team.Player.BodyType;
				}

				talkMan[i] = new GameObject(manID[i].ToString());
				talkMan[i].transform.parent = manAnchor[i].transform;
				talkMan[i].transform.localPosition = Vector3.zero;
				talkMan[i].transform.localScale = Vector3.one;
				talkMan[i].transform.localRotation = Quaternion.identity;
				ModelManager.Get.SetAvatar(ref talkMan[i], manData[i], manBodyType[i], EAnimatorType.TalkControl);
				LayerMgr.Get.SetLayerAllChildren(talkMan[i], ELayer.UIPlayer.ToString());
				manRender[i] = talkMan[i].GetComponentInChildren<SkinnedMeshRenderer>();
				manAnimator[i] = talkMan[i].GetComponent<Animator>();
			}
		}
	}

	public void ShowTutorial(TTutorial tu, int talkL, int talkR) {
		if (!Visible) {
			UIShow(true, false);
			InitTalkMan(talkL, talkR);
		}

		manID[0] = tu.TalkL;
		manID[1] = tu.TalkR;
		actionNo[0] = tu.ActionL;
		actionNo[1] = tu.ActionR;
		for (int i = 0; i < manNum; i++) {
			if (talkMan[i]) {
				if (manRender[i] && (GameData.DPlayers.ContainsKey(manID[i]) || manID[i] == -1)) {
					talkMan[i].SetActive(true);
					if (i == tu.TalkIndex) {
						manRender[i].material.color = new Color32(150, 150, 150, 255);

						if (manAnimator[i]) {
							int no = actionNo[i];
							if (no == 0)
								no = Random.Range(0, 2) + 1;
							
							manAnimator[i].Play("Talk" + no.ToString());
						}
					} else
						manRender[i].material.color = new Color32(75, 75, 75, 255);
				} else
					talkMan[i].SetActive(false);
			}
		}
	}
	/*
	public void ShowTutorial(TTutorial tu) {
		if (!Visible)
			UIShow(true);

		manID[0] = tu.TalkL;
		manID[1] = tu.TalkR;
		actionNo[0] = tu.ActionL;
		actionNo[1] = tu.ActionR;
		for (int i = 0; i < manNum; i++) {
			if (GameData.DPlayers.ContainsKey(manID[i])) {
				if (!talkMan[i]) {
					manData[i] = new TAvatar(manID[i]);
					manBodyType[i] = GameData.DPlayers[manID[i]].BodyType;
				}
			} else 
			if (manID[i] == -1) {
				if (!talkMan[i]) {
					GameFunction.ItemIdTranslateAvatar(ref GameData.Team.Player.Avatar, GameData.Team.Player.Items);
					manData[i] = GameData.Team.Player.Avatar;
					manBodyType[i] = GameData.Team.Player.BodyType;
				}
			}

			if (!talkMan[i] && (GameData.DPlayers.ContainsKey(manID[i]) || manID[i] == -1)) {
				talkMan[i] = new GameObject(manID[i].ToString());
				talkMan[i].transform.parent = manAnchor[i].transform;
				talkMan[i].transform.localPosition = Vector3.zero;
				talkMan[i].transform.localScale = Vector3.one;
				talkMan[i].transform.localRotation = Quaternion.identity;
				manRender[i] = null;
			}

			if (talkMan[i]) {
				ModelManager.Get.SetAvatar(ref talkMan[i], manData[i], manBodyType[i], EAnimatorType.TalkControl);
				LayerMgr.Get.SetLayerAllChildren(talkMan[i], ELayer.UIPlayer.ToString());

				if (!manRender[i])
					manRender[i] = talkMan[i].GetComponentInChildren<SkinnedMeshRenderer>();

				if (manRender[i]) {
					if (i == tu.TalkIndex) {
						manRender[i].material.color = new Color32(150, 150, 150, 255);
						Animator ani = talkMan[i].GetComponent<Animator>();
						if (ani) {
							int no = actionNo[i];
							if (no == 0)
								no = Random.Range(0, 2) + 1;

							ani.Play("Talk" + no.ToString());
						}
					} else
						manRender[i].material.color = new Color32(75, 75, 75, 255);
				}
			}
		}
	}*/
}