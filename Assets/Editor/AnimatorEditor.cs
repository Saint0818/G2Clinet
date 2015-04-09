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

//	private void init(){
//		FileUtil.CopyFileOrDirectory("Assets/Resources/Character/PlayerModel_1/AnimationControl.controller",
//		                             "Assets/Resources/Character/PlayerModel_0/AnimationControl.controller");
//	}

	void OnGUI(){
		GUI.Label(new Rect(50, 0, 50, 30), "ID:");
		strId = GUI.TextField(new Rect(80, 0, 100, 20), strId, 10);


		if(GUI.Button(new Rect(50, 40, 130, 20), "get AvatarControl")) {
			allMotionAnimationClip.Clear();
			allAnimationClip.Clear();
			controller = Resources.Load("Character/PlayerModel_"+strId+"/AvatarControl") as UnityEditor.Animations.AnimatorController;

			getAllData();
		}
		if(GUI.Button(new Rect(200, 40, 130, 20), "get AnimationControl")) {
			allMotionAnimationClip.Clear();
			allAnimationClip.Clear();
			controller = Resources.Load("Character/PlayerModel_"+strId+"/AnimationControl") as UnityEditor.Animations.AnimatorController;
			
			getAllData();
		}
		GUI.Label(new Rect(50, 80, 200, 30), "AnimationController");
		GUI.Label(new Rect(350, 80, 200, 30), "AnimationClips");
		if(allMotionAnimationClip.Count > 0 ){
			GUI.Label(new Rect(100, 120, 200, 30), "Count:"+allMotionAnimationClip.Count);
			scrollPositionController = GUI.BeginScrollView (new Rect (50, 150, 200, 200), scrollPositionController, new Rect (50, 150, 200, allMotionAnimationClip.Count * 20));
			for (int i=0; i<allMotionAnimationClip.Count; i++) {
				GUI.Label(new Rect(50, (20 * i)+ 150, 100, 20), allMotionAnimationClip[i].name);
			}
			GUI.EndScrollView ();
		} else {
			GUI.Label(new Rect(100, 120, 200, 30), "Count:0");
		}


		if(allAnimationClip.Count > 0) {
			GUI.Label(new Rect(350, 120, 400, 30), "Count:"+allAnimationClip.Count);
			scrollPositionAnimationClips = GUI.BeginScrollView (new Rect (300, 150, 200, 200), scrollPositionAnimationClips, new Rect (300, 150, 200, allAnimationClip.Count * 20));
			for (int i=0; i<allAnimationClip.Count; i++) {
				GUI.Label(new Rect(300, (20 * i)+ 150, 100, 20), allAnimationClip[i].name);
			}
			GUI.EndScrollView ();
		} else {
			GUI.Label(new Rect(100, 120, 200, 30), "Count:0");
		}

		if(GUI.Button(new Rect(50, 500, 100, 20), "change")) { 
			for(int i=0; i<controller.layers.Length; i++){
				recurrenceSubState(controller.layers[i].stateMachine);
			}
			AssetDatabase.SaveAssets();
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
			if(machine.states[i].state.motion.GetType().Name.Equals("BlendTree")) {
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
					if(machine.states[i].state.motion.name.Equals(allAnimationClip[k].name)) {
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

}
