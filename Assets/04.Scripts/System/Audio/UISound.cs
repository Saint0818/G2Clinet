using System.Collections;
using UnityEngine;

/// <summary>
/// Plays the specified sound.
/// </summary>

public class UISound : MonoBehaviour
{
    public SoundType type = SoundType.None;
    public SoundPlayMode mode = SoundPlayMode.OnClick;

    void OnEnable()
    {
        if (mode == SoundPlayMode.OnEnable)
        {
           Play();    
        }
    }

    void OnPress(bool isPressed)
    {
        if (mode == SoundPlayMode.OnClick && isPressed)
        {
           Play(); 
        }
    }
				
    public void Play()
    {
        if (type == SoundType.None)
        {
            AudioMgr.Get.PlaySound(SoundType.SD_Click_Btn);
        }
        else
			AudioMgr.Get.PlaySound(type);
    }
}
