using UnityEngine;
using System.Collections;

public class UISkillEffect : UIBase {
	private static UISkillEffect instance = null;
	private const string UIName = "UISkillEffect";

	private GameObject[] uiMotion = new GameObject[1];
	private UISprite[] spriteCardFrame = new UISprite[1];
	private UITexture[] textureCardInfo = new UITexture[1];
	private UILabel[] labelCardName = new UILabel[1];

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
				Get.spriteCardFrame[kind].spriteName = "SkillCard" + lv.ToString();
				Get.textureCardInfo[kind].mainTexture = GameData.CardTextures[picNo];
				Get.labelCardName[kind].name = name;
			}
		} else 
		if(instance)
			instance.Show(isShow);
	}

	protected override void InitCom() {
		uiMotion[0] = GameObject.Find (UIName + "/Center/CardMotion_0");
		spriteCardFrame[0] = GameObject.Find (UIName + "/Center/CardMotion_0/CardFrame").GetComponent<UISprite>();
		textureCardInfo[0] = GameObject.Find (UIName + "/Center/CardMotion_0/CardInfo").GetComponent<UITexture>();
		labelCardName[0] = GameObject.Find (UIName + "/Center/CardMotion_0/CardLabel").GetComponent<UILabel>();
	}
}
