using UnityEngine;
using System.Collections;

public enum ECondition {
	None = -1,
	Rare = 0,
	Cost = 1,
	Level = 2,
	Kind = 3,
	Attribute = 4
}

public enum EFilter {
	None = 0,
	Available = 1<<0,//1
	Select = 1<<1,//2
	Active = 1<<2,//4
	Passive = 1<<3//8
}

public class UISort : UIBase {
	private static UISort instance = null;
	private const string UIName = "UISort";

	private GameObject uiSortTeam;
	private GameObject uiSortCard;

	private UIToggle[] toggleCondition = new UIToggle[5];
	private UIToggle[] toggleFilter = new UIToggle[4];

	private ECondition sortCondition;
	private int sortFilter = 0;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}
	
	public static UISort Get {
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISort;
			
			return instance;
		}
	}
	
	public static void UIShow(bool isShow){
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
		uiSortTeam = GameObject.Find (UIName + "/Center/SortTeamGroup");
		uiSortCard = GameObject.Find (UIName + "/Center/SortCardGroup");

		toggleCondition[0] = GameObject.Find (UIName + "/Center/SortCardGroup/RarityCheck").GetComponent<UIToggle>();
		toggleCondition[1] = GameObject.Find (UIName + "/Center/SortCardGroup/CostCheck").GetComponent<UIToggle>();
		toggleCondition[2] = GameObject.Find (UIName + "/Center/SortCardGroup/LevelCheck").GetComponent<UIToggle>();
		toggleCondition[3] = GameObject.Find (UIName + "/Center/SortCardGroup/KindCheck").GetComponent<UIToggle>();
		toggleCondition[4] = GameObject.Find (UIName + "/Center/SortCardGroup/AttributeCheck").GetComponent<UIToggle>();

		toggleFilter[0] = GameObject.Find (UIName + "/Center/SortCardGroup/AvailableCheck").GetComponent<UIToggle>();
		toggleFilter[1] = GameObject.Find (UIName + "/Center/SortCardGroup/SelectedCheck").GetComponent<UIToggle>();
		toggleFilter[2] = GameObject.Find (UIName + "/Center/SortCardGroup/ActiveCheck").GetComponent<UIToggle>();
		toggleFilter[3] = GameObject.Find (UIName + "/Center/SortCardGroup/PassiveCheck").GetComponent<UIToggle>();

		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/RarityCheck")).onClick = RareChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/CostCheck")).onClick = CostChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/LevelCheck")).onClick = LevelChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/KindCheck")).onClick = KindChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/AttributeCheck")).onClick = AttributeChange;

		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/AvailableCheck")).onClick = AvailableChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/SelectedCheck")).onClick = SelectedChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/ActiveCheck")).onClick = ActiveChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/PassiveCheck")).onClick = PassiveChange;
		for(int i=0; i<toggleCondition.Length; i++) {
			toggleCondition[i].value = (i == 0);
		}

		SetBtnFun(UIName + "/Center/SortCardGroup/CheckBtn", CheckEvent);
		SetBtnFun(UIName + "/Center", CheckEvent);
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void CheckEvent() {
		UIShow(false);
	}

	public void RareChange(GameObject obj) { // Star
		conditionChange(ECondition.Rare);
	}

	public void CostChange(GameObject obj) { // Space
		conditionChange(ECondition.Cost);
	}
	
	public void LevelChange(GameObject obj) { // Lv
		conditionChange(ECondition.Level);
	}
	
	public void KindChange(GameObject obj) { // Kind
		conditionChange(ECondition.Kind);
	}
	
	public void AttributeChange(GameObject obj) { //Attribute
		conditionChange(ECondition.Attribute);
	}

	public void AvailableChange(GameObject obj) {
		filterChange(EFilter.Available, toggleFilter[0].value);
	}

	public void SelectedChange(GameObject obj) {
		filterChange(EFilter.Select, toggleFilter[1].value);
	}
	
	public void ActiveChange(GameObject obj) {
		filterChange(EFilter.Active, toggleFilter[2].value);
	}
	
	public void PassiveChange(GameObject obj) {
		filterChange(EFilter.Passive, toggleFilter[3].value);
	}

	private void conditionChange(ECondition condition) {
		for(int i=0; i<toggleCondition.Length; i++) {
			toggleCondition[i].value = (i == (int)condition);
		}
		sortCondition = condition;
		if(UISkillFormation.Visible)
			UISkillFormation.Get.SetSort(sortCondition, sortFilter);
	}

	private void filterChange(EFilter filter, bool isAdd) {
		if(isAdd)
			sortFilter += (int)filter;
		else 
			sortFilter -= (int)filter;
		Debug.Log("sortFilter:"+sortFilter);
		if(UISkillFormation.Visible)
			UISkillFormation.Get.SetSort(sortCondition, Mathf.Clamp(sortFilter,0, 15));
	}


}
