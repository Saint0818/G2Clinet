using UnityEngine;
using System.Collections;
using GameStruct;

public class UIGetSkillCard : UIBase {
	private static UIGetSkillCard instance = null;
	private const string UIName = "UIGetSkillCard";

	private TActiveSkillCard skillCard;
	private Animator animator;
	private bool isOpenImage = false;

	private GameObject[] sloganView = new GameObject[3];
	private UILabel[] labelSlogan = new UILabel[3];
	private UILabel[] labelKind = new UILabel[3];
	private UISprite[] spriteSkillKind = new UISprite[3];

	private UILabel labelSkillExplain ;

	public static bool Visible {
		get {
			if(instance)
				return instance.gameObject.activeInHierarchy;
			else
				return false;
		}

		set {
			if (instance) {
				if (!value)
					RemoveUI(UIName);
				else
					instance.Show(value);
			} else
				if (value)
					Get.Show(value);
		}
	}

	public static UIGetSkillCard Get
	{
		get {
			if (!instance) 
				instance = LoadUI(UIName) as UIGetSkillCard;

			return instance;
		}
	}

	protected override void InitCom() {
		animator = gameObject.GetComponent<Animator>();
		isOpenImage = false;
		skillCard = new TActiveSkillCard();
		skillCard.Init(GameObject.Find(UIName + "/Center/Window/ItemSkillCard"), new EventDelegate(OnOpenImage), false);

		for(int i=0; i < 3; i++) {
			sloganView[i] = GameObject.Find(UIName + "/Center/Window/DownBoard/MainView/" + i.ToString());
			labelSlogan[i] = sloganView[i].transform.Find("SloganLabel").GetComponent<UILabel>();
			labelKind[i] = sloganView[i].transform.Find("KindLabel").GetComponent<UILabel>();
			spriteSkillKind[i]= sloganView[i].transform.Find("SKillKind").GetComponent<UISprite>();
			sloganView[i].SetActive(false);
		}
		labelSkillExplain = GameObject.Find(UIName + "/Center/Window/DownBoard/MainView/SkillArea/SkillExplain").GetComponent<UILabel>();
		
		UIEventListener.Get(GameObject.Find(UIName + "/BottomRight/NextLabel")).onClick = OnClose;
	}

	private void setSloganLabel (int id, int quality){
		for(int i=0; i<3; i++) {
			labelSlogan[i].text = GameFunction.QualityNameSkill(quality);
			if(GameFunction.IsActiveSkill(id)) {
				labelKind[i].text = "[23A3A3FF]"+ TextConst.S(7111) +"[-]";
				spriteSkillKind[i].spriteName = "ActiveIcon";
			} else {
				labelKind[i].text = "[23A3A3FF]"+ TextConst.S(7112) +"[-]";
				spriteSkillKind[i].spriteName = "PassiveIcon";
			}
		}
	}

	private int getQuality (int quality) {
		if(quality < 4)
			return 0;
		else if(quality < 5)
			return 1;
		else
			return 2;
	}

//	public void Show(TSkill skill) {
//		if(GameData.DSkillData.ContainsKey(skill.ID)) {
//			UIShow(true);
//			skillCard.UpdateView(0, skill);
//			labelSkillExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[skill.ID].Explain,skill.ID, skill.Lv);
//			sloganView[getQuality(GameData.DSkillData[skill.ID].Quality)].SetActive(false);
//			setSloganLabel(skill.ID, GameData.DSkillData[skill.ID].Quality);
//		}
//	}

	public void ShowView(int id) {
		if(GameData.DItemData.ContainsKey(id)){
			if(GameData.DSkillData.ContainsKey(GameData.DItemData[id].Avatar)) {
				Visible = true;
				skillCard.UpdateViewItemData(GameData.DItemData[id]);
				labelSkillExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[GameData.DItemData[id].Avatar].Explain, GameData.DItemData[id].Avatar, GameData.DItemData[id].LV);
				sloganView[getQuality(GameData.DSkillData[GameData.DItemData[id].Avatar].Quality)].SetActive(true);
				setSloganLabel(GameData.DItemData[id].Avatar, GameData.DSkillData[GameData.DItemData[id].Avatar].Quality);
			}
		}
	}

	public void OnOpenImage () {
		if(isOpenImage) 
			animator.SetTrigger("Back");
	  	else 
			animator.SetTrigger("Go");
		
		isOpenImage = !isOpenImage;
	}

	public void OnClose (GameObject go) {
		if(UIGameResult.Visible) { 
			UIGameResult.Get.ShowBonusItem();
			UIGameResult.Get.ShowReturnButton();
		}
		
		if(UIBuyStore.Visible)
			UIBuyStore.Get.Gohead();
		Visible = false;
	}
}
