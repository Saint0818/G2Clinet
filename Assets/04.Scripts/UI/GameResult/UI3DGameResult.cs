using UnityEngine;

public class UI3DGameResult : UIBase {
	private static UI3DGameResult instance = null;
	private const string UIName = "UI3DGameResult";

//	private Animator animatorShowBasket;

	private AwardBasket[] awardBasket;
	private GameObject[] basket = new GameObject[3];
	private bool[] isChoosed  = new bool[3];

	void Start (){
		var obj = GameObject.Find("UI3D/3DCamera");
		if(obj)
			obj.SetActive(false);
		else
			Debug.LogWarning("Can't find UI3D.3DCamera.");
	}

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static void UIShow(bool isShow){
		if (instance)
			instance.Show(isShow);
		else {
			if (isShow){
				Get.Show(isShow);
			}
		}
	}
	
	public static UI3DGameResult Get
	{
		get {
            if (!instance) {
                UI3D.UIShow(true);
				instance = Load3DUI(UIName) as UI3DGameResult;
            }

			return instance;
		}
	}
	
	protected override void InitCom() {
//		animatorShowBasket = GameObject.Find(UIName + "/UI3DGameResultCamera/ShowAward").GetComponent<Animator>();
		awardBasket = GetComponentsInChildren<AwardBasket>();

		for (int i=0; i<basket.Length; i++) {
			basket[i] = GameObject.Find(UIName + "/UI3DGameResultCamera/ShowAward/"+i.ToString());
			UIEventListener.Get(basket[i]).onClick = OnChoose;
		}
	}

	public void OnChoose (GameObject go) {
		int result = 0;
		if(int.TryParse(go.name, out result)) {
			if(!isChoosed[result]) {
				UIGameResult.Get.ChooseLucky(result);
			}
		}
	}

	public void ChooseStart (int index) {
		awardBasket[index].OnChoose();
	}
}
