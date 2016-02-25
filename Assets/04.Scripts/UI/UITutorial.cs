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

    private GameObject spriteHint;
    private GameObject uiBottomHint;
    private GameObject uiCenterHint;
	private GameObject uiClick;
	private GameObject uiCenter;
    private GameObject uiMessage;
	private GameObject uiBackground;
	private UIButton buttonClick;
    private UILabel labelMessage;
    private UILabel labelBottomHint;
    private UILabel labelCenterHint;
	private TypewriterEffect writeEffect;
    private TypewriterEffect writeEffectBottomHint;
    private TypewriterEffect writeEffectCenterHint;
    private TweenScale tweenScale;
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
            UINotic.UIShow(false);
        
        if (instance) {
			if (!isShow) {
                if (instance.tweenScale != null)
                    instance.tweenScale.enabled = false;

                if (instance.NextEventID > 0 && GamePlayTutorial.Visible)
                    GamePlayTutorial.Get.CheckNextEvent(instance.NextEventID);

                UI3DTutorial.Get.ReleaseTalkMan();
				UI3DTutorial.UIShow(false);
                instance.Show(isShow);
				//RemoveUI(UIName);
			} else
				instance.Show(isShow);
		} else
		if (isShow) 
			Get.Show(isShow);
	}
	
	protected override void InitCom() {
		SetBtnFun (UIName + "/Center/Next", OnTutorial);
        uiBottomHint = GameObject.Find(UIName + "/Bottom");
        uiCenterHint = GameObject.Find(UIName + "/Center/Hint");
        labelBottomHint = GameObject.Find(UIName + "/Bottom/Hint/ScrollView/Label").GetComponent<UILabel>();
        labelCenterHint = GameObject.Find(UIName + "/Center/Hint/ScrollView/Label").GetComponent<UILabel>();
        writeEffectBottomHint = GameObject.Find(UIName + "/Bottom/Hint/ScrollView/Label").GetComponent<TypewriterEffect>();
        writeEffectCenterHint = GameObject.Find(UIName + "/Center/Hint/ScrollView/Label").GetComponent<TypewriterEffect>();
        uiMessage = GameObject.Find(UIName + "/Center/Message");
		labelMessage = GameObject.Find(UIName + "/Center/Message/Text").GetComponent<UILabel>();
		writeEffect = GameObject.Find(UIName + "/Center/Message/Text").GetComponent<TypewriterEffect>();
		writeEffect.onFinished.Add(new EventDelegate(OnTextFinish));
		uiBackground = GameObject.Find(UIName + "/CenterBg");
		uiCenter = GameObject.Find(UIName + "/Center");
		uiClick = GameObject.Find(UIName + "/Hint/Click");
        spriteHint = GameObject.Find(UIName + "/Hint/SpriteHint");
		buttonClick = uiClick.GetComponent<UIButton>();
		for (int i = 0; i < manNum; i++) {
			uiTalk[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString());
			labelTalk[i] = GameObject.Find(UIName + "/Center/Talk" + i.ToString() + "/Name").GetComponent<UILabel>();
		}

		labelMessage.text = "";
		string temp = labelMessage.processedText;
		writeEffect.ResetToBeginning();
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
        if (Visible && !uiClick.activeInHierarchy){
			if (GameData.DTutorial.ContainsKey(NowMessageIndex + 1)) {
				if (textFinish || string.IsNullOrEmpty(labelMessage.text) || labelMessage.text ==" ")
					ShowTutorial(NowMessageIndex / 100, NowMessageIndex % 100 + 1);
				else 
					writeEffect.Finish();
			} else 
				UIShow(false);
		}
	}

    IEnumerator waitNextTutorial(float sec) {
        yield return new WaitForSeconds(sec);
        OnTutorial();
    }

	IEnumerator showPlayer(TTutorial tu) {
		yield return new WaitForEndOfFrame();

		UI3DTutorial.Get.ShowTutorial(tu, talkManID[0], talkManID[1]);
	}

    private GameObject findGameObject(string path) {
        GameObject obj = GameObject.Find(path);
        if(obj) {
            UIScrollView sv = obj.GetComponent<UIScrollView>();
            if (sv != null) {
                UIButton[] objs = obj.GetComponentsInChildren<UIButton>();
                if (objs != null && objs.Length > 0)
                    obj = objs[0].gameObject;
            }
        }

        return obj;
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

                if (tweenScale != null)
                    tweenScale.enabled = false;

                spriteHint.SetActive(false);
                uiBottomHint.SetActive(false);
                uiCenterHint.SetActive(false);
                uiMessage.SetActive(false);
                uiBackground.SetActive(false);

				TTutorial tu = GameData.DTutorial[NowMessageIndex];

                if (tu.Wait >= 0.1f)
                    StartCoroutine(waitNextTutorial(tu.Wait));
                else
                if (!string.IsNullOrEmpty(tu.UIPath))
                    ShowNextStep(tu.UIPath, tu.Offsetx, tu.Offsety);
                else {
                    if (!string.IsNullOrEmpty(tu.ScalePath))
                        ShowScale(ref tu);
                    else
                    if (!string.IsNullOrEmpty(tu.HintPath))
                        ShowHint(ref tu);
                    else
                    if (!string.IsNullOrEmpty(tu.Text)) 
                        ShowMessage(ref tu);
                    else {
                        textFinish = true;
                        StartCoroutine(waitNextTutorial(0.1f));
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
            GameObject obj = findGameObject(path);
    		if(obj) {
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

    public void ShowScale(ref TTutorial tu) {
        try {
            GameObject obj = findGameObject(tu.ScalePath);
            if (obj) {
                UI3DTutorial.UIShow(false);
                uiCenter.SetActive(true);
                spriteHint.SetActive(true);
                spriteHint.transform.position = obj.transform.position;

                for (int i = 0; i < labelTalk.Length; i++)
                    labelTalk[i].text = "";
                
                obj = findGameObject(tu.ScalePath);
                if (obj) {
                    tweenScale = obj.AddComponent<TweenScale>();
                    tweenScale.from = Vector3.one;
                    tweenScale.to = new Vector3(2f, 2f, 2f);
                    tweenScale.duration = 0.3f;
                    tweenScale.style = UITweener.Style.PingPong;
                    tweenScale.PlayForward();
                }

                if (!string.IsNullOrEmpty(tu.Text)) {
                    uiBottomHint.SetActive(true);
                    labelBottomHint.text = tu.Text;
                    writeEffectBottomHint.ResetToBeginning();
                }

                if (!string.IsNullOrEmpty(tu.Hint)) {
                    uiCenterHint.SetActive(true);
                    uiCenterHint.transform.localPosition = new Vector3(tu.Offsetx, tu.Offsety, 0);
                    labelCenterHint.text = tu.Hint;
                    writeEffectCenterHint.ResetToBeginning();
                }
            } else {
                Debug.Log("No scale path " + tu.HintPath);
                StartCoroutine(waitNextTutorial(0.1f));
            }
        } catch (UnityException e) {
            Debug.Log("Scale path error " + e.ToString());
            UIShow(false);
        }
    }

    public void ShowHint(ref TTutorial tu) {
        try {
            GameObject obj = findGameObject(tu.HintPath);
            if (obj) {
                for (int i = 0; i < labelTalk.Length; i++)
                    labelTalk[i].text = "";

                UI3DTutorial.UIShow(false);
                uiCenter.SetActive(true);
                spriteHint.SetActive(true);
                spriteHint.transform.position = obj.transform.position;

                if (!string.IsNullOrEmpty(tu.Text)) {
                    uiBottomHint.SetActive(true);
                    labelBottomHint.text = tu.Text;
                    writeEffectBottomHint.ResetToBeginning();
                }

                if (!string.IsNullOrEmpty(tu.Hint)) {
                    uiCenterHint.SetActive(true);
                    uiCenterHint.transform.localPosition = new Vector3(tu.Offsetx, tu.Offsety, 0);
                    labelCenterHint.text = tu.Hint;
                    writeEffectCenterHint.ResetToBeginning();
                }
            } else {
                Debug.Log("No hint path " + tu.HintPath);
                StartCoroutine(waitNextTutorial(0.1f));
            }
        } catch (UnityException e) {
            Debug.Log("Hint path error " + e.ToString());
            UIShow(false);
        }
    }

    public void ShowMessage(ref TTutorial tu) {
        uiMessage.SetActive(true);
        uiCenter.SetActive(true);
        uiBackground.SetActive(true);
        uiClick.SetActive(false);
        labelMessage.text = tu.Text;
        string temp = labelMessage.processedText;
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
    }
}