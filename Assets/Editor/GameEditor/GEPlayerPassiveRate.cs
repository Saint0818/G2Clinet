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
	private List<TChangePassiveRate> passives = new List<TChangePassiveRate>();
	private SkillController skillController;
	
	private string[] options;
	private int index = 0;
	private int chooseIndex = 0;

	void OnFocus(){
		isChange = false;
		index = 0;
		passives.Clear();
		if(Selection.gameObjects.Length == 1) {
			isChoose = true;
			skillController = Selection.gameObjects[0].GetComponent<SkillController>();
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
		} else
			isChoose = false;
	}

	void OnGUI(){
		if(isChoose) {
			if(options != null)
				chooseIndex = EditorGUI.Popup(new Rect(0, 0, 200, 20), "Kind:", chooseIndex, options);
			
			GUILayout.Label(" ");
			GUILayout.Label(" ");
			for(int i=0; i<passives.Count; i++) {
				if(int.Parse(options[chooseIndex]) == passives[i].Kind) {
					GUILayout.Label("ID:" + passives[i].ID);
					GUILayout.Label("passives[i].Rate:" + passives[i].Rate);
					passives[i].Rate = Mathf.RoundToInt(GUILayout.HorizontalSlider((float) passives[i].Rate , 0, 100));
					GUILayout.Label("====================");

				}
			}

			GUI.backgroundColor = Color.red;
			if(isChange) 
				GUILayout.Label("Change Success");
			GUI.backgroundColor = Color.white;
			if(GUILayout.Button("Change")) {
				isChange = true;
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
