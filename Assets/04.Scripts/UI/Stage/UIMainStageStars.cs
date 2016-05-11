using UnityEngine;

/// <summary>
/// 主線關卡中, 負責小關卡星等的顯示.
/// </summary>
public class UIMainStageStars : MonoBehaviour
{
    public GameObject VisibleObj;
    public GameObject[] Stars;

    public void Show(int starNum)
    {
        VisibleObj.SetActive(true);

        clear();

        for(var i = 0; i < Stars.Length; i++)
        {
            if(i+1 <= starNum)
                Stars[i].SetActive(true);
        } 
    }

    private void clear()
    {
        foreach(GameObject star in Stars)
        {
            star.SetActive(false);
        }
    }

    public void Hide()
    {
        VisibleObj.SetActive(false);
    }
}