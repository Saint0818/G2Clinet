using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Audio;

public class AudioMgr : KnightSingleton<AudioMgr>
{
	public AudioMixerSnapshot Nomal;
	public AudioMixerSnapshot Paused;


	public void InitCom()
	{

	}

	public void PauseGame()
	{
		if (Time.timeScale == 0) {
			Paused.TransitionTo(.01f);	
		}
		else
			Nomal.TransitionTo(.01f);
	}
}

