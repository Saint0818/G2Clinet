using JetBrains.Annotations;
using UnityEngine;

[DisallowMultipleComponent]
public class UIConfirmDialog : MonoBehaviour
{
    public delegate void Action();
    public delegate void Action2(object extraInfo);
    public event Action2 OnYesListener;
    public event Action OnCancelClickListener;

    public GameObject Window;

    private object mExtraInfo;

    [UsedImplicitly]
    private void Awake()
    {
        Hide();
    }

    public void Show(object extraInfo = null)
    {
        Window.SetActive(true);
        mExtraInfo = extraInfo;
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

    public void OnYesClick()
    {
        Hide();

        if(OnYesListener != null)
            OnYesListener(mExtraInfo);
    }

    public void OnCancelClick()
    {
        Hide();

        if(OnCancelClickListener != null)
            OnCancelClickListener();
    }
}