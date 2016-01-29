﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using GameEnum;

public enum SoundType
{
    None,
    SD_Catch,
    SD_Dribble,
    SD_DunkNormal,
    SD_DoubleClick,
    SD_DCPerfect,
    SD_DCWeak,
    SD_ResultWin,
    SD_ShootNormal,
    //以下未接
    SD_ActiveLaunch,
    SD_BasketballAction0,
    SD_BasketballAction1,
    SD_BasketballAction2,
    SD_BattleStart_Btn,
    SD_Block,
    SD_Buy,
    SD_Cancel_Btn,
    SD_Check_Btn,
    SD_Click_Btn,
    SD_Compose,
    SD_DunkBreak,
    SD_Fall,
    SD_Instant,
    SD_Line,
    SD_LobbyCamara,
    SD_OnFire,
    SD_Prohibit,
    SD_Punch,
    SD_Rebound,
    SD_ResultCount,
    SD_ResultLose,
    SD_SelectDown,
    SD_Sell,
    SD_Steal,
    SD_Transition,
    SD_Unlock,
    SD_UpgradeItems,
    SD_UpgradePlayer,
    SD_Warning0,
    SD_Warning1
}

public enum EMusicType
{
    None,
    MU_Create,
    MU_BattleNormal,
    MU_BattlePVP,
    MU_ThemeSong
}

public class AudioMgr : KnightSingleton<AudioMgr>
{
    public string CurrentMusic = string.Empty;
    public AudioMixer MasterMix;
    public AudioMixerSnapshot Nomal;
    public AudioMixerSnapshot Paused;
    public AudioMixerSnapshot StartST;
    private Dictionary<string, AudioSource> DAudios = new Dictionary<string, AudioSource>();
    public bool init = false;

    void Awake()
    {
        AudioSource[] loads = gameObject.transform.GetComponentsInChildren<AudioSource>();
        if (loads.Length > 0)
        {
            for (int i = 0; i < loads.Length; i++)
            {
                if (!DAudios.ContainsKey(loads[i].name))
                    DAudios.Add(loads[i].name, loads[i]);
            }
        }
    }

    public void StartGame()
    {
//		if (GameData.Setting.Music) {
//			AudioMixerSnapshot[] s = new AudioMixerSnapshot[1]{ StartST };
//			float[] f = new float[1]{ 1 };
//			MasterMix.TransitionToSnapshots (s, f, 1);
//			PlayMusic (EMusicType.MU_ThemeSong);
//		}
    }

    public void PlayMusic(EMusicType type)
    {
        string name = type.ToString();
        PlayMusic(name);
    }

    public void PlayMusic(string name)
    {
        if (!init)
        {
            MusicOn(GameData.Setting.Music);
            SoundOn(GameData.Setting.Sound);
            init = true;
        }

        if (CurrentMusic != name && DAudios.ContainsKey(name))
        {

            if (CurrentMusic != string.Empty && DAudios.ContainsKey(CurrentMusic))
            {
                DAudios[CurrentMusic].Stop();
            }

			if (CurrentMusic == EMusicType.MU_BattleNormal.ToString() || CurrentMusic == EMusicType.MU_BattlePVP.ToString())
			{
				AudioMixerSnapshot[] s = new AudioMixerSnapshot[1]{ StartST };
				float[] f = new float[1]{ 1 };
				MasterMix.TransitionToSnapshots(s, f, 1);		
			}
			
            DAudios[name].Play();
            CurrentMusic = name;
        }
    }

    public void PauseGame()
    {
        if (Time.timeScale == 0)
        {
            if (Paused)
                Paused.TransitionTo(.01f);	
        }
        else if (StartST)
            StartST.TransitionTo(.01f);
    }

    public void PlaySound(SoundType type)
    {
        string soundName = type.ToString();
        PlaySound(soundName);
    }

    public void PlaySound(string name)
    {
        if (DAudios.ContainsKey(name))
            DAudios[name].Play();
        else
            Debug.LogError("Name : " + name);			
    }

    public void MusicOn(bool flag)
    {
        GameData.Setting.Music = flag;
        if (flag)
        {
            SetVol(AudioValuetype.musicVol, -6);
            PlayerPrefs.SetInt(ESave.MusicOn.ToString(), 1);
        }
        else
        {
            SetVol(AudioValuetype.musicVol, -80);
            PlayerPrefs.SetInt(ESave.MusicOn.ToString(), 0);
        }
        PlayerPrefs.Save();
    }

    public void SoundOn(bool flag)
    {
        GameData.Setting.Sound = flag;

        if (flag)
        {
            SetVol(AudioValuetype.soundVol, -6);
            PlayerPrefs.SetInt(ESave.SoundOn.ToString(), 1);
        }
        else
        {
            SetVol(AudioValuetype.soundVol, -80);
            PlayerPrefs.SetInt(ESave.SoundOn.ToString(), 0);
        }
		
        NGUITools.soundVolume = flag ? 1 : 0;
        PlayerPrefs.Save();
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
            MasterMix.SetFloat(type.ToString(), value);			
        }
    }

    void OnDestory()
    {
        Debug.LogError("Fuck you ");
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

