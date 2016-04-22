﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using GameEnum;

public enum SoundPlayMode
{
    OnEnable,
    OnClick
}

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

public class AudioMgr : MonoBehaviour
{
    private static AudioMgr instance;
    private string path = "Audio/Prefab/";
    public string CurrentMusic = string.Empty;
    public AudioMixer MasterMix;
    public AudioMixerSnapshot SplashSnapshot;
    public AudioMixerSnapshot PausedSnapshot;
    public AudioMixerSnapshot LobbySnapshot;
    public AudioMixerSnapshot CreateSnapshot;
    public AudioMixerSnapshot InGameSnapshot;
    public AudioMixerSnapshot GameResultSnapshot;

	private AudioMixerSnapshot currentSnapshot;
    private float snapshotTransitionTime = 2f;

    private Dictionary<string, AudioSource> DAudios = new Dictionary<string, AudioSource>();
    public bool init = false;

    public static AudioMgr Get {
        get {
            return instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = gameObject.GetComponent<AudioMgr>();
        if(SplashSnapshot != null)
            ChangeSnapshot(ref SplashSnapshot);
    }

    public void PlayMusic(EMusicType type)
    {
        string name = type.ToString();

        if (CurrentMusic != name)
        {
            switch (type)
            {
                case EMusicType.MU_Create:
                    ChangeSnapshot(ref CreateSnapshot);
                        break;

                case EMusicType.MU_ThemeSong:
                    ChangeSnapshot(ref LobbySnapshot);
                    break;

                case EMusicType.MU_BattleNormal:
                case EMusicType.MU_BattlePVP:
                    ChangeSnapshot(ref InGameSnapshot);
                    break;

            }
        }
       
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

        if (CurrentMusic != name && loadAudio(name))
        {
            DAudios[name].Play();
            CurrentMusic = name;
        }
    }

    private bool loadAudio(string name)
    {
        if (!DAudios.ContainsKey(name))
        {
            Object temp = Resources.Load(path + name);
            if (temp)
            {
                GameObject obj = Instantiate(temp) as GameObject;
                obj.transform.parent = gameObject.transform;

                obj.name = name;
                AudioSource clone = obj.GetComponent<AudioSource>();
                DAudios.Add(name, clone);
                return true;
            }
            else
            {
                Debug.LogError("Can not found Audio : " + name);
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    private void ChangeSnapshot(ref AudioMixerSnapshot next)
    {
        if (next != null){
            next.TransitionTo(snapshotTransitionTime);
        }
    }

    public void PauseGame(bool isparused)
    {
        if (isparused)
        {
            if (PausedSnapshot)
                PausedSnapshot.TransitionTo(0.01f);     
        }
        else
        {
            if (InGameSnapshot)
                InGameSnapshot.TransitionTo(0.01f);
        }
    }

    public void PlaySound(SoundType type)
    {
        string soundName = type.ToString();

        if(type == SoundType.SD_ResultWin || type == SoundType.SD_ResultLose)
            GameResultSnapshot.TransitionTo(0.01f); 
        
        PlaySound(soundName);
    }

    public void PlaySound(string name)
    {
        if (loadAudio(name))
        {
            DAudios[name].Play();
        }
    }

    public void MusicOn(bool flag)
    {
        GameData.Setting.Music = flag;
        if (flag)
        {
            setVol(AudioValuetype.musicVol, 0);
            PlayerPrefs.SetInt(ESave.MusicOn.ToString(), 1);
        }
        else
        {
            setVol(AudioValuetype.musicVol, -80);
            PlayerPrefs.SetInt(ESave.MusicOn.ToString(), 0);
        }
        PlayerPrefs.Save();
    }

    public void SoundOn(bool flag)
    {
        GameData.Setting.Sound = flag;

        if (flag)
        {
            setVol(AudioValuetype.soundVol, -6);
            PlayerPrefs.SetInt(ESave.SoundOn.ToString(), 1);
        }
        else
        {
            setVol(AudioValuetype.soundVol, -80);
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

    private void setVol(AudioValuetype type, float value)
    {
        if (MasterMix)
            MasterMix.SetFloat(type.ToString(), value);			
    }
}

