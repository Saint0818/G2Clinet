using System;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIGameLobbyMain : MonoBehaviour
{
    public event Action BackListener;
    public event Action MainStageListener;
    public event Action PVPListener;
    public event Action InstanceListener;

    public UIButton BackButton;
    public UIButton MainStageButton;
    public UIUnlockButton PvpUnlockButton;
    public UIUnlockButton InstanceUnlockButton;
	public UISprite Reddot;

    [UsedImplicitly]
    private void Awake()
    {
        BackButton.onClick.Add(new EventDelegate(onBackClick));
        MainStageButton.onClick.Add(new EventDelegate(onMainStageClick));
        PvpUnlockButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(onPvpClick));
        InstanceUnlockButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(onInstanceClick));
    }

    private void onBackClick()
    {
        if(BackListener != null)
            BackListener();
    }

    private void onMainStageClick()
    {
        if(MainStageListener != null)
            MainStageListener();
    }

    private void onPvpClick()
    {
        if(PVPListener != null)
            PVPListener();
    }

    private void onInstanceClick()
    {
        if(InstanceListener != null)
            InstanceListener();
    }

	public bool ReddotEnable
	{
		set{ Reddot.enabled = value;}
	}
}