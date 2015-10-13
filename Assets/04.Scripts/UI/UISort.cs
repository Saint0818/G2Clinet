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
	private static int sortKind = 0;
	private const string UIName = "UISort";

	//Skill
	private UIToggle[] toggleCondition = new UIToggle[5];
	private UIToggle[] toggleFilter = new UIToggle[4];
	private ECondition sortCondition;
	private int sortFilter = 0;

	//Avatar
	private GameObject[] objGroup = new GameObject[3];
	private UIToggle[] avatarCondition = new UIToggle[4];

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

	public static void UIShow(bool isShow, int kind = 0){
		SetKind(kind);

		if (instance) {
			if (!isShow)
				RemoveUI(UIName);
			else{
				instance.Show(isShow);
			}
		} else
			if (isShow)
				Get.Show(isShow);
	}

	protected override void InitCom() {

		string[] path = new string[3]{"SortCardGroup", "SortAvatarGroup", "SortTeamGroup"};

		for (int i = 0; i < objGroup.Length; i++) {
			objGroup[i] = GameObject.Find(UIName + "/Center/" + path[i]);

			if(i == 0 && objGroup[i])
			{
				//skill
				toggleCondition[0] = GameObject.Find (UIName + "/Center/SortCardGroup/RarityCheck").GetComponent<UIToggle>();
				toggleCondition[1] = GameObject.Find (UIName + "/Center/SortCardGroup/CostCheck").GetComponent<UIToggle>();
				toggleCondition[2] = GameObject.Find (UIName + "/Center/SortCardGroup/LevelCheck").GetComponent<UIToggle>();
				toggleCondition[3] = GameObject.Find (UIName + "/Center/SortCardGroup/KindCheck").GetComponent<UIToggle>();
				toggleCondition[4] = GameObject.Find (UIName + "/Center/SortCardGroup/AttributeCheck").GetComponent<UIToggle>();
				
				toggleFilter[0] = GameObject.Find (UIName + "/Center/SortCardGroup/AvailableCheck").GetComponent<UIToggle>();
				toggleFilter[1] = GameObject.Find (UIName + "/Center/SortCardGroup/SelectedCheck").GetComponent<UIToggle>();
				toggleFilter[2] = GameObject.Find (UIName + "/Center/SortCardGroup/ActiveCheck").GetComponent<UIToggle>();
				toggleFilter[3] = GameObject.Find (UIName + "/Center/SortCardGroup/PassiveCheck").GetComponent<UIToggle>();
				
				UIEventListener.Get (toggleCondition[0].gameObject).onClick = RareChange;
				UIEventListener.Get (toggleCondition[1].gameObject).onClick = CostChange;
				UIEventListener.Get (toggleCondition[2].gameObject).onClick = LevelChange;
				UIEventListener.Get (toggleCondition[3].gameObject).onClick = KindChange;
				UIEventListener.Get (toggleCondition[4].gameObject).onClick = AttributeChange;
				
				UIEventListener.Get (toggleFilter[0].gameObject).onClick = AvailableChange;
				UIEventListener.Get (toggleFilter[1].gameObject).onClick = SelectedChange;
				UIEventListener.Get (toggleFilter[2].gameObject).onClick = ActiveChange;
				UIEventListener.Get (toggleFilter[3].gameObject).onClick = PassiveChange;

				for(int j=0; j <toggleCondition.Length; j++) {
					toggleCondition[j].value = (j == 0);
				}
				
				SetBtnFun(UIName + "/Center/SortCardGroup/CheckBtn", CheckEvent);
				SetBtnFun(UIName + "/Center", CheckEvent);
			}
			else if(i == 1)
			{
				//Avatar
				avatarCondition[0] = objGroup[i].transform.FindChild("TimelimitCheck").gameObject.GetComponent<UIToggle>();
				avatarCondition[1] = objGroup[i].transform.FindChild("TimelessCheck").gameObject.GetComponent<UIToggle>();
				avatarCondition[2] = objGroup[i].transform.FindChild("AvailableCheck").GetComponent<UIToggle>();
				avatarCondition[3] = objGroup[i].transform.FindChild("SelectedCheck").GetComponent<UIToggle>();

				for(int j = 0; j < avatarCondition.Length; j++)
				{
					avatarCondition[j].name = j.ToString();
					UIEventListener.Get (avatarCondition[j].gameObject).onClick = OnSort;
				}
			}
			else
			{

			}

			objGroup[i].SetActive(false);
		}
	}

	public void OnSort(GameObject obj)
	{
		int index;

		if (int.TryParse (obj.name, out index)) {
			switch (sortKind) {
				case 0:
					break;	
				case 1:
					if(UIAvatarFitted.Visible)
						UIAvatarFitted.Get.SortView(index);
					break;
				case 2:
					break;
			}
		}
	}

	protected override void InitData() {
		
	}
	
	protected override void OnShow(bool isShow) {
		if (isShow) {
			UpdateGroup(sortKind);	
		}
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

	public static void SetKind(int kind)
	{
		sortKind = kind;
	}

	public void UpdateGroup(int kind)
	{
		if (kind < objGroup.Length)
			for (int i = 0; i < objGroup.Length; i++){
				objGroup[i].gameObject.SetActive(i == kind);
			}
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
