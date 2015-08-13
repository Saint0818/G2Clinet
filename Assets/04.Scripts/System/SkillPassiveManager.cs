using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct; 

public class SkillPassiveManager : KnightSingleton<SkillPassiveManager> {

	private PlayerBehaviour executePlayer;
	//PassiveSkill key: Kind  value: TSKill
	private Dictionary<int, List<TSkill>> passiveSkills = new Dictionary<int, List<TSkill>>();
	private EPassDirectState passDirect = EPassDirectState.Forward;

	public void InitPassive (PlayerBehaviour player){
		executePlayer = player;
		//Passive
		if (player.Attribute.Skills != null && player.Attribute.Skills.Length > 0) {
			for (int i = 0; i < player.Attribute.Skills.Length; i++) {
				if (GameData.SkillData.ContainsKey(player.Attribute.Skills[i].ID)) {
					TSkillData skillData = GameData.SkillData[player.Attribute.Skills[i].ID];
					
					player.Attribute.AddAttribute(skillData.AttrKind, skillData.Value(player.Attribute.Skills[i].Lv));
					
					int key = skillData.Kind;
					
					if (skillData.Kind == (int)ESkillKind.MoveDodge){
						player.MoveDodgeLv = player.Attribute.Skills[i].Lv;
						player.MoveDodgeRate = skillData.Rate(player.MoveDodgeLv);
					}
					
					if (skillData.Kind == (int)ESkillKind.Pick2) {
						player.PickBall2Lv = player.Attribute.Skills[i].Lv;
						player.PickBall2Rate = skillData.Rate(player.PickBall2Lv);
					}
					
					TSkill skill = new TSkill();
					skill.ID = player.Attribute.Skills [i].ID;
					skill.Lv = player.Attribute.Skills [i].Lv;
					if (passiveSkills.ContainsKey(key))
						passiveSkills [key].Add(skill);
					else {
						List<TSkill> pss = new List<TSkill>();
						pss.Add(skill);
						passiveSkills.Add(key, pss);
					}
				}
			}
		}
	}

	public EPlayerState PassiveSkill(ESkillSituation situation, ESkillKind kind, Vector3 v = default(Vector3), int isWideOpen = 0) {
		EPlayerState playerState = EPlayerState.Idle;
		try {
			playerState = (EPlayerState)System.Enum.Parse(typeof(EPlayerState), situation.ToString());
		} catch {
			LogMgr.Get.LogError("this situation isn't contain EPlayerState:" + situation.ToString());
		}
		
		bool isPerformPassive = false;
		int skillKind = (int)kind;
		if(passiveSkills.ContainsKey(skillKind) && !executePlayer.IsPass) {
			if (passiveSkills[skillKind].Count > 0){
				float angle = GameFunction.GetPlayerToObjectAngleByVector(this.transform, v);
				int passiveRate = -1;
				if (kind == ESkillKind.Pass) {
					passDirect = judgeDirect(angle);
					for(int i=0; i<passiveSkills[(int)skillKind].Count; i++) 
						if (GameData.SkillData[passiveSkills[skillKind][i].ID].Direct == (int)passDirect && executePlayer.IsMoving)
							passiveRate += GameData.SkillData[passiveSkills[(int)skillKind][i].ID].Rate(passiveSkills[(int)skillKind][i].Lv);
				} else
					for(int i=0; i<passiveSkills[(int)skillKind].Count; i++)
						passiveRate += GameData.SkillData[passiveSkills[(int)skillKind][i].ID].Rate(passiveSkills[(int)skillKind][i].Lv);
				
				isPerformPassive = (UnityEngine.Random.Range(0, 100) <= passiveRate) ? true : false;
			}
		}
		
		if (isPerformPassive){
			string animationName = string.Empty;
			for (int i=0; i<passiveSkills[skillKind].Count; i++) {
				if(kind == ESkillKind.Pass) {
					if(GameData.SkillData[passiveSkills[skillKind][i].ID].Direct == (int)passDirect) {
						if(UnityEngine.Random.Range(0, 100) <= GameData.SkillData[passiveSkills[skillKind][i].ID].Rate(passiveSkills[skillKind][i].Lv)){
							executePlayer.PassiveID = passiveSkills[skillKind][i].ID;
							executePlayer.PassiveLv = passiveSkills[skillKind][i].Lv;
							animationName = GameData.SkillData[executePlayer.PassiveID].Animation;
							break;
						}
					}
				} else 
					if(kind == ESkillKind.Shoot || kind == ESkillKind.NearShoot || kind == ESkillKind.UpHand || 
					  kind == ESkillKind.DownHand || kind == ESkillKind.Layup) { 
					if(UnityEngine.Random.Range(0, 100) <= GameData.SkillData[passiveSkills[skillKind][i].ID].Rate(passiveSkills[skillKind][i].Lv)) {
						if(isWideOpen != 0 && (passiveSkills[skillKind][i].ID == 412 || passiveSkills[skillKind][i].ID == 413)) {
							break;
						}
						executePlayer.PassiveID = passiveSkills[skillKind][i].ID;
						executePlayer.PassiveLv = passiveSkills[skillKind][i].Lv;
						animationName = GameData.SkillData[executePlayer.PassiveID].Animation;
						break;
					}
					
				} else {
					if(UnityEngine.Random.Range(0, 100) <= GameData.SkillData[passiveSkills[skillKind][i].ID].Rate(passiveSkills[skillKind][i].Lv)) {
						executePlayer.PassiveID = passiveSkills[skillKind][i].ID;
						executePlayer.PassiveLv = passiveSkills[skillKind][i].Lv;
						animationName = GameData.SkillData[executePlayer.PassiveID].Animation;
						break;
					}
				}
			}
			if (animationName != string.Empty) {
				try {
					return (EPlayerState)System.Enum.Parse(typeof(EPlayerState), animationName);
				} catch {
					if(GameStart.Get.IsDebugAnimation)
						LogMgr.Get.LogError("AnimationName: '" + animationName + "'was not found.");
					return playerState;
				}
			} else 
				return playerState;
		} else
			return playerState;
	}

	private EPassDirectState judgeDirect(float angle) {
		EPassDirectState directState = EPassDirectState.Forward;
		
		if (angle < 60f && angle > -60f)
			directState = EPassDirectState.Forward;
		else 
			if (angle <= -60f && angle > -120f)
				directState = EPassDirectState.Left;
		else 
			if (angle < 120f && angle >= 60f)
				directState = EPassDirectState.Right;
		else 
			if (angle >= 120f || angle <= -120f)
				directState = EPassDirectState.Back; 
		
		return directState;
	}

	public Dictionary<int, List<TSkill>> PassiveSkills {
		get {return passiveSkills;}
	}
}
