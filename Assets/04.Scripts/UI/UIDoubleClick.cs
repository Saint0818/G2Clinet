using UnityEngine;
using System.Collections;

public class UIDoubleClick : UIBase {
	public delegate void IntDelegate (int lv);

	private static UIDoubleClick instance = null;
	private const string UIName = "UIDoubleClick";

	private float framSpeed = 0;
	private float speed = 1f;
	private bool isStart = true;
	private Vector2 size;
	private UISprite checkCircle;

	private UISprite[] lvSprite = new UISprite[3];  


	public IntDelegate Finsh = null;

	public static UIDoubleClick Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIDoubleClick;
			
            return instance;
        }
    }
    
    public static bool Visible {
		get {
			if(instance)
				return (instance.gameObject.activeInHierarchy && instance.gameObject.activeSelf);
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow) {
		if (instance)
			instance.Show (isShow);
		else {
			if (isShow)
				Get.Show (isShow);
		}
	}

	public void Init()
	{
		if (instance) {
			checkCircle.width = 800;
			checkCircle.height = 800;
			size = new Vector2 (800, 800);
			isStart = true;
			SetLv(-1);

		}
	}

	void Update()
	{
		if (isStart) {
			size -= Vector2.one * framSpeed;
			checkCircle.width = (int)size.x;
			checkCircle.height = (int)size.y;
			if(size.x <= 0){
				isStart = false;
				if(Finsh != null)
					Finsh(0);
				UIShow(false);
			}
		}
	}

	public void SetSpeed(float value)
	{
		speed = value;
		framSpeed = 800 / (speed * 30);
	}

	private void CheckLv()
	{
		if(size.x < 150 || size.x > 400)
			SetLv(0);
		else if(size.x > 250 && size.x <= 400)
			SetLv(1);
		else if(size.x >= 150 && size.x <= 250)
			SetLv(2);
	}

	protected override void InitCom() {
		string name;

		for (int i = 0; i < lvSprite.Length; i++) {
			name = string.Format("UIDoubleClick/SceneClick/Lv/{0}", i);
			lvSprite[i] = GameObject.Find (name).GetComponent<UISprite>();
		}

		checkCircle = GameObject.Find (UIName + "/SceneClick/CheckCircle").GetComponent<UISprite> ();
	}

	protected override void OnShow (bool isShow)
	{
		base.OnShow (isShow);
		if(isShow)
			Init();
	}
	private void SetLv(int index)
	{
		for (int i = 0; i < lvSprite.Length; i++) {
			if(index == -1)
				lvSprite[i].gameObject.SetActive(false);
			else{
				if(index == i)
					lvSprite[i].gameObject.SetActive(true);
				else
					lvSprite[i].gameObject.SetActive(false);
			}
		}

		if (index != -1) {
			if(Finsh != null)
				Finsh(index);
			StartCoroutine("DelayClose");
		}
	}

	IEnumerator DelayClose()
	{
		yield return new WaitForSeconds (0.5f);
		UIShow (false);
	}

	public void ClickStop()
	{
		if (isStart) {
			isStart = false;
			CheckLv ();
		}
	}
}