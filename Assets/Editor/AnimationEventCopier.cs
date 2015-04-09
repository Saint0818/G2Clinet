using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class AnimationEventCopier : EditorWindow {
	[MenuItem ("GameEditor/AnimationEventCopier")]
	public static void BuildTool() {
		EditorWindow.GetWindowWithRect(typeof(AnimationEventCopier), new Rect(0, 0, 600, 600), true, "AnimationEventCopier").Show();
	}

	private AnimationClip sourceObject;
	private AnimationClip targetObject;
	private bool isCopy = false;

	private List<string> aryChangeName = new List<string>();
	private List<string> aryNoChangeName = new List<string>();

	private string sourceAnimtionID = string.Empty;
	private string targetAnimtionID = string.Empty;

	private Vector2 scrollPosition = Vector2.zero;
	private Vector2 scrollPositionNoChange = Vector2.zero;

	private GUIStyle style = new GUIStyle();
	

	void OnGUI() {
		style.normal.textColor = Color.red;

		GUI.Label(new Rect(0, 0, 300, 30), "All Animation Change");

		GUI.Label(new Rect(0, 30, 60, 20), "Source ID:");
		sourceAnimtionID = GUI.TextField(new Rect(60, 30, 200, 20), sourceAnimtionID);

		GUI.Label(new Rect(0, 60, 60, 20), "Target ID:");
		targetAnimtionID = GUI.TextField(new Rect(60, 60, 200, 20), targetAnimtionID);
		
		if (sourceAnimtionID != string.Empty && targetAnimtionID != string.Empty) {
			if(GUI.Button(new Rect(0, 90, 250, 20), "All Copy"))
				allCopyData();
		}

		GUI.Label(new Rect(0, 120, 100, 20), "Change List:", style);
		scrollPosition = GUI.BeginScrollView(new Rect(0, 140, 300, 200), scrollPosition, new Rect(0, 140, 300, (aryChangeName.Count * 20)));
		if(aryChangeName.Count > 0){
			for(int i=0; i<aryChangeName.Count; i++) {
				GUI.Label(new Rect(0, 140 + 20 * i, 100, 20), aryChangeName[i]);
			}
		}
		GUI.EndScrollView();


		GUI.Label(new Rect(0, 350, 100, 20), " No Change List:", style);
		scrollPositionNoChange = GUI.BeginScrollView(new Rect(0, 370, 300, 200), scrollPositionNoChange, new Rect(0, 370, 300, (aryNoChangeName.Count * 20)));
		if(aryNoChangeName.Count > 0){
			for(int i=0; i<aryNoChangeName.Count; i++) {
				GUI.Label(new Rect(0, 370 + (20 * i), 100, 20), aryNoChangeName[i]);
			}
		}
		GUI.EndScrollView();

		GUI.Label(new Rect(300, 0, 300, 30), "one by one");

		GUI.Label(new Rect(300, 30, 60, 20), "Source:");
		sourceObject = EditorGUI.ObjectField(new Rect(360, 30, 200, 20), sourceObject, typeof(AnimationClip), true) as AnimationClip;
		
		GUI.Label(new Rect(300, 60, 60, 20), "Target:");
		targetObject = EditorGUI.ObjectField(new Rect(360, 60, 200, 20), targetObject, typeof(AnimationClip), true) as AnimationClip;
	
		if (sourceObject != null && targetObject != null){
			if(sourceObject.name.Equals(targetObject.name)) {
				if (GUI.Button(new Rect(300, 90, 250, 20), "Copy"))
					oneCopyData();
			}
		}
		if(isCopy) {
			GUI.Label(new Rect(300, 120, 60, 20), "Copy Success!", style);
		}
	}

	void oneCopyData(){
		AnimationClip sourceAnimClip = sourceObject as AnimationClip;
		AnimationClip targetAnimClip = targetObject as AnimationClip;
		if ((targetAnimClip.hideFlags & HideFlags.NotEditable) != 0)
			DoAddEventImportedClip(sourceAnimClip, targetAnimClip, sourceAnimClip.length);
		else
			DoAddEventClip(sourceAnimClip, targetAnimClip);
	}
	
	void allCopyData() {

		aryChangeName.Clear();
		aryNoChangeName.Clear();

		UnityEngine.Object[] sourceObjs = Resources.LoadAll("Character/PlayerModel_"+sourceAnimtionID+"/Animation", typeof(AnimationClip));
		UnityEngine.Object[] targetObjs = Resources.LoadAll("Character/PlayerModel_"+targetAnimtionID+"/Animation", typeof(AnimationClip));
		for (int i=0; i<sourceObjs.Length; i++) {
			bool isMatch = false;
			for(int j=0; j<targetObjs.Length; j++) {
				if(sourceObjs[i].name.Equals(targetObjs[j].name)) {
					isMatch = true;
					AnimationClip sourceAnimClip = sourceObjs[i] as AnimationClip;
					AnimationClip targetAnimClip = targetObjs[j] as AnimationClip;
					if ((targetAnimClip.hideFlags & HideFlags.NotEditable) != 0)
						DoAddEventImportedClip(sourceAnimClip, targetAnimClip, sourceAnimClip.length);
					else
						DoAddEventClip(sourceAnimClip, targetAnimClip);
				}
			}
			if(!isMatch)
				aryNoChangeName.Add(sourceObjs[i].name);
		}


	}

    void DoAddEventClip(AnimationClip sourceAnimClip, AnimationClip targetAnimClip){
		if (sourceAnimClip != targetAnimClip){
			AnimationEvent[] sourceAnimEvents = AnimationUtility.GetAnimationEvents(sourceAnimClip); 
			AnimationUtility.SetAnimationEvents(targetAnimClip, sourceAnimEvents);
		}
	}
	
	void DoAddEventImportedClip(AnimationClip sourceAnimClip, AnimationClip targetAnimClip, float length){
		aryChangeName.Add(sourceAnimClip.name);

		ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(targetAnimClip)) as ModelImporter;
		if (modelImporter == null)
			return;
		
		SerializedObject serializedObject = new SerializedObject(modelImporter);
		SerializedProperty clipAnimations = serializedObject.FindProperty("m_ClipAnimations");
		
		if (!clipAnimations.isArray)
			return;
		
		for (int i = 0; i < clipAnimations.arraySize; i++){
			AnimationClipInfoProperties clipInfoProperties = new AnimationClipInfoProperties(clipAnimations.GetArrayElementAtIndex(i), length);
			if (clipInfoProperties.name == targetAnimClip.name){
				AnimationEvent[] sourceAnimEvents = AnimationUtility.GetAnimationEvents(sourceAnimClip);
				clipInfoProperties.SetEvents(sourceAnimEvents);
				serializedObject.ApplyModifiedProperties();
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(targetAnimClip));
				break;
			} 
		}
	}

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