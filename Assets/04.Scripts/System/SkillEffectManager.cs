using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
					if (skillEffects [i].DelayTime <= 0) {
						Debug.Log("EffectName:"+skillEffects[i].EffectName);
						playEffect(skillEffects[i].EffectName,
						           Vector3.zero,
						           skillEffects[i].Player,
						           skillEffects[i].Parent,
						           null,
						           skillEffects[i].Duration);
						skillEffects.RemoveAt(i);
					}
				}
			}
		}
	}

	public void OnShowEffect (PlayerBehaviour player, bool isPassiveID = true) {
		executePlayer = player;
		int skillID = 0;
		List<GameObject> objs1 = null;
		List<GameObject> objs2 = null;
		List<GameObject> objs3 = null;
		if(isPassiveID) {
			if (GameData.DSkillData.ContainsKey(player.PassiveID)) {
				if(GameData.DSkillData[player.PassiveID].TargetKind1 != 0) 
					objs1 = getSkillEffectPosition(1, GameData.DSkillData[player.PassiveID].TargetKind1, isPassiveID);
				if(GameData.DSkillData[player.PassiveID].TargetKind2 != 0)
					objs2 = getSkillEffectPosition(2, GameData.DSkillData[player.PassiveID].TargetKind2, isPassiveID);
				if(GameData.DSkillData[player.PassiveID].TargetKind3 != 0)
					objs3 = getSkillEffectPosition(3, GameData.DSkillData[player.PassiveID].TargetKind3, isPassiveID);
			}
		} else {
			if (GameData.DSkillData.ContainsKey(player.Attribute.ActiveSkill.ID)) {
				if(GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind1 != 0)
					objs1 = getSkillEffectPosition(1, GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind1, isPassiveID);
				if(GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind2 != 0)
					objs2 = getSkillEffectPosition(2, GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind2, isPassiveID);
				if(GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind3 != 0)
					objs3 = getSkillEffectPosition(3, GameData.DSkillData[player.Attribute.ActiveSkill.ID].TargetKind3, isPassiveID);
			}
		}

		if(isPassiveID) {
			if(player.PassiveID != -1) 
				skillID = player.PassiveID;
		} else {
			if(player.Attribute.ActiveSkill.ID != 0)
				skillID = player.Attribute.ActiveSkill.ID;
		}
		
		if(skillID != 0) {
			if(objs1 != null && objs1.Count != 0){
				int index = 0;
				for (int i=0; i<objs1.Count; i++) {
					GameObject parent = null;
					if(GameData.DSkillData[skillID].EffectParent1 == 1) {
						parent = objs1[i];
						index = i;
					}
					TSkillEffect skillEffect = new TSkillEffect();
					skillEffect.EffectName = "SkillEffect" + GameData.DSkillData[skillID].TargetEffect1;
					skillEffect.Position = Vector3.zero;
					skillEffect.Player = objs1[index];
					skillEffect.Parent = parent;
					skillEffect.Duration = GameData.DSkillData[skillID].Duration1;
					skillEffect.DelayTime = GameData.DSkillData[skillID].DelayTime1;
					skillEffects.Add(skillEffect);
				}
			}
			
			if(objs2 != null && objs2.Count != 0) {
				int index = 0;
				for (int i=0; i<objs2.Count; i++) {
					GameObject parent = null;
					if(GameData.DSkillData[skillID].EffectParent2 == 1) {
						parent = objs2[i];
						index = i;
					}
					TSkillEffect skillEffect = new TSkillEffect();
					skillEffect.EffectName = "SkillEffect" + GameData.DSkillData[skillID].TargetEffect2;
					skillEffect.Position = Vector3.zero;
					skillEffect.Player = objs2[index];
					skillEffect.Parent = parent;
					skillEffect.Duration = GameData.DSkillData[skillID].Duration2;
					skillEffect.DelayTime = GameData.DSkillData[skillID].DelayTime2;
					skillEffects.Add(skillEffect);
				}
			}
			
			if(objs3 != null && objs3.Count != 0) {
				int index = 0;
				for (int i=0; i<objs3.Count; i++) {
					GameObject parent = null;
					if(GameData.DSkillData[skillID].EffectParent3 == 1) {
						parent = objs3[i];
						index = i;
					}
					TSkillEffect skillEffect = new TSkillEffect();
					skillEffect.EffectName = "SkillEffect" + GameData.DSkillData[skillID].TargetEffect3;
					skillEffect.Position = Vector3.zero;
					skillEffect.Player = objs3[index];
					skillEffect.Parent = parent;
					skillEffect.Duration = GameData.DSkillData[skillID].Duration3;
					skillEffect.DelayTime = GameData.DSkillData[skillID].DelayTime3;
					skillEffects.Add(skillEffect);
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
				ps.InRange = GameData.DSkillData[executePlayer.Attribute.ActiveSkill.ID].Distance(executePlayer.Attribute.ActiveSkill.Lv);
			}
		}
	}

	private List<GameObject> getSkillEffectPosition (int index, int effectkind, bool isPassive) {
		string key = string.Empty;
		if(isPassive) 
			key = executePlayer.PassiveID + "_"+ index + "_" + effectkind;
		else 
			key = executePlayer.Attribute.ActiveSkill.ID + "_"  + index + "_" + effectkind;
		
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
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != executePlayer.Team)
						objs.Add(getPlayerChest(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 10://Emeny Head
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != executePlayer.Team)
						objs.Add(GameController.Get.GamePlayers[i].BodyHeight);
				} 
				break;
			case 11://Emeny Hand
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != executePlayer.Team)
						objs.Add(getPlayerHand(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 12://Emeny Feet
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
