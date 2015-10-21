using UnityEngine;
using System.Collections;
using GameStruct;

public class UITutorial : UIBase {
	private static UITutorial instance = null;
	private const string UIName = "UITutorial";
	private int NowMessageIndex = -1;
	private GameObject uiClick;
	private GameObject uiBG;
	private UILabel tutorialMessage;
	private UILabel tutorialTitle;

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
		if(instance) {
			if (!isShow) {
				Get.Show(isShow);
			} else
				instance.Show(isShow);
		} else
		if(isShow) 
			Get.Show(isShow);
	}
	
	protected override void InitCom() {
		SetBtnFun (UIName + "/Window/Close", OnTutorial);
		tutorialMessage = GameObject.Find(UIName + "/Center/Message").GetComponent<UILabel>();
		tutorialTitle = GameObject.Find(UIName + "/Center/Label").GetComponent<UILabel>();
		uiClick = GameObject.Find(UIName + "/Click");
		
	}

	public void ShowTutorial(int no, int line, bool autoClose=true) {
		if (GameData.ServerVersion != BundleVersion.Version) {
			UIShow(false);
			return;
		}
		NowMessageIndex  = no * 100 + line;

		if (GameData.DTutorial.ContainsKey(NowMessageIndex)) {
			TTutorial tu = GameData.DTutorial[NowMessageIndex];

			GameObject obj = ShowArrow(tu.UIpath, tu.Offsetx, tu.Offsety);
			if (obj && autoClose) {
				if(tu.UIpath != "AutoFind"){
					UIPanel Panel = obj.GetComponent<UIPanel>();
					if(Panel == null)
						Panel = obj.AddComponent<UIPanel>();

					Panel.depth = 3;
					obj.layer = LayerMask.NameToLayer("TopView");
					obj.SetActive(false);
					obj.SetActive(true);
				}
			} else {
				if(!Visible)
				  	Show(true);

				tutorialMessage.gameObject.SetActive(false);
				tutorialMessage.gameObject.SetActive(true);
				tutorialMessage.text = GameData.DTutorial[NowMessageIndex].Text;
				tutorialTitle.text = GameData.DTutorial[NowMessageIndex].Title;
			}
		}
		else{
			Debug.Log(NowMessageIndex.ToString() + " tutorial message index not found.");
		}
	}

    void ButtonClickClose(GameObject button) {
		UIPanel temp = button.GetComponent<UIPanel> ();
		if(temp != null)
			Destroy (temp);

		button.layer = LayerMask.NameToLayer("GameUI");
        UIEventListener listen = button.GetComponent<UIEventListener>();
        if (listen)
            Destroy(listen);

        OnTutorial();
    }

	public void OnTutorial() {
		if(NowMessageIndex > 100){
			if (GameData.DTutorial.ContainsKey(NowMessageIndex + 1)) {
				ShowTutorial(NowMessageIndex / 100, NowMessageIndex % 100 + 1);
			} else 
				Show(false);
		}else
			Show(false);
	}

 	public GameObject ShowArrow(string path, int offsetx, int offsety) {
		Vector3 v;
		
		switch(path){
		case "AutoFind":
			uiClick.SetActive(true);
			v = new Vector3(offsetx, offsety, uiClick.transform.localPosition.z);
			uiClick.transform.localPosition = v;
			return null;
		default:
			GameObject obj = GameObject.Find(path);
			if(obj) {
				uiClick.SetActive(true);
				v = obj.transform.localPosition;
				v.x = offsetx;
				v.y = offsety;
				uiClick.transform.localPosition = v;
				uiBG.SetActive(false);
				return obj;
			} else {
				uiBG.SetActive(false);
				return null;
			}
		}
	}
}
