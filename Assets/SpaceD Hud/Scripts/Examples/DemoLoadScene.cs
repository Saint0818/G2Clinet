using UnityEngine;
using System.Collections;

public class DemoLoadScene : MonoBehaviour {

	public string sceneName = "";

	public void LoadScene()
	{
		if (!string.IsNullOrEmpty(this.sceneName))
			Application.LoadLevel(this.sceneName);
	}
}
