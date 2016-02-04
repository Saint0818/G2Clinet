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
	private Animator animator;
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
		try {
		animator = gameObject.GetComponent<Animator>();
        mLabel = GameObject.Find(UIName + "/Center/ContentView/UIHintLabel").GetComponent<UILabel>();
	    mLabel.text = string.Empty;
		} catch {}
    }

    public void Hide()
    {
        UIShow(false);
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
		animator.SetTrigger("Restart");
	}

	private void showText (string text, Color color) {
		mLabel.text = text;
		mLabel.effectColor = color;
		animator.SetTrigger("Restart");
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

	public void ShowHint(string text, Color color, bool isForce = false)
    {
        if (GameData.Team.Player.Lv == 0)
            return;

		if(!isForce) {
			if(!Visible){
				Show(true);
				showText(text, color);
				StartCoroutine(autoHide());
			} else 
				if(!haveText(text))
					addText(text, color);
		} else {
			if(!Visible)
				Show(true);
			textList.Clear();
			showText(text, color);
		}
	}
}
