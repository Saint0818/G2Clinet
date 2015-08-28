using UnityEngine;
using System.Collections;

public class UISkillInfo : UIBase {
	private static UISkillInfo instance = null;
	private const string UIName = "UISkillInfo";
	
	private UILabel labelSkillName;
	private UILabel labelSkillLevel;
	private UILabel labelSkillInfo;
	
	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UISkillInfo Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillInfo;
			
			return instance;
		}
	}
	
	public static void UIShow(bool isShow, string name = "", string level = "", string info = ""){
		if(isShow) {
			Get.labelSkillName.text = name;
			Get.labelSkillLevel.text = level;
			Get.labelSkillInfo.text = info;
		}
		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}
	
	protected override void InitCom() {
		labelSkillName = GameObject.Find (UIName + "/Window/LabelNameTW").GetComponent<UILabel>();
		labelSkillLevel = GameObject.Find (UIName + "/Window/LabelLevel").GetComponent<UILabel>();
		labelSkillInfo = GameObject.Find (UIName + "/Window/LabelSkillinfo").GetComponent<UILabel>();
		SetBtnFun(UIName + "/BoxCollider", Close);
	}
	
	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void Close (){
		UIShow(false);
	}
}

