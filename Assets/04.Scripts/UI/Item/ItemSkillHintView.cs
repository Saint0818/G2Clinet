using UnityEngine;
using System.Collections;

public class ItemSkillHintView : MonoBehaviour {

	public UILabel SkillName;
	public UISprite SkillLevel;
	public UISprite SkillLevelball;
	public UILabel SkillMaxAnger;
	public UITexture SkillTexture;
	public UISprite SkillCard;


	public void UpdateUI (string name, string level, string quality, string anger, string maxAnger, int id) {

#if UNITY_EDITOR
		SkillName.text = name + "(" + id.ToString() + ")";
#else
		SkillName.text = name;
#endif
		SkillLevel.spriteName = "Cardicon" + level;
		SkillLevelball.spriteName = "Levelball" + quality;
		SkillMaxAnger.text = anger + "[13CECEFF]" + maxAnger + "[-]";
		SkillTexture.mainTexture = GameData.CardItemTexture(id);
		SkillCard.spriteName = "cardlevel_" + quality + "s";
	}
}
