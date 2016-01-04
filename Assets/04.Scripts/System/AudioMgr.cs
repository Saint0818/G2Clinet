using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using GameEnum;

public enum SoundType
{
	SD_Dunk,
	SD_Net,
	SD_GameEnd,
	SD_dribble,
	SD_CatchBall,
	SD_CatchMorale
}

public enum EMusicType
{
	None,
	MU_select,
	MU_game0,
	MU_game1
}

public class AudioMgr : KnightSingleton<AudioMgr>
{
	public EMusicType CurrentMusic = EMusicType.None;
	public AudioMixer MasterMix;
	public AudioMixerSnapshot Nomal;
	public AudioMixerSnapshot Paused;
	public AudioMixerSnapshot StartST;
	private Dictionary<string, AudioSource> DAudios = new Dictionary<string, AudioSource> ();
	public bool init = false;

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

	public void StartGame()
	{
		if (GameData.Setting.Music) {
			AudioMixerSnapshot[] s = new AudioMixerSnapshot[1]{ StartST };
			float[] f = new float[1]{ 1 };
			MasterMix.TransitionToSnapshots (s, f, 1);
			PlayMusic (EMusicType.MU_select);
		}
	}

	public void PlayMusic(EMusicType type)
	{
		string name = type.ToString ();
		if (DAudios.ContainsKey (name)) {

			if(CurrentMusic != EMusicType.None && DAudios.ContainsKey (CurrentMusic.ToString()))
				DAudios[CurrentMusic.ToString()].Stop();

			DAudios [name].Play ();
			CurrentMusic = type;
		}
		
		if (!init) {
			MusicOn(GameData.Setting.Music);
			SoundOn(GameData.Setting.Sound);
			init = true;
		}
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
		GameData.Setting.Music = flag;
		if (flag) {
			SetVol(AudioValuetype.musicVol, -6);
			PlayerPrefs.SetInt (ESave.MusicOn.ToString (), 1);
		} else {
			SetVol(AudioValuetype.musicVol, -80);
			PlayerPrefs.SetInt (ESave.MusicOn.ToString (), 0);
		}
		PlayerPrefs.Save ();
	}

	public void SoundOn(bool flag)
	{
		GameData.Setting.Sound = flag;

		if (flag) {
			SetVol(AudioValuetype.soundVol, -6);
			PlayerPrefs.SetInt (ESave.SoundOn.ToString (), 1);
		}
		else{
			SetVol(AudioValuetype.soundVol, -80);
			PlayerPrefs.SetInt (ESave.SoundOn.ToString(), 0);
		}
		
		NGUITools.soundVolume = flag? 1 : 0;
		PlayerPrefs.Save ();
	}

	private enum AudioValuetype
	{
		musicVol,
		soundVol
	}

	private void SetVol(AudioValuetype type, float value)
	{
		if (MasterMix) 
		{
			MasterMix.SetFloat (type.ToString(), value);			
		}
	}

//	private void SetMute(AudioValuetype type, bool isOn)
//	{
//		string typeName = type.ToString();
//		if (MasterMix) {
//			if(isOn)
//				MasterMix.ClearFloat(typeName);
//			else
//				MasterMix.SetFloat (typeName, -80);
//		}
//	}
}

