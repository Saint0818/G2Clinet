﻿using System;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 這是一個裝備道具. 會顯示裝備的圖示, 名稱, 數量, 鑲嵌資訊.(會用在裝備介面的左邊和中間)
/// </summary>
/// 使用方法:
/// <list type="number">
/// <item> Call Set() 設定道具資訊. </item>
/// <item> OnClickListener 註冊點擊事件. </item>
/// </list>
public class UIEquipItem : MonoBehaviour
{
    public event Action OnClickListener;

    public UISprite Icon;
    public GameObject EmptyIcon;

    public UILabel Text;
    public UILabel Amount;
    public GameObject RedPoint;
    public GameObject[] InlaySlots;
    public GameObject[] Inlays;

	private Transform tQuality;
	private UISprite qualityBG;

    [UsedImplicitly]
    private void Awake()
    {
		if(tQuality == null)
			tQuality = GameFunction.FindQualityBG(transform);
		if(tQuality != null)
			qualityBG = tQuality.GetComponent<UISprite>();
        // 暫時關閉.
        Text.gameObject.SetActive(false);
        hideAllInlay();
    }

    private void hideAllInlay()
    {
        foreach(GameObject inlay in Inlays)
        {
            inlay.SetActive(false);
        }

        foreach(GameObject slot in InlaySlots)
        {
            slot.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="showRedPoint"> 要不要顯示紅點. </param>
    /// <param name="showNum"> true: 強制顯示數量. </param>
    public void Set(UIValueItemData data, bool showRedPoint, bool showNum)
    {
        gameObject.SetActive(true);
        
        updateIcon(data);

        Text.text = data.Name;

        RedPoint.SetActive(showRedPoint);

        Amount.gameObject.SetActive(showNum);
        Amount.text = data.Num.ToString();

		if(qualityBG != null) {
			qualityBG.color = TextConst.ColorBG(data.Quality);
		}

        updateInlay(data.Inlay);
    }

    public Vector3 GetInlayPosition(int index)
    {
        return index < Inlays.Length ? Inlays[index].transform.position : Vector3.zero;
    }

    private void updateIcon(UIValueItemData data)
    {
        if(data.IsValid())
        {
            EmptyIcon.SetActive(false);
            Icon.gameObject.SetActive(true);
            Icon.atlas = data.Atlas;
            Icon.spriteName = data.Icon;

            GetComponent<UISprite>().spriteName = data.Frame;
            // 我認為這是 NGUI 的問題, 其實我改 UISprite 後, UIButton 的 normal 也應該要改才對.
            GetComponent<UIButton>().normalSprite = data.Frame;
        }
        else
        {
            EmptyIcon.SetActive(true);
            Icon.gameObject.SetActive(false);

            GetComponent<UISprite>().spriteName = string.Empty;
            // 我認為這是 NGUI 的問題, 其實我改 UISprite 後, UIButton 的 normal 也應該要改才對.
            GetComponent<UIButton>().normalSprite = string.Empty;
        }
    }

    private void updateInlay(bool[] inlayStatus)
    {
        hideAllInlay();

        for(var i = 0; i < inlayStatus.Length; i++)
        {
            InlaySlots[i].SetActive(true);
            Inlays[i].SetActive(inlayStatus[i]);
        }
    }

    /// <summary>
    /// 內部使用.
    /// </summary>
    public void NotifyClick()
    {
        if(OnClickListener != null)
            OnClickListener();
    }
}