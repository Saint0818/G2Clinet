using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;

public class TSkillAttribute
{
	public int ID;
	public int Kind;
	public float Value;
	public float CDTime;
}

public delegate void OnAddAttributeDelegate(int skillID, int kind, float value, float lifetime);
public delegate void OnFinishAttributeDelegate(int index, int kind, float value);

public class SkillAttribute : MonoBehaviour {
	public OnAddAttributeDelegate OnAddAttribute = null;
	public OnFinishAttributeDelegate OnFinishAttribute = null;
	private PlayerBehaviour executePlayer;
	public PlayerBehaviour ExecutePlayer {
		set {executePlayer = value;}
	}

	private List<TSkillAttribute> skillAttribute = new List<TSkillAttribute>();

	void OnDestroy () {
		skillAttribute.Clear();
	}

	public void ResetGame () {
		skillAttribute.Clear();
	}

	private int findSkillAttribute(int skillID) {
		for (int i = 0; i < skillAttribute.Count; i++)
			if (skillAttribute[i].ID == skillID) 
				return i;

		return -1;
	}

	public void UpdateSkillAttribute() {
		if(skillAttribute.Count > 0) {
			for (int i = skillAttribute.Count-1; i >= 0; i--) { 
				if (skillAttribute [i].CDTime > 0) {
					skillAttribute [i].CDTime -= Time.deltaTime * TimerMgr.Get.CrtTime;  
					if (skillAttribute [i].CDTime <= 0) {
						if(OnFinishAttribute != null)
							OnFinishAttribute(i, skillAttribute[i].Kind, -skillAttribute[i].Value);
						
						skillAttribute.RemoveAt(i);
					}
				}
			}
		}
	}

	//Check TargetKind and Add Value to Player
	public void CheckSkillValueAdd(TSkill tSkill) {
		if(GameData.DSkillData.ContainsKey(tSkill.ID)) {
			TSkillData skill = GameData.DSkillData[tSkill.ID];
			if(executePlayer.Attribute.IsHaveActiveSkill) {
				if(skill.TargetKind == 3) { // Buff & My Teammate
					for (int i = 0; i < executePlayer.GamePlayers.Count; i++) {
						if (executePlayer.GamePlayers[i].Team.GetHashCode() == executePlayer.Team.GetHashCode()) {
							executePlayer.GamePlayers[i].PlayerSkillAttribute.AddSkillAttribute(skill.ID, 
								skill.AttrKind, 
								skill.Value(tSkill.Lv), 
								skill.LifeTime(tSkill.Lv));
						}
					}
				} else if(skill.TargetKind == 5) {
					for (int i = 0; i < executePlayer.GamePlayers.Count; i++) {
						if (executePlayer.GamePlayers[i].Team.GetHashCode() != executePlayer.Team.GetHashCode()) {
							executePlayer.GamePlayers[i].PlayerSkillAttribute.AddSkillAttribute(skill.ID, 
								skill.AttrKind, 
								skill.Value(tSkill.Lv), 
								skill.LifeTime(tSkill.Lv));
						}
					}
				} else 
					AddSkillAttribute(skill.ID, skill.AttrKind, skill.Value(tSkill.Lv), skill.LifeTime(tSkill.Lv));
			} else 
				AddSkillAttribute(skill.ID, skill.AttrKind, skill.Value(tSkill.Lv), skill.LifeTime(tSkill.Lv));


		}
	}
	//Add Value to Player
	public void AddSkillAttribute (int skillID, int kind, float value, float lifetime) {
		if (value != 0) {
			int index = findSkillAttribute(skillID);
			float add = 0;
			if (index == -1) {
				TSkillAttribute item = new TSkillAttribute();
				item.ID = skillID;
				item.Kind = kind;
				item.Value = value;
				item.CDTime = lifetime;
				skillAttribute.Add(item);
				add = value;
			} else {
				skillAttribute[index].CDTime = lifetime;
				if (value > 0 && value > skillAttribute[index].Value) 
					add = value - skillAttribute[index].Value;
				else if (value < 0 && value < skillAttribute[index].Value) 
					add = value - skillAttribute[index].Value;
				
			}

			if(add != 0)
				if(OnAddAttribute != null)
					OnAddAttribute(skillID, kind, add, lifetime);
		}
	}
}
