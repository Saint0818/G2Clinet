using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

public class BasketPositionEdit : EditorWindow {

	[MenuItem ("GameEditor/BallPositionEdit")]
	private static void PositionEdit()
	{
		EditorWindow.GetWindowWithRect(typeof(BasketPositionEdit), new Rect(0, 0, 600, 600), true, "EditBallPosition").Show();
	}
	private string FileName = "";
	private TBasketShootPositionData[] basketShootPositionData = new TBasketShootPositionData[0];
	private TBasketShootPositionData[] basketShootPositionSaveData = new TBasketShootPositionData[0];
	private Dictionary<string, Vector3> basketTempShootPositionData = new Dictionary<string, Vector3>();
	private List<AnimationClip> allBasketAnimationClip = new List<AnimationClip>();
	private AnimationClipCurveData[] curveData = new AnimationClipCurveData[0];
	private Vector2 scrollPositionController = Vector2.zero;
	private Vector2 scrollPositionAnimationClips = Vector2.zero;
	
	private GUIStyle style = new GUIStyle();
	private bool isSave = false;
	void OnFocus(){
		FileName = Application.dataPath + "/Resources/GameData/ballposition.json";
		OnLoad();
	}

	void OnGUI() {
		style.normal.textColor = Color.red;

		GUI.Label(new Rect(50, 80, 200, 30), "json data");
		GUI.Label(new Rect(350, 80, 200, 30), "AllBasketAnimation");
		if(basketShootPositionData.Length > 0 ){
			GUI.Label(new Rect(100, 120, 200, 30), "Count:"+basketShootPositionData.Length);
			scrollPositionController = GUI.BeginScrollView (new Rect (50, 150, 200, 400), scrollPositionController, new Rect (50, 150, 200, basketShootPositionData.Length * 20));
			for (int i=0; i<basketShootPositionData.Length; i++) {
				GUI.Label(new Rect(50, (20 * i)+ 150, 150, 20), basketShootPositionData[i].AnimationName);	
			}
			GUI.EndScrollView ();
		} else {
			GUI.Label(new Rect(100, 120, 200, 30), "Count:0");
		}
		
		
		if(allBasketAnimationClip.Count > 0) {
			GUI.Label(new Rect(350, 120, 400, 30), "Count:"+allBasketAnimationClip.Count);
			scrollPositionAnimationClips = GUI.BeginScrollView (new Rect (300, 150, 200, 400), scrollPositionAnimationClips, new Rect (300, 150, 200, allBasketAnimationClip.Count * 20));
			for (int i=0; i<allBasketAnimationClip.Count; i++) {
				if(allBasketAnimationClip.Count > 0){
					bool isMatch = false;
					for(int j=0; j<basketShootPositionData.Length; j++) {
						if(basketShootPositionData[j].AnimationName.Equals(allBasketAnimationClip[i].name))
							isMatch = true;
					}
					if(isMatch) 
						GUI.Label(new Rect(300, (20 * i)+ 150, 150, 20), allBasketAnimationClip[i].name);
					else 
						GUI.Label(new Rect(300, (20 * i)+ 150, 150, 20), allBasketAnimationClip[i].name, style);
				} else
					GUI.Label(new Rect(300, (20 * i)+ 150, 150, 20), allBasketAnimationClip[i].name);
			}
			GUI.EndScrollView ();
		} else {
			GUI.Label(new Rect(100, 120, 200, 30), "Count:0");
		}

		if(GUI.Button(new Rect(200, 550, 200, 20), "Save"))
			OnSave();
		if(isSave)
			GUI.Label(new Rect(200, 575, 200, 20), "Save Success", style);
	}

	private void OnLoad(){
		//Right  All BasketAnimation
		isSave = false;
		allBasketAnimationClip.Clear();
//		UnityEngine.Object[] animationObjs = Resources.LoadAll("Stadiums/Basket/Animation", typeof(AnimationClip));
		string path = "Assets/01.Art/Scene/Stadium/Basket/Animation/";
		string[] files = Directory.GetFiles(path);
//		UnityEngine.Object[] animationObjs = AssetDatabase.LoadAllAssetsAtPath(path);
//		UnityEngine.Object[] animationObjs = AssetDatabase.LoadAllAssetsAtPath("Assets/01.Art/Scene/");
		List<UnityEngine.Object> animationObjs = new List<UnityEngine.Object>();
		for(int i=0; i<files.Length; i++) {
			UnityEngine.Object animationObj = AssetDatabase.LoadAssetAtPath(files[i], typeof(AnimationClip));
			if(animationObj != null) 
				animationObjs.Add(animationObj);
		}

		basketTempShootPositionData.Clear();
		for(int i=0; i<animationObjs.Count; i++) {
			if(animationObjs[i].name.Contains("BasketballAction_")){
				AnimationClip clip = animationObjs[i] as AnimationClip;
				if(!allBasketAnimationClip.Contains(clip))
					allBasketAnimationClip.Add(clip);
				
				curveData = AnimationUtility.GetAllCurves(clip, true);
				Vector3 pos = new Vector3();
				for(int j=0; j<curveData.Length; j++) {
					if(j<3){
						if(j==0)
							pos.x = curveData[j].curve.keys[0].value;
						else if(j==1)
							pos.y = curveData[j].curve.keys[0].value;
						else if(j==2)
							pos.z = curveData[j].curve.keys[0].value;
					} else if(j == 3){
						basketTempShootPositionData.Add(animationObjs[i].name, pos);
					}
				}
			}
		}
		allBasketAnimationClip.Sort(
			delegate(AnimationClip i1, AnimationClip i2) { 
			return i1.name.CompareTo(i2.name); 
		}
		);
		//Left  Json BasketAnimation
		if (File.Exists(FileName)){
			TextAsset tx = Resources.Load("GameData/ballposition") as TextAsset;
			if (tx){
				basketShootPositionData = (TBasketShootPositionData[])JsonConvert.DeserializeObject(tx.text, typeof(TBasketShootPositionData[]));
			} 
		}
	}

	private void OnSave(){
		basketShootPositionSaveData = new TBasketShootPositionData[basketTempShootPositionData.Count];
		int index = -1;
		foreach (KeyValuePair<string,Vector3> btsp in basketTempShootPositionData) {
			index ++ ;
			basketShootPositionSaveData[index].AnimationName = btsp.Key;
			basketShootPositionSaveData[index].ShootPositionX = btsp.Value.x;
			basketShootPositionSaveData[index].ShootPositionY = btsp.Value.y;
			basketShootPositionSaveData[index].ShootPositionZ = btsp.Value.z;
		}
		isSave = true;
		SaveFile(FileName, JsonConvert.SerializeObject(basketShootPositionSaveData));
	}

	public void SaveFile(string fileName, string Data){
		if (File.Exists(fileName))
			File.WriteAllText(fileName, string.Empty);
		
		using (FileStream myFile = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
			using (StreamWriter myWriter = new StreamWriter(myFile)) {
				myWriter.Write(Data);
				myWriter.Close();
			}
			myFile.Close();
		}
	}
}
