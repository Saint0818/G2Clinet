using System.Collections;
using UnityEngine;

/// <summary>
/// Plays the specified sound.
/// </summary>

public class UISound : MonoBehaviour
{
    public SoundType type = SoundType.None;

    void OnPress(bool isPressed)
    {
        if(isPressed){
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
            AudioMgr.Get.PlaySound(SoundType.SD_Click_Btn);
    }
}
