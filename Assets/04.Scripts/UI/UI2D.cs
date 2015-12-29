using UnityEngine;

public class UI2D : MonoBehaviour {
	private static UI2D instance = null;
	private const string UIName = "UI2D";

	public Camera Camera2D;
	public Camera CameraTop;

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
	
	public static UI2D Get
	{
		get {
			if (!instance) {
				GameObject obj = GameObject.Find(UIName);
				if (!obj) {
					GameObject obj2 = Resources.Load<GameObject>("Prefab/UI/" + UIName);
					if (obj2) {
						GameObject obj3 = Instantiate(obj2) as GameObject;
						obj3.name = UIName;
						instance = obj3.GetComponent<UI2D>();
						if(!instance) 
							instance = obj3.AddComponent<UI2D>();

						instance.InitCom();
					} else {
						obj2 = new GameObject();
						obj2.name = UIName;
						instance = obj2.AddComponent<UI2D>();
					}
				} else
					instance = obj.GetComponent<UI2D>();
			}

			return instance;
		}
	}

	public static void UIShow(bool isShow) {
		if(instance)
			instance.Show(isShow);
		else
		if(isShow)
        	Get.Show(isShow);
	}

	private void Show(bool isShow) {
		gameObject.SetActive(isShow);
	}
	
	private void InitCom() {
		Camera2D = GameObject.Find(UIName + "/2DCamera").GetComponent<Camera>();
		CameraTop = GameObject.Find(UIName + "/TopCamera").GetComponent<Camera>();

		//if (SceneMgr.Get.CurrentScene != ESceneName.SelectRole)
		//	gameObject.transform.localPosition = Vector3.up * 10;

		initResolution();
		FpsLimiter fl = gameObject.GetComponent<FpsLimiter>();
		if (!fl)
			gameObject.AddComponent<FpsLimiter>();

		#if UNITY_EDITOR
		
		#else
			/*
			UnibillDemo ud = gameObject.GetComponent<UnibillDemo>();
			if (!ud)
				gameObject.AddComponent<UnibillDemo>();
			#if UNITY_ANDROID
			PushNotificationsAndroid pn = gameObject.GetComponent<PushNotificationsAndroid>();
			if (!pn)
				gameObject.AddComponent<PushNotificationsAndroid>();
			#endif
			
			#if UNITY_IOS
			PushNotificationsIOS pn = gameObject.GetComponent<PushNotificationsIOS>();
			if (!pn)
	            gameObject.AddComponent<PushNotificationsIOS>();
			#endif
			*/
		#endif
	}

	private void initResolution() {
		UIRoot root = this.GetComponent<UIRoot>();
			
		if (root != null){
			float width = 0;
			float height = 0;
			
			if (Screen.width > Screen.height) {
				width = Screen.width;
				height = Screen.height;
			} else {
				width = Screen.height;
				height = Screen.width;
			}
			
			int rate = Mathf.CeilToInt(1.6f * 800f * height / width);
			if (rate > 800)
				root.manualHeight = rate;
		}
	}

	public float RootWidth {
		get {
			UIRoot root = this.GetComponent<UIRoot>();
			if (root)
				return root.manualWidth;
			else
				return 1280;
		}
	}

	public float RootHeight {
		get {
			UIRoot root = this.GetComponent<UIRoot>();
			if (root)
				return root.manualHeight;
			else
				return 720;
        }
    }

    public void OpenUI(string name) {
        switch (name) {
            case "UIGameLobby":
                UIGameLobby.Get.Show();
                UIMainLobby.Get.Hide();
                break;
            case  "UIAvatarFitted":
                UIAvatarFitted.UIShow(true);
                UIMainLobby.Get.Hide();
                break;
            case "UISkillFormation":
                UISkillFormation.UIShow(true);
                UIMainLobby.Get.Hide();
                break;
        }
    }
}
