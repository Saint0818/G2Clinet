using UnityEngine;

public class UISkillEffect : UIBase {
	private static UISkillEffect instance = null;
	private const string UIName = "UISkillEffect";

	private GameObject uiMotion;
	private UISprite spriteCardFrame;
	private UITexture textureCardInfo;
	private UILabel labelCardName;

	public static bool Visible{
		get{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UISkillEffect Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillEffect;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow, int kind = 0, int picNo = 0, int lv = 0, string name = ""){
		if(isShow) {
			Get.Show(isShow);
			if(picNo != 0 && lv != 0) {
				Get.spriteCardFrame.spriteName = "cardlevel_" + Mathf.Clamp(lv, 1, 3).ToString();
				Get.textureCardInfo.mainTexture = GameData.CardTexture(picNo);
				Get.labelCardName.text = name;
			}
		} else 
		if(instance)
			instance.Show(isShow);
	}

	protected override void InitCom() {
		uiMotion = GameObject.Find (UIName + "/Center/CardMotion");
		spriteCardFrame = GameObject.Find (UIName + "/Center/CardMotion/CardFrame").GetComponent<UISprite>();
		textureCardInfo = GameObject.Find (UIName + "/Center/CardMotion/CardInfo").GetComponent<UITexture>();
		labelCardName = GameObject.Find (UIName + "/Center/CardMotion/CardLabel").GetComponent<UILabel>();
	}
}
