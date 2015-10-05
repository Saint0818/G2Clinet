using UnityEngine;
using System.Collections;

public class UISkillEffect : UIBase {
	private static UISkillEffect instance = null;
	private const string UIName = "UISkillEffect";

	private GameObject[] uiMotion = new GameObject[3];
	private UISprite[] spriteCardFrame = new UISprite[3];
	private UITexture[] textureCardInfo = new UITexture[3];
	private UILabel[] labelCardName = new UILabel[3];

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
				for(int i=0; i<Get.uiMotion.Length; i++) {
					if(kind == i) {
						Get.uiMotion[i].SetActive(true);
						Get.spriteCardFrame[kind].spriteName = "cardlevel_" + lv.ToString();
						Get.textureCardInfo[kind].mainTexture = GameData.CardTexture(picNo);
						Get.labelCardName[kind].text = name;
					} else {
						Get.uiMotion[i].SetActive(false);
					}
				}
			}
		} else 
		if(instance)
			instance.Show(isShow);
	}

	protected override void InitCom() {
		for(int i=0; i<uiMotion.Length; i++) {
			uiMotion[i] = GameObject.Find (UIName + "/Center/CardMotion_"+i.ToString());
			spriteCardFrame[i] = GameObject.Find (UIName + "/Center/CardMotion_"+i.ToString()+"/CardFrame").GetComponent<UISprite>();
			textureCardInfo[i] = GameObject.Find (UIName + "/Center/CardMotion_"+i.ToString()+"/CardInfo").GetComponent<UITexture>();
			labelCardName[i] = GameObject.Find (UIName + "/Center/CardMotion_"+i.ToString()+"/CardLabel").GetComponent<UILabel>();
		}
	}
}
