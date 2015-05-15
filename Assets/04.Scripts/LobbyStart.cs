using UnityEngine;
using System.Collections;

public class LobbyStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UIHint.Get.ShowHint("Enter lobby.", Color.blue);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (GUI.Button(new Rect(0, 0, 100, 100), "Load Game Play"))
			Application.LoadLevel("PlayerControl");
	}

}
