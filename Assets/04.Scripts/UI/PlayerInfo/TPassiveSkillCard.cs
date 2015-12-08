using UnityEngine;
using System.Collections;
using GameStruct;

public class TPassiveSkillCard
{
	private GameObject item;
	private GameObject btnRemove;
	private UISprite SkillCard;
	private UISprite SkillLevel;
	private UITexture SkillTexture;
	private UILabel SkillName;
	private UILabel SkillCost;
	private bool isInit = false;
	private UIButton btn;
	
	public void Init(GameObject partent, GameObject go, EventDelegate btnFunc = null)
	{
		if (!isInit && go && partent) {
			go.transform.parent = partent.transform;
			go.transform.position = Vector3.zero;
			item = go;
			SkillCard =  go.transform.FindChild("SkillCard").gameObject.GetComponent<UISprite>();
			SkillLevel =  go.transform.FindChild("SkillLevel").gameObject.GetComponent<UISprite>();
			SkillName =  go.transform.FindChild("SkillName").gameObject.GetComponent<UILabel>();
			SkillCost =  go.transform.FindChild("SkillCost").gameObject.GetComponent<UILabel>();
			SkillTexture = go.transform.FindChild("SkillTexture").gameObject.GetComponent<UITexture>();
			btnRemove = go.transform.FindChild("BtnRemove").gameObject;
			btn = item.GetComponent<UIButton>();
			isInit =  SkillCard && SkillLevel && SkillName && SkillCost && btn && SkillTexture;
			
			if(isInit && btnFunc != null){
				btnRemove.SetActive(false);
				btn.onClick.Add(btnFunc);
			}
			else
				Debug.LogError("Init Error : TPassiveSkillCard");
		}
	}
	
	public void UpdateView(int index, TSkill skill, Vector3 pos)
	{
		if(isInit){
			item.name = index.ToString();
			
			if(GameData.DSkillData.ContainsKey(skill.ID)){
				SkillLevel.spriteName = "Levelball" + Mathf.Clamp(GameData.DSkillData[skill.ID].Quality, 1, 3).ToString();
				SkillCard.spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[skill.ID].Quality, 1, 3).ToString() + "s";
				SkillName.text = GameData.DSkillData[skill.ID].Name;
				SkillCost.text = Mathf.Max(GameData.DSkillData[skill.ID].Space(skill.Lv), 1).ToString();;
				SkillTexture.mainTexture = GameData.CardItemTexture(skill.ID);
			}

			item.transform.localPosition = pos;
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
