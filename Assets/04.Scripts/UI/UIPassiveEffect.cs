using UnityEngine;
using System.Collections;

public class UIPassiveEffect : UIBase {
	private static UIPassiveEffect instance = null;
	private const string UIName = "UIPassiveEffect";

	public GameObject uiCardMotion;
	public UISprite spriteCardFrame;
	public UITexture textureCardInfo;
	public UILabel labelCardLabel;

	public static bool Visible{
		get{
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UIPassiveEffect Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIPassiveEffect;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow, int picNo = 0, int lv = 0, string name = ""){
		if(isShow) {
			if(picNo != 0 && lv != 0) {
//				Get.Show(isShow);
				Get.Show(!GameController.Get.Joysticker.IsSkillShow);
				Get.uiCardMotion.SetActive(false);
				Get.uiCardMotion.SetActive(isShow);
				if(Get.IsInvoking("HiddenView")) {
					Get.CancelInvoke("HiddenView");
				}
				Get.Invoke("HiddenView", 1);
				Get.spriteCardFrame.spriteName = "SkillCard" + lv.ToString();
				Get.textureCardInfo.mainTexture = GameData.CardTextures[picNo];
				Get.labelCardLabel.text = name;
			}
		} else 
			if(Visible)
				instance.Show(isShow);
	}

	protected override void InitCom() {
		uiCardMotion = GameObject.Find (UIName + "/Center/CardMotion");
		spriteCardFrame = GameObject.Find (UIName + "/Center/CardMotion/CardGroup/CardFrame").GetComponent<UISprite>();
		textureCardInfo = GameObject.Find (UIName + "/Center/CardMotion/CardGroup/CardInfo").GetComponent<UITexture>();
		labelCardLabel = GameObject.Find (UIName + "/Center/CardMotion/CardLabel").GetComponent<UILabel>();
	}

	public void HiddenView(){
		instance.Show(false);
	}
}
