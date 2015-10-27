using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;


public class AnimatorEditor : EditorWindow {
	[MenuItem ("GameEditor/AnimatorEditor")]
	private static void BuildTool() {
		EditorWindow.GetWindowWithRect(typeof(AnimatorEditor), new Rect(0, 0, 600, 600), true, "AnimatorEditor").Show();
	}

	public UnityEditor.Animations.AnimatorController controller;

	public List<AnimationClip> allMotionAnimationClip = new List<AnimationClip>();
	public List<AnimationClip> allAnimationClip = new List<AnimationClip>();

	private string strId = "";
	private Vector2 scrollPositionController = Vector2.zero;
	private Vector2 scrollPositionAnimationClips = Vector2.zero;

	private GUIStyle style = new GUIStyle();
	private bool isChange = false;

	private bool isChange0 = false;
	private bool isChange1 = false;
	private bool isChange2 = false;

	private bool isGetAvatar = false;
	private bool isGetAnimtor = false;

//	private void init(){
//		FileUtil.CopyFileOrDirectory("Assets/Resources/Character/PlayerModel_1/AnimationControl.controller",
//		                             "Assets/Resources/Character/PlayerModel_0/AnimationControl.controller");
//	}

	void OnFocus(){
		allMotionAnimationClip.Clear();
		allAnimationClip.Clear();
		isChange0 = false;
		isChange1 = false;
		isChange2 = false;

		isGetAvatar = false;
		isGetAnimtor = false;
	}

	bool someOption;

	void OnGUI(){
		style.normal.textColor = Color.red;

		GUI.Label(new Rect(50, 0, 50, 30), "ID:");
//		strId = GUI.TextField(new Rect(80, 0, 100, 20), strId, 10);
		if(isChange0) 
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.white;

		if(GUI.Button(new Rect(50, 0, 100, 20), "PlayerModel_0")) {
			strId = "0";
			allMotionAnimationClip.Clear();
			allAnimationClip.Clear();
			isChange0 = true;
			isChange1 = false;
			isChange2 = false;
			isGetAvatar = false;
			isGetAnimtor = false;
		}

		if(isChange1) 
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.white;

		if(GUI.Button(new Rect(160, 0, 100, 20), "PlayerModel_1")) {
			strId = "1";
			allMotionAnimationClip.Clear();
			allAnimationClip.Clear();
			isChange0 = false;
			isChange1 = true;
			isChange2 = false;
			isGetAvatar = false;
			isGetAnimtor = false;
		}

		if(isChange2) 
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.white;

		if(GUI.Button(new Rect(270, 0, 100, 20), "PlayerModel_2")) {
			strId = "2";
			allMotionAnimationClip.Clear();
			allAnimationClip.Clear();
			isChange0 = false;
			isChange1 = false;
			isChange2 = true;
			isGetAvatar = false;
			isGetAnimtor = false;
		}

		if(isGetAvatar) 
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.white;

		if(GUI.Button(new Rect(50, 40, 130, 20), "Get AvatarControl")) {
			allMotionAnimationClip.Clear();
			allAnimationClip.Clear();
			isGetAvatar = true;
			isGetAnimtor = false;
			isChange = false;
			controller = Resources.Load("Character/PlayerModel_"+strId+"/AvatarControl") as UnityEditor.Animations.AnimatorController;
			if(controller)
				getAllData();
		}

		if (GUI.Button (new Rect (400, 0, 200, 20), "Init All AnimationContorller")) {

			UnityEditor.Animations.AnimatorController[] aniAy = new UnityEditor.Animations.AnimatorController[2];

			for(int i = 0; i < aniAy.Length; i++)
			{
				allAnimationClip.Clear();
				UnityEngine.Object[] animationObjs = Resources.LoadAll("Character/PlayerModel_"+i+"/Animation", typeof(AnimationClip));
				for(int k=0; k<animationObjs.Length; k++) {
					AnimationClip clip = animationObjs[k] as AnimationClip;
					if(!allAnimationClip.Contains(clip))
						allAnimationClip.Add(clip);
				}

				AssetDatabase.DeleteAsset("Assets/Resources/Character/PlayerModel_"+ i +"/AnimationControl.controller");
				AssetDatabase.CopyAsset ("Assets/Resources/Character/PlayerModel_2/AnimationControl.controller", "Assets/Resources/Character/PlayerModel_"+ i +"/AnimationControl.controller");

				AssetDatabase.Refresh();
				aniAy[i] = Resources.Load(string.Format("Character/PlayerModel_{0}/AnimationControl", i)) as UnityEditor.Animations.AnimatorController;

				if(aniAy[i])
					for(int j=0; j<aniAy[i].layers.Length; j++){
						if(j < aniAy[i].layers.Length)
							EditorUtility.DisplayProgressBar("SwitchAnimationClip", "Animator", j / aniAy[i].layers.Length);
					recurrenceSubState(aniAy[i].layers[j].stateMachine);
				}
				else
					Debug.LogError("Error");
			}

			EditorUtility.ClearProgressBar();
			AssetDatabase.SaveAssets();
			ShowHint("Done");
		}

		if(isGetAnimtor) 
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.white;

		if(GUI.Button(new Rect(200, 40, 230, 20), "Get AnimationControl for Game")) {
			allMotionAnimationClip.Clear();
			allAnimationClip.Clear();
			isGetAvatar = false;
			isGetAnimtor = true;
			isChange = false;
			controller = Resources.Load("Character/PlayerModel_"+strId+"/AnimationControl") as UnityEditor.Animations.AnimatorController;
			if(controller)
				getAllData();
		}
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(50, 80, 200, 30), "AnimationController");
		GUI.Label(new Rect(350, 80, 200, 30), "AnimationClips");
		if(allMotionAnimationClip.Count > 0 ){
			GUI.Label(new Rect(100, 120, 200, 30), "Count:"+allMotionAnimationClip.Count);
			scrollPositionController = GUI.BeginScrollView (new Rect (50, 150, 200, 400), scrollPositionController, new Rect (50, 150, 200, allMotionAnimationClip.Count * 20));
			for (int i=0; i<allMotionAnimationClip.Count; i++) {
				if(allAnimationClip.Count > 0){
					bool isMatch = false;
					for(int j=0; j<allAnimationClip.Count; j++) {
						if(allAnimationClip[j].name.Equals(allMotionAnimationClip[i].name))
							isMatch = true;
					}
					if(isMatch) 
						GUI.Label(new Rect(50, (20 * i)+ 150, 100, 20), allMotionAnimationClip[i].name);
					else 
						GUI.Label(new Rect(50, (20 * i)+ 150, 100, 20), allMotionAnimationClip[i].name, style);
				} else
					GUI.Label(new Rect(50, (20 * i)+ 150, 100, 20), allMotionAnimationClip[i].name);

			}
			GUI.EndScrollView ();
		} else {
			GUI.Label(new Rect(100, 120, 200, 30), "Count:0");
		}


		if(allAnimationClip.Count > 0) {
			GUI.Label(new Rect(350, 120, 400, 30), "Count:"+allAnimationClip.Count);
			scrollPositionAnimationClips = GUI.BeginScrollView (new Rect (300, 150, 200, 400), scrollPositionAnimationClips, new Rect (300, 150, 200, allAnimationClip.Count * 20));
			for (int i=0; i<allAnimationClip.Count; i++) {
				if(allMotionAnimationClip.Count > 0){
					bool isMatch = false;
					for(int j=0; j<allMotionAnimationClip.Count; j++) {
						if(allAnimationClip[i].name.Equals(allMotionAnimationClip[j].name))
							isMatch = true;
					}
					if(isMatch) 
						GUI.Label(new Rect(300, (20 * i)+ 150, 100, 20), allAnimationClip[i].name);
					else 
						GUI.Label(new Rect(300, (20 * i)+ 150, 100, 20), allAnimationClip[i].name, style);
				} else
					GUI.Label(new Rect(300, (20 * i)+ 150, 100, 20), allAnimationClip[i].name);
			}
			GUI.EndScrollView ();
		} else {
			GUI.Label(new Rect(100, 120, 200, 30), "Count:0");
		}


		if(allAnimationClip.Count > 0 && allMotionAnimationClip.Count > 0) {
			if(isChange)
				GUI.Label(new Rect(200, 560, 200, 15), "Change Success!", style);
			if(GUI.Button(new Rect(200, 575, 200, 20), "Change Animator Motion")) { 
				for(int i=0; i<controller.layers.Length; i++){
					recurrenceSubState(controller.layers[i].stateMachine);
				}
				isChange = true;
				AssetDatabase.SaveAssets();
			}
		}

		if(GUI.Button(new Rect(450, 40, 130, 20), "Get ShowControl")) {
			allMotionAnimationClip.Clear();
			allAnimationClip.Clear();
			isGetAvatar = true;
			isGetAnimtor = false;
			isChange = false;
			controller = Resources.Load("Character/PlayerModel_"+strId+"/ShowControl") as UnityEditor.Animations.AnimatorController;
			if(controller)
				getAllData();
		}
	}

	private void getAllData(){
		AnimationClip[] animationClip = controller.animationClips;
		for(int i=0; i<animationClip.Length; i++) {
			if(!allMotionAnimationClip.Contains(animationClip[i]))
				allMotionAnimationClip.Add(animationClip[i]);
		}
		allMotionAnimationClip.Sort(
			delegate(AnimationClip i1, AnimationClip i2) { 
			return i1.name.CompareTo(i2.name); 
		}
		);
		UnityEngine.Object[] animationObjs = Resources.LoadAll("Character/PlayerModel_"+strId+"/Animation", typeof(AnimationClip));
		for(int i=0; i<animationObjs.Length; i++) {
			AnimationClip clip = animationObjs[i] as AnimationClip;
			if(!allAnimationClip.Contains(clip))
				allAnimationClip.Add(clip);
		}
		allAnimationClip.Sort(
			delegate(AnimationClip i1, AnimationClip i2) { 
			return i1.name.CompareTo(i2.name); 
		}
		);
	}
	private void recurrenceSubState(AnimatorStateMachine machine) {
		int machineCount = machine.stateMachines.Length;
		int stateCount = machine.states.Length;

		for(int i=0; i<stateCount; i++) {
			if(machine.states[i].state.motion && machine.states[i].state.motion.GetType().Name.Equals("BlendTree")){
				UnityEditor.Animations.BlendTree blendTree = machine.states[i].state.motion as UnityEditor.Animations.BlendTree;
				List<float> aryTherhold = new List<float>();
				List<AnimationClip> aryAnimationClip = new List<AnimationClip>();
				int blendTreeCount = blendTree.children.Length;
				for(int j=0; j<blendTreeCount; j++) {
					aryTherhold.Add(blendTree.children[j].threshold);
					for(int k=0; k<allAnimationClip.Count; k++) {
						if(blendTree.children[j].motion.name.Equals(allAnimationClip[k].name)) {
							aryAnimationClip.Add(allAnimationClip[k]);
						}
					}
				}
				if(aryAnimationClip.Count == aryTherhold.Count) {
					int tempCount = blendTreeCount;
					for(int j=tempCount-1; j>=0; j--) {
						blendTree.RemoveChild(j);
					}
					for(int j=0; j<aryTherhold.Count; j++){
						blendTree.AddChild(aryAnimationClip[j], aryTherhold[j]);
					}
				}
			} else {
				for(int k=0; k<allAnimationClip.Count; k++) {
					if(machine.states[i].state.motion && machine.states[i].state.motion.name.Equals(allAnimationClip[k].name)) {
						machine.states[i].state.motion = allAnimationClip[k];
					}
				}
			}
		}
		if(machineCount > 0){
			for(int i=0; i<machineCount; i++) {
				recurrenceSubState(machine.stateMachines[i].stateMachine);
			}
		}
	}

	private void ShowHint(string str)
	{
		this.ShowNotification(new GUIContent(str));
	}
}
