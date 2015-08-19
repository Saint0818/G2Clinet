using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamePlayEnum;
using GameStruct;

namespace SkillControllerSpace {
//	
//	//ActiveSkill
//	private float activeTime  = 0;
//	private bool isUseSkill = false;


	public class SkillController : MonoBehaviour {
		private GameObject executePlayer;
		//PassiveSkill key: Kind  value: TSKill
		private EPassDirectState passDirect = EPassDirectState.Forward;
		public Dictionary<int, List<TSkill>> passiveSkills = new Dictionary<int, List<TSkill>>();
		public int passiveID;
		public int passiveLv;
		public int moveDodgeRate = 0;
		public int moveDodgeLv = 0;
		public int pickBall2Rate = 0;
		public int pickBall2Lv = 0;

		public void initSkillController(TPlayer Attribute, GameObject player){
			executePlayer = player;
			if (Attribute.Skills != null && Attribute.Skills.Length > 0) {
				for (int i = 0; i < Attribute.Skills.Length; i++) {
					if (GameData.SkillData.ContainsKey(Attribute.Skills[i].ID)) {
						TSkillData skillData = GameData.SkillData[Attribute.Skills[i].ID];
						
						Attribute.AddAttribute(skillData.AttrKind, skillData.Value(Attribute.Skills[i].Lv));
						
						int key = skillData.Kind;
						
						if (skillData.Kind == (int)ESkillKind.MoveDodge){
							moveDodgeLv = Attribute.Skills[i].Lv;
							moveDodgeRate = skillData.Rate(moveDodgeLv);
						}
						
						if (skillData.Kind == (int)ESkillKind.Pick2) {
							pickBall2Lv = Attribute.Skills[i].Lv;
							pickBall2Rate = skillData.Rate(pickBall2Lv);
						}
						
						TSkill skill = new TSkill();
						skill.ID = Attribute.Skills [i].ID;
						skill.Lv = Attribute.Skills [i].Lv;
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
			if(passiveSkills.ContainsKey(skillKind)) {
				if (passiveSkills[skillKind].Count > 0){
					float angle = GameFunction.GetPlayerToObjectAngleByVector(executePlayer.transform, v);
					int passiveRate = -1;
					if (kind == ESkillKind.Pass) {
						passDirect = judgeDirect(angle);
						for(int i=0; i<passiveSkills[(int)skillKind].Count; i++) 
							if (GameData.SkillData[passiveSkills[skillKind][i].ID].Direct == (int)passDirect)
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
								passiveID = passiveSkills[skillKind][i].ID;
								passiveLv = passiveSkills[skillKind][i].Lv;
								animationName = GameData.SkillData[passiveID].Animation;
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
							passiveID = passiveSkills[skillKind][i].ID;
							passiveLv = passiveSkills[skillKind][i].Lv;
							animationName = GameData.SkillData[passiveID].Animation;
							break;
						}
						
					} else {
						if(UnityEngine.Random.Range(0, 100) <= GameData.SkillData[passiveSkills[skillKind][i].ID].Rate(passiveSkills[skillKind][i].Lv)) {
							passiveID = passiveSkills[skillKind][i].ID;
							passiveLv = passiveSkills[skillKind][i].Lv;
							animationName = GameData.SkillData[passiveID].Animation;
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

	}
}
