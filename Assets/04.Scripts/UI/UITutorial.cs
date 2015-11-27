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
		if (instance) {
			if (!isShow) { 
				if (Get.clickObject)
					Get.clickObject.layer = Get.clickLayer;

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
		uiClick = GameObject.Find(UIName + "/Click");
		uiCenter = GameObject.Find(UIName + "/Center");

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
			listen.onClick = null;

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
			UI3DTutorial.UIShow(false);
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
			Show(false);
			return null;
		}
	}
}