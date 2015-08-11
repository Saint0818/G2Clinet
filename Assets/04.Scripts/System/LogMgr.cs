using UnityEngine;
using System.Collections;
using System.Text;

public class LogMgr : KnightSingleton<LogMgr> {

	public bool IsShowUI = false;
	private StringBuilder[] playerAni;

	void Awake()
	{
		playerAni = new StringBuilder[6];
		for (int i = 0; i < playerAni.Length; i++)
			playerAni [i] = new StringBuilder ();
	}

	public void Log(string str)
	{
		Debug.Log (str);
	}

	public void LogWarning(string str)
	{
		Debug.LogWarning (str);
	}

	public void LogError(string str)
	{
		Debug.LogError (str);
	}

	public void AddAnimationLog(int playeIndex, string str)
	{
		if (playerAni [playeIndex]!= null)
			playerAni [playeIndex].AppendLine (str);
	}

	public void AnimationError(int index, string str)
	{
		IsShowUI = true;
		Log (playerAni [index].ToString ());
		LogError (str);
	}

	void OnGUI()
	{
		if(IsShowUI)
			for(int i = 0; i < playerAni.Length; i ++)
				if(GUI.Button(new Rect(10,10 + i * 50,100,50),"Player" + i.ToString()))
					Log (playerAni [i].ToString ());
	}
	
}
