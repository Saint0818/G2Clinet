using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AnimationEventEditor : EditorWindow {
	[MenuItem ("GameEditor/AnimationEventEditor")]
	public static void BuildTool() {
		EditorWindow.GetWindowWithRect(typeof(AnimationEventEditor), new Rect(0, 0, 600, 600), true, "AnimationEventEditor").Show();
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


	private AnimationEvent[] aryEvent = new AnimationEvent[0];

	private Vector2 scrollPosition = Vector2.zero;
	
	private GUIStyle style = new GUIStyle();

	private int baseHeight = 140;

	void OnGUI(){
		style.normal.textColor = Color.red;
		
		GUI.Label(new Rect(0, 0, 300, 30), "Put AnimationClip");
		
		GUI.Label(new Rect(0, 30, 100, 20), "AnimationObject:");
		if(GUI.Button(new Rect (100, 30, 100, 20), "Get Event")) {
			init();
			loadEvent();
		}
		if(GUI.Button(new Rect (250, 30, 100, 20), "Add Event")) {
			addEvent();
		}
		sourceObject = EditorGUI.ObjectField(new Rect(0, 60, 200, 20), sourceObject, typeof(AnimationClip), true) as AnimationClip;

		GUI.Label(new Rect(0, 120, 100, 20), "AnimationEvent List:", style);
		scrollPosition = GUI.BeginScrollView(new Rect(0, baseHeight, 450, 400), scrollPosition, new Rect(0, baseHeight, 500, (aryTempEvent.Count * 140)));
		if(aryTempEvent.Count > 0){
			for(int i=0; i<aryTempEvent.Count; i++) {
				GUI.Label(new Rect(0, 140 + 20 + baseHeight * i, 100, 20), "FunctionName");
//				GUI.Label(new Rect(100, 140 + 20 + baseHeight * i, 100, 20), aryTempEvent[i].functionName);
				aryTempFunctionName[i] = GUI.TextField(new Rect(100, 140 + 20 + baseHeight * i, 200, 20) , aryTempFunctionName[i]);

				GUI.Label(new Rect(0, 140 + 40 + baseHeight * i, 100, 20), "Float");
//				GUI.Label(new Rect(100, 140 + 40 + baseHeight * i, 100, 20), aryTempEvent[i].floatParameter.ToString());
				aryTempFloatParameter[i] = EditorGUI.FloatField(new Rect(100, 140 + 40 + baseHeight * i, 200, 20), aryTempFloatParameter[i]);

				GUI.Label(new Rect(0, 140 + 60 + baseHeight * i, 100, 20), "Int");
//				GUI.Label(new Rect(100, 140 + 60 + baseHeight * i, 100, 20), aryTempEvent[i].intParameter.ToString());
				aryTempIntParameter[i] = EditorGUI.IntField(new Rect(100, 140 + 60 + baseHeight * i, 200, 20), aryTempIntParameter[i]);
				
				GUI.Label(new Rect(0, 140 + 80 + baseHeight * i, 100, 20), "String");
//				GUI.Label(new Rect(100, 140 + 80 + baseHeight * i, 100, 20), aryTempEvent[i].stringParameter);
				aryTempString[i] = EditorGUI.TextField(new Rect(100, 140 + 80 + baseHeight * i, 200, 20), aryTempString[i]);
				
				GUI.Label(new Rect(0, 140 + 100 + baseHeight * i, 100, 20), "Object");
				aryTempObject[i] = EditorGUI.ObjectField(new Rect(100, 140 + 100+ baseHeight * i, 200, 20), aryTempObject[i], typeof(Object), true) as Object;

				GUI.Label(new Rect(0, 140 + 120 + baseHeight * i, 100, 20), "Time");
//				GUI.Label(new Rect(100, 140 + 120 + baseHeight * i, 100, 20),(aryTempEvent[i].time/ sourceObject.length).ToString());
				aryTempTime[i] = EditorGUI.FloatField(new Rect(100, 140 + 120 + baseHeight * i, 200, 20), aryTempTime[i]);

				
				if(GUI.Button(new Rect(320, 140 + 20 + baseHeight * i, 100, 20), "Delete Event")) {
					deleteEvent(i);
				}
				GUI.Label(new Rect(0, 140 + 140 + baseHeight * i, 600, 20), "=====================================================================================================================================");
			}
		}
		GUI.EndScrollView();


		if(GUI.Button(new Rect (300, 550, 100, 20), "Save Event")) {
			isSave = true;
			AnimationEvent[] newEvents = new AnimationEvent[aryTempEvent.Count];
			for (int i=0; i<newEvents.Length; i++) {
				AnimationEvent event1 = new AnimationEvent();
				event1.functionName = aryTempFunctionName[i];
				event1.floatParameter = aryTempFloatParameter[i];
				event1.intParameter = aryTempIntParameter[i];
				event1.stringParameter = aryTempString[i];
				event1.objectReferenceParameter = aryTempObject[i];
				event1.time = aryTempTime[i] * sourceObject.length;
				newEvents[i] = event1;
			}
			DoAddEventImportedClip(sourceObject, newEvents, sourceObject.length);
		}
		if(isSave)
			GUI.Label(new Rect(300, 580 , 60, 20), "Save Success!", style);
	}

	private void init(){
		isSave = false;
		aryTempEvent.Clear();
		aryTempFunctionName.Clear();
		aryTempFloatParameter.Clear();
		aryTempIntParameter.Clear();
		aryTempString.Clear();
		aryTempObject.Clear();
		aryTempTime.Clear();
	}

	private void loadEvent(){
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
			aryTempTime.Add(aryEvent[i].time/ sourceObject.length);
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
