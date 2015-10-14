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
	private UIToggle[] avatarSort = new UIToggle[5];
	private UIToggle[] avatarFilter = new UIToggle[5];

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
				toggleCondition[0] = objGroup[i].transform.FindChild ("RarityCheck").GetComponent<UIToggle>();
				toggleCondition[1] = objGroup[i].transform.FindChild ("CostCheck").GetComponent<UIToggle>();
				toggleCondition[2] = objGroup[i].transform.FindChild ("LevelCheck").GetComponent<UIToggle>();
				toggleCondition[3] = objGroup[i].transform.FindChild ("KindCheck").GetComponent<UIToggle>();
				toggleCondition[4] = objGroup[i].transform.FindChild ("AttributeCheck").GetComponent<UIToggle>();

				toggleFilter[0] = objGroup[i].transform.FindChild ("AvailableCheck").GetComponent<UIToggle>();
				toggleFilter[1] = objGroup[i].transform.FindChild ("SelectedCheck").GetComponent<UIToggle>();
				toggleFilter[2] = objGroup[i].transform.FindChild ("ActiveCheck").GetComponent<UIToggle>();
				toggleFilter[3] = objGroup[i].transform.FindChild ("PassiveCheck").GetComponent<UIToggle>();

				for (int j=0; j<toggleCondition.Length; j++) {
					toggleCondition[j].name = j.ToString();
					toggleCondition[j].value = (j == 0);
					UIEventListener.Get (toggleCondition[j].gameObject).onClick = OnSort;
				}

				for (int j=0; j<toggleFilter.Length; j++) {
					toggleFilter[j].name = (j+10).ToString();
					UIEventListener.Get (toggleFilter[j].gameObject).onClick = OnSort;
				}
				
				SetBtnFun(UIName + "/Center/SortCardGroup/CheckBtn", CheckEvent);
				SetBtnFun(UIName + "/Center", CheckEvent);
			}
			else if(i == 1)
			{
				//Avatar
				avatarSort[0] = objGroup[i].transform.FindChild("TimelimitCheck").gameObject.GetComponent<UIToggle>();
				avatarSort[1] = objGroup[i].transform.FindChild("TimelessCheck").gameObject.GetComponent<UIToggle>();

				for(int j = 0; j < avatarSort.Length; j++){
					avatarSort[j].name = j.ToString();
					UIEventListener.Get (avatarSort[j].gameObject).onClick = OnSort;
				}

				avatarFilter[0] = objGroup[i].transform.FindChild("AvailableCheck").GetComponent<UIToggle>();
				avatarFilter[1] = objGroup[i].transform.FindChild("SelectedCheck").GetComponent<UIToggle>();
				avatarFilter[2] = objGroup[i].transform.FindChild("All").GetComponent<UIToggle>();

				for(int k = 0; k < avatarFilter.Length; k++){
					avatarFilter[k].name = k.ToString();
					UIEventListener.Get (avatarFilter[k].gameObject).onClick = OnSort;
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
					if(index < 10) {//condition
						for(int i=0; i<toggleCondition.Length; i++) {
							toggleCondition[i].value = (i == index);
						}
						sortCondition = (ECondition)index;
					} else {//filter
						if(toggleFilter[index - 10].value)
							sortFilter += (int)Mathf.Pow(2, (index - 10));
						else 
							sortFilter -= (int)Mathf.Pow(2, (index - 10));
					}
				
					if(UISkillFormation.Visible)
						UISkillFormation.Get.SetSort(sortCondition, Mathf.Clamp(sortFilter,0, 15));

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
	public void OnFilter(GameObject obj)
	{
		int index;
		
		if (int.TryParse (obj.name, out index)) {
			switch (sortFilter) {
			case 0:
				break;	
			case 1:
				if(UIAvatarFitted.Visible)
					UIAvatarFitted.Get.Filter(index);
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
}
