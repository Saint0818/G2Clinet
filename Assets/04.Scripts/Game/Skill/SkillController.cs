using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStruct;
using DG.Tweening;
using G2;
using GameEnum;

public struct TPassiveType {
	public TSkill Tskill;
	public int Rate;
}

public delegate void DoPassiveDelegate(TSkill skill);

public class SkillController : MonoBehaviour {
	public DoPassiveDelegate DoPassive = null;
	private PlayerBehaviour executePlayer;
	public PlayerBehaviour ExecutePlayer {
		set {executePlayer = value;}
	}

	//PassiveSkill key: Kind  value: TSKill
	private int passDirect = EPassDirectState.Forward;
	private Dictionary<string, List<GameObject>> activeSkillTargets = new Dictionary<string, List<GameObject>>(); // Record TargetKind
	public Dictionary<int, List<TPassiveType>> DPassiveSkills = new Dictionary<int, List<TPassiveType>>();//Skill
	public Dictionary<int, List<TPassiveType>> DExtraPassiveSkills = new Dictionary<int, List<TPassiveType>>();//Distance > 0 
	[HideInInspector]public TSkill ActiveSkillUsed;
	[HideInInspector]public TSkill PassiveSkillUsed;
	[HideInInspector]public int MoveDodgeRate = 0;

	//PlayerInfo for Init
	private bool isHavePlayerInfo = false;

    void OnDestroy() {
        activeSkillTargets.Clear();
        DPassiveSkills.Clear();
        DExtraPassiveSkills.Clear();
    }

	void Awake() {
		executePlayer = gameObject.GetComponent<PlayerBehaviour>();
	}

	private bool isExecuteSkillUI(EGameSituation situation) {
		return (situation == EGameSituation.JumpBall || situation == EGameSituation.GamerAttack || situation == EGameSituation.NPCAttack);
	}

	public void SkillUpdate(EGameSituation situation, bool isPlayerMe, PlayerBehaviour player)
	{
		//只在跳球跟進攻防守執行SkillUI
		if(isExecuteSkillUI (situation) && isPlayerMe && player && player.Attribute.ActiveSkills != null && player.Attribute.ActiveSkills.Count > 0 ){
			for(int i=0; i<player.Attribute.ActiveSkills.Count; i++) {
				if(player.Attribute.ActiveSkills[i].ID > 0 && GameController.Get.IsStart)
				{
					UIGame.Get.ShowSkillEnableUI(GameController.Get.IsStart, 
					                             i, 
												 player.IsAngerFull(player.Attribute.ActiveSkills[i]), 
													CanDoSkill(player.Attribute.ActiveSkills[i])
					                             );
				}
			}
		}
	}

	public void initSkillController(PlayerBehaviour player, Animator animatorControl){

		//PlayerInfo
		if(!isHavePlayerInfo) {
			isHavePlayerInfo = true;
			//Passive
			if (player.Attribute.SkillCards != null && player.Attribute.SkillCards.Length > 0) {
				for (int i = 0; i < player.Attribute.SkillCards.Length; i++) {
					if (GameData.DSkillData.ContainsKey(player.Attribute.SkillCards[i].ID) && !GameFunction.IsActiveSkill(player.Attribute.SkillCards[i].ID)) {
						TextureManager.Get.CardTexture(player.Attribute.SkillCards[i].ID);
						TSkillData skillData = GameData.DSkillData[player.Attribute.SkillCards[i].ID];
						int key = skillData.Kind;
						
						if (skillData.Kind == (int)ESkillKind.MoveDodge0){
							MoveDodgeRate = skillData.Rate(player.Attribute.SkillCards[i].Lv);
						}

						TPassiveType type = new TPassiveType();
						TSkill skill = new TSkill();
						skill.ID = player.Attribute.SkillCards [i].ID;
						skill.Lv = player.Attribute.SkillCards [i].Lv;
						type.Tskill = skill;
						type.Rate = GameData.DSkillData[player.Attribute.SkillCards [i].ID].Rate(player.Attribute.SkillCards [i].Lv);
						if (DPassiveSkills.ContainsKey(key))
							DPassiveSkills [key].Add(type);
						else {
							List<TPassiveType> pss = new List<TPassiveType>();
							pss.Add(type);
							DPassiveSkills.Add(key, pss);
						}

						if(!GameFunction.IsActiveSkill(player.Attribute.SkillCards[i].ID)  && GameData.DSkillData[player.Attribute.SkillCards[i].ID].Distance(player.Attribute.SkillCards[i].Lv) > 0) {
							if (DExtraPassiveSkills.ContainsKey(key))
								DExtraPassiveSkills [key].Add(type);
							else {
								List<TPassiveType> pss = new List<TPassiveType>();
								pss.Add(type);
								DExtraPassiveSkills.Add(key, pss);
							}
						}
					}
				}
			}
		}
	}

	//ActiveSkill & PassiveSkill======================================================================================

	public void ResetUsePassive () {
		if(!executePlayer.IsSkillShow)
			PassiveSkillUsed.Reset();

	}
	public void ResetUseActive () {
		if(!executePlayer.IsSkillShow)
			ActiveSkillUsed.Reset();
	}

	public void ResetUseSkill() {
		ResetUsePassive ();
		ResetUseActive () ;
	}

	public bool IsPassiveUse {
		get {return (PassiveSkillUsed.ID > 0);}
	}

	public bool IsActiveUse {
		get {return (ActiveSkillUsed.ID > 0);}
	}

	//PassiveSkill======================================================================================
	//judge Passer to Catcher Angle
	private int judgeDirect(float angle) {
		int directState = EPassDirectState.Forward;
		
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

	private string getPassAnimation (int direct) {
		switch (direct) {
		case 1://Forward
			return "Pass5";
		case 2://Back
			if(Random.Range(0, 2) == 0)
				return "Pass6";
			else
				return "Pass9";
		case 3://Left
			return "Pass7";
		case 4://Right
			return "Pass8";
		}

		return "Pass5";
	}

	
	private EPlayerState getPassiveSkill(ESkillSituation situation, ESkillKind kind, Vector3 v = default(Vector3), int isHaveDefPlayer = 0, float shootDistance = 0) {
		EPlayerState playerState = EPlayerState.Idle;
		try {
			if(kind == ESkillKind.Pick2 || kind == ESkillKind.MoveDodge0 || kind == ESkillKind.ShowOwnIn || kind == ESkillKind.ShowOwnOut)
				playerState = EPlayerState.Idle;
			else
				playerState = (EPlayerState)System.Enum.Parse(typeof(EPlayerState), situation.ToString());
		} catch {
			LogMgr.Get.LogWarning("this situation isn't contain EPlayerState:" + situation.ToString());
		}
		if(IsInbounds && kind == ESkillKind.Pass) {
			playerState = EPlayerState.Pass50;
		}

		if(executePlayer.IsGameAttack || kind == ESkillKind.ShowOwnIn) {
			string animationName = randomPassive(kind, v, isHaveDefPlayer, shootDistance);
			
			if (animationName != string.Empty) {
				try {
					return (EPlayerState)System.Enum.Parse(typeof(EPlayerState), animationName);
				} catch {
					LogMgr.Get.LogError("AnimationName: '" + animationName + "'was not found.");
					return playerState;
				}
			}
		}
		return playerState;
	}
	
	private string randomPassive(ESkillKind kind, Vector3 v = default(Vector3), int isHaveDefPlayer = 0, float shootDistance = 0) {
		int skillKind = (int)kind;
		//Part 1. Get Passive which is choosed. 
		List<TPassiveType> skills = new List<TPassiveType>();
		
		float angle = MathUtils.FindAngle(executePlayer.PlayerRefGameObject.transform, v);
		passDirect = judgeDirect(angle);
		
		if(DPassiveSkills.ContainsKey(skillKind)) {
			for (int i=0; i<DPassiveSkills[skillKind].Count; i++) {
				if(kind == ESkillKind.Pass) {
					//傳球只有一招技能，用方向去判斷動作
					if(UnityEngine.Random.Range(1, 100) <= DPassiveSkills[skillKind][i].Rate) 
						skills.Add(DPassiveSkills[skillKind][i]);
						
				} else 
				if(kind == ESkillKind.UpHand || kind == ESkillKind.DownHand) { 
					if(UnityEngine.Random.Range(1, 100) <= DPassiveSkills[skillKind][i].Rate) {
						skills.Add(DPassiveSkills[skillKind][i]);
					}
				} else 
				if(kind == ESkillKind.Layup) {
					if(shootDistance > GameConst.ShortShootDistance && shootDistance <= GameConst.LayupDistance) {
						if(UnityEngine.Random.Range(1, 100) <= DPassiveSkills[skillKind][i].Rate) {
							skills.Add(DPassiveSkills[skillKind][i]);
						}
					} else if(shootDistance > GameConst.LayupDistance) {
						if(GameData.DSkillData[DPassiveSkills[skillKind][i].Tskill.ID].Distance(DPassiveSkills[skillKind][i].Tskill.Lv) + GameConst.LayupDistance > shootDistance) {
							skills.Add(DPassiveSkills[skillKind][i]);
						}
					}
				} else 
				if(kind == ESkillKind.NearShoot) {
					if(shootDistance <= GameConst.ShortShootDistance) {
						if(UnityEngine.Random.Range(1, 100) <= DPassiveSkills[skillKind][i].Rate){
							if(isHaveDefPlayer != 0 && (DPassiveSkills[skillKind][i].Tskill.ID == 412 || DPassiveSkills[skillKind][i].Tskill.ID == 413))//if no def player, don't use 
								break;

							skills.Add(DPassiveSkills[skillKind][i]);
						}
					} else {
						if(GameData.DSkillData[DPassiveSkills[skillKind][i].Tskill.ID].Distance(DPassiveSkills[skillKind][i].Tskill.Lv) + GameConst.ShortShootDistance > shootDistance) {
							skills.Add(DPassiveSkills[skillKind][i]);
						}
					}
				} else 
				if(kind == ESkillKind.Shoot) {
					if(!executePlayer.IsMoving) {
						if(shootDistance > GameConst.ShortShootDistance && shootDistance <= GameConst.LongShootDistance) {
							if(UnityEngine.Random.Range(1, 100) <= DPassiveSkills[skillKind][i].Rate)
								skills.Add(DPassiveSkills[skillKind][i]);
						}
					}
				}else
				if(kind == ESkillKind.Dunk) {
					if(executePlayer.IsMoving) {
						if (shootDistance <= GameConst.DunkDistance) {
							if(UnityEngine.Random.Range(1, 100) <= DPassiveSkills[skillKind][i].Rate)
								skills.Add(DPassiveSkills[skillKind][i]);
						} else {
							if(GameData.DSkillData[DPassiveSkills[skillKind][i].Tskill.ID].Distance(DPassiveSkills[skillKind][i].Tskill.Lv) + GameConst.DunkDistance > shootDistance) {
								skills.Add(DPassiveSkills[skillKind][i]);
							}
						}
					} else {
						if (shootDistance <= GameConst.DunkDistanceNoMove) {
							if(UnityEngine.Random.Range(1, 100) <= DPassiveSkills[skillKind][i].Rate)
								skills.Add(DPassiveSkills[skillKind][i]);
						} else {
							if(GameData.DSkillData[DPassiveSkills[skillKind][i].Tskill.ID].Distance(DPassiveSkills[skillKind][i].Tskill.Lv) + GameConst.DunkDistanceNoMove > shootDistance) {
								skills.Add(DPassiveSkills[skillKind][i]);
							}
						}
					}
				} else
					if(UnityEngine.Random.Range(1, 100) <= DPassiveSkills[skillKind][i].Rate)
						skills.Add(DPassiveSkills[skillKind][i]);
			}
			//Part 2. Get Passive 
			if(skills.Count > 0) {
				AI.WeightedRandomizer<TPassiveType> randomizer = new AI.WeightedRandomizer<TPassiveType>();
				for(int i=0; i<skills.Count; i++) 
					if(skills[i].Rate > 0 )
						randomizer.AddOrUpdate(skills[i], skills[i].Rate);

				if(!randomizer.IsEmpty()) {
					TPassiveType type = randomizer.GetNext();
					PassiveSkillUsed = type.Tskill;
					if(kind == ESkillKind.Pass)
						return getPassAnimation(passDirect);
					else
						return GameData.DSkillData[type.Tskill.ID].Animation;
				} else 
					return string.Empty;
			} else 
				return string.Empty;
		} else 
			return string.Empty;
	}

	public bool DoPassiveSkill(ESkillSituation state, Vector3 v = default(Vector3), float shootDistance = 0) {
		bool Result = false;
		EPlayerState playerState = EPlayerState.Idle;
		switch(state) {
		case ESkillSituation.Block0:
			playerState = getPassiveSkill(ESkillSituation.Block0, ESkillKind.Block0, v);
			Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.Dunk0:
			playerState = getPassiveSkill(ESkillSituation.Dunk0, ESkillKind.Dunk, v, 0, shootDistance);
			Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.Fall1:
			playerState = getPassiveSkill(ESkillSituation.Fall1, ESkillKind.Fall1);
			Result = executePlayer.AniState(playerState);
			break;

		case ESkillSituation.Elbow0:
			playerState = getPassiveSkill(ESkillSituation.Elbow0, ESkillKind.Elbow0);
			Result = executePlayer.AniState (playerState);
			break;
			
		case ESkillSituation.JumpBall:
			playerState = getPassiveSkill(ESkillSituation.JumpBall, ESkillKind.JumpBall);
			Result = executePlayer.AniState (playerState);
			break;
			
		case ESkillSituation.KnockDown0:
			playerState = getPassiveSkill(ESkillSituation.KnockDown0, ESkillKind.KnockDown0);
			Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.Layup0:
			playerState = getPassiveSkill(ESkillSituation.Layup0, ESkillKind.Layup, Vector3.zero, 0, shootDistance);
			Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.MoveDodge:
			if(executePlayer.IsBallOwner) {
				int Dir = GameController.Get.HasDefPlayer(executePlayer, GameConst.CrossOverDistance, 50);
				if(Dir != 0 && IsHaveMoveDodge) {
					playerState = getPassiveSkill(ESkillSituation.MoveDodge, ESkillKind.MoveDodge0 , v);
					if(playerState != EPlayerState.Idle){
						Vector3 pos = CourtMgr.Get.ShootPoint [executePlayer.Team.GetHashCode()].transform.position;
						//Crossover 這裡是判斷某個範圍內才可以做動作    
						if(executePlayer.Team == ETeamKind.Self && executePlayer.transform.position.z >= 9.5)
							return Result;
						else 
							if(executePlayer.Team == ETeamKind.Npc && executePlayer.transform.position.z <= -9.5)
								return Result;
						
						int AddZ = 6;
						if(executePlayer.Team == ETeamKind.Npc)
							AddZ = -6;
						
						executePlayer.RotateTo(pos.x, pos.z);
						executePlayer.transform.DOMoveZ(executePlayer.transform.position.z + AddZ, GameConst.CrossTimeZ).SetEase(Ease.Linear);
						//Dir = 1: 有找到, 防守球員在前方; 2: 有找到, 防守球員在後方.
						if (Dir == 1) {
							executePlayer.transform.DOMoveX(executePlayer.transform.position.x - 1, GameConst.CrossTimeX).SetEase(Ease.Linear);
							playerState = EPlayerState.MoveDodge0;
						} else {
							executePlayer.transform.DOMoveX(executePlayer.transform.position.x + 1, GameConst.CrossTimeX).SetEase(Ease.Linear);
							playerState = EPlayerState.MoveDodge1;
						}			
						executePlayer.CoolDownCrossover = 4;
						Result = executePlayer.AniState(playerState);
					}
				} 
			}
			break;
			
		case ESkillSituation.Pick0:{
			playerState = getPassiveSkill(ESkillSituation.Pick0, ESkillKind.Pick2, v);
			//被防守範圍影響，因為有可能會沒有觸發，用idle判斷說有沒有執行，主要是沒有初始動作
			if(playerState == EPlayerState.Idle)
				Result = false;
			else
				Result = executePlayer.AniState(playerState, v);
			break;
		}
			
		case ESkillSituation.Pass5:
			playerState = getPassiveSkill(ESkillSituation.Pass5, ESkillKind.Pass, v);
			if(playerState != EPlayerState.Pass5)
				Result = executePlayer.AniState(playerState);
			else
				Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.Pass0:
			playerState = getPassiveSkill(ESkillSituation.Pass0, ESkillKind.Pass, v);
			if(playerState != EPlayerState.Pass0)
				Result = executePlayer.AniState(playerState);
			else
				Result = executePlayer.AniState(playerState, v);
			
			break;
			
		case ESkillSituation.Pass2:
			playerState = getPassiveSkill(ESkillSituation.Pass2, ESkillKind.Pass, v);
			if(playerState != EPlayerState.Pass2)
				Result = executePlayer.AniState(playerState);
			else
				Result = executePlayer.AniState(playerState, v);
			
			break;
			
		case ESkillSituation.Pass1:
			playerState = getPassiveSkill(ESkillSituation.Pass1, ESkillKind.Pass, v);
			if(playerState != EPlayerState.Pass1)
				Result = executePlayer.AniState(playerState);
			else
				Result = executePlayer.AniState(playerState, v);
			
			break;
			
		case ESkillSituation.Push0:
			playerState = getPassiveSkill(ESkillSituation.Push0, ESkillKind.Push);
			if(v == Vector3.zero)
				Result = executePlayer.AniState(playerState);
			else
				Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.Shoot0:
			playerState = getPassiveSkill(ESkillSituation.Shoot0, ESkillKind.Shoot, v, GameController.Get.HasDefPlayer(executePlayer, 1.5f, 40), shootDistance);
			Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.Shoot3:
			playerState = getPassiveSkill(ESkillSituation.Shoot3, ESkillKind.DownHand, Vector3.zero, GameController.Get.HasDefPlayer(executePlayer, 1.5f, 40), shootDistance);
			Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.Shoot2:
			playerState = getPassiveSkill(ESkillSituation.Shoot2, ESkillKind.UpHand, Vector3.zero, GameController.Get.HasDefPlayer(executePlayer, 1.5f, 40), shootDistance);
			Result = executePlayer.AniState(playerState, v);
			break;
			
		case ESkillSituation.Shoot1:
			playerState = getPassiveSkill(ESkillSituation.Shoot1, ESkillKind.NearShoot, Vector3.zero, GameController.Get.HasDefPlayer(executePlayer, 1.5f, 40), shootDistance);
			Result = executePlayer.AniState(playerState, v );
			break;

		case ESkillSituation.ShowOwnOut:
			playerState = getPassiveSkill(ESkillSituation.ShowOwnOut, ESkillKind.ShowOwnOut);
			if(playerState == EPlayerState.Idle)
				Result = false;
			else {
				executePlayer.AniState(playerState);
				Result = true; //沒做動作也可以觸發
			}
			break;

		case ESkillSituation.Steal0:	
			playerState = getPassiveSkill(ESkillSituation.Steal0, ESkillKind.Steal);
			Result = executePlayer.AniState(playerState, v);
			break;

		case ESkillSituation.Rebound0:
			playerState = getPassiveSkill(ESkillSituation.Rebound0, ESkillKind.Rebound);
			Result = executePlayer.AniState (playerState, v);
			break;
		}	

		if(GameController.Get.Situation == EGameSituation.SpecialAction) {
			if(state == ESkillSituation.ShowOwnIn) {
				playerState = getPassiveSkill(ESkillSituation.ShowOwnIn, ESkillKind.ShowOwnIn);
				if(playerState == EPlayerState.Idle)
					Result = false;
				else {
					executePlayer.AniState(playerState);
					Result = true; //沒做動作也可以觸發
				}
			}
		}

		try {
			if(Result && !playerState.ToString().Equals(state.ToString())){
				if(GameData.DSkillData.ContainsKey(PassiveSkillUsed.ID)) {
					if(DoPassive != null)
						DoPassive(PassiveSkillUsed);
				}
			}
		} catch {
			Debug.Log(executePlayer.name  + " is no State: " + state.ToString() +" or have no PassiveID:"+ PassiveSkillUsed.ID);
		}

		return Result;
	}

	//Active======================================================================================
	private bool checkSkillBaseSituation(TSkill tSkill) {
		if(executePlayer.Attribute.IsHaveActiveSkill && GameData.DSkillData.ContainsKey(tSkill.ID) && !IsActiveUse) {
			int kind = GameData.DSkillData[tSkill.ID].Kind;

			if( kind == 130 || kind == 140 || kind == 190 || kind == 200 || kind == 210 || kind == 220 || kind == 230 || kind == 300 || kind == 310)
				return  true;
			
			switch (GameController.Get.Situation) {
			case EGameSituation.GamerAttack:
				if(executePlayer.Team == ETeamKind.Self) {
					if (kind >= 10 && kind <= 70 && executePlayer.IsBallOwner) return true;
					if ((kind == 110 || kind == 180) && executePlayer.IsBallOwner) return true;
					if ((kind == 170 || kind == 171) && !executePlayer.IsBallOwner) return true;
					if (kind == 120 ) return true;
				} else {
					if (kind == 160 || kind == 161) return true;
					if (kind == 150 && !executePlayer.IsBallOwner && GameController.Get.CanUseStealSkill) return true;
					if (kind == 170 && !executePlayer.IsBallOwner) return true;
				}
				break;
			case EGameSituation.NPCAttack:
				if(executePlayer.Team == ETeamKind.Self){
					if (kind == 160 || kind == 161) return true;
					if (kind == 150 && !executePlayer.IsBallOwner && GameController.Get.CanUseStealSkill) return true;
					if ((kind == 170 || kind == 171) && !executePlayer.IsBallOwner) return true;
				} else  {
					if (kind >= 10 && kind <= 70 && executePlayer.IsBallOwner) return true;
					if ((kind == 110 || kind == 180) && executePlayer.IsBallOwner) return true;
					if (kind == 170 && !executePlayer.IsBallOwner) return true;
					if (kind == 120) return true;
				}
				break;
			}
		}
		return false;
	}

	private bool checkSkillDistance(TSkill tSkill, GameObject target = null) {
		if (executePlayer.CanUseActiveSkill(tSkill) && executePlayer.Attribute.IsHaveActiveSkill && tSkill.ID > 0 && GameData.DSkillData.ContainsKey(tSkill.ID)) {
			if (target) {
				if(GameData.DSkillData[tSkill.ID].TargetKind != 1 && GameData.DSkillData[tSkill.ID].TargetKind != 2) {
					//Target(People)
					if (target == executePlayer.PlayerRefGameObject || GameController.Get.GetDis(executePlayer, new Vector2(target.transform.position.x, target.transform.position.z)) <= 
					    GameData.DSkillData[tSkill.ID].Distance(tSkill.Lv)) {
							if (checkSkillBaseSituation(tSkill))
								return true;
						}
				} else{
					//Basket
					if (target == executePlayer.PlayerRefGameObject || GameController.Get.GetDis(executePlayer, new Vector2(CourtMgr.Get.ShootPoint [executePlayer.Team.GetHashCode()].transform.position.x, CourtMgr.Get.ShootPoint [executePlayer.Team.GetHashCode()].transform.position.z)) <= 
					    GameData.DSkillData[tSkill.ID].Distance(tSkill.Lv)){
						if (checkSkillBaseSituation(tSkill))
							return true;
					}
				}
			} else
				return true;
		}
		return false;
	}

	private bool checkSkillKind (TSkill tSkill) {
		if(GameData.DSkillData.ContainsKey(tSkill.ID)) {
			int kind = GameData.DSkillData[tSkill.ID].Kind;
			switch (kind) {
			case 140://Rebound
				if(GameController.Get.BallState == EBallState.CanRebound) return true;
				break;
			case 150://Steal
				if(GameController.Get.Situation == EGameSituation.NPCAttack && GameController.Get.BallState == EBallState.CanSteal) return true;
				break;
			case 160://Block
				if(GameController.Get.Situation == EGameSituation.NPCAttack && GameController.Get.BallState == EBallState.CanBlock) return true;
				break;
			case 161://灌籃剋星
				if(GameController.Get.Situation == EGameSituation.NPCAttack && GameController.Get.BallState == EBallState.CanDunkBlock) return true;
				break;
			default:
				return true;
			}
		}
		
		return false;
	}

	private List<GameObject> getActiveSkillTarget(TSkill tSkill) {
		if(executePlayer.Attribute.IsHaveActiveSkill) {
			if (GameData.DSkillData.ContainsKey(tSkill.ID)) {
				string key  =  GameData.DSkillData[tSkill.ID].TargetKind.ToString();
				if(activeSkillTargets.ContainsKey(key)) {
					return activeSkillTargets[key];
				} else {
					List<GameObject> objs = new List<GameObject>();
					switch (GameData.DSkillData[tSkill.ID].TargetKind) {
					case 0:// self
						objs.Add(executePlayer.PlayerRefGameObject);
						break;
					case 1://my basket
						objs.Add(CourtMgr.Get.BasketHoop[executePlayer.Team.GetHashCode()].gameObject);
						break;
					case 2:{//enemy basket
						int i = 1;
						if (executePlayer.Team == ETeamKind.Npc)
							i = 0;
						
						objs.Add(CourtMgr.Get.BasketHoop[i].gameObject);
						break;
					}
					case 3://my all teammates
						for (int i = 0; i < GameController.Get.GamePlayers.Count; i++) {
							if (GameController.Get.GamePlayers[i].Team == executePlayer.Team) {
								objs.Add(GameController.Get.GamePlayers[i].PlayerRefGameObject);
							}
						}
						break;
					case 5://all opponent
						for (int i = 0; i < GameController.Get.GamePlayers.Count; i++) {
							if (GameController.Get.GamePlayers[i].Team != executePlayer.Team) {
								objs.Add(GameController.Get.GamePlayers[i].PlayerRefGameObject);
							}
						}
						break;
					case 10://ball
                        objs.Add(CourtMgr.Get.RealBall.gameObject);
						break;
					}
					activeSkillTargets.Add(key , objs);
					return activeSkillTargets[key];
				}
			}
		}
		return null;
	}

	private bool checkTargetDistance (TSkill tSkill) {
		bool result = false;
		if (getActiveSkillTarget(tSkill) != null && getActiveSkillTarget(tSkill).Count > 0)
			for (int i = 0; i < getActiveSkillTarget(tSkill).Count; i++)
				if (checkSkillDistance(tSkill, getActiveSkillTarget(tSkill)[i]))
					result = true;

		return result;
	}

	public bool CanDoSkill (TSkill tSkill) {
		return (checkSkillBaseSituation(tSkill) && checkTargetDistance(tSkill) && checkSkillKind(tSkill));
	}

	public bool IsHaveMoveDodge
	{
		get { return DPassiveSkills.ContainsKey((int)ESkillKind.MoveDodge0);}
	}

	public bool IsHavePickBall2
	{
		get { return DPassiveSkills.ContainsKey((int)ESkillKind.Pick2);}
	}

	public bool IsInbounds {
		get {return (executePlayer.situation == EGameSituation.GamerInbounds || executePlayer.situation == EGameSituation.NPCInbounds);}
	}
}
