using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIGameLobbyImpl : MonoBehaviour
{
    public event CommonDelegateMethods.Action BackListener;
    public event CommonDelegateMethods.Action MainListener;

    [UsedImplicitly]
    private void Awake()
    {
    }

    public void OnBackClick()
    {
        if(BackListener != null)
            BackListener();
    }

    public void OnMainClick()
    {
        if(MainListener != null)
            MainListener();
    }
}