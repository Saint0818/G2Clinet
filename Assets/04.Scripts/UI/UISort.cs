using UnityEngine;
using System.Collections;

public enum ECondition {
	Date = 0,
	Rare = 1,
	Cost = 2,
	Level = 3,
	Kind = 4,
	Attribute = 5
}

public enum EFilter {
	All = 0,
	Available = 1,
	Select = 2
}

public class UISort : UIBase {
	private static UISort instance = null;
	private const string UIName = "UISort";

	private GameObject uiSortTeam;
	private GameObject uiSortCard;

	private UIToggle[] toggleCondition = new UIToggle[6];
	private UIToggle[] toggleFilter = new UIToggle[3];

	private ECondition sortCondition;
	private EFilter sortFilter;

	private bool isInit = true;

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
			instance.Show(isShow);
		} else
			if (isShow)
				Get.Show(isShow);
	}

	protected override void InitCom() {
		uiSortTeam = GameObject.Find (UIName + "/Center/SortTeamGroup");
		uiSortCard = GameObject.Find (UIName + "/Center/SortCardGroup");

		toggleCondition[0] = GameObject.Find (UIName + "/Center/SortCardGroup/DateCheck").GetComponent<UIToggle>();
		toggleCondition[1] = GameObject.Find (UIName + "/Center/SortCardGroup/RarityCheck").GetComponent<UIToggle>();
		toggleCondition[2] = GameObject.Find (UIName + "/Center/SortCardGroup/CostCheck").GetComponent<UIToggle>();
		toggleCondition[3] = GameObject.Find (UIName + "/Center/SortCardGroup/LevelCheck").GetComponent<UIToggle>();
		toggleCondition[4] = GameObject.Find (UIName + "/Center/SortCardGroup/KindCheck").GetComponent<UIToggle>();
		toggleCondition[5] = GameObject.Find (UIName + "/Center/SortCardGroup/AttributeCheck").GetComponent<UIToggle>();

		toggleFilter[0] = GameObject.Find (UIName + "/Center/SortCardGroup/AllCheck").GetComponent<UIToggle>();
		toggleFilter[1] = GameObject.Find (UIName + "/Center/SortCardGroup/AvailableCheck").GetComponent<UIToggle>();
		toggleFilter[2] = GameObject.Find (UIName + "/Center/SortCardGroup/SelectedCheck").GetComponent<UIToggle>();


		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/DateCheck")).onClick = DateChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/RarityCheck")).onClick = RareChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/CostCheck")).onClick = CostChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/LevelCheck")).onClick = LevelChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/KindCheck")).onClick = KindChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/AttributeCheck")).onClick = AttributeChange;
		
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/AllCheck")).onClick = AllChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/AvailableCheck")).onClick = AvailableChange;
		UIEventListener.Get (GameObject.Find (UIName + "/Center/SortCardGroup/SelectedCheck")).onClick = SelectedChange;
		for(int i=0; i<toggleCondition.Length; i++) {
			toggleCondition[i].value = (i == 0);
		}
		for(int i=0; i<toggleFilter.Length; i++) {
			toggleFilter[i].value = (i == 0);
		}

		SetBtnFun(UIName + "/Center/SortCardGroup/CheckBtn", CheckEvent);
		isInit = false;
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		
	}

	public void CheckEvent() {
		if(UISkillFormation.Visible)
			UISkillFormation.Get.SetSort(sortCondition, sortFilter);
		UIShow(false);
	}

	public void DateChange(GameObject obj) {
		conditionChange(ECondition.Date);
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

	public void AllChange(GameObject obj) {
		filterChange(EFilter.All);
	}

	public void AvailableChange(GameObject obj) {
		filterChange(EFilter.Available);
	}

	public void SelectedChange(GameObject obj) {
		filterChange(EFilter.Select);
	}

	private void conditionChange(ECondition condition) {
		if(!isInit){
			for(int i=0; i<toggleCondition.Length; i++) {
				toggleCondition[i].value = (i == (int)condition);
			}
			sortCondition = condition;
		}
	}

	private void filterChange(EFilter filter) {
		if(!isInit) {
			for(int i=0; i<toggleFilter.Length; i++) {
				toggleFilter[i].value = (i == (int)filter);
			}
			sortFilter = filter;
		}
	}


}
