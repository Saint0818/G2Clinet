using UnityEngine;
using UnityEditor;
using System.Collections;

public class GEMainMenu: EditorWindow {
	private const string menuRoot = "GameEditor/";

	[MenuItem(menuRoot + "1.AI Move", false, 1)]
	private static void OnAIMove() {
		GEAIMove.GetWindow<GEAIMove>(true, "1.AI Move").SetStyle();
	}

	[MenuItem (menuRoot + "2.Ball Position", false, 2)]
	private static void OnBallPosition() {
		GEBasketPosition.GetWindow<GEBasketPosition>(true, "2.Ball Position").SetStyle();
	}

	[MenuItem (menuRoot + "3.Player Score Rate", false, 3)]
	private static void OnPlayerScoreRate() {
		GEPlayerScoreRate.GetWindow<GEPlayerScoreRate>(true, "3.Player Score Rate").SetStyle();
	}

	[MenuItem (menuRoot + "4.Stage Tutorial", false, 4)]
	private static void OnGamePlayTutorial() {
		GEStageTutorial.GetWindow<GEStageTutorial>(true, "4.Stage Tutorial").SetStyle();
	}

	[MenuItem (menuRoot + "5.UI Toturial", false, 5)]
	private static void OnToturial() {
		GEUIToturial.GetWindow<GEUIToturial>(true, "5.UI Toturial").SetStyle();
	}
	
	[MenuItem (menuRoot + "6.Avatar", false, 6)]
	private static void OnAvatar() {
		GEAvatar.GetWindow<GEAvatar>(true, "6.Avatar").SetStyle();
	}
	
	[MenuItem (menuRoot + "9.GM Tool", false, 9)]
	private static void OnGM() {
		GEGMTool.GetWindow<GEGMTool>(true, "9.GM Tool").SetStyle();
    }

	[MenuItem(menuRoot + "10.Define Wizard", false, 10)]
	private static void OnDefineWizard() {
		GEGlobalDefinesWizard.CreateWizardFromMenu();
    }
	
	[MenuItem (menuRoot + "11.Check Tool", false, 11)]
	private static void OnDataChecker() {
		GEDataChecker.GetWindow<GEDataChecker>(true, "11.Check Tool").SetStyle();
	}

	[MenuItem (menuRoot + "12.Build Tool", false, 12)]
	private static void OnBuildTool() {
		GEBuildTool.GetWindow<GEBuildTool>(true, "12.Build Tool").SetStyle();
	}
	
	[MenuItem (menuRoot + "13.Player Passive Rate", false, 13)]
	private static void OnPlayerPassiveRate() {
		GEPlayerPassiveRate.GetWindow<GEPlayerPassiveRate>(true, "13.Player Passive Rate").SetStyle();
	}
}
