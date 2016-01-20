using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public struct THint {
	public string Text;
	public Color TextColor;
}

public class UIHint : UIBase
{
	private static UIHint instance = null;
	private const string UIName = "UIHint";
//	private float timer = -1;
	private UILabel mLabel;
	private Queue<THint> textList = new Queue<THint>();

    private const float AutoHideTime = 3f; // 單位: 秒.

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
//			if (!isShow)
//				RemoveUI(UIName);
//			else
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

	protected override void InitCom()
    {
        mLabel = GameObject.Find(UIName + "/Center/ContentView/UIHintLabel").GetComponent<UILabel>();
	    mLabel.text = string.Empty;

        SetBtnFun(UIName + "/Background", Hide);

    }

    public void Hide()
    {
        UIShow(false);
		textList.Clear();
    }

    private IEnumerator autoHide()
    {
        yield return new WaitForSeconds(AutoHideTime);
		if(textList.Count <= 0)
        	Hide();
		else {
			showText(textList.Dequeue());
			StartCoroutine(autoHide());
		}
    }

	private void showText (THint hint) {
		mLabel.text = hint.Text;
		mLabel.effectColor = hint.TextColor;
	}

	private void addText (string text, Color color) {
		THint hint = new THint();
		hint.Text = text;
		hint.TextColor = color;
		textList.Enqueue(hint);
	}

	private bool haveText (string text) {
		if(textList != null && textList.Count > 0)
			for(int i=0; i<textList.Count; i++)
				if(text.Equals(textList.ToArray()[i].Text))
					return true;
		return false;
	}

    public void ShowHint(string text, Color color)
    {
		if(!Visible){
			Show(true);
			mLabel.text = text;
			mLabel.effectColor = color;
			StartCoroutine(autoHide());
		} else 
			if(!haveText(text))
				addText(text, color);
	}
}
