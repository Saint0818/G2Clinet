using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public enum SoundType
{
	Audio,
	Dunk,
	Net,
	GameEnd
}

public class AudioMgr : KnightSingleton<AudioMgr>
{
	public AudioMixer MasterMix;
	public AudioMixerSnapshot Nomal;
	public AudioMixerSnapshot Paused;
	public AudioMixerSnapshot StartST;
	public bool IsMusicOn = false;
	
	private Dictionary<string, AudioSource> DAudios = new Dictionary<string, AudioSource> ();

	void Awake()
	{
		AudioSource[] loads = gameObject.transform.GetComponentsInChildren<AudioSource> ();
		if (loads.Length > 0) {
			for(int i = 0; i < loads.Length; i++)
			{
				if(!DAudios.ContainsKey(loads[i].name))
					DAudios.Add(loads[i].name, loads[i]);
			}
		}

		if (!PlayerPrefs.HasKey ("MusicOn")) {
			PlayerPrefs.SetInt ("MusicOn", 1);
			PlayerPrefs.Save ();
		}

		//KnightZhConverter.Get.Test ();
	}

	public void StartGame()
	{
		MusicOn (PlayerPrefs.GetInt ("MusicOn") == 1 ? true : false);

		AudioMixerSnapshot[] s = new AudioMixerSnapshot[1]{StartST};
		float[] f = new float[1]{1};
		MasterMix.TransitionToSnapshots (s, f, 1);

		if(DAudios.ContainsKey("Audio"))
			DAudios["Audio"].Play();
		
	}

	public void PauseGame()
	{
		if (Time.timeScale == 0) {
			if (Paused)
				Paused.TransitionTo(.01f);	
		}
		else
			if (StartST)
				StartST.TransitionTo(.01f);
	}

	public void PlaySound(SoundType type)
	{
		string soundName = type.ToString ();
		PlaySound(soundName);
	}

	public void PlaySound(string name)
	{
		if (DAudios.ContainsKey (name))
			DAudios [name].Play ();
	}

	public void MusicOn(bool flag)
	{
		IsMusicOn = flag;

		if (IsMusicOn) {
			PlayerPrefs.SetInt ("MusicOn", 1);
			if (MasterMix)
				MasterMix.ClearFloat("masterVol");
		}
		else{
			PlayerPrefs.SetInt ("MusicOn", 0);
			if (MasterMix)
				MasterMix.SetFloat ("masterVol", -80);
		}
		PlayerPrefs.Save ();
	}

}

