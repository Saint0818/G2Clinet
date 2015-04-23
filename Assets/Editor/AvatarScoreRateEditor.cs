using UnityEngine;
using UnityEditor;
using System.Collections;

public class AvatarScoreRateEditor : EditorWindow {

	[MenuItem ("GameEditor/AvatarScoreRateEditor")]
	private static void BuildTool() {
		EditorWindow.GetWindowWithRect(typeof(AvatarScoreRateEditor), new Rect(0, 0, 600, 1000), true, "AvatarScoreRateEditor").Show();
	}

	private bool isChoose = false;
	private bool isChange = false;
	private TScoreRate scoreRate;
	private PlayerBehaviour p;
	private Vector2 scrollPositionAnimation = Vector2.zero;

	void OnFocus(){
		isChange = false;
		scoreRate = new TScoreRate();
		if(Selection.gameObjects.Length == 1) {
			isChoose = true;
			p = Selection.gameObjects[0].GetComponent<PlayerBehaviour>();
			if(p != null) {
				scoreRate = p.ScoreRate;
			} else 
				isChoose = false;
		} else
			isChoose = false;
	}

	void OnGUI(){
		if(isChoose) {
			GUILayout.Label("Two Score Rate:" + scoreRate.TwoScoreRate );
			scoreRate.TwoScoreRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.TwoScoreRate , 0, 100));
			GUILayout.Label("Two Score Rate Deviation:" + scoreRate.TwoScoreRateDeviation );
			scoreRate.TwoScoreRateDeviation  = GUILayout.HorizontalSlider(scoreRate.TwoScoreRateDeviation , 0.0f, 1.0f);
			GUILayout.Label("Three Score Rate:" + scoreRate.ThreeScoreRate );
			scoreRate.ThreeScoreRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.ThreeScoreRate , 0, 100));
			GUILayout.Label("Three Score Rate Deviation:" + scoreRate.ThreeScoreRateDeviation );
			scoreRate.ThreeScoreRateDeviation  = GUILayout.HorizontalSlider(scoreRate.ThreeScoreRateDeviation , 0.0f, 1.0f);
			GUILayout.Label("DownHand Score Rate:" + scoreRate.DownHandScoreRate );
			scoreRate.DownHandScoreRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.DownHandScoreRate , 0, 100));
			GUILayout.Label("DownHand Swish Rate:" + scoreRate.DownHandSwishRate );
			scoreRate.DownHandSwishRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.DownHandSwishRate , 0, 100));
			GUILayout.Label("DownHand AirBall Rate:" + scoreRate.DownHandAirBallRate );
			scoreRate.DownHandAirBallRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.DownHandAirBallRate , 0, 100));
			GUILayout.Label("UpHand Score Rate:" + scoreRate.UpHandScoreRate );
			scoreRate.UpHandScoreRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.UpHandScoreRate , 0, 100));
			GUILayout.Label("UpHand Swish Rate:" + scoreRate.UpHandSwishRate );
			scoreRate.UpHandSwishRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.UpHandSwishRate , 0, 100));
			GUILayout.Label("UpHand AirBall Rate:" + scoreRate.UpHandAirBallRate );
			scoreRate.UpHandAirBallRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.UpHandAirBallRate , 0, 100));
			GUILayout.Label("Normal Score Rate:" + scoreRate.NormalScoreRate );
			scoreRate.NormalScoreRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.NormalScoreRate , 0, 100));
			GUILayout.Label("Normal Swish Rate:" + scoreRate.NormalSwishRate );
			scoreRate.NormalSwishRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.NormalSwishRate , 0, 100));
			GUILayout.Label("Normal AirBall Rate:" + scoreRate.NormalAirBallRate );
			scoreRate.NormalAirBallRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.NormalAirBallRate , 0, 100));
			GUILayout.Label("NearShot Score Rate:" + scoreRate.NearShotScoreRate );
			scoreRate.NearShotScoreRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.NearShotScoreRate , 0, 100));
			GUILayout.Label("NearShot Swish Rate:" + scoreRate.NearShotSwishRate );
			scoreRate.NearShotSwishRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.NearShotSwishRate , 0, 100));
			GUILayout.Label("NearShot AirBall Rate:" + scoreRate.NearShotAirBallRate );
			scoreRate.NearShotAirBallRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.NearShotAirBallRate , 0, 100));
			GUILayout.Label("LayUp Score Rate:" + scoreRate.LayUpScoreRate );
			scoreRate.LayUpScoreRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.LayUpScoreRate , 0, 100));
			GUILayout.Label("LayUp Swish Rate:" + scoreRate.LayUpSwishRate );
			scoreRate.LayUpSwishRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.LayUpSwishRate , 0, 100));
			GUILayout.Label("LayUp AirBall Rate:" + scoreRate.LayUpAirBallRate );
			scoreRate.LayUpAirBallRate  = Mathf.RoundToInt(GUILayout.HorizontalSlider((float)scoreRate.LayUpAirBallRate , 0, 100));


			GUI.backgroundColor = Color.red;
			if(isChange) 
				GUILayout.Label("Change Success");
			GUI.backgroundColor = Color.white;
			if(GUILayout.Button("Change")) {
				isChange = true;
				p.ScoreRate = scoreRate;
			}
		} else {
			GUILayout.Label("Please Choose One Player!!");
		}
	}
}
