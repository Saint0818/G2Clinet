using GameStruct;
using Newtonsoft.Json;
using UnityEngine;

public class UITutorial : UIBase {
	private static UITutorial instance = null;
	private const string UIName = "UITutorial";
	private int NowMessageIndex = -1;
	private GameObject uiClick;
	private GameObject uiCenter;
	private UILabel tutorialMessage;

	private GameObject clickObject;
	private int clickLayer;

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
		tutorialMessage = GameObject.Find(UIName + "/Center/Message").GetComponent<UILabel>();
		uiClick = GameObject.Find(UIName + "/Click");
		uiCenter = GameObject.Find(UIName + "/Center");
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
