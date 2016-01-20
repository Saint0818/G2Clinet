using System;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIGameLobbyMain : MonoBehaviour
{
    public event Action BackListener;
    public event Action MainStageListener;
    public event Action PvpListener;
    public event Action InstanceListener;

    public UIButton BackButton;
    public UIButton MainStageButton;
    public UIButton PvpButton;
    public UIButton InstanceButton;

    [UsedImplicitly]
    private void Awake()
    {
        BackButton.onClick.Add(new EventDelegate(OnBackClick));
        MainStageButton.onClick.Add(new EventDelegate(OnMainStageClick));
        PvpButton.onClick.Add(new EventDelegate(OnPvpClick));
        InstanceButton.onClick.Add(new EventDelegate(OnInstanceClick));
    }

    public void OnBackClick()
    {
        if(BackListener != null)
            BackListener();
    }

    public void OnMainStageClick()
    {
        if(MainStageListener != null)
            MainStageListener();
    }

    public void OnPvpClick()
    {
        if(PvpListener != null)
            PvpListener();
    }

    public void OnInstanceClick()
    {
        if(InstanceListener != null)
            InstanceListener();
    }
}