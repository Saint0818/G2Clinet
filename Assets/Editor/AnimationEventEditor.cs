﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AnimationEventEditor : EditorWindow {
	[MenuItem ("GameEditor/AnimationEventEditor")]
	public static void BuildTool() {
		EditorWindow.GetWindowWithRect(typeof(AnimationEventEditor), new Rect(0, 0, 600, 800), true, "AnimationEventEditor").Show();
	}

	private AnimationClip sourceObject;
	private bool isSave = false;

	private List<AnimationEvent> aryTempEvent = new List<AnimationEvent>();
	private List<string> aryTempFunctionName = new List<string>();
	private List<float> aryTempFloatParameter = new List<float>();
	private List<int> aryTempIntParameter = new List<int>();
	private List<string> aryTempString = new List<string>();
	private List<Object> aryTempObject = new List<Object>();
	private List<float> aryTempTime = new List<float>();

	private List<string> aryTagName = new List<string>();
	private string separateName = string.Empty;
	private int count = 0;
	private bool isCal = false;
	private int showCount = 0;

	private AnimationEvent[] aryEvent = new AnimationEvent[0];

	private Vector2 scrollPosition = Vector2.zero;
	private Vector2 scrollTagPosition = Vector2.zero;
	
	private GUIStyle style = new GUIStyle();

	private int baseHeight = 200;

	private int index = 0;
	private string[] options;
	public List<AnimationClip> allAnimationClip = new List<AnimationClip>();

	void OnFocus(){
		allAnimationClip.Clear();
		UnityEngine.Object[] animationObjs = Resources.LoadAll("Character/PlayerModel_2/Animation", typeof(AnimationClip));
		for(int i=0; i<animationObjs.Length; i++) {
			AnimationClip clip = animationObjs[i] as AnimationClip;
			if(!allAnimationClip.Contains(clip))
				allAnimationClip.Add(clip);
		}
		options = new string[allAnimationClip.Count];
		for(int i=0; i<allAnimationClip.Count; i++){
			options[i] = allAnimationClip[i].name;
		}
	}

	void OnGUI(){
		style.normal.textColor = Color.yellow;

		if(GUI.Button(new Rect (0, 50, 100, 20), "Get Event")) {
			init();
			loadEvent();
		}
		if(GUI.Button(new Rect (150, 50, 100, 20), "Add Event")) {
			addEvent();
		}

//		GUI.Label(new Rect(0, 50, 100, 20), "AnimationObject:");
		if(options != null)
			index = EditorGUI.Popup(new Rect(0, 20, 200, 20), "AnimationObject:", index, options);
		sourceObject = allAnimationClip[index];
//		sourceObject = EditorGUI.ObjectField(new Rect(100, 50, 200, 20), sourceObject, typeof(AnimationClip), true) as AnimationClip;
		
		
		GUI.Label(new Rect(0, 80, 180, 30), "AnimationClip Totally Length :");
		if(sourceObject != null)
			GUI.Label(new Rect(180, 80, 50, 30), (sourceObject.length * 30).ToString());
		
		GUI.Label(new Rect(230, 80, 10, 30), "|");
		GUI.Label(new Rect(240, 80, 200, 30), "AnimationClip Totally Time :");
		if(sourceObject != null)
			GUI.Label(new Rect(400, 80, 200, 30), sourceObject.length.ToString());

		GUI.Label(new Rect(0, 120, 150, 20), "AnimationEvent List Count:", style);
		GUI.Label(new Rect(160, 120, 100, 20), aryTempEvent.Count.ToString(), style);


		scrollTagPosition = GUI.BeginScrollView(new Rect(0, 140, 600, 80), scrollTagPosition, new Rect(0, 140, aryTagName.Count + 100, 80));

		if(GUI.Button(new Rect (0, 140, 100, 40), "All")){
			separateName = string.Empty;
			isCal = false;
			showCount = 0;
		}
		for (int i=0; i<aryTagName.Count; i++) {
			if(GUI.Button(new Rect (100 + 100*i, 140, 100, 40), aryTagName[i])){
				separateName = aryTagName[i];
				isCal = false;
				showCount = 0;
			}
		}

		GUI.EndScrollView();

		scrollPosition = GUI.BeginScrollView(new Rect(0, baseHeight, 530, 550), scrollPosition, new Rect(0, baseHeight, 500, (showCount * 200)));
		count = 0;
		if(aryTempEvent.Count > 0){
			for(int i=0; i<aryTempEvent.Count; i++) {
				if(string.IsNullOrEmpty(separateName)) {
					GUI.Label(new Rect(0, baseHeight + 20 + baseHeight * i, 100, 20), "FunctionName");
					aryTempFunctionName[i] = GUI.TextField(new Rect(100, baseHeight + 20 + baseHeight * i, 200, 20) , aryTempFunctionName[i]);

					GUI.Label(new Rect(0, baseHeight + 40 + baseHeight * i, 100, 20), "Float");
					aryTempFloatParameter[i] = EditorGUI.FloatField(new Rect(100, baseHeight + 40 + baseHeight * i, 200, 20), aryTempFloatParameter[i]);

					GUI.Label(new Rect(0, baseHeight + 60 + baseHeight * i, 100, 20), "Int");
					aryTempIntParameter[i] = EditorGUI.IntField(new Rect(100, baseHeight + 60 + baseHeight * i, 200, 20), aryTempIntParameter[i]);
					
					GUI.Label(new Rect(0, baseHeight + 80 + baseHeight * i, 100, 20), "String");
					aryTempString[i] = EditorGUI.TextField(new Rect(100, baseHeight + 80 + baseHeight * i, 200, 20), aryTempString[i]);
					
					GUI.Label(new Rect(0, baseHeight + 100 + baseHeight * i, 100, 20), "Object");
					aryTempObject[i] = EditorGUI.ObjectField(new Rect(100, baseHeight + 100+ baseHeight * i, 200, 20), aryTempObject[i], typeof(Object), true) as Object;

					GUI.Label(new Rect(0, baseHeight + 120 + baseHeight * i, 100, 20), "Time");
					aryTempTime[i] = EditorGUI.FloatField(new Rect(100, baseHeight + 120 + baseHeight * i, 200, 20), aryTempTime[i]);

					
					if(GUI.Button(new Rect(320, baseHeight + 40 + baseHeight * i, 180, 20), "Delete Event Index:"+(i+1).ToString())) {
						deleteEvent(i);
					}
					GUI.Label(new Rect(0, baseHeight + 140 + baseHeight * i, 600, 20), "=====================================================================================================================================");
					if(!isCal){
						showCount++;
					}
				} else {
					if(aryTempFunctionName[i].Equals(separateName)) {
						GUI.Label(new Rect(0, baseHeight + 20 + baseHeight * count, 100, 20), "FunctionName");
						aryTempFunctionName[i] = GUI.TextField(new Rect(100, baseHeight + 20 + baseHeight * count, 200, 20) , aryTempFunctionName[i]);
						
						GUI.Label(new Rect(0, baseHeight + 40 + baseHeight * count, 100, 20), "Float");
						aryTempFloatParameter[i] = EditorGUI.FloatField(new Rect(100, baseHeight + 40 + baseHeight * count, 200, 20), aryTempFloatParameter[i]);
						
						GUI.Label(new Rect(0, baseHeight + 60 + baseHeight * count, 100, 20), "Int");
						aryTempIntParameter[i] = EditorGUI.IntField(new Rect(100, baseHeight + 60 + baseHeight * count, 200, 20), aryTempIntParameter[i]);
						
						GUI.Label(new Rect(0, baseHeight + 80 + baseHeight * count, 100, 20), "String");
						aryTempString[i] = EditorGUI.TextField(new Rect(100, baseHeight + 80 + baseHeight * count, 200, 20), aryTempString[i]);
						
						GUI.Label(new Rect(0, baseHeight + 100 + baseHeight * count, 100, 20), "Object");
						aryTempObject[i] = EditorGUI.ObjectField(new Rect(100, baseHeight + 100+ baseHeight * count, 200, 20), aryTempObject[i], typeof(Object), true) as Object;
						
						GUI.Label(new Rect(0, baseHeight + 120 + baseHeight * count, 100, 20), "Time");
						aryTempTime[i] = EditorGUI.FloatField(new Rect(100, baseHeight + 120 + baseHeight * count, 200, 20), aryTempTime[i]);
						
						
						if(GUI.Button(new Rect(320, baseHeight + 40 + baseHeight * count, 180, 20), "Delete Event Index:"+(count+1).ToString())) {
							deleteEvent(i);
						}
						GUI.Label(new Rect(0, baseHeight + 140 + baseHeight * count, 600, 20), "=====================================================================================================================================");
						count ++;
					}
				}
			}
		}
		GUI.EndScrollView();
		if(!string.IsNullOrEmpty(separateName)) 
			showCount = count;
		else { 
			if(!isCal)
				isCal = true;
		}

		if(GUI.Button(new Rect (500, 770, 100, 20), "Save Event")) {
			isSave = true;
			AnimationEvent[] newEvents = new AnimationEvent[aryTempEvent.Count];
			for (int i=0; i<newEvents.Length; i++) {
				AnimationEvent event1 = new AnimationEvent();
				event1.functionName = aryTempFunctionName[i];
				event1.floatParameter = aryTempFloatParameter[i];
				event1.intParameter = aryTempIntParameter[i];
				event1.stringParameter = aryTempString[i];
				event1.objectReferenceParameter = aryTempObject[i];
				event1.time = aryTempTime[i];
				newEvents[i] = event1;
			}
			DoAddEventImportedClip(sourceObject, newEvents, sourceObject.length);
		}
		if(isSave)
			GUI.Label(new Rect(300, 580 , 60, 20), "Save Success!", style);
	}

	private void init(){
		separateName = string.Empty;
		isSave = false;
		aryTagName.Clear();
		aryTempEvent.Clear();
		aryTempFunctionName.Clear();
		aryTempFloatParameter.Clear();
		aryTempIntParameter.Clear();
		aryTempString.Clear();
		aryTempObject.Clear();
		aryTempTime.Clear();
	}

	private void loadEvent(){
		isCal = false;
		showCount = 0;
		if(sourceObject == null)
			return;
		aryEvent = AnimationUtility.GetAnimationEvents(sourceObject);
		for(int i=0; i<aryEvent.Length; i++) {
			aryTempEvent.Add(aryEvent[i]);
			aryTempFunctionName.Add(aryEvent[i].functionName);
			aryTempFloatParameter.Add(aryEvent[i].floatParameter);
			aryTempIntParameter.Add(aryEvent[i].intParameter);
			aryTempString.Add(aryEvent[i].stringParameter);
			aryTempObject.Add(aryEvent[i].objectReferenceParameter);
			aryTempTime.Add(aryEvent[i].time);

			if(!aryTagName.Contains(aryEvent[i].functionName))
				aryTagName.Add(aryEvent[i].functionName);
		}
	}

	private void addEvent(){
		isSave = false;
		AnimationEvent newEvent = new AnimationEvent();
		aryTempEvent.Add(newEvent);
		aryTempFunctionName.Add(newEvent.functionName);
		aryTempFloatParameter.Add(newEvent.floatParameter);
		aryTempIntParameter.Add(newEvent.intParameter);
		aryTempString.Add(newEvent.stringParameter);
		aryTempObject.Add(newEvent.objectReferenceParameter);
		aryTempTime.Add(newEvent.time);
	}

	private void deleteEvent(int index) {
		isSave = false;
		aryTempEvent.RemoveAt(index);
		aryTempFunctionName.RemoveAt(index);
		aryTempFloatParameter.RemoveAt(index);
		aryTempIntParameter.RemoveAt(index);
		aryTempString.RemoveAt(index);
		aryTempObject.RemoveAt(index);
		aryTempTime.RemoveAt(index);
	}

	void DoAddEventImportedClip(AnimationClip sourceAnimClip, AnimationEvent[] targetAnimEvent, float length){	
		ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sourceAnimClip)) as ModelImporter;
		if (modelImporter == null)
			return;
		
		SerializedObject serializedObject = new SerializedObject(modelImporter);
		SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
		
		if (!clipAnimations.isArray)
			return;
		
		for (int i = 0; i < clipAnimations.arraySize; i++){
			AnimationClipInfoProperties clipInfoProperties = new AnimationClipInfoProperties(clipAnimations.GetArrayElementAtIndex(i), length);
//			AnimationEvent[] sourceAnimEvents = AnimationUtility.GetAnimationEvents(sourceAnimClip);
			clipInfoProperties.SetEvents(targetAnimEvent);
			serializedObject.ApplyModifiedProperties();
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(sourceAnimClip));
		}
	}

//	void DoAddEventImportedClip(AnimationClip sourceAnimClip, AnimationClip targetAnimClip, float length){	
//		ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(targetAnimClip)) as ModelImporter;
//		if (modelImporter == null)
//			return;
//		
//		SerializedObject serializedObject = new SerializedObject(modelImporter);
//		SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
//		
//		if (!clipAnimations.isArray)
//			return;
//		
//		for (int i = 0; i < clipAnimations.arraySize; i++){
//			AnimationClipInfoProperties clipInfoProperties = new AnimationClipInfoProperties(clipAnimations.GetArrayElementAtIndex(i), length);
//			if (clipInfoProperties.name == targetAnimClip.name){
//				AnimationEvent[] sourceAnimEvents = AnimationUtility.GetAnimationEvents(sourceAnimClip);
//				clipInfoProperties.SetEvents(sourceAnimEvents);
//				serializedObject.ApplyModifiedProperties();
//				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(targetAnimClip));
//				break;
//			} 
//		}
//	}
	
	class AnimationClipInfoProperties {
		SerializedProperty m_Property;
		private float clipLength = 0f;
		
		private SerializedProperty Get(string property) { return m_Property.FindPropertyRelative(property); }
		
		public AnimationClipInfoProperties(SerializedProperty prop, float length) { m_Property = prop; clipLength = length;}
		
		public string name { get { return Get("name").stringValue; } set { Get("name").stringValue = value; } }
		public string takeName { get { return Get("takeName").stringValue; } set { Get("takeName").stringValue = value; } }
		public float firstFrame { get { return Get("firstFrame").floatValue; } set { Get("firstFrame").floatValue = value; } }
		public float lastFrame { get { return Get("lastFrame").floatValue; } set { Get("lastFrame").floatValue = value; } }
		public int wrapMode { get { return Get("wrapMode").intValue; } set { Get("wrapMode").intValue = value; } }
		public bool loop { get { return Get("loop").boolValue; } set { Get("loop").boolValue = value; } }
		
		// Mecanim animation properties
		public float orientationOffsetY { get { return Get("orientationOffsetY").floatValue; } set { Get("orientationOffsetY").floatValue = value; } }
		public float level { get { return Get("level").floatValue; } set { Get("level").floatValue = value; } }
		public float cycleOffset { get { return Get("cycleOffset").floatValue; } set { Get("cycleOffset").floatValue = value; } }
		public bool loopTime { get { return Get("loopTime").boolValue; } set { Get("loopTime").boolValue = value; } }
		public bool loopBlend { get { return Get("loopBlend").boolValue; } set { Get("loopBlend").boolValue = value; } }
		public bool loopBlendOrientation { get { return Get("loopBlendOrientation").boolValue; } set { Get("loopBlendOrientation").boolValue = value; } }
		public bool loopBlendPositionY { get { return Get("loopBlendPositionY").boolValue; } set { Get("loopBlendPositionY").boolValue = value; } }
		public bool loopBlendPositionXZ { get { return Get("loopBlendPositionXZ").boolValue; } set { Get("loopBlendPositionXZ").boolValue = value; } }
		public bool keepOriginalOrientation { get { return Get("keepOriginalOrientation").boolValue; } set { Get("keepOriginalOrientation").boolValue = value; } }
		public bool keepOriginalPositionY { get { return Get("keepOriginalPositionY").boolValue; } set { Get("keepOriginalPositionY").boolValue = value; } }
		public bool keepOriginalPositionXZ { get { return Get("keepOriginalPositionXZ").boolValue; } set { Get("keepOriginalPositionXZ").boolValue = value; } }
		public bool heightFromFeet { get { return Get("heightFromFeet").boolValue; } set { Get("heightFromFeet").boolValue = value; } }
		public bool mirror { get { return Get("mirror").boolValue; } set { Get("mirror").boolValue = value; } }
		
		public AnimationEvent GetEvent(int index){
			AnimationEvent evt = new AnimationEvent();
			SerializedProperty events = Get("events");
			
			if (events != null && events.isArray){
				if (index < events.arraySize){
					evt.floatParameter = events.GetArrayElementAtIndex(index).FindPropertyRelative("floatParameter").floatValue;
					evt.functionName = events.GetArrayElementAtIndex(index).FindPropertyRelative("functionName").stringValue;
					evt.intParameter = events.GetArrayElementAtIndex(index).FindPropertyRelative("intParameter").intValue;
					evt.objectReferenceParameter = events.GetArrayElementAtIndex(index).FindPropertyRelative("objectReferenceParameter").objectReferenceValue;
					evt.stringParameter = events.GetArrayElementAtIndex(index).FindPropertyRelative("data").stringValue;
					evt.time = events.GetArrayElementAtIndex(index).FindPropertyRelative("time").floatValue;
				}else{
					Debug.LogWarning("Invalid Event Index");
				}
			}
			
			return evt;
		}
		
		public void SetEvent(int index, AnimationEvent animationEvent){
			SerializedProperty events = Get("events");
			
			if (events != null && events.isArray){
				if (index < events.arraySize){
					events.GetArrayElementAtIndex(index).FindPropertyRelative("floatParameter").floatValue = animationEvent.floatParameter;
					events.GetArrayElementAtIndex(index).FindPropertyRelative("functionName").stringValue = animationEvent.functionName;
					events.GetArrayElementAtIndex(index).FindPropertyRelative("intParameter").intValue = animationEvent.intParameter;
					events.GetArrayElementAtIndex(index).FindPropertyRelative("objectReferenceParameter").objectReferenceValue = animationEvent.objectReferenceParameter;
					events.GetArrayElementAtIndex(index).FindPropertyRelative("data").stringValue = animationEvent.stringParameter;
					events.GetArrayElementAtIndex(index).FindPropertyRelative("time").floatValue = animationEvent.time / clipLength;
				}else{
					Debug.LogWarning("Invalid Event Index");
				}
			}
		}
		
		
		public void ClearEvents(){
			SerializedProperty events = Get("events");
			if (events != null && events.isArray){
				events.ClearArray();
			}
		}
		
		public int GetEventCount(){
			int ret = 0;
			SerializedProperty curves = Get("events");
			if (curves != null && curves.isArray){
				ret = curves.arraySize;
			}
			return ret;
		}
		
		public void SetEvents(AnimationEvent[] newEvents){
			SerializedProperty events = Get("events");
			if (events != null && events.isArray){
				events.ClearArray();	
				foreach (AnimationEvent evt in newEvents){
					events.InsertArrayElementAtIndex(events.arraySize);
					SetEvent(events.arraySize - 1, evt);
				}
			}
		}
		
		public AnimationEvent[] GetEvents(){
			AnimationEvent[] ret = new AnimationEvent[GetEventCount()];
			SerializedProperty events = Get("events");
			if (events != null && events.isArray){
				for (int i = 0; i < GetEventCount(); ++i){
					ret[i] = GetEvent(i);
				}
			}
			return ret;
		}
		
	}
}
