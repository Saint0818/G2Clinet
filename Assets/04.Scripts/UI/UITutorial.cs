using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class UITutorial : UIBase {
	private static UITutorial instance = null;
	private const string UIName = "UITutorial";

	public int NextEventID = 0;
	private int NowMessageIndex = -1;
	private int clickLayer;
	private bool textFinish = false;

	private GameObject clickObject;
	private GameObject uiClick;
	private GameObject uiCenter;
	private UIButton buttonClick;
	private UILabel tutorialMessage;
	private TypewriterEffect writeEffect;
	private const int manNum = 2;
	private GameObject[] uiTalk = new GameObject[manNum];
	private UILabel[] labelTalk = new UILabel[manNum];
	private int[] manID = new int[2];

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static UITutorial Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UITutorial;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow) {
		if (isShow)
			UIAnnouncement.UIShow(false);

		if (instance) {
			if (!isShow) { 
				//if (Get.clickObject)
				//	Get.clickObject.layer = Get.clickLayer;

				if (Get.NextEventID > 0 && GamePlayTutorial.Visible)
					GamePlayTutorial.Get.CheckNextEvent(Get.NextEventID);

				UI3DTutorial.UIShow(false);
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
		writeEffect.onFinished.Add(new EventDelegate(OnTextFinish));
		uiCenter = GameObject.Find(UIName + "/Center");
		uiClick = GameObject.Find(UIName + "/Hint/Click");
		buttonClick = uiClick.GetComponent<UIButton>();
		for (int i = 0; i < manNum; i++) {
			uiTalk[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString());
			labelTalk[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString() + "/Name").GetComponent<UILabel>();
		}

		tutorialMessage.text = "";
		string temp = tutorialMessage.processedText;
		writeEffect.ResetToBeginning();
	}

	public void ShowTutorial(int no, int line) {
		try {
			/*if (GameData.ServerVersion != BundleVersion.Version) {
				UIShow(false);
				return;
			}*/

			NowMessageIndex  = no * 100 + line;

			if (GameData.DTutorial.ContainsKey(NowMessageIndex)) {
				if (!Visible)
					UIShow(true);

				TTutorial tu = GameData.DTutorial[NowMessageIndex];
				if (string.IsNullOrEmpty(tu.UIpath)) {
					uiCenter.SetActive(true);
					uiClick.SetActive(false);
					tutorialMessage.text = tu.Text;
					string temp = tutorialMessage.processedText;
					writeEffect.ResetToBeginning();
					textFinish = false;
					manID[0] = tu.TalkL;
					manID[1] = tu.TalkR;
					for (int i = 0; i < manNum; i++) {
						if (GameData.DPlayers.ContainsKey(manID[i])) {
							uiTalk[i].SetActive(true);
							labelTalk[i].text = GameData.DPlayers[manID[i]].Name;
						} else 
						if (manID[i] == -1) {
							uiTalk[i].SetActive(true);
							labelTalk[i].text = GameData.Team.Player.Name;
						} else
							uiTalk[i].SetActive(false);
					}

					UI3DTutorial.Get.ShowTutorial(ref tu);
				} else
					ShowHint(tu.UIpath, tu.Offsetx, tu.Offsety);
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
			listen.onClick = null;

		if (clickObject) 
			clickObject.layer = clickLayer;
		
		clickObject = null;

		uiClick.SetActive(false);
        OnTutorial();
    }

	public void OnClickHint() {
		UIEventListener listen = buttonClick.GetComponent<UIEventListener>();
		if (listen) {
			listen.onClick = null;
			listen.onPress = null;
		}

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

	public void OnTextFinish() {
		textFinish = true;
	}

	public void OnTutorial() {
		if(!uiClick.activeInHierarchy){
			if (GameData.DTutorial.ContainsKey(NowMessageIndex + 1)) {
				if (textFinish || string.IsNullOrEmpty(tutorialMessage.text) || tutorialMessage.text ==" ")
					ShowTutorial(NowMessageIndex / 100, NowMessageIndex % 100 + 1);
				else {
					writeEffect.Finish();
				}
			} else {
				UIShow(false);
				if (GameData.DTutorial.ContainsKey(NowMessageIndex) && GameData.DTutorial[NowMessageIndex].Kind == 0) {
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

 	public void ShowHint(string path, int offsetx, int offsety) {
		bool found = false;
		GameObject obj = GameObject.Find(path);
		if(obj) {
			UI3DTutorial.UIShow(false);
			uiCenter.SetActive(false);
			uiClick.SetActive(true);


			buttonClick.onClick.Clear();
			UIButton btn = obj.GetComponent<UIButton>();
			if (btn && btn.onClick.Count > 0) {
				buttonClick.onClick.Add(btn.onClick[0]);
				found = true;
			} else {
				UIEventListener el = obj.GetComponent<UIEventListener>();
				if (el) {
					if (el.onPress != null) {
						UIEventListener.Get(buttonClick.gameObject).onPress = el.onPress;
						found = true;
					}

					if (el.onClick != null) {
						UIEventListener.Get(buttonClick.gameObject).onClick = el.onClick;
						found = true;
					}
				} else {
					UIStageSmall ss = obj.GetComponent<UIStageSmall>();
					if (ss) {
						buttonClick.onClick.Add(new EventDelegate(ss.OnClick));
						found = true;
					}
				}
			}
		}

		if (found) {
			buttonClick.onClick.Add(new EventDelegate(OnClickHint));

			buttonClick.name = obj.name;
			Vector3 v = obj.transform.position;
			v.x += offsetx;
			v.y += offsety;
			uiClick.transform.position = v;
		} else {
			Debug.Log("Tutorial click event not found " + path);
			UIShow(false);
			if (GameData.DTutorial.ContainsKey(NowMessageIndex) && GameData.DTutorial[NowMessageIndex].Kind == 0) {
				if (!GameData.Team.HaveTutorialFlag(GameData.DTutorial[NowMessageIndex].ID)) {
					GameData.Team.AddTutorialFlag(GameData.DTutorial[NowMessageIndex].ID);
					WWWForm form = new WWWForm();
					form.AddField("ID", GameData.DTutorial[NowMessageIndex].ID);
					SendHttp.Get.Command(URLConst.AddTutorialFlag, waitAddTutorialFlag, form, false);
				}
			}
		}

		//UIEventListener.Get(obj).onClick = ButtonClickClose;

		//if (clickObject) {
		//	clickObject.layer = clickLayer;
		//}

		//clickObject = obj;
		//clickLayer = obj.layer;

		//LayerMgr.Get.SetLayer(obj, ELayer.TopUI);
		//UIPanel Panel = obj.GetComponent<UIPanel>();
		//if(Panel == null)
		//	Panel = obj.AddComponent<UIPanel>();
		
		//Panel.depth = EUIDepth.TutorialButton.GetHashCode();
		//obj.SetActive(false);
		//obj.SetActive(true);
	}
}