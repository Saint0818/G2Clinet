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
	public bool IsMute = false;
	
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
	}

	public void PauseGame()
	{
		if (Time.timeScale == 0) {
			Paused.TransitionTo(.01f);	
		}
		else
			Nomal.TransitionTo(.01f);
	}

	public void PlaySound(SoundType type)
	{
		string soundName = type.ToString ();
		if (DAudios.ContainsKey (soundName))
			DAudios [soundName].Play ();
	}

	public void Mute(bool flag)
	{
		IsMute = flag;

		if (IsMute)
			MasterMix.SetFloat("masterVol", -80);
		else
			MasterMix.ClearFloat("masterVol");
	}

}

