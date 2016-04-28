using System.Collections.Generic;
using UnityEngine;

public class TSkillEffect {
	public PlayerBehaviour SelfPlayer;
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

	private List<TSkillEffect> skillEffects = new List<TSkillEffect>();
	private List<GameObject>[] objs = new List<GameObject>[3];

	private bool isJudgeDistance;

    void OnDestroy() {
		foreach (KeyValuePair<string, List<GameObject>> value in skillEffectPositions) 
			for(int i=0; i<value.Value.Count; i++) 
				Destroy(value.Value[i]);
			
        skillEffectPositions.Clear();
        skillEffects.Clear();
		for(int i=0; i<objs.Length; i++) 
			if(objs[i] != null) 
				objs[i].Clear();
			
    }

	void FixedUpdate() {
		if(skillEffects.Count > 0) {
			for (int i=0; i<skillEffects.Count; i++) {
				if (skillEffects [i].DelayTime > 0) {
					skillEffects [i].DelayTime -= Time.deltaTime * skillEffects [i].SelfPlayer.timeScale;  
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

	private bool isInRange (GameObject obj, PlayerBehaviour player) {
		return Vector2.Distance(new Vector2(GameController.Get.Joysticker.PlayerRefGameObject.transform.position.x, GameController.Get.Joysticker.PlayerRefGameObject.transform.position.z), 
			new Vector2(obj.transform.position.x, obj.transform.position.z)) <= GameData.DSkillData[player.ActiveSkillUsed.ID].Distance(player.ActiveSkillUsed.Lv);
	}

	private bool IsJudgeDistance (int[] targetkind) {
		for(int i=0; i<targetkind.Length; i++)
			if(targetkind[i] == 16 || targetkind[i] == 17 || targetkind[i] == 18 || targetkind[i] == 19)
				return true;

		return false;
	}

	public void OnShowEffect (PlayerBehaviour player, bool isPassiveID = true) {
		int skillID = 0;

		if(isPassiveID) {
			if(GameData.DSkillData.ContainsKey(player.PassiveSkillUsed.ID) && player.PlayerSkillController.IsPassiveUse) 
				skillID = player.PassiveSkillUsed.ID;
		} else {
			if(GameData.DSkillData.ContainsKey(player.ActiveSkillUsed.ID) && player.PlayerSkillController.IsActiveUse)
				skillID = player.ActiveSkillUsed.ID;
		}

		if(skillID > 0) {
			for(int i=0; i<objs.Length; i++) {
				objs[i] = getSkillEffectPosition(player, i, GameData.DSkillData[skillID].TargetKinds[i], isPassiveID);
					
				isJudgeDistance = false;
				if(!isPassiveID) 
					isJudgeDistance = IsJudgeDistance(GameData.DSkillData[skillID].TargetKinds);
				
				if(objs[i] != null && objs[i].Count != 0){
					for (int j=0; j<objs[i].Count; j++) {
						if((isJudgeDistance && isInRange(objs[i][j], player)) || !isJudgeDistance) {
							TSkillEffect skillEffect = new TSkillEffect();
							skillEffect.SelfPlayer = player;
							skillEffect.EffectName = "SkillEffect" + GameData.DSkillData[skillID].TargetEffects[i];
							skillEffect.Position = Vector3.zero;
							if(GameData.DSkillData[skillID].EffectParents[i] == 1) {
								skillEffect.Player = null;
								skillEffect.Parent = objs[i][j];
							} else {
								skillEffect.Player = objs[i][j];
								skillEffect.Parent = null;
							}
							skillEffect.Duration = GameData.DSkillData[skillID].Durations[i];
							skillEffect.DelayTime = GameData.DSkillData[skillID].DelayTimes[i];
							skillEffects.Add(skillEffect);
						}
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
	}

	private List<GameObject> getSkillEffectPosition (PlayerBehaviour player, int index, int effectkind, bool isPassive) {
		string key = string.Empty;
		if(isPassive) 
			key = player.TimerKind.GetHashCode() + "_"+ player.PassiveSkillUsed.ID + "_"+ index + "_" + effectkind;
		else 
			key = player.TimerKind.GetHashCode() + "_"+ player.ActiveSkillUsed.ID + "_"  + index + "_" + effectkind;
		
		if(skillEffectPositions.ContainsKey (key)) 
			return skillEffectPositions[key];
		
		if(effectkind != 0) {
			List<GameObject> objs = new List<GameObject>();
			switch(effectkind) {
			case 1://Self Body (Chest)
				objs.Add(getPlayerChest(player));
				break;
			case 2://Self Head
				objs.Add(player.BodyHeight);
				break;
			case 3://Self Hand
				objs.Add(getPlayerHand(player));				
				break;
			case 4://Self Feet
				objs.Add(player.PlayerRefGameObject);
				break;
			case 5://Teammate Body (Chest)
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team == player.Team && GameController.Get.GamePlayers[i].Index != player.Index)
						objs.Add(getPlayerChest(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 6://Teammate Head
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team == player.Team && GameController.Get.GamePlayers[i].Index != player.Index)
						objs.Add(GameController.Get.GamePlayers[i].BodyHeight);
				} 
				break;
			case 7://Teammate Hand
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team == player.Team && GameController.Get.GamePlayers[i].Index != player.Index)
						objs.Add(getPlayerHand(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 8://Teammate Feet
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team == player.Team && GameController.Get.GamePlayers[i].Index != player.Index)
						objs.Add(GameController.Get.GamePlayers[i].PlayerRefGameObject);
				} 
				break;
			case 9://Emeny Body (Chest)
			case 16:
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != player.Team)
						objs.Add(getPlayerChest(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 10://Emeny Head
			case 17:
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != player.Team)
						objs.Add(GameController.Get.GamePlayers[i].BodyHeight);
				} 
				break;
			case 11://Emeny Hand
			case 18:
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != player.Team)
						objs.Add(getPlayerHand(GameController.Get.GamePlayers[i]));
				} 
				break;
			case 12://Emeny Feet
			case 19:
				for(int i=0; i<GameController.Get.GamePlayers.Count; i++) {
					if(GameController.Get.GamePlayers[i].Team != player.Team)
						objs.Add(GameController.Get.GamePlayers[i].PlayerRefGameObject);
				} 
				break;
			case 13:
				objs.Add(CourtMgr.Get.ShootPoint[player.Team.GetHashCode()]);
				break;
			case 14:
				int team = 0;
				if(player.Team.GetHashCode() != team)
					team = 1;
				objs.Add(CourtMgr.Get.ShootPoint[team]);
				break;
			case 15:
                objs.Add(CourtMgr.Get.RealBall.gameObject);
				break;
			}
			skillEffectPositions.Add(key, objs);

			return skillEffectPositions[key];
		}
		return null;
	}
	
	private GameObject getPlayerChest (PlayerBehaviour player) {
		Transform t = null;
		t = player.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1");
		if(t != null)
			return t.gameObject;
		return null;
	}
	
	private GameObject getPlayerHand (PlayerBehaviour player) {
		Transform t = null;
		t = player.transform.Find("Bip01/Bip01 Spine/Bip01 Spine1/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 R Finger2");
		if(t != null)
			return t.gameObject;
		return null;
	}
}
