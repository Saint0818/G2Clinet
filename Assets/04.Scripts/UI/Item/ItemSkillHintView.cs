using UnityEngine;

public class ItemSkillHintView : MonoBehaviour {

	public UILabel SkillName;
	public UILabel SkillMaxAnger;
	public UILabel SkillLabelMax;
	public UITexture SkillTexture;
	public UISprite SkillCard;
	public UILabel SkillExplain;
	public UISprite SkillKind;
	public UISprite KindBg;


	public void UpdateUI (string name, int level, int quality, string anger, string maxAnger, int id) {

		SkillName.text = name;
		SkillMaxAnger.text = anger + "[13CECEFF]" + maxAnger + "[-]";
		SkillTexture.mainTexture = GameData.CardItemTexture(id);
		SkillCard.spriteName = "cardlevel_" + quality + "s";
		if(GameFunction.IsActiveSkill(id))
			SkillKind.spriteName = "ActiveIcon";
		else 
			SkillKind.spriteName = "PasstiveIcon";
		
		if(SkillLabelMax != null)
			SkillLabelMax.text = TextConst.S(7302);
		
		if(GameData.DSkillData.ContainsKey(id)) {
			SkillExplain.text = GameFunction.GetStringExplain(GameData.DSkillData[id].Explain, id, level);
			KindBg.spriteName = "APIcon" + GameData.DSkillData[id].Quality.ToString();
		}
	}
}
