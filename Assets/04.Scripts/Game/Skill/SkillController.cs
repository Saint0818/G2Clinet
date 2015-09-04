using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamePlayEnum;
using GameStruct;
using SkillBuffSpace;


namespace SkillControllerSpace {
	public delegate void OnAddAttributeDelegate(int kind, float value);

	public class SkillController : MonoBehaviour {
		public OnAddAttributeDelegate OnAddAttribute = null;

		private const int IDLimitActive = 10000;

		private PlayerBehaviour executePlayer;
		//PassiveSkill key: Kind  value: TSKill
		private EPassDirectState passDirect = EPassDirectState.Forward;
		private Dictionary<string, List<GameObject>> activeSkillTargets = new Dictionary<string, List<GameObject>>();
		public Dictionary<int, List<TSkill>> DPassiveSkills = new Dictionary<int, List<TSkill>>();//Skill
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
			skillBuff.UpdateBuff();
			updateSkillAttribute();
		}

		public void initSkillController(TPlayer attribute, PlayerBehaviour player, Animator animatorControl){
			executePlayer = player;

			//PlayerInfo
			if(!isHavePlayerInfo) {
				isHavePlayerInfo = true;
				skillAttribute.Clear();
				GameObject obj = Instantiate((Resources.Load("Effect/PlayerInfo") as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
				skillBuff = new SkillBuff();
				skillBuff.InitBuff(obj, attribute, player.gameObject);
				skillBuff.OnFinishBuff = FinishBuff;
				//Passive
				if (attribute.SkillCards != null && attribute.SkillCards.Length > 0) {
					for (int i = 0; i < attribute.SkillCards.Length; i++) {
						if (GameData.DSkillData.ContainsKey(attribute.SkillCards[i].ID)) {
							TSkillData skillData = GameData.DSkillData[attribute.SkillCards[i].ID];
							
//							attribute.AddAttribute(skillData.AttrKind, skillData.Value(attribute.SkillCards[i].Lv));
							
							int key = skillData.Kind;
							
							if (skillData.Kind == (int)ESkillKind.MoveDodge){
								MoveDodgeLv = attribute.SkillCards[i].Lv;
								MoveDodgeRate = skillData.Rate(MoveDodgeLv);
							}
							
							if (skillData.Kind == (int)ESkillKind.Pick2) {
								PickBall2Lv = attribute.SkillCards[i].Lv;
								PickBall2Rate = skillData.Rate(PickBall2Lv);
							}
							
							TSkill skill = new TSkill();
							skill.ID = attribute.SkillCards [i].ID;
							skill.Lv = attribute.SkillCards [i].Lv;
							if (DPassiveSkills.ContainsKey(key))
								DPassiveSkills [key].Add(skill);
							else {
								List<TSkill> pss = new List<TSkill>();
								pss.Add(skill);
								DPassiveSkills.Add(key, pss);
							}
						}
					}
				}

				//Active
				ActiveTime = 0;
				if (GameData.DSkillData.ContainsKey(attribute.ActiveSkill.ID)) {
					AnimationClip[] clips = animatorControl.runtimeAnimatorController.animationClips;
					if (clips != null && clips.Length > 0) {
						for (int i=0; i<clips.Length; i++) {
							if(clips[i].name.Equals(GameData.DSkillData [attribute.ActiveSkill.ID].Animation)) {
								ActiveTime = clips[i].length;
								break;
							}
						}
					}
				}
			}
		}

		//without Active, Acitve is run at the SkillBuff
		private void updateSkillAttribute() {
			for (int i = skillAttribute.Count-1; i >= 0; i--) { 
				if (skillAttribute [i].CDTime > 0 && skillAttribute [i].ID <= IDLimitActive) {
					skillAttribute [i].CDTime -= Time.deltaTime;  
					if (skillAttribute [i].CDTime <= 0) {
						if(OnAddAttribute != null) 
							OnAddAttribute(skillAttribute[i].Kind, -skillAttribute[i].Value);
						skillAttribute.RemoveAt(i);
					}
				}
			}
		}

		private float getDis(Vector3 someone, Vector3 target) {
			if(someone != null && target != Vector3.zero)
				return Vector3.Distance(someone, target);
			
			return -1;
		}
		
		private int findSkillAttribute(int skillID) {
			for (int i = 0; i < skillAttribute.Count; i++)
				if (skillAttribute[i].ID == skillID) 
					return i;
			
			return -1;
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
		
		public void HidePlayerName (){
			skillBuff.HideName();
		}
		
		public List<int> GetAllBuff (){
			return skillBuff.GetAllBuff();
		}
		
		public void FinishBuff (int skillID){
			int index = findSkillAttribute(skillID);
			if(OnAddAttribute != null) 
				OnAddAttribute(skillAttribute[index].Kind, -skillAttribute[index].Value);
			skillAttribute.RemoveAt(index);
		}
		
		public void Reset (){
			skillBuff.RemoveAllBuff();
		}

		public void AddSkillAttribute (int skillID, int kind, float value, float lifetime) {
			if (value != 0) {
				int index = findSkillAttribute(skillID);
				if(skillID >= IDLimitActive)
					skillBuff.AddBuff(skillID, lifetime);
				
				if (index == -1) {
					TSkillAttribute item = new TSkillAttribute();
					item.ID = skillID;
					item.Kind = kind;
					item.Value = value;
					item.CDTime = lifetime;
					skillAttribute.Add(item);

					if(OnAddAttribute != null) 
						OnAddAttribute(kind, value);
				} else {
					float add = 0;
					skillAttribute[index].CDTime = lifetime;
					if (value > 0 && value > skillAttribute[index].Value) 
						add = value - skillAttribute[index].Value;
					else
						if (value < 0 && value < skillAttribute[index].Value) 
							add = value - skillAttribute[index].Value;
					
					if (add != 0) {
						if(OnAddAttribute != null) 
							OnAddAttribute(kind, add);
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
			if(DPassiveSkills.ContainsKey(skillKind)) {
				if (DPassiveSkills[skillKind].Count > 0){
					float angle = GameFunction.GetPlayerToObjectAngleByVector(executePlayer.gameObject.transform, v);
					int passiveRate = -1;
					if (kind == ESkillKind.Pass) {
						passDirect = judgeDirect(angle);
						for(int i=0; i<DPassiveSkills[(int)skillKind].Count; i++) 
							if (GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Direct == (int)passDirect)
								passiveRate += GameData.DSkillData[DPassiveSkills[(int)skillKind][i].ID].Rate(DPassiveSkills[(int)skillKind][i].Lv);
					} else
						for(int i=0; i<DPassiveSkills[(int)skillKind].Count; i++)
							passiveRate += GameData.DSkillData[DPassiveSkills[(int)skillKind][i].ID].Rate(DPassiveSkills[(int)skillKind][i].Lv);
					
					isPerformPassive = (UnityEngine.Random.Range(0, 100) <= passiveRate) ? true : false;
				}
			}
			
			if (isPerformPassive){
				string animationName = string.Empty;
				for (int i=0; i<DPassiveSkills[skillKind].Count; i++) {
					if(kind == ESkillKind.Pass) {
						if(GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Direct == (int)passDirect) {
							if(UnityEngine.Random.Range(0, 100) <= GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Rate(DPassiveSkills[skillKind][i].Lv)){
								PassiveID = DPassiveSkills[skillKind][i].ID;
								PassiveLv = DPassiveSkills[skillKind][i].Lv;
								animationName = GameData.DSkillData[PassiveID].Animation;
								break;
							}
						}
					} else 
						if(kind == ESkillKind.Shoot || kind == ESkillKind.NearShoot || kind == ESkillKind.UpHand || 
						  kind == ESkillKind.DownHand || kind == ESkillKind.Layup) { 
						if(UnityEngine.Random.Range(0, 100) <= GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Rate(DPassiveSkills[skillKind][i].Lv)) {
							if(isWideOpen != 0 && (DPassiveSkills[skillKind][i].ID == 412 || DPassiveSkills[skillKind][i].ID == 413)) {
								break;
							}
							PassiveID = DPassiveSkills[skillKind][i].ID;
							PassiveLv = DPassiveSkills[skillKind][i].Lv;
							animationName = GameData.DSkillData[PassiveID].Animation;
							break;
						}
						
					} else {
						if(UnityEngine.Random.Range(0, 100) <= GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Rate(DPassiveSkills[skillKind][i].Lv)) {
							PassiveID = DPassiveSkills[skillKind][i].ID;
							PassiveLv = DPassiveSkills[skillKind][i].Lv;
							animationName = GameData.DSkillData[PassiveID].Animation;
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

		public bool CheckSkillSituationForAI(PlayerBehaviour player) {
			int kind = GameData.DSkillData[player.Attribute.ActiveSkill.ID].Situation;

			if(GameController.Get.Joysticker == executePlayer)
				return true;

			// all the time
			if(kind == 0) 
				return true;
			
			//Attack
			if(kind == 1) { 
				if(player.Team == ETeamKind.Self && GameController.Get.Situation == EGameSituation.AttackA)
					return true;	
				
				if(player.Team == ETeamKind.Npc && GameController.Get.Situation == EGameSituation.AttackB) 
					return true;	
			}
			
			//Deffence
			if(kind == 2) { 
				if(player.Team == ETeamKind.Self && GameController.Get.Situation == EGameSituation.AttackB) 
					return true;	
				
				if(player.Team == ETeamKind.Npc && GameController.Get.Situation == EGameSituation.AttackA) 
					return true;	
			}
			
			//It has enemy in own deffence range
			if(kind == 3) { 
				float distance = GameData.DSkillData[player.Attribute.ActiveSkill.ID].Distance(player.Attribute.ActiveSkill.Lv);
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != player.Team)
						if(distance >= getDis(GameController.Get.GamePlayers[i].transform.position, player.transform.position))
							return true;
				}
			}
			
			//Player is BallOwner
			if(kind == 4 && player.IsBallOwner)
				return true;
			
			return false;
		}
		
		public List<GameObject> GetActiveSkillTarget(PlayerBehaviour player) {
			if (GameData.DSkillData.ContainsKey(player.Attribute.ActiveSkill.ID)) {
				string key  =  GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind.ToString();
				if(activeSkillTargets.ContainsKey(key)) {
					return activeSkillTargets[key];
				} else {
					List<GameObject> objs = new List<GameObject>();
					switch (GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind) {
					case 0:// self
						objs.Add(player.gameObject);
						break;
					case 1://my basket
						objs.Add(CourtMgr.Get.BasketHoop[player.Team.GetHashCode()].gameObject);
						break;
					case 2:{//enemy basket
						int i = 1;
						if (player.Team == ETeamKind.Npc)
							i = 0;
						
						objs.Add(CourtMgr.Get.BasketHoop[i].gameObject);
						break;
					}
					case 3://my all teammates
						for (int i = 0; i < GameController.Get.GamePlayers.Count; i++) {
							if (GameController.Get.GamePlayers[i].Team == player.Team) {
								objs.Add(GameController.Get.GamePlayers[i].gameObject);
							}
						}
						break;
					case 10://ball
						objs.Add(CourtMgr.Get.RealBall);
						break;
					}
					activeSkillTargets.Add(key , objs);
					return activeSkillTargets[key];
				}
			}
			
			return null;
		}

	}
}
