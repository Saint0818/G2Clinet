using UnityEngine;
using System.Collections;
using GameStruct;

public class TActiveSkillCard
{
	/// <summary>
	/// 技能卡片結構.
	/// </summary>
	/// <remarks>
	/// 使用方法:必須要有技能卡母物件，方法有二種：1.已存在ＵＩ裡使用find再使用Init() 2.Resource.load必須先實體化再Init()
	/// 注意：必須先init()才能 initbuttonfunction()，不可再Init使用button.OnClick.Add, 否則會造成無窮迴圈
	/// </remarks>
	private GameObject self;
	private UITexture SkillPic;
	private UISprite SkillCard;
	private UILabel SkillName;
	private SkillCardStar SkillStar;
	private UISprite SkillSuit;
	private UISprite SkillKind;
	private UISprite SkillKindBg;
	public GameObject UnavailableMask;
	public GameObject InListCard;
	public GameObject SellSelect;
	public UILabel SellLabel;
	public GameObject SellSelectCover;
	public UISpriteAnimation LightAnimation;

	private bool isInit = false;
	private UIButton btn;
	
	public void Init(GameObject go, EventDelegate btnFunc = null, bool isFormation = false)
	{
		if (!isInit && go) {
			self = go;
			SkillPic = go.transform.FindChild("SkillPic").gameObject.GetComponent<UITexture>();
			SkillCard = go.transform.FindChild("SkillCard").gameObject.GetComponent<UISprite>();
			SkillName = go.transform.FindChild("SkillName").gameObject.GetComponent<UILabel>();
			for(int i=0; i<5; i++)
				SkillStar = go.transform.FindChild("SkillStar/StarBG" + i.ToString()).gameObject.GetComponent<SkillCardStar>();
			SkillSuit = go.transform.FindChild("SkillSuit").gameObject.GetComponent<UISprite>();
			SkillKind = go.transform.FindChild("SkillKind").gameObject.GetComponent<UISprite>();
			SkillKindBg = go.transform.FindChild("SkillKind/KindBg").gameObject.GetComponent<UISprite>();
			UnavailableMask = go.transform.FindChild("UnavailableMask").gameObject;
			InListCard = go.transform.FindChild("InListCard").gameObject;
			SellSelect = go.transform.FindChild("SellSelect").gameObject;
			SellLabel = go.transform.FindChild("SellSelect/SellLabel").gameObject.GetComponent<UILabel>();
			SellSelectCover = go.transform.FindChild("SellSelect/SellCover").gameObject;
			LightAnimation = go.transform.FindChild("InListCard/SpriteAnim/Shine").gameObject.GetComponent<UISpriteAnimation>();

			go.transform.FindChild ("InListCard/InlistLabel").gameObject.GetComponent<UILabel>().text = TextConst.S(7110);
			go.transform.FindChild ("SellSelect/SellCover/SellLabel").gameObject.GetComponent<UILabel>().text = TextConst.S(7114);

			UnavailableMask.SetActive(isFormation);
			InListCard.SetActive(isFormation);
			SellSelect.SetActive(isFormation);

			btn = self.GetComponent<UIButton>();
			isInit = SkillPic && SkillCard && SkillName && SkillStar && SkillSuit && SkillKind;

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
    			self.name = index.ToString();
    			SkillPic.mainTexture = GameData.CardTexture(skill.ID);
				SkillCard.spriteName = "cardlevel_" + Mathf.Clamp(skill.Lv, 1, GameData.DSkillData[skill.ID].MaxStar).ToString();
				SkillName.text = GameData.DSkillData[skill.ID].Name;
				SkillSuit.spriteName = "Levelball" + Mathf.Clamp(skill.Lv, 1, GameData.DSkillData[skill.ID].MaxStar).ToString();
				if(GameFunction.IsActiveSkill(skill.ID))
					SkillKind.spriteName = "ActiveIcon";
				else 
					SkillKind.spriteName = "PasstiveIcon";
				SkillKindBg.spriteName = "APIcon" + Mathf.Clamp(skill.Lv, 1, GameData.DSkillData[skill.ID].MaxStar).ToString();
            } else
                Debug.LogError("TActiveSkillCard.UpdateView skill id error " + skill.ID.ToString());
		}
		else
		{
			Debug.LogError("You needed to Init()");
		}
	}
	
	public bool Enable
	{
		set{self.gameObject.SetActive(value);}
	}


	//SkillFormation
	public bool IsInstall {
		get {return (InListCard != null && InListCard.activeSelf);}
	}

	public bool IsInstallIfDisapper {
		get {return (InListCard != null && InListCard.activeInHierarchy);}
	}

	public bool IsCanUse {
		get {return (UnavailableMask != null && !UnavailableMask.activeSelf);}
	}

	public void SetCoin (int money) {
		if(SellLabel != null)
			SellLabel.text = money.ToString();
	}
}
