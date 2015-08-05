using UnityEngine;
using System.Collections;

public class UISkillEffect : UIBase {
	private static UISkillEffect instance = null;
	private const string UIName = "UISkillEffect";

	private UISprite spriteCardFrame;
	private UITexture textureCardInfo;

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

	public static void UIShow(bool isShow, int picNo = 0, int lv = 0){
		if(isShow) {
			Get.Show(isShow);
			if(picNo != 0 && lv != 0) {
				Get.spriteCardFrame.spriteName = "SkillCard" + lv.ToString();
				Get.textureCardInfo.mainTexture = GameData.CardTextures[picNo];
			}
		} else 
		if(instance)
			instance.Show(isShow);
	}

	protected override void InitCom() {
		spriteCardFrame = GameObject.Find (UIName + "/Center/CardMotion/CardFrame").GetComponent<UISprite>();
		textureCardInfo = GameObject.Find (UIName + "/Center/CardMotion/CardInfo").GetComponent<UITexture>();
	}
}
