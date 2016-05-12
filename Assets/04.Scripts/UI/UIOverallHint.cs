using UnityEngine;
using System.Collections;

public class UIOverallHint : UIBase {
	private static UIOverallHint instance = null;
	private const string UIName = "UIOverallHint";

	private Animator animator;
	private UILabel overallHint;
	private GameObject[] goArrow = new GameObject[2];

	private float showTime = 3;
	private float waitTime = 0.5f;
	private float ranTime = 0.5f;
	private float tempAValue;

	private bool isClose = true;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
					RemoveUI(instance.gameObject);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else
			if (isShow) 						
				Get.Show(isShow);
	}

	public static UIOverallHint Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIOverallHint;

			return instance;
		}
	}

	void FixedUpdate () {
		if(showTime > 0) {
			showTime -= Time.deltaTime;
			if(showTime <= 0) 
				UIShow(false);
			else if(showTime <= 0.5f && !isClose) {
				isClose = true;
				animator.SetTrigger("Close");
			}
		}

		if(waitTime > 0) {
			waitTime -= Time.deltaTime;
			if(waitTime <= 0)
				waitTime = 0;
		}

		if(ranTime > 0 && waitTime <= 0) {
			ranTime -= Time.deltaTime;
			overallHint.text = string.Format("{0:00.000}", Random.Range(10f, 99f));
			if(ranTime <= 0)
				overallHint.text = tempAValue.ToString();
		}
	}

	protected override void InitCom() {
		animator = gameObject.GetComponent<Animator>();
		overallHint = GameObject.Find(UIName + "/Window/Bottom/OverallView/OverallLabel").GetComponent<UILabel>();
		goArrow[0] = GameObject.Find(UIName + "/Window/UpArrow");
		goArrow[1] = GameObject.Find(UIName + "/Window/DownArrow");
	}

	private void hideArrow () {
		for(int i=0; i<goArrow.Length; i++)
			goArrow[i].SetActive(false);
	}

	private void showArrow (bool isUp) {
		hideArrow ();
		if(isUp)
			goArrow[0].SetActive(true);
		else
			goArrow[1].SetActive(true);
	}

	public void ShowView (string value) {
		Visible = true;
		if(isClose) {
			isClose = false;
			animator.SetTrigger("Open");
		}
		overallHint.text = value;
		showTime = 3;
		ranTime = 0;
		waitTime = 0;
		hideArrow ();
	}

    /// <summary>
    /// 說明.
    /// </summary>
    /// <param name="bValue"> 之前數值 </param>
    /// <param name="aValue"> 之後變成的數值 </param>
    public void ShowView (float bValue, float aValue) {
		if(!Visible)
			waitTime = 0.5f;
		Visible = true;
		if(isClose) {
			isClose = false;
			animator.SetTrigger("Open");
		}
		overallHint.text = bValue.ToString();
		tempAValue = aValue;
		showTime = 3;
		ranTime = 0.5f;
		showArrow(aValue >= bValue);
	}
}
