﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamePlayEnum;
using GameStruct;
using SkillBuffSpace;

namespace SkillControllerSpace {
	public delegate void OnAddAttribute(int kind, float value);

	public class SkillController : MonoBehaviour {
		public OnAddAttribute onAddAttribute = null;

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

		//PlayerInfo
		private bool isHavePlayerInfo = false;

		//SkillAttribute impact BuffShow
		private SkillBuff skillBuff;
		private List<TSkillAttribute> skillAttribute = new List<TSkillAttribute>();

		public void SkillUpdate () {
			updateSkillAttirbe();
			skillBuff.UpdateBuff();
		}

		public void initSkillController(TPlayer attribute, GameObject player, Animator animatorControl){
			executePlayer = player;
			skillAttribute.Clear();

			//PlayerInfo
			if(!isHavePlayerInfo) {
				isHavePlayerInfo = true;
				GameObject obj = Instantiate((Resources.Load("Effect/PlayerInfo") as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
				skillBuff = new SkillBuff();
				skillBuff.InitBuff(obj, attribute, player);

				//Passive
				if (attribute.Skills != null && attribute.Skills.Length > 0) {
					for (int i = 0; i < attribute.Skills.Length; i++) {
						if (GameData.SkillData.ContainsKey(attribute.Skills[i].ID)) {
							TSkillData skillData = GameData.SkillData[attribute.Skills[i].ID];
							
							attribute.AddAttribute(skillData.AttrKind, skillData.Value(attribute.Skills[i].Lv));
							
							int key = skillData.Kind;
							
							if (skillData.Kind == (int)ESkillKind.MoveDodge){
								MoveDodgeLv = attribute.Skills[i].Lv;
								MoveDodgeRate = skillData.Rate(MoveDodgeLv);
							}
							
							if (skillData.Kind == (int)ESkillKind.Pick2) {
								PickBall2Lv = attribute.Skills[i].Lv;
								PickBall2Rate = skillData.Rate(PickBall2Lv);
							}
							
							TSkill skill = new TSkill();
							skill.ID = attribute.Skills [i].ID;
							skill.Lv = attribute.Skills [i].Lv;
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

				//Active
				ActiveTime = 0;
				if (GameData.SkillData.ContainsKey(attribute.ActiveSkill.ID)) {
					AnimationClip[] clips = animatorControl.runtimeAnimatorController.animationClips;
					if (clips != null && clips.Length > 0) {
						for (int i=0; i<clips.Length; i++) {
							if(clips[i].name.Equals(GameData.SkillData [attribute.ActiveSkill.ID].Animation)) {
								ActiveTime = clips[i].length;
								break;
							}
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

		public void AddSkillAttribute (int skillID, int kind, float value, float lifetime) {
			if (value != 0) {
				int index = findSkillAttribute(skillID);
				skillBuff.AddBuff(skillID, lifetime);
				
				if (index == -1) {
					TSkillAttribute item = new TSkillAttribute();
					item.ID = skillID;
					item.Kind = kind;
					item.Value = value;
					item.CDTime = lifetime;
					skillAttribute.Add(item);

					if(onAddAttribute != null) 
						onAddAttribute(kind, value);
				} else {
					float add = 0;
					skillAttribute[index].CDTime = lifetime;
					if (value > 0 && value > skillAttribute[index].Value) 
						add = value - skillAttribute[index].Value;
					else
						if (value < 0 && value < skillAttribute[index].Value) 
							add = value - skillAttribute[index].Value;
					
					if (add != 0) {
						if(onAddAttribute != null) 
							onAddAttribute(kind, add);
					}
				}
			}
		}

		private int findSkillAttribute(int skillID) {
			for (int i = 0; i < skillAttribute.Count; i++)
				if (skillAttribute[i].ID == skillID) 
					return i;
			
			return -1;
		}

		private void updateSkillAttirbe() {
			for (int i = skillAttribute.Count-1; i >= 0; i--) { 
				if (skillAttribute [i].CDTime > 0) {
					skillAttribute [i].CDTime -= Time.deltaTime;   
					if (skillAttribute [i].CDTime <= 0) {
						if(onAddAttribute != null) 
							onAddAttribute(skillAttribute[i].Kind, -skillAttribute[i].Value);
						skillAttribute.RemoveAt(i);
					}
				}
			}
		}


	}
}
