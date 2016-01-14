using System;
using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIGameLobbyImpl : MonoBehaviour
{
    public event Action BackListener;
    public event Action MainStageListener;
    public event Action PvpListener;

    public UIButton BackButton;
    public UIButton MainStageButton;
    public UIButton PvpButton;

    [UsedImplicitly]
    private void Awake()
    {
        BackButton.onClick.Add(new EventDelegate(OnBackClick));
        MainStageButton.onClick.Add(new EventDelegate(OnMainStageClick));
        PvpButton.onClick.Add(new EventDelegate(OnPvpClick));
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
}