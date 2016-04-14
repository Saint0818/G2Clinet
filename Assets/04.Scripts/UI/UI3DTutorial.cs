using GameStruct;
using UnityEngine;

public class UI3DTutorial : UIBase {
	private static UI3DTutorial instance = null;
	private const string UIName = "UI3DTutorial";
	
	private const int manNum = 2;
	private GameObject[] manAnchor = new GameObject[manNum];
	private GameObject[] talkMan = new GameObject[manNum];
    private TAvatarLoader[] manLoader = new TAvatarLoader[manNum];
	private TAvatar[] manData = new TAvatar[manNum];
	private int[] manBodyType = new int[manNum];
    private int[] manID = new int[manNum];
    private int[] actionNo = new int[manNum];

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
			if (!instance)
				instance = Load3DUI(UIName) as UI3DTutorial;

			return instance;
		}
	}

	void OnDestroy() {
		ReleaseTalkMan();
	}

	public void ReleaseTalkMan() {
        for (int i = 0; i < manNum; i++) {
            Destroy(talkMan[i]);
            manID[i] = 0;
        }
	}

	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow) {
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
		if (manID[0] != 0 && manID[0] != talkL) {
            if (talkMan[0]) {
			    Destroy(talkMan[0]);
                talkMan[0] = null;
            }
		}

		if (manID[1] != 0 && manID[1] != talkR) {
            if (talkMan[0]) {
			    Destroy(talkMan[1]);
                talkMan[1] = null;
            }
		}

		manID[0] = talkL;
		manID[1] = talkR;

		for (int i = 0; i < manNum; i++) {
			if (!talkMan[i] && (GameData.DPlayers.ContainsKey(manID[i]) || manID[i] == -1)) {
				if (GameData.DPlayers.ContainsKey(manID[i])) {
					manData[i] = new TAvatar(manID[i]);
					manBodyType[i] = GameData.DPlayers[manID[i]].BodyType;
				} else 
				if (manID[i] == -1) {
					//GameFunction.ItemIdTranslateAvatar(ref GameData.Team.Player.Avatar, GameData.Team.Player.Items);
                    GameData.Team.PlayerInit();
					manData[i] = GameData.Team.Player.Avatar;
					manBodyType[i] = GameData.Team.Player.BodyType;
				}

                manLoader[i] = TAvatarLoader.Load(manBodyType[i], manData[i], ref talkMan[i], manAnchor[i], new TLoadParameter(ELayer.UIPlayer));
			}
		}
	}

	public void ShowTutorial(TTutorial tu, int talkL, int talkR) {
		if (!Visible) {
			UIShow(true);
			InitTalkMan(talkL, talkR);
		}

		actionNo[0] = tu.ActionL;
		actionNo[1] = tu.ActionR;
		for (int i = 0; i < manNum; i++) {
			if (talkMan[i]) {
                if (manLoader[i] && (GameData.DPlayers.ContainsKey(manID[i]) || manID[i] == -1)) {
					talkMan[i].SetActive(true);
					if (i == tu.TalkIndex) {
                        manLoader[i].MaterialColor = new Color32(150, 150, 150, 255);
						int no = actionNo[i];
						if (no == 0)
							no = Random.Range(0, 2) + 1;
						
                        manLoader[i].Play("Talk" + no.ToString());
					} else
                        manLoader[i].MaterialColor = new Color32(75, 75, 75, 255);
				} else
					talkMan[i].SetActive(false);
			}
		}
	}
}