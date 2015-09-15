using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIConfirmDialog : MonoBehaviour
{
    public delegate void Action();
    public event Action OnYesListener;
    public event Action OnCancelClickListener;

    public GameObject Window;

    [UsedImplicitly]
    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        Window.SetActive(true);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void OnYesClick()
    {
        Hide();

        if(OnYesListener != null)
            OnYesListener();
    }

    public void OnCancelClick()
    {
        Hide();

        if(OnCancelClickListener != null)
            OnCancelClickListener();
    }
}