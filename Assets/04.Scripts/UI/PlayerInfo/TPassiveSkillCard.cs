using UnityEngine;
using System.Collections;
using GameStruct;

public class TPassiveSkillCard
{
	public TSkill Skill;
	private GameObject item;
	private UILabel SkillName;
	private UISprite SkillCard;
	private UITexture SkillTexture;
	private UISprite SkillKind;
	private UISprite SkillKindBg;
	private GameObject ForReinforce;
	private UILabel ForReinforceLabel;
	private SkillCardStar[] SkillStars;
	public GameObject BtnRemove;
	private bool isInit = false;
	private UIButton btn;
	
	public void Init(GameObject partent, GameObject go, EventDelegate btnFunc = null, bool isFormation = false)
	{
		if (!isInit && go && partent) {
			go.transform.parent = partent.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			item = go;
			SkillName =  go.transform.FindChild("SkillName").gameObject.GetComponent<UILabel>();
			SkillCard =  go.transform.FindChild("SkillCard").gameObject.GetComponent<UISprite>();
			SkillTexture = go.transform.FindChild("SkillTexture").gameObject.GetComponent<UITexture>();
			SkillKind = go.transform.FindChild("SkillKind").gameObject.GetComponent<UISprite>();
			SkillKindBg = go.transform.FindChild("SkillKind/KindBg").gameObject.GetComponent<UISprite>();
			ForReinforce = go.transform.FindChild("ForReinforce").gameObject;
			ForReinforceLabel = go.transform.FindChild("ForReinforce/SelectLabel").gameObject.GetComponent<UILabel>();
			SkillStars = new SkillCardStar[5];
			for(int i=0; i<SkillStars.Length; i++) 
				SkillStars[i] = go.transform.FindChild("SkillStar/StarBG" + i.ToString()).gameObject.GetComponent<SkillCardStar>();

			BtnRemove = go.transform.FindChild("BtnRemove").gameObject;
			btn = item.GetComponent<UIButton>();
			BtnRemove.SetActive(isFormation);
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
				SkillCard.spriteName = "cardlevel_" + GameData.DSkillData[skill.ID].Quality.ToString() + "s";
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
			SkillName =  go.transform.FindChild("SkillName").gameObject.GetComponent<UILabel>();
			SkillCard =  go.transform.FindChild("SkillCard").gameObject.GetComponent<UISprite>();
			SkillTexture = go.transform.FindChild("SkillTexture").gameObject.GetComponent<UITexture>();
			SkillKind = go.transform.FindChild("SkillKind").gameObject.GetComponent<UISprite>();
			SkillKindBg = go.transform.FindChild("SkillKind/KindBg").gameObject.GetComponent<UISprite>();
			ForReinforce = go.transform.FindChild("ForReinforce").gameObject;
			ForReinforceLabel = go.transform.FindChild("ForReinforce/SelectLabel").gameObject.GetComponent<UILabel>();
			SkillStars = new SkillCardStar[5];
			for(int i=0; i<SkillStars.Length; i++) 
				SkillStars[i] = go.transform.FindChild("SkillStar/StarBG" + i.ToString()).gameObject.GetComponent<SkillCardStar>();

			BtnRemove = go.transform.FindChild("BtnRemove").gameObject;
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
				SkillCard.spriteName = "cardlevel_" + GameData.DSkillData[id].Quality.ToString() + "s";
				SkillTexture.mainTexture = GameData.CardItemTexture(id);
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
	
	public bool Enable
	{
		set{item.gameObject.SetActive(value);}
	}
}
