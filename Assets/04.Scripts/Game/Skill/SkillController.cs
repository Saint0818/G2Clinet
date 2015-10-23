using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamePlayEnum;
using GameStruct;
using SkillBuffSpace;
using DG.Tweening;
using G2;

public delegate void OnAddAttributeDelegate(int kind, float value);

public class SkillController : MonoBehaviour {
	public OnAddAttributeDelegate OnAddAttribute = null;

	private PlayerBehaviour executePlayer;
	//PassiveSkill key: Kind  value: TSKill
	private EPassDirectState passDirect = EPassDirectState.Forward;
	private Dictionary<string, List<GameObject>> activeSkillTargets = new Dictionary<string, List<GameObject>>();
	public Dictionary<int, List<TSkill>> DPassiveSkills = new Dictionary<int, List<TSkill>>();//Skill
	[HideInInspector]
	public TSkill ActiveSkillUsed;
	[HideInInspector]
	public TSkill PassiveSkillUsed;
	[HideInInspector]
	public int MoveDodgeRate = 0;
	[HideInInspector]
	public int MoveDodgeLv = 0;
	[HideInInspector]
	public int PickBall2Rate = 0;
	[HideInInspector]
	public int PickBall2Lv = 0;

	//ActiveSkill
//	[HideInInspector]
//	public float[] ActiveTime  = new float[3];

	//PlayerInfo
	private bool isHavePlayerInfo = false;

	//SkillAttribute impact BuffShow
	private SkillBuff skillBuff;
	private List<TSkillAttribute> skillAttribute = new List<TSkillAttribute>();

	void FixedUpdate() {
		skillBuff.UpdateBuff();
		updateSkillAttribute();
		if (GameController.Get.Situation == EGameSituation.JumpBall || 
		    GameController.Get.Situation == EGameSituation.AttackGamer || 
		    GameController.Get.Situation == EGameSituation.AttackNPC)
			judgeSkillUI();
	}
	
	private void judgeSkillUI()
	{
		if(executePlayer && executePlayer == GameController.Get.Joysticker && GameController.Get.Joysticker.Attribute.ActiveSkills.Count > 0 ){
			for(int i=0; i<executePlayer.Attribute.ActiveSkills.Count; i++) {
				if(executePlayer.Attribute.ActiveSkills[i].ID > 0 && GameController.Get.IsStart)
				{
					CourtMgr.Get.SkillArea(executePlayer.Team.GetHashCode(), executePlayer.IsAngerFull(executePlayer.Attribute.ActiveSkills[i]));
					UIGame.Get.ShowSkillEnableUI(GameController.Get.IsStart, i, executePlayer.IsAngerFull(executePlayer.Attribute.ActiveSkills[i]), executePlayer.CheckSkill(executePlayer.Attribute.ActiveSkills[i]));
				}
			}
		}
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
						GameData.CardTexture(attribute.SkillCards[i].ID);
						TSkillData skillData = GameData.DSkillData[attribute.SkillCards[i].ID];
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
//			if(attribute.ActiveSkills.Count > 0) {
//				for(int i=0; i<attribute.ActiveSkills.Count; i++) {
//					if (GameData.DSkillData.ContainsKey(attribute.ActiveSkills[i].ID)) {
//						AnimationClip[] clips = animatorControl.runtimeAnimatorController.animationClips;
//						if (clips != null && clips.Length > 0) {
//							for (int j=0; j<clips.Length; j++) {
//								if(clips[j].name.Equals(GameData.DSkillData [attribute.ActiveSkills[i].ID].Animation)) {
//									ActiveTime[i] = clips[j].length;
//									break;
//								}
//							}
//						}
//					}
//				}
//			}
		}
	}

	//without Active, Acitve is run at the SkillBuff
	private void updateSkillAttribute() {
		for (int i = skillAttribute.Count-1; i >= 0; i--) { 
			if (skillAttribute [i].CDTime > 0 && skillAttribute [i].ID <= GameConst.ID_LimitActive) {
				skillAttribute [i].CDTime -= Time.deltaTime * TimerMgr.Get.CrtTime;  
				if (skillAttribute [i].CDTime <= 0) {
					if(OnAddAttribute != null) 
						OnAddAttribute(skillAttribute[i].Kind, -skillAttribute[i].Value);

					skillAttribute.RemoveAt(i);
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

	private EPlayerState getPassiveSkill(ESkillSituation situation, ESkillKind kind, Vector3 v = default(Vector3), int isHaveDefPlayer = 0) {
		EPlayerState playerState = EPlayerState.Idle;
		try {
			playerState = (EPlayerState)System.Enum.Parse(typeof(EPlayerState), situation.ToString());
		} catch {
			LogMgr.Get.LogError("this situation isn't contain EPlayerState:" + situation.ToString());
		}
		if((GameController.Get.Situation == EGameSituation.InboundsGamer || GameController.Get.Situation == EGameSituation.InboundsNPC) && kind == ESkillKind.Pass) {
			playerState = EPlayerState.Pass50;
		}
		if(GameController.Get.Situation == EGameSituation.AttackGamer || GameController.Get.Situation == EGameSituation.AttackNPC) {
			string animationName = randomPassive(kind, v, isHaveDefPlayer);

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

	private string randomPassive(ESkillKind kind, Vector3 v = default(Vector3), int isHaveDefPlayer = 0) {
		int skillKind = (int)kind;
		//Part 1. Get Passive which is choosed. 
		List<TSkill> skills = new List<TSkill>();

		float angle = MathUtils.FindAngle(executePlayer.gameObject.transform, v);
		passDirect = judgeDirect(angle);

		if(DPassiveSkills.ContainsKey(skillKind)) {
			for (int i=0; i<DPassiveSkills[skillKind].Count; i++) {
				if(kind == ESkillKind.Pass) 
					if(GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Direct == (int)passDirect) 
						if(UnityEngine.Random.Range(0, 100) <= GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Rate(DPassiveSkills[skillKind][i].Lv))
							skills.Add(DPassiveSkills[skillKind][i]);
				else 
				if(kind == ESkillKind.Shoot || kind == ESkillKind.NearShoot || kind == ESkillKind.UpHand || kind == ESkillKind.DownHand || kind == ESkillKind.Layup) { 
					if(UnityEngine.Random.Range(0, 100) <= GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Rate(DPassiveSkills[skillKind][i].Lv)) {
						if(isHaveDefPlayer != 0 && (DPassiveSkills[skillKind][i].ID == 412 || DPassiveSkills[skillKind][i].ID == 413))
							break;
						
						skills.Add(DPassiveSkills[skillKind][i]);
					}
				} else
					if(UnityEngine.Random.Range(0, 100) <= GameData.DSkillData[DPassiveSkills[skillKind][i].ID].Rate(DPassiveSkills[skillKind][i].Lv))
						skills.Add(DPassiveSkills[skillKind][i]);
			}
			//Part 2. Get Passive 
			if(skills.Count > 0) {
				AI.WeightedRandomizer<TSkill> randomizer = new AI.WeightedRandomizer<TSkill>();
				for(int i=0; i<skills.Count; i++) 
					randomizer.AddOrUpdate(skills[i], GameData.DSkillData[skills[i].ID].Rate(skills[i].Lv));
				
				TSkill skill = randomizer.GetNext();
				PassiveSkillUsed = skill;
				return GameData.DSkillData[skill.ID].Animation;
			} else 
				return string.Empty;
		} else 
			return string.Empty;
	}
	
	private bool checkSkillBaseSituation(PlayerBehaviour player, TSkill tSkill) {
		if(player.Attribute.ActiveSkills.Count > 0) {
			int kind = GameData.DSkillData[tSkill.ID].Kind;
			switch (GameController.Get.Situation) {
			case EGameSituation.AttackGamer:
				if(player.Team == ETeamKind.Self) {
					if (kind >= 1 && kind <= 7 && player.IsBallOwner )
						return true;
					
					if ((kind == 11 || kind == 18) && player.IsBallOwner) 
						return true;
					
					if (kind == 17 && !player.IsBallOwner) 
						return true;
					
					if (kind == 12 || kind == 13 || kind == 14 || kind == 19 || kind == 20 || kind == 21)
						return true;
					
				} else {
					if (kind == 16)
						return true;
					
					if (kind == 15 && !player.IsBallOwner && GameController.Get.CanUseStealSkill) 
						return true;
					
					if (kind == 17 && !player.IsBallOwner) 
						return true;
					
					if (kind == 13 || kind == 14 || kind == 19 || kind == 20 || kind == 21)
						return true;
				}	
				
				break;
			case EGameSituation.AttackNPC:
				if(player.Team == ETeamKind.Self) {
					if (kind == 16)
						return true;
					
					if (kind == 15 && !player.IsBallOwner && GameController.Get.CanUseStealSkill) 
						return true;
					
					if (kind == 17 && !player.IsBallOwner) 
						return true;
					
					if (kind == 13 || kind == 14 || kind == 19 || kind == 20 || kind == 21)
						return true;
					
				} else  {
					if (kind >= 1 && kind <= 7 && player.IsBallOwner )
						return true;
					
					if ((kind == 11 || kind == 18) && player.IsBallOwner) 
						return true;
					
					if (kind == 17 && !player.IsBallOwner) 
						return true;
					
					if (kind == 12 || kind == 13 || kind == 14 || kind == 19 || kind == 20 || kind == 21)
						return true;
				}
				
				break;
			}
		}
		return false;
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
			if(skillID >= GameConst.ID_LimitActive)
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

	public bool CheckSkill(PlayerBehaviour player, TSkill tSkill, GameObject target = null) {
		if (player.CanUseActiveSkill(tSkill) && player.Attribute.ActiveSkills.Count > 0 && tSkill.ID > 0) {
			if (target) {
				if(GameData.DSkillData[tSkill.ID].TargetKind != 1 && 
				   GameData.DSkillData[tSkill.ID].TargetKind != 2) {
					//Target(People)
					if (target == player.gameObject || GameController.Get.GetDis(player, new Vector2(target.transform.position.x, target.transform.position.z)) <= 
					    GameData.DSkillData[tSkill.ID].Distance(tSkill.Lv)) {
						if (checkSkillBaseSituation(player, tSkill))
							return true;
					}
				} else {
					//Basket
					if (target == player.gameObject || GameController.Get.GetDis(player, new Vector2(CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.x, CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position.z)) <= 
					    GameData.DSkillData[tSkill.ID].Distance(tSkill.Lv)) {
						if (checkSkillBaseSituation(player, tSkill))
							return true;
					}
				}
			} else
				return true;
		}
		
		return false;
	}
	
	public void AddSkillAttribute(PlayerBehaviour player, TSkill tSkill) {
		TSkillData skill = GameData.DSkillData[tSkill.ID];
		if(tSkill.ID >= GameConst.ID_LimitActive) {
			if(player.Attribute.ActiveSkills.Count > 0) {
				if(skill.Kind == 21 && skill.TargetKind == 3) {
					for (int i = 0; i < GameController.Get.GamePlayers.Count; i++) {
						if (GameController.Get.GamePlayers[i].Team.GetHashCode() == player.Team.GetHashCode()) {
							if(CheckSkill(player, tSkill, GameController.Get.GamePlayers[i].gameObject)) {
								GameController.Get.GamePlayers[i].AddSkillAttribute(skill.ID, 
								                                                    skill.AttrKind, 
								                                                    skill.Value(tSkill.Lv), 
								                                                    skill.LifeTime(tSkill.Lv));
							}
						}
					}
				} else {
					player.AddSkillAttribute(skill.ID, 
					                         skill.AttrKind, 
					                         skill.Value(tSkill.Lv), 
					                         skill.LifeTime(tSkill.Lv));
				}
			}
		} else {
			player.AddSkillAttribute(skill.ID, 
			                         skill.AttrKind, 
			                         skill.Value(tSkill.Lv), 
			                         skill.LifeTime(tSkill.Lv));
		}
	}

	public bool DoPassiveSkill(ESkillSituation State, PlayerBehaviour player = null, Vector3 v = default(Vector3)) {
		bool Result = false;
		EPlayerState playerState = EPlayerState.Idle;
		try {
			playerState = (EPlayerState)System.Enum.Parse(typeof(EPlayerState), State.ToString());
		} catch {
			playerState = EPlayerState.Idle;
		}
		
		if(player && (GameController.Get.Situation == EGameSituation.AttackGamer || 
		              GameController.Get.Situation == EGameSituation.AttackNPC || 
		              GameController.Get.Situation == EGameSituation.InboundsGamer|| 
		              GameController.Get.Situation == EGameSituation.InboundsNPC||
		              GameController.Get.Situation == EGameSituation.Opening||
		              GameController.Get.Situation == EGameSituation.JumpBall)) {
			switch(State) {
			case ESkillSituation.Block0:
				playerState = getPassiveSkill(ESkillSituation.Block0, ESkillKind.Block0, v);
				Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.Dunk0:
				playerState = getPassiveSkill(ESkillSituation.Dunk0, ESkillKind.Dunk, v);
				Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.Fall1:
				Result = true;
				break;
				
			case ESkillSituation.Fall2:
				Result = true;
				break;

			case ESkillSituation.Elbow0:
				playerState = getPassiveSkill(ESkillSituation.Elbow0, ESkillKind.Elbow0);
				Result = player.AniState (playerState);
				break;
				
			case ESkillSituation.JumpBall:
				playerState = getPassiveSkill(ESkillSituation.JumpBall, ESkillKind.JumpBall);
				Result = player.AniState (playerState);
				break;
				
			case ESkillSituation.KnockDown0:
				playerState = getPassiveSkill(ESkillSituation.KnockDown0, ESkillKind.KnockDown0);
				Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.Layup0:
				playerState = getPassiveSkill(ESkillSituation.Layup0, ESkillKind.Layup);
				Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.MoveDodge:
				if(player.IsBallOwner) {
					int Dir = GameController.Get.HasDefPlayer(player, GameConst.CrossOverDistance, 50);
					if(Dir != 0 && player.IsHaveMoveDodge) {
						if(Random.Range(0, 100) <= player.MoveDodgeRate) {
							Vector3 pos = CourtMgr.Get.ShootPoint [player.Team.GetHashCode()].transform.position;
							//Crossover     
							if(player.Team == ETeamKind.Self && player.transform.position.z >= 9.5)
								return Result;
							else 
								if(player.Team == ETeamKind.Npc && player.transform.position.z <= -9.5)
									return Result;
							
							int AddZ = 6;
							if(player.Team == ETeamKind.Npc)
								AddZ = -6;
							
							player.RotateTo(pos.x, pos.z);
							player.transform.DOMoveZ(player.transform.position.z + AddZ, GameConst.CrossTimeZ).SetEase(Ease.Linear);
							TSkill skill = new TSkill();
							if (Dir == 1) {
								player.transform.DOMoveX(player.transform.position.x - 1, GameConst.CrossTimeX).SetEase(Ease.Linear);
								playerState = EPlayerState.MoveDodge0;
								skill.ID = 1100;
							} else {
								player.transform.DOMoveX(player.transform.position.x + 1, GameConst.CrossTimeX).SetEase(Ease.Linear);
								playerState = EPlayerState.MoveDodge1;
								skill.ID = 1100;
							}			
							skill.Lv = player.MoveDodgeLv;
							player.PassiveSkillUsed = skill;
							
							GameController.Get.CoolDownCrossover = Time.time + 4;
							Result = player.AniState(playerState);
						}
					} 
				}
				break;
				
			case ESkillSituation.PickBall:{
				playerState = EPlayerState.Pick2;
				TSkill skill = new TSkill();
				skill.ID = 1310;
				skill.Lv = player.PickBall2Lv;
				player.PassiveSkillUsed = skill;

				Result = player.AniState(playerState, v);
				break;
			}
				
			case ESkillSituation.Pass4:
				playerState = getPassiveSkill(ESkillSituation.Pass4, ESkillKind.Pass, v);
				if(playerState != EPlayerState.Pass4)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.Pass0:
				playerState = getPassiveSkill(ESkillSituation.Pass0, ESkillKind.Pass, v);
				if(playerState != EPlayerState.Pass0)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);
				
				break;
				
			case ESkillSituation.Pass2:
				playerState = getPassiveSkill(ESkillSituation.Pass2, ESkillKind.Pass, v);
				if(playerState != EPlayerState.Pass2)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);
				
				break;
				
			case ESkillSituation.Pass1:
				playerState = getPassiveSkill(ESkillSituation.Pass1, ESkillKind.Pass, v);
				if(playerState != EPlayerState.Pass1)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);
				
				break;
				
			case ESkillSituation.Push0:
				playerState = getPassiveSkill(ESkillSituation.Push0, ESkillKind.Push);
				if(v == Vector3.zero)
					Result = player.AniState(playerState);
				else
					Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.Shoot0:
				playerState = getPassiveSkill(ESkillSituation.Shoot0, ESkillKind.Shoot, v, GameController.Get.HasDefPlayer(player, 1.5f, 40));
				Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.Shoot3:
				playerState = getPassiveSkill(ESkillSituation.Shoot3, ESkillKind.DownHand, Vector3.zero, GameController.Get.HasDefPlayer(player, 1.5f, 40));
				Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.Shoot2:
				playerState = getPassiveSkill(ESkillSituation.Shoot2, ESkillKind.UpHand, Vector3.zero, GameController.Get.HasDefPlayer(player, 1.5f, 40));
				Result = player.AniState(playerState, v);
				break;
				
			case ESkillSituation.Shoot1:
				playerState = getPassiveSkill(ESkillSituation.Shoot1, ESkillKind.NearShoot, Vector3.zero, GameController.Get.HasDefPlayer(player, 1.5f, 40));
				Result = player.AniState(playerState, v );
				break;

			case ESkillSituation.ShowOwnIn:
				playerState = getPassiveSkill(ESkillSituation.ShowOwnIn, ESkillKind.ShowOwnIn);
				Result = player.AniState(playerState);
				break;

			case ESkillSituation.ShowOwnOut:
				playerState = getPassiveSkill(ESkillSituation.ShowOwnOut, ESkillKind.ShowOwnOut);
				Result = player.AniState(playerState);
				break;
				
			case ESkillSituation.Steal0:	
				playerState = getPassiveSkill(ESkillSituation.Steal0, ESkillKind.Steal);
				Result = player.AniState(playerState, v);
				break;

			case ESkillSituation.Rebound:
				playerState = getPassiveSkill(ESkillSituation.Rebound, ESkillKind.Rebound);
				Result = player.AniState (playerState, v);
				break;
			}	
		}
		try {
			if(Result && !playerState.ToString().Equals(State.ToString())){
				if(GameData.DSkillData.ContainsKey(player.PassiveSkillUsed.ID)) {
					AddSkillAttribute(player, player.PassiveSkillUsed);
					if(!player.IsUseSkill)
						UIPassiveEffect.Get.ShowCard(player, player.PassiveSkillUsed.ID, player.PassiveSkillUsed.Lv);
					SkillEffectManager.Get.OnShowEffect(player, true);
					player.GameRecord.PassiveSkill++;
				}
			}
		} catch {
			Debug.Log(player.name  +" is no State: "+ State.ToString() +" or have no PassiveID:"+ player.PassiveSkillUsed.ID);
		}
		
		return Result;
	}
	
	public List<GameObject> GetActiveSkillTarget(PlayerBehaviour player, TSkill tSkill) {
		if(player.Attribute.ActiveSkills.Count > 0) {
			if (GameData.DSkillData.ContainsKey(tSkill.ID)) {
				string key  =  GameData.DSkillData[tSkill.ID].TargetKind.ToString();
				if(activeSkillTargets.ContainsKey(key)) {
					return activeSkillTargets[key];
				} else {
					List<GameObject> objs = new List<GameObject>();
					switch (GameData.DSkillData[tSkill.ID].TargetKind) {
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
		}
		return null;
	}

}
