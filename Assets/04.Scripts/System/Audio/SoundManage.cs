using UnityEngine;

public class SoundManage : MonoBehaviour
{
	private static SoundManage instance = null;
	private static AudioSource audioSource;

	public static void Init() {
		if (instance == null) {
			GameObject go = new GameObject ("SoundManage");
			audioSource = go.AddComponent<AudioSource>();
			instance = go.AddComponent<SoundManage> ();
		}
	}

	public static void PlayBGM(int index) {
		if (audioSource) {
//			if(TeamManager.Music){
//				if (index >= 0 && index < BGMs.Length && BGMIndex != index){
//					AudioClip clip = (AudioClip)Resources.Load("Sounds/" + BGMs[index], typeof(AudioClip));
//					if (clip != null) {
//						BGMIndex = index;
//						audioSource.Stop();
//						audioSource.clip = clip;
//						audioSource.volume = BGMVolume / 2;
//						audioSource.loop = true;
//						audioSource.Play();
//					}
//				}
//			}
		}
	}

	public static void ControlMusic(){
//		if (audioSource) {
//			if(TeamManager.Music){
//				if(BGMIndex < 0)
//					BGMIndex = 1;
//
//				if (BGMIndex >= 0 && BGMIndex < BGMs.Length){
//					AudioClip clip = (AudioClip)Resources.Load("Sounds/" + BGMs[BGMIndex], typeof(AudioClip));
//					if (clip != null) {
//						audioSource.Stop();
//						audioSource.clip = clip;
//						audioSource.volume = BGMVolume / 2;
//						audioSource.loop = true;
//						audioSource.Play();
//					}
//				}
//			}else
//				audioSource.Stop();
//		}
	}

	public static void PlaySound(int index) {
		if (audioSource) {
//			if(TeamManager.Sound){
//				if (index >= 0 && index < Sounds.Length){
//					AudioClip clip = (AudioClip)Resources.Load("Sounds/" + Sounds[index], typeof(AudioClip));
//					if (clip != null) 
//						audioSource.PlayOneShot(clip, BGMVolume * 1.5f);
//				}
//			}
		}
	}
}
