using UnityEngine;
using System.Collections;
using GameStruct;

public struct TSkillCardValue {
	public GameObject[] AttrView;
	public UILabel[] GroupLabel;
	public UILabel[] ValueLabel0;

	public void Init (Transform t) {
		AttrView = new GameObject[6];
		GroupLabel = new UILabel[6];
		ValueLabel0 = new UILabel[6];

		for(int i=0; i<AttrView.Length; i++) {
			AttrView[i] = t.FindChild("AttrView" + i.ToString()).gameObject;
			GroupLabel[i] = AttrView[i].transform.FindChild("GroupLabel").GetComponent<UILabel>();
			ValueLabel0[i] = AttrView[i].transform.FindChild("ValueLabel0").GetComponent<UILabel>();
		}
	}

	private void hideAll () {
		for(int i=0; i<AttrView.Length; i++) 
			AttrView[i].SetActive(false);	
	}

	public void UpdateView(TSkill skill) {
		hideAll ();
		int index = 0;
		if(GameData.DSkillData.ContainsKey(skill.ID)) {
			if(GameData.DSkillData[skill.ID].space > 0) {
				AttrView[index].SetActive(true);	
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Space(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				if(GameFunction.IsActiveSkill(skill.ID)) {
					GroupLabel[index].text = TextConst.S(7207);
					ValueLabel0[index].text = GameData.DSkillData[skill.ID].MaxAnger.ToString();
				} else {
					GroupLabel[index].text = TextConst.S(7206);
					ValueLabel0[index].text = GameData.DSkillData[skill.ID].Rate(skill.Lv).ToString();
				}
				index ++;
			}
			if(GameData.DSkillData[skill.ID].aniRate > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7404);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].AniRate(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].distance > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7405);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Distance(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].valueBase > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(10500 + GameData.DSkillData[skill.ID].AttrKind);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].Value(skill.Lv).ToString();
				index ++;
			}
			if(GameData.DSkillData[skill.ID].lifeTime > 0) {
				AttrView[index].SetActive(true);	
				GroupLabel[index].text = TextConst.S(7406);
				ValueLabel0[index].text = GameData.DSkillData[skill.ID].LifeTime(skill.Lv).ToString();
				index ++;
			}
		}
	}
}

public struct TSkillCardMaterial {
	public GameObject[] mMaterial;
	public UIButton[] MaterialItem;
	public UISprite[] ElementPic;
	public UILabel[] NameLabel;
	public UILabel[] AmountLabel; // 99/99

	public void Init (GameObject obj, EventDelegate btnFun) {
		mMaterial = new GameObject[3];
		MaterialItem = new UIButton[3];
		ElementPic = new UISprite[3];
		NameLabel = new UILabel[3];
		AmountLabel = new UILabel[3];

		for (int i=0; i<3; i++) {
			mMaterial[i] = obj.transform.FindChild("ElementSlot" + i.ToString()).gameObject;
			MaterialItem[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem").GetComponent<UIButton>();
			ElementPic[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/ElementPic").GetComponent<UISprite>();
			NameLabel[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/NameLabel").GetComponent<UILabel>();
			AmountLabel[i] = obj.transform.FindChild("ElementSlot" + i.ToString() + "/View/MaterialItem/AmountLabel").GetComponent<UILabel>();
		}
	}

	public void UpdateView (TItemData itemData) {
		HideAllMaterial ();
		if(GameData.Team.HasMaterialSkillCard(itemData.ID) && GameData.DSkillData.ContainsKey(itemData.Avatar)) {
			int index = 0;
			if(GameData.DSkillData[itemData.Avatar].Material1 != 0 && GameData.DSkillData[itemData.Avatar].MaterialNum1 != 0) {
				if(GameData.DItemAtlas.ContainsKey(GameData.AtlasName(itemData.Atlas))) {
					ElementPic[index].atlas = GameData.DItemAtlas[GameData.AtlasName(itemData.Atlas)];
				}
				ElementPic[index].spriteName = GameData.DSkillData[itemData.Avatar].PictureNo + "s";
				NameLabel[index].text = itemData.Name;

				TMaterialSkillCard materialSkillCard = new TMaterialSkillCard();
				int materialSkillCardIndex = GameData.Team.FindMaterialSkillCard(GameData.DSkillData[itemData.Avatar].Material1, ref materialSkillCard);

				if(GameData.Team.HasMaterialSkillCard(itemData.ID))
					AmountLabel[index].text = materialSkillCardIndex + "/" + GameData.DSkillData[itemData.Avatar].MaterialNum1.ToString();
			}
		}
	}

	private void setPick (int index, TItemData itemData) {
//		if(GameData.DSkillData[])
	}

	public void HideAllMaterial () {
		for (int i=0; i<mMaterial.Length; i++) 
			mMaterial[i].SetActive(false);
	}


}

public class UISkillEvolution : UIBase {
	private static UISkillEvolution instance = null;
	private const string UIName = "UISkillEvolution";

	private TActiveSkillCard[] skillCards = new TActiveSkillCard[2];
	private TSkillCardValue[] skillCardValues = new TSkillCardValue[2];
	private TSkillCardMaterial[] skillCardMaterial = new TSkillCardMaterial[3];

	private GameObject goEvolution;
	private GameObject labelWarning;


	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}
	}

	public static void UIShow(bool isShow){
		if (instance)
			if (!isShow)
				RemoveUI(UIName);
			else
				instance.Show(isShow);
		else
			if (isShow)
				Get.Show(isShow);
	}

	public static UISkillEvolution Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UISkillEvolution;

			return instance;
		}
	}

	protected override void InitCom() {
		goEvolution = GameObject.Find(UIName + "/Center/View/RightPart");
		labelWarning = GameObject.Find(UIName + "/Center/View/RightPart/WarningLabel");
		for(int i=0; i<skillCards.Length; i++) {
			skillCards[i] = new TActiveSkillCard();
			skillCards[i].Init(GameObject.Find(UIName + "/Center/View/LeftPart/ItemSkillCard"+i.ToString()));
			skillCardValues[i] = new TSkillCardValue();
			skillCardValues[i].Init(GameObject.Find(UIName + "/Center/View/LeftPart/ReinforceInfo"+i.ToString()).transform);
		}
		for (int i=0; i<skillCardMaterial.Length; i++) {
			skillCardMaterial[i] = new TSkillCardMaterial();
			skillCardMaterial[i].Init(GameObject.Find(UIName + "/Center/View/RightPart/Evolution"), new EventDelegate(OnSearch));
		}

		SetBtnFun(UIName + "Window/BottomLeft/BackBtn", OnClose);
		labelWarning.SetActive(false);
	}

	public void OnSearch () {
		
	}

	public void Show (TSkill currentSkill, TSkill nextSkill) {
		UIShow(true);
		labelWarning.SetActive((currentSkill.ID == nextSkill.ID));
		goEvolution.SetActive(!labelWarning.activeInHierarchy);

		skillCards[0].UpdateViewFormation(currentSkill, false);
		skillCards[1].UpdateViewFormation(nextSkill, false);
		skillCardValues[0].UpdateView(currentSkill);
		skillCardValues[1].UpdateView(nextSkill);
		if(GameData.DSkillData.ContainsKey(currentSkill.ID)) {
			if(GameData.DItemData.ContainsKey (GameData.DSkillData[currentSkill.ID].Material1)) {
				skillCardMaterial[0].UpdateView(GameData.DItemData[GameData.DSkillData[currentSkill.ID].Material1]);
				skillCardMaterial[1].UpdateView(GameData.DItemData[GameData.DSkillData[currentSkill.ID].Material2]);
				skillCardMaterial[2].UpdateView(GameData.DItemData[GameData.DSkillData[currentSkill.ID].Material3]);
			}
		}
	}

	public void OnClose () {
		UIShow(false);
	}

}
