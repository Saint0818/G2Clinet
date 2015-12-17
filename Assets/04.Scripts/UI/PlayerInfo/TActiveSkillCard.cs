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
	/// 注意：必須先init()才能 initbtttonfunction()，不可再Init使用button.OnClick.Add, 否則會造成無窮迴圈
	/// </remarks>
	private GameObject self;
	private UITexture SkillPic;
	private UISprite SkillCard;
	private UISprite SkillLevel;
	private UILabel SkillName;
	private UISprite SkillStar;
	private bool isInit = false;
	private UIButton btn;
	
	public void Init(GameObject go, EventDelegate btnFunc = null)
	{
		if (!isInit && go) {
			self = go;
			SkillPic = go.transform.FindChild("SkillPic").gameObject.GetComponent<UITexture>();
			SkillCard =  go.transform.FindChild("SkillCard").gameObject.GetComponent<UISprite>();
			SkillLevel =  go.transform.FindChild("SkillLevel").gameObject.GetComponent<UISprite>();
			SkillName =  go.transform.FindChild("SkillName").gameObject.GetComponent<UILabel>();
			SkillStar =  go.transform.FindChild("SkillStar").gameObject.GetComponent<UISprite>();
			btn = self.GetComponent<UIButton>();
			isInit = SkillPic && SkillCard && SkillLevel && SkillName && SkillStar;
			
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
    			SkillCard.spriteName = "cardlevel_" + Mathf.Clamp(GameData.DSkillData[skill.ID].Quality, 1, 3).ToString();
    			SkillStar.spriteName = "Staricon" + Mathf.Clamp(GameData.DSkillData[skill.ID].Star, 1, GameData.DSkillData[skill.ID].MaxStar).ToString();
    			SkillLevel.spriteName = "Cardicon" + Mathf.Clamp(skill.Lv, 1, 5).ToString();
    			
    			if(GameData.DSkillData.ContainsKey(skill.ID)){
    				SkillName.text = GameData.DSkillData[skill.ID].Name;
    			}
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
}
