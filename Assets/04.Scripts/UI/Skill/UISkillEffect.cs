using UnityEngine;
using GameStruct;

public class UISkillEffect : UIBase {
	private static UISkillEffect instance = null;
	private const string UIName = "UISkillEffect";

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
	
	public static UISkillEffect Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillEffect;
			
			return instance;
		}
	}

	public static void UIShow(bool isShow){
		if (instance) {
			instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}

	protected override void InitCom() {
//		uiMotion = GameObject.Find (UIName + "/Center/CardMotion");
		spriteCardFrame = GameObject.Find (UIName + "/Center/CardMotion/CardFrame").GetComponent<UISprite>();
		textureCardInfo = GameObject.Find (UIName + "/Center/CardMotion/CardInfo").GetComponent<UITexture>();
		labelCardName = GameObject.Find (UIName + "/Center/CardMotion/CardLabel").GetComponent<UILabel>();
	}

	public void ShowView (TSkill skill) {
		if(GameData.DSkillData.ContainsKey (skill.ID)) {
			UIShow(true);
			spriteCardFrame.spriteName =  GameFunction.CardLevelName(skill.ID);
            textureCardInfo.mainTexture = TextureManager.Get.CardTexture(GameData.DSkillData[skill.ID].PictureNo);
			labelCardName.text = GameData.DSkillData[skill.ID].Name;
		}
	}
}
