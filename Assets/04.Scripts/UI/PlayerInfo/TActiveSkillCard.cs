using GameStruct;
using UnityEngine;

public class TActiveSkillCard
{
	/// <summary>
	/// 技能卡片結構.
	/// </summary>
	/// <remarks>
	/// 使用方法:必須要有技能卡母物件，方法有二種：1.已存在ＵＩ裡使用find再使用Init() 2.Resource.load必須先實體化再Init()
	/// 注意：必須先init()才能 initbuttonfunction()，不可再Init使用button.OnClick.Add, 否則會造成無窮迴圈
	/// </remarks>
	public TSkill Skill;
	private GameObject self;
	private UITexture SkillPic;
	private UISprite SkillCard;
	private UILabel SkillName;
	private SkillCardStar[] SkillStars;
	private UISprite SkillSuit;
	private UISprite SkillKind;
	private UISprite SkillKindBg;
	public GameObject UnavailableMask;
	public UILabel UnavailableMaskLabel;
	public GameObject DragMask;
	public GameObject InListCard;
	public GameObject SellSelect;
	public UILabel SellLabel;
	public GameObject SellSelectCover;
	public UISpriteAnimation LightAnimation;
	public GameObject RedPoint;

	private bool isInit = false;
	private UIButton btn;
	
	public void Init(GameObject go, EventDelegate btnFunc = null, bool isFormation = false)
	{
		if (!isInit && go) {
			self = go;
			SkillPic = go.transform.Find("SkillPic").gameObject.GetComponent<UITexture>();
			SkillCard = go.transform.Find("SkillCard").gameObject.GetComponent<UISprite>();
			SkillName = go.transform.Find("SkillName").gameObject.GetComponent<UILabel>();
			SkillStars = new SkillCardStar[5];
			for(int i=0; i<SkillStars.Length; i++)
				SkillStars[i] = go.transform.Find("SkillStar/StarBG" + i.ToString()).gameObject.GetComponent<SkillCardStar>();
			SkillSuit = go.transform.Find("SkillSuit").gameObject.GetComponent<UISprite>();
			SkillKind = go.transform.Find("SkillKind").gameObject.GetComponent<UISprite>();
			SkillKindBg = go.transform.Find("SkillKind/KindBg").gameObject.GetComponent<UISprite>();
			UnavailableMask = go.transform.Find("UnavailableMask").gameObject;
			if(go.transform.Find("UnavailableMask/UnavailableLabel") != null) {
				UnavailableMaskLabel = go.transform.Find("UnavailableMask/UnavailableLabel").GetComponent<UILabel>();
				UnavailableMaskLabel.text = TextConst.S(7115);
			}
			if(go.transform.Find("DragMask") != null)
				DragMask = go.transform.Find("DragMask").gameObject;
			InListCard = go.transform.Find("InListCard").gameObject;
			SellSelect = go.transform.Find("SellSelect").gameObject;
			SellLabel = go.transform.Find("SellSelect/SellLabel").gameObject.GetComponent<UILabel>();
			SellSelectCover = go.transform.Find("SellSelect/SellCover").gameObject;
			LightAnimation = go.transform.Find("InListCard/SpriteAnim/Shine").gameObject.GetComponent<UISpriteAnimation>();
			if(go.transform.Find("RedPoint"))
				RedPoint = go.transform.Find("RedPoint").gameObject;

			go.transform.Find ("InListCard/InlistLabel").gameObject.GetComponent<UILabel>().text = TextConst.S(7110);
			go.transform.Find ("SellSelect/SellCover/SellLabel").gameObject.GetComponent<UILabel>().text = TextConst.S(7114);

			if(RedPoint != null)
				RedPoint.SetActive(false);
			UnavailableMask.SetActive(isFormation);
			if(DragMask != null)
				DragMask.SetActive(false);
			InListCard.SetActive(isFormation);
			SellSelect.SetActive(isFormation);

			btn = self.GetComponent<UIButton>();
			isInit = SkillPic && SkillCard && SkillName  && SkillSuit && SkillKind;

			if(!isInit)
				Debug.LogError("TActive Init Fail!");
			
            if(isInit && btnFunc != null && btn){
				btn.onClick.Add(btnFunc);
			}
		}
	}
	
	public void UpdateView(int index, TSkill skill)
	{
		if(isInit){
            if (GameData.DSkillData.ContainsKey(skill.ID)) {
				Skill = skill;
    			self.name = index.ToString();
    			SkillPic.mainTexture = GameData.CardTexture(skill.ID);
				SkillCard.spriteName = "cardlevel_" + GameData.DSkillData[skill.ID].Quality.ToString();
				SkillName.text = GameData.DSkillData[skill.ID].Name;
//				SkillName.color = TextConst.Color(GameData.DSkillData[skill.ID].Quality);
				SkillSuit.spriteName = "Levelball" + GameData.DSkillData[skill.ID].Quality.ToString();
				if(GameFunction.IsActiveSkill(skill.ID))
					SkillKind.spriteName = "ActiveIcon";
				else 
					SkillKind.spriteName = "PasstiveIcon";
				SkillKindBg.spriteName = "APIcon" + GameData.DSkillData[skill.ID].Quality.ToString();
				GameFunction.ShowStar(ref SkillStars, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);
            } else
                Debug.LogError("TActiveSkillCard.UpdateView skill id error " + skill.ID.ToString());
		}
		else
		{
			Debug.LogError("You needed to Init()");
		}
	}

	public void UpdateViewItemData(TItemData itemData)
	{
		if(isInit){
			if (GameData.DSkillData.ContainsKey(itemData.Avatar)) {
				self.name = itemData.ID.ToString();
				SkillPic.mainTexture = GameData.CardTexture(itemData.Avatar);
				SkillCard.spriteName = "cardlevel_" + GameData.DSkillData[itemData.Avatar].Quality.ToString();
				SkillName.text = GameData.DSkillData[itemData.Avatar].Name;
//				SkillName.color = TextConst.Color(GameData.DSkillData[itemData.Avatar].Quality);
				SkillSuit.spriteName = "Levelball" + GameData.DSkillData[itemData.Avatar].Quality.ToString();
				if(GameFunction.IsActiveSkill(itemData.Avatar))
					SkillKind.spriteName = "ActiveIcon";
				else 
					SkillKind.spriteName = "PasstiveIcon";
				SkillKindBg.spriteName = "APIcon" + GameData.DSkillData[itemData.Avatar].Quality.ToString();
				GameFunction.ShowStar(ref SkillStars, itemData.LV, GameData.DSkillData[itemData.Avatar].Quality, GameData.DSkillData[itemData.Avatar].MaxStar);
			} else
				Debug.LogError("TActiveSkillCard.UpdateView skill id error " + itemData.Avatar.ToString());
		}
		else
		{
			Debug.LogError("You needed to Init()");
		}
	}

	public void UpdateViewFormation(TSkill skill, bool isEquip)
	{
		if(isInit){
			if (GameData.DSkillData.ContainsKey(skill.ID)) {
				Skill = skill;
				SkillPic.mainTexture = GameData.CardTexture(skill.ID);
				SkillCard.spriteName = "cardlevel_" + GameData.DSkillData[skill.ID].Quality.ToString();
				SkillName.text = GameData.DSkillData[skill.ID].Name;
				SkillSuit.spriteName = "Levelball" + GameData.DSkillData[skill.ID].Quality.ToString();
				if(GameFunction.IsActiveSkill(skill.ID))
					SkillKind.spriteName = "ActiveIcon";
				else 
					SkillKind.spriteName = "PasstiveIcon";
				SkillKindBg.spriteName = "APIcon" + GameData.DSkillData[skill.ID].Quality.ToString();

				UnavailableMask.SetActive(false);
				if(DragMask != null)
					DragMask.SetActive(false);
				InListCard.SetActive(isEquip);
				SellSelect.SetActive(false);
				SellSelectCover.SetActive(false);
				GameFunction.ShowStar(ref SkillStars, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);

			} else
				Debug.LogError("TActiveSkillCard.UpdateView skill id error " + skill.ID.ToString());
		}
		else
		{
			Debug.LogError("You needed to Init()");
		}
	}

	public GameObject MySkillCard {
		get { return self;}
	}
	
	public bool Enable
	{
		set{self.gameObject.SetActive(value);}
	}

	//For reniforce
	public void ShowStarForRein (int oriStar, int count) {
		if((oriStar + count) >= SkillStars.Length) {
			for (int i=0; i<SkillStars.Length; i++) {
				if(i > (oriStar - 1) && i < SkillStars.Length) {
					SkillStars[i].ShowStarPreview();
				}
			}
		} else {
			for (int i=0; i<SkillStars.Length; i++) {
				if(i > (oriStar - 1) && i <= (oriStar + count - 1)) {
					SkillStars[i].HideStarPreview();
					SkillStars[i].ShowStarPreview();
				} else if(i > (oriStar + count - 1)){
					SkillStars[i].HideStarPreview();
				}
			}
		}
	}

	public void HideAllPreviewStar () {
		for (int i=0; i<SkillStars.Length; i++) 
			SkillStars[i].HideStarPreview();
	}

	public void ShowGetStar (int index) {
		if(index >=0 && index < SkillStars.Length) {
			SkillStars[index -1].ShowGetStar();
			SkillStars[index -1].ShowStar();
		}
	}

	public void HideAllGetStar () {
		for (int i=0; i<SkillStars.Length; i++) 
			SkillStars[i].HideGetStar();
	}


	//SkillFormation
	public bool DragCard {
		set {if(DragMask != null) DragMask.SetActive(value);}
	}

	public bool IsInstall {
		get {return (InListCard != null && InListCard.activeSelf);}
		set {InListCard.SetActive(value);}
	}

	public bool IsCanUse {
		get {return (UnavailableMask != null && !UnavailableMask.activeSelf);}
		set {UnavailableMask.SetActive(value);}
	}

	public bool IsSold {
		get {return (SellSelectCover != null && SellSelectCover.activeSelf);}
		set {SellSelectCover.SetActive(value);}
	}

	public bool ShowSell {
		set {SellSelect.SetActive(value);}
	}

	public void SetCoin (int money) {
		if(SellLabel != null)
			SellLabel.text = money.ToString();
	}

	public bool CheckRedPoint {
		get {if(RedPoint != null)
				return RedPoint.activeSelf;
			else 
				return false;}
		set {if(RedPoint != null)
				RedPoint.SetActive(value);}
	}
}
