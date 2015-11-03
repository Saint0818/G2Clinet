using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class UITutorial : UIBase {
	private static UITutorial instance = null;
	private const string UIName = "UITutorial";

	private int NowMessageIndex = -1;
	private int clickLayer;
	private GameObject clickObject;

	private GameObject uiClick;
	private GameObject uiCenter;
	private UILabel tutorialMessage;
	private TypewriterEffect writeEffect;
	private const int manNum = 2;
	private GameObject[] uiTalk = new GameObject[manNum];
	private GameObject[] manAnchor = new GameObject[manNum];
	private GameObject[] talkMan = new GameObject[manNum];
	private SkinnedMeshRenderer[] manRender = new SkinnedMeshRenderer[manNum];
	private TAvatar[] manData = new TAvatar[manNum];
	private int[] manBodyType = new int[manNum];
	private UILabel[] labelTalk = new UILabel[manNum];
	private int[] manID = new int[2];

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

	public static UITutorial Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UITutorial;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			if (!isShow) { 
				if (Get.clickObject)
					Get.clickObject.layer = Get.clickLayer;

				RemoveUI(UIName);
			} else
				instance.Show(isShow);
		} else
		if (isShow) 
			Get.Show(isShow);
	}
	
	protected override void InitCom() {
		SetBtnFun (UIName + "/Center/Next", OnTutorial);
		tutorialMessage = GameObject.Find(UIName + "/Center/Message/Text").GetComponent<UILabel>();
		writeEffect = GameObject.Find(UIName + "/Center/Message/Text").GetComponent<TypewriterEffect>();
		uiClick = GameObject.Find(UIName + "/Click");
		uiCenter = GameObject.Find(UIName + "/Center");

		for (int i = 0; i < manNum; i++) {
			uiTalk[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString());
			manAnchor[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString() + "/Man");
			labelTalk[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString() + "/Name").GetComponent<UILabel>();
		}
	}

	public void ShowTutorial(int no, int line) {
		try {
			if (GameData.ServerVersion != BundleVersion.Version) {
				UIShow(false);
				return;
			}

			NowMessageIndex  = no * 100 + line;

			if (GameData.DTutorial.ContainsKey(NowMessageIndex)) {
				UIShow(true);
				TTutorial tu = GameData.DTutorial[NowMessageIndex];

				if (string.IsNullOrEmpty(tu.UIpath)) {
					uiCenter.SetActive(true);
					uiClick.SetActive(false);
					tutorialMessage.gameObject.SetActive(true);
					tutorialMessage.text = tu.Text;
					writeEffect.ResetToBeginning();
					manID[0] = tu.TalkL;
					manID[1] = tu.TalkR;
					for (int i = 0; i < manNum; i++) {
						if (talkMan[i] && talkMan[i].name != manID[i].ToString()) {
							Destroy(talkMan[i]);
							talkMan[i] = null;
						}

						if (GameData.DPlayers.ContainsKey(manID[i])) {
							labelTalk[i].text = GameData.DPlayers[manID[i]].Name;
							if (!talkMan[i]) {
								manData[i] = new TAvatar(manID[i]);
								manBodyType[i] = GameData.DPlayers[manID[i]].BodyType;
							}
						} else 
						if (manID[i] == -1) {
							labelTalk[i].text = GameData.Team.Player.Name;
							if (!talkMan[i]) {
								manData[i] = GameData.Team.Player.Avatar;
								manBodyType[i] = GameData.Team.Player.BodyType;
							}
						}

						if (!talkMan[i] && (GameData.DPlayers.ContainsKey(manID[i]) || manID[i] == -1)) {
							talkMan[i] = new GameObject(manID[i].ToString());
							talkMan[i].transform.parent = manAnchor[i].transform;
							talkMan[i].transform.localPosition = Vector3.zero;
							talkMan[i].transform.eulerAngles = new Vector3(0, 180, 0);
							talkMan[i].transform.localScale = new Vector3(300, 300, 300);
							manRender[i] = null;
						}

						if (talkMan[i]) {
							uiTalk[i].SetActive(true);
							ModelManager.Get.SetAvatar(ref talkMan[i], manData[i], manBodyType[i], EAnimatorType.AvatarControl);
							LayerMgr.Get.SetLayerAllChildren(talkMan[i], ELayer.TopUI.ToString());

							if (!manRender[i])
								manRender[i] = talkMan[i].GetComponentInChildren<SkinnedMeshRenderer>();

							if (manRender[i]) {
								if (i == tu.TalkIndex)
									manRender[i].material.color = new Color32(150, 150, 150, 255);
								else
									manRender[i].material.color = new Color32(75, 75, 75, 255);
							}
						} else
							uiTalk[i].SetActive(false);
					}
				} else
					ShowArrow(tu.UIpath, tu.Offsetx, tu.Offsety);
			} else {
				Debug.Log(NowMessageIndex.ToString() + " tutorial message index not found.");
			}
		} catch (UnityException e) {
			Debug.Log(e.ToString());
		}
	}

    void ButtonClickClose(GameObject button) {
		UIPanel temp = button.GetComponent<UIPanel> ();
		if(temp != null)
			Destroy (temp);

        UIEventListener listen = button.GetComponent<UIEventListener>();
        if (listen)
            Destroy(listen);

		if (clickObject) 
			clickObject.layer = clickLayer;
		
		clickObject = null;

		uiClick.SetActive(false);
        OnTutorial();
    }

	private void waitAddTutorialFlag(bool ok, WWW www) {
		if (ok) {
			TTeam team = JsonConvert.DeserializeObject <TTeam>(www.text, SendHttp.Get.JsonSetting); 
			if (team.TutorialFlags != null)
				GameData.Team.TutorialFlags = team.TutorialFlags;
		}
	} 

	public void OnTutorial() {
		if(!uiClick.activeInHierarchy){
			if (GameData.DTutorial.ContainsKey(NowMessageIndex + 1))
				ShowTutorial(NowMessageIndex / 100, NowMessageIndex % 100 + 1);
			else {
				Show(false);
				if (GameData.DTutorial.ContainsKey(NowMessageIndex)) {
					if (!GameData.Team.HaveTutorialFlag(GameData.DTutorial[NowMessageIndex].ID)) {
						GameData.Team.AddTutorialFlag(GameData.DTutorial[NowMessageIndex].ID);
						WWWForm form = new WWWForm();
						form.AddField("ID", GameData.DTutorial[NowMessageIndex].ID);
						SendHttp.Get.Command(URLConst.AddTutorialFlag, waitAddTutorialFlag, form, false);
					}
				}
			}
		}
	}

 	public GameObject ShowArrow(string path, int offsetx, int offsety) {
		Vector3 v;
		GameObject obj = GameObject.Find(path);
		if(obj) {
			uiCenter.SetActive(false);
			uiClick.SetActive(true);

            UIEventListener.Get(obj).onClick = ButtonClickClose;

			v = obj.transform.position;
			v.x += offsetx;
			v.y += offsety;
			uiClick.transform.position = v;

			if (clickObject) {
				clickObject.layer = clickLayer;
			}

			clickObject = obj;
			clickLayer = obj.layer;

			LayerMgr.Get.SetLayer(obj, ELayer.TopUI);
			UIPanel Panel = obj.GetComponent<UIPanel>();
			if(Panel == null)
				Panel = obj.AddComponent<UIPanel>();
			
			Panel.depth = EUIDepth.TutorialButton.GetHashCode();
			obj.SetActive(false);
			obj.SetActive(true);
			return obj;
		} else {
			Debug.Log("Button not found " + path);
			return null;
		}
	}
}