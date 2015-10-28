using System.Collections.Generic;
using UnityEngine;

public class TSkillEffect {
	public int SkillID;
	public string EffectName;
	public Vector3 Position;
	public GameObject Player;
	public GameObject Parent;
	public float Duration;
	public float DelayTime;
}

public class SkillEffectManager : KnightSingleton<SkillEffectManager> {

	private Dictionary<string, List<GameObject>> skillEffectPositions = new Dictionary<string, List<GameObject>>();
	private PlayerBehaviour executePlayer;

	private List<TSkillEffect> skillEffects = new List<TSkillEffect>();

	void FixedUpdate() {
		if(skillEffects.Count > 0) {
			for (int i=0; i<skillEffects.Count; i++) {
				if (skillEffects [i].DelayTime > 0) {
					skillEffects [i].DelayTime -= Time.deltaTime * TimerMgr.Get.CrtTime;  
					if (skillEffects [i].DelayTime <= 0) 
						playSkillEffect(i);
				} else
					playSkillEffect(i);
			}
		}
	}

	private void playSkillEffect (int index){
		if(index >= 0 && index < skillEffects.Count) {
			playEffect(skillEffects[index].EffectName,
			           Vector3.zero,
			           skillEffects[index].Player,
			           skillEffects[index].Parent,
			           null,
			           skillEffects[index].Duration);
			skillEffects.RemoveAt(index);
		}
	}

	private bool isInRange (GameObject obj) {
		return Vector2.Distance(new Vector2(GameController.Get.Joysticker.PlayerRefGameObject.transform.position.x, GameController.Get.Joysticker.PlayerRefGameObject.transform.position.z), 
		                        new Vector2(obj.transform.position.x, obj.transform.position.z)) <= GameData.DSkillData[executePlayer.PassiveSkillUsed.ID].Distance(executePlayer.PassiveSkillUsed.Lv);
	}

	public void OnShowEffect (PlayerBehaviour player, bool isPassiveID = true) {
		executePlayer = player;
		int skillID = 0;
		List<GameObject> objs1 = null;
		List<GameObject> objs2 = null;
		List<GameObject> objs3 = null;
		if(isPassiveID) {
			if (GameData.DSkillData.ContainsKey(player.PassiveSkillUsed.ID)) {
				if(GameData.DSkillData[player.PassiveSkillUsed.ID].TargetKind1 != 0) 
					objs1 = getSkillEffectPosition(1, GameData.DSkillData[player.PassiveSkillUsed.ID].TargetKind1, isPassiveID);
				if(GameData.DSkillData[player.PassiveSkillUsed.ID].TargetKind2 != 0)
					objs2 = getSkillEffectPosition(2, GameData.DSkillData[player.PassiveSkillUsed.ID].TargetKind2, isPassiveID);
				if(GameData.DSkillData[player.PassiveSkillUsed.ID].TargetKind3 != 0)
					objs3 = getSkillEffectPosition(3, GameData.DSkillData[player.PassiveSkillUsed.ID].TargetKind3, isPassiveID);
			}
			
			if(player.PassiveSkillUsed.ID != -1) 
				skillID = player.PassiveSkillUsed.ID;
		} else {
			if (GameData.DSkillData.ContainsKey(player.ActiveSkillUsed.ID)) {
				if(GameData.DSkillData[player.ActiveSkillUsed.ID].TargetKind1 != 0)
					objs1 = getSkillEffectPosition(1, GameData.DSkillData[player.ActiveSkillUsed.ID].TargetKind1, isPassiveID);
				if(GameData.DSkillData[player.ActiveSkillUsed.ID].TargetKind2 != 0)
					objs2 = getSkillEffectPosition(2, GameData.DSkillData[player.ActiveSkillUsed.ID].TargetKind2, isPassiveID);
				if(GameData.DSkillData[player.ActiveSkillUsed.ID].TargetKind3 != 0)
					objs3 = getSkillEffectPosition(3, GameData.DSkillData[player.ActiveSkillUsed.ID].TargetKind3, isPassiveID);
			}
			
			if(player.ActiveSkillUsed.ID != 0)
				skillID = player.ActiveSkillUsed.ID;
		}

		if(skillID != 0 && GameData.DSkillData.ContainsKey(skillID)) {
			bool isJudgeDistance = false;
			if(!isPassiveID) {
				if(GameData.DSkillData[skillID].TargetKind1 == 16 || GameData.DSkillData[skillID].TargetKind1 == 17 ||
				   GameData.DSkillData[skillID].TargetKind1 == 18 || GameData.DSkillData[skillID].TargetKind1 == 19)
					isJudgeDistance = true;
				else 
					isJudgeDistance = false;
			}

			if(objs1 != null && objs1.Count != 0){
				for (int i=0; i<objs1.Count; i++) {
					if((isJudgeDistance && isInRange(objs1[i])) || !isJudgeDistance) {
						TSkillEffect skillEffect = new TSkillEffect();
						skillEffect.EffectName = "SkillEffect" + GameData.DSkillData[skillID].TargetEffect1;
						skillEffect.Position = Vector3.zero;
						if(GameData.DSkillData[skillID].EffectParent1 == 1) {
							skillEffect.Player = null;
							skillEffect.Parent = objs1[i];
						} else {
							skillEffect.Player = objs1[i];
							skillEffect.Parent = null;
						}
						skillEffect.Duration = GameData.DSkillData[skillID].Duration1;
						skillEffect.DelayTime = GameData.DSkillData[skillID].DelayTime1;
						skillEffects.Add(skillEffect);
					}
				}
			}

			if(!isPassiveID) {
				if(GameData.DSkillData[skillID].TargetKind2 == 16 || GameData.DSkillData[skillID].TargetKind2 == 17 ||
				   GameData.DSkillData[skillID].TargetKind2 == 18 || GameData.DSkillData[skillID].TargetKind2 == 19)
					isJudgeDistance = true;
				else 
					isJudgeDistance = false;
			}
			
			if(objs2 != null && objs2.Count != 0) {
				for (int i=0; i<objs2.Count; i++) {
					if((isJudgeDistance && isInRange(objs2[i])) || !isJudgeDistance) {
						TSkillEffect skillEffect = new TSkillEffect();
						skillEffect.EffectName = "SkillEffect" + GameData.DSkillData[skillID].TargetEffect2;
						skillEffect.Position = Vector3.zero;
						if(GameData.DSkillData[skillID].EffectParent2 == 1) {
							skillEffect.Player = null;
							skillEffect.Parent = objs2[i];
						} else {
							skillEffect.Player = objs2[i];
							skillEffect.Parent = null;
						}
						skillEffect.Duration = GameData.DSkillData[skillID].Duration2;
						skillEffect.DelayTime = GameData.DSkillData[skillID].DelayTime2;
						skillEffects.Add(skillEffect);
					}
				}
			}

			if(!isPassiveID) {
				if(GameData.DSkillData[skillID].TargetKind3 == 16 || GameData.DSkillData[skillID].TargetKind3 == 17 ||
				   GameData.DSkillData[skillID].TargetKind3 == 18 || GameData.DSkillData[skillID].TargetKind3 == 19)
					isJudgeDistance = true;
				else 
					isJudgeDistance = false;
			}
			
			if(objs3 != null && objs3.Count != 0) {
				for (int i=0; i<objs3.Count; i++) {
					if((isJudgeDistance && isInRange(objs3[i])) || !isJudgeDistance) {
						TSkillEffect skillEffect = new TSkillEffect();
						skillEffect.EffectName = "SkillEffect" + GameData.DSkillData[skillID].TargetEffect3;
						skillEffect.Position = Vector3.zero;
						if(GameData.DSkillData[skillID].EffectParent3 == 1) {
							skillEffect.Player = null;
							skillEffect.Parent = objs3[i];
						} else {
							skillEffect.Player = objs3[i];
							skillEffect.Parent = null;
						}
						skillEffect.Duration = GameData.DSkillData[skillID].Duration3;
						skillEffect.DelayTime = GameData.DSkillData[skillID].DelayTime3;
						skillEffects.Add(skillEffect);
					}
				}
			}
		}
	}

	private void playEffect (string effectName, Vector3 position, GameObject player = null, GameObject parent = null, GameObject followObj = null, float lifeTime = 0) {
		GameObject obj = null;
		if(player == null){
			//local
			obj = EffectManager.Get.PlayEffect(effectName, position, parent, followObj, lifeTime);
			if (obj)
				obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
			else
				Debug.Log("Effect not found " + effectName);
		} else {
			//global
			obj = EffectManager.Get.PlayEffect(effectName, player.transform.position, parent, followObj, lifeTime);
			if (obj)
				obj.transform.rotation = player.transform.rotation;
			else
				Debug.Log("Effect not found " + effectName);
		}
		
		if (effectName == "SkillEffect1700" && obj) {
			PushSkillTrigger ps = obj.GetComponent<PushSkillTrigger>();
			if (ps) {
				ps.pusher = executePlayer;
				if(GameData.DSkillData.ContainsKey(executePlayer.ActiveSkillUsed.ID))
					ps.InRange = GameData.DSkillData[executePlayer.ActiveSkillUsed.ID].Distance(executePlayer.ActiveSkillUsed.Lv);
			}
		}
	}

	private List<GameObject> getSkillEffectPosition (int index, int effectkind, bool isPassive) {
		string key = string.Empty;
		if(isPassive) 
			key = executePlayer.PassiveSkillUsed.ID + "_"+ index + "_" + effectkind;
		else 
			key = executePlayer.ActiveSkillUsed.ID + "_"  + index + "_" + effectkind;
		
		if(skillEffectPositions.ContainsKey (key)) 
			return skillEffectPositions[key];
		
		if(effectkind != 0) {
			List<GameObject> objs = new List<GameObject>();
			switch(effectkind) {
			case 1://Self Body (Chest)
				objs.Add(getPlayerChest(executePlayer));
				break;
			case 2://Self Head
				objs.Add(executePlayer.BodyHeight);
				break;
			case 3://Self Hand
				objs.Add(getPlayerHand(executePlayer));				
				break;
			case 4://Self Feet
				objs.Add(executePlayer.gameObject);
				break;
			case 5://Teammate Body (Chest)
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team == executePlayer.Team && GameController.Get.GamePlayers[i].Index != executePlayer.Index)
						objs.Add(getPlayerChest(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 6://Teammate Head
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team == executePlayer.Team && GameController.Get.GamePlayers[i].Index != executePlayer.Index)
						objs.Add(GameController.Get.GamePlayers[i].BodyHeight);
				} 
				break;
			case 7://Teammate Hand
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team == executePlayer.Team && GameController.Get.GamePlayers[i].Index != executePlayer.Index)
						objs.Add(getPlayerHand(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 8://Teammate Feet
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team == executePlayer.Team && GameController.Get.GamePlayers[i].Index != executePlayer.Index)
						objs.Add(GameController.Get.GamePlayers[i].gameObject);
				} 
				break;
			case 9://Emeny Body (Chest)
			case 16:
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != executePlayer.Team)
						objs.Add(getPlayerChest(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 10://Emeny Head
			case 17:
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != executePlayer.Team)
						objs.Add(GameController.Get.GamePlayers[i].BodyHeight);
				} 
				break;
			case 11://Emeny Hand
			case 18:
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != executePlayer.Team)
						objs.Add(getPlayerHand(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 12://Emeny Feet
			case 19:
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != executePlayer.Team)
						objs.Add(GameController.Get.GamePlayers[i].gameObject);
				} 
				break;
			case 13:
				objs.Add(CourtMgr.Get.ShootPoint[executePlayer.Team.GetHashCode()]);
				break;
			case 14:
				int team = 0;
				if(executePlayer.Team.GetHashCode() != team)
					team = 1;
				objs.Add(CourtMgr.Get.ShootPoint[team]);
				break;
			case 15:
				objs.Add(CourtMgr.Get.RealBall);
				break;
			}
			skillEffectPositions.Add(key, objs);
			
			return skillEffectPositions[key];
		}
		return null;
	}
	
	private GameObject getPlayerChest (PlayerBehaviour player) {
		Transform t = null;
		t = player.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1");
		if(t != null)
			return t.gameObject;
		return null;
	}
	
	private GameObject getPlayerHand (PlayerBehaviour player) {
		Transform t = null;
		t = player.transform.FindChild("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/DummyHand_R");
		if(t != null)
			return t.gameObject;
		return null;
	}
}
