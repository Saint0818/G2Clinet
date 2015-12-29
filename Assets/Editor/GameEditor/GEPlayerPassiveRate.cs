using UnityEngine;
using UnityEditor;
using GameStruct;
using System.Collections;
using System.Collections.Generic;

public class TChangePassiveRate {
	public int Kind;
	public int ID;
	public int Lv;
	public int Rate;
	public TChangePassiveRate (){
		Kind = 0;
		ID = 0;
		Lv = 0;
		Rate = 0;
	}
}

public class GEPlayerPassiveRate : GEBase {
	private bool isChoose = false;
	private bool isChange = false;
	private Dictionary<int, List<TPassiveType>> UpdatePassiveSkills = new Dictionary<int, List<TPassiveType>>();//Skill
	private Dictionary<int, List<TPassiveType>> UpdateDExtraPassiveSkills = new Dictionary<int, List<TPassiveType>>();//Skill
	private List<TChangePassiveRate> passives = new List<TChangePassiveRate>();
	private SkillController skillController;

	private bool isInstall = false;
	private string chooseID = "1";
	private string[] options;
	private int index = 0;
	private int chooseIndex = 0;
	
	private Vector2 scrollPosition = Vector2.zero;

	void OnEnable () {
		if(GameController.Get.GamePlayers.Count > 0 ){
			isChoose = true;
			GameController.Get.GamePlayers[0].Attribute.SetID(1);
			GameController.Get.GamePlayers[0].InitAttr();
			skillController = GameController.Get.GamePlayers[0].GetComponent<SkillController>();
			if(skillController != null) {
				options = new string[skillController.DPassiveSkills.Count];
				foreach (KeyValuePair<int, List<TPassiveType>> types in skillController.DPassiveSkills) {
					options[index] = types.Key.ToString();
					index ++;
					for (int i=0; i<types.Value.Count; i++){
						TChangePassiveRate change = new TChangePassiveRate();
						change.Kind  = types.Key;
						change.ID = types.Value[i].Tskill.ID;
						change.Lv = types.Value[i].Tskill.Lv;
						change.Rate = types.Value[i].Rate;
						passives.Add(change);
					}
				}
			} else 
				isChoose = false;
		}
	}

//	void OnFocus(){
//		isChange = false;
//		isInstall = false;
//		index = 0;
//		passives.Clear();
//		if(Selection.gameObjects.Length == 1) {
//			isChoose = true;
//			skillController = Selection.gameObjects[0].GetComponent<SkillController>();
//			if(skillController != null) {
//				options = new string[skillController.DPassiveSkills.Count];
//				foreach (KeyValuePair<int, List<TPassiveType>> types in skillController.DPassiveSkills) {
//					options[index] = types.Key.ToString();
//					index ++;
//					for (int i=0; i<types.Value.Count; i++){
//						TChangePassiveRate change = new TChangePassiveRate();
//						change.Kind  = types.Key;
//						change.ID = types.Value[i].Tskill.ID;
//						change.Lv = types.Value[i].Tskill.Lv;
//						change.Rate = types.Value[i].Rate;
//						passives.Add(change);
//					}
//				}
//			} else 
//				isChoose = false;
//		} else
//			isChoose = false;
//	}

	void OnGUI(){
		if(isChoose) {
			if(options != null)
				chooseIndex = EditorGUI.Popup(new Rect(0, 0, 200, 20), "Kind:", chooseIndex, options);
			GUILayout.Label(" ");
			GUILayout.Label(" ");
			chooseID = GUILayout.TextField(chooseID, 10);
			if(GUILayout.Button("Change normal Rate")) {
				GameController.Get.GamePlayers[0].Attribute.SetID(int.Parse(chooseID));
				GameController.Get.GamePlayers[0].InitAttr();
			}
			if(!isInstall && passives.Count == 0) {
				if(GUILayout.Button("Install All Passive")) {
					foreach(KeyValuePair<int, TSkillData> tskill in GameData.DSkillData) {
						if (tskill.Key > 100 && !GameFunction.IsActiveSkill(tskill.Key)) {
							
							TPassiveType type = new TPassiveType();
							TSkill skill = new TSkill();
							skill.ID = tskill.Value.ID;
							skill.Lv = 2;
							type.Tskill = skill;
							type.Rate = 0;
							if (UpdatePassiveSkills.ContainsKey(tskill.Value.Kind))
								UpdatePassiveSkills [tskill.Value.Kind].Add(type);
							else {
								List<TPassiveType> pss = new List<TPassiveType>();
								pss.Add(type);
								UpdatePassiveSkills.Add(tskill.Value.Kind, pss);
							}

							if(!GameFunction.IsActiveSkill(tskill.Value.ID) && GameData.DSkillData[tskill.Value.ID].Distance(2) > 0) {
								if (UpdateDExtraPassiveSkills.ContainsKey(tskill.Value.Kind))
									UpdateDExtraPassiveSkills [tskill.Value.Kind].Add(type);
								else {
									List<TPassiveType> pss = new List<TPassiveType>();
									pss.Add(type);
									UpdateDExtraPassiveSkills.Add(tskill.Value.Kind, pss);
								}
							}
						}
					}
					skillController.DPassiveSkills = UpdatePassiveSkills;
					skillController.DExtraPassiveSkills = UpdateDExtraPassiveSkills;
					isInstall = true;
					OnEnable();
				}
			}

//			scrollPosition = GUI.BeginScrollView(new Rect(0, 60, 600, 450), scrollPosition, new Rect(0, 60, 560, (passives.Count * 50)));
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			for(int i=0; i<passives.Count; i++) {
				if(int.Parse(options[chooseIndex]) == passives[i].Kind) {
					GUILayout.Label("ID:" + passives[i].ID);
					GUILayout.Label("Name:" + GameData.DSkillData[passives[i].ID].Name);
					GUILayout.Label("AnimationName:" + GameData.DSkillData[passives[i].ID].Animation);
					GUILayout.Label("Explain:" + GameFunction.GetStringExplain(GameData.DSkillData[passives[i].ID].Explain, passives[i].ID, passives[i].Lv));
//					GUILayout.Label("Explain:" + string.Format(GameData.DSkillData[passives[i].ID].Explain, 
//					                                           GameData.DSkillData[passives[i].ID].MaxAnger,
//					                                           GameData.DSkillData[passives[i].ID].AniRate(passives[i].Lv),
//					                                           GameData.DSkillData[passives[i].ID].Distance(passives[i].Lv),
//					                                           GameData.DSkillData[passives[i].ID].AttrKind,
//					                                           GameData.DSkillData[passives[i].ID].Value(passives[i].Lv),
//					                                           GameData.DSkillData[passives[i].ID].LifeTime(passives[i].Lv)));
					GUILayout.Label("passives[i].Rate:" + passives[i].Rate);
					passives[i].Rate = Mathf.RoundToInt(GUILayout.HorizontalSlider((float) passives[i].Rate , -1, 100));
				}
			}
			GUILayout.EndScrollView();

			GUI.backgroundColor = Color.red;
			if(isChange) 
				GUILayout.Label("Change Success");
			GUI.backgroundColor = Color.white;
			if(GUILayout.Button("Change Passive Rate")) {
				isChange = true;
				UpdatePassiveSkills.Clear();
				for(int i=0; i<passives.Count; i++) {
					TPassiveType type = new TPassiveType();
					TSkill skill = new TSkill();
					skill.ID = passives[i].ID;
					skill.Lv = passives[i].Lv;
					type.Tskill = skill;
					type.Rate = passives[i].Rate;
					if (UpdatePassiveSkills.ContainsKey(passives[i].Kind))
						UpdatePassiveSkills [passives[i].Kind].Add(type);
					else {
						List<TPassiveType> pss = new List<TPassiveType>();
						pss.Add(type);
						UpdatePassiveSkills.Add(passives[i].Kind, pss);
					}
				}

				skillController.DPassiveSkills = UpdatePassiveSkills;
			}
		} else {
			GUILayout.Label("Please Choose One Player!!");
		}
	}
}
