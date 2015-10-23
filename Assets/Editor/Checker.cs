﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using GameEnum;
using GameStruct;
using System.Collections.Generic;
using System;
//using System.Text;
using Newtonsoft.Json;

public class Checker : EditorWindow {
	[MenuItem ("Knight49/CheckTool")]
	// Use this for initialization
	private static void BuildTool()
	{
		EditorWindow.GetWindowWithRect(typeof(Checker), new Rect(0, 0, 800, 400), true, "CheckTool").Show();
	}

//	private string[] checkSubject = new string[3]{"Wait", "Wait", "Wait"};
	private bool[] states = new bool[3];
	private Color[] statesColor = new Color[3];
//	private StringBuilder ErrorData = new StringBuilder();
	
	private Dictionary<string, string> AnimationEventFunctionData = new Dictionary<string, string>();
	private Dictionary<string, string> AnimationEventStringData = new Dictionary<string, string>();
	private Dictionary<string, string> AnimationStateData = new Dictionary<string, string>();
	
	void OnGUI()
	{
		if (GUILayout.Button("Test AnimationEvent", GUILayout.Width(200)))
		{
			for(int i = 0;i < states.Length;i++){
				states[i] = false;
				statesColor[i] = Color.grey;
			}

			InitCheckData();
			TestAnimationEvent();
			TestAnimationVsSkillData();

		}
		GUILayout.Toggle (states[0], "AnimationEvent");
		GUILayout.Toggle (states[1], "Animation Vs SkillData");
		GUILayout.Toggle (states[2], "AnimationEvent");
	}

	public void TestAnimationEvent()
	{
		bool haveError = false;
		statesColor [0] = Color.grey; 
		for(int i = 0; i < 3;i++){
			UnityEditor.Animations.AnimatorController controller = Resources.Load("Character/PlayerModel_"+i+"/AnimationControl") as UnityEditor.Animations.AnimatorController;
			if(controller){
				AnimationClip[] animationClip = controller.animationClips;
				for(int j = 0;j < animationClip.Length;j++){
					for(int k = 0; k < animationClip[j].events.Length; k++){
						if(AnimationEventFunctionData.ContainsKey(animationClip[j].events[k].functionName)){
							if(animationClip[j].events[k].functionName == EanimationEventFunction.AnimationEvent.ToString()){
								if(!AnimationEventStringData.ContainsKey(animationClip[j].events[k].stringParameter)){
									haveError = true;
									Debug.LogError(string.Format("Error Animationve Event , Player : {0}, animationClip : {1} , stringParameter : {2}", 
									                             i, animationClip[j].name, animationClip[j].events[k].stringParameter));
								}
							}
							else{
								//TODO:Other Event Work
//								Debug.LogError("Other Work : " + animationClip[j].events[k].functionName);
							}
						}
						else
						{
							haveError = true;
							Debug.LogError(string.Format("Error Animationve Function not found, Player : {0}, animationClip :{1}, FunctionName :{2}", 
							                             i, animationClip[j].name, animationClip[j].events[k].functionName));

						}
					}
				}
			}
		}

		states [0] = !haveError;
		statesColor [0] = (haveError == true? Color.red : Color.white); 
	}

	public void TestAnimationByCode()
	{

	}


	private void AddError(int kind, string message)
	{

	}

	private void InitCheckData()
	{
		//AnimationFunction
		foreach (EanimationEventFunction item in Enum.GetValues(typeof(EanimationEventFunction))){
			if(!AnimationEventFunctionData.ContainsKey(item.ToString())){
				AnimationEventFunctionData.Add(item.ToString(), item.ToString());
			}
		}
		//AnimationEventString
		foreach (EAnimationEventString item in Enum.GetValues(typeof(EAnimationEventString))){
			if(!AnimationEventStringData.ContainsKey(item.ToString())){
				AnimationEventStringData.Add(item.ToString(), item.ToString());
			}
		}

		//AnimationSate
		foreach (EPlayerState item in Enum.GetValues(typeof(EPlayerState))){
			if(!AnimationStateData.ContainsKey(item.ToString())){
				AnimationStateData.Add(item.ToString(), item.ToString());
			}
		}

		//
	}

	private void TestAnimationVsSkillData ()
	{
		bool haveError = false;
		statesColor [1] = Color.grey;
		TextAsset text = Resources.Load("GameData/skill") as TextAsset;
		TSkillData[] data = (TSkillData[])JsonConvert.DeserializeObject (text.text, typeof(TSkillData[]));
		for (int i = 0; i < data.Length; i++) {
			if(data[i].Animation == null || data[i].Animation == string.Empty)
				Debug.LogError("Skill Data Error, ID : " + data[i].ID + ", Current Animation : Empty");
			else{
				if(!AnimationStateData.ContainsKey(data[i].Animation)){
					haveError = true;
					Debug.LogError("Skill Data Error, ID : " + data[i].ID + ", Current Animation : " + data[i].Animation);
				}
			}
		}

		states [1] = !haveError;
		statesColor [1] = (haveError == true? Color.red : Color.white);
	}
}
