using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamePlayEnum;
using GameStruct;

namespace SkillControllerSpace {
	public class SkillController : MonoBehaviour {
		private GameObject executePlayer;
		//PassiveSkill key: Kind  value: TSKill
		private EPassDirectState passDirect = EPassDirectState.Forward;
		public Dictionary<int, List<TSkill>> PassiveSkills = new Dictionary<int, List<TSkill>>();
		public int PassiveID;
		public int PassiveLv;
		public int MoveDodgeRate = 0;
		public int MoveDodgeLv = 0;
		public int PickBall2Rate = 0;
		public int PickBall2Lv = 0;

		//ActiveSkill
		public float ActiveTime  = 0;

		public void initSkillController(TPlayer Attribute, GameObject player, Animator animatorControl){
			executePlayer = player;
			if (Attribute.Skills != null && Attribute.Skills.Length > 0) {
				for (int i = 0; i < Attribute.Skills.Length; i++) {
					if (GameData.SkillData.ContainsKey(Attribute.Skills[i].ID)) {
						TSkillData skillData = GameData.SkillData[Attribute.Skills[i].ID];
						
						Attribute.AddAttribute(skillData.AttrKind, skillData.Value(Attribute.Skills[i].Lv));
						
						int key = skillData.Kind;
						
						if (skillData.Kind == (int)ESkillKind.MoveDodge){
							MoveDodgeLv = Attribute.Skills[i].Lv;
							MoveDodgeRate = skillData.Rate(MoveDodgeLv);
						}
						
						if (skillData.Kind == (int)ESkillKind.Pick2) {
							PickBall2Lv = Attribute.Skills[i].Lv;
							PickBall2Rate = skillData.Rate(PickBall2Lv);
						}
						
						TSkill skill = new TSkill();
						skill.ID = Attribute.Skills [i].ID;
						skill.Lv = Attribute.Skills [i].Lv;
						if (PassiveSkills.ContainsKey(key))
							PassiveSkills [key].Add(skill);
						else {
							List<TSkill> pss = new List<TSkill>();
							pss.Add(skill);
							PassiveSkills.Add(key, pss);
						}
					}
				}
			}

			ActiveTime = 0;
			if (GameData.SkillData.ContainsKey(Attribute.ActiveSkill.ID)) {
				AnimationClip[] clips = animatorControl.runtimeAnimatorController.animationClips;
				if (clips != null && clips.Length > 0) {
					for (int i=0; i<clips.Length; i++) {
						if(clips[i].name.Equals(GameData.SkillData [Attribute.ActiveSkill.ID].Animation)) {
							ActiveTime = clips[i].length;
							break;
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
			if(PassiveSkills.ContainsKey(skillKind)) {
				if (PassiveSkills[skillKind].Count > 0){
					float angle = GameFunction.GetPlayerToObjectAngleByVector(executePlayer.transform, v);
					int passiveRate = -1;
					if (kind == ESkillKind.Pass) {
						passDirect = judgeDirect(angle);
						for(int i=0; i<PassiveSkills[(int)skillKind].Count; i++) 
							if (GameData.SkillData[PassiveSkills[skillKind][i].ID].Direct == (int)passDirect)
								passiveRate += GameData.SkillData[PassiveSkills[(int)skillKind][i].ID].Rate(PassiveSkills[(int)skillKind][i].Lv);
					} else
						for(int i=0; i<PassiveSkills[(int)skillKind].Count; i++)
							passiveRate += GameData.SkillData[PassiveSkills[(int)skillKind][i].ID].Rate(PassiveSkills[(int)skillKind][i].Lv);
					
					isPerformPassive = (UnityEngine.Random.Range(0, 100) <= passiveRate) ? true : false;
				}
			}
			
			if (isPerformPassive){
				string animationName = string.Empty;
				for (int i=0; i<PassiveSkills[skillKind].Count; i++) {
					if(kind == ESkillKind.Pass) {
						if(GameData.SkillData[PassiveSkills[skillKind][i].ID].Direct == (int)passDirect) {
							if(UnityEngine.Random.Range(0, 100) <= GameData.SkillData[PassiveSkills[skillKind][i].ID].Rate(PassiveSkills[skillKind][i].Lv)){
								PassiveID = PassiveSkills[skillKind][i].ID;
								PassiveLv = PassiveSkills[skillKind][i].Lv;
								animationName = GameData.SkillData[PassiveID].Animation;
								break;
							}
						}
					} else 
						if(kind == ESkillKind.Shoot || kind == ESkillKind.NearShoot || kind == ESkillKind.UpHand || 
						  kind == ESkillKind.DownHand || kind == ESkillKind.Layup) { 
						if(UnityEngine.Random.Range(0, 100) <= GameData.SkillData[PassiveSkills[skillKind][i].ID].Rate(PassiveSkills[skillKind][i].Lv)) {
							if(isWideOpen != 0 && (PassiveSkills[skillKind][i].ID == 412 || PassiveSkills[skillKind][i].ID == 413)) {
								break;
							}
							PassiveID = PassiveSkills[skillKind][i].ID;
							PassiveLv = PassiveSkills[skillKind][i].Lv;
							animationName = GameData.SkillData[PassiveID].Animation;
							break;
						}
						
					} else {
						if(UnityEngine.Random.Range(0, 100) <= GameData.SkillData[PassiveSkills[skillKind][i].ID].Rate(PassiveSkills[skillKind][i].Lv)) {
							PassiveID = PassiveSkills[skillKind][i].ID;
							PassiveLv = PassiveSkills[skillKind][i].Lv;
							animationName = GameData.SkillData[PassiveID].Animation;
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
