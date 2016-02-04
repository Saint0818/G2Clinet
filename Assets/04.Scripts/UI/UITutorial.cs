using System.Collections;
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

    private GameObject uiHint;
	private GameObject uiClick;
	private GameObject uiCenter;
	private GameObject uiBackground;
	private UIButton buttonClick;
	private UILabel tutorialMessage;
	private TypewriterEffect writeEffect;
	private const int manNum = 2;
	private GameObject[] uiTalk = new GameObject[manNum];
	private UILabel[] labelTalk = new UILabel[manNum];
	private int[] manID = new int[manNum];
	private int[] talkManID = new int[manNum]; //for loading 3D

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
				if (Get.NextEventID > 0 && GamePlayTutorial.Visible)
					GamePlayTutorial.Get.CheckNextEvent(Get.NextEventID);

				UI3DTutorial.UIShow(false);
				Get.Show(isShow);
				//RemoveUI(UIName);
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
		uiBackground = GameObject.Find(UIName + "/CenterBg");
		uiCenter = GameObject.Find(UIName + "/Center");
		uiClick = GameObject.Find(UIName + "/Hint/Click");
        uiHint = GameObject.Find(UIName + "/Hint/Hint");
		buttonClick = uiClick.GetComponent<UIButton>();
		for (int i = 0; i < manNum; i++) {
			uiTalk[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString());
			labelTalk[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString() + "/Name").GetComponent<UILabel>();
		}

		tutorialMessage.text = "";
		string temp = tutorialMessage.processedText;
		writeEffect.ResetToBeginning();
	}
	/*
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
    }*/

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
				else 
					writeEffect.Finish();
			} else 
				UIShow(false);
		}
	}

    IEnumerator waitNextTutorial() {
        yield return new WaitForEndOfFrame();
        OnTutorial();
    }

	IEnumerator showPlayer(TTutorial tu) {
		yield return new WaitForEndOfFrame();

		UI3DTutorial.Get.ShowTutorial(tu, talkManID[0], talkManID[1]);
	}
	
	public void ShowTutorial(int id, int line) {
		try {
			/*if (GameData.ServerVersion != BundleVersion.Version) {
				UIShow(false);
				return;
			}*/

            if (!GameStart.Get.OpenTutorial) {
                UIShow(false);
                return;
            }
			
			NowMessageIndex  = id * 100 + line;
			
			if (GameData.DTutorial.ContainsKey(NowMessageIndex)) {
				if (!Visible) {
					UIShow(true);
					GameFunction.FindTalkManID(id, ref talkManID);
					if (!GameData.Team.HaveTutorialFlag(GameData.DTutorial[NowMessageIndex].ID)) {
						GameData.Team.AddTutorialFlag(GameData.DTutorial[NowMessageIndex].ID);
						WWWForm form = new WWWForm();
						form.AddField("ID", GameData.DTutorial[NowMessageIndex].ID);
						SendHttp.Get.Command(URLConst.AddTutorialFlag, waitAddTutorialFlag, form, false);
					}
				}

                uiHint.SetActive(false);

				TTutorial tu = GameData.DTutorial[NowMessageIndex];
                if (!string.IsNullOrEmpty(tu.UIPath))
                    ShowNextStep(tu.UIPath, tu.Offsetx, tu.Offsety);
                else {
                    if (!string.IsNullOrEmpty(tu.HintPath))
                        ShowHint(tu.HintPath, tu.Offsetx, tu.Offsety);

                    if (!string.IsNullOrEmpty(tu.Text)) {
    					uiCenter.SetActive(true);
    					uiBackground.SetActive(true);
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
    							if (string.IsNullOrEmpty(GameData.Team.Player.Name))
    								labelTalk[i].text = TextConst.S (3404);
    							else
    								labelTalk[i].text = GameData.Team.Player.Name;
    						} else
    							uiTalk[i].SetActive(false);
    					}
    					
    					StartCoroutine(showPlayer(tu));
                    } else {
                        textFinish = true;
                        StartCoroutine(waitNextTutorial());
                    }
				}
			} else {
				Debug.Log(NowMessageIndex.ToString() + " tutorial message index not found.");
				UIShow(false);
			}
		} catch (UnityException e) {
			Debug.Log(e.ToString());
			UIShow(false);
		}
	}

 	public void ShowNextStep(string path, int offsetx, int offsety) {
		try {
    		bool found = false;
    		GameObject obj = GameObject.Find(path);
    		if(obj) {
                UIScrollView sv = obj.GetComponent<UIScrollView>();
                if (sv != null) {
                    UIButton[] objs = obj.GetComponentsInChildren<UIButton>();
                    if (objs != null && objs.Length > 0)
                        obj = objs[0].gameObject;
                }

                Vector3 v = UI2D.Get.Camera2D.WorldToScreenPoint(obj.transform.position);
                if (v.x > 0 && v.x < Screen.width && v.y > 0 && v.y < Screen.height) {
                    UI3DTutorial.UIShow(false);
                    uiCenter.SetActive(false);
                    uiBackground.SetActive(false);
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
    		}
		} catch (UnityException e) {
			Debug.Log("Tutorial click error " + e.ToString());
			UIShow(false);
		}
	}

    public void ShowHint(string path, int offsetx, int offsety) {
        try {
            GameObject obj = GameObject.Find(path);
            UIScrollView sv = obj.GetComponent<UIScrollView>();
            if (sv != null) {
                GameObject[] objs = obj.GetComponents<GameObject>();
                if (objs != null && objs.Length > 0)
                    obj = objs[0];
            }

            if(obj) {
                Vector3 v = obj.transform.position;
                v.x += offsetx;
                v.y += offsety;
                uiHint.SetActive(true);
                uiHint.transform.position = v;
            }
        } catch (UnityException e) {
            Debug.Log("Tutorial click error " + e.ToString());
            UIShow(false);
        }
    }
}