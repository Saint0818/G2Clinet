using UnityEngine;
using System.Collections;

public class UIDoubleClick : UIBase {
	public delegate void IntDelegate (int lv);
	public delegate void PlayerDelegate (int lv, PlayerBehaviour player);

	private static UIDoubleClick instance = null;
	private const string UIName = "UIDoubleClick";

	private float framSpeed = 0;
	private float framSpeed2 = 0;
	private float speed = 1f;
	private float SecondSpeed = 1f;
	private float SecondSpeedRate = 0.8f;
	private bool isStart = true;
	private Vector2 size;
	private Vector2 size2;
	private Vector2 SecondStartSize;

	private UISprite[] checkCircle = new UISprite[2];
	private UISprite[] lvSprite = new UISprite[3];  


	private IntDelegate finsh = null;
	private PlayerDelegate finshPalyer = null;

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
			checkCircle[0].width = 800;
			checkCircle[0].height = 800;
			checkCircle[1].width = 800;
			checkCircle[1].height = 800;
			size = new Vector2 (800, 800);
			size2 = new Vector2 (800, 800);
			SecondStartSize = size * SecondSpeedRate;
			isStart = true;
			SetLv(-1);
			checkCircle[1].gameObject.SetActive(false);
		}
	}

	void FixedUpdate()
	{
		if (isStart) {
			size -= Vector2.one * framSpeed;
			checkCircle[0].width = (int)size.x;
			checkCircle[0].height = (int)size.y;

			Debug.Log("size0 : " + size);
			if(size.x < SecondStartSize.x){
				checkCircle[1].gameObject.SetActive(true);
				size2 -= Vector2.one * framSpeed2;
				checkCircle[1].width = (int)size2.x;
				checkCircle[1].height = (int)size2.y;
				if(size2.x <= 0){
					checkCircle[1].gameObject.SetActive(false);
				}
				Debug.Log("size2 : " + size);
				
			}

			if(size.x <= 0){
				isStart = false;
				if(finsh != null)
					finsh(0);
				UIShow(false);
			}
		}
	}

	private PlayerBehaviour crtPlayer;

	public void SetData(float value, IntDelegate intFunction = null, PlayerDelegate playerFunction = null ,PlayerBehaviour player = null)
	{
		finsh = intFunction;
		finshPalyer = playerFunction;
		crtPlayer = player;
		speed = value;
		framSpeed = 800 / (speed * 30);
		framSpeed2 = 800 / (speed * 30 * SecondSpeedRate);
		Debug.Log (framSpeed + " : " + framSpeed2);
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

		checkCircle[0] = GameObject.Find (UIName + "/SceneClick/CheckCircle0").GetComponent<UISprite> ();
		checkCircle[1] = GameObject.Find (UIName + "/SceneClick/CheckCircle1").GetComponent<UISprite> ();
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
			if(finsh != null)
				finsh(index);
			else if(crtPlayer != null)
				finshPalyer(index, crtPlayer);

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