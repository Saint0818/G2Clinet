using UnityEngine;

public class UIHint : UIBase
{
	private static UIHint instance = null;
	private const string UIName = "UIHint";
//	private float timer = -1;
	private UILabel mLabel;

	public static bool Visible
	{
		get
		{
			if(instance)
				return instance.gameObject.activeInHierarchy;

            return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if(instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		}
		else
		if(isShow)
			Get.Show(isShow);
	}

	public static UIHint Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIHint;
			
			return instance;
		}
	}

	protected override void InitCom() {
//		for (int i = 0; i < LabelHints.Length; i ++) {
//			LabelHints[i] = GameObject.Find(UIName + "/Message" + i.ToString()).GetComponent<UILabel>();
//			LabelHints[i].text = "";
//			LabelHints[i].gameObject.SetActive(false);
//		}

        mLabel = GameObject.Find(UIName + "/Center/ContentView/UIHintLabel").GetComponent<UILabel>();
	    mLabel.text = string.Empty;

        SetBtnFun(UIName + "/Background", Hide);
    }

    public void Hide()
    {
        UIShow(false);
    }

    public void ShowHint(string text, Color color)
    {
		if(!Visible)
			Show(true);

	    mLabel.text = text;
	    mLabel.effectColor = color;

//		for (int i = LabelHints.Length-1; i >= 0; i --) 
//			if (LabelHints[i].gameObject.activeInHierarchy && LabelHints[i].text == text) 
//				return;
//
//		timer = 3;
//		bool flag = false;
//		for (int i = 0; i < LabelHints.Length; i ++) {
//
//			if (!LabelHints[i].gameObject.activeInHierarchy) {
//				LabelHints[i].gameObject.SetActive(true);
//				LabelHints[i].text = text;
//				LabelHints[i].effectColor = color;
//				flag = true;
//				
//				break;
//			}
//		}
//		
//		if (!flag) {
//			for (int i = LabelHints.Length-1; i > 0; i --) {
//				if (LabelHints[i].gameObject.activeInHierarchy) {
//					if (i > 0) {
//						LabelHints[i-1].text = LabelHints[i].text;
//						LabelHints[i-1].effectColor = LabelHints[i].effectColor;
//					}
//					break;
//				}
//			}
//			
//			LabelHints[LabelHints.Length-1].text = text;
//			LabelHints[LabelHints.Length-1].effectColor = color;
//		} 
	}

//	void Update ()
//	{
//		if (timer > 0) {
//			timer -= Time.deltaTime;
//			if (timer <= 0) {
//				for (int i = LabelHints.Length-1; i >= 0; i --) {
//					if (LabelHints[i].gameObject.activeInHierarchy) {
//						if (i > 0) {
//							for (int j = 0; j < i; j ++) {
//								LabelHints[j].text = LabelHints[j+1].text;
//								LabelHints[j].effectColor = LabelHints[j+1].effectColor;
//							}
//						}
//						
//						LabelHints[i].gameObject.SetActive(false);
//						break;
//					}
//				}
//
//				if (LabelHints[0].gameObject.activeInHierarchy)
//					timer = 3;
//			}
//		}
//	}

//	public bool NoHint() {
//		if (!Visible)
//			return true;
//		else {
//			for (int i = LabelHints.Length-1; i >= 0; i --) 
//				if (LabelHints[i].gameObject.activeInHierarchy)
//					return false;
//
//			return true;
//		}
//	}
}
