using GameStruct;
using UnityEngine;

public class TPassiveSkillCard
{
	public TSkill Skill;
	public GameObject item;
	private UILabel SkillName;
	private UISprite SkillCard;
	private UITexture SkillTexture;
	private UILabel SkillCost;
	private UISprite SkillKind;
	private UISprite SkillKindBg;
	private GameObject ForReinforce;
	private UILabel ForReinforceLabel;
	private SkillCardStar[] SkillStars;
	public GameObject BtnRemove;
	private bool isInit = false;
	private UIButton btn;
	//For Reinforce
	public int CardIndex;
	
	public void Init(GameObject partent, GameObject go, EventDelegate btnFunc = null, bool isFormation = false)
	{
		if (!isInit && go && partent) {
			go.transform.parent = partent.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			item = go;
			SkillName =  go.transform.Find("SkillName").gameObject.GetComponent<UILabel>();
			SkillCard =  go.transform.Find("SkillCard").gameObject.GetComponent<UISprite>();
			SkillTexture = go.transform.Find("SkillTexture").gameObject.GetComponent<UITexture>();
			SkillCost = go.transform.Find("SkillCost").gameObject.GetComponent<UILabel>();
			SkillKind = go.transform.Find("SkillKind").gameObject.GetComponent<UISprite>();
			SkillKindBg = go.transform.Find("SkillKind/KindBg").gameObject.GetComponent<UISprite>();
			ForReinforce = go.transform.Find("ForReinforce").gameObject;
			ForReinforceLabel = go.transform.Find("ForReinforce/SelectLabel").gameObject.GetComponent<UILabel>();
			SkillStars = new SkillCardStar[5];
			for(int i=0; i<SkillStars.Length; i++) 
				SkillStars[i] = go.transform.Find("SkillStar/StarBG" + i.ToString()).gameObject.GetComponent<SkillCardStar>();

			BtnRemove = go.transform.Find("BtnRemove").gameObject;
			btn = item.GetComponent<UIButton>();
			BtnRemove.SetActive(isFormation);
			SkillCost.gameObject.SetActive(isFormation);
			ForReinforce.SetActive(false);
			isInit =  SkillCard  && SkillName  && btn && SkillTexture && SkillKind && SkillKindBg;
			
			if(isInit){
				if (btnFunc != null)
					btn.onClick.Add(btnFunc);
			} else
				Debug.LogError("Init Error : TPassiveSkillCard");
		}
	}
	
	public void UpdateView(int index, TSkill skill, Vector3 pos)
	{
		if(isInit){
			Skill = skill;
			item.name = index.ToString();
			
			if(GameData.DSkillData.ContainsKey(skill.ID)){
				SkillName.text = GameData.DSkillData[skill.ID].Name;
//				SkillName.color = TextConst.Color(GameData.DSkillData[skill.ID].Quality);
				SkillCard.spriteName = GameFunction.CardLevelName(skill.ID) + "s";
				SkillTexture.mainTexture = GameData.CardItemTexture(skill.ID);
				if(GameFunction.IsActiveSkill(skill.ID))
					SkillKind.spriteName = "ActiveIcon";
				else 
					SkillKind.spriteName = "PasstiveIcon";
				SkillKindBg.spriteName = "APIcon" + GameData.DSkillData[skill.ID].Quality.ToString();
				GameFunction.ShowStar_Item(ref SkillStars, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);
			}


			item.transform.localPosition = pos;
		}
		else
		{
			Debug.LogError("You needed to Init()");
		}
	}

	public void InitFormation(GameObject go)
	{
		if (!isInit && go) {
			go.transform.localScale = Vector3.one;
			SkillName =  go.transform.Find("SkillName").gameObject.GetComponent<UILabel>();
			SkillCard =  go.transform.Find("SkillCard").gameObject.GetComponent<UISprite>();
			SkillTexture = go.transform.Find("SkillTexture").gameObject.GetComponent<UITexture>();
			SkillCost = go.transform.Find("SkillCost").gameObject.GetComponent<UILabel>();
			SkillKind = go.transform.Find("SkillKind").gameObject.GetComponent<UISprite>();
			SkillKindBg = go.transform.Find("SkillKind/KindBg").gameObject.GetComponent<UISprite>();
			ForReinforce = go.transform.Find("ForReinforce").gameObject;
			ForReinforceLabel = go.transform.Find("ForReinforce/SelectLabel").gameObject.GetComponent<UILabel>();
			SkillStars = new SkillCardStar[5];
			for(int i=0; i<SkillStars.Length; i++) 
				SkillStars[i] = go.transform.Find("SkillStar/StarBG" + i.ToString()).gameObject.GetComponent<SkillCardStar>();

			BtnRemove = go.transform.Find("BtnRemove").gameObject;
			BtnRemove.SetActive(true);
			ForReinforce.SetActive(false);
			isInit =  SkillCard  && SkillName && SkillTexture && SkillKind && SkillKindBg;

			if(!isInit)
				Debug.LogError("Init Error : TPassiveSkillCard");
		}
	}

	public void UpdateViewFormation(int id, int lv)
	{
		if(isInit){
			if(GameData.DSkillData.ContainsKey(id)){
				SkillName.text = GameData.DSkillData[id].Name;
//				SkillName.color = TextConst.Color(GameData.DSkillData[id].Quality);
				SkillCard.spriteName = GameFunction.CardLevelName(id) + "s";
				SkillTexture.mainTexture = GameData.CardItemTexture(id);
				SkillCost.text = string.Format(TextConst.S(7501), GameData.DSkillData[id].Space(lv));
				if(GameFunction.IsActiveSkill(id))
					SkillKind.spriteName = "ActiveIcon";
				else 
					SkillKind.spriteName = "PasstiveIcon";
				SkillKindBg.spriteName = "APIcon" + GameData.DSkillData[id].Quality.ToString();
				GameFunction.ShowStar_Item(ref SkillStars, lv, GameData.DSkillData[id].Quality, GameData.DSkillData[id].MaxStar);
			}
		}
		else
		{
			Debug.LogError("You needed to Init()");
		}
	}

	public void InitReinforce(GameObject go, int skillCardIndex)
	{
		if (!isInit && go) {
			item = go;
			CardIndex = skillCardIndex;
			go.transform.localScale = Vector3.one;
			SkillName =  go.transform.Find("SkillName").gameObject.GetComponent<UILabel>();
			SkillCard =  go.transform.Find("SkillCard").gameObject.GetComponent<UISprite>();
			SkillTexture = go.transform.Find("SkillTexture").gameObject.GetComponent<UITexture>();
			SkillCost = go.transform.Find("SkillCost").gameObject.GetComponent<UILabel>();
			SkillKind = go.transform.Find("SkillKind").gameObject.GetComponent<UISprite>();
			SkillKindBg = go.transform.Find("SkillKind/KindBg").gameObject.GetComponent<UISprite>();
			ForReinforce = go.transform.Find("ForReinforce").gameObject;
			ForReinforceLabel = go.transform.Find("ForReinforce/SelectLabel").gameObject.GetComponent<UILabel>();
			SkillStars = new SkillCardStar[5];
			for(int i=0; i<SkillStars.Length; i++) 
				SkillStars[i] = go.transform.Find("SkillStar/StarBG" + i.ToString()).gameObject.GetComponent<SkillCardStar>();

			BtnRemove = go.transform.Find("BtnRemove").gameObject;
			BtnRemove.SetActive(false);
			ForReinforce.SetActive(false);
			isInit =  SkillCard  && SkillName && SkillTexture && SkillKind && SkillKindBg;

			if(!isInit)
				Debug.LogError("Init Error : TPassiveSkillCard");
		}
	}

	public void UpdateViewReinforce(TSkill skill)
	{
		if(isInit){
			if(GameData.DSkillData.ContainsKey(skill.ID)){
				Skill = skill;
				SkillName.text = GameData.DSkillData[skill.ID].Name;
//				SkillName.color = TextConst.Color(GameData.DSkillData[skill.ID].Quality);
				SkillCard.spriteName = GameFunction.CardLevelName(skill.ID) + "s";
				SkillTexture.mainTexture = GameData.CardItemTexture(skill.ID);
				SkillCost.text = string.Format(TextConst.S(7502), GameData.DSkillData[skill.ID].ExpInlay(skill.Lv));
				if(GameFunction.IsActiveSkill(skill.ID))
					SkillKind.spriteName = "ActiveIcon";
				else 
					SkillKind.spriteName = "PasstiveIcon";
				SkillKindBg.spriteName = "APIcon" + GameData.DSkillData[skill.ID].Quality.ToString();
				GameFunction.ShowStar_Item(ref SkillStars, skill.Lv, GameData.DSkillData[skill.ID].Quality, GameData.DSkillData[skill.ID].MaxStar);
			}
		}
		else
		{
			Debug.LogError("You needed to Init()");
		}
	}

	public void ChooseReinforce (bool isShow, int index = 0) {
		ForReinforce.SetActive(isShow);
		ForReinforceLabel.text = index.ToString();
	}

	public void InitSuitItem(GameObject go, UIEventListener.VoidDelegate btnFun)
	{
		if (!isInit && go) {
			item = go;
			UIEventListener.Get(item).onClick = btnFun;
			go.transform.localScale = Vector3.one;
			SkillName =  go.transform.Find("SkillName").gameObject.GetComponent<UILabel>();
			SkillCard =  go.transform.Find("SkillCard").gameObject.GetComponent<UISprite>();
			SkillTexture = go.transform.Find("SkillTexture").gameObject.GetComponent<UITexture>();
			SkillCost = go.transform.Find("SkillCost").gameObject.GetComponent<UILabel>();
			SkillKind = go.transform.Find("SkillKind").gameObject.GetComponent<UISprite>();
			SkillKindBg = go.transform.Find("SkillKind/KindBg").gameObject.GetComponent<UISprite>();
			ForReinforce = go.transform.Find("ForReinforce").gameObject;
			ForReinforceLabel = go.transform.Find("ForReinforce/SelectLabel").gameObject.GetComponent<UILabel>();
			SkillStars = new SkillCardStar[5];
			for(int i=0; i<SkillStars.Length; i++) 
				SkillStars[i] = go.transform.Find("SkillStar/StarBG" + i.ToString()).gameObject.GetComponent<SkillCardStar>();

			BtnRemove = go.transform.Find("BtnRemove").gameObject;
			BtnRemove.SetActive(false);
			ForReinforce.SetActive(false);
			SkillCost.gameObject.SetActive(false);
			isInit =  SkillCard  && SkillName && SkillTexture && SkillKind && SkillKindBg;

			if(!isInit)
				Debug.LogError("Init Error : TPassiveSkillCard");
		}
	}

	public void UpdateViewSuitItem(int id)
	{
		if(isInit){
			if(GameData.DSkillData.ContainsKey(id)){
				item.name = id.ToString();
				SkillName.text = GameData.DSkillData[id].Name;
				SkillCard.spriteName = GameFunction.CardLevelName(id) + "s";
				SkillTexture.mainTexture = GameData.CardItemTexture(id);
				if(GameFunction.IsActiveSkill(id))
					SkillKind.spriteName = "ActiveIcon";
				else 
					SkillKind.spriteName = "PasstiveIcon";
				SkillKindBg.spriteName = "APIcon" + GameData.DSkillData[id].Quality.ToString();
				GameFunction.ShowStar_Item(ref SkillStars, GameData.DSkillData[id].MaxStar, GameData.DSkillData[id].Quality, GameData.DSkillData[id].MaxStar);
			}
		}
		else
		{
			Debug.LogError("You needed to Init()");
		}
	}

	public string Name {
		get {return item.name;}
	}

	public bool Enable
	{
		set{item.gameObject.SetActive(value);}
	}
}
