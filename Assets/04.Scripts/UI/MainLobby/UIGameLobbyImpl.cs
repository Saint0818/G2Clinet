using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIGameLobbyImpl : MonoBehaviour
{
    public event CommonDelegateMethods.Action BackListener;

    [UsedImplicitly]
    private void Awake()
    {
    }

    public void OnBackClick()
    {
        if(BackListener != null)
            BackListener();
    }
}